using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 1776)]
    [Serializable]
    public struct InfantryClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0xA83DE8);
        public static ref DynamicVectorClass<Pointer<InfantryClass>> Array { get => ref DynamicVectorClass<Pointer<InfantryClass>>.GetDynamicVector(ArrayPointer); }

        public unsafe bool IsDeployed()
        {
            var func = (delegate* unmanaged[Thiscall]<ref InfantryClass, Bool>)Helpers.GetVirtualFunctionPointer(Pointer<InfantryClass>.AsPointer(ref this), 341);
            return func(ref this);
        }

        public unsafe bool PlayAnim(int animNumber, bool bUnk, int dwUnk)
        {
            var func = (delegate* unmanaged[Thiscall]<ref InfantryClass, int, Bool, int, Bool>)Helpers.GetVirtualFunctionPointer(Pointer<InfantryClass>.AsPointer(ref this), 342);
            return func(ref this, animNumber, bUnk, dwUnk);
        }

        [FieldOffset(0)] public FootClass Base;

        [FieldOffset(1728)] public Pointer<InfantryTypeClass> Type;

        [FieldOffset(1732)] public SequenceAnimType SequenceAnim;

        [FieldOffset(1748)] public int PanicDurationLeft;

        [FieldOffset(1752)] public Bool PermanentBerzerk;

        [FieldOffset(1753)] public Bool Technician;

        [FieldOffset(1754)] public Bool Stoked;

        [FieldOffset(1755)] public Bool Crawling;

        [FieldOffset(1756)] public Bool ZoneCheat;

    }
}
