﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SZGUIFeleves.Models;
using SZGUIFeleves.Models.DrawableObjects;
using SZGUIFeleves.ViewModels;

namespace SZGUIFeleves.Logic
{
    public delegate void DrawDelegate(int WindowSizeWidth, int WindowSizeHeight);
    public delegate void ChangeState(GameStates state);

    public enum ButtonKey
    {
        W, A, S, D,
        Up, Down, Left, Right,
        Space, C, LeftCtrl,
        Q, E,
        MouseLeft, MouseRight,
        Escape
    }

    public class GameLogic : IGameModel, IGameControl
    {
        #region App Constants
        private const int FPSTarget = 100;
        #endregion

        #region Draw Variables
        /// <summary>
        /// At the end of the main loop, add all Objects to these lists
        /// to pass the objects to the renderer
        /// <para>WorldSpace - IsAffectedByCamera is true</para>
        /// <para>ScreenSpace - IsAffectedByCamera is false</para>
        /// </summary>
        public List<DrawableObject> ObjectsToDisplayWorldSpace { get; set; }
        public List<DrawableObject> ObjectsToDisplayScreenSpace { get; set; }
        #endregion

        #region App Variables
        public Vec2d WindowSize { get; set; }

        public event DrawDelegate DrawEvent;
        public event ChangeState ChangeState;
        public GameStates CurrentState { get; set; }
        public DispatcherTimer MainLoopTimer { get; set; }
        private MovementLogic MovementLogic { get; set; }
        private DateTime ElapsedTime { get; set; }
        private double Elapsed { get; set; }
        private double CycleMilliseconds { get; set; }
        private List<double> RecentFPS { get; set; }
        public int FPS
        {
            get
            {
                if (RecentFPS is null || RecentFPS.Count() == 0)
                    return 0;
                else
                {
                    return (int)Math.Floor(RecentFPS.Average());
                }
            }
        }
        private Dictionary<ButtonKey, bool> ButtonFlags { get; set; }
        public Camera Camera { get; set; }
        public List<DrawableObject> MovingBackgrounds { get; set; }
        private double MovingBackgroundSpeed { get; set; }

        private List<Emitter> Emitters { get; set; }
        private Emitter BloodEmitter { get; set; }
        private Emitter CheckpointEmitter { get; set; }

        public Scene CurrentScene { get; set; }
        public double BestTime { get; set; }
        public Vec2d MousePosition { get; set; }

        public ButtonKey LastPressedDirection { get; set; }

        public Stopwatch SceneTimer { get; set; }

        public Random rnd { get; set; }
        public double TrapColliderMinus = 2.5f;
        public Vec2d PlayerPosition { get; set; }

        #endregion

        #region Checkpoint
        private Checkpoint LastCheckpoint { get; set; }
        private int MaxLives { get; set; }
        private int Lives { get; set; }
        private List<Rectangle> LivesUI { get; set; }
        #endregion

        #region Lighting Variables
        private const int shadowPasses = 3;
        private const int shadowIntensity = 10;
        private const int lightBlendingAlpha = 40;
        private Color LightColor { get; set; }
        public bool IsThereShadow { get; set; }
        #endregion

        #region Sounds and Music
        private System.Windows.Media.MediaPlayer RunSound { get; set; }
        private System.Windows.Media.MediaPlayer JumpSound { get; set; }
        private System.Windows.Media.MediaPlayer DeathSound { get; set; }
        private System.Windows.Media.MediaPlayer BackgroundSound { get; set; }
        private bool IsRunning { get; set; }
        #endregion

        public GameLogic(int WindowSizeWidth, int WindowSizeHeight)
        {
            WindowSize = new Vec2d(WindowSizeWidth, WindowSizeHeight);
            MovementLogic = new MovementLogic();
            CurrentState = GameStates.Pause;

            rnd = new Random((int)DateTime.Now.Ticks);

            // Calculating frame interval with the given FPS target
            CycleMilliseconds = 1.0f / FPSTarget * 1000.0f;
            RecentFPS = new List<double>();
            List<DrawableObject> Objects = new List<DrawableObject>();

            ObjectsToDisplayWorldSpace = new List<DrawableObject>();
            ObjectsToDisplayScreenSpace = new List<DrawableObject>();
            MovingBackgrounds = new List<DrawableObject>();
            MovingBackgroundSpeed = 1f;
            Emitters = new List<Emitter>();

            // Creating main loop timer
            MainLoopTimer = new DispatcherTimer();
            MainLoopTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)CycleMilliseconds);
            MainLoopTimer.Tick += MainLoopTimer_Tick;
            ElapsedTime = DateTime.Now;

