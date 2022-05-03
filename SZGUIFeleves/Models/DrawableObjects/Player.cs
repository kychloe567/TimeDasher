using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models.DrawableObjects
{
    public class Player : Rectangle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double SpeedX { get; set; }
        public double SpeedY { get; set; }

        /// <summary>
        /// ┌---┐
        /// |   |
        /// x   |
        /// |   |
        /// └---┘
        /// </summary>
        /// <returns>Returns a new Vec2d instance.</returns>
        public Vec2d GetMiddleLeft()
        {
            return new Vec2d(Position.X, (Position.Y + Size.Y / 2));
        }

        /// <summary>
        /// ┌---┐
        /// |   |
        /// |   x
        /// |   |
        /// └---┘
        /// </summary>
        /// <returns>Returns a new Vec2d instance.</returns>
        public Vec2d GetMiddleRight()
        {
            return new Vec2d(Position.X + Size.X, (Position.Y + Size.Y / 2));
        }
    }
}
