using LevelEditor.Helpers;
using LevelEditor.Models;
using Newtonsoft.Json;
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
    public delegate void ItemsUpdatedDelegate(List<DrawableObject> background, List<DrawableObject> foreground, List<DrawableObject> decoration);
    public delegate void SetsUpdatedDelegate(List<string> sets);
    public delegate void ToolChangedDelegate(Tool tool);

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
        public event SetsUpdatedDelegate SetsUpdated;
        public event ItemsUpdatedDelegate ItemsUpdated;
        public event ToolChangedDelegate ToolChanged;
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
        private const int GridSize = 64;
        private const int ObjectSizeMult = GridSize/32;
        private SelectedItem SelectedItem { get; set; }
        private bool AlreadyDeleted { get; set; }
        public Tool CurrentTool { get; set; }
        #endregion

        #region Tool Variables
        public DrawableObject SelectedPlacedItem { get; set; }

        public Vec2d SelectionPos { get; set; }
        public Vec2d SelectionSize { get; set; }
        public List<DrawableObject> SelectedPlacedItems { get; set; }
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


            Camera = new Camera(WindowSize);

            MousePosition = new Vec2d();

            CurrentCameraPosition = WindowSize / 2;
            Camera.UpdatePosition(CurrentCameraPosition, 0);

            MousePositionWorldSpace = CurrentCameraPosition - WindowSize / 2 + MousePosition;

            // Place tool item
            Rectangle r = new Rectangle(MousePosition, new Vec2d(GridSize, GridSize))
            {
                DrawPriority = DrawPriority.Top
            };
            SelectedItem = new SelectedItem() { Object = r };

            CurrentTool = Tool.Move;

            SelectedPlacedItems = new List<DrawableObject>();
        }

        /// <summary>
        /// Game logic main enter
        /// </summary>
        public void Start()
        {
            // Get all the sets in the Textures folder
            var sets = Directory.GetDirectories("Textures\\").ToList();
            for (int i = 0; i < sets.Count; i++)
                sets[i] = Path.GetFileName(sets[i]);

            // Tell the viewmodel to update the setlist
            SetsUpdated.Invoke(sets);

            MainLoopTimer.Start();
        }

        /// <summary>
        /// Viewmodel calls this when the user selects an object
        /// </summary>
        /// <param name="obj"></param>
        public void SetCurrentTexture(DrawableObject obj)
        {
            // Set selected (placeable) item
            SelectedItem.SelectedTexture = obj.Texture;

            // Calculate red and green textures
            SelectedItem.SelectedTextureRed = ImageColoring.SetColor(obj.Texture, ImageColoring.ColorFilters.Red);
            SelectedItem.SelectedTextureGreen = ImageColoring.SetColor(obj.Texture, ImageColoring.ColorFilters.Green);

            // Copy the selected object
            SelectedItem.Object = (obj as Rectangle).GetCopy();
            SelectedItem.Object.Texture = obj.Texture;
        }

        /// <summary>
        /// This gets called whenever the user changes the current set
        /// <para>Loads in the textures from the given set</para>
        /// </summary>
        /// <param name="currentSet"></param>
        private void LoadSet(string currentSet)
        {
            string objectsPath = "Textures\\";
            objectsPath += currentSet;

            List<DrawableObject> background = new List<DrawableObject>();
            List<DrawableObject> foreground = new List<DrawableObject>();
            List<DrawableObject> decoration = new List<DrawableObject>();

            // Static objects
            #region Textures
            foreach (var image in new DirectoryInfo(objectsPath + "\\Background\\").GetFiles("*.png").OrderBy(x => x.Name, new TextureComparer()))
            {
                BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                Rectangle r = new Rectangle();
                r.Texture = bi;
                r.TexturePath = image.FullName;
                r.Size = new Vec2d(bi.PixelWidth * ObjectSizeMult, bi.PixelHeight * ObjectSizeMult);
                r.ObjectType = DrawableObject.ObjectTypes.Background;
                background.Add(r);
            }
            foreach (var image in new DirectoryInfo(objectsPath + "\\Foreground\\").GetFiles("*.png").OrderBy(x => x.Name, new TextureComparer()))
            {
                BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                Rectangle r = new Rectangle();
                r.Texture = bi;
                r.TexturePath = image.FullName;
                r.Size = new Vec2d(bi.PixelWidth * ObjectSizeMult, bi.PixelHeight * ObjectSizeMult);
                r.ObjectType = DrawableObject.ObjectTypes.Foreground;
                foreground.Add(r);
            }
            foreach (var image in new DirectoryInfo(objectsPath + "\\Decoration\\").GetFiles("*.png").OrderBy(x => x.Name, new TextureComparer()))
            {
                BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                Rectangle r = new Rectangle();
                r.Texture = bi;
                r.TexturePath = image.FullName;
                r.Size = new Vec2d(bi.PixelWidth * ObjectSizeMult, bi.PixelHeight * ObjectSizeMult);
                r.ObjectType = DrawableObject.ObjectTypes.Decoration;
                decoration.Add(r);
            }
            #endregion

            // Animated objects
            #region Animations
            foreach (var ani in new DirectoryInfo(objectsPath + "\\Background\\").GetDirectories())
            {
                bool first = true;

                Rectangle r = new Rectangle();
                Animation a = new Animation(ani.Name);

                foreach (var image in new DirectoryInfo(ani.FullName).GetFiles("*.png").OrderBy(x => x.Name, new TextureComparer()))
                {
                    BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                    if (first)
                    {
                        first = false;
                        r.Texture = bi;
                        r.TexturePath = image.FullName;
                        r.Size = new Vec2d(bi.PixelWidth * ObjectSizeMult, bi.PixelHeight * ObjectSizeMult);
                        r.ObjectType = DrawableObject.ObjectTypes.Background;
                    }

                    // TODO: animation time in file?
                    a.AddTexture(bi, image.FullName, 0.2);
                }

                r.StateMachine = new StateMachine(a);

                background.Add(r);
            }
            foreach (var ani in new DirectoryInfo(objectsPath + "\\Foreground\\").GetDirectories())
            {
                bool first = true;

                Rectangle r = new Rectangle();
                Animation a = new Animation(ani.Name);

                foreach (var image in new DirectoryInfo(ani.FullName).GetFiles("*.png").OrderBy(x => x.Name, new TextureComparer()))
                {
                    BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                    if (first)
                    {
                        first = false;
                        r.Texture = bi;
                        r.TexturePath = image.FullName;
                        r.Size = new Vec2d(bi.PixelWidth * ObjectSizeMult, bi.PixelHeight * ObjectSizeMult);
                        r.ObjectType = DrawableObject.ObjectTypes.Foreground;
                    }

                    // TODO: animation time in file?
                    a.AddTexture(bi, image.FullName, 0.2);
                }

                r.StateMachine = new StateMachine(a);

                foreground.Add(r);
            }
            foreach (var ani in new DirectoryInfo(objectsPath + "\\Decoration\\").GetDirectories())
            {
                bool first = true;

                Rectangle r = new Rectangle();
                Animation a = new Animation(ani.Name);

                foreach (var image in new DirectoryInfo(ani.FullName).GetFiles("*.png").OrderBy(x => x.Name, new TextureComparer()))
                {
                    BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                    if (first)
                    {
                        first = false;
                        r.Texture = bi;
                        r.TexturePath = image.FullName;
                        r.Size = new Vec2d(bi.PixelWidth * ObjectSizeMult, bi.PixelHeight * ObjectSizeMult);
                        r.ObjectType = DrawableObject.ObjectTypes.Decoration;
                    }

                    // TODO: animation time in file?
                    a.AddTexture(bi, image.FullName, 0.2);
                }

                r.StateMachine = new StateMachine(a);

                decoration.Add(r);
            }
            #endregion

            // Send textures to the viewmodel
            ItemsUpdated.Invoke(background, foreground, decoration);
        }

        /// <summary>
        /// Viewmodel calls this when the user changes the current set
        /// </summary>
        /// <param name="currentSet"></param>
        public void SetChanged(string currentSet)
        {
            LoadSet(currentSet);
        }

        /// <summary>
        /// Called by the Main Window through IGameControl
        /// <para>Sets the given button flag in the button dictionary</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isDown"></param>
        public void SetButtonFlag(ButtonKey key, bool isDown)
        {
            // View panning
            if (key == ButtonKey.MouseMiddle && !isDown)
            {
                CurrentCameraPosition += MouseDrag - MousePosition;
                Camera.UpdatePosition(CurrentCameraPosition, 0);
            }
            else if (key == ButtonKey.MouseMiddle && isDown)
            {
                MouseDrag = new Vec2d(MousePosition);
            }

            if (CurrentTool == Tool.Move)
            {
                if(isDown && key == ButtonKey.MouseLeft)
                {
                    // Check if mouse is on an object (to move)
                    if (SelectedPlacedItem is null)
                    {
                        for (int i = Objects.Count - 1; i >= 0; i--)
                        {
                            if (Objects[i].Intersects(MousePositionWorldSpace))
                            {
                                SelectedPlacedItem = Objects[i];
                                break;
                            }
                        }
                    }
                    else
                        SelectedPlacedItem = null;
                }
                
            }
            else if (CurrentTool == Tool.Selection)
            {
                if (key == ButtonKey.MouseLeft)
                {
                    if (isDown)
                    {
                        // Start Selection rectangle
                        SelectionPos = MousePositionWorldSpace;
                    }
                    else
                    {
                        // Selection is done, check for objects inside the selection rect
                        Rectangle selectionRect = new Rectangle(SelectionPos, SelectionSize);
                        SelectedPlacedItems = new List<DrawableObject>();
                        for (int i = Objects.Count - 1; i >= 0; i--)
                        {
                            if (selectionRect.Intersects(Objects[i]))
                            {
                                SelectedPlacedItems.Add(Objects[i]);
                            }
                        }
                        CurrentTool = Tool.Move;
                        ToolChanged.Invoke(CurrentTool);
                    }
                }
            }
            else if (CurrentTool == Tool.Place)
            {
                if (key == ButtonKey.MouseRight && AlreadyDeleted && !isDown)
                    AlreadyDeleted = false;

                if (key == ButtonKey.Space)
                    Camera.Zoom = 1;
            }

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

            if(!ButtonFlags[ButtonKey.MouseMiddle] && CurrentTool == Tool.Move && SelectedPlacedItem != null)
            {
                if (SelectedPlacedItem.ObjectType != DrawableObject.ObjectTypes.Decoration)
                {
                    SelectedPlacedItem.Position = new Vec2d(MousePositionWorldSpace.x - (MousePositionWorldSpace.x % GridSize),
                                                      MousePositionWorldSpace.y - (MousePositionWorldSpace.y % GridSize));

                    if (MousePositionWorldSpace.x < 0)
                    {
                        SelectedPlacedItem.Position.x -= GridSize;
                    }
                    if (MousePositionWorldSpace.y < 0)
                    {
                        SelectedPlacedItem.Position.y -= GridSize;
                    }
                }
                else
                {
                    Rectangle r = SelectedPlacedItem as Rectangle;
                    SelectedPlacedItem.Position = MousePositionWorldSpace - r.Size / 2;
                    if (SelectedPlacedItem.Position.x < Camera.CenteredPosition.x)
                        SelectedPlacedItem.Position.x = Camera.CenteredPosition.x;
                    if (SelectedPlacedItem.Position.y < Camera.CenteredPosition.y)
                        SelectedPlacedItem.Position.y = Camera.CenteredPosition.y;
                    if (SelectedPlacedItem.Position.x + r.Size.x > Camera.CenteredPosition.x + Camera.WindowSize.x)
                        SelectedPlacedItem.Position.x = Camera.CenteredPosition.x + Camera.WindowSize.x - r.Size.x;
                    if (SelectedPlacedItem.Position.y + r.Size.y > Camera.CenteredPosition.y + Camera.WindowSize.y)
                        SelectedPlacedItem.Position.y = Camera.CenteredPosition.y + Camera.WindowSize.y - r.Size.y;
                }
            }
            else if (!ButtonFlags[ButtonKey.MouseMiddle] && CurrentTool == Tool.Selection && ButtonFlags[ButtonKey.MouseLeft])
            {
                SelectionSize = new Vec2d(MousePositionWorldSpace - SelectionPos);
            }
            else if (!ButtonFlags[ButtonKey.MouseMiddle] && CurrentTool == Tool.Place && SelectedItem.Object is Rectangle r)
            {
                if (r.ObjectType != DrawableObject.ObjectTypes.Decoration)
                {
                    SelectedItem.Object.Position = new Vec2d(MousePositionWorldSpace.x - (MousePositionWorldSpace.x % GridSize),
                                                      MousePositionWorldSpace.y - (MousePositionWorldSpace.y % GridSize));

                    if (MousePositionWorldSpace.x < 0)
                    {
                        SelectedItem.Object.Position.x -= GridSize;
                    }
                    if (MousePositionWorldSpace.y < 0)
                    {
                        SelectedItem.Object.Position.y -= GridSize;
                    }
                }
                else
                {
                    SelectedItem.Object.Position = MousePositionWorldSpace - r.Size / 2;
                    if (SelectedItem.Object.Position.x < Camera.CenteredPosition.x) 
                        SelectedItem.Object.Position.x = Camera.CenteredPosition.x;
                    if (SelectedItem.Object.Position.y < Camera.CenteredPosition.y) 
                        SelectedItem.Object.Position.y = Camera.CenteredPosition.y;
                    if (SelectedItem.Object.Position.x + r.Size.x > Camera.CenteredPosition.x + Camera.WindowSize.x) 
                        SelectedItem.Object.Position.x = Camera.CenteredPosition.x + Camera.WindowSize.x - r.Size.x;
                    if (SelectedItem.Object.Position.y + r.Size.y > Camera.CenteredPosition.y + Camera.WindowSize.y) 
                        SelectedItem.Object.Position.y = Camera.CenteredPosition.y + Camera.WindowSize.y - r.Size.y;
                }

                if (SelectedItem != null && SelectedItem.SelectedTexture != null && SelectedItem.Object.ObjectType != DrawableObject.ObjectTypes.Decoration)
                {
                    bool already = false;

                    foreach (var obj in Objects)
                    {
                        if (obj.Intersects(MousePositionWorldSpace))
                        {
                            SelectedItem.Object.Texture = SelectedItem.SelectedTextureRed;
                            already = true;
                            break;
                        }
                    }

                    if (!already && !SelectedItem.Object.Texture.Equals(SelectedItem.SelectedTextureGreen))
                        SelectedItem.Object.Texture = SelectedItem.SelectedTextureGreen;
                }
            }
        }

        public void DeltaMouseWheel(double delta)
        {
            //Camera.Zoom += delta;
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
                if (!(obj.StateMachine is null))
                    obj.StateMachine.Update();

                ObjectsToDisplayWorldSpace.Add(obj);
            }
            if(CurrentTool == Tool.Place)
                ObjectsToDisplayWorldSpace.Add(SelectedItem.Object);
            else if(CurrentTool == Tool.Selection && ButtonFlags[ButtonKey.MouseLeft])
            {
                //TODO
                if (SelectionSize is null)
                    ;
                else
                {

                    Rectangle selectionRect = new Rectangle(SelectionPos, SelectionSize)
                    {
                        IsFilled = false,
                        OutLineColor = Color.Green,
                        OutLineThickness = 1
                    };
                    ObjectsToDisplayWorldSpace.Add(selectionRect);
                }
            }

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

            if (CurrentTool == Tool.Move)
            {
                
            }
            else if (CurrentTool == Tool.Selection)
            {

            }
            else if (CurrentTool == Tool.Place)
            {
                if (ButtonFlags[ButtonKey.MouseLeft])
                {
                    var toPlace = SelectedItem.Object.GetCopy();
                    toPlace.Texture = SelectedItem.SelectedTexture;

                    bool already = false;
                    for (int i = 0; i < Objects.Count; i++)
                    {
                        if (toPlace.Intersects(Objects[i]))
                        {
                            if (toPlace.ObjectType == DrawableObject.ObjectTypes.Background &&
                               Objects[i].ObjectType == DrawableObject.ObjectTypes.Background)
                                already = true;
                            else if (toPlace.ObjectType == DrawableObject.ObjectTypes.Foreground &&
                                    Objects[i].ObjectType == DrawableObject.ObjectTypes.Foreground)
                                already = true;
                            else if (toPlace.ObjectType == DrawableObject.ObjectTypes.Decoration &&
                                    Objects[i].ObjectType == DrawableObject.ObjectTypes.Decoration &&
                                    toPlace.Texture.Equals(Objects[i].Texture))
                                already = true;
                        }
                    }

                    if (!already)
                        Objects.Add(toPlace);

                    File.WriteAllText("asd.json", JsonConvert.SerializeObject(toPlace));
                }

                if (ButtonFlags[ButtonKey.MouseRight] && !AlreadyDeleted)
                {
                    for (int i = Objects.Count - 1; i >= 0; i--)
                    {
                        if (Objects[i].Intersects(MousePositionWorldSpace))
                        {
                            Objects.RemoveAt(i);
                            AlreadyDeleted = true;
                            break;
                        }
                    }
                }

                if (ButtonFlags[ButtonKey.Q])
                {
                    for (int i = Objects.Count - 1; i >= 0; i--)
                    {
                        if (Objects[i].Intersects(MousePositionWorldSpace))
                        {
                            SelectedItem.SelectedTexture = Objects[i].Texture.Clone();
                            SelectedItem.SelectedTextureRed = ImageColoring.SetColor(SelectedItem.SelectedTexture, ImageColoring.ColorFilters.Red);
                            SelectedItem.SelectedTextureGreen = ImageColoring.SetColor(SelectedItem.SelectedTexture, ImageColoring.ColorFilters.Green);
                            SelectedItem.Object = Objects[i].GetCopy();
                            SelectedItem.Object.Texture = SelectedItem.SelectedTexture;
                            break;
                        }
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

        public void ResetScene()
        {
            Objects.Clear();

            CurrentCameraPosition = WindowSize / 2;
            Camera.UpdatePosition(CurrentCameraPosition, 0);

            MousePositionWorldSpace = CurrentCameraPosition - WindowSize / 2 + MousePosition;
        }

        public void LoadScene(Scene s)
        {
            ResetScene();
            foreach (var obj in s.Objects)
                Objects.Add(obj);
        }

        public Scene SaveScene(string title)
        {
            return new Scene(title, Objects, -1, new List<DynamicPointLight>());
        }
    }
}
