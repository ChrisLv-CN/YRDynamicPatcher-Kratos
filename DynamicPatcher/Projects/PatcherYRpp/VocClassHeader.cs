using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct VocClassHeader
    {
        [FieldOffset(0)] public IntPtr Next;

        [FieldOffset(4)] public IntPtr Prev;

        [FieldOffset(8)] public int Magic;

    }
}
