using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Helpers;

namespace SZGUIFeleves.Models
{
    public class Vec2d
    {
        public double X { get; set; }
        public double Y { get; set; }
        public object Temp { get; set; }

        public Vec2d()
        {
            X = 0.0f;
            Y = 0.0f;
        }

        public Vec2d(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vec2d(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vec2d(Vec2d v)
        {
            this.X = v.X;
            this.Y = v.Y;
        }

        public override string ToString()
        {
            return "x: " + X.ToString() + " y: " + Y.ToString();
        }

        #region Operator overloads
        public static Vec2d operator +(Vec2d a, Vec2d b)
        {
            return new Vec2d(a.X + b.X, a.Y + b.Y);
        }

        public static Vec2d operator -(Vec2d a, Vec2d b)
        {
            return new Vec2d(a.X - b.X, a.Y - b.Y);
        }

        public static Vec2d operator *(Vec2d a, int x)
        {
            return new Vec2d(a.X * x, a.Y * x);
        }

        public static Vec2d operator *(Vec2d a, Vec2d b)
        {
            return new Vec2d(a.X * b.X, a.Y * b.Y);
        }

        public static Vec2d operator /(Vec2d a, int x)
        {
            if (x == 0)
                return a;
            return new Vec2d(a.X / x, a.Y / x);
        }

        public static Vec2d operator *(Vec2d a, double x)
        {
            return new Vec2d(a.X * x, a.Y * x);
        }

        public static Vec2d operator /(Vec2d a, double x)
        {
            if (x == 0)
                return a;
            return new Vec2d(a.X / x, a.Y / x);
        }

        public static bool operator ==(Vec2d a, Vec2d b)
        {
            if (a is null || b is null)
                return false;

            if (a.X == b.X && a.Y == b.Y)
                return true;
            else return false;
        }

        public static bool operator !=(Vec2d a, Vec2d b)
        {
            if (a is null || b is null)
                return false;

            if (a.X != b.X || a.Y != b.Y)
                return true;
            else return false;
        }
        public override bool Equals(object obj)
        {
            Vec2d objv = (Vec2d)obj;
            if (X == objv.X && Y == objv.Y)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return (int)X * (int)Y;
        }
        #endregion

        #region Properties and static functions
        public double Length
        {
            get
            {
                return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
            }
        }

        public Vec2d Normalised()
        {
            double l = Length;
            if (l == 0)
                return new Vec2d(X, Y);

            return new Vec2d(X / l, Y / l);
        }

        public double Angle
        {
            get
            {
                return MathHelper.NormalizeAngle(MathHelper.ConvertToDegrees(Math.Atan2(Y, X)));// + 180;
            }
        }

        public static double DotProduct(Vec2d a, Vec2d b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static double CrossProduct(Vec2d a, Vec2d b)
        {
            return (a.X * b.Y) - (a.Y * b.X);
        }
        #endregion

    }
}
