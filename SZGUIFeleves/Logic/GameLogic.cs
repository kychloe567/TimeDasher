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

            ObjectsToDisplay.Add(new Ellipse(new Vec2d((int)WindowSize.x / 2, (int)WindowSize.y / 2), new Vec2d(35,35), Color.White));
            ObjectsToDisplay.Add(new Rectangle(new Vec2d(), new Vec2d(1, 1), Color.White));

            Control();  // Keyboard/Mouse input
            Update();   // Game logic update

            ObjectsToDisplay.Sort();    // Sorting drawable objects by DrawPriority (not necessary if items added in order)
            DrawEvent.Invoke(); // Invoking the OnRender function in the Display class through event
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
        }
    }
}
