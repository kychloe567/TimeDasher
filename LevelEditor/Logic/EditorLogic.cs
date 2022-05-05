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
using SZGUIFeleves.Helpers;
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

        private Tool currentTool;
        public Tool CurrentTool
        {
            get { return currentTool; }
            set
            {
                currentTool = value;
                ClearSelections();
            }
        }
        #endregion

        #region Tool Variables
        private DrawableObject SelectedPlacedItem { get; set; }

        private Vec2d SelectionPos { get; set; }
        private Vec2d SelectionSize { get; set; }
        private List<DrawableObject> SelectedPlacedItems { get; set; }
        private DrawableObject SelectedPlacedItemsCenter { get; set; }
        private Vec2d SelectedPlacedItemsMove { get; set; }
        private bool AllSelectedIsDecor { get; set; }
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

            SelectionPos = new Vec2d();
            SelectionSize = new Vec2d();
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
            ToolChanged.Invoke(CurrentTool);

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

            ClearSelections();
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
        /// Clears all selections to avoid problems when switching tools
        /// <para>If objects were still moving then they're put back to their original position</para>
        /// </summary>
        private void ClearSelections()
        {
            if (SelectedPlacedItems != null)
            {
                foreach (var item in SelectedPlacedItems)
                {
                    item.Position = new Vec2d(item.TempPosition);
                    item.Texture = item.TempTexture.Clone();
                    item.TempTexture = null;
                }
                SelectedPlacedItems.Clear();
                SelectedPlacedItems = null;
                SelectedPlacedItemsCenter = null;
                SelectedPlacedItemsMove = null;
            }

            if (SelectedPlacedItem != null)
            {
                SelectedPlacedItem.Position = new Vec2d(SelectedPlacedItem.TempPosition);
                SelectedPlacedItem.Texture = SelectedPlacedItem.TempTexture.Clone();
                SelectedPlacedItem.TempTexture = null;
                SelectedPlacedItem = null;
            }
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

            // If user is trying to pick a placed object (to place again)
            if(key == ButtonKey.Q)
            {
                bool found = false;
                for (int i = Objects.Count - 1; i >= 0; i--)
                {
                    if (Objects[i].Intersects(MousePositionWorldSpace))
                    {
                        SelectedItem.SelectedTexture = Objects[i].Texture.Clone();
                        SelectedItem.SelectedTextureRed = ImageColoring.SetColor(SelectedItem.SelectedTexture, ImageColoring.ColorFilters.Red);
                        SelectedItem.SelectedTextureGreen = ImageColoring.SetColor(SelectedItem.SelectedTexture, ImageColoring.ColorFilters.Green);
                        SelectedItem.Object = Objects[i].GetCopy();
                        SelectedItem.Object.Texture = SelectedItem.SelectedTexture;
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    ClearSelections();
                    CurrentTool = Tool.Place;
                    ToolChanged.Invoke(CurrentTool);
                }
            }
            // Object deletion - Delete key
            else if((CurrentTool == Tool.Move || currentTool == Tool.Selection) && key == ButtonKey.Delete && isDown)
            {
                if(CurrentTool == Tool.Move && SelectedPlacedItem != null)
                {
                    Objects.Remove(SelectedPlacedItem);
                    ClearSelections();
                }
                else if(CurrentTool == Tool.Selection && SelectedPlacedItems != null)
                {
                    foreach(var item in SelectedPlacedItems)
                        Objects.Remove(item);
                    ClearSelections();
                }
            }
            if (CurrentTool == Tool.Move)
            {
                if(isDown && key == ButtonKey.MouseLeft)
                {
                    // If there is no selected item yet
                    if (SelectedPlacedItem is null)
                    {
                        ClearSelections();

                        // Check if mouse is on an object (to move)
                        for (int i = Objects.Count - 1; i >= 0; i--)
                        {
                            if (Objects[i].Intersects(MousePositionWorldSpace))
                            {
                                Objects[i].TempPosition = new Vec2d(Objects[i].Position);
                                Objects[i].TempTexture = Objects[i].Texture.Clone();
                                Objects[i].Texture = ImageColoring.SetColor(Objects[i].Texture, ImageColoring.ColorFilters.Green);
                                SelectedPlacedItem = Objects[i];
                                break;
                            }
                        }
                    }
                    // If the mouse clicked again while moving, place it
                    else
                    {
                        SelectedPlacedItem.Texture = SelectedPlacedItem.TempTexture.Clone();
                        SelectedPlacedItem.TempTexture = null;
                        SelectedPlacedItem = null;
                    }
                }
                
            }
            else if (CurrentTool == Tool.Selection)
            {
                bool tryToMove = false;

                // Check if the mouse is on an object in the SelectedPlacedItems list.
                // Set the "center" item as the item the mouse clicked on
                if (SelectedPlacedItems != null)
                {
                    foreach (var item in SelectedPlacedItems)
                    {
                        if (item.Intersects(MousePositionWorldSpace))
                        {
                            tryToMove = true;
                            SelectedPlacedItemsCenter = item;
                            break;
                        }
                    }
                }

                // If the selection started not on an object, the user is starting to select
                if (key == ButtonKey.MouseLeft && !tryToMove)
                {
                    ClearSelections();
                    if (isDown)
                    {
                        // Start Selection rectangle
                        SelectionPos = MousePositionWorldSpace;
                    }
                    else
                    {
                        // Selection is done
                        SelectedPlacedItems = new List<DrawableObject>();

                        // Normalize selectionRect (negative size -> positive size with corresponding position)
                        Rectangle selectionRect = MathHelper.NormalizeSize(new Rectangle(SelectionPos, SelectionSize));

                        AllSelectedIsDecor = true;
                        for (int i = Objects.Count - 1; i >= 0; i--)
                        {
                            //Check for objects inside the selectionr rect
                            if (selectionRect.Intersects(Objects[i]))
                            {
                                Objects[i].TempPosition = new Vec2d(Objects[i].Position);
                                Objects[i].TempTexture = Objects[i].Texture.Clone();
                                Objects[i].Texture = ImageColoring.SetColor(Objects[i].Texture, ImageColoring.ColorFilters.Green);
                                SelectedPlacedItems.Add(Objects[i]);

                                // If there is even one not decor, the moving is grid based
                                // If all objects are decor, the moving is pixel based
                                if (Objects[i].ObjectType != DrawableObject.ObjectTypes.Decoration)
                                    AllSelectedIsDecor = false;
                            }
                        }

                        SelectionPos = new Vec2d();
                        SelectionSize = new Vec2d();
                    }
                }
                // Selection already done and the mouse is on an selected object (to move)
                else if(key == ButtonKey.MouseLeft && tryToMove)
                {
                    // The user clicked a second time, selected items moving is done, place them
                    if (!ButtonFlags[ButtonKey.MouseMiddle] && SelectedPlacedItems != null && SelectedPlacedItemsMove != null && isDown)
                    {
                        foreach (var item in SelectedPlacedItems)
                        {
                            item.Texture = item.TempTexture.Clone();
                            item.TempTexture = null;
                        }
                        SelectedPlacedItems.Clear();
                        SelectedPlacedItems = null;
                        SelectedPlacedItemsCenter = null;
                        SelectedPlacedItemsMove = null;
                    }
                    // The user clicked the first time, start to move the selected items
                    else if (!ButtonFlags[ButtonKey.MouseMiddle] && SelectedPlacedItems != null && SelectedPlacedItemsMove == null)
                    {
                        if (tryToMove && key == ButtonKey.MouseLeft)
                        {
                            SelectedPlacedItemsMove = MousePositionWorldSpace;
                        }
                        // Drop selection if the user clicks with not the left button
                        // Objects are placed back to their original position
                        else
                        {
                            foreach (var item in SelectedPlacedItems)
                            {
                                item.Position = new Vec2d(item.TempPosition);
                                item.Texture = item.TempTexture.Clone();
                                item.TempTexture = null;
                            }
                            SelectedPlacedItems = null;

                            CurrentTool = Tool.Move;
                            ToolChanged.Invoke(CurrentTool);
                        }
                    }
                }
            }
            // If user is trying to place an object
            if(CurrentTool == Tool.Place && key == ButtonKey.MouseLeft)
            {
                var toPlace = SelectedItem.Object.GetCopy();
                toPlace.Texture = SelectedItem.SelectedTexture;

                // Check if the object the user is trying to place is not on an another object
                // Decor on decor is allowed
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

        /// <summary>
        /// Sets the current mouse position in screen space to world space
        /// <para>Manages tools/movement</para>
        /// </summary>
        /// <param name="position"></param>
        public void SetMousePosition(Vec2d position)
        {
            // Screen space -> World space
            // Translating by the Camera's position
            MousePosition = position;
            MousePositionWorldSpace = CurrentCameraPosition - WindowSize / 2 + MousePosition;

            // If the user is currently moving the selected items (Tool.Selection)
            if (!ButtonFlags[ButtonKey.MouseMiddle] && SelectedPlacedItems != null && SelectedPlacedItemsMove != null)
            {
                // If there is even one not decor object in the selected items
                // Move them grid based
                if(!AllSelectedIsDecor)
                {
                    // Calculating the "center" object's grid location
                    SelectedPlacedItemsCenter.Position = new Vec2d(MousePositionWorldSpace.x - (MousePositionWorldSpace.x % GridSize),
                                                          MousePositionWorldSpace.y - (MousePositionWorldSpace.y % GridSize));

                    // Y angle is offset by -1 gridunit
                    if (MousePositionWorldSpace.x < 0) { SelectedPlacedItemsCenter.Position.x -= GridSize; }
                    if (MousePositionWorldSpace.y < 0) { SelectedPlacedItemsCenter.Position.y -= GridSize; }

                    foreach (var item in SelectedPlacedItems)
                    {
                        // Skipping the already calculated "center" object's position
                        if (item == SelectedPlacedItemsCenter)
                            continue;

                        // Calculating the current objects position relative to the center and offset by the moved position
                        Vec2d relative = item.TempPosition - SelectedPlacedItemsCenter.TempPosition;
                        item.Position = SelectedPlacedItemsCenter.Position + relative;

                        // Y angle is offset by -1 gridunit
                        if (MousePositionWorldSpace.x < 0) { item.Position.x -= GridSize; }
                        if (MousePositionWorldSpace.y < 0) { item.Position.y -= GridSize; }
                    }
                }
                // If all selected objects are decor
                // Move them pixel based
                else
                {
                    Rectangle r = SelectedPlacedItemsCenter as Rectangle;
                    SelectedPlacedItemsCenter.Position = MousePositionWorldSpace - r.Size / 2;

                    // Clamp "center" object's position the the screen
                    if (SelectedPlacedItemsCenter.Position.x < Camera.CenteredPosition.x)
                        SelectedPlacedItemsCenter.Position.x = Camera.CenteredPosition.x;
                    if (SelectedPlacedItemsCenter.Position.y < Camera.CenteredPosition.y)
                        SelectedPlacedItemsCenter.Position.y = Camera.CenteredPosition.y;
                    if (SelectedPlacedItemsCenter.Position.x + r.Size.x > Camera.CenteredPosition.x + Camera.WindowSize.x)
                        SelectedPlacedItemsCenter.Position.x = Camera.CenteredPosition.x + Camera.WindowSize.x - r.Size.x;
                    if (SelectedPlacedItemsCenter.Position.y + r.Size.y > Camera.CenteredPosition.y + Camera.WindowSize.y)
                        SelectedPlacedItemsCenter.Position.y = Camera.CenteredPosition.y + Camera.WindowSize.y - r.Size.y;

                    foreach (var item in SelectedPlacedItems)
                    {
                        // Skipping the already calculated "center" object's position
                        if (item == SelectedPlacedItemsCenter)
                            continue;

                        // Calculating the current objects position relative to the center and offset by the moved position
                        Vec2d relative = item.TempPosition - SelectedPlacedItemsCenter.TempPosition;
                        item.Position = SelectedPlacedItemsCenter.Position + relative;
                    }
                }
            }
            // If the user is currently moving the selected item (Tool.Move)
            else if (!ButtonFlags[ButtonKey.MouseMiddle] && CurrentTool == Tool.Move && SelectedPlacedItem != null)
            {
                // If selected object is not decor
                // Move it grid based
                if (SelectedPlacedItem.ObjectType != DrawableObject.ObjectTypes.Decoration)
                {
                    // Calculating selected object's grid location
                    SelectedPlacedItem.Position = new Vec2d(MousePositionWorldSpace.x - (MousePositionWorldSpace.x % GridSize),
                                                      MousePositionWorldSpace.y - (MousePositionWorldSpace.y % GridSize));

                    // Y angle is offset by -1 gridunit
                    if (MousePositionWorldSpace.x < 0) { SelectedPlacedItem.Position.x -= GridSize; }
                    if (MousePositionWorldSpace.y < 0) { SelectedPlacedItem.Position.y -= GridSize; }
                }
                // If selected object is decor
                // Move it pixel based
                else
                {
                    Rectangle r = SelectedPlacedItem as Rectangle;
                    SelectedPlacedItem.Position = MousePositionWorldSpace - r.Size / 2;

                    // Clamp "center" object's position the the screen
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
            // If the user is currently selecting
            else if (!ButtonFlags[ButtonKey.MouseMiddle] && CurrentTool == Tool.Selection && ButtonFlags[ButtonKey.MouseLeft])
            {
                SelectionSize = new Vec2d(MousePositionWorldSpace - SelectionPos);
            }
            // If the user is currently placing an object
            else if (!ButtonFlags[ButtonKey.MouseMiddle] && CurrentTool == Tool.Place && SelectedItem.Object is Rectangle r)
            {
                // If the object is not a decor
                if (r.ObjectType != DrawableObject.ObjectTypes.Decoration)
                {
                    // Calculating grid location
                    SelectedItem.Object.Position = new Vec2d(MousePositionWorldSpace.x - (MousePositionWorldSpace.x % GridSize),
                                                      MousePositionWorldSpace.y - (MousePositionWorldSpace.y % GridSize));

                    // Y angle is offset by -1 gridunit
                    if (MousePositionWorldSpace.x < 0) { SelectedItem.Object.Position.x -= GridSize; }
                    if (MousePositionWorldSpace.y < 0) { SelectedItem.Object.Position.y -= GridSize; }
                }
                else
                {
                    SelectedItem.Object.Position = MousePositionWorldSpace - r.Size / 2;

                    // Clamp position the the screen
                    if (SelectedItem.Object.Position.x < Camera.CenteredPosition.x) 
                        SelectedItem.Object.Position.x = Camera.CenteredPosition.x;
                    if (SelectedItem.Object.Position.y < Camera.CenteredPosition.y) 
                        SelectedItem.Object.Position.y = Camera.CenteredPosition.y;
                    if (SelectedItem.Object.Position.x + r.Size.x > Camera.CenteredPosition.x + Camera.WindowSize.x) 
                        SelectedItem.Object.Position.x = Camera.CenteredPosition.x + Camera.WindowSize.x - r.Size.x;
                    if (SelectedItem.Object.Position.y + r.Size.y > Camera.CenteredPosition.y + Camera.WindowSize.y) 
                        SelectedItem.Object.Position.y = Camera.CenteredPosition.y + Camera.WindowSize.y - r.Size.y;
                }

                // If placeable object is not a decor check if gridposition is already occupied
                // Change texture to red if occupied, to green if not
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

        /// <summary>
        /// Zooming
        /// <para>Not working yet</para>
        /// </summary>
        /// <param name="delta"></param>
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

            // Passing placeable item to the display
            if(CurrentTool == Tool.Place)
                ObjectsToDisplayWorldSpace.Add(SelectedItem.Object);
            // Passing selection rect to the display
            else if(CurrentTool == Tool.Selection && ButtonFlags[ButtonKey.MouseLeft])
            {
                Rectangle selectionRect = new Rectangle(SelectionPos, SelectionSize)
                {
                    IsFilled = false,
                    OutLineColor = Color.Green,
                    OutLineThickness = 1
                };
                ObjectsToDisplayWorldSpace.Add(MathHelper.NormalizeSize(selectionRect));
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
        }

        /// <summary>
        /// Logic Update
        /// </summary>
        private void Update()
        {
            // Game Logic Update
        }

        /// <summary>
        /// Clear scene and reset the camera.
        /// </summary>
        public void ResetScene()
        {
            Objects.Clear();

            CurrentCameraPosition = WindowSize / 2;
            Camera.UpdatePosition(CurrentCameraPosition, 0);

            MousePositionWorldSpace = CurrentCameraPosition - WindowSize / 2 + MousePosition;
        }

        /// <summary>
        /// Load given scene into objects
        /// </summary>
        /// <param name="s"></param>
        public void LoadScene(Scene s)
        {
            ResetScene();
            foreach (var obj in s.Objects)
                Objects.Add(obj);
        }

        /// <summary>
        /// Return the current state as a scene
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public Scene SaveScene(string title)
        {
            return new Scene(title, Objects, -1, new List<DynamicPointLight>());
        }
    }
}
