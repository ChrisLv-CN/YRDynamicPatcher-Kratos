using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 120)]
    public struct HoverLocomotionClass
    {

        [FieldOffset(24)] public CoordStruct Destination; // 0x18 HoverLocomotionClass

        [FieldOffset(36)] public CoordStruct HeadToCoord; // 0x24 HoverLocomotionClass

        [FieldOffset(48)] public FacingStruct Facing; // 0x30 HoverLocomotionClass

    }
}
