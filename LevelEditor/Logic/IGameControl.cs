using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Models;

namespace LevelEditor.Logic
{
    public interface IGameControl
    {
        void SetButtonFlag(ButtonKey key, bool isDown);
        void SetMousePosition(Vec2d position);
        void DeltaMouseWheel(double delta);
        void WindowSizeChanged(int WindowWidth, int WindowHeight);
    }
}
