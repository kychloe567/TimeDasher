using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Models;

namespace SZGUIFeleves.Helpers
{
    public static class MathHelper
    {
        public static double ConvertToRadians(double angle)
        {
            return Math.PI / 180 * angle;
        }

        public static double ConvertToDegrees(double angle)
        {
            return angle * 180 / Math.PI;
        }

        /// <summary>
        /// Converts the given angle to [0,360]
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double NormalizeAngle(double angle)
        {
            double newAngle = angle;

            if (newAngle > 360)
            {
                while (newAngle > 360)
                    newAngle -= 360;
            }
            else if (newAngle < 0)
            {
                while (newAngle < 0)
                    newAngle += 360;
            }

            return newAngle;
        }

        public static double MapFunction(double x, double inMin, double inMax, double outMin, double outMax)
        {
            return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }

        public static double Clamp(double low, double high, double value)
        {
            if (value >= high)
                return high;
            else if (value <= low)
                return low;
            else
                return value;
        }

        public static Rectangle NormalizeSize(Rectangle rect)
        {
            if (rect.Size.x >= 0 && rect.Size.y >= 0)
                return rect;

            Rectangle toReturn = rect.GetCopy();
            if(rect.Size.x < 0)
            {
                toReturn.Position.x += rect.Size.x;
                toReturn.Size.x = Math.Abs(rect.Size.x);
            }
            if(rect.Size.y < 0)
            {
                toReturn.Position.y += rect.Size.y;
                toReturn.Size.y = Math.Abs(rect.Size.y);
            }

            return toReturn;
        }
    }
}
