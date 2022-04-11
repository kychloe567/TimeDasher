using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using SZGUIFeleves.Models;

namespace LevelEditor.Logic
{
    public interface IEditorControl
    {
        void SetCurrentTexture(DrawableObject obj);
        void SetChanged(string currentSet);
        event SetsUpdatedDelegate SetsUpdated;
        event ItemsUpdatedDelegate ItemsUpdated;
        void ResetScene();
        void LoadScene(Scene s);
        Scene SaveScene(string title);
        void ToolChanged(Tool tool);


        void SetButtonFlag(ButtonKey key, bool isDown);
        void SetMousePosition(Vec2d position);
        void DeltaMouseWheel(double delta);
        void WindowSizeChanged(int WindowWidth, int WindowHeight);
    }
}
