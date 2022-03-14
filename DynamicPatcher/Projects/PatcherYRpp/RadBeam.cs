using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 200)]
    public struct RadBeam
    {

        public static unsafe Pointer<RadBeam> Allocate(RadBeamType type)
        {
            var func = (delegate* unmanaged[Thiscall]<int, RadBeamType, IntPtr>)ASM.FastCallTransferStation;
            return func(0x659110, type);
        }

        public unsafe void SetColor(ColorStruct color)
        {
            this.Color = color;
        }

        public unsafe void SetCoordsSource(CoordStruct location)
        {
            this.SourceLocation = location;
        }

        public unsafe void SetCoordsTarget(CoordStruct location)
        {
            this.TargetLocation = location;
        }

        [FieldOffset(4)] public Pointer<TechnoClass> Owner;

        [FieldOffset(16)] public RadBeamType Type;

        [FieldOffset(32)] public ColorStruct Color;

        [FieldOffset(36)] public CoordStruct SourceLocation;

        [FieldOffset(48)] public CoordStruct TargetLocation;

        [FieldOffset(60)] public int Period;

        [FieldOffset(64)] public double Amplitude;
    }
}
