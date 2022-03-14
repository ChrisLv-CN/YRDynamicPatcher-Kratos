using Extension.FX.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Scripts.Particle
{
    public class FXBoxLocation : FXScript
    {
        public FXBoxLocation(FXSystem system, FXEmitter emitter) : base(system, emitter)
        {
        }

        // Input Parameters

        public Vector3 BoxSize { get; set; } = new Vector3(100, 100, 100);
        public Vector3 BoxOffset { get; set; }
        public int SpawnGroupMask { get; set; } = 0;

        public override FXScript Clone(FXSystem system = null, FXEmitter emitter = null)
        {
            var boxLocation = new FXBoxLocation(system ?? System, emitter ?? Emitter);

            boxLocation.BoxSize = BoxSize;
            boxLocation.BoxOffset = BoxOffset;
            boxLocation.SpawnGroupMask = SpawnGroupMask;

            return boxLocation;
        }

        public override void ParticleSpawn(FXParticle particle)
        {
            if(particle.SpawnGroup == SpawnGroupMask)
            {
                particle.Position += BoxOffset + FXEngine.CalculateRandomPointInBox(BoxSize);
            }
        }
    }
}
