using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX
{
    public class FXModule : ICloneable
    {
        public FXModule(FXSystem system, FXEmitter emitter)
        {
            System = system;
            Emitter = emitter;
            Scripts = new List<FXScript>();
        }

        protected FXModule(FXSystem system, FXEmitter emitter, List<FXScript> scripts)
        {
            System = system;
            Emitter = emitter;
            Scripts = scripts;
        }

        // Variables

        public FXSystem System { get; }
        public FXEmitter Emitter { get; }

        public List<FXScript> Scripts { get; }

        public virtual FXModule Clone(FXSystem system = null, FXEmitter emitter = null)
        {
            return new FXModule(
                system ?? System,
                emitter ?? Emitter,
                (from s in Scripts select s.Clone(system, emitter)).ToList()
                );
        }

        public void Add(params FXScript[] scripts)
        {
            Scripts.AddRange(scripts);
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
