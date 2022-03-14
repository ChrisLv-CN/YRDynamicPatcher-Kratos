using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 352)]
    public struct WeaponTypeClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0x887568);

        public static YRPP.GLOBAL_DVC_ARRAY<WeaponTypeClass> ABSTRACTTYPE_ARRAY = new YRPP.GLOBAL_DVC_ARRAY<WeaponTypeClass>(ArrayPointer);

        [FieldOffset(0)] public AbstractTypeClass Base;
        [FieldOffset(152)] public int AmbientDamage;
        [FieldOffset(156)] public int Burst;
        [FieldOffset(160)] public IntPtr projectile;
        public Pointer<BulletTypeClass> Projectile { get => projectile; set => projectile = value; }

        [FieldOffset(164)] public int Damage;
        [FieldOffset(168)] public int Speed;
        [FieldOffset(172)] public Pointer<WarheadTypeClass> Warhead;
        [FieldOffset(176)] public int ROF;
        [FieldOffset(180)] public int Range; // int(256 * ini value)
        [FieldOffset(184)] public int MinimumRange; // int(256 * ini value)
        [FieldOffset(188)] public DynamicVectorClass<int> Report;       //sound indices
        [FieldOffset(216)] public DynamicVectorClass<int> DownReport;   //sound indices
        [FieldOffset(244)] public DynamicVectorClass<Pointer<AnimTypeClass>> Anim;
        [FieldOffset(272)] public Pointer<AnimTypeClass> OccupantAnim;
        [FieldOffset(276)] public Pointer<AnimTypeClass> AssaultAnim;
        [FieldOffset(280)] public Pointer<AnimTypeClass> OpenToppedAnim;
        [FieldOffset(284)] public Pointer<ParticleSystemTypeClass> AttachedParticleSystem;
        [FieldOffset(288)] public ColorStruct LaserInnerColor;
        [FieldOffset(291)] public ColorStruct LaserOuterColor;
        [FieldOffset(294)] public ColorStruct LaserOuterSpread;
        [FieldOffset(297)] public Bool UseFireParticles;
        [FieldOffset(298)] public Bool UseSparkParticles;
        [FieldOffset(299)] public Bool OmniFire;
        [FieldOffset(300)] public Bool DistributedWeaponFire;
        [FieldOffset(301)] public Bool IsRailgun;
        [FieldOffset(302)] public Bool Lobber;
        [FieldOffset(303)] public Bool Bright;
        [FieldOffset(304)] public Bool IsSonic;
        [FieldOffset(305)] public Bool Spawner;
        [FieldOffset(306)] public Bool LimboLaunch;
        [FieldOffset(307)] public Bool DecloakToFire;
        [FieldOffset(308)] public Bool CellRangefinding;
        [FieldOffset(309)] public Bool FireOnce;
        [FieldOffset(310)] public Bool NeverUse;
        [FieldOffset(311)] public Bool RevealOnFire;
        [FieldOffset(312)] public Bool TerrainFire;
        [FieldOffset(313)] public Bool SabotageCursor;
        [FieldOffset(314)] public Bool MigAttackCursor;
        [FieldOffset(315)] public Bool DisguiseFireOnly;
        [FieldOffset(316)] public int DisguiseFakeBlinkTime;
        [FieldOffset(320)] public Bool InfiniteMindControl;
        [FieldOffset(321)] public Bool FireWhileMoving;
        [FieldOffset(322)] public Bool DrainWeapon;
        [FieldOffset(323)] public Bool FireInTransport;
        [FieldOffset(324)] public Bool Suicide;
        [FieldOffset(325)] public Bool TurboBoost;
        [FieldOffset(326)] public Bool Supress;
        [FieldOffset(327)] public Bool Camera;
        [FieldOffset(328)] public Bool Charges;
        [FieldOffset(329)] public Bool IsLaser;
        [FieldOffset(330)] public Bool DiskLaser;
        [FieldOffset(331)] public Bool IsLine;
        [FieldOffset(332)] public Bool IsBigLaser;
        [FieldOffset(333)] public Bool IsHouseColor;
        [FieldOffset(334)] public char LaserDuration;
        [FieldOffset(335)] public Bool IonSensitive;
        [FieldOffset(336)] public Bool AreaFire;
        [FieldOffset(337)] public Bool IsElectricBolt;
        [FieldOffset(338)] public Bool DrawBoltAsLaser;
        [FieldOffset(339)] public Bool IsAlternateColor;
        [FieldOffset(340)] public Bool IsRadBeam;
        [FieldOffset(341)] public Bool IsRadEruption;
        [FieldOffset(344)] public int RadLevel;
        [FieldOffset(348)] public Bool IsMagBeam;

    }
}