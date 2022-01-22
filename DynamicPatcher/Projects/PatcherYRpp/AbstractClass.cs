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
        public unsafe AbstractType WhatAmI()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, AbstractType>)Helpers.GetVirtualFunctionPointer(Pointer<AbstractClass>.AsPointer(ref this), 11);
            return func(ref this);
        }

        public unsafe int GetOwningHouseIndex()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, int>)Helpers.GetVirtualFunctionPointer(Pointer<AbstractClass>.AsPointer(ref this), 14);
            return func(ref this);
        }

        public unsafe Pointer<HouseClass> GetOwningHouse()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, IntPtr>)Helpers.GetVirtualFunctionPointer(Pointer<AbstractClass>.AsPointer(ref this), 15);
            return func(ref this);
        }

        public unsafe int GetArrayIndex()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, int>)Helpers.GetVirtualFunctionPointer(Pointer<AbstractClass>.AsPointer(ref this), 16);
            return func(ref this);
        }

        public unsafe bool IsDead()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, byte>)Helpers.GetVirtualFunctionPointer(Pointer<AbstractClass>.AsPointer(ref this), 17);
            return Convert.ToBoolean(func(ref this));
        }

        public unsafe CoordStruct GetCoords()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, IntPtr, IntPtr>)Helpers.GetVirtualFunctionPointer(Pointer<AbstractClass>.AsPointer(ref this), 18);

            CoordStruct ret = default;
            func(ref this, Pointer<CoordStruct>.AsPointer(ref ret));
            return ret;
        }

        public unsafe bool IsOnFloor()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, byte>)Helpers.GetVirtualFunctionPointer(Pointer<AbstractClass>.AsPointer(ref this), 20);
            return Convert.ToBoolean(func(ref this));
        }

        public unsafe bool IsInAir()
        {
            var func = (delegate* unmanaged[Thiscall]<ref AbstractClass, byte>)Helpers.GetVirtualFunctionPointer(Pointer<AbstractClass>.AsPointer(ref this), 21);
            return Convert.ToBoolean(func(ref this));
        }

        [FieldOffset(0)]
        public int Vfptr;

        [FieldOffset(16)]
        public int UniqueID;

        [FieldOffset(20)]
        public byte AbstractFlags;

        [FieldOffset(24)]
        public int unknown_18;

        [FieldOffset(28)]
        public int RefCount;

        [FieldOffset(32)]
        public byte Dirty;

        [FieldOffset(33)]
        public int padding_21;
    }

}
