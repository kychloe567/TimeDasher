using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public class Shadow : Polygon
    {
        public Shadow(Vec2d position, List<Vec2d> points) : base(position, points)
        {
            Color = new Color(255, 255, 255, 150);
            OutLineColor = new Color(0, 0, 0, 0);
            OutLineThickness = 0;
            DrawPriority = DrawPriority.Bottom;
        }

        public Shadow(Vec2d position, List<Vec2d> points, Color color) : base(position, points, color)
        {
            OutLineColor = new Color(0, 0, 0, 0);
            OutLineThickness = 0;
            DrawPriority = DrawPriority.Bottom;
        }
    }
}
