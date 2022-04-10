using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Models;

namespace SZGUIFeleves.Logic
{
    internal class MovementLogic
    {
        private Vec2d Gravity = new Vec2d(0, 2);

        public void Move(DrawableObject objToMove, Vec2d direction, double velocity)
        {
            objToMove.Position += direction * velocity;
        }

        public void GravityMove(DrawableObject objToMove, double timeElapsed)
        {
            objToMove.Position += objToMove.Velocity * timeElapsed;
            objToMove.Velocity += Gravity * timeElapsed;
        }
    }
}
