using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public class ColorHSV
    {
        public int H { get; set; }
        public int S { get; set; }
        public int V { get; set; }

        public ColorHSV()
        {
            H = 0;
            S = 0;
            V = 0;
        }

        public ColorHSV(int h, int s, int v)
        {
            H = h;
            S = s;
            V = v;
        }
    }
}