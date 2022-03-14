using Extension.FX.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX
{
    public abstract class FXScript : ICloneable
    {
        protected FXScript(FXSystem system, FXEmitter emitter)
        {
            System = system;
            Emitter = emitter;
        }

        // Variables

        public FXSystem System { get; }
        public FXEmitter Emitter { get; }

        public abstract FXScript Clone(FXSystem system = null, FXEmitter emitter = null);

        public virtual void SystemSpawn(Vector3 position)
        {
            throw new NotImplementedException($"{this} has not implement SystemSpawn!");
        }

        public virtual void SystemUpdate()
        {
            throw new NotImplementedException($"{this} has not implement SystemUpdate!");
        }
        public virtual void EmitterSpawn(Vector3 position)
        {
            throw new NotImplementedException($"{this} has not implement EmitterSpawn!");
        }

        public virtual void EmitterUpdate()
        {
            throw new NotImplementedException($"{this} has not implement EmitterUpdate!");
        }
        public virtual void ParticleSpawn(FXParticle particle)
        {
            throw new NotImplementedException($"{this} has not implement ParticleSpawn!");
        }
        public virtual void ParticleUpdate(FXParticle particle)
        {
            throw new NotImplementedException($"{this} has not implement ParticleUpdate!");
        }

        public override string ToString()
        {
            return GetType().FullName;
        }

        object ICloneable.Clone()
        {
            return Clone(System, Emitter);
        }
    }
}
