using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Helpers;

namespace SZGUIFeleves.Models
{
    public class Trap : DrawableObject
    {
        public Vec2d Size { get; set; }
        public double Right
        {
            get { return Position.x + Size.x; }
        }
        public double Bottom
        {
            get { return Position.y + Size.y; }
        }
        public override Vec2d GetMiddle()
        {
            return new Vec2d((Position.x + Size.x / 2), (Position.y + Size.y / 2));
        }
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override DrawableObject GetCopy()
        {
            throw new NotImplementedException();
        }


        public override bool Intersects(DrawableObject d)
        {
            if (d is Circle c)
            {
                double closestX = MathHelper.Clamp(Position.x, Right, c.Position.x);
                double closestY = MathHelper.Clamp(Position.y, Bottom, c.Position.y);

                double distanceX = c.Position.x - closestX;
                double distanceY = c.Position.y - closestY;

                double distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
                return distanceSquared < (c.Radius * c.Radius);
            }
            else if (d is Line l)
            {
                List<Line> lines = new List<Line>()
                {
                    new Line(Position, new Vec2d(Position.x + Size.x, Position.y)),
                    new Line(new Vec2d(Position.x + Size.x, Position.y), new Vec2d(Position.x + Size.x, Position.y + Size.y)),
                    new Line(new Vec2d(Position.x + Size.x, Position.y + Size.y), new Vec2d(Position.x, Position.y + Size.y)),
                    new Line(new Vec2d(Position.x, Position.y + Size.y), new Vec2d(Position))
                };

                bool intersects = false;
                foreach (Line li in lines)
                {
                    if (l.Intersects(li))
                    {
                        intersects = true;
                        break;
                    }
                }

                return intersects;
            }
            else if (d is Player j)
            {
                j.Dies();
                return true;
            }
            else if (d is Rectangle r)
            {
                return !(r.Position.x >= Right || r.Right <= Position.x || r.Position.y >= Bottom || r.Bottom <= Position.y);
            }
            else if (d is Polygon p)
            {
                throw new NotImplementedException();
            }
            else if (d is Text t)
            {
                throw new NotImplementedException();
            }

            return false;
        }

        public override bool IsVisible(Camera camera)
        {
            throw new NotImplementedException();
        }

    }
}
