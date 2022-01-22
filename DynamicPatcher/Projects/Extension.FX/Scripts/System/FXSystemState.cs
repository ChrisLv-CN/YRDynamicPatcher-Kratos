using Extension.FX.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Scripts.System
{
    public class FXSystemState : FXScript
    {
        public FXSystemState(FXSystem system, FXEmitter emitter) : base(system, emitter)
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
            var state = new FXSystemState(system ?? System, emitter ?? Emitter);
            state.DelayFirstLoopOnly = DelayFirstLoopOnly;
            state.LoopCount = LoopCount;
            state.LoopDelay = LoopDelay;
            state.LoopDuration = LoopDuration;
            state.RecalculateDurationEachLoop = RecalculateDurationEachLoop;
            return state;
        }

        public override void SystemUpdate()
        {
            bool loopCountIncreased;
            // DELAY: Copy first round of loop duration and delay into LoopedAge, CurrentLoopDuration, and CurrentLoopDelay
            if (System.Age == 0)
            {
                System.LoopedAge = -LoopDelay;
                System.CurrentLoopDuration = Math.Max(LoopDuration, FXEngine.DeltaTime);
                System.CurrentLoopDelay = LoopDelay;
            }

            if (LoopCount > 1)
            {
                // If LoopedAge > LoopDuration then increment loop count and store the remainder in LoopedAge. 
                // The Emitter is still delayed if LoopedAge < 0.0.
                System.Age += FXEngine.DeltaTime;

                var nextLoopedAge = System.LoopedAge + FXEngine.DeltaTime;
                loopCountIncreased = Math.Max((int)(nextLoopedAge / System.CurrentLoopDuration), 0) > 0;

                if (loopCountIncreased)
                {
                    System.LoopCount++;
                    System.LoopedAge = 0;
                }
                else
                {
                    System.LoopedAge = nextLoopedAge;
                }
            }
            else
            {
                // Loop Once behavior, feed the looped age variable for stack behavior consistency
                System.Age += FXEngine.DeltaTime;
                System.LoopedAge += FXEngine.DeltaTime;
                loopCountIncreased = System.LoopedAge >= System.CurrentLoopDuration;
            }


            if (loopCountIncreased)
            {
                if (LoopCount > 1)
                {
                    // DELAY: If the loop count really did go up, we need to factor in delays, decide on the new loop variables
                    if (RecalculateDurationEachLoop)
                    {
                        System.CurrentLoopDuration = LoopDuration;
                    }
                    System.CurrentLoopDelay = DelayFirstLoopOnly ? 0 : LoopDelay;
                    System.LoopedAge -= System.CurrentLoopDelay;
                }
                else
                {
                    // LOOP ONCE Age variables
                    System.CurrentLoopDuration = LoopDuration;
                    System.LoopedAge = 0;

                }
            }

            System.NormalizedLoopedAge = System.LoopedAge / System.CurrentLoopDuration;

            if (System.LoopCount >= LoopCount)
            {
                System.ExecutionState = FXExecutionState.Inactive;
            }
        }
    }
}
