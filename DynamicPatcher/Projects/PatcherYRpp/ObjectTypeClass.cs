using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PatcherYRpp.FileFormats;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 660)]
    public struct ObjectTypeClass
    {
        public unsafe void Dimension2(Pointer<CoordStruct> pDest)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectTypeClass, IntPtr, void>)
                this.GetVirtualFunctionPointer(31);
            func(ref this, pDest);
        }

        public unsafe CoordStruct Dimension2()
        {
            CoordStruct ret = default;
            Dimension2(Pointer<CoordStruct>.AsPointer(ref ret));
            return ret;
        }

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

        [FieldOffset(164)] public Pointer<SHPStruct> Image;

        [FieldOffset(168)] public Bool ImageAllocated;

        [FieldOffset(172)] public Pointer<SHPStruct> AlphaImage;

        [FieldOffset(176)] public VoxelStruct MainVoxel;

        [FieldOffset(184)] public VoxelStruct TurretVoxel;

        [FieldOffset(192)] public VoxelStruct BarrelVoxel;

        [FieldOffset(200)] public VoxelStruct chargerTurrets_first;
        public Pointer<VoxelStruct> ChargeTurrets => Pointer<VoxelStruct>.AsPointer(ref chargerTurrets_first);

        [FieldOffset(344)] public VoxelStruct chargerBarrels_first;
        public Pointer<VoxelStruct> ChargerBarrels => Pointer<VoxelStruct>.AsPointer(ref chargerBarrels_first);

        [FieldOffset(488)] public Bool NoSpawnAlt;

        [FieldOffset(560)] public Bool Selectable;

        [FieldOffset(562)] public Bool Insignificant;

        [FieldOffset(563)] public Bool Immune;

    }
}
