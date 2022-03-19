using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public class Circle : DrawableObject
    {
        public double Radius { get; set; }

        #region Constructors
        public Circle() : base() { }

        public Circle(Vec2d position, double radius) : base(position)
        {
            Radius = radius;
        }

        public Circle(Vec2d position, double radius, Color color) : base(position, color)
        {
            Radius = radius;
        }
        #endregion

        public override Vec2d GetMiddle()
        {
            return new Vec2d(Position.x, Position.y);
        }
    }
}
