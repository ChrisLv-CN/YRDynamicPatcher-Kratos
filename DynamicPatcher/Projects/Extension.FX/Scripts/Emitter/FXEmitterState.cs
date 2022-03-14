using Extension.FX.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Scripts.Emitter
{
    public class FXEmitterState : FXScript
    {
        public FXEmitterState(FXSystem system, FXEmitter emitter) : base(system, emitter)
        {
        }

        // Input Parameters

        public bool DelayFirstLoopOnly { get; set; } = false;
        public int LoopCount { get; set; } = 1;
        public float LoopDelay { get; set; } = 0;
        public float LoopDuration { get; set; } = 5.0f;
        public bool RecalculateDurationEachLoop { get; set; } = false;

        public override FXScript Clone(FXSystem system = null, FXEmitter emitter = null)
        {
            var state = new FXEmitterState(system ?? System, emitter ?? Emitter);
            state.DelayFirstLoopOnly = DelayFirstLoopOnly;
            state.LoopCount = LoopCount;
            state.LoopDelay = LoopDelay;
            state.LoopDuration = LoopDuration;
            state.RecalculateDurationEachLoop = RecalculateDurationEachLoop;
            return state;
        }

        public override void EmitterUpdate()
        {
            bool loopCountIncreased;
            // DELAY: Copy first round of loop duration and delay into LoopedAge, CurrentLoopDuration, and CurrentLoopDelay
            if (Emitter.Age == 0)
            {
                Emitter.LoopedAge = -LoopDelay;
                Emitter.CurrentLoopDuration = Math.Max(LoopDuration, FXEngine.DeltaTime);
                Emitter.CurrentLoopDelay = LoopDelay;
            }

            if (LoopCount > 1)
            {
                // If LoopedAge > LoopDuration then increment loop count and store the remainder in LoopedAge. 
                // The Emitter is still delayed if LoopedAge < 0.0.
                Emitter.Age += FXEngine.DeltaTime;

                var nextLoopedAge = Emitter.LoopedAge + FXEngine.DeltaTime;
                loopCountIncreased = Math.Max((int)(nextLoopedAge / Emitter.CurrentLoopDuration), 0) > 0;

                if (loopCountIncreased)
                {
                    Emitter.LoopedAge = 0;
                }
                else
                {
                    Emitter.LoopedAge = nextLoopedAge;
                }
            }
            else
            {
                // Loop Once behavior, feed the looped age variable for stack behavior consistency
                Emitter.Age += FXEngine.DeltaTime;
                Emitter.LoopedAge += FXEngine.DeltaTime;
                loopCountIncreased = Emitter.LoopedAge >= Emitter.CurrentLoopDuration;
            }


            if (loopCountIncreased)
            {
                Emitter.LoopCount++;
                if (LoopCount > 1)
                {
                    // DELAY: If the loop count really did go up, we need to factor in delays, decide on the new loop variables
                    if (RecalculateDurationEachLoop)
                    {
                        Emitter.CurrentLoopDuration = LoopDuration;
                    }
                    Emitter.CurrentLoopDelay = DelayFirstLoopOnly ? 0 : LoopDelay;
                    Emitter.LoopedAge -= Emitter.CurrentLoopDelay;
                }
                else
                {
                    // LOOP ONCE Age variables
                    Emitter.CurrentLoopDuration = LoopDuration;
                    Emitter.LoopedAge = 0;

                }
            }

            Emitter.NormalizedLoopedAge = Emitter.LoopedAge / Emitter.CurrentLoopDuration;

            if (Emitter.LoopCount >= LoopCount)
            {
                Emitter.ExecutionState = FXExecutionState.Inactive;
            }
        }
    }
}
