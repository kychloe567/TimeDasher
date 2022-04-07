using System;
using SZGUIFeleves.Models;

namespace SZGUIFeleves.Logic
{
    internal interface IGravityLogic
    {
        void Falling(DrawableObject obj);
        void Jumping(DrawableObject obj, double v0);
        //void IsFalling(DrawableObject obj, DateTime now, bool value);
        //void IsJumping(DrawableObject obj, DateTime now, bool value);
    }
}