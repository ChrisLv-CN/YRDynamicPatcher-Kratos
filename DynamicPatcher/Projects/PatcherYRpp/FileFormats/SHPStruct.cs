using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp.FileFormats
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SHPStruct
    {
        public static unsafe void Destructor(Pointer<SHPReference> pThis)
        {
            var func = (delegate* unmanaged[Thiscall]<ref SHPReference, void>)Helpers.GetVirtualFunctionPointer(pThis, 0);
            func(ref pThis.Ref);
        }
        public unsafe void Load()
        {
            var func = (delegate* unmanaged[Thiscall]<ref SHPStruct, void>)0x69E090;
            func(ref this);
        }
        public unsafe void Unload()
        {
            var func = (delegate* unmanaged[Thiscall]<ref SHPStruct, void>)0x69E100;
            func(ref this);
        }
        public unsafe Pointer<SHPFile> GetData()
        {
            var func = (delegate* unmanaged[Thiscall]<ref SHPStruct, IntPtr>)0x69E580;
            return func(ref this);
        }
        public unsafe RectangleStruct GetFrameBounds(int idxFrame)
        {
            RectangleStruct tmp = default;
            var func = (delegate* unmanaged[Thiscall]<ref SHPStruct, ref RectangleStruct, int, IntPtr>)0x69E7E0;
            func(ref this, ref tmp, idxFrame);
            return tmp;
        }
        public unsafe ColorStruct GetColor(int idxFrame)
        {
            ColorStruct tmp = default;
            var func = (delegate* unmanaged[Thiscall]<ref SHPStruct, ref ColorStruct, int, IntPtr>)0x69E860;
            func(ref this, ref tmp, idxFrame);
            return tmp;
        }
        public unsafe Pointer<byte> GetPixels(int idxFrame)
        {
            var func = (delegate* unmanaged[Thiscall]<ref SHPStruct, int, IntPtr>)0x69E740;
            return func(ref this, idxFrame);
        }
        public unsafe bool HasCompression(int idxFrame)
        {
            var func = (delegate* unmanaged[Thiscall]<ref SHPStruct, int, Bool>)0x69E900;
            return func(ref this, idxFrame);
        }
        public unsafe bool IsReference()
        {
            return Type == 0xFFFF;
        }
        public unsafe Pointer<SHPReference> AsReference()
        {
            return IsReference() ? Pointer<SHPStruct>.AsPointer(ref this).Convert<SHPReference>() : Pointer<SHPReference>.Zero;
        }
        public unsafe Pointer<SHPFile> AsFile()
        {
            return !IsReference() ? Pointer<SHPStruct>.AsPointer(ref this).Convert<SHPFile>() : Pointer<SHPFile>.Zero;
        }


        public ushort Type;
        public short Width;
        public short Height;
        public short Frames;
    }

    [StructLayout(LayoutKind.Explicit, Size = 36)]
    public struct SHPReference
    {
        public static unsafe void Constructor(Pointer<SHPReference> pThis, string fileName)
        {
            var func = (delegate* unmanaged[Thiscall]<ref SHPReference, IntPtr, void>)0x69E430;
            func(ref pThis.Ref, new AnsiString(fileName));
        }
        public static unsafe void Destructor(Pointer<SHPReference> pThis)
        {
            var func = (delegate* unmanaged[Thiscall]<ref SHPReference, void>)Helpers.GetVirtualFunctionPointer(pThis, 0);
            func(ref pThis.Ref);
        }

        [FieldOffset(0)] public SHPStruct Base;
        [FieldOffset(8)] public AnsiStringPointer Filename;
        [FieldOffset(12)] public Pointer<SHPStruct> Data;
        [FieldOffset(16)] public Bool Loaded;
        [FieldOffset(20)] public int INdex;
        [FieldOffset(24)] public IntPtr next;
        public Pointer<SHPReference> Next => next;
        [FieldOffset(28)] public IntPtr prev;
        public Pointer<SHPReference> Prev => prev;
        [FieldOffset(32)] public UInt32 _20;
    }

    [StructLayout(LayoutKind.Explicit, Size = 24)]
    public struct SHPFrame
    {
        [FieldOffset(0)] public short Left;
        [FieldOffset(2)] public short Top;
        [FieldOffset(4)] public short Width;
        [FieldOffset(6)] public short Height;
        [FieldOffset(8)] public uint Flags;
        [FieldOffset(12)] public ColorStruct Color;
        [FieldOffset(16)] public uint _10;
        [FieldOffset(20)] public int Offset;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SHPFile
    {
        public SHPStruct Base;
        public SHPFrame FirstFrame;
        public Pointer<SHPFrame> Frames => Pointer<SHPFrame>.AsPointer(ref FirstFrame);
    }
}
