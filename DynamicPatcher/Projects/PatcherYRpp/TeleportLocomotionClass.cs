using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 76)]
    public struct TeleportLocomotionClass
    {

        [FieldOffset(28)] public CoordStruct Destination; // 0x1C

        [FieldOffset(40)] public CoordStruct HeadToCoord; // 0x28

        [FieldOffset(52)] public TimerStruct Timer; // 0x3C

    }
}
