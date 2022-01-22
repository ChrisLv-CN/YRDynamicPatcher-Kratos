using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 112)]
    [Serializable]
    public struct DriveLocomotionClass
    {

        [FieldOffset(52)] public CoordStruct DestinationCoord; // 0x34 DriveLocomotionClass

        [FieldOffset(64)] public CoordStruct HeadToCoord; // 0x40 DriveLocomotionClass

        [FieldOffset(98)] public Bool IsRotating; // 0x62 DriveLocomotionClass

        [FieldOffset(99)] public Bool IsDriving; // 0x63 DriveLocomotionClass

        [FieldOffset(101)] public Bool IsLocked; // 0x65 DriveLocomotionClass
    }
}
