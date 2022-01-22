using Extension.FX.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Scripts.Particle
{
    public class FXInitializeParticle : FXScript
    {
        public FXInitializeParticle(FXSystem system, FXEmitter emitter) : base(system, emitter)
        {
        }

        // Input Parameters

        public FXLifetimeMode LifetimeMode { get; set; } = FXLifetimeMode.Random;
        public float Lifetime { get; set; } = 1;
        public float LifetimeMin { get; set; } = 1;
        public float LifetimeMax { get; set; } = 2.25f;

        public FXMassInitializationMode MassMode { get; set; } = FXMassInitializationMode.DirectSet;
        public float Mass { get; set; } = 1;
        public float MassMin { get; set; } = 0.75f;
        public float MassMax { get; set; } = 5.0f;

        public override FXScript Clone(FXSystem system = null, FXEmitter emitter = null)
        {
            var initializeParticle = new FXInitializeParticle(system ?? System, emitter ?? Emitter);

            initializeParticle.LifetimeMode = LifetimeMode;
            initializeParticle.Lifetime = Lifetime;
            initializeParticle.LifetimeMin = LifetimeMin;
            initializeParticle.LifetimeMax = LifetimeMax;

            initializeParticle.MassMode = MassMode;
            initializeParticle.Mass = Mass;
            initializeParticle.MassMin = MassMin;
            initializeParticle.MassMax = MassMax;

            return initializeParticle;
        }

        public override void ParticleSpawn(FXParticle particle)
        {
            particle.Position = Emitter.WorldPosition;

            switch (LifetimeMode)
            {
                case FXLifetimeMode.DirectSet:
                    particle.Lifetime = Lifetime;
                    break;
                case FXLifetimeMode.Random:
                    particle.Lifetime = FXEngine.CalculateRandomRange(LifetimeMin, LifetimeMax);
                    break;
            }

            switch (MassMode)
            {
                case FXMassInitializationMode.DirectSet:
                    particle.Mass = Mass;
                    break;
                case FXMassInitializationMode.Random:
                    particle.Mass = FXEngine.CalculateRandomRange(MassMin, MassMax);
                    break;
            }
        }
    }
}
