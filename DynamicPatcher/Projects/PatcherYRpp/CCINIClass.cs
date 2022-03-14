using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 88)]
    public struct INIClass
    {

    }

    [StructLayout(LayoutKind.Explicit, Size = 88)]
    public struct CCINIClass
    {
        private static IntPtr ppINI_Rules = new IntPtr(0x887048);
        private static IntPtr pINI_AI = new IntPtr(0x887128);
        private static IntPtr pINI_Art = new IntPtr(0x887180);

        public static Pointer<CCINIClass> INI_Rules { get => ((Pointer<Pointer<CCINIClass>>)ppINI_Rules).Data; set => ((Pointer<Pointer<CCINIClass>>)ppINI_Rules).Ref = value; }
        public static Pointer<CCINIClass> INI_AI { get => pINI_AI; set => pINI_AI = value; }
        public static Pointer<CCINIClass> INI_Art { get => pINI_Art; set => pINI_Art = value; }

        //Parses an INI file from a CCFile
        public unsafe Pointer<CCINIClass> ReadCCFile(Pointer<CCFileClass> pCCFile, byte bUnk = 0, int iUnk = 0)
        {
            var func = (delegate* unmanaged[Thiscall]<ref CCINIClass, IntPtr, byte, int, IntPtr>)0x4741F0;
            return func(ref this, pCCFile, bUnk, iUnk);
        }

        public unsafe void Reset()
        {
            var func = (delegate* unmanaged[Thiscall]<ref CCINIClass, void>)0x526B00;
            func(ref this);
        }
        public unsafe int GetKeyCount(AnsiString section)
        {
            var func = (delegate* unmanaged[Thiscall]<ref CCINIClass, IntPtr, int>)0x526960;
            return func(ref this, section);
        }
        public unsafe AnsiString GetKeyName(AnsiString section, int keyIndex)
        {
            var func = (delegate* unmanaged[Thiscall]<ref CCINIClass, IntPtr, int, IntPtr>)0x526CC0;
            return func(ref this, section, keyIndex);
        }

        public unsafe int ReadString(AnsiString section, AnsiString key, AnsiString def, byte[] buffer, int bufferSize)
        {
            var func = (delegate* unmanaged[Thiscall]<ref CCINIClass, IntPtr, IntPtr, IntPtr, byte[], int, int>)0x528A10;
            return func(ref this, section, key, def, buffer, bufferSize);
        }

        public static unsafe void Constructor(Pointer<CCINIClass> pThis)
        {
            var func = (delegate* unmanaged[Thiscall]<ref CCINIClass, void>)0x535AA0;
            func(ref pThis.Ref);
            pThis.Ref.Digested = false;
            pThis.Convert<int>().Ref = 0x7E1AF4;
        }

        public static unsafe void Destructor(Pointer<CCINIClass> pThis)
        {
            var func = (delegate* unmanaged[Thiscall]<ref CCINIClass, Bool, void>)Helpers.GetVirtualFunctionPointer(pThis, 0);
            func(ref pThis.Ref, false);
        }

        [FieldOffset(0)] public INIClass Base;

        [FieldOffset(64)] public Bool Digested;
    }
}
