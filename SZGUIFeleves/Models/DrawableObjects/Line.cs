using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public class Line : DrawableObject
    {
        public Vec2d Position2 { get; set; }
        public double Width { get; set; }

        #region Constructors
        public Line() : base()
        {
            Position2 = new Vec2d();
            Width = 1.0f;
        }

        public Line(Vec2d position, Vec2d position2) : base(position)
        {
            Position2 = position2;
            Width = 1.0f;
        }

        public Line(Vec2d position, Vec2d position2, Color color) : base(position, color)
        {
            Position2 = position2;
            Width = 1.0f;
        }

        public Line(Vec2d position, Vec2d position2, double width) : base(position)
        {
            Position2 = position2;
            Width = width;
        }

        public Line(Vec2d position, Vec2d position2, Color color, double width) : base(position, color)
        {
            Position2 = position2;
            Width = width;
        }
        #endregion

        public override Vec2d GetMiddle()
        {
            return new Vec2d((Position.x + Position2.x) / 2, (Position.y + Position2.y) / 2);
        }

        public override bool IsVisible(Camera camera)
        {
            Vec2d centeredPos = camera.CenteredPosition;

            if (Position.x >= centeredPos.x && Position.x < centeredPos.x + camera.WindowSize.x &&
               Position.y >= centeredPos.y && Position.y < centeredPos.y + camera.WindowSize.y)
                return true;
            else if (Position2.x >= centeredPos.x && Position2.x < centeredPos.x + camera.WindowSize.x &&
               Position2.y >= centeredPos.y && Position2.y < centeredPos.y + camera.WindowSize.y)
                return true;
            else return false;
        }

        public override Line GetCopy()
        {
            Line l = new Line(new Vec2d(Position),new Vec2d(Position2), new Color(Color), Width)
            {
                Rotation = Rotation,
                OutLineThickness = OutLineThickness,
                OutLineColor = new Color(OutLineColor),
                IsFilled = IsFilled,
                DrawPriority = DrawPriority,
                IsAffectedByCamera = IsAffectedByCamera,
                IsPlayer = IsPlayer
            };

            if (Texture != null)
                l.Texture = Texture.Clone();

            if (StateMachine != null)
                l.StateMachine = StateMachine.GetCopy();

            return l;
        }
    }
}
