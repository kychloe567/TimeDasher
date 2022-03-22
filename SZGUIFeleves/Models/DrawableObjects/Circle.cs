using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public class Circle : DrawableObject
    {
        public double Radius { get; set; }

        #region Constructors
        public Circle() : base() { }

        public Circle(Vec2d position, double radius) : base(position)
        {
            Radius = radius;
        }

        public Circle(Vec2d position, double radius, Color color) : base(position, color)
        {
            Radius = radius;
        }
        #endregion

        public override Vec2d GetMiddle()
        {
            return new Vec2d(Position.x, Position.y);
        }

        public override bool IsVisible(Camera camera)
        {
            Vec2d centeredPos = camera.CenteredPosition;
            if (Position.x + Radius >= centeredPos.x && Position.x - Radius < centeredPos.x + camera.WindowSize.x &&
               Position.y + Radius >= centeredPos.y && Position.y - Radius < centeredPos.y + camera.WindowSize.y)
                return true;
            else return false;
        }

        public override Circle GetCopy()
        {
            Circle c = new Circle(new Vec2d(Position), Radius, new Color(Color))
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
                c.Texture = Texture.Clone();

            if (StateMachine != null)
                c.StateMachine = StateMachine.GetCopy();

            return c;
        }
    }
}
