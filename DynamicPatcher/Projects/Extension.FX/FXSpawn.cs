using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX
{
    public struct FXSpawnInfo
    {
        public int Count;
        public float InterpStartDt;
        public float IntervalDt;
        public int SpawnGroup;
    }
    public abstract class FXSpawn : FXScript
    {

        public FXSpawn(FXSystem system, FXEmitter emitter, FXSpawnInfo spawnInfo = new FXSpawnInfo()) : base(system, emitter)
        {
            SpawnInfo = spawnInfo;
        }

        // Parameters

        public FXSpawnInfo SpawnInfo;

        public float CurrentSpawnCount { get; set; }
        public float CurrentLoopDelay { get; set; }
        public float LoopedAge { get; set; }

        // Transient

        public bool CanEverSpawn { get; set; }

        public override void EmitterUpdate()
        {
            if (CanEverSpawn)
            {
                SpawnParticles();
            }
            else
            {
                bool loopCountIncreased;

                if (CurrentSpawnCount == 0)
                {
                    LoopedAge = -SpawnInfo.InterpStartDt;
                    CurrentLoopDelay = FXEngine.DeltaTime;
                }

                var nextLoopedAge = LoopedAge + FXEngine.DeltaTime;
                loopCountIncreased = nextLoopedAge > CurrentLoopDelay;

                if (loopCountIncreased)
                {
                    CurrentSpawnCount++;
                    LoopedAge = 0;
                    CurrentLoopDelay = SpawnInfo.IntervalDt;
                }
                else
                {
                    LoopedAge = nextLoopedAge;
                }

                if (loopCountIncreased)
                {
                    SpawnParticles();
                }
            }

            CanEverSpawn = false;
        }

        private void SpawnParticles()
        {
            for (int idx = 0; idx < SpawnInfo.Count; idx++)
            {
                Emitter.SpawnParticle();
                SpawnInfo.Count--;
            }
        }
    }
}
