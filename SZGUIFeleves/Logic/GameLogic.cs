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
        public Vec2d WindowSize { get; set; }

        public event DrawDelegate DrawEvent;
        private DispatcherTimer MainLoopTimer { get; set; }
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
            MovementLogic = new MovementLogic();

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

            //CurrentScene = SceneManager.GetScene("movementTest");
            if (CurrentScene is null)
                CurrentScene = SceneManager.GetDefaultScene();

            // This is an example for creating a scene/level
            #region
            List<DrawableObject> Objects = new List<DrawableObject>();
            //Objects.Add(new Circle(new Vec2d(500, 200), 25, Color.Green) { IsPlayer = true });
            //Objects.Add(new Rectangle(new Vec2d(100, 100), new Vec2d(50, 50), Color.Red));


            //Scene s = new Scene("movementTest", Objects, 0, new List<DynamicPointLight>());
            //SceneManager.SaveScene(s);
            #endregion

            CurrentScene = SceneManager.GetSceneByName("try1");
            if (CurrentScene is null)
                CurrentScene = SceneManager.GetDefaultScene();

            Objects.Add(new Player()
            {
                IsPlayer = true,
                Position = new Vec2d(200, 100),
                Size = new Vec2d(30, 70),
                Color = Color.White
            });
            for (int i = 1; i <= 4; i++)
            {
                Objects.Add(new Rectangle()
                {
                    Position = new Vec2d((3 + i) * 100, 100),
                    Size = new Vec2d(100, 100),
                });
                Objects.Add(new Rectangle()
                {
                    Position = new Vec2d(i * 100, 300),
                    Size = new Vec2d(100, 100),
                });
                Objects.Add(new Rectangle()
                {
                    Position = new Vec2d((3 + i) * 100, 500),
                    Size = new Vec2d(100, 100),
                });
                Objects.Add(new Rectangle()
                {
                    Position = new Vec2d(i * 100, 700),
                    Size = new Vec2d(100, 100),
                });
            }
            Objects.Add(new Rectangle()
            {
                Position = new Vec2d(600, 300),
                Size = new Vec2d(100, 100),
            });
            Objects.Add(new Rectangle()
            {
                Position = new Vec2d(200, 500),
                Size = new Vec2d(100, 100),
            });
            CurrentScene.Objects = Objects;


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

            // To avoid 'teleporting' objects in case of a FPS drop.
            if (Elapsed > 0.3)
                Elapsed = 0.3;

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
            //ObjectsToDisplayWorldSpace.Add(new Text(new Vec2d(10, 10), FPS.ToString(), 25, new Color(255, 255, 255)));
            #endregion

            // Camera and objects sort
            #region
            Camera.UpdatePosition(WindowSize / 2, Elapsed);
            // Then uncomment this
            //Camera.UpdatePosition(CurrentScene.Objects[CurrentScene.PlayerIndex].Position, Elapsed);

            CurrentScene.Objects.Sort(); // Sorting drawable objects by DrawPriority (not necessary if items added in order)
            #endregion

            foreach (var obj in CurrentScene.Objects)
            {
                #region StateMachine and IsAffectedByCamera
                if (!(obj.StateMachine is null))
                    obj.StateMachine.Update();

                if (obj.IsAffectedByCamera)
                    ObjectsToDisplayWorldSpace.Add(obj);
                else
                    ObjectsToDisplayScreenSpace.Add(obj);
                #endregion 

                if (obj.IsPlayer && obj is Player p)
                {
                    PlayerMovement(p, ref up, ref left, ref down, ref right);
                }
            }

            Control(up, left, down, right);
            Update();

            // Invoking the OnRender function in the Display class through event
            DrawEvent.Invoke();
        }

        /// <summary>
        /// Input checking
        /// </summary>
        private void Control(bool up = true, bool left = true, bool down = true, bool right = true)
        {
            // Up
            if (ButtonFlags[ButtonKey.W] && up && CurrentScene.Objects[CurrentScene.PlayerIndex].IsOnGround)
            {
                IsGravitySet(CurrentScene.Objects[CurrentScene.PlayerIndex], true, new Vec2d(0, -300));
                MovementLogic.Move(CurrentScene.Objects[CurrentScene.PlayerIndex], Elapsed);
                CurrentScene.Objects[CurrentScene.PlayerIndex].IsOnGround = false;
            }

            // Reset the IsOnGround property if key 'W' isn't pressed to avoid infinite jumping.
            if (!ButtonFlags[ButtonKey.W] && !CurrentScene.Objects[CurrentScene.PlayerIndex].IsOnGround)
                CurrentScene.Objects[CurrentScene.PlayerIndex].IsOnGround = true;

            // Left
            if (ButtonFlags[ButtonKey.A] && left)
                MovementLogic.Move(CurrentScene.Objects[CurrentScene.PlayerIndex], new Vec2d(-1, 0), 200.0f * Elapsed);

            // Down
            if (ButtonFlags[ButtonKey.S] && down)
                MovementLogic.Move(CurrentScene.Objects[CurrentScene.PlayerIndex], new Vec2d(0, 1), 200.0f * Elapsed);

            // Right
            if (ButtonFlags[ButtonKey.D] && right)
                MovementLogic.Move(CurrentScene.Objects[CurrentScene.PlayerIndex], new Vec2d(1, 0), 200.0f * Elapsed);

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

            foreach (var item in CurrentScene.Objects)
            {
                if (!p.Equals(item) && item is Rectangle r && p.Intersects(item))
                {
                    doesIntersect = true;

                    // Angle of Vector IO.
                    double vecInDegrees = (p.GetMiddleLeft() - r.GetMiddle()).Length >= (p.GetMiddleRight() - r.GetMiddle()).Length
                        ? (p.GetMiddleLeft() - r.GetMiddle()).Angle
                        : (p.GetMiddleRight() - r.GetMiddle()).Angle;

                    //double vecInDegrees = (obj.GetMiddle() - item.GetMiddle()).Angle;
                    if (vecInDegrees < 45 || vecInDegrees > 315)
                    {
                        // Player is on the RIGHT side
                        if (p.Position.x < r.Position.x + r.Size.x)
                        {
                            p.Position.x = r.Position.x + r.Size.x;
                        }
                        r.Color = Color.Red;
                        left = false;
                    }
                    else if (vecInDegrees >= 45 && vecInDegrees <= 135)
                    {
                        // Player is UNDER item
                        if (p.Position.y < r.Position.y + r.Size.y)
                        {
                            p.Position.y = r.Position.y + r.Size.y;
                        }
                        r.Color = Color.Yellow;
                        up = false;

                        if (p.IsGravity)
                        {
                            p.Velocity.x = -p.Velocity.x;
                            p.Velocity.y = -p.Velocity.y;
                        }
                    }
                    else if (vecInDegrees > 135 && vecInDegrees < 225)
                    {
                        // Player is on the LEFT side
                        if (p.Right > r.Position.x)
                        {
                            p.Position.x = r.Position.x - p.Size.x;
                        }
                        r.Color = Color.Blue;
                        right = false;
                    }
                    else
                    {
                        // Player is ABOVE item
                        if (p.Bottom > r.Position.y)
                        {
                            p.Position.y = r.Position.y - p.Size.y;
                        }
                        r.Color = Color.Green;
                        down = false;

                        // Turn off gravity
                        IsGravitySet(p, false, null);
                    }
                }
                else if (!p.Equals(item) && item.Color != Color.Gray)
                {
                    item.Color = Color.Gray;
                }
            }
            if (!doesIntersect && !p.IsGravity)
                IsGravitySet(p, true, new Vec2d(0, 0));
            else if (!doesIntersect && p.IsGravity)
                up = false;
        }

        private void IsGravitySet(DrawableObject obj, bool value, Vec2d newVelocity)
        {
            obj.IsGravity = value;
            if (value)
                obj.Velocity = newVelocity;
        }
    }
}
