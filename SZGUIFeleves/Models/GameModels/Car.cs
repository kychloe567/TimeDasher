using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Helpers;

namespace SZGUIFeleves.Models
{
    public class Car
    {
        public Vec2d Pos;
        public Vec2d Vel;
        public Vec2d Acc;
        public double RotationAngle { get; set; }
        public double RotationAngleRad
        {
            get
            {
                return MathHelper.ConvertToRadians(RotationAngle);
            }
        }

        public Vec2d Size;

        public Rectangle Rect
        {
            get
            {
                Rectangle r = new Rectangle(Pos, Size, Color.White);
                r.Rotation = RotationAngle;
                return r;
            }
        }

        public Car(Vec2d pos, Vec2d size)
        {
            Pos = pos;
            Size = size;
            Acc = new Vec2d();
            Vel = new Vec2d();
            RotationAngle = 0;
        }

        public void Move(double speed)
        {
            Acc += new Vec2d(Math.Cos(RotationAngleRad) * speed, Math.Sin(RotationAngleRad) * speed);
        }

        public void Brake()
        {
            Vel *= 0.7f;
        }

        public void Update()
        {
            Vel += Acc;
            Pos += Vel;

            Vel *= 0.9f;

            Acc.Zero();
        }
    }
}
