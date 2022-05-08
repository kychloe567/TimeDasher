using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using LevelEditor.Logic;
using LevelEditor.Models;
using SZGUIFeleves.Models;

namespace LevelEditor.Controller
{
    class GameController
    {
        IEditorControl controller;

        public GameController(IEditorControl controller)
        {
            this.controller = controller;
        }

        public void SetCurrentTexture(DrawableObject obj)
        {
            controller.SetCurrentTexture(obj);
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

        public void MouseMoved(double x, double y)
        {
            controller.SetMousePosition(new Vec2d(x,y));
        }

        public void SetMouseLeftButton(bool isDown)
        {
            try
            {
                controller.SetButtonFlag(ButtonKey.MouseLeft, isDown);
            }
            catch (Exception exc) { }
        }

        public void SetMouseRightButton(bool isDown)
        {
            try
            {
                controller.SetButtonFlag(ButtonKey.MouseRight, isDown);
            }
            catch (Exception) { }
        }

        public void SetMouseMiddleButton(bool isDown)
        {
            try
            {
                controller.SetButtonFlag(ButtonKey.MouseMiddle, isDown);
            }
            catch (Exception) { }
        }

        public void DeltaMouseWheel(double delta)
        {
            controller.DeltaMouseWheel(delta);
        }

        public void WindowSizeChanged(int WindowWidth, int WindowHeight)
        {
            controller.WindowSizeChanged(WindowWidth, WindowHeight);
        }
    }
}
