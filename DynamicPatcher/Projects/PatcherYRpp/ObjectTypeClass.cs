using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 660)]
    public struct ObjectTypeClass
    {
        public unsafe bool SpawnAtMapCoords(CellStruct mapCoords, Pointer<HouseClass> pOwner)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectTypeClass, ref CellStruct, IntPtr, Bool>)
                this.GetVirtualFunctionPointer(32);
            return func(ref this, ref mapCoords, pOwner);
        }

        public unsafe int GetActualCost(Pointer<HouseClass> pOwner)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectTypeClass, IntPtr, int>)
                this.GetVirtualFunctionPointer(33);
            return func(ref this, pOwner);
        }

        public unsafe Pointer<ObjectClass> CreateObject(Pointer<HouseClass> pOwner)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectTypeClass, IntPtr, IntPtr>)
                this.GetVirtualFunctionPointer(35);
            return func(ref this, pOwner);
        }

        [FieldOffset(0)]
        public AbstractTypeClass Base;

        [FieldOffset(156)] public Armor Armor;

        [FieldOffset(160)] public int Strength;

        [FieldOffset(560)] public Bool Selectable;

        [FieldOffset(562)] public Bool Insignificant;

        [FieldOffset(563)] public Bool Immune;

    }
}
