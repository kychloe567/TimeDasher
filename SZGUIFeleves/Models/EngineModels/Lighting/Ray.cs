using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Helpers;

namespace SZGUIFeleves.Models
{
    public class Ray
    {
        public Vec2d Position { get; set; }
        public Vec2d Direction { get; set; }
        public double Angle { get; set; }

        public Ray()
        {
            Position = new Vec2d();
            Direction = new Vec2d();
        }

        public Ray(Vec2d position, Vec2d direction)
        {
            this.Position = position;
            this.Direction = direction;
            this.Angle = direction.Angle;
        }

        public Ray(Vec2d position, double angle)
        {
            this.Position = position;
            double rad = MathHelper.ConvertToRadians(angle);
            this.Direction = new Vec2d(Math.Cos(rad), Math.Sin(rad));
            this.Angle = angle;
        }


        public Vec2d Cast(Line line)
        {
            var v1 = Position - line.Position;
            var v2 = line.Position2 - line.Position;
            var v3 = new Vec2d(-Direction.Y, Direction.X);

            var dot = Vec2d.DotProduct(v2,v3);
            if (Math.Abs(dot) < 0.000001)
                return null;

            var t1 = Vec2d.CrossProduct(v2, v1) / dot;
            var t2 = Vec2d.DotProduct(v1, v3) / dot;

            if (t1 >= 0.0 && (t2 >= 0.0 && t2 <= 1.0))
                return new Vec2d(Position + (Direction * t1)) { Temp = t1 };

            return null;
        }
    }
}
