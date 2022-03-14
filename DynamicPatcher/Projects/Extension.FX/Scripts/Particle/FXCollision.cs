using Extension.FX.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Scripts.Particle
{
    [FXPostDependency(nameof(FXSolveForcesAndVelocity))]
    public class FXCollision : FXScript
    {
        public FXCollision(FXSystem system, FXEmitter emitter) : base(system, emitter)
        {
        }

        public override FXScript Clone(FXSystem system = null, FXEmitter emitter = null)
        {
            var collision = new FXCollision(system ?? System, emitter ?? Emitter);

            return collision;
        }

        public override void ParticleUpdate(FXParticle particle)
        {
            // Init indirection var and protect from NANs
            var collisionMassReplacement = Math.Max(particle.Mass, 0.001);

            // Simulation Stage Support
            // ----------------------------
            // Notes on work -in-progress implementation of simulation stage integration:
            // Post solve velocities are ignored on collisions.
            // Velocity and force could be could be clamped in the solver, which would break this solver's assumptions.
            // Previous position should be used for trace starts.

            Vector3 collision_ParticlePositionReplacement;
            Vector3 collision_PredictedParticlePositionForSimStages;
            Vector3 collision_ParticleVelocityReplacement;
            Vector3 collision_ParticlePhysicsForceReplacement;

            collision_ParticlePositionReplacement = particle.Position;
            collision_ParticleVelocityReplacement = particle.Velocity;
            collision_ParticlePhysicsForceReplacement = particle.PhysicsForce;
    }
    }
}
