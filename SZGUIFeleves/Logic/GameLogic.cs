using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SZGUIFeleves.Models;
using SZGUIFeleves.Models.DrawableObjects;

namespace SZGUIFeleves.Logic
{
    public delegate void DrawDelegate();

    public enum ButtonKey // TODO: More buttons to add if needed
    {
        W, A, S, D,
        Up, Down, Left, Right,
        Space, C, LeftCtrl,
        Q, E,
        MouseLeft, MouseRight
    }

    public class GameLogic : IGameModel, IGameControl
    {
        int iterationCountTEST = 0;
        #region App Constants
        private const int FPSTarget = 60;
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
        private IGravityLogic gravityLogic;

        public Vec2d WindowSize { get; set; }

        public event DrawDelegate DrawEvent;
        private DispatcherTimer MainLoopTimer { get; set; }
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
        public Scene CurrentScene { get; set; }
        public Vec2d MousePosition { get; set; }
        #endregion

        #region Lighting Variables
        private const int shadowPasses = 5;
        private const int shadowIntensity = 4;
        private const int lightBlendingAlpha = 150;
        private Color LightColor { get; set; }
        #endregion

        public GameLogic(int WindowSizeWidth, int WindowSizeHeight)
        {
            WindowSize = new Vec2d(WindowSizeWidth, WindowSizeHeight);
            gravityLogic = new GravityLogic();

            // Calculating frame interval with the given FPS target
            CycleMilliseconds = 1.0f / FPSTarget * 1000.0f;
            RecentFPS = new List<double>();

            ObjectsToDisplayWorldSpace = new List<DrawableObject>();
            ObjectsToDisplayScreenSpace = new List<DrawableObject>();

            // Creating main loop timer
            MainLoopTimer = new DispatcherTimer();
            MainLoopTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)CycleMilliseconds);
            MainLoopTimer.Tick += MainLoopTimer_Tick;
            ElapsedTime = DateTime.Now;

            // Creating the button flags dictionary
            ButtonFlags = new Dictionary<ButtonKey, bool>();
            foreach (ButtonKey b in Enum.GetValues(typeof(ButtonKey)))
                ButtonFlags.Add(b, false);


            LightColor = new Color(255, 234, 176, lightBlendingAlpha);
            Camera = new Camera(WindowSize)
            {
                DeadZone = new Vec2d(75, 20),
            };

            MousePosition = new Vec2d();

            // This is an example for creating a scene/level
            //List<DrawableObject> Objects = new List<DrawableObject>();
            //Objects.Add(new Circle(new Vec2d(500, 200), 25, Color.Green) { IsPlayer = true });
            //Objects.Add(new Rectangle(new Vec2d(100, 100), new Vec2d(50, 50), Color.Red));
            //Scene s = new Scene("try1", new List<DrawableObject>(Objects), 0, new List<DynamicPointLight>());
            //SceneManager.SaveScene(s);

            CurrentScene = SceneManager.GetScene("try1");
            if (CurrentScene is null)
                CurrentScene = SceneManager.GetDefaultScene();

            CurrentScene.Objects.Add(new Player() {
                IsPlayer = true,
                IsFilled = true,
                GravityStart = DateTime.Now,
                Size = new Vec2d(50, 50),
                Position = new Vec2d(100, 100),
                Color = Color.Purple,
                TextureOpacity = 50,
            });
            CurrentScene.Objects.Add(new Rectangle()
            {
                Position = new Vec2d(120, 170),
                Size = new Vec2d(250, 25),
                IsFilled = true,
                Color = Color.Red
            });
            CurrentScene.Objects.Add(new Rectangle()
            {
                Position = new Vec2d(220, 270),
                Size = new Vec2d(250, 25),
                IsFilled = true,
                Color = Color.Red
            });

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

            // Uncomment to get FPS property -> To display averaged FPS
            //double currentFps = 1.0f / Elapsed;
            //RecentFPS.Add(currentFps);
            //if (RecentFPS.Count > 20)
            //    RecentFPS.Remove(RecentFPS.First());
            //ObjectsToDisplayWorldSpace.Add(new Text(new Vec2d(10, 10), FPS.ToString(), 25, new Color(255, 255, 255)));

            Control();  // Keyboard/Mouse input
            Update();   // Game logic update


            // Remove after player has been created 
            Camera.UpdatePosition(WindowSize / 2, Elapsed);
            // Then uncomment this
            //Camera.UpdatePosition(CurrentScene.Objects[CurrentScene.PlayerIndex].Position, Elapsed);

