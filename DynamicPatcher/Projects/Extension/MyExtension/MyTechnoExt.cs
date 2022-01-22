using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;


namespace Extension.Ext
{


    [Serializable]
    public enum DrivingState
    {
        Moving = 0, Stand = 1, Start = 2, Stop = 3
    }


    public partial class TechnoExt
    {
        // public const byte YES = 1;
        // public const byte NO = 0;

        public string MyExtensionTest = nameof(MyExtensionTest);

        public CoordStruct LastLocation;
        public bool IsDead;

        public Mission LastMission;
        public DrivingState DrivingState;

        public void OnInit()
        {
            TechnoClass_Init_AircraftDive();
            TechnoClass_Init_AttachEffect();
            TechnoClass_Init_AttackBeacon();
            TechnoClass_Init_AntiBullet();
            TechnoClass_Init_AutoFireAreaWeapon();
            TechnoClass_Init_ConvertType();
            TechnoClass_Init_DecoyMissile();
            TechnoClass_Init_GiftBox();
            TechnoClass_Init_OverrideWeapon();
            TechnoClass_Init_Trail();
            TechnoClass_Init_VirtualUnit();
        }

        public unsafe void OnUpdate()
        {

            TechnoClass_Update_DestroySelf();
            // 检查死亡
            if (!IsDead)
            {
                IsDead = OwnerObject.Ref.Base.Health <= 0 || !OwnerObject.Ref.Base.IsAlive || OwnerObject.Ref.IsCrashing || OwnerObject.Ref.IsSinking;
                // Logger.Log("{0} {1} update事件检测到死亡.", OwnerObject, OwnerObject.Ref.Type.Ref.Base.Base.ID);
            }
            if (!IsDead)
            {
                // 死亡不会执行的，坠落，下沉等
                // Logger.Log("Techno {0} OnUpdate Calling.", OwnerObject.Ref.Type.Ref.Base.Base.ID);
                // 检查启停状态
                Mission mission = OwnerObject.Convert<MissionClass>().Ref.CurrentMission;
                switch (mission)
                {
                    case Mission.Move:
                    case Mission.AttackMove:
                        // 上一次任务不是这两个说明是起步
                        if (Mission.Move != LastMission && Mission.AttackMove != LastMission)
                        {
                            DrivingState = DrivingState.Start;
                        }
                        else
                        {
                            DrivingState = DrivingState.Moving;
                        }
                        break;
                    default:
                        // 上一次任务如果是Move或者AttackMove说明是刹车
                        if (Mission.Move == LastMission || Mission.AttackMove == LastMission)
                        {
                            DrivingState = DrivingState.Stop;
                        }
                        else
                        {
                            DrivingState = DrivingState.Stand;
                        }
                        break;
                }
                LastMission = mission;

                TechnoClass_Update_AircraftDive();
                TechnoClass_Update_AircraftPut();
                TechnoClass_Update_AntiBullet();
                TechnoClass_Update_AttackBeacon();
                TechnoClass_Update_AutoFireAreaWeapon();
                TechnoClass_Update_ChaosAnim();
                TechnoClass_Update_ConvertType();
                TechnoClass_Update_CrawlingFLH();
                TechnoClass_Update_DecoyMissile();
                TechnoClass_Update_FixGattlingStage();
                TechnoClass_Update_GiftBox();
                TechnoClass_Update_JumpjetFacingToTarget();
                TechnoClass_Update_Passengers();
                TechnoClass_Update_SpawnFireOnce();
                TechnoClass_Update_SpawnSupport();

                TechnoClass_Update_CustomWeapon();
            }
            // 死亡还会执行的，坠落，沉没等
            TechnoClass_Update_AttachEffect();
            TechnoClass_Update_Trail();

            LastLocation = OwnerObject.Ref.Base.Base.GetCoords();
        }

        public unsafe void OnUnInit()
        {
            // Logger.Log("{0} 注销.", OwnerObject);
            TechnoClass_UnInit_AttachEffect();
        }

        public unsafe void OnTemporalUpdate(Pointer<TemporalClass> pTemporal)
        {
            // TemporalClass_UpdateA_Stand(pTemporal);
        }

        public unsafe void OnPut(Pointer<CoordStruct> pCoord, Direction faceDir)
        {
            TechnoClass_Put_AircraftPut(pCoord, faceDir);
            TechnoClass_Put_AttachEffect(pCoord, faceDir);
            TechnoClass_Put_DestroySelf(pCoord, faceDir);
        }

        public unsafe void OnRemove()
        {
            // Logger.Log("{0} UnLimbo", OwnerObject.Ref.Type.Ref.Base.Base.ID);
            TechnoClass_Remove_AttachEffect();
        }

