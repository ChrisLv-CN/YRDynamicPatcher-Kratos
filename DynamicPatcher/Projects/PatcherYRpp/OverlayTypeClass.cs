using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 700)]
    public struct OverlayTypeClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0xA83D80);

        public static YRPP.GLOBAL_DVC_ARRAY<OverlayTypeClass> ABSTRACTTYPE_ARRAY = new YRPP.GLOBAL_DVC_ARRAY<OverlayTypeClass>(ArrayPointer);

        [FieldOffset(0)] public ObjectTypeClass Base;
        [FieldOffset(0)] public AbstractTypeClass BaseAbstractType;

        [FieldOffset(660)] public int ArrayIndex;
        [FieldOffset(664)] public LandType LandType;
        [FieldOffset(668)] public Pointer<AnimTypeClass> CellAnim;
        [FieldOffset(672)] public int DamageLevels;
        [FieldOffset(676)] public int Strength;
        [FieldOffset(680)] public Bool Wall;
        [FieldOffset(681)] public Bool Tiberium;
        [FieldOffset(682)] public Bool Crate;
        [FieldOffset(683)] public Bool CrateTrigger;
        [FieldOffset(684)] public Bool NoUseTileLandType;
        [FieldOffset(685)] public Bool IsVeinholeMonster;
        [FieldOffset(686)] public Bool IsVeins;
        [FieldOffset(687)] public Bool ImageLoaded;   //not INI
        [FieldOffset(688)] public Bool Explodes;
        [FieldOffset(689)] public Bool ChainReaction;
        [FieldOffset(690)] public Bool Overrides;
        [FieldOffset(691)] public Bool DrawFlat;
        [FieldOffset(692)] public Bool IsRubble;
        [FieldOffset(693)] public Bool IsARock;
        [FieldOffset(694)] public ColorStruct RadarColor;
    }
}
