using DynamicPatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 36)]
    public struct AbstractClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0xA8E360);
        public static ref DynamicVectorClass<Pointer<ObjectClass>> Array { get => ref DynamicVectorClass<Pointer<ObjectClass>>.GetDynamicVector(ArrayPointer); }

        public unsafe void Init()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, void>)this.GetVirtualFunctionPointer(9);
            func(ref this);
        }

        public unsafe void PointerExpired(Pointer<AbstractClass> pAbstract, bool removed)
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, IntPtr, Bool, void>)this.GetVirtualFunctionPointer(10);
            func(ref this, pAbstract, removed);
        }

        public unsafe AbstractType WhatAmI()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, AbstractType>)this.GetVirtualFunctionPointer(11);
            return func(ref this);
        }

        public unsafe int Size()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, int>)this.GetVirtualFunctionPointer(12);
            return func(ref this);
        }

        public unsafe void CalculateChecksum(Pointer<Checksummer> checksum)
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, IntPtr, void>)this.GetVirtualFunctionPointer(13);
            func(ref this, checksum);
        }

        public unsafe int GetOwningHouseIndex()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, int>)this.GetVirtualFunctionPointer(14);
            return func(ref this);
        }

        public unsafe Pointer<HouseClass> GetOwningHouse()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, IntPtr>)this.GetVirtualFunctionPointer(15);
            return func(ref this);
        }

        public unsafe int GetArrayIndex()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, int>)this.GetVirtualFunctionPointer(16);
            return func(ref this);
        }

        public unsafe bool IsDead()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, Bool>)this.GetVirtualFunctionPointer(17);
            return func(ref this);
        }

        public unsafe CoordStruct GetCoords()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, IntPtr, IntPtr>)this.GetVirtualFunctionPointer(18);

            CoordStruct ret = default;
            func(ref this, Pointer<CoordStruct>.AsPointer(ref ret));
            return ret;
        }

        public unsafe CoordStruct GetDestination(Pointer<TechnoClass> pDocker = default)
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, IntPtr, IntPtr, IntPtr>)this.GetVirtualFunctionPointer(19);

            CoordStruct ret = default;
            func(ref this, Pointer<CoordStruct>.AsPointer(ref ret), pDocker);
            return ret;
        }

        public unsafe bool IsOnFloor()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, Bool>)this.GetVirtualFunctionPointer(20);
            return func(ref this);
        }

        public unsafe bool IsInAir()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, Bool>)this.GetVirtualFunctionPointer(21);
            return func(ref this);
        }

        public unsafe void Update()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, void>)this.GetVirtualFunctionPointer(23);
            func(ref this);
        }

        public unsafe void AnnounceExpiredPointer(bool removed = true)
        {
            var func = (delegate* unmanaged[Thiscall]<int, ref AbstractClass, Bool, void>)ASM.FastCallTransferStation;
            func(0x7258D0, ref this, removed);
        }

        [FieldOffset(0)] public IntPtr Vfptr;

        [FieldOffset(16)] public int UniqueID;

        [FieldOffset(20)] public AbstractFlags AbstractFlags;

        [FieldOffset(24)] public int unknown_18;

        [FieldOffset(28)] public int RefCount;

        [FieldOffset(32)] public Bool Dirty;

        [FieldOffset(33)] public int padding_21;
    }

}
