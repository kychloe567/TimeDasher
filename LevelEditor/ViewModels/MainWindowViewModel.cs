using LevelEditor.Logic;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SZGUIFeleves.Models;
using Ookii.Dialogs.Wpf;
using System.IO;
using LevelEditor.Models;
using SZGUIFeleves.Models.DrawableObjects;

namespace LevelEditor.ViewModels
{
    public class MainWindowViewModel : ObservableRecipient
    {
        IEditorControl logic { get; set; }

        public ObservableCollection<string> Sets { get; set; }

        private string selectedSet;
        public string SelectedSet
        {
            get { return selectedSet; }
            set
            {
                selectedSet = value;
                OnPropertyChanged();
                logic.SetChanged(selectedSet);
            }
        }

        public ObservableCollection<Image> ForegroundObjects { get; set; }
        public ObservableCollection<Image> BackgroundObjects { get; set; }
        public ObservableCollection<Image> DecorationObjects { get; set; }
        public ObservableCollection<Image> TrapObjects { get; set; }

        #region Selection Management
        private int selectedTab;
        public int SelectedTab
        {
            get { return selectedTab; }
            set
            {
                if (value == 0)
                    CurrentCustomLayer = -2;
                else if (value == 1)
                    CurrentCustomLayer = -3;
                else
                    CurrentCustomLayer = -1;

                selectedTab = value;
                SelectionChanged();
            }
        }
        private Image selectedImageForeground;
        public Image SelectedImageForeground
        {
            get { return selectedImageForeground; }
            set
            {
                selectedImageForeground = value;
                SelectionChanged();
            }
        }
        private Image selectedImageBackground;
        public Image SelectedImageBackground
        {
            get { return selectedImageBackground; }
            set
            {
                selectedImageBackground = value;
                SelectionChanged();
            }
        }
        private Image selectedImageDecoration;
        public Image SelectedImageDecoration
        {
            get { return selectedImageDecoration; }
            set
            {
                selectedImageDecoration = value;
                SelectionChanged();
            }
        }
        private Image selectedImageTrap;
        public Image SelectedImageTrap
        {
            get { return selectedImageTrap; }
            set
            {
                selectedImageTrap = value;
                SelectionChanged();
            }
        }
        private DrawableObject SelectedDrawable
        {
            get
            {
                if (SelectedTab == 0)
                {
                    if (SelectedImageForeground is null)
                        return null;
                    return SelectedImageForeground.Tag as DrawableObject;
                }
                else if (SelectedTab == 1)
                {
                    if (SelectedImageBackground is null)
                        return null;
                    return SelectedImageBackground.Tag as DrawableObject;
                }
                else if (SelectedTab == 2)
                {
                    if (SelectedImageDecoration is null)
                        return null;
                    return SelectedImageDecoration.Tag as DrawableObject;
                }
                else
                {
                    if (SelectedImageTrap is null)
                        return null;
                    return SelectedImageTrap.Tag as DrawableObject;
                }
            }
        }
        #endregion

        #region DrawPriority Management
        private DrawPriority drawLevel;
        public DrawPriority DrawLevel
        {
            get { return drawLevel; }
            set
            {
                drawLevel = value;
                OnPropertyChanged();
                SelectionChanged();
            }
        }

        private int currentCustomLayer;
        public int CurrentCustomLayer
        {
            get { return currentCustomLayer; }
            set
            {
                currentCustomLayer = value;
                DrawLevel = DrawPriority.Custom(currentCustomLayer);
                OnPropertyChanged();
            }
        }

        public ICommand TopCommand { get; set; }
        public ICommand BottomCommand { get; set; }
        public ICommand HigherCommand { get; set; }
        public ICommand LowerCommand { get; set; }

        #endregion

        #region FileMenu
        public ICommand NewCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand ExportToGameCommand { get; set; }
        #endregion

        #region EditorSettings
        private const int ObjectSize = 48;
        #endregion

        #region EditorButtons
        private bool playerEmitsLight;
        public bool PlayerEmitsLight
        {
            get { return playerEmitsLight; }
            set { playerEmitsLight = value; OnPropertyChanged(); }
        }
        private bool isItNight;
        public bool IsItNight
        {
            get { return isItNight; }
            set 
            { 
                isItNight = value;
                if (!(SelectedBackground is null))
                    logic.BackgroundChanged(SelectedBackground, isItNight);
                OnPropertyChanged(); 
            }
        }
        public ICommand MoveToolCommand { get; set; }
        public ICommand SelectionToolCommand { get; set; }
        public ICommand PlayerToolCommand { get; set; }
        public ICommand CheckpointToolCommand { get; set; }
        public ICommand EndToolCommand { get; set; }

