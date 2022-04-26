using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp.FileFormats
{
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct MotLib
    {
        [FieldOffset(0)]public bool LoadedFailed;
        [FieldOffset(4)]public int LayerCount;
        [FieldOffset(8)]public int FrameCount;
        [FieldOffset(12)]public Pointer<Matrix3DStruct> Matrixes;
    }
}
