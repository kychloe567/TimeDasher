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

        //For the moving traps, examined in Logic
        public bool IsMoving { get; set; }

        public Trap() { }
        public Trap(Vec2d Position, Vec2d Size)
        {
            this.Position = Position;
            this.Size = Size;
        }
        public Trap(DrawableObject d)
        {
            Position = d.Position;
            Size = (d as Rectangle).Size;
            Rotation = d.Rotation;
            OutLineThickness = d.OutLineThickness;
            OutLineColor = new Color(d.OutLineColor);
            IsFilled = d.IsFilled;
            DrawPriority = d.DrawPriority;
            IsAffectedByCamera = d.IsAffectedByCamera;
            IsPlayer = d.IsPlayer;
            ObjectType = d.ObjectType;
            Velocity = d.Velocity;

            if (d.Texture != null)
            {
                Texture = d.Texture.Clone();
                TexturePath = d.TexturePath;
            }

            if (d.StateMachine != null)
            {
                StateMachine = d.StateMachine.GetCopy();
                d.StateMachine.Start();
            }
        }

        public override Trap GetCopy()
        {
            Trap t = new Trap(new Vec2d(Position), new Vec2d(Size))
            {
                Rotation = Rotation,
                OutLineThickness = OutLineThickness,
                OutLineColor = new Color(OutLineColor),
                IsFilled = IsFilled,
                DrawPriority = DrawPriority,
                IsAffectedByCamera = IsAffectedByCamera,
                IsPlayer = IsPlayer,
                ObjectType = ObjectType,
                Velocity = Velocity
            };

            if (Texture != null)
            {
                t.Texture = Texture.Clone();
                t.TexturePath = TexturePath;
            }

            if (StateMachine != null)
                t.StateMachine = StateMachine.GetCopy();

            return t;
        }

    }
}
