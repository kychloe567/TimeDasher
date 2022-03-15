using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Logic
{
    public interface IGameControl
    {
        void SetButtonFlag(ButtonKey key, bool isDown);
        void WindowSizeChanged(int WindowWidth, int WindowHeight);
    }
}
