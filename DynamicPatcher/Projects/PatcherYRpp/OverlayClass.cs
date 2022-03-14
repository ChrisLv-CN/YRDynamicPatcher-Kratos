using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 176)]
    public struct OverlayClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0xA8EC50);
        public static ref DynamicVectorClass<Pointer<OverlayClass>> Array { get => ref DynamicVectorClass<Pointer<OverlayClass>>.GetDynamicVector(ArrayPointer); }

        [FieldOffset(0)] public ObjectClass Base;

        [FieldOffset(172)] public Pointer<OverlayTypeClass> Type;
    }
}
