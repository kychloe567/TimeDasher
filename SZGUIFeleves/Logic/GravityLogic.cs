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
        private const double GravityCoe = 10;

        public void Falling(DrawableObject obj)
        {
            if (obj != null)
            {
                double tmp = Gravity(0, GravityCoe, obj.TimeElapsed);
                obj.Position.y += tmp;
            }
        }

        public void Jumping(DrawableObject obj)
        {
            if (obj != null)
            {
                // While object is rising.
                if (Gravity(25, GravityCoe, obj.TimeElapsed) > 0)
                {
                    double tmp = Gravity(25, GravityCoe, obj.TimeElapsed);
                    obj.Position.y -= tmp;
                }
            }
        }
        public void IsFalling(DrawableObject obj, DateTime now, bool value)
        {
            obj.IsFalling = value;
            if (value)
                obj.FallingStart = now;
        }

        public void IsJumping(DrawableObject obj, DateTime now, bool value)
        {
            obj.IsJumping = value;
            if (value)
                obj.FallingStart = now;
        }
        }
    }
}
