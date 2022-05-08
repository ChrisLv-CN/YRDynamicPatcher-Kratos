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

        public bool IsDead;
        public DrivingState DrivingState;

        public bool IsBuilding;

        private Mission lastMission;
        private CoordStruct lastLocation;
        
        private event System.Action OnUnInitAction;

		private event System.Action OnRenderAction;
		private event System.Action OnRender2Action;

        private event System.Action OnUpdateAction;
        private event System.Action OnTemporalUpdateAction;
        private event System.Action<Pointer<CoordStruct>, short> OnPutAction;
        private event System.Action OnRemoveAction;

        private event System.Action<Pointer<int>, int, Pointer<WarheadTypeClass>, Pointer<ObjectClass>, bool, bool, Pointer<HouseClass>> OnReceiveDamageAction;
        private event System.Action<Pointer<int>, Pointer<WarheadTypeClass>, DamageState> OnReceiveDamage2Action;
        private event System.Action OnDestroyAction;
        private event System.Action OnDestroyAnimAction;

        private event System.Action<Pointer<AbstractClass>, int> OnFireAction;
        private event System.Action OnFireOnceAction;

        public void ChangeType()
        {
        }

        public void OnInit()
        {
            IsBuilding = OwnerObject.Ref.Base.Base.WhatAmI() == AbstractType.Building;

            TechnoClass_Init_AircraftDive();
            TechnoClass_Init_AircraftPut();
            TechnoClass_Init_AttachEffect();
            TechnoClass_Init_AttackBeacon();
            TechnoClass_Init_AntiBullet();
            TechnoClass_Init_AutoFireAreaWeapon();
            TechnoClass_Init_ChaosAnim();
            TechnoClass_Init_ConvertType();
            TechnoClass_Init_DecoyMissile();
            TechnoClass_Init_DeployToTransform();
            TechnoClass_Init_Deselect();
            TechnoClass_Init_ExtraFireWeapon();
            TechnoClass_Init_Fighter_Area_Guard();
            TechnoClass_Init_FixGattlingStage();
            TechnoClass_Init_HealthBarText();
            TechnoClass_Init_JumpjetFacingToTarget();
            TechnoClass_Init_OverrideWeapon();
            TechnoClass_Init_SuperWeapon();
            TechnoClass_Init_SpawnMissileHoming();
            TechnoClass_Init_SpawnSupport();
            TechnoClass_Init_Trail();
            TechnoClass_Init_UnitDeployFireOnce();

            TechnoClass_Init_GiftBox();
            TechnoClass_Init_VirtualUnit();
        }

        public unsafe void OnRender()
        {
            // if (!MyMaster.IsNull)
            // {
            //     Logger.Log($"{Game.CurrentFrame} - 渲染替身 {OwnerObject} [{Type.OwnerObject.Ref.Base.Base.ID}] 开始，位置 {OwnerObject.Ref.Base.Base.GetCoords()}");
            // }

            OnRenderAction?.Invoke();

            TechnoClass_Render_AttachEffect(); // 调整替身位置到和本体一样

            // TechnoClass_Render_ChaosAnim();
            // TechnoClass_Render_Trail();
        }

        public unsafe void OnRender2()
        {
            OnRender2Action?.Invoke();

            TechnoClass_Render2_AttachEffect(); // 再次调整替身的位置
        }

        public unsafe void OnUpdate()
        {

            TechnoClass_Update_DestroySelf();
            // 检查死亡
            if (!IsDead)
            {
                IsDead = OwnerObject.IsDead();
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
                        if (Mission.Move != lastMission && Mission.AttackMove != lastMission)
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
                        if (Mission.Move == lastMission || Mission.AttackMove == lastMission)
                        {
                            DrivingState = DrivingState.Stop;
                        }
                        else
                        {
                            DrivingState = DrivingState.Stand;
                        }
                        break;
                }
                lastMission = mission;

                OnUpdateAction?.Invoke();

                // TechnoClass_Update_AircraftDive();
                // TechnoClass_Update_AircraftPut();
                // TechnoClass_Update_AntiBullet();
                // TechnoClass_Update_AttackBeacon();
                // TechnoClass_Update_AutoFireAreaWeapon();
                TechnoClass_Update_ConvertType();
                // TechnoClass_Update_CrawlingFLH();
                // TechnoClass_Update_DecoyMissile();
                TechnoClass_Update_Deselect();
                // TechnoClass_Update_DeployToTransform();
                // TechnoClass_Update_FixGattlingStage();
                TechnoClass_Update_GiftBox();
                // TechnoClass_Update_JumpjetFacingToTarget();
                TechnoClass_Update_Paintball();
                TechnoClass_Update_Passengers();
                TechnoClass_Update_SpawnFireOnce();
                // TechnoClass_Update_SpawnMissileHoming();
                // TechnoClass_Update_SpawnSupport();
                // TechnoClass_Update_Trail();
                TechnoClass_Update_CustomWeapon();
                // TechnoClass_Update_Fighter_Area_Guard();
            }
            // 死亡还会执行的，坠落，沉没等
            TechnoClass_Update_DamageText();
            TechnoClass_Update_AttachEffect();

            lastLocation = OwnerObject.Ref.Base.Base.GetCoords();
        }

        public unsafe void OnTemporalUpdate(Pointer<TemporalClass> pTemporal)
        {
            OnTemporalUpdateAction?.Invoke();

            TemporalClass_UpdateA_AttachEffect(pTemporal);
        }

        public unsafe void OnPut(Pointer<CoordStruct> pCoord, short faceDirValue8)
        {
            OnPutAction?.Invoke(pCoord, faceDirValue8);

            // TechnoClass_Put_AircraftPut(pCoord, faceDirValue8);
            TechnoClass_Put_AttachEffect(pCoord, faceDirValue8);
            TechnoClass_Put_DestroySelf(pCoord, faceDirValue8);
            // TechnoClass_Put_SpawnMissileHoming(pCoord, faceDirValue8);
            // TechnoClass_Put_Trail(pCoord, faceDirValue8);
        }

        public unsafe void OnRemove()
        {
            OnRemoveAction?.Invoke();

            // Logger.Log("{0} UnLimbo", OwnerObject.Ref.Type.Ref.Base.Base.ID);
            TechnoClass_Remove_AttachEffect();
            // TechnoClass_Remove_Trail(IsDead);
        }

        public unsafe void OnReceiveDamage(Pointer<int> pDamage, int distanceFromEpicenter, Pointer<WarheadTypeClass> pWH,
            Pointer<ObjectClass> pAttacker, bool ignoreDefenses, bool preventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        {
            // 检查死亡
            if (IsDead || OwnerObject.IsDead())
            {
                IsDead = true;
                // Logger.Log("{0} {1} ReceiveDamage事件检测到死亡.", OwnerObject, OwnerObject.Ref.Type.Ref.Base.Base.ID);
                return;
            }
            // Logger.Log("{0} Take Damage {1}", OwnerObject.Ref.Type.Ref.Base.Base.ID, pDamage.Data);
            if (AffectMe(pAttacker, pWH, pAttackingHouse, out WarheadTypeExt whExt) && DamageMe(pDamage.Data, distanceFromEpicenter, whExt, out int realDamage))
            {
                TechnoClass_ReceiveDamage_AttachEffect(pDamage, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse, whExt, realDamage);
                // TechnoClass_ReceiveDamage_TauntWarhead(pDamage, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse, whExt, realDamage);
            }

            TechnoClass_ReceiveDamage_Stand(pDamage, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse);
        }

        public unsafe void OnReceiveDamage2(Pointer<int> pRealDamage, Pointer<WarheadTypeClass> pWH, DamageState damageState)
        {

            OnReceiveDamage2Action?.Invoke(pRealDamage, pWH, damageState);

            TechnoClass_ReceiveDamage2_DamageText(pRealDamage, pWH, damageState);
            TechnoClass_ReceiveDamage2_GiftBox(pRealDamage, pWH, damageState);
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
            IsDead = true;

            OnDestroyAction?.Invoke();

            TechnoClass_Destroy_AttachEffect();
            TechnoClass_Destroy_ConvertType();
            TechnoClass_Destroy_GiftBox();
        }

        public unsafe void DestroyAnim()
        {
            TechnoClass_Destroy_DestroyAnims();
        }

        public unsafe void OnUnInit()
        {
            IsDead = true;
            // Logger.Log("{0} 注销.", OwnerObject);
            TechnoClass_UnInit_AttachEffect();
        }

        public unsafe void CanFire(Pointer<AbstractClass> pTarget, Pointer<WeaponTypeClass> pWeapon, ref bool ceaseFire)
        {
            if (ceaseFire = TechnoClass_CanFire_Disable(pTarget, pWeapon))
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

            OnFireAction?.Invoke(pTarget, weaponIndex);

            // TechnoClass_OnFire_AircraftDive(pTarget, weaponIndex);
            // TechnoClass_OnFire_AttackBeacon_Recruit(pTarget, weaponIndex);
            // TechnoClass_OnFire_ExtraFireWeapon(pTarget, weaponIndex);
            TechnoClass_OnFire_RockerPitch(pTarget, weaponIndex);
            // TechnoClass_OnFire_SpawnSupport(pTarget, weaponIndex);
            TechnoClass_OnFire_SuperWeapon(pTarget, weaponIndex);
        }

        public unsafe void OnFireOnce()
        {
            OnFireOnceAction?.Invoke();
        }

        public unsafe void OnRegisterDestruction(Pointer<TechnoClass> pKiller, int cost, ref bool skip)
        {
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
            if (!(selectable = TechnoClass_Select_Deselect()))
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
            TechnoClass_DrawHealthBar_Building_Text(length, pLocation, pBound);

        }

        public unsafe void DrawHealthBar_Other(int length, Pointer<Point2D> pLocation, Pointer<RectangleStruct> pBound)
        {
            // Point2D pos = new Point2D(0, 30);
            // pos += pLocation.Ref;
            // RectangleStruct bound = new RectangleStruct(0, 0, 1112, 688);
            // DSurface.Temp.Ref.DrawSHP(FileSystem.MOUSE_PAL, FileSystem.POWEROFF_SHP, 1, pos, bound, (BlitterFlags)0xE00);
            TechnoClass_DrawHealthBar_Other_Text(length, pLocation, pBound);

        }

        public unsafe void DrawSHP_Colour(REGISTERS* R)
        {
            TechnoClass_DrawSHP_Paintball(R);
            TechnoClass_DrawSHP_Colour(R);
        }

        public unsafe void DrawVXL_Colour(REGISTERS* R, bool isBuilding)
        {
            TechnoClass_DrawVXL_Paintball(R, isBuilding);
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

    public partial class TechnoTypeExt : ITypeExtension
    {
        public SwizzleablePointer<SuperWeaponTypeClass> FireSuperWeapon = new SwizzleablePointer<SuperWeaponTypeClass>(IntPtr.Zero);

        public CoordStruct TurretOffset = new CoordStruct();

        [INILoadAction]
        public void LoadINI(Pointer<CCINIClass> pINI)
        {
            // rules reader
            INIReader reader = new INIReader(pINI);
            string section = OwnerObject.Ref.Base.Base.ID;

            // art reader
            INIReader artReader = reader;
            if (null != CCINIClass.INI_Art && !CCINIClass.INI_Art.IsNull)
            {
                artReader = new INIReader(CCINIClass.INI_Art);
            }
            string artSection = section;
            string image = default;
            if (reader.ReadNormal(section, "Image", ref image))
            {
                artSection = image;
            }

            reader.ReadSuperWeapon(section, nameof(FireSuperWeapon), ref FireSuperWeapon.Pointer);

            ReadAresFlags(reader, section);

            string turretOffsetStr = null;
            if (artReader.ReadNormal(artSection, "TurretOffset", ref turretOffsetStr))
            {
                // Logger.Log("类型{0}炮塔偏移{1}", artSection, turretOffsetStr);
                try
                {
                    CoordStruct offset = new CoordStruct();
                    if (!string.IsNullOrEmpty(turretOffsetStr))
                    {
                        turretOffsetStr = turretOffsetStr.Trim();
                        if (turretOffsetStr.IndexOf(",") > -1)
                        {
                            string[] pos = turretOffsetStr.Split(',');
                            if (null != pos && pos.Length > 0)
                            {
                                for (int i = 0; i < pos.Length; i++)
                                {
                                    int value = Convert.ToInt32(pos[i].Trim());
                                    switch (i)
                                    {
                                        case 0:
                                            offset.X = value;
                                            break;
                                        case 1:
                                            offset.Y = value;
                                            break;
                                        case 2:
                                            offset.Z = value;
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            offset.X = Convert.ToInt32(turretOffsetStr);
                        }
                    }
                    this.TurretOffset = offset;
                }
                catch (Exception e)
                {
                    Logger.LogError("Illegal value {0} of TurretOffset in Type {1}", turretOffsetStr, section);
                    Logger.PrintException(e);
                }
            }

            ReadAircraftDive(reader, section);
            ReadAircraftPut(reader, section);
            ReadAntiBullet(reader, section);
            ReadAttachEffect(reader, section);
            ReadAttackBeacon(reader, section);
            ReadAutoFireAreaWeapon(reader, section);
            ReadCrawlingFLH(reader, section, artReader, artSection);
            ReadDecoyMissile(reader, section);
            ReadDeployToTransform(reader, section);
            ReadDeselect(reader, section);
            ReadDestroyAnims(reader, section);
            ReadDestroySelf(reader, section);
            ReadExtraFireWeapon(reader, section, artReader, artSection);
            ReadFireSuperWeapon(reader, section);
            ReadGiftBox(reader, section);
            ReadHelthText(reader, section);
            ReadJumpjetFacingToTarget(reader, section);
            ReadOverrideWeapon(reader, section);
            ReadPassengers(reader, section);
            ReadSpawnFireOnce(reader, section);
            ReadSpawnMissileHoming(reader, section);
            ReadSpawnSupport(reader, section, artReader, artSection);
            ReadTrail(reader, section, artReader, artSection);
            ReadVirtualUnit(reader, section);
            ReadFighterGuardData(reader, section);
        }

        [LoadAction]
        public void Load(IStream stream) { }

        [SaveAction]
        public void Save(IStream stream) { }
    }
}
