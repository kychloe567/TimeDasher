using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models.DrawableObjects
{
    public class MovingBackground:DrawableObject
    {
        public Vec2d Size { get; set; }
        public Vec2d CameraPosition { get; set; }
        public Vec2d CameraOffset { get; set; }
        public Vec2d CameraDeadZone { get; set; }
        public Vec2d CameraDamping { get; set; }

        public void UpdatePosition(Vec2d targetPosition, double elapsed)
        {
            Vec2d nextPosition = targetPosition + CameraOffset;
            Vec2d delta = new Vec2d();

            if (Position.X == 0 && Position.Y == 0)
            {
                Position = nextPosition;
                CameraPosition = Position;
            }

            if (nextPosition.X <= Position.X - CameraDeadZone.X / 2 || nextPosition.X > Position.X + CameraDeadZone.X / 2)
            {
                if (nextPosition.X > Position.X)
                    delta.X = targetPosition.X - (Position.X + CameraDeadZone.X / 2);
                else
                    delta.X = targetPosition.X - (Position.X - CameraDeadZone.X / 2);

            }

            if (nextPosition.Y <= Position.Y - CameraDeadZone.Y / 2 || nextPosition.Y > Position.Y + CameraDeadZone.Y / 2)
            {
                if (nextPosition.Y > Position.Y)
                    delta.Y = targetPosition.Y - (Position.Y + CameraDeadZone.Y / 2);
                else
                    delta.Y = targetPosition.Y - (Position.Y - CameraDeadZone.Y / 2);

            }

            // TODO: Better damping
            // TODO: Look ahead
            if (delta.X != 0 || delta.Y != 0)
            {
                CameraPosition = Position + delta;

                Vec2d vel = targetPosition - Position;

                if (CameraDamping.X == 0)
                {
                    Position.X += delta.X;
                }
                else
                {
                    Position.X += (targetPosition.X - Position.X) * elapsed / CameraDamping.X;
                }

                if (CameraDamping.Y == 0)
                {
                    Position.Y += delta.Y;
                }
                else
                {
                    Position.X += (targetPosition.Y - Position.Y) * elapsed / CameraDamping.X;
                }
            }
        }

        public override bool Intersects(DrawableObject d)
        {
            return false;
        }

        public override Vec2d GetMiddle()
        {
            return new Vec2d((Position.X + Size.X / 2), (Position.Y + Size.Y / 2));
        }

        public override bool IsVisible(Camera camera)
        {
            Vec2d centeredPos = camera.CenteredPosition;
            if (Position.X + Size.X >= centeredPos.X && Position.X < centeredPos.X + camera.WindowSize.X &&
               Position.Y + Size.Y >= centeredPos.Y && Position.Y < centeredPos.Y + camera.WindowSize.Y)
                return true;
            else return false;
        }

        public override DrawableObject GetCopy()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (obj is Rectangle r && Position == r.Position && Size == r.Size && Color == r.Color)
                return true;
            return false;
        }
    }
}
