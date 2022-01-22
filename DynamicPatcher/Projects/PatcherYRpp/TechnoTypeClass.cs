using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 3576)]
    [Serializable]
    public struct TechnoTypeClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0xA8EB00);

        public static YRPP.GLOBAL_DVC_ARRAY<TechnoTypeClass> ABSTRACTTYPE_ARRAY = new YRPP.GLOBAL_DVC_ARRAY<TechnoTypeClass>(ArrayPointer);

        [FieldOffset(0)]
        public ObjectTypeClass Base;

        [FieldOffset(668)] public AbilitiesStruct VeteranAbilities;

        [FieldOffset(686)] public AbilitiesStruct EliteAbilities;

        [FieldOffset(760)] public int SlowdownDistance;

        [FieldOffset(844)] public Guid Locomotor;

        [FieldOffset(944)] public float PitchAngle;

        [FieldOffset(988)] public int LeptonMindControlOffset;

        [FieldOffset(992)] public int PixelSelectionBracketDelta;

        [FieldOffset(1464)] public int GuardRange;

        [FieldOffset(1512)] public int Sight;

        [FieldOffset(1552)] public int Cost;

        [FieldOffset(1560)] public int FlightLevel;

        [FieldOffset(1656)] public int Speed;

        [FieldOffset(1664)] public int InitialAmmo;

        [FieldOffset(1668)] public int Ammo;

        [FieldOffset(1672)] public int IFVMode;

        [FieldOffset(1676)] public int AirRangeBonus;

        [FieldOffset(1688)] public int Reload;

        [FieldOffset(1692)] public int EmptyReload;

        [FieldOffset(1696)] public int ReloadIncrement;

        [FieldOffset(1749)] public Bool AllowedToStartInMultiplayer;

        [FieldOffset(1820)] public int ROT;

        [FieldOffset(1824)] public int TurretOffset;

        [FieldOffset(2060)] public int WeaponCount;

        [FieldOffset(2068)] public CoordStruct turretWeaponFLH_first; // index 6 - 10 is AlternateFLH0 - AlternateFLH4, if no data use Weapon1FLH's data.
        public Pointer<CoordStruct> TurretWeaponFLH => Pointer<CoordStruct>.AsPointer(ref turretWeaponFLH_first);

        [FieldOffset(2200)] public WeaponStruct weapon_first;
        public Pointer<WeaponStruct> Weapon => Pointer<WeaponStruct>.AsPointer(ref weapon_first);

        [FieldOffset(2708)] public WeaponStruct eliteWeapon_first;
        public Pointer<WeaponStruct> EliteWeapon => Pointer<WeaponStruct>.AsPointer(ref eliteWeapon_first);

        [FieldOffset(3212)] public Bool TypeImmune;

        [FieldOffset(3213)] public Bool MoveToShroud;

        [FieldOffset(3214)] public Bool Trainable;

        [FieldOffset(3215)] public Bool DamageSparks;

        [FieldOffset(3216)] public Bool TargetLaser;

        [FieldOffset(3231)] public Bool DontScore;

        [FieldOffset(3233)] public Bool Turret;

        [FieldOffset(3277)] public Bool Crewed;

        [FieldOffset(3280)] public Bool Cloakable;

        [FieldOffset(3284)] public Bool Teleporter;

        [FieldOffset(3285)] public Bool IsGattling;

        [FieldOffset(3288)] public int WeaponStages;

        [FieldOffset(3292)] public int weaponStage_first;
        public Pointer<int> WeaponStage => Pointer<int>.AsPointer(ref weaponStage_first);

        [FieldOffset(3292)] public int eliteStage_first;
        public Pointer<int> EliteStage => Pointer<int>.AsPointer(ref eliteStage_first);

        [FieldOffset(3348)] public Bool SelfHealing;

        [FieldOffset(3349)] public Bool Explodes;

        [FieldOffset(3361)] public Bool TurretSpins;

        [FieldOffset(3367)] public Bool HunterSeeker;

        [FieldOffset(3368)] public Bool Crusher;

        [FieldOffset(3369)] public Bool OmniCrusher;

        [FieldOffset(3370)] public Bool OmniCrushResistant;

        [FieldOffset(3371)] public Bool TiltsWhenCrushes;

        [FieldOffset(3372)] public Bool IsSubterranean;

        [FieldOffset(3373)] public Bool AutoCrush;

        [FieldOffset(3374)] public Bool Bunkerable;

        [FieldOffset(3375)] public Bool CanDisguise;

        [FieldOffset(3376)] public Bool PermaDisguise;

        [FieldOffset(3377)] public Bool DetectDisguise;

        [FieldOffset(3378)] public Bool DisguiseWhenStill;

        [FieldOffset(3379)] public Bool CanApproachTarget;

        [FieldOffset(3380)] public Bool CanRecalcApproachTarget;

        [FieldOffset(3412)] public Bool Spawned;

        [FieldOffset(3476)] public Bool Jumpjet;

        [FieldOffset(3516)] public Bool IsSelectableCombatant;

        public Pointer<WeaponTypeClass> get_Primary() => Weapon[0].WeaponType;
        public Pointer<WeaponTypeClass> get_Secondary() => Weapon[1].WeaponType;
        public Pointer<WeaponTypeClass> get_Weapon(int i) => Weapon[i].WeaponType;
        public CoordStruct get_WeaponFLH(int i) => Weapon[i].FLH;
        public Pointer<WeaponTypeClass> get_ElitePrimary() => EliteWeapon[0].WeaponType;
        public Pointer<WeaponTypeClass> get_EliteSecondary() => EliteWeapon[1].WeaponType;
        public Pointer<WeaponTypeClass> get_EliteWeapon(int i) => EliteWeapon[i].WeaponType;
        public CoordStruct get_EliteWeaponFLH(int i) => EliteWeapon[i].FLH;

        public static Pointer<TechnoTypeClass> Find(string id)
        {
            return ABSTRACTTYPE_ARRAY.Find(id);
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 18)]
    [Serializable]
    public struct AbilitiesStruct
    {
        public Bool FASTER; //0x00
        public Bool STRONGER; //0x01
        public Bool FIREPOWER; //0x02
        public Bool SCATTER; //0x03
        public Bool ROF; //0x04
        public Bool SIGHT; //0x05
        public Bool CLOAK; //0x06
        public Bool TIBERIUM_PROOF; //0x07
        public Bool VEIN_PROOF; //0x08
        public Bool SELF_HEAL; //0x09
        public Bool EXPLODES; //0x0A
        public Bool RADAR_INVISIBLE; //0x0B
        public Bool SENSORS; //0x0C
        public Bool FEARLESS; //0x0D
        public Bool C4; //0x0E
        public Bool TIBERIUM_HEAL; //0x0F
        public Bool GUARD_AREA; //0x10
        public Bool CRUSHER; //0x11
    }

    [StructLayout(LayoutKind.Explicit, Size = 28)]
    [Serializable]
    public struct WeaponStruct
    {
        [FieldOffset(0)] public Pointer<WeaponTypeClass> WeaponType;
        [FieldOffset(4)] public CoordStruct FLH;
        [FieldOffset(16)] public int BarrelLength;
        [FieldOffset(20)] public int BarrelThickness;
        [FieldOffset(24)] public Bool TurretLocked;
    }
}
