using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 328)]
    [Serializable]
    public struct VocClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0xB1D378);

        public static ref DynamicVectorClass<Pointer<VocClass>> Array { get => ref DynamicVectorClass<Pointer<VocClass>>.GetDynamicVector(ArrayPointer); }

        public static unsafe int FindIndex(string soundName)
        {
            var func = (delegate* unmanaged[Thiscall]<int, string, int>)ASM.FastCallTransferStation;
            return func(0x7514D0, soundName);
        }

        /* Play a sound independant of the position.
           n = Index of VocClass in Array to be played
           Volume = 0.0f to 1.0f
           Panning = 0x0000 (left) to 0x4000 (right) (0x2000 is center)
           */
        public static unsafe void PlayGlobal(int soundIndex, int panning, float volume, Pointer<AudioController> ctrl = default)
        {
            var func = (delegate* unmanaged[Thiscall]<int, int, int, float, IntPtr, void>)ASM.FastCallTransferStation;
            func(0x750920, soundIndex, panning, volume, ctrl);
        }

        /* Play a sound at a certain Position.
               n = Index of VocClass in Array to be played */
        public static unsafe void PlayAt(int soundIndex, CoordStruct location, Pointer<AudioController> ctrl = default)
        {
            var func = (delegate* unmanaged[Thiscall]<int, int, ref CoordStruct, IntPtr, void>)ASM.FastCallTransferStation;
            func(0x7509E0, soundIndex, ref location, ctrl);
        }

        // calls the one above ^ - probably sanity checks and whatnot
        public static unsafe void PlayIndexAtPos(int soundIndex, CoordStruct location, int a3)
        {
            var func = (delegate* unmanaged[Thiscall]<int, int, ref CoordStruct, int, void>)ASM.FastCallTransferStation;
            func(0x750E20, soundIndex, ref location, a3);
        }

        [FieldOffset(0)] public VocClassHeader Header;

        [FieldOffset(108)] public string Name;

    }
}
