using Extension.FX.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX
{
    public class FXSystem : ICloneable
    {
        public FXSystem()
        {
            MSystemSpawn = new FXModule(this, null);
            MSystemUpdate = new FXModule(this, null);
            Emitters = new List<FXEmitter>();
            Map = new FXParameterMap();
        }

        /// <summary>
        /// Clone Constructor.
        /// </summary>
        protected FXSystem(FXModule mSystemSpawn, FXModule mSystemUpdate, List<FXEmitter> emitters, FXParameterMap map)
        {
            MSystemSpawn = mSystemSpawn.Clone(this, null);
            MSystemUpdate = mSystemUpdate.Clone(this, null);
            Emitters = (from e in emitters select e.Clone(this)).ToList();
            Map = map.Clone();
        }

        // Variables

        public List<FXEmitter> Emitters { get; }
        public FXParameterMap Map { get; }

        // Modules

        public FXModule MSystemSpawn { get; }
        public FXModule MSystemUpdate { get; }

        // Parameters

        public float Age { get; set; }
        public float CurrentLoopDelay { get; set; }
        public float CurrentLoopDuration { get; set; }
        public int LoopCount { get; set; }
        public float LoopedAge { get; set; }
        public float NormalizedLoopedAge { get; set; }
        public FXExecutionState ExecutionState { get; set; }


        public Vector3 Position { get; set; }

        public virtual FXSystem Clone()
        {
            FXSystem system = new FXSystem(
                MSystemSpawn,
                MSystemUpdate,
                Emitters,
                Map
                );

            system.Age = Age;
            system.CurrentLoopDelay = CurrentLoopDelay;
            system.CurrentLoopDuration = CurrentLoopDuration;
            system.LoopCount = LoopCount;
            system.LoopedAge = LoopedAge;
            system.NormalizedLoopedAge = NormalizedLoopedAge;
            system.ExecutionState = ExecutionState;

            system.Position = Position;

            return system;
        }

        public virtual void Spawn(Vector3 position)
        {
            Position = position;

            foreach(var script in MSystemSpawn.Scripts)
            {
                script.SystemSpawn(position);
            }

            SpawnEmitter();
        }
        private void SpawnEmitter()
        {
            if (FXEngine.EnableParallelSpawn)
            {
                Parallel.ForEach(Emitters, emitter => emitter.Spawn(Position));
            }
            else
            {
                foreach (var emitter in Emitters)
                {
                    emitter.Spawn(Position);
                }
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

            foreach (var script in MSystemUpdate.Scripts)
            {
                script.SystemUpdate();
            }

            UpdateEmitter();

            if (ExecutionState == FXExecutionState.Inactive)
            {
                if (Emitters.All(e => e.ExecutionState == FXExecutionState.InactiveClear))
                {
                    ExecutionState = FXExecutionState.InactiveClear;
                    Emitters.ForEach(e => e.ExecutionState = FXExecutionState.Complete);
                }
            }
        }
        private void UpdateEmitter()
        {
            if (FXEngine.EnableParallelUpdate)
            {
                Parallel.ForEach(Emitters, emitter => emitter.Update());
            }
            else
            {
                foreach (var emitter in Emitters)
                {
                    emitter.Update();
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

            if (FXEngine.EnableParallelRender)
            {
                Parallel.ForEach(Emitters, emitter => emitter.Render());
            }
            else
            {
                foreach (var emitter in Emitters)
                {
                    emitter.Render();
                }
            }
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
