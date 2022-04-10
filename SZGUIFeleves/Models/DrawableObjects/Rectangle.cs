using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Helpers;

namespace SZGUIFeleves.Models
{
    public class Rectangle : DrawableObject
    {
        public Vec2d Size { get; set; }
        public Vec2d CornerRadius { get; set; }
        public bool IsRounded { get; set; }

        public double Right
        {
            get { return Position.X + Size.X; }
        }
        
        public double Bottom
        {
            get { return Position.Y + Size.Y; }
        }

        #region Constructors
        public Rectangle() : base()
        {
            Size = new Vec2d();
            IsRounded = false;
        }

        public Rectangle(Vec2d position, Vec2d size) : base(position)
        {
            Size = size;
            IsRounded = false;
        }

        public Rectangle(Vec2d position, Vec2d size, Color color) : base(position, color)
        {
            Size = size;
            IsRounded = false;
        }
        #endregion

        public override Vec2d GetMiddle()
        {
            return new Vec2d((Position.X + Size.X / 2), (Position.Y + Size.Y / 2));
        }

        public override bool IsVisible(Camera camera)
        {
            Vec2d centeredPos = camera.CenteredPosition;
            if (Position.X + Size.X >= centeredPos.X && Position.X < centeredPos.X + camera.WindowSize.X &&
               Position.Y + Size.Y >= centeredPos.Y && Position.Y < centeredPos.Y + camera.WindowSize.Y)
                return true;
            else return false;
        }

        public override Rectangle GetCopy()
        {
            Rectangle r = new Rectangle(new Vec2d(Position), new Vec2d(Size), new Color(Color))
            {
                Rotation = Rotation,
                OutLineThickness = OutLineThickness,
                OutLineColor = new Color(OutLineColor),
                IsFilled = IsFilled,
                DrawPriority = DrawPriority,
                IsAffectedByCamera = IsAffectedByCamera,
                IsPlayer = IsPlayer
            };

            if (Texture != null)
                r.Texture = Texture.Clone();

            if (StateMachine != null)
                r.StateMachine = StateMachine.GetCopy();

            return r;
        }

        public override bool Intersects(DrawableObject d)
        {
            if (d is Circle c)
            {
                double closestX = MathHelper.Clamp(Position.X, Right, c.Position.X);
                double closestY = MathHelper.Clamp(Position.Y, Bottom, c.Position.Y);

                double distanceX = c.Position.X - closestX;
                double distanceY = c.Position.Y - closestY;

                double distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
                return distanceSquared < (c.Radius * c.Radius);
            }
            else if (d is Line l)
            {
                List<Line> lines = new List<Line>()
                {
                    new Line(Position, new Vec2d(Position.X + Size.X, Position.Y)),
                    new Line(new Vec2d(Position.X + Size.X, Position.Y), new Vec2d(Position.X + Size.X, Position.Y + Size.Y)),
                    new Line(new Vec2d(Position.X + Size.X, Position.Y + Size.Y), new Vec2d(Position.X, Position.Y + Size.Y)),
                    new Line(new Vec2d(Position.X, Position.Y + Size.Y), new Vec2d(Position))
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
            else if (d is Rectangle r)
            {
                return !(r.Position.X > Right || r.Right < Position.X || r.Position.Y > Bottom || r.Bottom < Position.Y);
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

        public override bool Equals(object obj)
        {
            if (obj is Rectangle r && Position == r.Position && Size == r.Size && Color == r.Color)
                return true;
            return false;
        }
    }
}
