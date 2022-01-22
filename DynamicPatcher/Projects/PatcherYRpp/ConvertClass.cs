using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 392)]
    [Serializable]
    public struct ConvertClass
    {
       
        public static readonly IntPtr ArrayPointer = new IntPtr(0x89ECF8);
        public static ref DynamicVectorClass<Pointer<ConvertClass>> Array { get => ref DynamicVectorClass<Pointer<ConvertClass>>.GetDynamicVector(ArrayPointer); }


    }
}
