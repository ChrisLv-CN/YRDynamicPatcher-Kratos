using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 432)]
    public struct HouseTypeClass
	{
		public static readonly IntPtr ArrayPointer = new IntPtr(0xA83C98);

		public static YRPP.GLOBAL_DVC_ARRAY<HouseTypeClass> ABSTRACTTYPE_ARRAY = new YRPP.GLOBAL_DVC_ARRAY<HouseTypeClass>(ArrayPointer);



		public Pointer<HouseTypeClass> FindParentCountry()
		{
			return ABSTRACTTYPE_ARRAY.Find(ParentCountry);
		}
		public int FindParentCountryIndex()
		{
			return FindIndexOfName(ParentCountry);
		}
		public static unsafe int FindIndexOfName(AnsiString name)
		{
			var func = (delegate* unmanaged[Thiscall]<IntPtr, int>)0x5117D0;
			return func(name);
		}

		[FieldOffset(0)] public AbstractTypeClass Base;

		[FieldOffset(152)] public byte ParentCountry_first;
		public AnsiStringPointer ParentCountry => Pointer<byte>.AsPointer(ref ParentCountry_first);

		[FieldOffset(180)] public int ArrayIndex;
		[FieldOffset(184)] public int ArrayIndex2; //dunno why
		[FieldOffset(188)] public int SideIndex;
		[FieldOffset(192)] public int ColorSchemeIndex;

		//are these unused TS leftovers?
		[FieldOffset(200)] public double FirepowerMult;
		[FieldOffset(208)] public double GroundspeedMult;
		[FieldOffset(216)] public double AirspeedMult;
		[FieldOffset(224)] public double ArmorMult;
		[FieldOffset(232)] public double ROFMult;
		[FieldOffset(240)] public double CostMult;
		[FieldOffset(248)] public double BuildtimeMult;

		[FieldOffset(256)] public float ArmorInfantryMult;
		[FieldOffset(260)] public float ArmorUnitsMult;
		[FieldOffset(264)] public float ArmorAircraftMult;
		[FieldOffset(268)] public float ArmorBuildingsMult;
		[FieldOffset(272)] public float ArmorDefensesMult;

		[FieldOffset(276)] public float CostInfantryMult;
		[FieldOffset(280)] public float CostUnitsMult;
		[FieldOffset(284)] public float CostAircraftMult;
		[FieldOffset(288)] public float CostBuildingsMult;
		[FieldOffset(292)] public float CostDefensesMult;

		[FieldOffset(296)] public float SpeedInfantryMult;
		[FieldOffset(300)] public float SpeedUnitsMult;
		[FieldOffset(304)] public float SpeedAircraftMult;

		[FieldOffset(308)] public float BuildtimeInfantryMult;
		[FieldOffset(312)] public float BuildtimeUnitsMult;
		[FieldOffset(316)] public float BuildtimeAircraftMult;
		[FieldOffset(320)] public float BuildtimeBuildingsMult;
		[FieldOffset(324)] public float BuildtimeDefensesMult;

		[FieldOffset(328)] public float IncomeMult;

		[FieldOffset(332)] public DynamicVectorClass<Pointer<InfantryTypeClass>> VeteranInfantry;
		[FieldOffset(360)] public DynamicVectorClass<Pointer<UnitTypeClass>> VeteranUnits;
		[FieldOffset(388)] public DynamicVectorClass<Pointer<AircraftTypeClass>> VeteranAircraft;

		[FieldOffset(416)] public byte Suffix_first;
		public AnsiStringPointer Suffix => Pointer<byte>.AsPointer(ref Suffix_first);

		[FieldOffset(420)] public char Prefix;
		[FieldOffset(421)] public Bool Multiplay;
		[FieldOffset(422)] public Bool MultiplayPassive;
		[FieldOffset(423)] public Bool WallOwner;
		[FieldOffset(424)] public Bool SmartAI; //"smart"?
	}
}



