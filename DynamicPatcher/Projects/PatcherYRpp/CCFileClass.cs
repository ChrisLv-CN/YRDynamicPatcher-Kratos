using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 108)]
    public struct CCFileClass
    {
        public unsafe AnsiString GetFileName()
        {
            var func = (delegate* unmanaged[Thiscall]<ref CCFileClass, IntPtr>)this.GetVirtualFunctionPointer(1);
            return func(ref this);
        }
        public unsafe bool Exists(bool writeShared = false)
        {
            var func = (delegate* unmanaged[Thiscall]<ref CCFileClass, Bool, Bool>)this.GetVirtualFunctionPointer(5);
            return func(ref this, writeShared);
        }

        public static unsafe void Constructor(Pointer<CCFileClass> pThis, string fileName)
        {
            var func = (delegate* unmanaged[Thiscall]<ref CCFileClass, IntPtr, void>)0x4739F0;
            func(ref pThis.Ref, new AnsiString(fileName));
        }

        public static unsafe void Destructor(Pointer<CCFileClass> pThis)
        {
            var func = (delegate* unmanaged[Thiscall]<ref CCFileClass, Bool, void>)Helpers.GetVirtualFunctionPointer(pThis, 0);
            func(ref pThis.Ref, false);
        }


        public unsafe IntPtr ReadWholeFile()
        {
            var func = (delegate* unmanaged[Thiscall]<ref CCFileClass, IntPtr>)0x4A3890;
            return func(ref this);
        }


        [FieldOffset(12)] public int FilePointer;
        [FieldOffset(16)] public int FileSize;

        [FieldOffset(24)] public AnsiStringPointer FileName;

        [FieldOffset(32)] public Bool FileNameAllocated;
    }
}
