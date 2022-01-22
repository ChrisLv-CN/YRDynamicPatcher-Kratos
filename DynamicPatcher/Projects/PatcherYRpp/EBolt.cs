using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 48)]
    [Serializable]
    public struct EBolt
    {

        public static unsafe void Constructor(Pointer<EBolt> pThis)
        {
            var func = (delegate* unmanaged[Thiscall]<ref EBolt, void>)0x4C1E10;
            func(ref pThis.Ref);
        }

        public unsafe void Fire(CoordStruct point1, CoordStruct point2, int arg18)
        {
            var func = (delegate* unmanaged[Thiscall]<ref EBolt, CoordStruct, CoordStruct, int, void>)0x4C2A60;
            func(ref this, point1, point2, arg18);
        }

        [FieldOffset(0)] public CoordStruct Point1;

        [FieldOffset(12)] public CoordStruct Point2;

        [FieldOffset(28)] public int Random; //Random number between 0 and 256

        [FieldOffset(32)] public Pointer<TechnoClass> Owner; //ingame this is a UnitClass but needed to circumvent some issues

        [FieldOffset(36)] public int WeaponSlot; // which weapon # to use from owner

        [FieldOffset(40)] public int Lifetime; // this is >>= 1 each time DrawAll() is called, 0 => dtor (inline). Hi, welcome to dumb ideas.

        [FieldOffset(44)] public Bool AlternateColor;
    }
}
