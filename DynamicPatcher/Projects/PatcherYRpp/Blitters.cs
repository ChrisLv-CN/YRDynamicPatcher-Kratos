using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BlitterCore
    {
        public int Vfptr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RLEBlitterCore
    {
        public int Vfptr;
        Pointer<byte> Data;
    }
}
