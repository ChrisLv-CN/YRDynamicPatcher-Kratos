using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 96)]
    public struct FlyLocomotionClass
    {


        [FieldOffset(0)] public LocomotionClass Base;

		[FieldOffset(24)] public Bool AirportBound;
		[FieldOffset(28)] public CoordStruct MovingDestination;
		[FieldOffset(40)] public CoordStruct XYZ2;
		[FieldOffset(52)] public Bool HasMoveOrder;
		[FieldOffset(56)] public int FlightLevel;
		[FieldOffset(64)] public double TargetSpeed;
		[FieldOffset(72)] public double CurrentSpeed;
		[FieldOffset(80)] public byte IsTakingOff;
		[FieldOffset(81)] public Bool IsLanding;
		[FieldOffset(82)] public Bool WasLanding;
		[FieldOffset(83)] public Bool unknown_bool_53;
		[FieldOffset(84)] public int unknown_54;
		[FieldOffset(88)] public int unknown_58;
		[FieldOffset(92)] public Bool IsElevating;
		[FieldOffset(93)] public Bool unknown_bool_5D;
		[FieldOffset(94)] public Bool unknown_bool_5E;
		[FieldOffset(95)] public Bool unknown_bool_5F;
	}
}
