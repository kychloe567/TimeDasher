using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public class Ellipse : DrawableObject
    {
        public Vec2d Radius { get; set; }

        #region Constructors
        public Ellipse() : base()
        {
            Radius = new Vec2d();
        }

        public Ellipse(Vec2d position, Vec2d radius) : base(position)
        {
            Radius = radius;
        }

        public Ellipse(Vec2d position, Vec2d radius, Color color) : base(position, color)
        {
            Radius = radius;
        }

        public Ellipse(Vec2d position, double radius) : base(position)
        {
            Radius = new Vec2d(radius,radius);
        }

        public Ellipse(Vec2d position, double radius, Color color) : base(position, color)
        {
            Radius = new Vec2d(radius, radius);
        }
        #endregion

        public override Vec2d GetMiddle()
        {
            return new Vec2d(Position.x, Position.y);
        }
    }
}
