using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Helpers;

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

        public override bool IsVisible(Camera camera)
        {
            Vec2d centeredPos = camera.CenteredPosition;
            if (Position.x + Radius >= centeredPos.x && Position.x - Radius < centeredPos.x + camera.WindowSize.x &&
               Position.y + Radius >= centeredPos.y && Position.y - Radius < centeredPos.y + camera.WindowSize.y)
                return true;
            else return false;
        }

        public override Circle GetCopy()
        {
            Circle c = new Circle(new Vec2d(Position), Radius, new Color(Color))
            {
                Rotation = Rotation,
                OutLineThickness = OutLineThickness,
                OutLineColor = new Color(OutLineColor),
                IsFilled = IsFilled,
                DrawPriority = DrawPriority,
                IsAffectedByCamera = IsAffectedByCamera,
                IsPlayer = IsPlayer,
                ObjectType = ObjectType
            };

            if (Texture != null)
            {
                c.Texture = Texture.Clone();
                c.TexturePath = TexturePath;
            }

            if (StateMachine != null)
                c.StateMachine = StateMachine.GetCopy();

            return c;
        }

        public override bool Intersects(DrawableObject d)
        {
            if(d is Circle circ)
            {
                if ((Position - circ.Position).Length <= Radius + circ.Radius)
                    return true;
                return false;
            }
            else if(d is Line l)
            {
                Vec2d de = l.Position2 - l.Position;
                Vec2d f = l.Position - Position;

                double a = Vec2d.DotProduct(de, de);
                double b = 2 * Vec2d.DotProduct(f, de);
                double c = Vec2d.DotProduct(f,f) - Radius * Radius;

                double discriminant = b * b - 4 * a * c;
                if (discriminant < 0)
                    return false;
                else
                {
                    discriminant = Math.Sqrt(discriminant);

                    double t1 = (-b - discriminant) / (2 * a);
                    double t2 = (-b + discriminant) / (2 * a);

                    if (t1 >= 0 && t1 <= 1)
                        return true;

                    if (t2 >= 0 && t2 <= 1)
                        return true;

                    return false;
                }
            }
            else if(d is Rectangle r)
            {
                double closestX = MathHelper.Clamp(r.Position.x, r.Right, Position.x);
                double closestY = MathHelper.Clamp(r.Position.y, r.Bottom, Position.y);

                double distanceX = Position.x - closestX;
                double distanceY = Position.y - closestY;

                double distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
                return distanceSquared < (Radius * Radius);
            }
            else if(d is Polygon p)
            {
                throw new NotImplementedException();
            }
            else if(d is Text t)
            {
                throw new NotImplementedException();
            }

            return false;
        }

        public override bool Intersects(Vec2d v)
        {
            if ((v - Position).Length <= Radius)
                return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is Circle c && c.Position == Position && c.Radius == Radius && c.Color == Color)
                return true;
            return false;
        }
    }
}
