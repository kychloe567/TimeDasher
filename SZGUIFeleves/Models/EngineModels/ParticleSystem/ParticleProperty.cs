using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public class ParticleProperty
    {
        // Particle Properties
        public DrawableObject Shape { get; set; }
        public Vec2d Position { get; set; }
        public double SpeedStart { get; set; }
        public double SpeedEnd { get; set; }
        public double SpeedLimit { get; set; }
        public Color ColorStart { get; set; }
        public Color ColorEnd { get; set; }

        /// <summary>
        /// In degrees
        /// </summary>
        public double RotationStart { get; set; }
        /// <summary>
        /// In degrees
        /// </summary>
        public double RotationEnd { get; set; }

        public double LifeTime { get; set; }

        // Emitter Properties
        public double EmittingDelay { get; set; }

        /// <summary>
        /// Multiplies the count of the emitted particles per emit
        /// </summary>
        public double EmittingMultiplier { get; set; }

        /// <summary>
        /// Base angle which direction the particles go (+EmittingAngleVariation)
        /// </summary>
        public double EmittingAngle { get; set; }

        public double EmittingTime { get; set; }

        /// <summary>
        /// I.e EmittingAngleVariation = 5
        /// <para>Angle then can be x-5 < x < x+5</para>
        /// </summary>
        public int EmittingAngleVariation { get; set; }

        public Vec2d EmittingPositionVariation { get; set; }
        public double EmittingSpeedVariation { get; set; }
        public bool EmittingOnlyByUser { get; set; }
        public double Gravity { get; set; }

        public ParticleProperty()
        {
            Position = new Vec2d();
            Shape = new Circle(Position, 1, Color.White);
            SpeedLimit = -1;
            ColorStart = Color.White;
            ColorEnd = Color.White;
            LifeTime = 0;

            EmittingDelay = 1;
            EmittingMultiplier = 1;
            EmittingPositionVariation = new Vec2d();
            EmittingOnlyByUser = false;
            EmittingTime = 1;
            Gravity = -1;
        }
    }
}
