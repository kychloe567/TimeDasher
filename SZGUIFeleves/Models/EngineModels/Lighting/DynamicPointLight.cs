using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Helpers;
using SZGUIFeleves.Models.DrawableObjects;

namespace SZGUIFeleves.Models
{
    public class DynamicPointLight
    {
        public Vec2d Position { get; set; }

        public DynamicPointLight()
        {
            Position = new Vec2d();
        }

        public DynamicPointLight(Vec2d position)
        {
            Position = position;
        }

        private List<Vec2d> ObjectIsCutOff(Line l, Camera camera)
        {
            List<Line> lineSegments = new List<Line>()
            {
                new Line(new Vec2d(camera.CenteredPosition.x, camera.CenteredPosition.y), new Vec2d(camera.CenteredPosition.x + camera.WindowSize.x, camera.CenteredPosition.y)),
                new Line(new Vec2d(camera.CenteredPosition.x + camera.WindowSize.x, camera.CenteredPosition.y), new Vec2d(camera.CenteredPosition.x + camera.WindowSize.x, camera.CenteredPosition.y + camera.WindowSize.y)),
                new Line(new Vec2d(camera.CenteredPosition.x + camera.WindowSize.x, camera.CenteredPosition.y + camera.WindowSize.y), new Vec2d(camera.CenteredPosition.x, camera.CenteredPosition.y + camera.WindowSize.y)),
                new Line(new Vec2d(camera.CenteredPosition.x, camera.CenteredPosition.y + camera.WindowSize.y), new Vec2d(camera.CenteredPosition.x, camera.CenteredPosition.y))
            };

            List<Vec2d> points = new List<Vec2d>();
            foreach(Line cLine in lineSegments)
            {
                double tUp = (cLine.Position.x - l.Position.x) * (l.Position.y - l.Position2.y) - (cLine.Position.y - l.Position.y) * (l.Position.x - l.Position2.x);
                double tDown = (cLine.Position.x - cLine.Position2.x) * (l.Position.y - l.Position2.y) - (cLine.Position.y - cLine.Position2.y) * (l.Position.x - l.Position2.x);
                double t = tUp / tDown;

                double uUp = (cLine.Position.x - l.Position.x) * (cLine.Position.y - cLine.Position2.y) - (cLine.Position.y - l.Position.y) * (cLine.Position.x - cLine.Position2.x);
                double uDown = (cLine.Position.x - cLine.Position2.x) * (l.Position.y - l.Position2.y) - (cLine.Position.y - cLine.Position2.y) * (l.Position.x - l.Position2.x);
                double u = uUp / uDown;

                if (t < 0 || t > 1 || u < 0 || u > 1)
                    continue;

                points.Add(new Vec2d(cLine.Position.x + t*(cLine.Position2.x-cLine.Position.x), cLine.Position.y + t*(cLine.Position2.y-cLine.Position.y)));
            }
            if (points.Count == 0)
                return null;

            return points;
        }

