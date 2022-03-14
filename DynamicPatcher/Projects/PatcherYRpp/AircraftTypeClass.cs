using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 3600)]
    public struct AircraftTypeClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0xA8B218);

        public static YRPP.GLOBAL_DVC_ARRAY<AircraftTypeClass> ABSTRACTTYPE_ARRAY = new YRPP.GLOBAL_DVC_ARRAY<AircraftTypeClass>(ArrayPointer);


        [FieldOffset(0)] public TechnoTypeClass Base;
        [FieldOffset(0)] public ObjectTypeClass BaseObjectType;
        [FieldOffset(0)] public AbstractTypeClass BaseAbstractType;

        [FieldOffset(3580)] public Bool Carryall;

        [FieldOffset(3584)] public Pointer<AnimTypeClass> Trailer;

        [FieldOffset(3588)] public int SpawnDelay;

        [FieldOffset(3592)] public Bool Rotors;

        [FieldOffset(3593)] public Bool CustomRotor;

        [FieldOffset(3594)] public Bool Landable;

        [FieldOffset(3595)] public Bool FlyBy;

        [FieldOffset(3596)] public Bool FlyBack;

        [FieldOffset(3597)] public Bool AirportBound;

        [FieldOffset(3598)] public Bool Fighter;

    }
}
