using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 92)]
    public struct RocketLocomotionClass
    {
        [FieldOffset(24)] public CoordStruct Destination;

        [FieldOffset(36)] public TimerStruct Timer24;

        [FieldOffset(52)] public TimerStruct Timer34;

        [FieldOffset(81)] public Bool UseElite;
    }
}
