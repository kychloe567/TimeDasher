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
using LevelEditor.Controller;
using LevelEditor.Logic;
using LevelEditor.ViewModels;
using SZGUIFeleves.Models;

namespace LevelEditor
{
    public partial class MainWindow : Window
    {
        GameController controller;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RenderOptions.SetBitmapScalingMode(MainGrid, BitmapScalingMode.NearestNeighbor);

            EditorLogic logic = new EditorLogic((int)MainGrid.ActualWidth, (int)MainGrid.ActualHeight);

            display.SetupModel(logic, (int)MainGrid.ActualWidth, (int)MainGrid.ActualHeight);
            controller = new GameController(logic);

            DataContext = new MainWindowViewModel(logic);

            logic.Start();
        }

        // Events ----------------------------------------------------------------------------
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (controller != null)
                controller.WindowSizeChanged((int)MainGrid.ActualWidth, (int)MainGrid.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (controller != null)
                controller.KeyPressed(e.Key);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (controller != null)
                controller.KeyReleased(e.Key);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (controller != null)
            {
                var pos = e.GetPosition(MainGrid);
                controller.MouseMoved(pos.X, pos.Y);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (controller != null)
            {
                if (e.ChangedButton == MouseButton.Left)
                    controller.SetMouseLeftButton(true);
                else if (e.ChangedButton == MouseButton.Right)
                    controller.SetMouseRightButton(true);
                else if (e.ChangedButton == MouseButton.Middle)
                    controller.SetMouseMiddleButton(true);
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (controller != null)
            {
                if (e.ChangedButton == MouseButton.Left)
                    controller.SetMouseLeftButton(false);
                else if (e.ChangedButton == MouseButton.Right)
                    controller.SetMouseRightButton(false);
                else if (e.ChangedButton == MouseButton.Middle)
                    controller.SetMouseMiddleButton(false);
            }
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (controller != null)
            {
                controller.DeltaMouseWheel(e.Delta/1000.0);
            }
        }
    }
}
