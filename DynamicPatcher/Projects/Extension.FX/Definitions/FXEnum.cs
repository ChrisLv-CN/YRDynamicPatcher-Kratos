using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Definitions
{
    public enum FXExecutionState
    {
        /// <summary> The System or Emitter simulates and allows spawning. </summary>
        Active,
        /// <summary> The System or Emitter simulates but does not allow any new spawning. </summary>
        Inactive,
        /// <summary> The System or Emitter destroys all Particles it owns, and then moves to the Inactive Execution State. </summary>
        InactiveClear,
        /// <summary> The System or Emitter does not simulate and does not render. </summary>
        Complete
    }

    public enum FXLifetimeMode
    {
        DirectSet,
        Random
    }
    public enum FXMassInitializationMode
    {
        DirectSet,
        Random
    }

    public enum FXCoordinateSpace
    {
        World,
        Local
    }
}
