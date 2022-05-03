using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public class Line : DrawableObject
    {
        public Vec2d Position2 { get; set; }
        public double Width { get; set; }

        #region Constructors
        public Line() : base()
        {
            Position2 = new Vec2d();
            Width = 1.0f;
        }

        public Line(Vec2d position, Vec2d position2) : base(position)
        {
            Position2 = position2;
            Width = 1.0f;
        }

        public Line(Vec2d position, Vec2d position2, Color color) : base(position, color)
        {
            Position2 = position2;
            Width = 1.0f;
        }

        public Line(Vec2d position, Vec2d position2, double width) : base(position)
        {
            Position2 = position2;
            Width = width;
        }

        public Line(Vec2d position, Vec2d position2, Color color, double width) : base(position, color)
        {
            Position2 = position2;
            Width = width;
        }
        #endregion

        public override Vec2d GetMiddle()
        {
            return new Vec2d((Position.x + Position2.x) / 2, (Position.y + Position2.y) / 2);
        }

        public override bool IsVisible(Camera camera)
        {
            Vec2d centeredPos = camera.CenteredPosition;

            if (Position.x > camera.CenteredPosition.x && Position.x < camera.CenteredPosition.x + camera.WindowSize.x &&
                Position.y > camera.CenteredPosition.y && Position.y < camera.CenteredPosition.y + camera.WindowSize.y &&
                Position2.x > camera.CenteredPosition.x && Position2.x < camera.CenteredPosition.x + camera.WindowSize.x &&
                Position2.y > camera.CenteredPosition.y && Position2.y < camera.CenteredPosition.y + camera.WindowSize.y)
                return true;
            else return false;
        }

        public override Line GetCopy()
        {
            Line l = new Line(new Vec2d(Position),new Vec2d(Position2), new Color(Color), Width)
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
                l.Texture = Texture.Clone();
                l.TexturePath = TexturePath;
            }

            if (StateMachine != null)
                l.StateMachine = StateMachine.GetCopy();

            return l;
        }

        public override bool Intersects(DrawableObject d)
        {
            if (d is Circle circ)
            {
                Vec2d de = Position2 - Position;
                Vec2d f = Position - circ.Position;

                double a = Vec2d.DotProduct(de, de);
                double b = 2 * Vec2d.DotProduct(f, de);
                double c = Vec2d.DotProduct(f, f) - circ.Radius * circ.Radius;

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
            else if (d is Line l)
            {
                double q = (Position.Y - l.Position.Y) * (l.Position2.X - l.Position.X) - (Position.X - l.Position.X) * (l.Position2.Y - l.Position.Y);
                double de = (Position2.X - Position.X) * (l.Position2.Y - l.Position.Y) - (Position2.Y - Position.Y) * (l.Position2.X - l.Position.X);

                if (de == 0)
                    return false;

                double r = q / de;

                q = (Position.Y - l.Position.Y) * (Position2.X - Position.X) - (Position.X - l.Position.X) * (Position2.Y - Position.Y);
                double s = q / de;

                if (r < 0 || r > 1 || s < 0 || s > 1)
                    return false;

                return true;
            }
            else if (d is Rectangle r)
            {
                List<Line> lines = new List<Line>()
                {
                    new Line(r.Position, new Vec2d(r.Position.X + r.Size.X, r.Position.Y)),
                    new Line(new Vec2d(r.Position.X + r.Size.X, r.Position.Y), new Vec2d(r.Position.X + r.Size.X, r.Position.Y + r.Size.Y)),
                    new Line(new Vec2d(r.Position.X + r.Size.X, r.Position.Y + r.Size.Y), new Vec2d(r.Position.X, r.Position.Y + r.Size.Y)),
                    new Line(new Vec2d(r.Position.X, r.Position.Y + r.Size.Y), new Vec2d(r.Position))
                };

                bool intersects = false;
                foreach (Line li in lines)
                {
                    if (Intersects(li))
                    {
                        intersects = true;
                        break;
                    }
                }

                return intersects;
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

        // TODO
        public override bool Intersects(Vec2d v)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is Line l && Position == l.Position && Position2 == l.Position2 && Color == l.Color)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Position);
            hash.Add(Position2);
            hash.Add(Rotation);
            hash.Add(OutLineThickness);
            hash.Add(OutLineColor);
            hash.Add(Color);
            hash.Add(DrawPriority);
            return hash.ToHashCode();
        }
    }
}
