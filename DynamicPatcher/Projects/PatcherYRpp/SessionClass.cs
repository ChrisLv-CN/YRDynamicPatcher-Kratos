using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 12508)]
    public struct SessionClass
    {

        private static IntPtr instance = new IntPtr(0xA8B238);
        public static ref SessionClass Instance { get => ref instance.Convert<SessionClass>().Ref; }

        [FieldOffset(0)] public GameMode GameMode;

    }
}
