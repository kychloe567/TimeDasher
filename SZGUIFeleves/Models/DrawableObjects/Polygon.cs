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
            if (!(Middle is null))
                return Middle;

            Vec2d middle = new Vec2d(Position);
            foreach(Vec2d point in Points)
                middle += point;

            middle /= Points.Count() + 1;
            return middle;
        }
    }
}
