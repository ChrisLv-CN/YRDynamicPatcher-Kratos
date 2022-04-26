using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 108)]
    public struct ShipLocomotionClass
    {
        [FieldOffset(28)] public double Ramp1;

        [FieldOffset(32)] public double Ramp2;

        [FieldOffset(52)] public CoordStruct Destination; // 0x34 ShipLocomotionClass

        [FieldOffset(64)] public CoordStruct HeadToCoord; // 0x40 ShipLocomotionClass

        [FieldOffset(98)] public Bool IsRotating; // 0x62 ShipLocomotionClass

        [FieldOffset(99)] public Bool IsDriving; // 0x63 ShipLocomotionClass

        [FieldOffset(101)] public Bool IsLocked; // 0x65 ShipLocomotionClass
    }
}
