using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Helpers;

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

        public List<Polygon> GetShadows(List<DrawableObject> objects, Vec2d WindowSize, int alpha)
        {
            List<Polygon> shadows = new List<Polygon>();

            foreach (var obj in objects)
            {
                if (obj is Polygon)
                    continue;

                Vec2d leftCorner = null;
                Vec2d rightCorner = null;
                double minAngle = 9999;
                double maxAngle = -1;

                if (obj is Rectangle rect)
                {
                    if (Position.x > rect.Position.x && Position.y > rect.Position.y &&
                       Position.x < rect.Position.x + rect.Size.x && Position.y < rect.Position.y + rect.Size.y)
                        continue;

                    List<Vec2d> corners = new List<Vec2d>();
                    corners.Add(new Vec2d(rect.Position.x, rect.Position.y));
                    corners.Add(new Vec2d(rect.Position.x + rect.Size.x, rect.Position.y));
                    corners.Add(new Vec2d(rect.Position.x + rect.Size.x, rect.Position.y + rect.Size.y));
                    corners.Add(new Vec2d(rect.Position.x, rect.Position.y + rect.Size.y));

                    // We are on left side so angles won't work properly
                    // The most left corner is at [270-360] and the right corners are [0-90],
                    // because of that minAngle at the right corners are smaller, and that's wrong.
                    // TODO: Better idea xd
                    if (Position.x < rect.Position.x &&
                       Position.y > rect.Position.y &&
                       Position.y < rect.Position.y + rect.Size.y)
                    {
                        minAngle = (corners[0] - Position).Angle;
                        leftCorner = corners[0];
                        maxAngle = (corners[3] - Position).Angle;
                        rightCorner = corners[3];
                    }
                    else
                    {
                        // All other sides are fine
                        foreach (Vec2d corner in corners)
                        {
                            double angle = (corner - Position).Angle;
                            if (angle < minAngle)
                            {
                                minAngle = angle;
                                leftCorner = corner;
                            }
                            if (angle > maxAngle)
                            {
                                maxAngle = angle;
                                rightCorner = corner;
                            }
                        }
                    }
                }
                else if(obj is Line l)
                {
                    leftCorner = l.Position;
                    minAngle = (leftCorner - Position).Angle;
                    rightCorner = l.Position2;
                    maxAngle = (rightCorner - Position).Angle;
                }
                else if(obj is Ellipse e)
                {
                    // TODO: Ellipse shadow
                    if (e.Radius.x != e.Radius.y)
                        continue;

                    Vec2d thalesPoint = Position + (e.Position - Position) / 2;
                    double thalesRadius = ((e.Position - Position) / 2).Length;

                    double distBetweenCircles = (thalesPoint - e.Position).Length;

                    double a = (e.Radius.x * e.Radius.x - thalesRadius * thalesRadius + distBetweenCircles * distBetweenCircles) / (2 * distBetweenCircles);
                    double h = Math.Sqrt(e.Radius.x * e.Radius.x - a * a);

                    double cx2 = e.Position.x + a * (thalesPoint.x - e.Position.x) / distBetweenCircles;
                    double cy2 = e.Position.y + a * (thalesPoint.y - e.Position.y) / distBetweenCircles;

                    leftCorner = new Vec2d(cx2 - h * (thalesPoint.y - e.Position.y) / distBetweenCircles, (cy2 + h * (thalesPoint.x - e.Position.x) / distBetweenCircles));
                    minAngle = (leftCorner - Position).Angle;
                    rightCorner = new Vec2d(cx2 + h * (thalesPoint.y - e.Position.y) / distBetweenCircles, (cy2 - h * (thalesPoint.x - e.Position.x) / distBetweenCircles));
                    maxAngle = (rightCorner - Position).Angle;
                }

                if (leftCorner is null || rightCorner is null || leftCorner.x is double.NaN || rightCorner.x is double.NaN)
                    continue;

                int leftPointEdgeIndex = -1;
                int rightPointEdgeIndex = -1;

                Vec2d leftPoint = null;
                Vec2d rightPoint = null;

                List<Line> lines = new List<Line>();
                lines.Add(new Line(new Vec2d(0, 0), new Vec2d(WindowSize.x, 0)));
                lines.Add(new Line(new Vec2d(WindowSize.x, 0), new Vec2d(WindowSize.x, WindowSize.y)));
                lines.Add(new Line(new Vec2d(WindowSize.x, WindowSize.y), new Vec2d(0, WindowSize.y)));
                lines.Add(new Line(new Vec2d(0, WindowSize.y), new Vec2d(0, 0)));

                for(int lineI = 0; lineI < lines.Count; lineI++)
                {
                    Vec2d intersectionLeft = new Ray(leftCorner, minAngle).Cast(lines[lineI]);
                    if (!(intersectionLeft is null))
                    {
                        leftPoint = intersectionLeft;
                        leftPointEdgeIndex = lineI;
                    }

                    Vec2d intersectionRight = new Ray(rightCorner, maxAngle).Cast(lines[lineI]);
                    if (!(intersectionRight is null))
                    {
                        rightPoint = intersectionRight;
                        rightPointEdgeIndex = lineI;
                    }
                }

                leftPoint = new Vec2d(Math.Round(leftPoint.x, 3), Math.Round(leftPoint.y, 3));
                rightPoint = new Vec2d(Math.Round(rightPoint.x, 3), Math.Round(rightPoint.y, 3));

                if (leftPointEdgeIndex == rightPointEdgeIndex)
                {
                    Polygon p = new Polygon(leftCorner, new List<Vec2d> 
                        { leftPoint, rightPoint, rightCorner }, 
                        new Color(0, 0, 0, alpha));
                    p.DrawPriority = DrawPriority.Bottom;
                    shadows.Add(p);
                }
                else if(Math.Abs(leftPointEdgeIndex-rightPointEdgeIndex) == 1 || 
                    (leftPointEdgeIndex == 0 && rightPointEdgeIndex == 3) || 
                    (leftPointEdgeIndex == 3 && rightPointEdgeIndex == 0))
                {
                    if (leftPointEdgeIndex > rightPointEdgeIndex &&
                      !(leftPointEdgeIndex == 3 && rightPointEdgeIndex == 0))
                        rightPointEdgeIndex = leftPointEdgeIndex;

                    Polygon p = new Polygon(leftCorner, new List<Vec2d> 
                        { leftPoint, lines[rightPointEdgeIndex].Position, rightPoint, rightCorner }, 
                        new Color(0, 0, 0, alpha));
                    p.DrawPriority = DrawPriority.Bottom;
                    shadows.Add(p);
                }
                else
                {
                    Polygon p = new Polygon(leftCorner, new List<Vec2d> 
                        { leftPoint, lines[leftPointEdgeIndex].Position2, lines[rightPointEdgeIndex].Position, rightPoint, rightCorner }, 
                        new Color(0, 0, 0, alpha));
                    p.DrawPriority = DrawPriority.Bottom;
                    shadows.Add(p);
                }
            }

            return shadows;
        }
    }
}
