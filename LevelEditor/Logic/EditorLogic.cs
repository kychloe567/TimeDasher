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

namespace LevelEditor.Logic
{
    public delegate void DrawDelegate();
    public delegate void ItemsUpdatedDelegate(List<BitmapImage> e);

    public enum ButtonKey // TODO: More buttons to add if needed
    {
        W, A, S, D,
        Up, Down, Left, Right,
        Space, C, LeftCtrl,
        Q, E,
        MouseLeft, MouseRight, MouseMiddle
    }

    public class EditorLogic : IEditorModel, IEditorControl
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
        private List<DrawableObject> Objects { get; set; }
        #endregion

        #region App Variables
        public Vec2d WindowSize { get; set; }

        public event DrawDelegate DrawEvent;
        public event ItemsUpdatedDelegate ItemsUpdated;
        private DispatcherTimer MainLoopTimer { get; set; }
        private DateTime ElapsedTime { get; set; }
        private double Elapsed { get; set; }
        private double CycleMilliseconds { get; set; }
        private Dictionary<ButtonKey, bool> ButtonFlags { get; set; }
        public Camera Camera { get; set; }
        private Vec2d MousePosition { get; set; }
        private Vec2d MouseDrag { get; set; }
        private Vec2d CurrentCameraPosition { get; set; }
        #endregion

        #region Editor Variables
        private const double OrigGridSize = 64;
        private double GridSize = OrigGridSize;
        private List<Line> GridLines { get; set; }
        #endregion

        public EditorLogic(int WindowSizeWidth, int WindowSizeHeight)
        {
            WindowSize = new Vec2d(WindowSizeWidth, WindowSizeHeight);

            // Calculating frame interval with the given FPS target
            CycleMilliseconds = 1.0f / FPSTarget * 1000.0f;

            ObjectsToDisplayWorldSpace = new List<DrawableObject>();
            Objects = new List<DrawableObject>();
            GridLines = new List<Line>();

            // Creating main loop timer
            MainLoopTimer = new DispatcherTimer();
            MainLoopTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)CycleMilliseconds);
            MainLoopTimer.Tick += MainLoopTimer_Tick;
            ElapsedTime = DateTime.Now;

            // Creating the button flags dictionary
            ButtonFlags = new Dictionary<ButtonKey, bool>();
            foreach (ButtonKey b in Enum.GetValues(typeof(ButtonKey)))
                ButtonFlags.Add(b, false);


            Camera = new Camera(WindowSize)
            {
                DeadZone = new Vec2d(0, 0),
            };

            MousePosition = new Vec2d();

            //int xGrid = 0;
            //int yGrid = 0;
            //while(xGrid < WindowSize.x + GridSize*2)
            //{
            //    Line l = new Line(new Vec2d(xGrid, 0), new Vec2d(xGrid, WindowSize.y), new Color(100, 100, 100));
            //    GridLines.Add(l);
            //    Line l2 = new Line(new Vec2d(0, yGrid), new Vec2d(WindowSize.x, yGrid), new Color(100, 100, 100));
            //    GridLines.Add(l2);

            //    xGrid += GridSize;
            //    yGrid += GridSize;
            //}

            CurrentCameraPosition = WindowSize / 2;
            Camera.UpdatePosition(CurrentCameraPosition, 0);

            Rectangle r = new Rectangle(MousePosition, new Vec2d(GridSize, GridSize), Color.Red);
            r.OrigSize = r.Size;
            Objects.Add(r);
        }

        /// <summary>
        /// Game logic main enter
        /// </summary>
        public void Start()
        {
            string objectsPath = "CitySet";
            List<BitmapImage> items = new List<BitmapImage>();

            foreach(var image in new DirectoryInfo(objectsPath + "\\").GetFiles("*.png"))
            {
                BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                items.Add(bi);
            }

            //foreach (string image in Directory.GetFiles(objectsPath + "\\", "*.png"))
            //{
            //    BitmapImage bi = new BitmapImage(new Uri(image, UriKind.RelativeOrAbsolute));
            //    items.Add(bi);
            //}
            ItemsUpdated.Invoke(items);

            MainLoopTimer.Start();
        }

        public void SetCurrentTexture(BitmapImage bi)
        {
            Objects[0].Texture = bi;
        }

        /// <summary>
        /// Called by the Main Window through IGameControl
        /// <para>Sets the given button flag in the button dictionary</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isDown"></param>
        public void SetButtonFlag(ButtonKey key, bool isDown)
        {
            if (key == ButtonKey.MouseMiddle && !isDown)
            {
                CurrentCameraPosition += MouseDrag - MousePosition;
                Camera.UpdatePosition(CurrentCameraPosition, 0);
            }
            else if (key == ButtonKey.MouseMiddle && isDown)
            {
                MouseDrag = new Vec2d(MousePosition);
            }

            if(key == ButtonKey.MouseLeft && !isDown)
            {
                var toPlace = Objects[0].GetCopy();
                bool already = false;
                for (int i = 1; i < Objects.Count; i++)
                {
                    ;
                }

                //Objects.Add();
            }

            if (key == ButtonKey.Space)
                Camera.Zoom = 1;

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

        public void SetMousePosition(Vec2d position)
        {
            MousePosition = position;

            if(!ButtonFlags[ButtonKey.MouseMiddle])
            {
                Vec2d pos = CurrentCameraPosition - WindowSize / 2 + MousePosition;
                Objects[0].Position = new Vec2d(pos.x - (pos.x % GridSize), pos.y - (pos.y % GridSize));
            }
        }

        public void DeltaMouseWheel(double delta)
        {
            Camera.Zoom += delta;
            GridSize = OrigGridSize * Camera.Zoom;
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

            Control();  // Keyboard/Mouse input
            Update();   // Game logic update


            if (ButtonFlags[ButtonKey.MouseMiddle])
            {
                Camera.UpdatePosition(CurrentCameraPosition + (MouseDrag - MousePosition), Elapsed);
            }


            Objects.Sort(); // Sorting drawable objects by DrawPriority (not necessary if items added in order)
            foreach (var obj in Objects)
            {
                if(obj is Rectangle r)
                {
                    r.Size = r.OrigSize * Camera.Zoom;
                }

                ObjectsToDisplayWorldSpace.Add(obj);
            }
            //foreach (var obj in GridLines)
            //{
            //    ObjectsToDisplayWorldSpace.Add(obj);
            //}
            DrawEvent.Invoke(); // Invoking the OnRender function in the Display class through event
        }

        /// <summary>
        /// Input checking
        /// </summary>
        private void Control()
        {
            //Button control checks
            if (ButtonFlags[ButtonKey.Space])
            {
            }
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
