using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public class Rectangle : DrawableObject
    {
        public Vec2d Size { get; set; }

        #region Constructors
        public Rectangle() : base()
        {
            Size = new Vec2d();
        }

        public Rectangle(Vec2d position, Vec2d size) : base(position)
        {
            Size = size;
        }

        public Rectangle(Vec2d position, Vec2d size, Color color) : base(position, color)
        {
            Size = size;
        }
        #endregion

        public override Vec2d GetMiddle()
        {
            return new Vec2d(Position.x + (Size.x/2), Position.y + (Size.y / 2));
        }
    }
}
