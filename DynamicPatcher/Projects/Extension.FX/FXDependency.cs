using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX
{
    public abstract class FXDependencyAttribute : Attribute
    {
        public FXDependencyAttribute(string typeName)
        {
            this.FXTypeName = typeName;
        }

        public string FXTypeName { get; }
    }
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class FXPreDependencyAttribute : FXDependencyAttribute
    {
        public FXPreDependencyAttribute(string typeName) : base(typeName)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class FXPostDependencyAttribute : FXDependencyAttribute
    {
        public FXPostDependencyAttribute(string typeName) : base(typeName)
        {
        }
    }

    [Serializable]
    public class FXDependencyException : Exception
    {
        public FXDependencyException() { }
        public FXDependencyException(string message) : base(message) { }
        public FXDependencyException(string message, Exception inner) : base(message, inner) { }
        protected FXDependencyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class FXDependencyInspector
    {
        public static void InspectSystem(FXSystem system)
        {
            foreach (var emitter in system.Emitters)
            {
                InspectEmitter(emitter);
            }

            InspectModule(system.MSystemSpawn);
            InspectModule(system.MSystemUpdate);
        }

        public static void InspectEmitter(FXEmitter emitter)
        {
            InspectModule(emitter.MEmitterSpawn);
            InspectModule(emitter.MEmitterUpdate);

            InspectModule(emitter.MParticleSpawn);
            InspectModule(emitter.MParticleUpdate);

            InspectModule(emitter.MRender);
        }

        public static void InspectModule(FXModule module)
        {
            int length = module.Scripts.Count;
            for (int idx = 0; idx < length; idx++)
            {
                FXScript cur = module.Scripts[idx];

                var preScripts = module.Scripts.GetRange(0, idx);
                var postScripts = module.Scripts.GetRange(idx + 1, length - idx - 1);

                var preDependencies = cur.GetType().GetCustomAttributes(typeof(FXPreDependencyAttribute), true) as FXDependencyAttribute[];
                var postDependencies = cur.GetType().GetCustomAttributes(typeof(FXPostDependencyAttribute), true) as FXDependencyAttribute[];

                var unsatisfied = preDependencies.First(d => preScripts.Any(s => s.GetType().Name == d.FXTypeName));

                if (unsatisfied != null)
                {
                    ReportUnsatisfied(module, cur, unsatisfied);
                }

                unsatisfied = postDependencies.First(d => postScripts.Any(s => s.GetType().Name == d.FXTypeName));

                if (unsatisfied != null)
                {
                    ReportUnsatisfied(module, cur, unsatisfied);
                }
            }
        }

        private static void ReportUnsatisfied(FXModule module, FXScript script, FXDependencyAttribute dependency)
        {
            FXSystem system = module.System;
            FXEmitter emitter = module.Emitter;

            string message = "FXDependency unsatisfied! (";

            message += $"{system.GetType().Name}.";

            if (emitter != null)
            {
                message += $"{emitter.GetType().Name}.";
            }

            string which = dependency is FXPreDependencyAttribute ? "Pre-Dependency" : "Post-Dependency";
            message += $"{script.GetType().Name} need {which} {dependency.FXTypeName}";

            message += ")";

            throw new FXDependencyException(message);
        }
    }
}
