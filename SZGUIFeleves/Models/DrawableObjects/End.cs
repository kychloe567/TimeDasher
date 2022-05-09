using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models.DrawableObjects
{
    public class End : Rectangle
    {
        public End(Vec2d Position, Vec2d Size) : base(Position, Size) { }

        public End(DrawableObject d)
        {
            Position = d.Position;
            Size = (d as Rectangle).Size;
            Rotation = d.Rotation;
            OutLineThickness = d.OutLineThickness;
            OutLineColor = new Color(d.OutLineColor);
            IsFilled = d.IsFilled;
            IsAffectedByCamera = d.IsAffectedByCamera;
            IsPlayer = d.IsPlayer;
            ObjectType = d.ObjectType;
            Velocity = d.Velocity;

            if (d.DrawPriority.Type == DrawPriority.PriorityType.Custom)
            {
                DrawPriority = DrawPriority.Custom(d.DrawPriority.CustomPriority);
            }

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

        public override End GetCopy()
        {
            End t = new End(new Vec2d(Position), new Vec2d(Size))
            {
                Rotation = Rotation,
                OutLineThickness = OutLineThickness,
                OutLineColor = new Color(OutLineColor),
                IsFilled = IsFilled,
                IsAffectedByCamera = IsAffectedByCamera,
                IsPlayer = IsPlayer,
                ObjectType = ObjectType,
                Velocity = Velocity
            };

            if (DrawPriority.Type == DrawPriority.PriorityType.Custom)
            {
                t.DrawPriority = DrawPriority.Custom(DrawPriority.CustomPriority);
            }

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
