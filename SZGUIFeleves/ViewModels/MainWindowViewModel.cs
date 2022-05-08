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
        Menu, Leaderboard, Game
    }

    public class MainWindowViewModel : ObservableRecipient
    {
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

        public ICommand StartButtonCommand { get; set; }
        public ICommand LeaderboardButtonCommand { get; set; }
        public ICommand BackButtonCommand { get; set; }
        public ICommand LevelEditorButtonCommand { get; set; }
        public ICommand ExitButtonCommand { get; set; }

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
        }

        public void GameLogicPass(ref GameLogic gameLogic)
        {
            GameLogic = gameLogic;
        }

        private void StartGame()
        {
            GameState = GameStates.Game;
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
