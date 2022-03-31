using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public class Rectangle : DrawableObject
    {
        public Vec2d OrigSize { get; set; }
        public Vec2d Size { get; set; }
        public Vec2d CornerRadius { get; set; }
        public bool IsRounded { get; set; }

        #region Constructors
        public Rectangle() : base()
        {
            Size = new Vec2d();
            IsRounded = false;
        }

        public Rectangle(Vec2d position, Vec2d size) : base(position)
        {
            Size = size;
            IsRounded = false;
        }

        public Rectangle(Vec2d position, Vec2d size, Color color) : base(position, color)
        {
            Size = size;
            IsRounded = false;
        }
        #endregion

        public override Vec2d GetMiddle()
        {
            return new Vec2d((Position.x + Size.x / 2), (Position.y + Size.y / 2));
        }

        public override bool IsVisible(Camera camera)
        {
            Vec2d centeredPos = camera.CenteredPosition;
            if (Position.x + Size.x >= centeredPos.x && Position.x < centeredPos.x + camera.WindowSize.x &&
               Position.y + Size.y >= centeredPos.y && Position.y < centeredPos.y + camera.WindowSize.y)
                return true;
            else return false;
        }

        public override Rectangle GetCopy()
        {
            Rectangle r = new Rectangle(new Vec2d(Position), new Vec2d(Size), new Color(Color))
            {
                OrigSize = OrigSize,
                Rotation = Rotation,
                OutLineThickness = OutLineThickness,
                OutLineColor = new Color(OutLineColor),
                IsFilled = IsFilled,
                DrawPriority = DrawPriority,
                IsAffectedByCamera = IsAffectedByCamera,
                IsPlayer = IsPlayer
            };

            if (Texture != null)
                r.Texture = Texture.Clone();

            if (StateMachine != null)
                r.StateMachine = StateMachine.GetCopy();

            return r;
        }
    }
}
