using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Helpers;

namespace SZGUIFeleves.Models
{
    public class Particle
    {
        public DrawableObject Shape { get; set; }
        public object OriginalSize { get; set; }
        public double Angle { get; set; }
        public Vec2d Velocity { get; set; }
        public double SpeedStart { get; set; }
        public double SpeedEnd { get; set; }
        public double SpeedLimit { get; set; }
        public Color ColorStart { get; set; }
        public Color ColorEnd { get; set; }
        public double RotationStart { get; set; }
        public double RotationEnd { get; set; }

        public double LifeTime { get; set; }
        public double RemainingLifeTime { get; set; }

        public Particle(ParticleProperty particleProperty, double angle, Vec2d Position, double speed)
        {
            Shape = particleProperty.Shape.GetCopy();
            Shape.Position = Position;
            Angle = angle;
            SpeedStart = speed;
            SpeedEnd = particleProperty.SpeedEnd;
            SpeedLimit = particleProperty.SpeedLimit;
            ColorStart = new Color(particleProperty.ColorStart);
            ColorEnd = new Color(particleProperty.ColorEnd);
            RotationStart = particleProperty.RotationStart;
            RotationEnd = particleProperty.RotationEnd;
            LifeTime = particleProperty.LifeTime;
            RemainingLifeTime = LifeTime;

            double angleRad = MathHelper.ConvertToRadians(Angle);
            Velocity = new Vec2d(Math.Cos(angleRad), Math.Sin(angleRad)) * speed;
        }

        public void Update(double elapsed, double gravity)
        {
            RemainingLifeTime -= elapsed;
            double ts = 1 - RemainingLifeTime / LifeTime;

            Velocity += new Vec2d(0, gravity);
            if(SpeedLimit != -1 && Velocity.Length > SpeedLimit)
                Velocity = Velocity.Normalised() * SpeedLimit;

            Update_(elapsed, ts);
        }

        public void Update(double elapsed)
        {
            RemainingLifeTime -= elapsed;
            double ts = 1 - RemainingLifeTime / LifeTime;

            double currentSpeed = (double)Interpolater.GetInterpolatedValue(SpeedStart, SpeedEnd, ts);
            Velocity = Velocity.Normalised() * currentSpeed;
            if (SpeedLimit != -1 && Velocity.Length > SpeedLimit)
                Velocity = Velocity.Normalised() * SpeedLimit;

            Update_(elapsed, ts);
        }

        private void Update_(double elapsed, double ts)
        {
            Shape.Position += Velocity * elapsed;
            Shape.Rotation = (double)Interpolater.GetInterpolatedValue(RotationStart, RotationEnd, ts);

            if (Shape.Texture is null)
                Shape.Color = (Color)Interpolater.GetInterpolatedValue(ColorStart, ColorEnd, ts);
            else
                Shape.TextureOpacity = (double)Interpolater.GetInterpolatedValue((double)ColorStart.A, (double)ColorEnd.A, ts) / 255.0f;
        }
    }
}
