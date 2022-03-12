using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 52)]
    public struct MechLocomotionClass
    {

        [FieldOffset(24)] public CoordStruct Destination; // 0x18 MechLocomotionClass

        [FieldOffset(36)] public CoordStruct HeadToCoord; // 0x24 MechLocomotionClass

        [FieldOffset(48)] public Bool IsMoving; // 0x30 MechLocomotionClass

    }
}
