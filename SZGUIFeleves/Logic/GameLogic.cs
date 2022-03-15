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

    public enum ButtonKey
    {
        W, A, S, D,
        Up, Down, Left, Right,
        Space, C, LeftCtrl
    }

    public class GameLogic : IGameModel, IGameControl
    {
        #region Draw Variables
        public List<DrawableObject> ObjectsToDisplay { get; set; }
        #endregion

        #region App Variables
        public Vec2d WindowSize { get; set; }

        public event DrawDelegate DrawEvent;
        private DispatcherTimer MainLoopTimer;
        private DateTime ElapsedTime { get; set; }
        private double Elapsed { get; set; }
        private Dictionary<ButtonKey, bool> ButtonFlags { get; set; }

        private const int FPSTarget = 60;
        private double CycleMilliseconds { get; set; }
        #endregion

        public GameLogic(int WindowSizeWidth, int WindowSizeHeight)
        {
            WindowSize = new Vec2d(WindowSizeWidth, WindowSizeHeight);

            CycleMilliseconds = 1.0f / FPSTarget * 1000.0f;

            ObjectsToDisplay = new List<DrawableObject>();

            MainLoopTimer = new DispatcherTimer();
            MainLoopTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)CycleMilliseconds);
            MainLoopTimer.Tick += MainLoopTimer_Tick;
            ElapsedTime = DateTime.Now;

            ButtonFlags = new Dictionary<ButtonKey, bool>();
            foreach (ButtonKey b in Enum.GetValues(typeof(ButtonKey)))
                ButtonFlags.Add(b, false);
        }

        public void Start()
        {
            MainLoopTimer.Start();
        }

        public void SetButtonFlag(ButtonKey key, bool isDown)
        {
            ButtonFlags[key] = isDown;
        }

        public void WindowSizeChanged(int WindowSizeWidth, int WindowSizeHeight)
        {
            WindowSize.x = WindowSizeWidth;
            WindowSize.y = WindowSizeHeight;
        }

        private void MainLoopTimer_Tick(object sender, EventArgs e)
        {
            Elapsed = (DateTime.Now - ElapsedTime).TotalSeconds;
            ElapsedTime = DateTime.Now;

            Control();
            Update();
            DrawEvent.Invoke();
        }

        private void Control()
        {
            //Button control checks
            //if (ButtonFlags[ButtonKey.W])
            //    ;
        }

        private void Update()
        {
            //Game Logic Update
        }
    }
}
