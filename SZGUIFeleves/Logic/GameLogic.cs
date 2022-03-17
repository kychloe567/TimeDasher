using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using SZGUIFeleves.Models;

namespace SZGUIFeleves.Logic
{
    public delegate void DrawDelegate();

    public enum ButtonKey // TODO: More buttons to add if needed
    {
        W, A, S, D,
        Up, Down, Left, Right,
        Space, C, LeftCtrl
    }

    public class GameLogic : IGameModel, IGameControl
    {
        #region App Constants
        private const int FPSTarget = 60;
        #endregion

        #region Draw Variables
        public List<DrawableObject> ObjectsToDisplay { get; set; }
        public byte[] BackgroundByteArray { get; set; }
        private byte[] StarterBackgroundByteArray { get; set; }
        #endregion

        #region App Variables
        public Vec2d WindowSize { get; set; }

        public event DrawDelegate DrawEvent;
        private DispatcherTimer MainLoopTimer { get; set; }
        private DateTime ElapsedTime { get; set; }
        private double Elapsed { get; set; }
        private Dictionary<ButtonKey, bool> ButtonFlags { get; set; }
        private double CycleMilliseconds { get; set; }
        #endregion

        #region Temporary Variables
        private List<DrawableObject> Objects { get; set; }
        private List<DynamicPointLight> PointLights { get; set; }
        #endregion

        public GameLogic(int WindowSizeWidth, int WindowSizeHeight)
        {
            WindowSize = new Vec2d(WindowSizeWidth, WindowSizeHeight);

            // Calculating frame interval with the given FPS target
            CycleMilliseconds = 1.0f / FPSTarget * 1000.0f;

            ObjectsToDisplay = new List<DrawableObject>();

            // Creating main loop timer
            MainLoopTimer = new DispatcherTimer();
            MainLoopTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)CycleMilliseconds);
            MainLoopTimer.Tick += MainLoopTimer_Tick;
            ElapsedTime = DateTime.Now;

            // Creating the button flags dictionary
            ButtonFlags = new Dictionary<ButtonKey, bool>();
            foreach (ButtonKey b in Enum.GetValues(typeof(ButtonKey)))
                ButtonFlags.Add(b, false);

            Objects = new List<DrawableObject>();
            PointLights = new List<DynamicPointLight>();

            Vec2d middle = new Vec2d(WindowSizeWidth / 2, WindowSizeHeight / 2);
            PointLights.Add(new DynamicPointLight(middle, 1.0f, 10.0f));

            Rectangle r1 = new Rectangle(middle + new Vec2d(-100, -100), new Vec2d(200, 20), new Color(255, 0, 0));
            Rectangle r2 = new Rectangle(middle + new Vec2d(100, -100), new Vec2d(50, 50), new Color(255, 0, 0));
            Rectangle r3 = new Rectangle(middle + new Vec2d(-100, 100), new Vec2d(50, 50), new Color(255, 0, 0));
            Rectangle r4 = new Rectangle(middle + new Vec2d(100, 100), new Vec2d(100, 50), new Color(255, 0, 0));
            Objects.Add(r1);
            Objects.Add(r2);
            Objects.Add(r3);
            Objects.Add(r4);
            //Line l = new Line(middle + new Vec2d(-100, -100), middle + new Vec2d(100, -100), new Color(255, 255, 255), 2);
            //Objects.Add(l);
            StarterBackgroundByteArray = CreateScreenArray();
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

            foreach (var o in Objects)
                ObjectsToDisplay.Add(o);

            Control();  // Keyboard/Mouse input
            Update();   // Game logic update

            ObjectsToDisplay.Sort();    // Sorting drawable objects by DrawPriority (not necessary if items added in order)
            DrawEvent.Invoke(); // Invoking the OnRender function in the Display class through event
        }

        /// <summary>
        /// Creates the default byte array for the background (for lighting)
        /// at the start of the game
        /// </summary>
        /// <returns></returns>
        public byte[] CreateScreenArray()
        {
            byte[] array = new byte[(int)WindowSize.x * (int)WindowSize.y * 4];
            for (int i = 0; i < array.Count(); i+=4)
            {
                array[i] = 255;
                array[i+1] = 255;
                array[i+2] = 255;
                array[i+3] = 255;
                //if ((i + 1) % 4 == 0)
                //{
                //    array[i] = 255;
                //}
            }
            return array;
        }

        /// <summary>
        /// Input checking
        /// </summary>
        private void Control()
        {
            // Button control checks
            //if (ButtonFlags[ButtonKey.W])
            //    ;
        }

        /// <summary>
        /// Logic Update
        /// </summary>
        private void Update()
        {
            // Game Logic Update

            // Background
            BackgroundByteArray = StarterBackgroundByteArray.ToArray();

            // Lighting
            foreach(DynamicPointLight dpl in PointLights)
            {
                var shadows = dpl.GetShadows(ObjectsToDisplay, WindowSize);

                foreach(var shadow in shadows)
                {
                    ObjectsToDisplay.Add(shadow);
                }
            }
        }
    }
}
