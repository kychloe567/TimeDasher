using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models.DrawableObjects
{
    public class Checkpoint : Rectangle
    {
        public Checkpoint(Vec2d pos, Vec2d size) : base(pos, size) { }

        public override Checkpoint GetCopy()
        {
            Checkpoint cp = new Checkpoint(new Vec2d(Position), new Vec2d(Size))
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
                cp.DrawPriority = DrawPriority.Custom(DrawPriority.CustomPriority);
            }

            if (Texture != null)
            {
                cp.Texture = Texture.Clone();
                cp.TexturePath = TexturePath;
            }

            if (StateMachine != null)
                cp.StateMachine = StateMachine.GetCopy();

            return cp;
        }
    }
}
