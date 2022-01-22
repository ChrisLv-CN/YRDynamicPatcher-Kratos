using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Scripts.Emitter
{
    [FXPreDependency(nameof(FXEmitterState))]
    public class FXSpawnRate : FXSpawn
    {
        public FXSpawnRate(FXSystem system, FXEmitter emitter) : base(system, emitter)
        {
        }

        public float SpawnRate { get; set; } = 0;
        public float SpawnProbability { get; set; } = 1;
        public int SpawnGroup { get; set; } = 0;

        public override FXScript Clone(FXSystem system = null, FXEmitter emitter = null)
        {
            var spawnRate = new FXSpawnRate(system ?? System, emitter ?? Emitter);

            spawnRate.SpawnInfo = SpawnInfo;

            spawnRate.SpawnRate = SpawnRate;
            spawnRate.SpawnProbability = SpawnProbability;
            spawnRate.SpawnGroup = SpawnGroup;

            return spawnRate;
        }

        public override void EmitterUpdate()
        {
            var spawnRate = SpawnRate;
            var intervalDT = 1 / spawnRate;
            var interpStartDT = intervalDT * (1 - SpawnRemainder);

            var spawnValue = spawnRate * (Emitter.LoopedAge >= 0 ? 1 : 0) * FXEngine.DeltaTime + SpawnRemainder;
            var spawnCount = (int)spawnValue;
            SpawnRemainder = spawnValue - spawnCount;

            SpawnInfo.Count = SpawnProbability >= 1 || SpawnProbability > FXEngine.CalculateRandomRange() ? spawnCount : 0;
            SpawnInfo.InterpStartDt = interpStartDT;
            SpawnInfo.IntervalDt = intervalDT;
            SpawnInfo.SpawnGroup = SpawnGroup;

            CanEverSpawn = true;

            base.EmitterUpdate();
        }

        // Unique Attribute

        public float SpawnRemainder { get; set; }
    }
}
