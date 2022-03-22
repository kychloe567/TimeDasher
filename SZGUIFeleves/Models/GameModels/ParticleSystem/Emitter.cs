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
                for (int i = 0; i < ParticleProperty.EmittingMultiplier; i++)
                {
                    double angle = ParticleProperty.EmittingAngle + rnd.Next(-ParticleProperty.EmittingAngleVariation, ParticleProperty.EmittingAngleVariation + 1);
                    Vec2d position = ParticleProperty.Position + new Vec2d(rnd.Next(-(int)ParticleProperty.EmittingPositionVariation.x, (int)ParticleProperty.EmittingPositionVariation.x),
                                                                           rnd.Next(-(int)ParticleProperty.EmittingPositionVariation.y, (int)ParticleProperty.EmittingPositionVariation.y));

                    Particle p = new Particle(ParticleProperty, angle, position);
                    Particles.Add(p);
                }

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
