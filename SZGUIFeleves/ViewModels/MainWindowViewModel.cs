using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SZGUIFeleves.Controller;
using SZGUIFeleves.Logic;
using SZGUIFeleves.Models;

namespace SZGUIFeleves.ViewModels
{
    public enum GameStates
    {
        Menu, LevelSelection, Leaderboard, Game, Pause
    }

    public class MainWindowViewModel : ObservableRecipient
    {

        #region GameLogic
        private GameStates gameState;
        public GameStates GameState
        {
            get { return gameState; }
            set
            {
                gameState = value;
                OnPropertyChanged();
            }
        }

        private GameLogic GameLogic { get; set; }

        private string currentScene;
        public string CurrentScene
        {
            get { return currentScene; }
            set
            {
                currentScene = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Buttons
        public ICommand StartButtonCommand { get; set; }
        public ICommand LeaderboardButtonCommand { get; set; }
        public ICommand BackButtonCommand { get; set; }
        public ICommand LevelEditorButtonCommand { get; set; }
        public ICommand ExitButtonCommand { get; set; }

        public ICommand MenuButtonCommand { get; set; }
        public ICommand ResumeButtonCommand { get; set; }

        public ICommand BackToMenuCommand { get; set; }
        #endregion

        public ICommand StartSceneCommand { get; set; }

        public ICommand StartCustomSceneCommand { get; set; }

        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; OnPropertyChanged(); }
        }

        #region BestTimeTexts
        private string bestTimeStreet1;
        public string BestTimeStreet1
        {
            get { return bestTimeStreet1; }
            set { bestTimeStreet1 = value; OnPropertyChanged(); }
        }
        private string bestTimeStreet2;
        public string BestTimeStreet2
        {
            get { return bestTimeStreet2; }
            set { bestTimeStreet2 = value; OnPropertyChanged(); }
        }
        private string bestTimeAsia1;
        public string BestTimeAsia1
        {
            get { return bestTimeAsia1; }
            set { bestTimeAsia1 = value; OnPropertyChanged(); }
        }
        private string bestTimeAsia2;
        public string BestTimeAsia2
        {
            get { return bestTimeAsia2; }
            set { bestTimeAsia2 = value; OnPropertyChanged(); }
        }
        private string bestTimeMarket1;
        public string BestTimeMarket1
        {
            get { return bestTimeMarket1; }
            set { bestTimeMarket1 = value; OnPropertyChanged(); }
        }
        private string bestTimeMarket2;
        public string BestTimeMarket2
        {
            get { return bestTimeMarket2; }
            set { bestTimeMarket2 = value; OnPropertyChanged(); }
        }
        private string bestTimeJungle1;
        public string BestTimeJungle1
        {
            get { return bestTimeJungle1; }
            set { bestTimeJungle1 = value; OnPropertyChanged(); }
        }
        private string bestTimeJungle2;
        public string BestTimeJungle2
        {
            get { return bestTimeJungle2; }
            set { bestTimeJungle2 = value; OnPropertyChanged(); }
        }
        #endregion

        #region Leaderboard
        private Dictionary<string, List<LeaderboardScore>> leaderBoard;
        public Dictionary<string, List<LeaderboardScore>> LeaderBoard
        {
            get { return leaderBoard; }
            set
            {
                leaderBoard = new Dictionary<string, List<LeaderboardScore>>(value);
                OnPropertyChanged();
            }
        }

