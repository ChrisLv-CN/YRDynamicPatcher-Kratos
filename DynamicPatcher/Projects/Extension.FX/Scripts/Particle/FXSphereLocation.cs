using Extension.FX.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Scripts.Particle
{
    public class FXSphereLocation : FXScript
    {
        public FXSphereLocation(FXSystem system, FXEmitter emitter) : base(system, emitter)
        {
        }

        // Input Parameters

        public float SphereRadius { get; set; } = 100;
        public Vector3 Offset { get; set; }
        public int SpawnGroupMask { get; set; } = 0;

        public override FXScript Clone(FXSystem system = null, FXEmitter emitter = null)
        {
            var sphereLocation = new FXSphereLocation(system ?? System, emitter ?? Emitter);

            sphereLocation.SphereRadius = SphereRadius;
            sphereLocation.Offset = Offset;
            sphereLocation.SpawnGroupMask = SpawnGroupMask;

            return sphereLocation;
        }

        public override void ParticleSpawn(FXParticle particle)
        {
            if (particle.SpawnGroup == SpawnGroupMask)
            {
                particle.Position += Offset + FXEngine.CalculateRandomPointInSphere(0, SphereRadius);
            }
        }
    }
}
