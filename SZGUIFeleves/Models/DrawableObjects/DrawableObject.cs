using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SZGUIFeleves.Models
{
    public abstract class DrawableObject : IComparable
    {
        public Vec2d Position { get; set; }
        public double OutLineThickness { get; set; }
        public Color OutLineColor { get; set; }
        public Color Color { get; set; }
        public bool IsFilled { get; set; }
        public BitmapImage Texture { get; set; }
        public DrawPriority DrawPriority { get; set; }

        public DrawableObject()
        {
            Position = new Vec2d();
            Color = new Color();
            OutLineThickness = 1.0f;
            OutLineColor = new Color();
            IsFilled = true;
            DrawPriority = DrawPriority.Default;
        }

        public DrawableObject(Vec2d position)
        {
            Position = position;
            Color = new Color();
            OutLineThickness = 1.0f;
            OutLineColor = new Color();
            IsFilled = true;
            DrawPriority = DrawPriority.Default;
        }

        public DrawableObject(Vec2d position, Color color)
        {
            Position = position;
            Color = color;
            OutLineThickness = 1.0f;
            OutLineColor = color;
            IsFilled = true;
            DrawPriority = DrawPriority.Default;
        }

        public abstract Vec2d GetMiddle();

        public int CompareTo(object obj)
        {
            if (obj is null || !(obj is DrawableObject))
                return 1;
            else if (this is null || !(this is DrawableObject))
                return -1;

            return DrawPriority.CompareTo((obj as DrawableObject).DrawPriority);
        }
    }
}