        private Tool currentTool;
        public Tool CurrentTool
        {
            get { return currentTool; }
            set
            {
                currentTool = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Background
        public ObservableCollection<string> Backgrounds { get; set; }
        private string selectedBackground;
        public string SelectedBackground
        {
            get { return selectedBackground; }
            set
            {
                selectedBackground = value;
                if(!(selectedBackground is null))
                    logic.BackgroundChanged(selectedBackground, IsItNight);
                OnPropertyChanged();
            }
        }
        #endregion

        public MainWindowViewModel(IEditorControl logic)
        {
            this.logic = logic;

            ForegroundObjects = new ObservableCollection<Image>();
            BackgroundObjects = new ObservableCollection<Image>();
            DecorationObjects = new ObservableCollection<Image>();
            TrapObjects = new ObservableCollection<Image>();

            logic.SetsUpdated += Logic_SetsUpdated;
            logic.ItemsUpdated += Logic_ItemsUpdated;
            logic.ToolChanged += Logic_ToolChanged;

            TopCommand = new RelayCommand(
                () => DrawLevel = DrawPriority.Top);
            BottomCommand = new RelayCommand(
                () => DrawLevel = DrawPriority.Bottom);
            HigherCommand = new RelayCommand(
                () => CurrentCustomLayer++);
            LowerCommand = new RelayCommand(
                () => CurrentCustomLayer--);

            NewCommand = new RelayCommand(
                () => logic.ResetScene(true));

            LoadCommand = new RelayCommand(
                () => Load());

            SaveCommand = new RelayCommand(
                () => Save());

            ExitCommand = new RelayCommand(
                () => Application.Current.Shutdown());

            MoveToolCommand = new RelayCommand(
                () => SetCurrentTool(Tool.Move));

            SelectionToolCommand = new RelayCommand(
                () => SetCurrentTool(Tool.Selection));

            PlayerToolCommand = new RelayCommand(
                () => SetCurrentTool(Tool.Player));

            CheckpointToolCommand = new RelayCommand(
                () => SetCurrentTool(Tool.Checkpoint));

            EndToolCommand = new RelayCommand(
                () => SetCurrentTool(Tool.End));

            ExportToGameCommand = new RelayCommand(
                () => ExportToGame());

            DrawLevel = DrawPriority.Default;
            CurrentCustomLayer = -2;
        }

        private void Logic_ToolChanged(Tool tool)
        {
            CurrentTool = tool;
        }

        private void Load()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Json files|*.json";
            if (Directory.Exists(Directory.GetCurrentDirectory() + "\\Scenes\\CustomScenes"))
                ofd.InitialDirectory = Directory.GetCurrentDirectory() + "\\Scenes\\CustomScenes";

            if (ofd.ShowDialog() == true)
            {
                Scene s = SceneManager.GetSceneByFullPath(ofd.FileName);
                if(s.MovingBackground.Set != null)
                    SelectedBackground = s.MovingBackground.Set;
                logic.LoadScene(s);
            }
        }

        private void Save()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Json files|*.json";
            if (Directory.Exists(Directory.GetCurrentDirectory() + "\\Scenes\\CustomScenes"))
                sfd.InitialDirectory = Directory.GetCurrentDirectory() + "\\Scenes\\CustomScenes";

            if (sfd.ShowDialog() == true)
            {
                string title = Path.GetFileNameWithoutExtension(sfd.FileName);
                SceneManager.SaveScene(logic.SaveScene(title), sfd.FileName, PlayerEmitsLight);
            }
        }

        private void ExportToGame()
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Scenes\\CustomScenes"))
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Json files|*.json";
            
            sfd.InitialDirectory = Directory.GetCurrentDirectory() + "\\Scenes\\CustomScenes";

            if (sfd.ShowDialog() == true)
            {
                string title = Path.GetFileNameWithoutExtension(sfd.FileName);
                SceneManager.SaveScene(logic.SaveScene(title), sfd.FileName, PlayerEmitsLight);
            }
        }

        private void SetCurrentTool(Tool tool)
        {
            logic.CurrentTool = tool;
            CurrentTool = tool;
        }

        private void SelectionChanged()
        {
            DrawableObject d = SelectedDrawable;
            if (logic != null && d != null)
            {
                logic.CurrentTool = Tool.Place;
                CurrentTool = Tool.Place;
                d.DrawPriority = DrawLevel;

                if (SelectedTab == 3)
                {
                    logic.SetCurrentTexture(new Trap(d));
                }
                else
                    logic.SetCurrentTexture(d);
            }
        }

        private void Logic_SetsUpdated(List<string> sets)
        {
            Backgrounds = new ObservableCollection<string>(sets);
            Sets = new ObservableCollection<string>(sets);
            SelectedSet = Sets.First();
        }

        private void Logic_ItemsUpdated(List<DrawableObject> background, List<DrawableObject> foreground, 
                                        List<DrawableObject> decoration, List<DrawableObject> traps)
        {
            BackgroundObjects.Clear();
            ForegroundObjects.Clear();
            DecorationObjects.Clear();
            TrapObjects.Clear();

            foreach (var obj in background)
            {
                obj.Position = new Vec2d(-200, -200);
                Image i = new Image()
                {
                    Source = obj.Texture,
                    Height = ObjectSize,
                    Width = ObjectSize,
                    Tag = obj
                };

                BackgroundObjects.Add(i);
            }
            foreach (var obj in foreground)
            {
                obj.Position = new Vec2d(-200, -200);
                Image i = new Image()
                {
                    Source = obj.Texture,
                    Height = ObjectSize,
                    Width = ObjectSize,
                    Tag = obj
                };

                ForegroundObjects.Add(i);
            }

            //TODO: Size management
            foreach (var obj in decoration)
            {
                obj.Position = new Vec2d(-200, -200);
                Image i = new Image()
                {
                    Source = obj.Texture,
                    Height = ObjectSize,
                    Width = ObjectSize,
                    Tag = obj
                };

                DecorationObjects.Add(i);
            }

            foreach (var obj in traps)
            {
                obj.Position = new Vec2d(-200, -200);
                Image i = new Image()
                {
                    Source = obj.Texture,
                    Height = ObjectSize,
                    Width = ObjectSize,
                    Tag = obj
                };

                TrapObjects.Add(i);
            }
        }
    }
}
