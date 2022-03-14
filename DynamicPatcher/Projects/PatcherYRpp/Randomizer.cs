using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 1012)]
    public struct Randomizer
    {

        // for any randomization happening inside a match (odds of a survivor, crate, etc), use the ScenarioClass::Random object instead!
        // this object should only be used for RMG and other randomness outside a match
        private static IntPtr instance = new IntPtr(0x886B88);
        public static ref Pointer<Randomizer> Instance { get => ref instance.Convert<Pointer<Randomizer>>().Ref; }

        public static Randomizer Global()
        {
            return Instance.Ref;
        }

        public static unsafe void Constructor(Pointer<Randomizer> pThis, double seed)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Randomizer, double, void>)0x65C6D0;
            func(ref pThis.Ref, seed);
        }

        public unsafe double Random()
        {
            var func = (delegate* unmanaged[Thiscall]<ref Randomizer, double>)0x65C780;
            return func(ref this);
        }

        public unsafe int RandomRanged(int min, int max)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Randomizer, int, int, int>)0x65C7E0;
            return func(ref this, min, max);
        }

        [FieldOffset(0)] public Bool unknown_00;

        [FieldOffset(4)] public int Next1;
        [FieldOffset(8)] public int Next2;
        // [FieldOffset(12)] public long[] Table[0xFA];
    }
}