        private int selectedLeaderboardTab;
        public int SelectedLeaderboardTab
        {
            get { return selectedLeaderboardTab; }
            set
            {
                selectedLeaderboardTab = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            GameState = GameStates.Menu;
            Init();
        }

        public void Init()
        {
            LeaderboardController.Run();

            StartButtonCommand = new RelayCommand(() => StartGame());
            LeaderboardButtonCommand = new RelayCommand(() => Leaderboard());
            LevelEditorButtonCommand = new RelayCommand(() => StartLevelEditor());
            BackButtonCommand = new RelayCommand(() => GameState = GameStates.Menu);
            ExitButtonCommand = new RelayCommand(() => ExitGame());

            ResumeButtonCommand = new RelayCommand(() => ResumeGame());
            MenuButtonCommand = new RelayCommand(() => Menu());

            BackToMenuCommand = new RelayCommand(() => Menu(true));

            StartSceneCommand = new RelayCommand<string>((string scene) => StartLevel(scene));
            StartCustomSceneCommand = new RelayCommand<string>((string scene) => StartCustomLevel());
        }


        public void GameLogicPass(ref GameLogic gameLogic)
        {
            GameLogic = gameLogic;
            GameLogic.ChangeState += ChangeState;
        }
        public void ChangeState(GameStates state)
        {
            GameState = state;
            if(state == GameStates.Game)
            {
                GameLogic.SceneTimer.Start();
            }
        }

        private void Menu(bool onlyMenu = false)
        {
            GameState = GameStates.Menu;
            if (!onlyMenu)
            {
                GameLogic.CurrentState = GameStates.Pause;
                GameLogic.SceneTimer.Stop();
            }
        }

        private void ResumeGame()
        {
            GameState = GameStates.Game;
            GameLogic.CurrentState = GameState;
            GameLogic.MainLoopTimer.Start();
            GameLogic.SceneTimer.Start();
        }

        private void StartGame()
        {
            GameState = GameStates.LevelSelection;
            
            if(!(Username is null) && Username != "")
            {
                var scores = LeaderboardController.GetAll(Username);

                if (scores.ContainsKey("street1"))
                    BestTimeStreet1 = scores["street1"].Timespan;
                else
                    BestTimeStreet1 = "-";
                if (scores.ContainsKey("street2"))
                    BestTimeStreet2 = scores["street2"].Timespan;
                else
                    BestTimeStreet2 = "-";

                if (scores.ContainsKey("asia1"))
                    BestTimeAsia1 = scores["asia1"].Timespan;
                else
                    BestTimeAsia1 = "-";
                if (scores.ContainsKey("asia2"))
                    BestTimeAsia2 = scores["asia2"].Timespan;
                else
                    BestTimeAsia2 = "-";

                if (scores.ContainsKey("market1"))
                    BestTimeMarket1 = scores["market1"].Timespan;
                else
                    BestTimeMarket1 = "-";
                if (scores.ContainsKey("market2"))
                    BestTimeMarket2 = scores["market2"].Timespan;
                else
                    BestTimeMarket2 = "-";

                if (scores.ContainsKey("jungle1"))
                    BestTimeJungle1 = scores["jungle1"].Timespan;
                else
                    BestTimeJungle1 = "-";
                if (scores.ContainsKey("jungle2"))
                    BestTimeJungle2 = scores["jungle2"].Timespan;
                else
                    BestTimeJungle2 = "-";
            }
            else
            {
                BestTimeStreet1 = "-";
                BestTimeStreet2 = "-";
                BestTimeAsia1 = "-";
                BestTimeAsia2 = "-";
                BestTimeMarket1 = "-";
                BestTimeMarket2 = "-";
                BestTimeJungle1 = "-";
                BestTimeJungle2 = "-";
            }
        }

        private void StartLevel(string scene, bool custom = false)
        {
            if (!custom)
            {
                if (!File.Exists(Directory.GetCurrentDirectory() + "\\Scenes\\" + scene + ".json"))
                    return;
            }
            else
            {
                if (!File.Exists(Directory.GetCurrentDirectory() + "\\Scenes\\CustomScenes\\" + scene + ".json"))
                    return;
            }

            CurrentScene = scene;

            GameLogic.SetScene(CurrentScene);
            GameState = GameStates.Game;
            GameLogic.CurrentState = GameState;
            GameLogic.Start();
            GameLogic.SceneTimer.Start();
        }

        private void StartCustomLevel()
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Scenes\\CustomScenes"))
                return;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Json files|*.json";
            ofd.InitialDirectory = Directory.GetCurrentDirectory() + "\\Scenes\\CustomScenes";

            if (ofd.ShowDialog() == true)
            {
                StartLevel(Path.GetFileNameWithoutExtension(ofd.FileName), true);
            }
        }

        private void Leaderboard()
        {
            GameState = GameStates.Leaderboard;

            //LeaderboardController.DeleteAll();
            //Random rnd = new Random((int)DateTime.Now.Ticks);
            //for (int i = 0; i < 50; i++)
            //{
            //    int r = rnd.Next(0, 100000)/100;
            //    LeaderboardController.AddNew(new LeaderboardScore()
            //    {
            //        Name = "Teszt",
            //        Date = DateTime.Now,
            //        SceneTitle = "Teszt",
            //        Seconds = r
            //    });

            //}

            var unordered = LeaderboardController.GetAll();
            foreach(KeyValuePair<string,List<LeaderboardScore>> ls in unordered)
            {
                unordered[ls.Key] = unordered[ls.Key].OrderBy(x => x.Seconds).Take(15).ToList();
            }
            LeaderBoard = unordered;
            if(LeaderBoard.Count > 0)
                SelectedLeaderboardTab = 0;
        }

        private void StartLevelEditor()
        {
            try { Process.Start("LevelEditor.exe"); }
            catch { }
        }

        private void ExitGame()
        {
            Application.Current.Shutdown();
        }
    }
}
