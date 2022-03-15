using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SZGUIFeleves.Models
{
    /*
    
    dc.DrawRectangle;
    dc.DrawLine;
    dc.DrawGeometry;
    dc.DrawEllipse;
    dc.DrawText;
    dc.DrawImage;
    dc.DrawVideo;

    */

    public abstract class DrawableObject
    {
        public Vec2d Position { get; set; }
        public double OutLineThickness { get; set; }
        public Color OutLineColor { get; set; }
        public Color Color { get; set; }
        public bool IsFilled { get; set; }
        public BitmapImage Texture { get; set; }

        public DrawableObject()
        {
            Position = new Vec2d();
            Color = new Color();
            OutLineThickness = 1.0f;
            OutLineColor = new Color();
            IsFilled = true;
        }

        public DrawableObject(Vec2d position)
        {
            Position = position;
            Color = new Color();
            OutLineThickness = 1.0f;
            OutLineColor = new Color();
            IsFilled = true;
        }

        public DrawableObject(Vec2d position, Color color)
        {
            Position = position;
            Color = color;
            OutLineThickness = 1.0f;
            OutLineColor = color;
            IsFilled = true;
        }

        public abstract Vec2d GetMiddle();
    }
}
