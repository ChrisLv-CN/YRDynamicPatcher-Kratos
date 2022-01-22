using Extension.FX.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Scripts.Emitter
{
    [FXPreDependency(nameof(FXEmitterState))]
    public class FXSpawnBurstInstantaneous : FXSpawn
    {
        public FXSpawnBurstInstantaneous(FXSystem system, FXEmitter emitter) : base(system, emitter)
        {
            var ageLink = new FXLinkParameter<float>(nameof(Age), "Emitter.LoopedAge");
            ageLink.SetContext(system, emitter, this);
            Age = ageLink;
        }

        public int SpawnCount { get; set; } = 1;
        public float SpawnTime { get; set; } = 0;
        public float SpawnProbability { get; set; } = 1;
        public FXParameter<float> Age { get; set; }
        public int SpawnGroup { get; set; } = 0;


        public override FXScript Clone(FXSystem system = null, FXEmitter emitter = null)
        {
            var spawnBurstInstantaneous = new FXSpawnBurstInstantaneous(system ?? System, emitter ?? Emitter);

            spawnBurstInstantaneous.SpawnInfo = SpawnInfo;

            spawnBurstInstantaneous.SpawnCount = SpawnCount;
            spawnBurstInstantaneous.SpawnTime = SpawnTime;
            spawnBurstInstantaneous.SpawnProbability = SpawnProbability;
            spawnBurstInstantaneous.Age = Age.Clone(spawnBurstInstantaneous);
            spawnBurstInstantaneous.SpawnGroup = SpawnGroup;

            return spawnBurstInstantaneous;
        }

        public override void EmitterUpdate()
        {
            float age = Age; // default link to Emitter.LoopedAge

            SpawnInfo.InterpStartDt = SpawnTime + FXEngine.DeltaTime - age;

            bool shouldSpawn = SpawnInfo.InterpStartDt >= 0 && SpawnTime - age < 0;
            if (SpawnProbability < 1)
            {
                shouldSpawn &= SpawnProbability > FXEngine.CalculateRandomRange();
            }

            SpawnInfo.Count = shouldSpawn ? SpawnCount : 0;
            SpawnInfo.IntervalDt = 0;
            SpawnInfo.SpawnGroup = SpawnGroup;

            base.EmitterUpdate();
        }
    }
}
