using Extension.FX.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX
{
    public class FXEmitter : ICloneable
    {
        public FXEmitter(FXSystem system) : this(system, new FXParticle())
        {
        }

        public FXEmitter(FXSystem system, FXParticle prototype)
        {
            System = system;
            MEmitterSpawn = new FXModule(system, this);
            MEmitterUpdate = new FXModule(system, this);
            MParticleSpawn = new FXModule(system, this);
            MParticleUpdate = new FXModule(system, this);
            MRender = new FXModule(system, this);

            Prototype = prototype.Clone();

            Particles = new List<FXParticle>();
            Map = new FXParameterMap();
        }

        /// <summary>
        /// Clone Constructor.
        /// </summary>
        protected FXEmitter(FXSystem system, FXModule mEmitterSpawn, FXModule mEmitterUpdate, FXModule mParticleSpawn, FXModule mParticleUpdate, FXModule mRender, FXParticle prototype, List<FXParticle> particles, FXParameterMap map)
        {
            System = system;
            MEmitterSpawn = mEmitterSpawn.Clone(system, this);
            MEmitterUpdate = mEmitterUpdate.Clone(system, this);
            MParticleSpawn = mParticleSpawn.Clone(system, this);
            MParticleUpdate = mParticleUpdate.Clone(system, this);
            MRender = mRender.Clone(system, this);

            Prototype = prototype.Clone();

            Particles = (from p in particles select p.Clone()).ToList();
            Map = map.Clone();
        }

        // Variables

        public FXSystem System { get; }

        public List<FXParticle> Particles { get; private set; }

        public FXParticle Prototype { get; set; }
        public FXParameterMap Map { get; }

        // Modules

        public FXModule MEmitterSpawn { get; }
        public FXModule MEmitterUpdate { get; }
        public FXModule MParticleSpawn { get; }
        public FXModule MParticleUpdate { get; }
        public FXModule MRender { get; }

        // Parameters

        public float Age { get; set; }
        public float CurrentLoopDelay { get; set; }
        public float CurrentLoopDuration { get; set; }
        public int LoopCount { get; set; }
        public float LoopedAge { get; set; }
        public float NormalizedLoopedAge { get; set; }
        public FXExecutionState ExecutionState { get; set; }

        public Vector3 LocalPosition { get; set; }
        public FXCoordinateSpace CoordinateSpace { get; set; }
        public Vector3 WorldPosition
        {
            get
            {
                return CoordinateSpace == FXCoordinateSpace.World ? System.Position + LocalPosition : LocalPosition;
            }
            set
            {
                LocalPosition = CoordinateSpace == FXCoordinateSpace.World ? value - System.Position : value;
            }
        }

        public virtual FXEmitter Clone(FXSystem system = null)
        {
            FXEmitter emitter = null;
            emitter = new FXEmitter(
                system ?? System,
                MEmitterSpawn,
                MEmitterUpdate,
                MParticleSpawn,
                MParticleUpdate,
                MRender,
                Prototype,
                Particles,
                Map
                );

            emitter.Age = Age;
            emitter.CurrentLoopDelay = CurrentLoopDelay;
            emitter.CurrentLoopDuration = CurrentLoopDuration;
            emitter.LoopCount = LoopCount;
            emitter.LoopedAge = LoopedAge;
            emitter.NormalizedLoopedAge = NormalizedLoopedAge;
            emitter.ExecutionState = ExecutionState;

            emitter.LocalPosition = LocalPosition;
            emitter.CoordinateSpace = CoordinateSpace;

            return emitter;
        }

        public virtual void Spawn(Vector3 position)
        {
            WorldPosition = position;

            foreach (var script in MEmitterSpawn.Scripts)
            {
                script.EmitterSpawn(position);
            }
        }

        public virtual void Update()
        {
            switch (ExecutionState)
            {
                case FXExecutionState.InactiveClear:
                case FXExecutionState.Complete:
                    return;
            }

            foreach (var script in MEmitterUpdate.Scripts)
            {
                script.EmitterUpdate();
            }

            UpdateParticles();
            ClearParticles();

            if (ExecutionState == FXExecutionState.Inactive)
            {
                if(Particles.Count == 0)
                {
                    ExecutionState = FXExecutionState.InactiveClear;
                }
            }
        }
        private void UpdateParticles()
        {
            foreach (var script in MParticleUpdate.Scripts)
            {
                if (FXEngine.EnableParallelUpdate)
                {
                    Parallel.ForEach(Particles, particle => script.ParticleUpdate(particle));
                }
                else
                {
                    foreach (var particle in Particles)
                    {
                        script.ParticleUpdate(particle);
                    }
                }
            }
        }

        public virtual void Render()
        {
            switch (ExecutionState)
            {
                case FXExecutionState.InactiveClear:
                case FXExecutionState.Complete:
                    return;
            }

            foreach (var script in MRender.Scripts)
            {
                var renderer = script as FXRenderer;
                if (FXEngine.EnableParallelRender)
                {
                    Parallel.ForEach(Particles, particle => renderer.ParticleRender(particle));
                }
                else
                {
                    foreach (var particle in Particles)
                    {
                        renderer.ParticleRender(particle);
                    }
                }
            }
        }

        public FXParticle SpawnParticle()
        {
            FXParticle particle = Prototype.Clone();
            Particles.Add(particle);

            foreach (var script in MParticleSpawn.Scripts)
            {
                script.ParticleSpawn(particle);
            }

            return particle;
        }

        public void ClearParticles()
        {
            var inactiveParticles = from p in Particles where !p.Alive select p;
            if (inactiveParticles.Any())
            {
                Particles = Particles.Except(inactiveParticles).ToList();

                foreach (var p in inactiveParticles)
                {
                    if (p.Map.TryGetValue("DrawObject", out var parameter))
                    {
                        var drawObject = parameter.Value as Graphic.FXDrawObject;
                        drawObject.Dispose();
                    }
                }
            }
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
