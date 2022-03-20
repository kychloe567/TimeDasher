using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SZGUIFeleves.Models;

namespace SZGUIFeleves.Logic
{
    public delegate void DrawDelegate();

    public enum ButtonKey // TODO: More buttons to add if needed
    {
        W, A, S, D,
        Up, Down, Left, Right,
        Space, C, LeftCtrl,
        Q, E
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

            // This is an example for creating a scene/level
            //List<DrawableObject> Objects = new List<DrawableObject>();
            //Objects.Add(new Circle(new Vec2d(500, 200), 25, Color.Green) { IsPlayer = true });
            //Objects.Add(new Rectangle(new Vec2d(100, 100), new Vec2d(50, 50), Color.Red));
            //Scene s = new Scene("try1", new List<DrawableObject>(Objects), 0, new List<DynamicPointLight>());
            //SceneManager.SaveScene(s);

            CurrentScene = SceneManager.GetScene("try1");
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
            //ObjectsToDisplay.Add(new Text(new Vec2d(10, 10), FPS.ToString(), 25, new Color(255, 255, 255)));

            Control();  // Keyboard/Mouse input
            Update();   // Game logic update

            Camera.UpdatePosition(CurrentScene.Objects[CurrentScene.PlayerIndex].Position, Elapsed);

            CurrentScene.Objects.Sort(); // Sorting drawable objects by DrawPriority (not necessary if items added in order)
            foreach (var obj in CurrentScene.Objects)
            {
                if (!(obj.StateMachine is null))
                    obj.StateMachine.Update();

                if (obj.IsAffectedByCamera)
                    ObjectsToDisplayWorldSpace.Add(obj);
                else
                    ObjectsToDisplayScreenSpace.Add(obj);
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
                CurrentScene.Objects[CurrentScene.PlayerIndex].Position.y -= 100.0f * Elapsed;
            if (ButtonFlags[ButtonKey.S])
                CurrentScene.Objects[CurrentScene.PlayerIndex].Position.y += 100.0f * Elapsed;
            if (ButtonFlags[ButtonKey.A])
                CurrentScene.Objects[CurrentScene.PlayerIndex].Position.x -= 100.0f * Elapsed;
            if (ButtonFlags[ButtonKey.D])
                CurrentScene.Objects[CurrentScene.PlayerIndex].Position.x += 100.0f * Elapsed;
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
                    CurrentScene.Objects.Add(shadow);
                }
                dpl.Position = originalPos;
            }
        }
    }
}
