using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public static class ColorConverter
    {
        public static ColorHSV RGBtoHSV(Color rgb)
        {
            int max = Math.Max(rgb.R, Math.Max(rgb.G, rgb.B));
            int min = Math.Min(rgb.R, Math.Min(rgb.G, rgb.B));
            System.Drawing.Color color = System.Drawing.Color.FromArgb((byte)rgb.A, (byte)rgb.R, (byte)rgb.G, (byte)rgb.B);

            double hue = Math.Round(color.GetHue(), 2);
            double saturation = Math.Round((max == 0) ? 0 : 1d - (1d * min / max), 0);
            double value = Math.Round(max / 255d, 0);
            return new ColorHSV((int)hue, (int)saturation, (int)value);
        }

        public static Color HSVtoRGB(ColorHSV hsv)
        {
            double H = hsv.H;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (hsv.V <= 0)
            { R = G = B = 0; }
            else if (hsv.S <= 0)
            {
                R = G = B = hsv.V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = hsv.V * (1 - hsv.S);
                double qv = hsv.V * (1 - hsv.S * f);
                double tv = hsv.V * (1 - hsv.S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = hsv.V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = hsv.V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = hsv.V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = hsv.V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = hsv.V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = hsv.V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = hsv.V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = hsv.V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = hsv.V; // Just pretend its black/white
                        break;
                }
            }

            return new Color((int)(R* 255.0), (int)(G * 255.0), (int)(B * 255.0));

            //int hi = Convert.ToInt32(Math.Floor(hsv.H / 60.0)) % 6;
            //double f = hsv.H / 60 - Math.Floor(hsv.H / 60.0);

            //double value = hsv.V * 255;
            //int v = Convert.ToInt32(value);
            //int p = Convert.ToInt32(value * (1 - hsv.S));
            //int q = Convert.ToInt32(value * (1 - f * hsv.S));
            //int t = Convert.ToInt32(value * (1 - (1 - f) * hsv.S));

            //if (hi == 0)
            //    return new Color(v, t, p);
            //else if (hi == 1)
            //    return new Color(q, v, p);
            //else if (hi == 2)
            //    return new Color(p, v, t);
            //else if (hi == 3)
            //    return new Color(p, q, v);
            //else if (hi == 4)
            //    return new Color(t, p, v);
            //else
            //    return new Color(v, p, q);
        }
    }
}