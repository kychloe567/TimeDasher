using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Helpers;

namespace SZGUIFeleves.Models
{
    public static class Interpolater
    {
        public static object GetInterpolatedValue(object Start, object End, double value)
        {
            if (End is null)
                return Start;

            if (Start is Vec2d sv2 && End is Vec2d ev2) // Velocity and Size
            {
                double x = MathHelper.MapFunction(value, 0, 1, sv2.x, ev2.x);
                double y = MathHelper.MapFunction(value, 0, 1, sv2.y, ev2.y);

                return new Vec2d(x, y);
            }
            else if (Start is double sd && End is double ed) // Rotation
            {
                return MathHelper.MapFunction(value, 0, 1, sd, ed);
            }
            else if (Start is Color sc && End is Color ec)
            {
                ColorHSV StartHSV = ColorConverter.RGBtoHSV(sc);
                ColorHSV EndHSV = ColorConverter.RGBtoHSV(ec);

                int h = (int)((EndHSV.H - StartHSV.H) * value + StartHSV.H);
                int s = (int)((EndHSV.S - StartHSV.S) * value + StartHSV.S);
                int v = (int)((EndHSV.V - StartHSV.V) * value + StartHSV.V);
                int a = (int)((ec.A - sc.A) * value + sc.A);

                Color newColor = ColorConverter.HSVtoRGB(new ColorHSV(h, s, v));
                newColor.A = a;

                return newColor;
            }

            return null;
        }
    }
}
