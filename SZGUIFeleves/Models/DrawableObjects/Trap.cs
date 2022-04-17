using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models.DrawableObjects
{
    public class Trap : Rectangle
    {
        public double SpeedX { get; set; }
        public double SpeedY { get; set; }
        public bool IsMoving { get; set; }


    }
}
