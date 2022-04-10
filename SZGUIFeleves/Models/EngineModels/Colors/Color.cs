using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public class Color
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int A { get; set; }

        public Color()
        {
            R = 0;
            G = 0;
            B = 0;
            A = 255;
        }

        public Color(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
            A = 255;
        }

        public Color(int r, int g, int b, int a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color(Color c)
        {
            R = c.R;
            G = c.G;
            B = c.B;
            A = c.A;
        }

        #region Static colors
        public static Color Black{ get { return new Color(0, 0, 0); } }
        public static Color Gray{ get { return new Color(100, 100, 100); } }
        public static Color White{ get { return new Color(255, 255, 255); } }
        public static Color Red{ get { return new Color(255, 0, 0); } }
        public static Color Green{ get { return new Color(0, 255, 0); } }
        public static Color Blue{ get { return new Color(0, 0, 255); } }
        public static Color Yellow{ get { return new Color(255, 255, 0); } }
        public static Color Turqoise{ get { return new Color(0, 255, 255); } }
        public static Color Purple { get { return new Color(255, 0, 255); } }
        #endregion

        public static bool operator ==(Color a, Color b)
        {
            if (a is null || b is null)
                return false;

            if (a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A)
                return true;
            else return false;
        }

        public static bool operator !=(Color a, Color b)
        {
            if (a is null || b is null)
                return false;

            if (a.R != b.R || a.G != b.G || a.B != b.B || a.A != b.A)
                return true;
            else return false;
        }
    }
}
