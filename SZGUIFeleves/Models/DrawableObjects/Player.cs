using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models.DrawableObjects
{
    public class Player : DrawableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double SpeedX { get; set; }
        public double SpeedY { get; set; }
        

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Player && (obj as Player).Id == this.Id)
            {
                return true;
            }
            return false;
        }

        public override DrawableObject GetCopy()
        {
            throw new NotImplementedException();
        }

        public override Vec2d GetMiddle()
        {
            throw new NotImplementedException();
        }

        public override bool Intersects(DrawableObject d)
        {
            throw new NotImplementedException();
        }

        public override bool IsVisible(Camera camera)
        {
            throw new NotImplementedException();
        }
    }
}
