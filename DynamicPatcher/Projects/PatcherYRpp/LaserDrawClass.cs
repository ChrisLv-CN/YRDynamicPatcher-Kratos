using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PatcherYRpp.YRPP;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 92)]
    public struct LaserDrawClass
    {
        public static unsafe void Constructor(Pointer<LaserDrawClass> pThis, CoordStruct source, CoordStruct target, int zAdjust, byte unknown,
            ColorStruct innerColor, ColorStruct outerColor, ColorStruct outerSpread,
            int duration, bool blinks = false, bool fades = true,
            float startIntensity = 1.0f, float endIntensity = 0.0f)
        {
            var func = (delegate* unmanaged[Thiscall]<ref LaserDrawClass, CoordStruct, CoordStruct, int, byte, 
                ColorStruct, ColorStruct, ColorStruct,  int, Bool, Bool, float, float, void>)0x54FE60;
            func(ref pThis.Ref, source, target, zAdjust, unknown, innerColor, outerColor, outerSpread, duration, blinks, fades, startIntensity, endIntensity);
        }

        public static void Constructor(Pointer<LaserDrawClass> pThis, CoordStruct source, CoordStruct target, ColorStruct innerColor,
            ColorStruct outerColor, ColorStruct outerSpread, int duration, bool blinks = false)
        {
            Constructor(pThis, source, target, 0, 1, innerColor, outerColor, outerSpread, duration, blinks);
        }

        public static void Constructor(Pointer<LaserDrawClass> pThis, CoordStruct source, CoordStruct target, ColorStruct innerColor,
            ColorStruct outerColor, ColorStruct outerSpread, int duration)
        {
            Constructor(pThis, source, target, 0, 1, innerColor, outerColor, outerSpread, duration);
        }

        public unsafe void Destructor()
        {
            var func = (delegate* unmanaged[Thiscall]<ref LaserDrawClass, void>)0x54FFB0;
            func(ref this);
        }

        [FieldOffset(28)] public int Thickness; // only respected if IsHouseColor
        [FieldOffset(32)] public Bool IsHouseColor;
        [FieldOffset(33)] public Bool IsSupported; // this changes the values for InnerColor (false: halve, true: double), HouseColor only

        [FieldOffset(65)] public ColorStruct InnerColor;
        [FieldOffset(68)] public ColorStruct OuterColor;
        [FieldOffset(71)] public ColorStruct OuterSpread;
        [FieldOffset(76)] public int Duration;
        [FieldOffset(80)] public Bool Blinks;
        [FieldOffset(81)] public byte BlinkState;
        [FieldOffset(82)] public Bool Fades;
    }
}