        public Shadow GetShadows(List<DrawableObject> objects, Vec2d WindowSize, Camera camera)
        {
            Vec2d cPos = new Vec2d(camera.CenteredPosition);
            // Get all unique points and all segments

            List<Vec2d> uniquePoints = new List<Vec2d>()
            {
                 new Vec2d(cPos.x-10,cPos.y-10),
                 new Vec2d(cPos.x-10, cPos.y + WindowSize.y + 10),
                 new Vec2d(cPos.x + WindowSize.x + 10, cPos.y + WindowSize.y + 10),
                 new Vec2d(cPos.x + WindowSize.x + 10, cPos.y - 10)
            };
            List<Line> lineSegments = new List<Line>()
            {
                new Line(new Vec2d(cPos.x-10, cPos.y-10), new Vec2d(cPos.x + WindowSize.x + 10, cPos.y -10)),
                new Line(new Vec2d(cPos.x + WindowSize.x+10, cPos.y-10), new Vec2d(cPos.x + WindowSize.x+10, cPos.y + WindowSize.y+10)),
                new Line(new Vec2d(cPos.x + WindowSize.x+10, cPos.y + WindowSize.y+10), new Vec2d(cPos.x-10, cPos.y + WindowSize.y+10)),
                new Line(new Vec2d(cPos.x-10 ,cPos.y + WindowSize.y+10), new Vec2d(cPos.x-10, cPos.y-10))
            };

            foreach (DrawableObject obj in objects)
            {
                if (obj is Player)
                    continue;

                if (!obj.IsVisible(camera))
                    continue;

                if (obj is Rectangle r)
                {
                    if (IsInside(r))
                        return null;

                    if(!uniquePoints.Contains(r.Position))
                        uniquePoints.Add(r.Position);
                    if (!uniquePoints.Contains(r.Position + new Vec2d(0, r.Size.y)))
                        uniquePoints.Add(r.Position + new Vec2d(0,r.Size.y));
                    if (!uniquePoints.Contains(r.Position + r.Size))
                        uniquePoints.Add(r.Position + r.Size);
                    if (!uniquePoints.Contains(r.Position + new Vec2d(r.Size.x, 0)))
                        uniquePoints.Add(r.Position + new Vec2d(r.Size.x, 0));


                    lineSegments.Add(new Line(r.Position, new Vec2d(r.Position.x + r.Size.x, r.Position.y)));
                    lineSegments.Add(new Line(new Vec2d(r.Position.x + r.Size.x, r.Position.y), new Vec2d(r.Position.x + r.Size.x, r.Position.y + r.Size.y)));
                    lineSegments.Add(new Line(new Vec2d(r.Position.x + r.Size.x, r.Position.y + r.Size.y), new Vec2d(r.Position.x, r.Position.y + r.Size.y)));
                    lineSegments.Add(new Line(new Vec2d(r.Position.x, r.Position.y + r.Size.y), new Vec2d(r.Position)));

                    List<Vec2d> cutoff = ObjectIsCutOff(lineSegments[lineSegments.Count - 4], camera);
                    if (!(cutoff is null))
                        uniquePoints.AddRange(cutoff);

                    cutoff = ObjectIsCutOff(lineSegments[lineSegments.Count - 3], camera);
                    if (!(cutoff is null))
                        uniquePoints.AddRange(cutoff);

                    cutoff = ObjectIsCutOff(lineSegments[lineSegments.Count - 2], camera);
                    if (!(cutoff is null))
                        uniquePoints.AddRange(cutoff);

                    cutoff = ObjectIsCutOff(lineSegments[lineSegments.Count - 1], camera);
                    if (!(cutoff is null))
                        uniquePoints.AddRange(cutoff);
                }
                else if(obj is Line l)
                {
                    if (!uniquePoints.Contains(l.Position))
                        uniquePoints.Add(l.Position);
                    if (!uniquePoints.Contains(l.Position2))
                        uniquePoints.Add(l.Position2);

                    lineSegments.Add(l);
                }
                else if (obj is Circle e)
                {
                    if (IsInside(e))
                        return null;

                    if (e.Radius != e.Radius)
                        continue;

                    Vec2d thalesPoint = Position + (e.Position - Position) / 2;
                    double thalesRadius = ((e.Position - Position) / 2).Length;

                    double distBetweenCircles = (thalesPoint - e.Position).Length;

                    double a = (e.Radius * e.Radius - thalesRadius * thalesRadius + distBetweenCircles * distBetweenCircles) / (2 * distBetweenCircles);
                    double h = Math.Sqrt(e.Radius * e.Radius - a * a);

                    double cx2 = e.Position.x + a * (thalesPoint.x - e.Position.x) / distBetweenCircles;
                    double cy2 = e.Position.y + a * (thalesPoint.y - e.Position.y) / distBetweenCircles;

                    List<Vec2d> circlePoints = new List<Vec2d>();

                    for (int ang = 0; ang < 360; ang+=30)
                    {
                        double dx = Math.Cos(MathHelper.ConvertToRadians(ang)) * e.Radius * 0.99;
                        double dy = Math.Sin(MathHelper.ConvertToRadians(ang)) * e.Radius * 0.99;

                        circlePoints.Add(e.Position + new Vec2d(dx, dy));
                    }

                    for (int i = 0; i < circlePoints.Count()-1; i++)
                    {
                        uniquePoints.Add(circlePoints[i]);
                        lineSegments.Add(new Line(circlePoints[i], circlePoints[i + 1]));
                    }
                    lineSegments.Add(new Line(circlePoints.Last(), circlePoints.First()));
                    uniquePoints.Add(circlePoints.Last());
                }
            }

            // Get all angles
            List<double> uniqueAngles = new List<double>();
            foreach(Vec2d p in uniquePoints)
            {
                var angle = Math.Atan2(p.y - Position.y, p.x - Position.x);
                uniqueAngles.Add(angle - 0.00001);
                uniqueAngles.Add(angle);
                uniqueAngles.Add(angle + 0.00001);
                p.Temp = angle;
            }

            List<Vec2d> intersects = new List<Vec2d>();

            foreach(double angle in uniqueAngles)
            {
                double dx = Math.Cos(angle);
                double dy = Math.Sin(angle);

                Ray r = new Ray(Position, new Vec2d(dx, dy));

                double minDist = 99999;
                Vec2d closestIntersect = null;
                foreach(Line l in lineSegments)
                {
                    Vec2d p = r.Cast(l);
                    if (p is null)
                        continue;

                    if((double)p.Temp < minDist)
                    {
                        minDist = (double)p.Temp;
                        closestIntersect = p;
                    }
                }

                if (closestIntersect is null)
                    continue;

                closestIntersect.Temp = angle;

                intersects.Add(closestIntersect);
            }

            intersects.Sort((a, b) => ((double)b.Temp).CompareTo((double)a.Temp));
            Vec2d pos = intersects[0];

            intersects.RemoveAt(0);

            Shadow shadow = new Shadow(pos, intersects)
            {
                DrawPriority = DrawPriority.Top
            };
            return shadow;
        }

        private bool IsInside(Rectangle r)
        {
            if (Position.x > r.Position.x && Position.x < r.Position.x + r.Size.x && Position.y > r.Position.y && Position.y < r.Position.y + r.Size.y)
                return true;
            else return false;
        }

        private bool IsInside(Circle e)
        {
            if ((Position - e.Position).Length <= e.Radius)
                return true;
            else return false;
        }
    }
}
