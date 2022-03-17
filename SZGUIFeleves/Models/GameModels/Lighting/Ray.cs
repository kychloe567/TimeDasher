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

        public Vec2d Cast(Rectangle rect)
        {
            double x1 = rect.Position.x;
            double y1 = rect.Position.y;
            double x2 = rect.Position.x + rect.Size.x;
            double y2 = rect.Position.y + rect.Size.y;

            double x3 = Position.x;
            double y3 = Position.y;
            double x4 = Position.x + Direction.x;
            double y4 = Position.y + Direction.y;

            double den = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (den == 0)
            {
                return null;
            }

            double t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / den;
            double u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / den;
            if (t > 0 && t < 1 && u > 0)
            {
                Vec2d pt = new Vec2d();
                pt.x = x1 + t * (x2 - x1);
                pt.y = y1 + t * (y2 - y1);
                return pt;
            }
            else
                return null;
        }

        public Vec2d Cast(Line line)
        {
            var v1 = Position - line.Position;
            var v2 = line.Position2 - line.Position;
            var v3 = new Vec2d(-Direction.y, Direction.x);


            var dot = Vec2d.DotProduct(v2,v3);
            if (Math.Abs(dot) < 0.000001)
                return null;

            var t1 = Vec2d.CrossProduct(v2, v1) / dot;
            var t2 = Vec2d.DotProduct(v1, v3) / dot;

            if (t1 >= 0.0 && (t2 >= 0.0 && t2 <= 1.0))
                return new Vec2d(Position + (Direction * t1));

            return null;
        }

        public static List<Ray> GetRays(Vec2d position, int angle)
        {
            List<Ray> rays = new List<Ray>();

            int currentAngle = 0;
            while(currentAngle < 360)
            {
                rays.Add(new Ray(position, currentAngle));

                currentAngle += angle;
            }

            return rays;
        }
    }
}
