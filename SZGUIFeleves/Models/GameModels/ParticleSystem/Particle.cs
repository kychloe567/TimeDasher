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
        public double SpeedStart { get; set; }
        public double SpeedEnd { get; set; }
        public Color ColorStart { get; set; }
        public Color ColorEnd { get; set; }
        public double RotationStart { get; set; }
        public double RotationEnd { get; set; }

        public double LifeTime { get; set; }
        public double RemainingLifeTime { get; set; }

        public Particle(ParticleProperty particleProperty, double angle, Vec2d Position)
        {
            Shape = particleProperty.Shape.GetCopy();
            Shape.Position = Position;
            Angle = angle;
            SpeedStart = particleProperty.SpeedStart;
            SpeedEnd = particleProperty.SpeedEnd;
            ColorStart = new Color(particleProperty.ColorStart);
            ColorEnd = new Color(particleProperty.ColorEnd);
            RotationStart = particleProperty.RotationStart;
            RotationEnd = particleProperty.RotationEnd;
            LifeTime = particleProperty.LifeTime;
            RemainingLifeTime = LifeTime;
        }

        public void Update(double Elapsed)
        {
            RemainingLifeTime -= Elapsed;
            double ts = 1 - RemainingLifeTime / LifeTime;

            double currentSpeed = (double)Interpolater.GetInterpolatedValue(SpeedStart, SpeedEnd, ts) * Elapsed;
            double angleRad = MathHelper.ConvertToRadians(Angle);
            Vec2d velocity = new Vec2d(Math.Cos(angleRad), Math.Sin(angleRad)) * currentSpeed;

            Shape.Position += velocity;
            Shape.Rotation = (double)Interpolater.GetInterpolatedValue(RotationStart, RotationEnd, ts);

            if(Shape.Texture is null)
                Shape.Color = (Color)Interpolater.GetInterpolatedValue(ColorStart, ColorEnd, ts);
            else
                Shape.TextureOpacity = (double)Interpolater.GetInterpolatedValue((double)ColorStart.A, (double)ColorEnd.A, ts) / 255.0f;
        }
    }
}
