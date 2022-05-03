using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public class Camera
    {
        public Vec2d Position { get; set; }
        public Vec2d TargetPosition { get; set; }
        public Vec2d CenteredPosition
        {
            get
            {
                return Position - WindowSize/2;
            }
        }
        public Vec2d WindowSize { get; set; }

        // Settings
        public double Zoom { get; set; }
        public Vec2d Offset { get; set; }
        public Vec2d DeadZone { get; set; }
        public Vec2d Damping { get; set; }
        public double LookAheadTime { get; set; }

        public Camera(Vec2d windowSize)
        {
            Position = new Vec2d();
            WindowSize = windowSize;

            Zoom = 1.0f;
            Offset = new Vec2d();
            DeadZone = new Vec2d();
            Damping = new Vec2d();
            LookAheadTime = 0.0f;
        }
        public Camera(Vec2d position, Vec2d windowSize)
        {
            Position = position;
            WindowSize = windowSize;

            Zoom = 1.0f;
            Offset = new Vec2d();
            DeadZone = new Vec2d();
            Damping = new Vec2d();
            LookAheadTime = 0.0f;
        }

        public void UpdatePosition(Vec2d targetPosition, double elapsed)
        {
            Vec2d nextPosition = targetPosition + Offset;
            Vec2d delta = new Vec2d();

            if (Position.X == 0 && Position.Y == 0)
            {
                Position = nextPosition;
                TargetPosition = Position;
            }

            if (nextPosition.X <= Position.X - DeadZone.X / 2 || nextPosition.X > Position.X + DeadZone.X / 2)
            {
                if (nextPosition.X > Position.X)
                    delta.X = targetPosition.X - (Position.X + DeadZone.X / 2);
                else
                    delta.X = targetPosition.X - (Position.X - DeadZone.X / 2);

            }

            if(nextPosition.Y <= Position.Y - DeadZone.Y / 2 || nextPosition.Y > Position.Y + DeadZone.Y / 2)
            {
                if (nextPosition.Y > Position.Y)
                    delta.Y = targetPosition.Y - (Position.Y + DeadZone.Y / 2);
                else
                    delta.Y = targetPosition.Y - (Position.Y - DeadZone.Y / 2);

            }

            // TODO: Better damping
            // TODO: Look ahead
            if(delta.X != 0 || delta.Y != 0)
            {
                TargetPosition = Position + delta;

                Vec2d vel = targetPosition - Position;

                if (Damping.X == 0)
                {
                    Position.X += delta.X;
                }
                else
                {
                    Position.X += (targetPosition.X - Position.X) * elapsed / Damping.X;
                }

                if (Damping.Y == 0)
                {
                    Position.Y += delta.Y;
                }
                else
                {
                    Position.X += (targetPosition.Y - Position.Y) * elapsed / Damping.X;
                }
            }
        }
    }
}
