using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Helpers;

namespace SZGUIFeleves.Models
{
    public class ParticleProperty
    {
        // Particle Properties
        public Vec2d Position { get; set; }
        public double SpeedStart { get; set; }
        public double SpeedEnd { get; set; }
        /// <summary>
        /// In degrees
        /// </summary>
        public Color ColorStart { get; set; }
        public Color ColorEnd { get; set; }
        public double RotationStart { get; set; }
        public double RotationEnd { get; set; }
        public double LifeTime { get; set; }

        // Emitter Properties
        public double EmittingDelay { get; set; }
        public double EmittingAngle { get; set; }

        /// <summary>
        /// I.e EmittingAngleVariation = 5
        /// <para>Angle then can be x-5 < x < x+5</para>
        /// </summary>
        public int EmittingAngleVariation { get; set; }

        public ParticleProperty() { }
    }

    public class Particle : Circle
    {
        public double Angle { get; set; }
        public double SpeedStart { get; set; }
        public double SpeedEnd { get; set; }
        public Color ColorStart { get; set; }
        public Color ColorEnd { get; set; }
        public double RotationStart { get; set; }
        public double RotationEnd { get; set; }

        public double LifeTime { get; set; }
        public double RemainingLifeTime { get; set; }

        public Particle(ParticleProperty particleProperty, double angle)
        {
            Position = new Vec2d(particleProperty.Position);
            Angle = angle;
            SpeedStart = particleProperty.SpeedStart;
            SpeedEnd = particleProperty.SpeedEnd;
            ColorStart = new Color(particleProperty.ColorStart);
            ColorEnd = new Color(particleProperty.ColorEnd);
            RotationStart = particleProperty.RotationStart;
            RotationEnd = particleProperty.RotationEnd;
            LifeTime = particleProperty.LifeTime;
            RemainingLifeTime = LifeTime;

            // TODO: Radius better management, maybe not only circle?
            Radius = 5;
        }

        public void Update(double Elapsed)
        {
            RemainingLifeTime -= Elapsed;
            double ts = 1 - RemainingLifeTime / LifeTime;

            double currentSpeed = (double)Interpolater.GetInterpolatedValue(SpeedStart, SpeedEnd, ts) * Elapsed;
            double angleRad = MathHelper.ConvertToRadians(Angle);
            Vec2d velocity = new Vec2d(Math.Cos(angleRad), Math.Sin(angleRad)) * currentSpeed;

            Position += velocity;
            Rotation = (double)Interpolater.GetInterpolatedValue(RotationStart, RotationEnd, ts);
            Color = (Color)Interpolater.GetInterpolatedValue(ColorStart, ColorEnd, ts);
        }
    }

    public class Emitter
    {
        private Random rnd = new Random((int)DateTime.Now.Ticks);

        public List<Particle> Particles { get; private set; }
        private ParticleProperty ParticleProperty { get; set; }
        private double LastEmitted { get; set; }

        public Emitter(ParticleProperty particleProperty)
        {
            ParticleProperty = particleProperty;
            LastEmitted = particleProperty.EmittingDelay;

            Particles = new List<Particle>();
        }

        public void Update(double Elapsed)
        {
            LastEmitted += Elapsed;
            if(LastEmitted >= ParticleProperty.EmittingDelay)
            {
                double angle = ParticleProperty.EmittingAngle + rnd.Next(-ParticleProperty.EmittingAngleVariation, ParticleProperty.EmittingAngleVariation + 1);
                Particle p = new Particle(ParticleProperty, angle);
                Particles.Add(p);

                LastEmitted = 0;
            }

            Particles.RemoveAll(x => x.RemainingLifeTime <= 0);
            foreach(Particle p in Particles)
            {
                p.Update(Elapsed);
            }
        }
    }
}
