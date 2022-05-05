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
            return new Vec2d(Position.x, (Position.y + Size.y / 2));
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
            return new Vec2d(Position.x + Size.x, (Position.y + Size.y / 2));
        }

        public override Player GetCopy()
        {
            Player p = new Player();
            var props = typeof(Rectangle).GetProperties().Where(p => !p.GetIndexParameters().Any());
            foreach (var prop in props)
            {
                if (prop.CanWrite && prop.Name != "Position")
                    prop.SetValue(p, prop.GetValue(this));
            }
            p.Position = new Vec2d(Position);

            return p;
        }
    }
}
