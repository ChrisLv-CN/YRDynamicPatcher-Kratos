using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 3704)]
    [Serializable]
    public struct UnitTypeClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0xA83CE0);

        public static YRPP.GLOBAL_DVC_ARRAY<UnitTypeClass> ABSTRACTTYPE_ARRAY = new YRPP.GLOBAL_DVC_ARRAY<UnitTypeClass>(ArrayPointer);


        [FieldOffset(0)] public TechnoTypeClass Base;

    }
}
