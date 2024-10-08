﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public class Polygon : DrawableObject
    {
        public List<Vec2d> Points { get; set; }

        #region Constructors
        public Polygon() : base()
        {
            Points = new List<Vec2d>();
        }

        public Polygon(Vec2d position, List<Vec2d> points) : base(position)
        {
            Points = points;
        }

        public Polygon(Vec2d position, List<Vec2d> points, Color color) : base(position, color)
        {
            Points = points;
        }
        #endregion

        public void AddPoint(Vec2d point)
        {
            Points.Add(point);
        }

        /// <summary>
        /// Returns the centroid of the polygon
        /// </summary>
        /// <returns></returns>
        public override Vec2d GetMiddle()
        {
            Vec2d middle = new Vec2d(Position);
            foreach(Vec2d point in Points)
                middle += point;

            middle /= Points.Count() + 1;
            return middle;
        }

        public override bool IsVisible(Camera camera)
        {
            bool any = false;
            if (Position.x > camera.CenteredPosition.x && Position.x < camera.CenteredPosition.x + camera.WindowSize.x &&
               Position.y > camera.CenteredPosition.y && Position.y < camera.CenteredPosition.y + camera.WindowSize.y)
                any = true;

            if (!any)
            {
                foreach (Vec2d p in Points)
                {
                    if (p.x >= camera.CenteredPosition.x && p.x < camera.CenteredPosition.x + camera.WindowSize.x &&
                        p.y >= camera.CenteredPosition.y && p.y < camera.CenteredPosition.y + camera.WindowSize.y)
                    {
                        any = true;
                        break;
                    }
                }
            }

            return any;
        }

        public override Polygon GetCopy()
        {
            List<Vec2d> points = new List<Vec2d>();
            foreach (Vec2d pt in Points)
                points.Add(new Vec2d(pt));

            Polygon p = new Polygon(new Vec2d(Position), points, new Color(Color))
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
                p.Texture = Texture.Clone();
                p.TexturePath = TexturePath;
            }

            if (StateMachine != null)
                p.StateMachine = StateMachine.GetCopy();

            return p;
        }

        public override bool Intersects(DrawableObject d)
        {
            throw new NotImplementedException();
        }

        public override bool Intersects(Vec2d v)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Polygon))
                return false;

            Polygon p = obj as Polygon;

            if (Position != p.Position || Color != p.Color || Points.Count != p.Points.Count)
                return false;

            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i] != p.Points[i])
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Position);
            hash.Add(Rotation);
            hash.Add(OutLineThickness);
            hash.Add(OutLineColor);
            hash.Add(Color);
            hash.Add(DrawPriority);
            hash.Add(Points);
            return hash.ToHashCode();
        }
    }
}