            // Creating the button flags dictionary
            ButtonFlags = new Dictionary<ButtonKey, bool>();
            foreach (ButtonKey b in Enum.GetValues(typeof(ButtonKey)))
                ButtonFlags.Add(b, false);


            //LightColor = new Color(255, 234, 176, lightBlendingAlpha);
            //LightColor = new Color(255, 255, 255, lightBlendingAlpha);
            //LightColor = new Color(255, 255, 112, lightBlendingAlpha);
            LightColor = new Color(255, 255, 200, lightBlendingAlpha);
            Camera = new Camera(WindowSize)
            {
                DeadZone = new Vec2d(5,5),
            };

            LastPressedDirection = ButtonKey.D;

            MousePosition = new Vec2d();

            MaxLives = 3;
            LivesUI = new List<Rectangle>();

            ParticleProperty bloodParticleProperty = new ParticleProperty()
            {
                Shape = new Circle(new Vec2d(), 2) { DrawPriority = DrawPriority.Top },
                Position = new Vec2d(0,0),
                SpeedStart = 100,
                SpeedEnd = 200,
                SpeedLimit = 250,
                ColorStart = new Color(255, 0, 0, 255),
                ColorEnd = new Color(255, 0, 0, 0),
                RotationStart = 0,
                RotationEnd = 0,
                LifeTime = 1,
                EmittingDelay = 0.2,
                EmittingMultiplier = 100,
                EmittingAngle = 0,
                EmittingAngleVariation = 360,
                EmittingPositionVariation = new Vec2d(0, 0),
                EmittingSpeedVariation = 100,
                Gravity = 12,
                EmittingOnlyByUser = true
            };
            BloodEmitter = new Emitter(bloodParticleProperty);
            Emitters.Add(BloodEmitter);

            ParticleProperty checkpointParticleProperty = new ParticleProperty()
            {
                Shape = new Circle(new Vec2d(), 2) { DrawPriority = DrawPriority.Top },
                Position = new Vec2d(0,0),
                SpeedStart = 100,
                SpeedEnd = 200,
                SpeedLimit = 250,
                ColorStart = new Color(0, 255, 0, 255),
                ColorEnd = new Color(0, 0, 255, 0),
                RotationStart = 0,
                RotationEnd = 0,
                LifeTime = 1,
                EmittingDelay = 0.2,
                EmittingMultiplier = 100,
                EmittingAngle = 0,
                EmittingAngleVariation = 360,
                EmittingPositionVariation = new Vec2d(0, 0),
                EmittingSpeedVariation = 100,
                Gravity = 0,
                EmittingOnlyByUser = true
            };
            CheckpointEmitter = new Emitter(checkpointParticleProperty);
            Emitters.Add(CheckpointEmitter);

            #region Sounds
            RunSound = new System.Windows.Media.MediaPlayer();

            JumpSound = new System.Windows.Media.MediaPlayer();

            DeathSound = new System.Windows.Media.MediaPlayer();

            BackgroundSound = new System.Windows.Media.MediaPlayer();
            BackgroundSound.Open(new Uri("Sounds\\bgmusic1.wav", UriKind.RelativeOrAbsolute));
            BackgroundSound.SpeedRatio = 1.0f;
            BackgroundSound.Position = TimeSpan.Zero;
            BackgroundSound.Volume = 0.05f;
            BackgroundSound.Play();
            #endregion


            // Emitter example settings
            //ParticleProperty particleProperty = new ParticleProperty()
            //{
            //    Shape = new Circle(new Vec2d(), 2),
            //    Position = WindowSize / 2,
            //    SpeedStart = 150,
            //    SpeedEnd = 100,
            //    SpeedLimit = 250,
            //    ColorStart = new Color(255, 0, 0, 100),
            //    ColorEnd = new Color(255, 180, 0, 0),
            //    RotationStart = 0,
            //    RotationEnd = 0,
            //    LifeTime = 1,
            //    EmittingDelay = 0.2,
            //    EmittingMultiplier = 50,
            //    EmittingAngle = 180,
            //    EmittingAngleVariation = 90,
            //    EmittingPositionVariation = new Vec2d(0, 0),
            //    EmittingSpeedVariation = 40,
            //    Gravity = 15,
            //    EmittingOnlyByUser = true
            //};

