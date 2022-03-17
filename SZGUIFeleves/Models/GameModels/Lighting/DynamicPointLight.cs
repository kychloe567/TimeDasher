using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public class DynamicPointLight
    {
        public Vec2d Position { get; set; }
        public double Intensity { get; set; }
        public double Radius { get; set; }

        public DynamicPointLight()
        {
            Position = new Vec2d();
        }

        public DynamicPointLight(Vec2d position)
        {
            this.Position = position;
            this.Intensity = 1;
            this.Radius = 1;
        }

        public DynamicPointLight(Vec2d position, double radius)
        {
            this.Position = position;
            this.Intensity = 1;
            this.Radius = radius;
        }

        public DynamicPointLight(Vec2d position, double intensity, double radius)
        {
            this.Position = position;
            this.Intensity = intensity;
            this.Radius = radius;
        }

        public List<Polygon> GetShadows(List<DrawableObject> objects, Vec2d WindowSize)
        {
            List<Polygon> shadows = new List<Polygon>();

            foreach (var obj in objects)
            {
                Vec2d leftCorner = null;
                Vec2d rightCorner = null;
                double minAngle = 9999;
                double maxAngle = -1;

                if (obj is Rectangle rect)
                {
                    List<Vec2d> corners = new List<Vec2d>();
                    corners.Add(new Vec2d(rect.Position.x, rect.Position.y));
                    corners.Add(new Vec2d(rect.Position.x + rect.Size.x, rect.Position.y));
                    corners.Add(new Vec2d(rect.Position.x + rect.Size.x, rect.Position.y + rect.Size.y));
                    corners.Add(new Vec2d(rect.Position.x, rect.Position.y + rect.Size.y));

                    foreach (Vec2d corner in corners)
                    {
                        double angle = (corner - Position).Angle;
                        if(angle < minAngle)
                        {
                            minAngle = angle;
                            leftCorner = corner;
                        }
                        if(angle > maxAngle)
                        {
                            maxAngle = angle;
                            rightCorner = corner;
                        }
                    }
                }

                if (leftCorner is null || rightCorner is null)
                    continue;

                Vec2d leftPoint = null;
                Vec2d rightPoint = null;

                List<Line> lines = new List<Line>();
                lines.Add(new Line(new Vec2d(0, 0), new Vec2d(WindowSize.x, 0)));
                lines.Add(new Line(new Vec2d(WindowSize.x, 0), new Vec2d(WindowSize.x, WindowSize.y)));
                lines.Add(new Line(new Vec2d(WindowSize.x, WindowSize.y), new Vec2d(0, WindowSize.y)));
                lines.Add(new Line(new Vec2d(0, 0), new Vec2d(0, WindowSize.y)));

                foreach(Line l in lines)
                {
                    Vec2d intersectionLeft = new Ray(leftCorner, minAngle).Cast(l);
                    if (!(intersectionLeft is null))
                        leftPoint = intersectionLeft;

                    Vec2d intersectionRight = new Ray(rightCorner, maxAngle).Cast(l);
                    if (!(intersectionRight is null))
                        rightPoint = intersectionRight;
                }

                leftPoint = new Vec2d(Math.Round(leftPoint.x, 3), Math.Round(leftPoint.y, 3));
                rightPoint = new Vec2d(Math.Round(rightPoint.x, 3), Math.Round(rightPoint.y, 3));

                Polygon p = new Polygon(leftCorner, new List<Vec2d> { leftPoint, rightPoint, rightCorner }, new Color(0, 0, 0));
                p.DrawPriority = DrawPriority.Bottom;
                shadows.Add(p);
            }

            return shadows;







            //List<Ray> rays = Ray.GetRays(Position, 1);

            // TODO: swap for loops for efficiency
            //foreach (Ray r in rays)
            //{
            //    foreach (var obj in objects)
            //    {
            //        if (obj.IsTransparent)
            //            continue;
            //    }
            //}

            //List<Tuple<double, double>> AnglePairs = new List<Tuple<double, double>>();
            //foreach (var obj in objects)
            //{
            //    if (obj.IsTransparent)
            //        continue;

            //    double startAngle = -1;
            //    double endAngle = -1;
            //    foreach (Ray r in rays)
            //    {
            //        if (obj is Rectangle rect)
            //        {
            //            List<Line> lines = new List<Line>();
            //            lines.Add(new Line(new Vec2d(rect.Position.x, rect.Position.y), new Vec2d(rect.Position.x + rect.Size.x, rect.Position.y)));
            //            lines.Add(new Line(new Vec2d(rect.Position.x + rect.Size.x, rect.Position.y), new Vec2d(rect.Position.x + rect.Size.x, rect.Position.y + rect.Size.y)));
            //            lines.Add(new Line(new Vec2d(rect.Position.x + rect.Size.x, rect.Position.y + rect.Size.y), new Vec2d(rect.Position.x, rect.Position.y + rect.Size.y)));
            //            lines.Add(new Line(new Vec2d(rect.Position.x, rect.Position.y), new Vec2d(rect.Position.x, rect.Position.y + rect.Size.y)));

            //            double minAngle = 9999;
            //            double maxAngle = -1;
            //            foreach (Line l in lines)
            //            {
            //                Vec2d intersection = r.Cast(rect);
            //                if (intersection is null)
            //                    continue;

            //                if (r.Angle < minAngle)
            //                    minAngle = r.Angle;

            //                if (r.Angle > maxAngle)
            //                    maxAngle = r.Angle;
            //            }

            //            if (startAngle == -1 && minAngle != 9999)
            //                startAngle = minAngle;

            //            if(maxAngle != -1)
            //                endAngle = maxAngle;
            //        }
            //        if (obj is Line line)
            //        {
            //            Vec2d intersection = r.Cast(line);
            //            if (intersection is null)
            //                continue;

            //            if (startAngle == -1)
            //                startAngle = r.Angle;

            //            endAngle = r.Angle;
            //        }
            //    }

            //    if (startAngle != -1 && endAngle != -1)
            //        AnglePairs.Add(new Tuple<double, double>(startAngle, endAngle));
            //}



            //return new List<Vec2d>();
        }
    }
}
