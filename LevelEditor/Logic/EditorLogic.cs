using LevelEditor.Helpers;
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
    public delegate void ItemsUpdatedDelegate(List<BitmapImage> background, List<BitmapImage> foreground, List<BitmapImage> decoration);

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
        private Vec2d MousePositionWorldSpace { get; set; }
        private Vec2d MouseDrag { get; set; }
        private Vec2d CurrentCameraPosition { get; set; }
        #endregion

        #region Editor Variables
        private const double OrigGridSize = 64;
        private double GridSize = OrigGridSize;
        private DrawableObject SelectedItem { get; set; }
        private BitmapImage SelectedTexture { get; set; }
        private BitmapImage SelectedTextureRed { get; set; }
        private BitmapImage SelectedTextureGreen { get; set; }
        #endregion

        public EditorLogic(int WindowSizeWidth, int WindowSizeHeight)
        {
            WindowSize = new Vec2d(WindowSizeWidth, WindowSizeHeight);

            // Calculating frame interval with the given FPS target
            CycleMilliseconds = 1.0f / FPSTarget * 1000.0f;

            ObjectsToDisplayWorldSpace = new List<DrawableObject>();
            Objects = new List<DrawableObject>();

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

            CurrentCameraPosition = WindowSize / 2;
            Camera.UpdatePosition(CurrentCameraPosition, 0);

            MousePositionWorldSpace = CurrentCameraPosition - WindowSize / 2 + MousePosition;

            Rectangle r = new Rectangle(MousePosition, new Vec2d(GridSize, GridSize))
            {
                DrawPriority = DrawPriority.Top
            };
            r.OrigSize = r.Size;
            SelectedItem = r;
        }

        /// <summary>
        /// Game logic main enter
        /// </summary>
        public void Start()
        {
            string objectsPath = "Textures\\CitySet";
            List<BitmapImage> background = new List<BitmapImage>();
            List<BitmapImage> foreground = new List<BitmapImage>();
            List<BitmapImage> decoration = new List<BitmapImage>();

            foreach(var image in new DirectoryInfo(objectsPath + "\\Background\\").GetFiles("*.png"))
            {
                BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                background.Add(bi);
            }
            foreach(var image in new DirectoryInfo(objectsPath + "\\Foreground\\").GetFiles("*.png"))
            {
                BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                foreground.Add(bi);
            }
            //foreach(var image in new DirectoryInfo(objectsPath + "\\Decoration\\").GetFiles("*.png"))
            //{
            //    BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
            //    decoration.Add(bi);
            //}

            ItemsUpdated.Invoke(background, foreground, decoration);

            MainLoopTimer.Start();
        }

        public void SetCurrentTexture(BitmapImage bi)
        {
            SelectedTexture = bi;
            SelectedTextureRed = ImageColoring.SetColor(bi, ImageColoring.ColorFilters.Red);
            SelectedTextureGreen = ImageColoring.SetColor(bi, ImageColoring.ColorFilters.Green);
            SelectedItem.Texture = bi;
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
            MousePositionWorldSpace = CurrentCameraPosition - WindowSize / 2 + MousePosition;

            if (!ButtonFlags[ButtonKey.MouseMiddle])
            {
                SelectedItem.Position = new Vec2d(MousePositionWorldSpace.x - (MousePositionWorldSpace.x % GridSize),
                                                  MousePositionWorldSpace.y - (MousePositionWorldSpace.y % GridSize));

                if (MousePositionWorldSpace.x < 0)
                {
                    SelectedItem.Position.x -= GridSize;
                }
                if (MousePositionWorldSpace.y < 0)
                {
                    SelectedItem.Position.y -= GridSize;
                }
            }

            if (SelectedItem != null && SelectedTexture != null)
            {
                bool already = false;
                Circle checkCircle = new Circle(MousePositionWorldSpace, 1);

                foreach (var obj in Objects)
                {
                    if (obj.Intersects(checkCircle))
                    {
                        SelectedItem.Texture = SelectedTextureRed;
                        already = true;
                        break;
                    }
                }

                if (!already && !SelectedItem.Texture.Equals(SelectedTextureGreen))
                    SelectedItem.Texture = SelectedTextureGreen;
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

            Objects.Sort(); // Sorting drawable objects by DrawPriority (not necessary if items added in order)
            foreach (var obj in Objects)
            {
                if(obj is Rectangle r)
                {
                    r.Size = r.OrigSize * Camera.Zoom;
                }

                ObjectsToDisplayWorldSpace.Add(obj);
            }
            ObjectsToDisplayWorldSpace.Add(SelectedItem);

            DrawEvent.Invoke(); // Invoking the OnRender function in the Display class through event
        }

        /// <summary>
        /// Input checking
        /// </summary>
        private void Control()
        {
            //Button control checks
            if (ButtonFlags[ButtonKey.MouseMiddle])
            {
                Camera.UpdatePosition(CurrentCameraPosition + (MouseDrag - MousePosition), Elapsed);
            }

            Circle checkCircle = new Circle(MousePositionWorldSpace, 1);

            if (ButtonFlags[ButtonKey.MouseLeft])
            {
                var toPlace = SelectedItem.GetCopy();
                toPlace.Texture = SelectedTexture;
                toPlace.DrawPriority = DrawPriority.Default;

                bool already = false;
                for (int i = 0; i < Objects.Count; i++)
                {
                    if (toPlace.Intersects(Objects[i]))
                    {
                        already = true;
                        break;
                    }
                }

                if (!already)
                    Objects.Add(toPlace);
            }

            if (ButtonFlags[ButtonKey.MouseRight])
            {
                for (int i = Objects.Count - 1; i >= 0; i--)
                {
                    if (Objects[i].Intersects(checkCircle))
                    {
                        Objects.RemoveAt(i);
                        break;
                    }
                }
            }

            if(ButtonFlags[ButtonKey.Q])
            {
                for (int i = Objects.Count - 1; i >= 0; i--)
                {
                    if (Objects[i].Intersects(checkCircle))
                    {
                        SelectedTexture = Objects[i].Texture.Clone();
                        SelectedTextureRed = ImageColoring.SetColor(SelectedTexture, ImageColoring.ColorFilters.Red);
                        SelectedTextureGreen = ImageColoring.SetColor(SelectedTexture, ImageColoring.ColorFilters.Green);
                        SelectedItem.Texture = SelectedTexture;
                        break;
                    }
                }
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