        public unsafe void OnReceiveDamage(Pointer<int> pDamage, int distanceFromEpicenter, Pointer<WarheadTypeClass> pWH,
            Pointer<ObjectClass> pAttacker, bool ignoreDefenses, bool preventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        {
            // 检查死亡
            if (OwnerObject.Ref.Base.Health <= 0 || !OwnerObject.Ref.Base.IsAlive)
            {
                // Logger.Log("{0} {1} ReceiveDamage事件检测到死亡.", OwnerObject, OwnerObject.Ref.Type.Ref.Base.Base.ID);
                return;
            }
            // Logger.Log("{0} Take Damage {1}", OwnerObject.Ref.Type.Ref.Base.Base.ID, pDamage.Data);
            if (AffectMe(pAttacker, pWH, pAttackingHouse, out WarheadTypeExt whExt) && DamageMe(pDamage.Data, distanceFromEpicenter, whExt, out int realDamage))
            {
                TechnoClass_ReceiveDamage_AttachEffect(pDamage, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse, whExt, realDamage);
                // TechnoClass_ReceiveDamage_TauntWarhead(pDamage, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse, whExt, realDamage);
            }

        }

        public unsafe bool AffectMe(Pointer<ObjectClass> pAttacker, Pointer<WarheadTypeClass> pWH, Pointer<HouseClass> pHouse, out WarheadTypeExt warheadTypeExt)
        {
            warheadTypeExt = WarheadTypeExt.ExtMap.Find(pWH);
            return AffectMe(pAttacker, pWH, pHouse, warheadTypeExt);
        }

        public unsafe bool AffectMe(Pointer<ObjectClass> pAttacker, Pointer<WarheadTypeClass> pWH, Pointer<HouseClass> pTargetHouse, WarheadTypeExt warheadTypeExt)
        {
            if (!pAttacker.IsNull && null != warheadTypeExt)
            {
                Pointer<HouseClass> pOwnerHouse = OwnerObject.Ref.Owner;
                return warheadTypeExt.CanAffectHouse(pOwnerHouse, pTargetHouse);
            }
            return true;
        }


        public unsafe bool DamageMe(int damage, int distanceFromEpicenter, WarheadTypeExt warheadTypeExt, out int realDamage, bool effectsRequireDamage = false)
        {
            // 计算实际伤害
            if (damage > 0)
            {
                realDamage = MapClass.GetTotalDamage(damage, warheadTypeExt.OwnerObject, OwnerObject.Ref.Base.Type.Ref.Armor, distanceFromEpicenter);
            }
            else
            {
                realDamage = -MapClass.GetTotalDamage(-damage, warheadTypeExt.OwnerObject, OwnerObject.Ref.Base.Type.Ref.Armor, distanceFromEpicenter);
            }
            if (null != warheadTypeExt)
            {
                if (damage == 0)
                {
                    return warheadTypeExt.Ares.AllowZeroDamage;
                }
                else
                {
                    if (warheadTypeExt.Ares.EffectsRequireVerses)
                    {
                        // 必须要可以造成伤害
                        if (MapClass.GetTotalDamage(RulesClass.Global().MaxDamage, warheadTypeExt.OwnerObject, OwnerObject.Ref.Base.Type.Ref.Armor, 0) == 0)
                        {
                            // 弹头无法对该类型护甲造成伤害
                            return false;
                        }
                        // 伤害非零，当EffectsRequireDamage=yes时，必须至少造成1点实际伤害
                        if (effectsRequireDamage || warheadTypeExt.Ares.EffectsRequireDamage)
                        {
                            // Logger.Log("{0} 收到伤害 {1}, 弹头 {2}, 爆心距离{3}, 实际伤害{4}", OwnerObject.Ref.Type.Ref.Base.Base.ID, damage, warheadTypeExt.OwnerObject.Ref.Base.ID, distanceFromEpicenter, realDamage);
                            return realDamage != 0;
                        }
                    }
                }
            }
            return true;
        }

        public unsafe void OnDestroy()
        {
            // Logger.Log("{0} {1} 收到足够的伤害死亡.", OwnerObject, OwnerObject.Ref.Type.Ref.Base.Base.ID);

            TechnoClass_Destroy_AttachEffect();
            TechnoClass_Destroy_ConvertType();
            TechnoClass_Destroy_DestroyAnims();
        }


        public unsafe void CanFire(Pointer<AbstractClass> pTarget, Pointer<WeaponTypeClass> pWeapon, ref bool ceaseFire)
        {
            if (ceaseFire = TechnoClass_CanFire_AttackBeacon(pTarget, pWeapon))
            {
                return;
            }
            if (ceaseFire = TechnoClass_CanFire_Passengers(pTarget, pWeapon))
            {
                return;
            }

        }

        public unsafe void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            TechnoClass_OnFire_AircraftDive(pTarget, weaponIndex);
            TechnoClass_OnFire_AttackBeacon(pTarget, weaponIndex);
            TechnoClass_OnFire_ExtraFireWeapon(pTarget, weaponIndex);
            TechnoClass_OnFire_OverrideWeapon(pTarget, weaponIndex);
            TechnoClass_OnFire_RockerPitch(pTarget, weaponIndex);
            TechnoClass_OnFire_SpawnSupport(pTarget, weaponIndex);
            TechnoClass_OnFire_SuperWeapon(pTarget, weaponIndex);
        }

