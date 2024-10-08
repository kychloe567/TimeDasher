﻿using System;
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
        public int TempIndex { get; set; }

        public double Right
        {
            get { return Position.x + Size.x; }
        }
        
        public double Bottom
        {
            get { return Position.y + Size.y; }
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
            return new Vec2d((Position.x + Size.x / 2), (Position.y + Size.y / 2));
        }

        public override bool IsVisible(Camera camera)
        {
            if (Position.x + Size.x > camera.CenteredPosition.x && Position.x < camera.CenteredPosition.x + camera.WindowSize.x &&
               Position.y + Size.y > camera.CenteredPosition.y && Position.y < camera.CenteredPosition.y + camera.WindowSize.y)
                return true;
            return false;
        }

        public override Rectangle GetCopy()
        {
            Rectangle r = new Rectangle(new Vec2d(Position), new Vec2d(Size), new Color(Color))
            {
                Rotation = Rotation,
                OutLineThickness = OutLineThickness,
                OutLineColor = new Color(OutLineColor),
                IsFilled = IsFilled,
                IsAffectedByCamera = IsAffectedByCamera,
                IsPlayer = IsPlayer,
                ObjectType = ObjectType,
                Velocity = Velocity
            };

            if(DrawPriority.Type == DrawPriority.PriorityType.Custom)
            {
                r.DrawPriority = DrawPriority.Custom(DrawPriority.CustomPriority);
            }

            if (Texture != null)
            {
                r.Texture = Texture.Clone();
                r.TexturePath = TexturePath;
            }

            if (StateMachine != null)
                r.StateMachine = StateMachine.GetCopy();

            return r;
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
            else if (d is Rectangle r)
            {
                return !(r.Position.x > Right || r.Right < Position.x || r.Position.y > Bottom || r.Bottom < Position.y);
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

        public bool IntersectsEquals(DrawableObject d)
        {
            if (d is Rectangle r)
                return !(r.Position.x >= Right || r.Right <= Position.x || r.Position.y >= Bottom || r.Bottom <= Position.y);
            else
                throw new NotImplementedException();
        }

        public override bool Intersects(Vec2d v)
        {
            if (v.x >= Position.x && v.x <= Right && v.y >= Position.y && v.y <= Bottom)
                return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is Rectangle r && Position == r.Position && Size == r.Size && Color == r.Color)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Position);
            hash.Add(Rotation);
            hash.Add(OutLineThickness);
            hash.Add(OutLineColor);
            hash.Add(Color);
            hash.Add(TexturePath);
            hash.Add(DrawPriority);
            hash.Add(ObjectType);
            hash.Add(Size);
            return hash.ToHashCode();
        }
    }
}
