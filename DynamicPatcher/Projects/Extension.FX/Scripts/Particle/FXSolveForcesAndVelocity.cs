using Extension.FX.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Scripts.Particle
{
    public class FXSolveForcesAndVelocity : FXScript
    {
        public FXSolveForcesAndVelocity(FXSystem system, FXEmitter emitter) : base(system, emitter)
        {
        }

        // Input Parameters

        public float? AccelerationLimit { get; set; }
        public float? SpeedLimit { get; set; }

        public override FXScript Clone(FXSystem system = null, FXEmitter emitter = null)
        {
            var solveForcesAndVelocity = new FXSolveForcesAndVelocity(system ?? System, emitter ?? Emitter);

            return solveForcesAndVelocity;
        }

        public override void ParticleUpdate(FXParticle particle)
        {
            var physicsForce = particle.PhysicsForce;
            var physicsDrag = particle.PhysicsDrag;

            // Apply Mass to Physics Force
            physicsForce *= (float)(1 / Math.Max(particle.Mass, 0.0001));

            // Apply Forces to Velocity, Apply Drag irrespective of Mass
            var velocity = particle.Velocity;
            velocity = (physicsForce * FXEngine.DeltaTime + velocity) / (FXEngine.DeltaTime * Math.Max(physicsDrag, 0) + 1);

            // Clamps Particle.Velocity to a maximum value if Module.Clamp Velocity is true.
            if (SpeedLimit.HasValue)
            {
                velocity = velocity.Direction() * FXEngine.Clamp((float)velocity.Length(), 0, (float)SpeedLimit);
            }

            // Limit Particle Acceleration
            if (AccelerationLimit.HasValue)
            {
                var delta = velocity - particle.Velocity;
                velocity = particle.Velocity + delta.Direction() * FXEngine.Clamp((float)delta.Length(), 0, (float)AccelerationLimit);
            }

            // Apply velocity to derive the new particle position
            particle.Position += velocity * FXEngine.DeltaTime;
            particle.Velocity = velocity;

            // Reset Transient
            particle.PhysicsForce = new Vector3();
            particle.PhysicsDrag = 0;
        }
    }
}