        public unsafe void OnRegisterDestruction(Pointer<TechnoClass> pKiller, int cost, ref bool skip)
        {
            if (skip = TechnoClass_RegisterDestruction_OverrideWeapon(pKiller, cost))
            {
                return;
            }
            if (skip = TechnoClass_RegisterDestruction_StandUnit(pKiller, cost))
            {
                return;
            }
        }

        public unsafe void OnSelect(ref bool selectable)
        {
            if (!(selectable = TechnoClass_Select_VirtualUnit()))
            {
                return;
            }
        }

        public unsafe void OnGuardCommand()
        {
            // TechnoClass_GuardCommand_AttachEffect();
        }

        public unsafe void OnStopCommand()
        {
            TechnoClass_StopCommand_AttachEffect();
        }

        public unsafe void DrawHealthBar_Building(int length, Pointer<Point2D> pLocation, Pointer<RectangleStruct> pBound)
        {

        }

        public unsafe void DrawHealthBar_Other(int length, Pointer<Point2D> pLocation, Pointer<RectangleStruct> pBound)
        {
            // Logger.Log("Point2D = {0}, RectangleStruct = {1}", pLocation.Ref, pBound.Ref);
            // Point2D pos = new Point2D(0, 30);
            // pos += pLocation.Ref;
            // RectangleStruct bound = new RectangleStruct(0, 0, 1112, 688);
            // DSurface.Temp.Ref.DrawSHP(FileSystem.MOUSE_PAL, FileSystem.POWEROFF_SHP, 1, pos, bound, (BlitterFlags)0xE00);
        }

        public unsafe void DrawSHP_Colour(REGISTERS* R)
        {
            TechnoClass_DrawSHP_Paintball(R);
            TechnoClass_DrawSHP_Colour(R);
        }

        public unsafe void DrawVXL_Colour(REGISTERS* R)
        {
            TechnoClass_DrawVXL_Paintball(R);
        }

        public unsafe void OnDestroy_UnitClass()
        {

        }

        // public unsafe void CanEnterCell_UnitClass(Pointer<TechnoClass> pOccuiper, ref bool ignoreOccupier)
        // {
        //     ignoreOccupier = TechnoClass_CanEnterCell_Stand(pOccuiper);
        // }

        // public unsafe void CanEnterCell_InfantryClass(Pointer<TechnoClass> pOccuiper, ref bool ignoreOccupier)
        // {
        //     ignoreOccupier = TechnoClass_CanEnterCell_Stand(pOccuiper);
        // }

    }

    public partial class TechnoTypeExt
    {

        [INILoadAction]
        public void LoadINI(Pointer<CCINIClass> pINI)
        {
            INIReader reader = new INIReader(pINI);
            string section = OwnerObject.Ref.Base.Base.ID;

            ReadAresFlags(reader, section);

            ReadAircraftDive(reader, section);
            ReadAircraftPut(reader, section);
            ReadAntiBullet(reader, section);
            ReadAttachEffect(reader, section);
            ReadAttackBeacon(reader, section);
            ReadAutoFireAreaWeapon(reader, section);
            ReadCrawlingFLH(reader, section);
            ReadDecoyMissile(reader, section);
            ReadDestroyAnims(reader, section);
            ReadDestroySelf(reader, section);
            ReadExtraFireWeapon(reader, section);
            ReadFireSuperWeapon(reader, section);
            ReadGiftBox(reader, section);
            ReadJumpjetFacingToTarget(reader, section);
            ReadOverrideWeapon(reader, section);
            ReadPassengers(reader, section);
            ReadSpawnFireOnce(reader, section);
            ReadSpawnSupport(reader, section);
            ReadTrail(reader, section);
            ReadVirtualUnit(reader, section);
        }

        [LoadAction]
        public void Load(IStream stream) { }

        [SaveAction]
        public void Save(IStream stream) { }
    }
}
