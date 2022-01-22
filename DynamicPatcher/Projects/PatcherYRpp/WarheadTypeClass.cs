using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 464)]
    [Serializable]
    public struct WarheadTypeClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0x8874C0);

        public static YRPP.GLOBAL_DVC_ARRAY<WarheadTypeClass> ABSTRACTTYPE_ARRAY = new YRPP.GLOBAL_DVC_ARRAY<WarheadTypeClass>(ArrayPointer);

        [FieldOffset(0)] public AbstractTypeClass Base;
        [FieldOffset(152)] public float Deform;

        [FieldOffset(292)] public float CellSpread;
        [FieldOffset(296)] public float CellInset;
        [FieldOffset(300)] public float PercentAtMax;

        [FieldOffset(324)] public Bool Wall;
        [FieldOffset(325)] public Bool WallAbsoluteDestroyer;
        [FieldOffset(326)] public Bool PenetratesBunker;
        [FieldOffset(327)] public Bool Wood;
        [FieldOffset(328)] public Bool Tiberium;
        [FieldOffset(329)] public Bool unknown_bool_149;
        [FieldOffset(330)] public Bool Sparky;
        [FieldOffset(331)] public Bool Sonic;
        [FieldOffset(332)] public Bool Fire;
        [FieldOffset(333)] public Bool Conventional;
        [FieldOffset(334)] public Bool Rocker;
        [FieldOffset(335)] public Bool DirectRocker;
        [FieldOffset(336)] public Bool Bright;
        [FieldOffset(337)] public Bool CLDisableRed;
        [FieldOffset(338)] public Bool CLDisableGreen;
        [FieldOffset(339)] public Bool CLDisableBlue;
        [FieldOffset(340)] public Bool EMEffect;
        [FieldOffset(341)] public Bool MindControl;
        [FieldOffset(342)] public Bool Poison;
        [FieldOffset(343)] public Bool IvanBomb;
        [FieldOffset(344)] public Bool ElectricAssault;
        [FieldOffset(345)] public Bool Parasite;
        [FieldOffset(346)] public Bool Temporal;
        [FieldOffset(347)] public Bool IsLocomotor;
        //[FieldOffset(348)] public Guid Locomotor;
        [FieldOffset(364)] public Bool Airstrike;
        [FieldOffset(365)] public Bool Psychedelic;
        [FieldOffset(366)] public Bool BombDisarm;
        [FieldOffset(368)] public int Paralyzes;
        [FieldOffset(372)] public Bool Culling;
        [FieldOffset(373)] public Bool MakesDisguise;
        [FieldOffset(374)] public Bool NukeMaker;
        [FieldOffset(375)] public Bool Radiation;
        [FieldOffset(376)] public Bool PsychicDamage;
        [FieldOffset(377)] public Bool AffectsAllies;
        [FieldOffset(378)] public Bool Bullets;
        [FieldOffset(379)] public Bool Veinhole;
        [FieldOffset(380)] public int ShakeXlo;
        [FieldOffset(384)] public int ShakeXhi;
        [FieldOffset(388)] public int ShakeYlo;
        [FieldOffset(392)] public int ShakeYhi;

        [FieldOffset(424)] public DynamicVectorClass<int> DebrisMaximums;
        [FieldOffset(452)] public int MaxDebris;
        [FieldOffset(456)] public int MinDebris;
    }
}
