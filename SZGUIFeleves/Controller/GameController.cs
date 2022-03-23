using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SZGUIFeleves.Logic;

namespace SZGUIFeleves.Controller
{
    class GameController
    {
        IGameControl controller;

        public GameController(IGameControl controller)
        {
            this.controller = controller;
        }

        public void KeyPressed(Key key)
        {
            try
            {
                controller.SetButtonFlag((ButtonKey)Enum.Parse(typeof(ButtonKey), key.ToString()), true);
            }
            catch (Exception) { }
        }

        public void KeyReleased(Key key)
        {
            try
            {
                controller.SetButtonFlag((ButtonKey)Enum.Parse(typeof(ButtonKey), key.ToString()), false);
            }
            catch (Exception) { }
        }

        public void MouseLeft(bool isDown, double x, double y)
        {
            controller.SetMousePosition(x, y);
            controller.SetButtonFlag(ButtonKey.MouseLeft, isDown);
        }

        public void MouseRight(bool isDown, double x, double y)
        {
            controller.SetMousePosition(x, y);
            controller.SetButtonFlag(ButtonKey.MouseRight, isDown);
        }

        public void WindowSizeChanged(int WindowWidth, int WindowHeight)
        {
            controller.WindowSizeChanged(WindowWidth, WindowHeight);
        }
    }
}
