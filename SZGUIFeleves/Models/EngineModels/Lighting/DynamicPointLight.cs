﻿using System;
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
        public double Radius { get; set; }

        public DynamicPointLight()
        {
            Position = new Vec2d();
        }

        public DynamicPointLight(Vec2d position)
        {
            Position = position;
            Radius = 1;
        }

        public DynamicPointLight(Vec2d position, double radius)
        {
            Position = position;
            Radius = radius;
        }

        public Shadow GetShadows(List<DrawableObject> objects, Vec2d WindowSize)
        {
            // Get all unique points and all segments
            List<Vec2d> uniquePoints = new List<Vec2d>()
            {
                 new Vec2d(0,0),
                 new Vec2d(0, WindowSize.Y),
                 new Vec2d(WindowSize.X, WindowSize.Y),
                 new Vec2d(WindowSize.X, 0)
            };
            List<Line> lineSegments = new List<Line>()
            {
                new Line(new Vec2d(0, 0), new Vec2d(WindowSize.X, 0)),
                new Line(new Vec2d(WindowSize.X, 0), new Vec2d(WindowSize.X, WindowSize.Y)),
                new Line(new Vec2d(WindowSize.X, WindowSize.Y), new Vec2d(0, WindowSize.Y)),
                new Line(new Vec2d(0, WindowSize.Y), new Vec2d(0, 0))
            };
            foreach (DrawableObject obj in objects)
            {
                if(obj is Rectangle r)
                {
                    if (IsInside(r))
                        return null;

                    if(!uniquePoints.Contains(r.Position))
                        uniquePoints.Add(r.Position);
                    if (!uniquePoints.Contains(r.Position + new Vec2d(0, r.Size.Y)))
                        uniquePoints.Add(r.Position + new Vec2d(0,r.Size.Y));
                    if (!uniquePoints.Contains(r.Position + r.Size))
                        uniquePoints.Add(r.Position + r.Size);
                    if (!uniquePoints.Contains(r.Position + new Vec2d(r.Size.X, 0)))
                        uniquePoints.Add(r.Position + new Vec2d(r.Size.X, 0));

                    lineSegments.Add(new Line(r.Position, new Vec2d(r.Position.X + r.Size.X, r.Position.Y)));
                    lineSegments.Add(new Line(new Vec2d(r.Position.X + r.Size.X, r.Position.Y), new Vec2d(r.Position.X + r.Size.X, r.Position.Y + r.Size.Y)));
                    lineSegments.Add(new Line(new Vec2d(r.Position.X + r.Size.X, r.Position.Y + r.Size.Y), new Vec2d(r.Position.X, r.Position.Y + r.Size.Y)));
                    lineSegments.Add(new Line(new Vec2d(r.Position.X, r.Position.Y + r.Size.Y), new Vec2d(r.Position)));
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

                    double cx2 = e.Position.X + a * (thalesPoint.X - e.Position.X) / distBetweenCircles;
                    double cy2 = e.Position.Y + a * (thalesPoint.Y - e.Position.Y) / distBetweenCircles;

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
                var angle = Math.Atan2(p.Y - Position.Y, p.X - Position.X);
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


            Shadow shadow = new Shadow(pos, intersects);
            return shadow;
        }

        private bool IsInside(Rectangle r)
        {
            if (Position.X > r.Position.X && Position.X < r.Position.X + r.Size.X && Position.Y > r.Position.Y && Position.Y < r.Position.Y + r.Size.Y)
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
