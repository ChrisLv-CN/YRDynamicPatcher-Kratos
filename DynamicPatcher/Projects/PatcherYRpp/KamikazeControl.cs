using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct KamikazeControl
    {
        [FieldOffset(0)] public Pointer<AircraftClass> Aircraft;
        [FieldOffset(4)] public Pointer<AbstractClass> Cell;
    }
}
