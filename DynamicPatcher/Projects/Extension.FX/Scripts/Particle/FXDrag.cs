using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Scripts.Particle
{
    class FXDrag : FXScript
    {
        public FXDrag(FXSystem system, FXEmitter emitter) : base(system, emitter)
        {
        }

        // Input Parameters

        public float Drag { get; set; } = 1;
        public float RotationalDrag { get; set; } = 1;

        public override FXScript Clone(FXSystem system = null, FXEmitter emitter = null)
        {
            var drag = new FXDrag(system ?? System, emitter ?? Emitter);

            drag.Drag = Drag;
            drag.RotationalDrag = RotationalDrag;

            return drag;
        }

        public override void ParticleUpdate(FXParticle particle)
        {
            particle.PhysicsDrag += Drag;
            particle.PhysicsRotationalDrag += RotationalDrag;
        }
    }
}
