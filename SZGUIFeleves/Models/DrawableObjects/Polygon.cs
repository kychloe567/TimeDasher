using System;
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
            Vec2d centeredPos = camera.CenteredPosition;

            bool any = false;
            if (Position.x >= centeredPos.x && Position.x < centeredPos.x + camera.WindowSize.x &&
               Position.y >= centeredPos.y && Position.y < centeredPos.y + camera.WindowSize.y)
                any = true;

            if (!any)
            {
                foreach (Vec2d p in Points)
                {
                    if (p.x >= centeredPos.x && p.x < centeredPos.x + camera.WindowSize.x &&
                        p.y >= centeredPos.y && p.y < centeredPos.y + camera.WindowSize.y)
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
                IsPlayer = IsPlayer
            };

            if (Texture != null)
                p.Texture = Texture.Clone();

            if (StateMachine != null)
                p.StateMachine = StateMachine.GetCopy();

            return p;
        }
    }
}
