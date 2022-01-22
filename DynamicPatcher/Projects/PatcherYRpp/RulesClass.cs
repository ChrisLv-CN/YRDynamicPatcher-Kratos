using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 6336)]
    [Serializable]
    public struct RulesClass
    {

        private static IntPtr instance = new IntPtr(0x8871E0);
        public static ref Pointer<RulesClass> Instance { get => ref instance.Convert<Pointer<RulesClass>>().Ref; }

        public static RulesClass Global()
        {
            return Instance.Ref;
        }

        [FieldOffset(1640)] public double VeteranRatio;

        [FieldOffset(1648)] public double VeteranCombat;

        [FieldOffset(1656)] public double VeteranSpeed;

        [FieldOffset(1664)] public double VeteranSight;

        [FieldOffset(1672)] public double VeteranArmor;

        [FieldOffset(1680)] public double VeteranROF;

        [FieldOffset(1688)] public double VeteranCap;

        [FieldOffset(4008)] public Pointer<WarheadTypeClass> C4Warhead;

        [FieldOffset(4012)] public Pointer<WarheadTypeClass> CrushWarhead;

        [FieldOffset(5816)] public int Gravity;

        [FieldOffset(5832)] public int MaxDamage;

        [FieldOffset(5940)] public int BallisticScatter; // value in *256

        [FieldOffset(6148)] public int RadDurationMultiple;

        [FieldOffset(6152)] public int RadApplicationDelay;

        [FieldOffset(6156)] public int RadLevelMax;

        [FieldOffset(6160)] public int RadLevelDelay;

        [FieldOffset(6164)] public int RadLightDelay;

        [FieldOffset(6168)] public double RadLevelFactor;

        [FieldOffset(6176)] public double RadLightFactor;

        [FieldOffset(6184)] public double RadTintFactor;

        [FieldOffset(6192)] public ColorStruct RadColor;

        [FieldOffset(6196)] public Pointer<WarheadTypeClass> RadSiteWarhead;

        [FieldOffset(6246)] public ColorStruct ChronoBeamColor;
        
        [FieldOffset(6249)] public ColorStruct MagnaBeamColor;

        [FieldOffset(6260)] public ColorStruct colorAdd_first;
        public Pointer<ColorStruct> ColorAdd => Pointer<ColorStruct>.AsPointer(ref colorAdd_first);

        [FieldOffset(6308)] public int laserTargetColor;
        public static ColorStruct LaserTargetColor => Instance.Ref.ColorAdd[Instance.Ref.laserTargetColor];

        [FieldOffset(6312)] public int ironCurtainColor;
        public static ColorStruct IronCurtainColor => Instance.Ref.ColorAdd[Instance.Ref.ironCurtainColor];

        [FieldOffset(6316)] public int berserkColor;
        public static ColorStruct BerserkColor => Instance.Ref.ColorAdd[Instance.Ref.berserkColor];

        [FieldOffset(6320)] public int forceShieldColor;
        public static ColorStruct ForceShieldColor => Instance.Ref.ColorAdd[Instance.Ref.forceShieldColor];

    }
}
