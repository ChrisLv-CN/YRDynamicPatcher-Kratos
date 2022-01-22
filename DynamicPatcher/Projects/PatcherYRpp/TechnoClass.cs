using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PatcherYRpp.FileFormats;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 1312)]
    [Serializable]
    public struct TechnoClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0xA8EC78);
        public static ref DynamicVectorClass<Pointer<TechnoClass>> Array { get => ref DynamicVectorClass<Pointer<TechnoClass>>.GetDynamicVector(ArrayPointer); }

        public unsafe Pointer<TechnoTypeClass> Type
        {
            get
            {
                var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr>)0x6F3270;
                return func(ref this);
            }
        }

        public unsafe bool IsVoxel()
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, byte>)Helpers.GetVirtualFunctionPointer(Pointer<TechnoClass>.AsPointer(ref this), 166);
            return Convert.ToBoolean(func(ref this));
        }

        public unsafe Pointer<FacingStruct> GetTurretFacing(ref FacingStruct facing)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, ref FacingStruct, IntPtr>)Helpers.GetVirtualFunctionPointer(Pointer<TechnoClass>.AsPointer(ref this), 170);
            return func(ref this, ref facing);
        }

        public unsafe Pointer<FacingStruct> GetRealFacing(ref FacingStruct facing)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, ref FacingStruct, IntPtr>)Helpers.GetVirtualFunctionPointer(Pointer<TechnoClass>.AsPointer(ref this), 194);
            return func(ref this, ref facing);
        }

        public unsafe int GetROF(int weaponIndex)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, int, int>)Helpers.GetVirtualFunctionPointer(Pointer<TechnoClass>.AsPointer(ref this), 198);
            return func(ref this, weaponIndex);
        }

        public unsafe int SelectWeapon(Pointer<AbstractClass> pTarget)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr, int>)Helpers.GetVirtualFunctionPointer(Pointer<TechnoClass>.AsPointer(ref this), 185);
            return func(ref this, pTarget);
        }

        public unsafe int GetZAdjustment()
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, int>)Helpers.GetVirtualFunctionPointer(Pointer<TechnoClass>.AsPointer(ref this), 187);
            return func(ref this);
        }

        public unsafe int GetZGradient()
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, int>)Helpers.GetVirtualFunctionPointer(Pointer<TechnoClass>.AsPointer(ref this), 188);
            return func(ref this);
        }

        public unsafe bool IsCloseEnough(Pointer<AbstractClass> pTarget, int weaponIdx)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr, int, Bool>)Helpers.GetVirtualFunctionPointer(Pointer<TechnoClass>.AsPointer(ref this), 234);
            return func(ref this, pTarget, weaponIdx);
        }

        public unsafe int Destroyed(Pointer<ObjectClass> pKiller)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr, int>)Helpers.GetVirtualFunctionPointer(Pointer<TechnoClass>.AsPointer(ref this), 238);
            return func(ref this, pKiller);
        }

        public unsafe FireError GetFireErrorWithoutRange(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr, int, FireError>)Helpers.GetVirtualFunctionPointer(Pointer<TechnoClass>.AsPointer(ref this), 239);
            return func(ref this, pTarget, weaponIndex);
        }

        public unsafe FireError GetFireError(Pointer<AbstractClass> pTarget, int weaponIndex, Bool checkRange)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr, int, Bool, FireError>)Helpers.GetVirtualFunctionPointer(Pointer<TechnoClass>.AsPointer(ref this), 240);
            return func(ref this, pTarget, weaponIndex, checkRange);
        }

        public unsafe void SetTarget(Pointer<AbstractClass> pTarget)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr, void>)Helpers.GetVirtualFunctionPointer(Pointer<TechnoClass>.AsPointer(ref this), 242);
            func(ref this, pTarget);
        }

        public unsafe void SetTargetForPassengers(Pointer<AbstractClass> pTarget)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr, void>)0x710550;
            func(ref this, pTarget);
        }

        public unsafe Pointer<BulletClass> Fire(Pointer<AbstractClass> pTarget, int idxWeapon)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr, int, IntPtr>)Helpers.GetVirtualFunctionPointer(Pointer<TechnoClass>.AsPointer(ref this), 243);
            return func(ref this, pTarget, idxWeapon);
        }

        public unsafe Pointer<BulletClass> Fire_IgnoreType(Pointer<AbstractClass> pTarget, int idxWeapon)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr, int, IntPtr>)0x6FDD50;
            return func(ref this, pTarget, idxWeapon);
        }

        public unsafe Pointer<WeaponStruct> GetDeployWeapon()
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr>)0x6FB080;
            return func(ref this);
        }

        public unsafe Pointer<WeaponStruct> GetTurretWeapon()
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr>)0x6FB080;
            return func(ref this);
        }

        public unsafe Pointer<WeaponStruct> GetWeapon(int i)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, int, IntPtr>)Helpers.GetVirtualFunctionPointer(Pointer<TechnoClass>.AsPointer(ref this), 254);
            return func(ref this, i);
        }

        public unsafe bool HasTurret()
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, Bool>)Helpers.GetVirtualFunctionPointer(Pointer<TechnoClass>.AsPointer(ref this), 255);
            return func(ref this);
        }

        public unsafe void SetDestination(Pointer<CellClass> pCell, bool unknow)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr, Bool, void>)Helpers.GetVirtualFunctionPointer(Pointer<TechnoClass>.AsPointer(ref this), 288);
            func(ref this, pCell, unknow);
        }

        public unsafe void SetFocus(Pointer<AbstractClass> pTarget)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr, void>)0x70C610;
            func(ref this, pTarget);
        }

        // for gattlings
        public unsafe void SetCurrentWeaponStage(int weaponIndex)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, int, void>)0x70DDD0;
            func(ref this, weaponIndex);
        }

        // returns the house that created this object (factoring in Mind Control)
        public unsafe Pointer<HouseClass> GetOriginalOwner()
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr>)0x70F820;
            return func(ref this);
        }

        public unsafe void FireDeathWeapon(int additionalDamage)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, int, void>)0x70D690;
            func(ref this, additionalDamage);
        }

        public unsafe bool HasAbility(Ability ability)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, Ability, Bool>)0x70D0D0;
            return func(ref this, ability);
        }

        public unsafe Pointer<LaserDrawClass> CreateLaser(Pointer<AbstractClass> pTarget, int weaponIndex, Pointer<WeaponTypeClass> pWeapon, CoordStruct sourceCoord)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr, int, IntPtr, ref CoordStruct, IntPtr>)0x6FD210;
            return func(ref this, pTarget, weaponIndex, pWeapon, ref sourceCoord);
        }

        public unsafe Pointer<EBolt> Electric_Zap(Pointer<AbstractClass> pTarget, Pointer<WeaponTypeClass> pWeapon, CoordStruct sourceCoord)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TechnoClass, IntPtr, IntPtr, ref CoordStruct, IntPtr>)0x6FD460;
            return func(ref this, pTarget, pWeapon, ref sourceCoord);
        }

        [FieldOffset(0)] public ObjectClass Base;

        [FieldOffset(276)] public PassengersClass Passengers;

        [FieldOffset(284)] public IntPtr transporter; // unit carrying me
        public Pointer<TechnoClass> Transporter { get => transporter; set => transporter = value; }

        [FieldOffset(292)] public int CurrentTurretNumber; // for IFV/gattling/charge turrets

        [FieldOffset(308)] public Bool InAir;

        [FieldOffset(312)] public int CurrentWeaponNumber; // for IFV/gattling

        [FieldOffset(316)] public Rank CurrentRanking; // only used for promotion detection

        [FieldOffset(320)] public int CurrentGattlingStage;

        [FieldOffset(324)] public int GattlingValue; // sum of RateUps and RateDowns

        [FieldOffset(336)] public VeterancyStruct Veterancy;

        [FieldOffset(344)] public double ArmorMultiplier;

        [FieldOffset(352)] public double FirepowerMultiplier;

        [FieldOffset(360)] public TimerStruct IdleActionTimer;

        [FieldOffset(372)] public TimerStruct RadarFlashTimer;

        [FieldOffset(384)] public TimerStruct TargetingTimer; //Duration = 45 on init!

        [FieldOffset(396)] public TimerStruct IronCurtainTimer;

        [FieldOffset(408)] public TimerStruct IronTintTimer; // how often to alternate the effect color

        [FieldOffset(420)] public int IronTintStage;

        [FieldOffset(424)] public TimerStruct AirstrikeTimer;

        [FieldOffset(436)] public TimerStruct AirstrikeTintTimer; // tracks alternation of the effect color

        [FieldOffset(448)] public int AirstrikeTintStage;

        [FieldOffset(452)] public int ForceShielded; //0 or 1, NOT a bool - is this under ForceShield as opposed to IC?

        [FieldOffset(456)] public Bool Deactivated; //Robot Tanks without power for instance

        [FieldOffset(480)] public TimerStruct InfantryBlinkTimer; // Rules->InfantryBlinkDisguiseTime , detects mirage firing per description

        [FieldOffset(492)] public TimerStruct DisguiseBlinkTimer; // disguise disruption timer

        [FieldOffset(508)] public TimerStruct ReloadTimer;

        [FieldOffset(532)] public int Group;

        [FieldOffset(536)] public IntPtr focus;
        public Pointer<AbstractClass> Focus { get => focus; set => focus = value; }

        [FieldOffset(540)] private IntPtr owner;
        public Pointer<HouseClass> Owner { get => owner; set => owner = value; }

        [FieldOffset(544)] public CloakStates CloakStates;

        [FieldOffset(548)] public ProgressTimer CloakProgress; // phase from [opaque] -> [fading] -> [transparent] , [General]CloakingStages= long

        [FieldOffset(576)] public TimerStruct CloakDelayTimer; // delay before cloaking again

        [FieldOffset(588)] public float WarpFactor; // don't ask! set to 0 in CTOR, never modified, only used as ((this->Fetch_ID) + this->WarpFactor) % 400 for something in cloak ripple

        [FieldOffset(624)] public Bool BeingWarpedOut; // is being warped by CLEG

        [FieldOffset(625)] public Bool WarpingOut; // phasing in after chrono-jump

        [FieldOffset(628)] public IntPtr temporalImUsing; // CLEG attacking Power Plant : CLEG's this
        public Pointer<TemporalClass> TemporalImUsing { get => temporalImUsing; set => temporalImUsing = value; }

        [FieldOffset(632)] public IntPtr temporalTargetingMe; // CLEG attacking Power Plant : PowerPlant's this
        public Pointer<TemporalClass> TemporalTargetingMe { get => temporalTargetingMe; set => temporalTargetingMe = value; }

        [FieldOffset(636)] public Bool IsImmobilized; // by chrono aftereffects

        [FieldOffset(644)] public int ChronoLockRemaining; // countdown after chronosphere warps things around

        [FieldOffset(648)] public CoordStruct ChronoDestCoords; // teleport loco and chsphere set this

        [FieldOffset(664)] public Bool Berzerk;

        [FieldOffset(680)] public IntPtr directRockerLinkedUnit;
        public Pointer<FootClass> DirectRockerLinkedUnit { get => directRockerLinkedUnit; set => directRockerLinkedUnit = value; }

        [FieldOffset(684)] public IntPtr locomotorTarget; // mag->LocoTarget = victim
        public Pointer<FootClass> LocomotorTarget { get => locomotorTarget; set => locomotorTarget = value; }

        [FieldOffset(688)] public IntPtr locomotorSource; // victim->LocoSource = mag
        public Pointer<FootClass> LocomotorSource { get => locomotorSource; set => locomotorSource = value; }

        [FieldOffset(692)] public Pointer<AbstractClass> Target;

        [FieldOffset(696)] public Pointer<AbstractClass> LastTarget;

        [FieldOffset(720)] public IntPtr spawnManager;
        public Pointer<SpawnManagerClass> SpawnManager { get => spawnManager; set => spawnManager = value; }

        [FieldOffset(724)] public IntPtr spawnOwner;
        public Pointer<TechnoClass> SpawnOwner { get => spawnOwner; set => spawnOwner = value; }

        [FieldOffset(728)] public IntPtr slaveManager;
        public Pointer<SlaveManagerClass> SlaveManager { get => slaveManager; set => slaveManager = value; }


        [FieldOffset(732)] public IntPtr slaveOwner;
        public Pointer<TechnoClass> SlaveOwner { get => slaveOwner; set => slaveOwner = value; }


        [FieldOffset(744)] public float PitchAngle; // not exactly, and it doesn't affect the drawing, only internal state of a dropship

        [FieldOffset(762)] public int Ammo;

        // rocking effect
        [FieldOffset(808)] public float AngleRotatedSideways; // in this frame, in radians - if abs() exceeds pi/2, it dies

        [FieldOffset(812)] public float AngleRotatedForwards; // same

        // set these and leave the previous two alone!
        // if these are set, the unit will roll up to pi/4, by this step each frame, and balance back
        [FieldOffset(816)] public float RockingSidewaysPerFrame; // left to right - positive pushes left side up

        [FieldOffset(820)] public float RockingForwardsPerFrame; // back to front - positive pushes ass up

        [FieldOffset(880)] public FacingStruct BarrelFacing;

        [FieldOffset(904)] public FacingStruct Facing;

        [FieldOffset(928)] public FacingStruct TurretFacing;

        [FieldOffset(952)] public int CurrentBurstIndex;

        [FieldOffset(956)] public TimerStruct TargetLaserTimer;

        [FieldOffset(970)] public short Shipsink_3CA;

        [FieldOffset(973)] public Bool IsSinking;

        [FieldOffset(974)] public Bool WasSinkingAlready; // if(IsSinking && !WasSinkingAlready) { play SinkingSound; WasSinkingAlready = 1; }

        [FieldOffset(977)] public Bool HasBeenAttacked; // ReceiveDamage when not HouseClass_IsAlly

        [FieldOffset(978)] public Bool Cloakable;

        [FieldOffset(979)] public Bool IsPrimaryFactory; // doubleclicking a warfac/barracks sets it as primary

        [FieldOffset(980)] public Bool Spawned;

        [FieldOffset(1060)] public Bool IsOnCarryall;

        [FieldOffset(1061)] public Bool IsCrashing;

        [FieldOffset(1062)] public Bool WasCrashingAlready;

        [FieldOffset(1063)] public Bool IsBeingManipulated;

        [FieldOffset(1064)] public IntPtr beingManipulatedBy;
        public Pointer<TechnoClass> BeingManipulatedBy { get => beingManipulatedBy; set => beingManipulatedBy = value; }

        [FieldOffset(1068)] public IntPtr chronoWarpedByHouse;
        public Pointer<HouseClass> ChronoWarpedByHouse { get => chronoWarpedByHouse; set => chronoWarpedByHouse = value; }

        [FieldOffset(1073)] public Bool IsMouseHovering;

        // [FieldOffset(1112)] public DynamicVectorClass<Pointer<AbstractClass>> CurrentTargets;
        // if DistributedFire=yes, this is used to determine which possible targets should be ignored in the latest threat scan
        // [FieldOffset(1136)] public DynamicVectorClass<Pointer<AbstractClass>> AttackedTargets;

        [FieldOffset(1184)] public Bool TurretFacingChanging;

        [FieldOffset(1284)] public int EMPLockRemaining;

        [FieldOffset(1288)] public int ThreatPosed;

        [FieldOffset(1292)] public int ShouldLoseTargetNow;

        [FieldOffset(1304)] public IntPtr disguise;
        public Pointer<ObjectTypeClass> Disguise { get => disguise; set => disguise = value; }

        [FieldOffset(1308)] public IntPtr disguisedAsHouse;
        public Pointer<HouseClass> DisguisedAsHouse { get => disguisedAsHouse; set => disguisedAsHouse = value; }


        public FacingStruct GetTurretFacing()
        {
            FacingStruct facing = new FacingStruct();
            GetTurretFacing(ref facing);
            return facing;
        }

        public FacingStruct GetRealFacing()
        {
            FacingStruct facing = new FacingStruct();
            GetRealFacing(ref facing);
            return facing;
        }

        public unsafe void DrawSHPImage(Pointer<SHPStruct> SHP, int frameIdx, Point2D pos, RectangleStruct bound,
            int a7, int a8, int ZAdjust, uint arg9, int a11, int extraGlow, int tintAdded,
            Pointer<SHPStruct> BUILDINGZ_SHA, uint argD, uint ZS_X, uint ZS_Y, int a18)
        {
            var func = (delegate* unmanaged[Thiscall]<int, ref TechnoClass,
                IntPtr, int, IntPtr, IntPtr,
                int, int, int, uint, int, int, int,
                IntPtr, uint, uint, uint, int, void>)ASM.FastCallTransferStation;
            func(0x705E00, ref this,
                SHP, frameIdx, Pointer<Point2D>.AsPointer(ref pos), Pointer<RectangleStruct>.AsPointer(ref bound),
                a7, a8, ZAdjust, arg9, a11, extraGlow, tintAdded,
                BUILDINGZ_SHA, argD, ZS_X, ZS_Y, a18);
        }

        public unsafe void DrawSHPImage(Pointer<SHPStruct> SHP, int frameIdx, Point2D pos, RectangleStruct bound)
        {
            DrawSHPImage(SHP, frameIdx, pos, bound, 0, 0, 0, 0, 0, 1000, 0, new IntPtr(0x0), 0, 0, 0, 0);
        }

    }



    [StructLayout(LayoutKind.Explicit, Size = 4)]
    [Serializable]
    public struct VeterancyStruct
    {
        [FieldOffset(0)] public float Veterancy;

        public void Add(int ownerCost, int victimCost)
        {
            Add((double)(victimCost / (ownerCost * RulesClass.Global().VeteranRatio)));
        }

        public void Add(double value)
        {
            double val = this.Veterancy + value;
            if (val > RulesClass.Global().VeteranCap)
            {
                val = RulesClass.Global().VeteranCap;
            }
            this.Veterancy = (float)val;
        }

        public bool IsElite()
        {
            return Veterancy >= 2.0f;
        }

        public bool IsVeteran()
        {
            return Veterancy >= 1.0f && Veterancy < 2.0f;
        }

        public bool IsRookie()
        {
            return Veterancy >= 0.0f && Veterancy < 1.0f;
        }

        public bool IsNegative()
        {
            return Veterancy < 0.0f;
        }

        public Rank GetRemainingLevel()
        {
            if (IsElite())
                return Rank.Elite;
            if (IsVeteran())
                return Rank.Veteran;
            return Rank.Rookie;
        }

        public void Reset()
        {
            Veterancy = 0.0f;
        }

        public void SetRookie(bool notReally = true)
        {
            Veterancy = notReally ? -0.25f : 0.0f;
        }

        public void SetVeteran(bool notReally = true)
        {
            Veterancy = notReally ? 1.0f : 0.0f;
        }

        public void SetElite(bool notReally = true)
        {
            Veterancy = notReally ? 2.0f : 0.0f;
        }
    }
}
