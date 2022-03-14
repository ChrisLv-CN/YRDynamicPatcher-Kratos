using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 700)]
    public struct TerrainTypeClass
    {
        [FieldOffset(0)] public ObjectTypeClass Base;
		[FieldOffset(0)] public AbstractTypeClass BaseAbstractType;

		[FieldOffset(660)] public int ArrayIndex;
		[FieldOffset(664)] public int Foundation;
		[FieldOffset(668)] public ColorStruct RadarColor;
		[FieldOffset(672)] public int AnimationRate;
		[FieldOffset(676)] public float AnimationProbability;
		[FieldOffset(680)] public int TemperateOccupationBits;
		[FieldOffset(684)] public int SnowOccupationBits;
		[FieldOffset(688)] public Bool WaterBound;
		[FieldOffset(689)] public Bool SpawnsTiberium;
		[FieldOffset(690)] public Bool IsFlammable;
		[FieldOffset(691)] public Bool IsAnimated;
		[FieldOffset(692)] public Bool IsVeinhole;
		[FieldOffset(696)] public Pointer<CellStruct> FoundationData;
	}
}
