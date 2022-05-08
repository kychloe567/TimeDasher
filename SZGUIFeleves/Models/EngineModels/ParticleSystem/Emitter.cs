using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Helpers;

namespace SZGUIFeleves.Models
{
    public class Emitter
    {
        private Random rnd = new Random((int)DateTime.Now.Ticks);

        public List<Particle> Particles { get; private set; }
        private ParticleProperty ParticleProperty { get; set; }
        private double LastEmitted { get; set; }
        public double EmittingTime { get; set; }

        public Emitter(ParticleProperty particleProperty)
        {
            EmittingTime = particleProperty.EmittingTime;
            ParticleProperty = particleProperty;
            LastEmitted = particleProperty.EmittingDelay;

            Particles = new List<Particle>();
        }

        public void Update(double Elapsed)
        {
            if (!ParticleProperty.EmittingOnlyByUser)
            {
                EmittingTime -= Elapsed;
                LastEmitted += Elapsed;
                if (LastEmitted >= ParticleProperty.EmittingDelay)
                {
                    for (int i = 0; i < ParticleProperty.EmittingMultiplier; i++)
                    {
                        double angle = ParticleProperty.EmittingAngle + rnd.Next(-ParticleProperty.EmittingAngleVariation, ParticleProperty.EmittingAngleVariation + 1);
                        Vec2d position = ParticleProperty.Position + new Vec2d(rnd.Next(-(int)ParticleProperty.EmittingPositionVariation.x, (int)ParticleProperty.EmittingPositionVariation.x),
                                                                               rnd.Next(-(int)ParticleProperty.EmittingPositionVariation.y, (int)ParticleProperty.EmittingPositionVariation.y));
                        double speed = ParticleProperty.SpeedStart + rnd.Next((int)-ParticleProperty.EmittingSpeedVariation, (int)ParticleProperty.EmittingSpeedVariation);

                        Particle p = new Particle(ParticleProperty, angle, position, speed);
                        Particles.Add(p);
                    }

                    LastEmitted = 0;
                }
            }

            Particles.RemoveAll(x => x.RemainingLifeTime <= 0);
            foreach(Particle p in Particles)
            {
                if (ParticleProperty.Gravity != -1)
                    p.Update(Elapsed, ParticleProperty.Gravity);
                else
                    p.Update(Elapsed);
            }
        }

        public void Emit(Vec2d Position)
        {
            for (int i = 0; i < ParticleProperty.EmittingMultiplier; i++)
            {
                double angle = ParticleProperty.EmittingAngle + rnd.Next(-ParticleProperty.EmittingAngleVariation, ParticleProperty.EmittingAngleVariation + 1);
                Vec2d position = Position + new Vec2d(rnd.Next(-(int)ParticleProperty.EmittingPositionVariation.x, (int)ParticleProperty.EmittingPositionVariation.x),
                                                                       rnd.Next(-(int)ParticleProperty.EmittingPositionVariation.y, (int)ParticleProperty.EmittingPositionVariation.y));
                double speed = ParticleProperty.SpeedStart + rnd.Next((int)-ParticleProperty.EmittingSpeedVariation, (int)ParticleProperty.EmittingSpeedVariation);

                Particle p = new Particle(ParticleProperty, angle, position, speed);
                Particles.Add(p);
            }
        }
    }
}