            CurrentScene.Objects.Sort(); // Sorting drawable objects by DrawPriority (not necessary if items added in order)
            foreach (var obj in CurrentScene.Objects)
            {
                if (!(obj.StateMachine is null))
                    obj.StateMachine.Update();

                if (obj.IsAffectedByCamera)
                    ObjectsToDisplayWorldSpace.Add(obj);
                else
                    ObjectsToDisplayScreenSpace.Add(obj);

                if (obj.IsPlayer)
                {
                    // TODO
                    // Intersect check v1.0
                    bool doesIntersect = false;
                    foreach (var subItem in CurrentScene.Objects)
                    {
                        // If player intersects with another object, gravity should be disabled.
                        if (!obj.Equals(subItem) && obj.Intersects(subItem))
                        {
                            //// obj.IsFalling = false;
                            //gravityLogic.IsFalling(obj, ElapsedTime, false);
                            doesIntersect = true;

                            if (!obj.IsJumping)
                                obj.IsGravitySet(false);
                            break;
                        }
                    }

                    if (!doesIntersect && !obj.IsGravity)
                    {
                        obj.IsGravitySet(true);
                    }

                    // iterationCountTEST++;
                    // string output = iterationCountTEST + ":\nBEFORE:\t" + obj.GravityStart.ToString() + "\t" + obj.GravityTimeElapsed.ToString();

                    // Time elapsed update
                    obj.GravityTimeElapsed = (DateTime.Now - obj.GravityStart).TotalSeconds;

                    // output += "\nAFTER:\t" + obj.GravityStart.ToString() + "\t" + obj.GravityTimeElapsed.ToString() + "\n\n";



                    // If player doesn't intersect with another object.
                    if (obj.IsJumping && obj.IsGravity)
                    {
                        // If code is here, then IsJumping == true. DoesIntersect is unknown yet.
                        // JUMP - v0 > 0
                        gravityLogic.Jumping(obj, 10);
                    }
                    else if (doesIntersect)
                    {
                        // If code is here, then IsJumping == false.
                        // STOP

                    }
                    else //if (!doesIntersect && !obj.IsJumping)
                    {
                        // If code is here, then doesIntersect == false && IsJumping == false.
                        // FALL - v0 = 0
                        gravityLogic.Jumping(obj, 0);
                    }
                    //if (doesIntersect && obj.IsJumping)
                    //{
                    //    // obj.IsFalling = true;
                    //    gravityLogic.IsFalling(obj, ElapsedTime, true);
                    //    // obj.IsJumping = false;
                    //    gravityLogic.IsJumping(obj, ElapsedTime, false);

                    //    // Update TimeElapsed
                    //    //obj.FallingStart = DateTime.Now;
                    //}

                    //// Gravity check
                    //if (obj.IsFalling)
                    //{
                    //    obj.TimeElapsed = (ElapsedTime - obj.FallingStart).TotalSeconds;
                    //    gravityLogic.Falling(obj);
                    //}
                    //else if (obj.IsJumping)
                    //{
                    //    obj.TimeElapsed = (DateTime.Now - obj.FallingStart).TotalSeconds;
                    //    gravityLogic.Jumping(obj);
                    //}
                }
            }
            DrawEvent.Invoke(); // Invoking the OnRender function in the Display class through event
        }

        /// <summary>
        /// Input checking
        /// </summary>
        private void Control()
        {
            // Button control checks
            if (ButtonFlags[ButtonKey.W])
                if (!CurrentScene.Objects[CurrentScene.PlayerIndex].IsGravity && !CurrentScene.Objects[CurrentScene.PlayerIndex].IsJumping)
                    CurrentScene.Objects[CurrentScene.PlayerIndex].IsGravitySet(true, true);
                //CurrentScene.Objects[CurrentScene.PlayerIndex].Position.y -= 100.0f * Elapsed;
            //if (ButtonFlags[ButtonKey.S])
            //    CurrentScene.Objects[CurrentScene.PlayerIndex].Position.y += 100.0f * Elapsed;
            if (ButtonFlags[ButtonKey.A])
                CurrentScene.Objects[CurrentScene.PlayerIndex].Position.x -= 200.0f * Elapsed;
            if (ButtonFlags[ButtonKey.D])
                CurrentScene.Objects[CurrentScene.PlayerIndex].Position.x += 200.0f * Elapsed;
            //if (ButtonFlags[ButtonKey.Space])
            //    gravityLogic.IsJumping(CurrentScene.Objects[CurrentScene.PlayerIndex], ElapsedTime, true);
        }

        /// <summary>
        /// Logic Update
        /// </summary>
        private void Update()
        {
            // Game Logic Update

            // Lighting
            foreach (DynamicPointLight dpl in CurrentScene.PointLights)
            {
                Vec2d originalPos = new Vec2d(dpl.Position);
                for (int i = 0; i < shadowPasses; i++)
                {
                    dpl.Position = originalPos + new Vec2d(i*shadowIntensity, i*shadowIntensity);
                    var shadow = dpl.GetShadows(CurrentScene.Objects, WindowSize);
                    if (shadow is null)
                        continue;

                    shadow.Color = LightColor;
                    ObjectsToDisplayWorldSpace.Add(shadow);
                }
                dpl.Position = originalPos;
            }
        }
    }
}
