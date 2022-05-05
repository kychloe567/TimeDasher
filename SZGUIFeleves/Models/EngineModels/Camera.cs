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

            if (Position.x == 0 && Position.y == 0)
            {
                Position = nextPosition;
                TargetPosition = Position;
            }

            if (nextPosition.x <= Position.x - DeadZone.x / 2 || nextPosition.x > Position.x + DeadZone.x / 2)
            {
                if (nextPosition.x > Position.x)
                    delta.x = targetPosition.x - (Position.x + DeadZone.x / 2);
                else
                    delta.x = targetPosition.x - (Position.x - DeadZone.x / 2);

            }

            if (nextPosition.y <= Position.y - DeadZone.y / 2 || nextPosition.y > Position.y + DeadZone.y / 2)
            {
                if (nextPosition.y > Position.y)
                    delta.y = targetPosition.y - (Position.y + DeadZone.y / 2);
                else
                    delta.y = targetPosition.y - (Position.y - DeadZone.y / 2);

            }

            // TODO: Better damping
            // TODO: Look ahead
            if (delta.x != 0 || delta.y != 0)
            {
                Position = Position + delta*elapsed*5;

                return;
                //Vec2d vel = targetPosition - Position;

                //if (Damping.x == 0)
                //{
                //    Position.x += delta.x;
                //}
                //else
                //{
                //    Position.x += (targetPosition.x - Position.x) * elapsed / Damping.x;
                //}

                //if (Damping.y == 0)
                //{
                //    Position.y += delta.y;
                //}
                //else
                //{
                //    Position.x += (targetPosition.y - Position.y) * elapsed / Damping.x;
                //}
            }
        }
    }
}
