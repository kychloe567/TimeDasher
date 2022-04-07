using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Models;

namespace SZGUIFeleves.Logic
{
    internal class GravityLogic : IGravityLogic
    {
        private const double GravityCoe = 50;

        public void Falling(DrawableObject obj)
        {
            //if (obj != null)
            //{
            //    double tmp = Gravity(0, GravityCoe, obj.TimeElapsed);
            //    obj.Position.y += tmp;
            //}
        }

        public void Jumping(DrawableObject obj, double v0)
        {
            if (obj != null)
            {
                // While object is rising.
                double tmp = Gravity(v0, GravityCoe, obj.GravityTimeElapsed);

                if (obj.IsJumping && tmp > 0)
                    obj.IsGravitySet(obj.IsGravity); // - Set isJumping to false

                obj.Position.y += tmp;
            }
        }

        private double Gravity(double v0, double gravityCoe, double timeElapsed)
        {
            //double tmp = -v0 + (gravityCoe / 2) * Math.Pow((timeElapsed - (5 * v0 * v0)), 2);
            double tmp = -v0 + (gravityCoe / 2) * Math.Pow((timeElapsed - Math.Sqrt(v0 / (gravityCoe / 2))), 2);

            using (StreamWriter sw = new StreamWriter("timelapsed_8.txt", true))
            {
                sw.WriteLine("TimeElapsed: " + ((decimal)timeElapsed) + "\nTmpVal: " + (decimal)tmp + "\n");
            }

            return tmp;
        }
        //public void IsFalling(DrawableObject obj, DateTime now, bool value)
        //{
        //    obj.IsFalling = value;
        //    if (value)
        //        obj.FallingStart = now;
        //}

        //public void IsJumping(DrawableObject obj, DateTime now, bool value)
        //{
        //    obj.IsJumping = value;
        //    if (value)
        //        obj.FallingStart = now;
        //}
    }
}
