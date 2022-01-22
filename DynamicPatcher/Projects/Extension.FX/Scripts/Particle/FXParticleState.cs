using Extension.FX.Definitions;
using Extension.FX.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Scripts.Particle
{
    public class FXParticleState : FXScript
    {
        public FXParticleState(FXSystem system, FXEmitter emitter) : base(system, emitter)
        {
            //var lifetimeLink = new FXLinkParameter<float>(nameof(Lifetime), "Particle.Lifetime");
            //lifetimeLink.SetContext(system, emitter, this);
            //Lifetime = lifetimeLink;
        }

        // Input Parameters

        public bool KillParticlesWhenLifetimeHasElapsed { get; set; } = true;
        public bool LoopParticlesLifetime { get; set; } = false;
        public bool LetInfinitelyLivedParticlesDieWhenEmitterDeactivates { get; set; } = false;
        //public FXParameter<float> Lifetime { get; set; }
        public float DeltaTime { get; set; } = FXEngine.DeltaTime;

        public override FXScript Clone(FXSystem system = null, FXEmitter emitter = null)
        {
            var state = new FXParticleState(system ?? System, emitter ?? Emitter);
            state.KillParticlesWhenLifetimeHasElapsed = KillParticlesWhenLifetimeHasElapsed;
            //state.Lifetime = Lifetime.Clone(state);
            state.DeltaTime = DeltaTime;

            return state;
        }

        public override void ParticleUpdate(FXParticle particle)
        {
            bool shouldInactive = System.ExecutionState != FXExecutionState.Active || Emitter.ExecutionState != FXExecutionState.Active;
            shouldInactive &= LetInfinitelyLivedParticlesDieWhenEmitterDeactivates;

            var nextAge = particle.Age + DeltaTime;
            var safeLifetime = Math.Max(/*Lifetime*/particle.Lifetime, 0.00001f); // default to Particle.Lifetime
            var safeLifetime_smaller = safeLifetime - 0.0001f;

            if (KillParticlesWhenLifetimeHasElapsed)
            {
                particle.Age = nextAge;

                if(nextAge >= safeLifetime_smaller)
                {
                    particle.Alive = false;
                }
            }
            else
            {
                // UpdateAge for infinitely lived particles

                if (LoopParticlesLifetime && !shouldInactive)
                {
                    particle.Age = nextAge % safeLifetime;
                }
                else
                {
                    particle.Age = nextAge;
                }

                if (particle.Age > safeLifetime_smaller && shouldInactive)
                {
                    particle.Alive = false;
                }
            }

            particle.NormalizedAge = particle.Age / safeLifetime;
        }
    }
}
