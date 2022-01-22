using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp.FileFormats
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct VoxelStruct
    {
        [FieldOffset(0)] public Pointer<VoxLib> VXL;

        [FieldOffset(4)] public Pointer<MotLib> HVA;
    }
}
