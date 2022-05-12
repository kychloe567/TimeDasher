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
        public double x { get; set; }
        public double y { get; set; }
        public object Temp { get; set; }
        public object Temp2 { get; set; }

        public Vec2d()
        {
            x = 0.0f;
            y = 0.0f;
        }

        public Vec2d(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vec2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Vec2d(Vec2d v)
        {
            x = v.x;
            y = v.y;
            Temp = v.Temp;
            Temp2 = v.Temp2;
        }

        public override string ToString()
        {
            return "x: " + x.ToString() + " y: " + y.ToString();
        }

        #region Operator overloads
        public static Vec2d operator +(Vec2d a, Vec2d b)
        {
            return new Vec2d(a.x + b.x, a.y + b.y);
        }

        public static Vec2d operator -(Vec2d a, Vec2d b)
        {
            return new Vec2d(a.x - b.x, a.y - b.y);
        }

        public static Vec2d operator *(Vec2d a, int x)
        {
            return new Vec2d(a.x * x, a.y * x);
        }

        public static Vec2d operator *(Vec2d a, Vec2d b)
        {
            return new Vec2d(a.x * b.x, a.y * b.y);
        }

        public static Vec2d operator /(Vec2d a, int x)
        {
            if (x == 0)
                return a;
            return new Vec2d(a.x / x, a.y / x);
        }

        public static Vec2d operator *(Vec2d a, double x)
        {
            return new Vec2d(a.x * x, a.y * x);
        }

        public static Vec2d operator /(Vec2d a, double x)
        {
            if (x == 0)
                return a;
            return new Vec2d(a.x / x, a.y / x);
        }

        public static bool operator ==(Vec2d a, Vec2d b)
        {
            if (a is null && b is null)
                return true;
            else if (a is null || b is null)
                return false;

            if (a.x == b.x && a.y == b.y)
                return true;
            else return false;
        }

        public static bool operator !=(Vec2d a, Vec2d b)
        {
            if (a is null && !(b is null))
                return true;
            else if (b is null && !(a is null))
                return true;
            else if (a is null || b is null)
                return false;

            if (a.x != b.x || a.y != b.y)
                return true;
            else return false;
        }
        public override bool Equals(object obj)
        {
            Vec2d objv = (Vec2d)obj;
            if (x == objv.x && y == objv.y)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return (int)x * (int)y;
        }
        #endregion

        #region Properties and static functions
        public double Length
        {
            get
            {
                return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            }
        }

        public Vec2d Normalised()
        {
            double l = Length;
            if (l == 0)
                return new Vec2d(x, y);

            return new Vec2d(x / l, y / l);
        }

        public double Angle
        {
            get
            {
                return MathHelper.NormalizeAngle(MathHelper.ConvertToDegrees(Math.Atan2(y, x)));// + 180;
            }
        }

        public static double DotProduct(Vec2d a, Vec2d b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static double CrossProduct(Vec2d a, Vec2d b)
        {
            return (a.x * b.y) - (a.y * b.x);
        }

        public static Vec2d Scale(Vec2d a, double zoom)
        {
            return a * zoom;
        }
        #endregion

    }
}
