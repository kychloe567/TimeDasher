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
        private Vec2d Gravity = new Vec2d(0, 500);

        public void Move(DrawableObject objToMove, Vec2d direction, double velocity, double timeElapsed = -1)
        {
            objToMove.Position += direction * velocity;
            if (objToMove.IsGravity && timeElapsed > 0)
            {
                objToMove.Position += objToMove.Velocity * timeElapsed;
            }
        }

        // Gravity
        public void Move(DrawableObject objToMove, double timeElapsed)
        {
            objToMove.Position += objToMove.Velocity * timeElapsed * 1.5;
            objToMove.Velocity += Gravity * timeElapsed * 2;
        }
    }
}
