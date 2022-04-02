using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Models;

namespace SZGUIFeleves.Logic
{
    internal class GravityLogic : IGravityLogic
    {
        private const double GravityCoe = 9.81;

        public void Falling(DrawableObject obj)
        {
            if (obj != null)
            {
                obj.Position.y += Gravity(obj.TimeElapsed);
            }
        }

        private double Gravity(double timeElapsed)
        {
            return GravityCoe * Math.Pow(timeElapsed, 2);
        }
    }
}
