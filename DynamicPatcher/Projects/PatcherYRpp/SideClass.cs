using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 180)]
    public struct SideClass
    {
        static public readonly IntPtr ArrayPointer = new IntPtr(0x8B4120);

        static public YRPP.GLOBAL_DVC_ARRAY<SideClass> ABSTRACTTYPE_ARRAY = new YRPP.GLOBAL_DVC_ARRAY<SideClass>(ArrayPointer);



        [FieldOffset(0)] public AbstractTypeClass Base;

        [FieldOffset(152)] public DynamicVectorClass<int> HouseTypes;
    }
}