            //emitter = new Emitter(particleProperty);
        }

        /// <summary>
        /// Game logic main enter
        /// </summary>
        public void Start()
        {
            MainLoopTimer.Start();
        }

        /// <summary>
        /// Called by the Main Window through IGameControl
        /// <para>Sets the given button flag in the button dictionary</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isDown"></param>
        public void SetButtonFlag(ButtonKey key, bool isDown)
        {
            ButtonFlags[key] = isDown;

            if(key == ButtonKey.Escape && !isDown)
            {
                if(CurrentState == GameStates.Pause)
                {
                    ChangeState.Invoke(GameStates.Game);
                    CurrentState = GameStates.Game;
                    SceneTimer.Start();
                    MainLoopTimer.Start();
                }
                else if(CurrentState == GameStates.Game)
                {
                    ChangeState.Invoke(GameStates.Pause);
                    CurrentState = GameStates.Pause;
                    SceneTimer.Stop();
                    MainLoopTimer.Stop();
                }
                
            }
        }

        public void SetScene(string title, double bestTime)
        {
            BestTime = bestTime;
            CurrentScene = SceneManager.GetSceneByName(title);
            if (CurrentScene is null)
                CurrentScene = SceneManager.GetDefaultScene();

            CurrentScene.Objects[CurrentScene.PlayerIndex].DrawPriority = DrawPriority.Top;
            IsThereShadow = !(CurrentScene.PlayerLight is null);
            PlayerPosition = CurrentScene.Objects[CurrentScene.PlayerIndex].Position + (CurrentScene.Objects[CurrentScene.PlayerIndex] as Rectangle).Size / 2;

            LastCheckpoint = null;
            Lives = MaxLives;
            SetLivesUI();

            SceneTimer = new Stopwatch();
            SceneTimer.Start();
            MovingBackgrounds = CurrentScene.MovingBackground.GetDefault(WindowSize);
        }

        public void SetMousePosition(double x, double y)
        {
            MousePosition.x = x;
            MousePosition.y = y;
        }

        /// <summary>
        /// Called by the Main Window through IGameControl
        /// </summary>
        /// <param name="WindowSizeWidth"></param>
        /// <param name="WindowSizeHeight"></param>
        public void WindowSizeChanged(int WindowSizeWidth, int WindowSizeHeight)
        {
            WindowSize.x = WindowSizeWidth;
            WindowSize.y = WindowSizeHeight;
        }

        private void PlaySound(string sound, double speed = 1.0f, double volume = 0.25f, double position = 0.0f)
        {
            if(sound == "run")
            {
                RunSound = new System.Windows.Media.MediaPlayer();
                RunSound.Open(new Uri("Sounds\\run.wav", UriKind.RelativeOrAbsolute));
                RunSound.SpeedRatio = 1.0f;
                RunSound.Position = TimeSpan.FromSeconds(position);
                //RunSound.Position = TimeSpan.Zero;
                RunSound.Volume = 0.2f;
                RunSound.Play();
            }
            else if(sound == "jump")
            {
                JumpSound.Open(new Uri("Sounds\\jump.wav", UriKind.RelativeOrAbsolute));
                JumpSound.SpeedRatio = speed;
                JumpSound.Position = TimeSpan.FromSeconds(position);
                JumpSound.Volume = volume;
                JumpSound.Play();
            }
            else if(sound == "death")
            {
                DeathSound.Open(new Uri("Sounds\\death.wav", UriKind.RelativeOrAbsolute));
                DeathSound.SpeedRatio = speed;
                DeathSound.Position = TimeSpan.FromSeconds(position);
                DeathSound.Volume = volume;
                DeathSound.Play();
            }
        }

        public void Mute()
        {
            if (BackgroundSound.Volume == 0)
            {
                BackgroundSound.Volume = 0.05f;
                BackgroundSound.Position = TimeSpan.Zero;
            }
            else
                BackgroundSound.Volume = 0;
        }

        /// <summary>
        /// Main game logic loop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainLoopTimer_Tick(object sender, EventArgs e)
        {
            // Calculating delta time for physics calculations
            Elapsed = (DateTime.Now - ElapsedTime).TotalSeconds;
            ElapsedTime = DateTime.Now;

            // To avoid 'teleporting' objects in case of a FPS drop.
            if (Elapsed > 0.5)
                Elapsed = FPSTarget/1000.0f;

            // Which directions are allowed at this frame.
            #region
            bool up = true;
            bool left = true;
            bool down = true;
            bool right = true;
            #endregion

            // Uncomment to get FPS property -> To display averaged FPS
            #region
            //double currentFps = 1.0f / Elapsed;
            //RecentFPS.Add(currentFps);
            //if (RecentFPS.Count > 20)
            //    RecentFPS.Remove(RecentFPS.First());
            //ObjectsToDisplayScreenSpace.Add(new Text(new Vec2d(10, 10), FPS.ToString(), 25, new Color(255, 255, 255)));
            #endregion

            Update();
            Camera.UpdatePosition(CurrentScene.Objects[CurrentScene.PlayerIndex].Position, Elapsed);


            foreach (var obj in CurrentScene.Objects)
            {
                if (obj.IsPlayer && obj is Player p)
                {
                    PlayerMovement(p, ref up, ref left, ref down, ref right);
                }

                if (obj is Checkpoint cp)
                {
                    if (cp.Intersects(CurrentScene.Objects[CurrentScene.PlayerIndex]) && LastCheckpoint != cp)
                    {
                        CheckpointEmitter.Emit(cp.GetMiddle());

                        if (!(LastCheckpoint is null))
                            LastCheckpoint.Texture = new BitmapImage(new Uri("Textures\\checkpoint.png", UriKind.RelativeOrAbsolute));
                        LastCheckpoint = cp;
                        LastCheckpoint.Texture = new BitmapImage(new Uri("Textures\\checkpointChecked.png", UriKind.RelativeOrAbsolute));
                    }
                }

                if(obj is End end)
                {
                    if (end.Intersects(CurrentScene.Objects[CurrentScene.PlayerIndex]))
                    {
                        RunSound.Stop();
                        JumpSound.Stop();
                        foreach (ButtonKey key in ButtonFlags.Keys)
                            ButtonFlags[key] = false;

                        SceneTimer.Stop();
                        MainLoopTimer.Stop();
                        ChangeState.Invoke(GameStates.End);
                        CurrentState = GameStates.End;
                    }
                }

                if (obj is Trap t)
                {
                    t.Position.x += TrapColliderMinus;
                    t.Position.y += TrapColliderMinus;
                    t.Size.x -= TrapColliderMinus*2;
                    t.Size.y -= TrapColliderMinus*2;

                    if(t.Intersects(CurrentScene.Objects[CurrentScene.PlayerIndex]))
                    {
                        PlayerDies();
                    }

                    t.Position.x -= TrapColliderMinus;
                    t.Position.y -= TrapColliderMinus;
                    t.Size.x += TrapColliderMinus*2;
                    t.Size.y += TrapColliderMinus*2;
                }

                #region StateMachine and IsAffectedByCamera
                if (!(obj.StateMachine is null) && CurrentState != GameStates.Pause)
                    obj.StateMachine.Update();

                if (obj.IsAffectedByCamera)
                    ObjectsToDisplayWorldSpace.Add(obj);
                else
                    ObjectsToDisplayScreenSpace.Add(obj);
                #endregion 
            }


            //foreach (Rectangle r in CurrentScene.MergedForeground)
            //{
            //    r.OutLineColor = Color.Red;
            //    r.OutLineThickness = 1;
            //    r.Color = new Color(0, 0, 0, 0);
            //    ObjectsToDisplayWorldSpace.Add(r);
            //}

            MovingBackgrounds = CurrentScene.MovingBackground.UpdateBackground(WindowSize);

            Control(up, left, down, right);

            if (CurrentScene.Objects[CurrentScene.PlayerIndex].Position.y > CurrentScene.LowestPoint+100)
                PlayerDies();

            if (!(CurrentScene.PlayerLight is null))
                CurrentScene.PlayerLight.Position = CurrentScene.Objects[CurrentScene.PlayerIndex].Position;

            #region Emitter
            for (int i = Emitters.Count() - 1; i >= 0; i--)
            {
                Emitters[i].Update(Elapsed);
                if (Emitters[i].EmittingTime <= 0)
                    Emitters.Remove(Emitters[i]);
                else
                {
                    foreach (Particle p in Emitters[i].Particles)
                    {
                        ObjectsToDisplayWorldSpace.Add(p.Shape);
                    }
                }
            }
            #endregion

            #region TimerText
            Text timerText = new Text(new Vec2d(WindowSize.x / 2 - 90, 10), SceneTimer.Elapsed.ToString(@"mm\:ss\.fff"), 40, Color.White)
            {
                OutLineColor = Color.Black,
                OutLineThickness = 2
            };
            ObjectsToDisplayScreenSpace.Add(timerText);
            #endregion

            #region Lives
            foreach(DrawableObject d in LivesUI)
            {
                ObjectsToDisplayScreenSpace.Add(d);
            }
            #endregion

            #region BackgroundMusic
            if(BackgroundSound.Position == BackgroundSound.NaturalDuration)
            {
                BackgroundSound.Position = TimeSpan.Zero;
            }
            #endregion

            PlayerPosition = CurrentScene.Objects[CurrentScene.PlayerIndex].Position + (CurrentScene.Objects[CurrentScene.PlayerIndex] as Rectangle).Size/2;
            ObjectsToDisplayWorldSpace.Sort();
            // Invoking the OnRender function in the Display class through event
            DrawEvent.Invoke((int)WindowSize.x, (int)WindowSize.y);
        }

        private void SetLivesUI()
        {
            LivesUI = new List<Rectangle>();
            for (int i = 0; i < 3; i++)
            {
                LivesUI.Add(new Rectangle(new Vec2d(25 + (i * 40), 25), new Vec2d(40, 40)));
                if (Lives >= (i + 1))
                    LivesUI.Last().Texture = new BitmapImage(new Uri("UITextures\\heart.png", UriKind.RelativeOrAbsolute));
                else
                    LivesUI.Last().Texture = new BitmapImage(new Uri("UITextures\\heartempty.png", UriKind.RelativeOrAbsolute));
            }
        }

        private void PlayerDies()
        {
            PlaySound("death");

            BloodEmitter.Emit(CurrentScene.Objects[CurrentScene.PlayerIndex].Position);

            if (LastCheckpoint is null)
            {
                SetScene(CurrentScene.Title, BestTime);
            }
            else
            {
                Lives--;
                if(Lives <= 0)
                    SetScene(CurrentScene.Title, BestTime);
                else
                {
                    CurrentScene.Objects[CurrentScene.PlayerIndex].Position = new Vec2d(LastCheckpoint.Position);
                    SetLivesUI();
                }
            }
        }

        /// <summary>
        /// Input checking
        /// </summary>
        private void Control(bool up = true, bool left = true, bool down = true, bool right = true)
        {
            if (!ButtonFlags[ButtonKey.A] && !ButtonFlags[ButtonKey.D] &&
                IsRunning)
            {
                RunSound.Stop();
                IsRunning = false;
            }
            if ((ButtonFlags[ButtonKey.A] || ButtonFlags[ButtonKey.D]) &&
                RunSound.Position == RunSound.NaturalDuration && IsRunning)
            {
                PlaySound("run", speed:1.0f, position: 0);
            }

            // Up
            if ((ButtonFlags[ButtonKey.W] || ButtonFlags[ButtonKey.Space]) && up && CurrentScene.Objects[CurrentScene.PlayerIndex].IsOnGround)
            {

                Rectangle testRectangle = new Rectangle(new Vec2d(CurrentScene.Objects[CurrentScene.PlayerIndex].Position),
                                                        new Vec2d((CurrentScene.Objects[CurrentScene.PlayerIndex] as Rectangle).Size));
                testRectangle.Position.y -= testRectangle.Size.y;
                testRectangle.Size.x /= 2;
                testRectangle.Position.x += testRectangle.Size.x / 2;

                bool hitCeiling = false;
                foreach (DrawableObject d in CurrentScene.Objects)
                {
                    if (!(d is Rectangle) || d is Player || d.ObjectType != DrawableObject.ObjectTypes.Foreground)
                        continue;

                    if (testRectangle.Intersects(d as Rectangle))
                        hitCeiling = true;
                }

                PlaySound("jump", volume:0.08f);
                RunSound.Stop();
                IsRunning = false;

                if(hitCeiling)
                    IsGravitySet(CurrentScene.Objects[CurrentScene.PlayerIndex], true, new Vec2d(0, -200));
                else
                    IsGravitySet(CurrentScene.Objects[CurrentScene.PlayerIndex], true, new Vec2d(0, -375));

                MovementLogic.Move(CurrentScene.Objects[CurrentScene.PlayerIndex], Elapsed);
                CurrentScene.Objects[CurrentScene.PlayerIndex].IsOnGround = false;

                if (ButtonFlags[ButtonKey.A] && left)
                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.SetState("jumpingleft", stopOn: 3);
                else if (ButtonFlags[ButtonKey.D] && right)
                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.SetState("jumpingright", stopOn: 3);
                else if (LastPressedDirection == ButtonKey.D)
                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.SetState("jumpingright", stopOn: 3);
                else if (LastPressedDirection == ButtonKey.A)
                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.SetState("jumpingleft", stopOn: 3);
            }
            // Reset the IsOnGround property if key 'W' isn't pressed to avoid infinite jumping.
            if ((!ButtonFlags[ButtonKey.W] && !ButtonFlags[ButtonKey.Space]) && !CurrentScene.Objects[CurrentScene.PlayerIndex].IsOnGround)
            {
                CurrentScene.Objects[CurrentScene.PlayerIndex].IsOnGround = true;
            }

            // Left
            else if (ButtonFlags[ButtonKey.A] && left)
            {
                if (!IsRunning && !CurrentScene.Objects[CurrentScene.PlayerIndex].IsGravity)
                {
                    IsRunning = true;
                    PlaySound("run", speed: 1.0f, position:rnd.Next(10,60)/10.0f);
                }

                LastPressedDirection = ButtonKey.A;
                MovementLogic.Move(CurrentScene.Objects[CurrentScene.PlayerIndex], new Vec2d(-1.5, 0), 200.0f * Elapsed);
                //CurrentScene.MovingBackground.BackgroundPosition += 0.2 * MovingBackgroundSpeed;
                //CurrentScene.MovingBackground.FarPosition += 0.75 * MovingBackgroundSpeed;
                //CurrentScene.MovingBackground.MiddlePosition += 1.65 * MovingBackgroundSpeed;
                //CurrentScene.MovingBackground.ClosePosition += 2.55 * MovingBackgroundSpeed;

                if (!(CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine is null) &&
                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.CurrentState != "runleft" &&
                    !CurrentScene.Objects[CurrentScene.PlayerIndex].IsGravity)
                {
                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.SetState("runleft");
                }
                else if (!(CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine is null) &&
                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.CurrentState != "jumpingleft" &&
                    CurrentScene.Objects[CurrentScene.PlayerIndex].IsGravity)
                {
                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.SetState("jumpingleft");
                }
            }

            // Down
            else if (ButtonFlags[ButtonKey.S] && down)
                MovementLogic.Move(CurrentScene.Objects[CurrentScene.PlayerIndex], new Vec2d(0, 1), 200.0f * Elapsed);

            // Right
            else if (ButtonFlags[ButtonKey.D] && right)
            {
                if (!IsRunning && !CurrentScene.Objects[CurrentScene.PlayerIndex].IsGravity)
                {
                    IsRunning = true;
                    PlaySound("run", speed: 1.0f, position: rnd.Next(10, 60) / 10.0f);
                }

                LastPressedDirection = ButtonKey.D;
                MovementLogic.Move(CurrentScene.Objects[CurrentScene.PlayerIndex], new Vec2d(1.5, 0), 200.0f * Elapsed);
                //CurrentScene.MovingBackground.BackgroundPosition -= 0.2 * MovingBackgroundSpeed;
                //CurrentScene.MovingBackground.FarPosition -= 0.75 * MovingBackgroundSpeed;
                //CurrentScene.MovingBackground.MiddlePosition -= 1.65 * MovingBackgroundSpeed;
                //CurrentScene.MovingBackground.ClosePosition -= 2.55 * MovingBackgroundSpeed;

                if (!(CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine is null) &&
                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.CurrentState != "runright" &&
                    !CurrentScene.Objects[CurrentScene.PlayerIndex].IsGravity)
                {
                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.SetState("runright");
                }
                else if (!(CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine is null) &&
                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.CurrentState != "jumpingright" &&
                    CurrentScene.Objects[CurrentScene.PlayerIndex].IsGravity)
                {
                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.SetState("jumpingright");
                }
            }

            else if (!(CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine is null) &&
                !ButtonFlags[ButtonKey.A] && !ButtonFlags[ButtonKey.D] &&
                !CurrentScene.Objects[CurrentScene.PlayerIndex].IsGravity)
            {
                if (LastPressedDirection == ButtonKey.A &&
                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.CurrentState != "idleleft")
                {
                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.SetState("idleleft");
                }
                else if (LastPressedDirection == ButtonKey.D &&
                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.CurrentState != "idleright")
                {
                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.SetState("idleright");
                }
            }

            // If player is in the air, the Move() is called due to gravity.
            // --------------- WARNING: it affects only on Player object!! ----------------
            if (CurrentScene.Objects[CurrentScene.PlayerIndex].IsGravity)
                MovementLogic.Move(CurrentScene.Objects[CurrentScene.PlayerIndex], Elapsed);
        }

        /// <summary>
        /// Logic Update
        /// </summary>
        private void Update()
        {
            // Game Logic Update

            // Lighting
            List<DrawableObject> mergedForeground = new List<DrawableObject>(CurrentScene.MergedForeground);
            foreach (DynamicPointLight dpl in CurrentScene.PointLights)
            {
                Vec2d originalPos = new Vec2d(dpl.Position);
                for (int i = 0; i < shadowPasses; i++)
                {
                    dpl.Position = originalPos + new Vec2d(1,0) + new Vec2d(i * shadowIntensity, i * shadowIntensity);
                    Shadow shadow = dpl.GetShadows(mergedForeground, WindowSize, Camera);

                    if (shadow is null)
                        continue;

                    shadow.Color = LightColor;
                    shadow.DrawPriority = DrawPriority.Custom(100);
                    ObjectsToDisplayWorldSpace.Add(shadow);
                }
                dpl.Position = originalPos;
            }

            if (!(CurrentScene.PlayerLight is null))
            {
                Vec2d originalPos = new Vec2d(CurrentScene.PlayerLight.Position);
                for (int i = 0; i < shadowPasses; i++)
                {
                    CurrentScene.PlayerLight.Position = originalPos + new Vec2d(1, 0) + new Vec2d(i * shadowIntensity, i * shadowIntensity);
                    Shadow shadow = CurrentScene.PlayerLight.GetShadows(mergedForeground, WindowSize, Camera);

                    if (shadow is null)
                        continue;

                    shadow.Color = LightColor;
                    shadow.DrawPriority = DrawPriority.Custom(100);
                    ObjectsToDisplayWorldSpace.Add(shadow);
                }
                CurrentScene.PlayerLight.Position = originalPos;
            }
        }

        /// <summary>
        /// Player's movement logic.
        /// </summary>
        /// <param name="p">Player instance to be moved</param>
        /// <param name="up">Direction allowance</param>
        /// <param name="left">Direction allowance</param>
        /// <param name="down">Direction allowance</param>
        /// <param name="right">Direction allowance</param>
        private void PlayerMovement(Player p, ref bool up, ref bool left, ref bool down, ref bool right)
        {
            bool doesIntersect = false;
            bool leftWall = false;
            bool rightWall = false;

            foreach (var item in CurrentScene.Objects)
            {
                if (!p.Equals(item) && item is Rectangle r && p.Intersects(item) && item.ObjectType == DrawableObject.ObjectTypes.Foreground)
                {
                    doesIntersect = true;

                    // Angle of Vector IO.
                    double vecInDegrees = (p.GetMiddleLeft() - r.GetMiddle()).Length >= (p.GetMiddleRight() - r.GetMiddle()).Length
                        ? (p.GetMiddleLeft() - r.GetMiddle()).Angle
                        : (p.GetMiddleRight() - r.GetMiddle()).Angle;

                    //double vecInDegrees = (obj.GetMiddle() - item.GetMiddle()).Angle;
                    if (vecInDegrees < 45 || vecInDegrees > 315)
                    {
                        leftWall = true;
                        // Player is on the RIGHT side
                        if (p.Position.x < r.Position.x + r.Size.x)
                        {
                            //double delta = r.Position.x + r.Size.x - p.Position.x;
                            //CurrentScene.MovingBackground.BackgroundPosition += delta / 6 * MovingBackgroundSpeed;
                            //CurrentScene.MovingBackground.FarPosition += delta / 2 * MovingBackgroundSpeed;
                            //CurrentScene.MovingBackground.MiddlePosition += delta * 1.1 * MovingBackgroundSpeed;
                            //CurrentScene.MovingBackground.ClosePosition += delta * 1.7 * MovingBackgroundSpeed;

                            p.Position.x = r.Position.x + r.Size.x;
                        }
                        //r.Color = Color.Red;
                        left = false;
                    }
                    else if (vecInDegrees >= 45 && vecInDegrees <= 135)
                    {
                        // Player is UNDER item
                        if (p.Position.y < r.Position.y + r.Size.y)
                        {
                            p.Position.y = r.Position.y + r.Size.y+10;
                        }
                        //r.Color = Color.Yellow;
                        up = false;


                        if (p.IsGravity)
                        {
                            p.Velocity.x = -p.Velocity.x;
                            p.Velocity.y = -p.Velocity.y;
                        }
                    }
                    else if (vecInDegrees > 135 && vecInDegrees < 225)
                    {
                        rightWall = true;
                        // Player is on the LEFT side
                        if (p.Right > r.Position.x)
                        {
                            //double delta = p.Right - r.Position.x;
                            //CurrentScene.MovingBackground.BackgroundPosition -= delta / 6 * MovingBackgroundSpeed;
                            //CurrentScene.MovingBackground.FarPosition -= delta / 2 * MovingBackgroundSpeed;
                            //CurrentScene.MovingBackground.MiddlePosition -= delta * 1.1 * MovingBackgroundSpeed;
                            //CurrentScene.MovingBackground.ClosePosition -= delta * 1.7 * MovingBackgroundSpeed;

                            p.Position.x = r.Position.x - p.Size.x;
                        }
                        //r.Color = Color.Blue;
                        right = false;
                    }
                    else
                    {
                        // Player is ABOVE item
                        if (p.Bottom > r.Position.y)
                        {
                            p.Position.y = r.Position.y - p.Size.y;
                        }
                        //r.Color = Color.Green;
                        down = false;

                        // Turn off gravity
                        if (!(CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine is null) &&
                                !ButtonFlags[ButtonKey.A] && !ButtonFlags[ButtonKey.D] &&
                                !CurrentScene.Objects[CurrentScene.PlayerIndex].IsGravity)
                        {
                            if (LastPressedDirection == ButtonKey.A &&
                                CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.CurrentState != "idleleft")
                            {
                                CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.SetState("idleleft");
                            }
                            else if (LastPressedDirection == ButtonKey.D &&
                                    CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.CurrentState != "idleright")
                            {
                                CurrentScene.Objects[CurrentScene.PlayerIndex].StateMachine.SetState("idleright");
                            }
                        }
                        IsGravitySet(p, false, null);
                    }
                }
                //else if (!p.Equals(item) && item.Color != Color.Gray)
                //{
                //    item.Color = Color.Gray;
                //}
            }

            if(ButtonFlags[ButtonKey.D] && !rightWall)
            {
                CurrentScene.MovingBackground.BackgroundPosition -= 0.2 * MovingBackgroundSpeed;
                CurrentScene.MovingBackground.FarPosition -= 0.75 * MovingBackgroundSpeed;
                CurrentScene.MovingBackground.MiddlePosition -= 1.65 * MovingBackgroundSpeed;
                CurrentScene.MovingBackground.ClosePosition -= 2.55 * MovingBackgroundSpeed;
            }
            else if(ButtonFlags[ButtonKey.A] && !leftWall)
            {
                CurrentScene.MovingBackground.BackgroundPosition += 0.2 * MovingBackgroundSpeed;
                CurrentScene.MovingBackground.FarPosition += 0.75 * MovingBackgroundSpeed;
                CurrentScene.MovingBackground.MiddlePosition += 1.65 * MovingBackgroundSpeed;
                CurrentScene.MovingBackground.ClosePosition += 2.55 * MovingBackgroundSpeed;
            }

            if (!doesIntersect && !p.IsGravity)
                IsGravitySet(p, true, new Vec2d(0, 0));
            else if (!doesIntersect && p.IsGravity)
                up = false;
            else if (!up && down && !p.IsGravity)
                IsGravitySet(p, true, new Vec2d(0, 1));
        }

        private void IsGravitySet(DrawableObject obj, bool value, Vec2d newVelocity)
        {
            obj.IsGravity = value;
            if (value)
                obj.Velocity = newVelocity;
        }
    }
}