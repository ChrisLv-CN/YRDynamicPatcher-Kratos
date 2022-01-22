using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 888)]
    [Serializable]
    public struct AnimTypeClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0x8B4150);

        public static YRPP.GLOBAL_DVC_ARRAY<AnimTypeClass> ABSTRACTTYPE_ARRAY = new YRPP.GLOBAL_DVC_ARRAY<AnimTypeClass>(ArrayPointer);

        [FieldOffset(0)] public ObjectTypeClass Base;

        [FieldOffset(660)] public int ArrayIndex;
        [FieldOffset(664)] public int MiddleFrameIndex;
        [FieldOffset(668)] public int MiddleFrameWidth;
        [FieldOffset(672)] public int MiddleFrameHeight;
        [FieldOffset(676)] public byte unknown_2A4;

        [FieldOffset(680)] public double Damage;
        [FieldOffset(688)] public int Rate;
        [FieldOffset(692)] public int Start;
        [FieldOffset(696)] public int LoopStart;
        [FieldOffset(700)] public int LoopEnd;
        [FieldOffset(704)] public int End;
        [FieldOffset(708)] public int LoopCount;
        [FieldOffset(712)] public IntPtr next;
        public Pointer<AnimTypeClass> Next { get => next; set => next = value; }
        [FieldOffset(716)] public int SpawnsParticle; // index of that ParticleTypeClass
        [FieldOffset(720)] public int NumParticles;
        [FieldOffset(724)] public int DetailLevel;
        [FieldOffset(728)] public int TranslucencyDetailLevel;
        [FieldOffset(732)] public RandomStruct RandomLoopDelay;
        [FieldOffset(740)] public RandomStruct RandomRate;
        [FieldOffset(748)] public int Translucency;
        [FieldOffset(752)] public IntPtr spawns;
        public Pointer<AnimTypeClass> Spawns { get => spawns; set => spawns = value; }
        [FieldOffset(756)] public int SpawnCount;
        [FieldOffset(760)] public int Report;     //VocClass index
        [FieldOffset(764)] public int StopSound;      //VocClass index
        [FieldOffset(768)] public IntPtr bounceAnim;
        public Pointer<AnimTypeClass> BounceAnim { get => bounceAnim; set => bounceAnim = value; }
        [FieldOffset(772)] public IntPtr expireAnim;
        public Pointer<AnimTypeClass> ExpireAnim { get => expireAnim; set => expireAnim = value; }
        [FieldOffset(776)] public IntPtr trailerAnim;
        public Pointer<AnimTypeClass> TrailerAnim { get => trailerAnim; set => trailerAnim = value; }
        [FieldOffset(780)] public int TrailerSeperation;  //MISTYPE BY WESTWOOD!
        [FieldOffset(784)] public double Elasticity;
        [FieldOffset(792)] public double MinZVel;
        [FieldOffset(800)] public double unknown_double_320;
        [FieldOffset(808)] public double MaxXYVel;
        [FieldOffset(816)] public IntPtr warhead;
        public Pointer<WarheadTypeClass> Warhead { get => warhead; set => warhead = value; }
        [FieldOffset(820)] public int DamageRadius;
        [FieldOffset(824)] public IntPtr tiberiumSpawnType;
        public Pointer<OverlayTypeClass> TiberiumSpawnType { get => tiberiumSpawnType; set => tiberiumSpawnType = value; }
        [FieldOffset(828)] public int TiberiumSpreadRadius;
        [FieldOffset(832)] public int YSortAdjust;
        [FieldOffset(836)] public int YDrawOffset;
        [FieldOffset(840)] public int ZAdjust;
        [FieldOffset(844)] public int MakeInfantry;
        [FieldOffset(848)] public int RunningFrames;
        [FieldOffset(852)] public Bool IsFlamingGuy;
        [FieldOffset(853)] public Bool IsVeins;
        [FieldOffset(854)] public Bool IsMeteor;
        [FieldOffset(855)] public Bool TiberiumChainReaction;
        [FieldOffset(856)] public Bool IsTiberium;
        [FieldOffset(857)] public Bool HideIfNoOre;
        [FieldOffset(858)] public Bool Bouncer;
        [FieldOffset(859)] public Bool Tiled;
        [FieldOffset(860)] public Bool ShouldUseCellDrawer;
        [FieldOffset(861)] public Bool UseNormalLight;
        [FieldOffset(862)] public Bool DemandLoad; // not loaded from ini anymore
        [FieldOffset(863)] public Bool FreeLoad;  // not loaded from ini anymore
        [FieldOffset(864)] public Bool IsAnimatedTiberium;
        [FieldOffset(865)] public Bool AltPalette;
        [FieldOffset(866)] public Bool Normalized;
        [FieldOffset(868)] public Layer Layer;
        [FieldOffset(872)] public Bool DoubleThick;
        [FieldOffset(873)] public Bool Flat;
        [FieldOffset(874)] public Bool Translucent;
        [FieldOffset(875)] public Bool Scorch;
        [FieldOffset(876)] public Bool Flamer;
        [FieldOffset(877)] public Bool Crater;
        [FieldOffset(878)] public Bool ForceBigCraters;
        [FieldOffset(879)] public Bool Sticky;
        [FieldOffset(880)] public Bool PingPong;
        [FieldOffset(881)] public Bool Reverse;
        [FieldOffset(882)] public Bool Shadow;
        [FieldOffset(883)] public Bool PsiWarning;
        [FieldOffset(884)] public Bool ShouldFogRemove;
    }
}
