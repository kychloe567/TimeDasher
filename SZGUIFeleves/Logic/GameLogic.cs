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

        #region Game Variables
        private Car Car { get; set; }
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

            Car = new Car(new Vec2d(100, 100), new Vec2d(40,20));
            Car.Color = Color.White;
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

            ObjectsToDisplay = new List<DrawableObject>();
            ObjectsToDisplay.Add(Car);


            Control();
            Update();

            Line l = new Line(Car.FrontMiddle, Car.FrontMiddleOut, Color.Red,2);
            ObjectsToDisplay.Add(l);



            ObjectsToDisplay.Sort();
            DrawEvent.Invoke();
        }

        private void Control()
        {
            //Button control checks
            if (ButtonFlags[ButtonKey.W])
                Car.Move(Car.SPEED * Elapsed);
            if (ButtonFlags[ButtonKey.S])
                Car.Move(-Car.SPEED * Elapsed);
            if (ButtonFlags[ButtonKey.A])
                Car.RotateWheel(-Car.ROTATION_SPEED * Elapsed);
            if (ButtonFlags[ButtonKey.D])
                Car.RotateWheel(Car.ROTATION_SPEED * Elapsed);

            if (ButtonFlags[ButtonKey.Space])
                Car.Brake();
        }

        private void Update()
        {
            Car.Update();
            //Game Logic Update
        }
    }
}
