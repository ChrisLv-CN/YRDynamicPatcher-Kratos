using Extension.FX.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Scripts.Particle
{
    public class FXAddVelocityFromPoint : FXScript
    {
        public FXAddVelocityFromPoint(FXSystem system, FXEmitter emitter) : base(system, emitter)
        {
        }

        // Input Parameters
        public FXParameter<float> VelocityStrength { get; set; } = new FXParameter<float>(nameof(VelocityStrength), 100);
        public Vector3 Offset { get; set; }
        public bool Invert { get; set; } = true;

        public override FXScript Clone(FXSystem system = null, FXEmitter emitter = null)
        {
            var addVelocityFromPoint = new FXAddVelocityFromPoint(system ?? System, emitter ?? Emitter);

            addVelocityFromPoint.VelocityStrength = VelocityStrength.Clone(addVelocityFromPoint);
            addVelocityFromPoint.Offset = Offset;
            addVelocityFromPoint.Invert = Invert;

            return addVelocityFromPoint;
        }

        public override void ParticleSpawn(FXParticle particle)
        {
            var start = particle.Position;
            var end = Emitter.WorldPosition + Offset;

            float speed = VelocityStrength * (Invert ? -1 : 1);

            if(start == end)
            {
                particle.Velocity += FXEngine.CalculateRandomUnitVector() * speed;
            }
            else
            {
                particle.Velocity += (end - start).Direction() * speed;
            }
        }
    }
}
