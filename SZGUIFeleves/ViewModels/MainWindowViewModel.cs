using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
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
        Menu, Leaderboard, Game, Pause
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

        private void Menu()
        {
            GameState = GameStates.Menu;
            GameLogic.CurrentState = GameStates.Pause;
            GameLogic.SceneTimer.Stop();
        }

        private void ResumeGame()
        {
            GameState = GameStates.Game;
            GameLogic.CurrentState = GameState;
            GameLogic.SceneTimer.Start();
        }

        private void StartGame()
        {
            GameState = GameStates.Game;
            CurrentScene = "tree";

            GameLogic.SetScene(CurrentScene);
            GameLogic.CurrentState = GameState;
            GameLogic.Start();
            GameLogic.SceneTimer.Start();
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
            
        }

        private void ExitGame()
        {
            Application.Current.Shutdown();
        }
    }
}
