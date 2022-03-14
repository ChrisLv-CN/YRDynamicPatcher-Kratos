using Extension.FX.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX
{
    public class FXParticle : ICloneable
    {
        public FXParticle()
        {
            Map = new FXParameterMap();
        }

        /// <summary>
        /// Clone Constructor.
        /// </summary>
        protected FXParticle(FXParameterMap map)
        {
            Map = map.Clone();
        }

        // Variables

        public FXParameterMap Map { get; }

        // Parameters

        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }


        public float Age { get; set; }
        public float Lifetime { get; set; }
        public float NormalizedAge { get; set; }
        public float Mass { get; set; }
        public int SpawnGroup { get; set; }

        // Data 

        public bool Alive { get; set; } = true;

        // Transient

        public Vector3 PhysicsForce;
        public float PhysicsDrag;
        public float PhysicsRotationalDrag;

        public virtual FXParticle Clone()
        {
            FXParticle particle = new FXParticle(Map);

            particle.Position = Position;
            particle.Velocity = Velocity;

            particle.Age = Age;
            particle.Lifetime = Lifetime;
            particle.NormalizedAge = NormalizedAge;
            particle.Mass = Mass;
            particle.SpawnGroup = SpawnGroup;

            particle.Alive = Alive;

            return particle;
        }

        public override string ToString()
        {
            return GetType().FullName;
        }
        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
