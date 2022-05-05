using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SZGUIFeleves.Controller;
using SZGUIFeleves.Logic;
using SZGUIFeleves.ViewModels;

namespace SZGUIFeleves
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameController controller;
        MainWindowViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RenderOptions.SetBitmapScalingMode(MainGrid, BitmapScalingMode.NearestNeighbor);
            GameLogic logic = new GameLogic((int)MainGrid.ActualWidth, (int)MainGrid.ActualHeight);

            ViewModel = DataContext as MainWindowViewModel;
            ViewModel.GameLogicPass(ref logic);

            display.SetupModel(logic, (int)MainGrid.ActualWidth, (int)MainGrid.ActualHeight);
            controller = new GameController(logic);
            logic.Start();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(controller != null)
                controller.WindowSizeChanged((int)MainGrid.ActualWidth, (int)MainGrid.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (controller != null && ViewModel.GameState == GameStates.Game)
                controller.KeyPressed(e.Key);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (controller != null && ViewModel.GameState == GameStates.Game)
                controller.KeyReleased(e.Key);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(controller != null && ViewModel.GameState == GameStates.Game)
            {
                Point p = e.GetPosition(MainGrid);
                if (e.ChangedButton == MouseButton.Left)
                    controller.MouseLeft(true, p.X, p.Y);
                else if (e.ChangedButton == MouseButton.Right)
                    controller.MouseRight(true, p.X, p.Y);
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (controller != null && ViewModel.GameState == GameStates.Game)
            {
                Point p = e.GetPosition(MainGrid);
                if (e.ChangedButton == MouseButton.Left)
                    controller.MouseLeft(false, p.X, p.Y);
                else if (e.ChangedButton == MouseButton.Right)
                    controller.MouseRight(false, p.X, p.Y);
            }
        }
    }
}
