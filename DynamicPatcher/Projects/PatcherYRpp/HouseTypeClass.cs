using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 432)]
    [Serializable]
    public struct HouseTypeClass
    {

        public static readonly IntPtr ArraryPointer = new IntPtr(0xA83C98);

        public static YRPP.GLOBAL_DVC_ARRAY<HouseTypeClass> ABSTRACTTYPE_ARRAY = new YRPP.GLOBAL_DVC_ARRAY<HouseTypeClass>(ArraryPointer);

        [FieldOffset(0)]
        public AbstractTypeClass Base;

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

    }
}
