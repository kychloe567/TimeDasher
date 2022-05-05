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

namespace SZGUIFeleves.ViewModels
{
    public enum GameStates
    {
        Menu, Game
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
        public ICommand ExitButtonCommand { get; set; }

        public MainWindowViewModel()
        {
            GameState = GameStates.Menu;
            Init();
        }

        public void Init()
        {
            StartButtonCommand = new RelayCommand(() => StartGame());
            LeaderboardButtonCommand = new RelayCommand(() => Leaderboard());
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
            //LeaderboardController.Run();
            //var a = LeaderboardController.GetAll()
        }

        private void ExitGame()
        {
            Application.Current.Shutdown();
        }
    }
}
