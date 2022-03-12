using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 60)]
    public struct WalkLocomotionClass
    {

        [FieldOffset(24)] public Bool Piggy;

        [FieldOffset(28)] public CoordStruct Destination;

        [FieldOffset(40)] public CoordStruct HeadToCoord;

        [FieldOffset(52)] public Bool IsMoving;

        [FieldOffset(54)] public Bool IsReallyMoving;

        [FieldOffset(56)] public Bool Piggyback;
    }
}
