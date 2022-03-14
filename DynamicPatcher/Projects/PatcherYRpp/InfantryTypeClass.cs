using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 3792)]
    public struct InfantryTypeClass
    {
        
        public static readonly IntPtr ArrayPointer = new IntPtr(0xA8E348);

        public static YRPP.GLOBAL_DVC_ARRAY<InfantryTypeClass> ABSTRACTTYPE_ARRAY = new YRPP.GLOBAL_DVC_ARRAY<InfantryTypeClass>(ArrayPointer);

        [FieldOffset(0)] public TechnoTypeClass Base;
        [FieldOffset(0)] public ObjectTypeClass BaseObjectType;
        [FieldOffset(0)] public AbstractTypeClass BaseAbstractType;

        [FieldOffset(3773)] public Bool Crawls;

    }
}
