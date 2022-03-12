using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 152)]
    [Serializable]
    public struct JumpjetLocomotionClass
    {

        [FieldOffset(28)] public double TurnRate;

        [FieldOffset(32)] public int Speed;

        [FieldOffset(36)] public int Climb;

        [FieldOffset(40)] public int Crash;

        [FieldOffset(44)] public int Height;

        [FieldOffset(48)] public int Accel;

        [FieldOffset(52)] public int Wobbles;

        [FieldOffset(56)] public int Deviation;

        [FieldOffset(60)] public Bool NoWobbles;

        [FieldOffset(64)] public CoordStruct HeadToCoord;

        [FieldOffset(76)] public Bool IsMoving;

        [FieldOffset(84)] public FacingStruct LocomotionFacing;

    }
}
