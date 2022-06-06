using System.Diagnostics;
using DynamicPatcher;
using Extension.Utilities;
using Extension.Script;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    public partial class AttachEffect
    {
        public Stand Stand;

        private void InitStand()
        {
            if (null != Type.StandType)
            {
                this.Stand = Type.StandType.CreateObject(Type);
                RegisterAction(Stand);
            }
        }

    }

    [Serializable]
    public class Stand : Effect<StandType>
    {

        public SwizzleablePointer<TechnoClass> pStand;

        private bool isBuilding = false;
        private bool onStopCommand = false;
        private bool notBeHuman = false;

        public Stand()
        {
            this.pStand = new SwizzleablePointer<TechnoClass>(IntPtr.Zero);
        }

        public override bool IsAlive()
        {
            if (pStand.IsNull || pStand.Pointer.IsDead())
            {
                pStand.Pointer = IntPtr.Zero;
                return false;
            }
            return true;
        }

        // 激活
        public override void OnEnable(Pointer<ObjectClass> pObject, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker)
        {
            CreateAndPutStand(pObject, pHouse);
        }

        private void CreateAndPutStand(Pointer<ObjectClass> pObject, Pointer<HouseClass> pHouse)
        {
            CoordStruct location = pObject.Ref.Base.GetCoords();

            Pointer<TechnoTypeClass> pType = TechnoTypeClass.Find(Type.Type);
            if (!pType.IsNull)
            {
                // 创建替身
                pStand.Pointer = pType.Ref.Base.CreateObject(pHouse).Convert<TechnoClass>();
                if (!pStand.IsNull)
                {
                    // 同步部分扩展设置
                    TechnoExt ext = TechnoExt.ExtMap.Find(pStand);
                    if (null != ext)
                    {
                        if (this.Type.VirtualUnit)
                        {
                            ext.VirtualUnit = this.Type.VirtualUnit;
                        }

                        switch (pObject.Ref.Base.WhatAmI())
                        {
                            case AbstractType.Bullet:
                                Pointer<BulletClass> pBullet = pObject.Convert<BulletClass>();
                                // 附加在抛射体上的，取抛射体的所有者
                                ext.MyMaster.Pointer = pBullet.Ref.Owner;
                                break;
                            default:
                                Pointer<TechnoClass> pTechno = pObject.Convert<TechnoClass>();
                                ext.MyMaster.Pointer = pTechno;
                                // 同步AE状态机
                                if (!pTechno.Ref.Owner.IsNull && pHouse == pTechno.Ref.Owner)
                                {
                                    TechnoExt masterExt = TechnoExt.ExtMap.Find(pObject.Convert<TechnoClass>());
                                    // 染色
                                    ext.AttachEffectManager.PaintballState = masterExt.AttachEffectManager.PaintballState;
                                    // 箱子加成
                                    ext.CrateStatus = masterExt.CrateStatus;
                                }
                                break;
                        }
                        ext.StandType = Type;
                    }

                    // 初始化替身
                    pStand.Ref.Base.Mark(MarkType.UP); // 拔起，不在地图上
                    // pStand.Ref.Base.IsOnMap = false;
                    // pStand.Ref.Base.NeedsRedraw = true;
                    bool canGuard = pHouse.Ref.ControlledByHuman();
                    if (pStand.Ref.Base.Base.WhatAmI() == AbstractType.Building)
                    {
                        isBuilding = true;
                        canGuard = true;
                    }
                    else
                    {
                        // lock locomotor
                        pStand.Pointer.Convert<FootClass>().Ref.Locomotor.Lock();
                    }
                    // only computer units can hunt
                    Mission mission = canGuard ? Mission.Guard : Mission.Hunt;
                    pStand.Pointer.Convert<MissionClass>().Ref.QueueMission(mission, false);

                    if (!pObject.IsInvisible())
                    {
                        // Logger.Log($"{Game.CurrentFrame} - put stand on {location}");
                        // 在格子位置刷出替身单位
                        if (!TryPutStand(location))
                        {
                            // 刷不出来？
                            Disable(location);
                            return;
                        }
                    }

                    // 放置到指定位置
                    LocationMark locationMark = StandHelper.GetLocation(pObject, Type);
                    if (default != locationMark.Location)
                    {
                        SetLocation(locationMark.Location);
                        // 强扭朝向
                        ForceSetFacing(locationMark.Direction);
                    }
                    // Logger.Log($"{Game.CurrentFrame} - 创建替身 {pStand.Pointer}[{Type.Type}], JOJO {pObject}[{pObject.Ref.Type.Ref.Base.ID}]");
                }
            }

        }

        private bool TryPutStand(CoordStruct location)
        {
            if (MapClass.Instance.TryGetCellAt(location, out Pointer<CellClass> pCell))
            {
                var occFlags = pCell.Ref.OccupationFlags;
                // if (occFlags.HasFlag(OccupationFlags.Buildings))
                // {
                //     // Logger.Log("当前格子上有建筑");
                //     BulletEffectHelper.RedCell(pCell.Ref.GetCoordsWithBridge(), 128, 1, 450);
                //     BulletEffectHelper.RedLineZ(pCell.Ref.GetCoordsWithBridge(), 1024, 1, 450);
                //     SpeedType speedType = pStand.Ref.Type.Ref.SpeedType;
                //     MovementZone movementZone = pStand.Ref.Type.Ref.MovementZone;
                //     CellStruct cell = MapClass.Instance.Pathfinding_Find(ref pCell.Ref.MapCoords, speedType, movementZone, 1, 1, false);
                //     if (MapClass.Instance.TryGetCellAt(cell, out Pointer<CellClass> pNewCell))
                //     {
                //         pCell = pNewCell;
                //         occFlags = pCell.Ref.OccupationFlags;
                //         Pointer<ObjectClass> pObject = pCell.Ref.GetContent();
                //         // Logger.Log("找到一个满足条件的空格子, occFlags = {0}, pObject = {1}", occFlags, pObject.IsNull ? "null" : pObject.Ref.Base.WhatAmI());
                //     }
                // }
                pStand.Ref.Base.OnBridge = pCell.Ref.ContainsBridge();
                CoordStruct xyz = pCell.Ref.GetCoordsWithBridge();
                ++Game.IKnowWhatImDoing;
                pStand.Ref.Base.Put(xyz, 0);
                --Game.IKnowWhatImDoing;
                pCell.Ref.OccupationFlags = occFlags;
                return true;
            }
            // ++Game.IKnowWhatImDoing;
            // bool isPut = pStand.Ref.Base.Put(location, 0);
            // --Game.IKnowWhatImDoing;
            return false;
        }

        // 销毁
        public override void Disable(CoordStruct location)
        {
            // Logger.Log($"{Game.CurrentFrame} - {AEType.Name} 替身 {Type.Type} 销毁");
            if (pStand.IsNull)
            {
                return;
            }
            ExplodesOrDisappear(false);
        }

        private void ExplodesOrDisappear(bool remove)
        {
            // Logger.Log($"{Game.CurrentFrame} {AEType.Name} 替身 {Type.Type} 注销");
            bool explodes = Type.Explodes || notBeHuman;
            TechnoExt ext = TechnoExt.ExtMap.Find(pStand);
            if (null != ext)
            {
                // Logger.Log($"{Game.CurrentFrame} {AEType.Name} 替身 {pStand.Pointer}[{Type.Type}] Explodes = {explodes} 下DestroySelf订单");
                ext.DestroySelfState.DestroyNow(!explodes);
            }
            else
            {
                if (explodes)
                {
                    // Logger.Log($"{Game.CurrentFrame} {AEType.Name} 替身 {pStand.Pointer}[{Type.Type}] 自爆, 没有发现EXT");
                    pStand.Ref.Base.TakeDamage(pStand.Ref.Base.Health + 1, pStand.Ref.Type.Ref.Crewed);
                    if (remove)
                    {
                        pStand.Ref.Base.Remove();
                    }
                }
                else
                {
                    // Logger.Log($"{Game.CurrentFrame} {AEType.Name} 替身 {Type.Type} 移除, 没有发现EXT");
                    pStand.Ref.Base.Remove();
                    // pStand.Ref.Base.UnInit(); // 替身攻击建筑时死亡会导致崩溃，莫名其妙的bug
                    pStand.Ref.Base.TakeDamage(pStand.Ref.Base.Health + 1, false);
                }
            }
            pStand.Pointer = IntPtr.Zero;
        }

        public override void OnRender2(Pointer<ObjectClass> pObject, CoordStruct location)
        {
            if (!isBuilding && pObject.CastToFoot(out Pointer<FootClass> pMaster))
            {
                // synch Tilt
                if (!Type.IsTrain)
                {
                    // rocker Squid capture ship
                    // pStand.Ref.AngleRotatedForwards = pMaster.Ref.Base.AngleRotatedForwards;
                    // pStand.Ref.AngleRotatedSideways = pMaster.Ref.Base.AngleRotatedSideways;

                    if (Type.SameTilter)
                    {
                        float forwards = pMaster.Ref.Base.AngleRotatedForwards;
                        float sideways = pMaster.Ref.Base.AngleRotatedSideways;
                        float t = 0f;
                        // Logger.Log($"{Game.CurrentFrame} 替身 朝向 {Type.Direction}, forwards = {forwards}, sideways = {sideways}");
                        // 计算方向
                        switch (Type.Direction)
                        {
                            case 0: // 正前 N
                                break;
                            case 2: // 前右 NE
                                break;
                            case 4: // 正右 E
                                t = forwards;
                                forwards = -sideways;
                                sideways = t;
                                break;
                            case 6: // 右后 SE
                                break;
                            case 8: // 正后 S
                                sideways = -sideways;
                                break;
                            case 10: // 后左 SW
                            case 12: // 正左 W
                                t = forwards;
                                forwards = sideways;
                                sideways = -t;
                                break;
                            case 14: // 前左 NW
                                break;
                        }
                        pStand.Ref.AngleRotatedForwards = forwards;
                        pStand.Ref.AngleRotatedSideways = sideways;
                        pStand.Ref.RockingForwardsPerFrame = forwards;
                        pStand.Ref.RockingSidewaysPerFrame = sideways;

                        ILocomotion masterLoco = pMaster.Ref.Locomotor;
                        ILocomotion standLoco = pStand.Pointer.Convert<FootClass>().Ref.Locomotor;

                        Guid masterLocoId = masterLoco.ToLocomotionClass().Ref.GetClassID();
                        Guid standLocoId = standLoco.ToLocomotionClass().Ref.GetClassID();
                        if (masterLocoId == LocomotionClass.Drive && standLocoId == LocomotionClass.Drive)
                        {
                            Pointer<DriveLocomotionClass> pMasterLoco = masterLoco.ToLocomotionClass<DriveLocomotionClass>();
                            Pointer<DriveLocomotionClass> pStandLoco = masterLoco.ToLocomotionClass<DriveLocomotionClass>();
                            pStandLoco.Ref.Ramp1 = pMasterLoco.Ref.Ramp1;
                            pStandLoco.Ref.Ramp2 = pMasterLoco.Ref.Ramp2;
                        }
                        else if (masterLocoId == LocomotionClass.Ship && standLocoId == LocomotionClass.Ship)
                        {
                            Pointer<ShipLocomotionClass> pMasterLoco = masterLoco.ToLocomotionClass<ShipLocomotionClass>();
                            Pointer<ShipLocomotionClass> pStandLoco = masterLoco.ToLocomotionClass<ShipLocomotionClass>();
                            pStandLoco.Ref.Ramp1 = pMasterLoco.Ref.Ramp1;
                            pStandLoco.Ref.Ramp2 = pMasterLoco.Ref.Ramp2;
                        }

                        // 播放行动动画的测试
                        // if (standLocoId == LocomotionClass.Drive)
                        // {
                        //     Pointer<DriveLocomotionClass> pStandLoco = masterLoco.ToLocomotionClass<DriveLocomotionClass>();
                        //     pStandLoco.Ref.IsDriving = true;
                        // }
                    }
                }


                // synch Moving anim
                if (Type.IsTrain)
                {
                    // switch (pStand.Ref.Base.Base.WhatAmI())
                    // {
                    //     case AbstractType.Infantry:
                    //         Pointer<InfantryClass> pInf = pStand.Pointer.Convert<InfantryClass>();
                    //         // pInf.Convert<FootClass>().Ref.Inf_PlayAnim(SequenceAnimType.FIRE_WEAPON);

                    //         pInf.Ref.SequenceAnim = SequenceAnimType.FIRE_WEAPON;
                    //         break;
                    //     case AbstractType.Unit:

                    //         break;
                    // }
                    // CoordStruct sourcePos = pStand.Ref.Base.Base.GetCoords();
                    // ILocomotion loco = pStand.Pointer.Convert<FootClass>().Ref.Locomotor;
                    // Guid locoId = loco.ToLocomotionClass().Ref.GetClassID();
                    // if (LocomotionClass.Walk == locoId)
                    // {
                    //     Pointer<WalkLocomotionClass> pLoco = loco.ToLocomotionClass<WalkLocomotionClass>();
                    //     if (masterIsMoving)
                    //     {
                    //         pLoco.Ref.Destination = ExHelper.GetFLHAbsoluteCoords(pStand.Pointer, new CoordStruct(1024, 0, 0));
                    //         pLoco.Ref.IsMoving = false;
                    //     }
                    //     else
                    //     {
                    //         pLoco.Ref.Destination = default;
                    //         pLoco.Ref.IsMoving = false;
                    //     }
                    // }
                    // else if (LocomotionClass.Mech == locoId)
                    // {
                    //     Pointer<MechLocomotionClass> pLoco = loco.ToLocomotionClass<MechLocomotionClass>();
                    //     if (masterIsMoving)
                    //     {
                    //         pLoco.Ref.Destination = ExHelper.GetFLHAbsoluteCoords(pStand.Pointer, new CoordStruct(1024, 0, 0));
                    //         pLoco.Ref.IsMoving = true;
                    //     }
                    //     else
                    //     {
                    //         pLoco.Ref.Destination = default;
                    //         pLoco.Ref.IsMoving = false;
                    //     }

                    // }
                }

            }
        }

        public override void OnUpdate(Pointer<ObjectClass> pObject, CoordStruct location, bool isDead)
        {
            // 只同步状态，位置和朝向由StandManager控制
            if (pObject.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                UpdateState(pTechno);
            }
            else if (pObject.CastToBullet(out Pointer<BulletClass> pBullet))
            {
                UpdateState(pBullet);
            }
        }

        public override void OnTemporalUpdate(TechnoExt ext, Pointer<TemporalClass> pTemporal)
        {
            Pointer<TechnoClass> pMaster = ext.OwnerObject;
            if (pStand.Ref.Owner == pMaster.Ref.Owner)
            {
                pStand.Ref.BeingWarpedOut = pMaster.Ref.BeingWarpedOut; // 超时空冻结
            }
            // Logger.Log($"{Game.CurrentFrame} - 超时空冻结 {pMaster.Ref.BeingWarpedOut}");
        }

        public void UpdateState(Pointer<BulletClass> pBullet)
        {
            // Logger.Log($"{Game.CurrentFrame} 抛射体上的 {AEType.Name} 替身 {Type.Type} {(pStand.Ref.Base.IsAlive ? "存活" : "死亡")}");
            // Synch Target
            RemoveStandIllegalTarget();
            Pointer<AbstractClass> target = pBullet.Ref.Target;
            if (Type.SameTarget && !target.IsNull)
            {
                pStand.Ref.SetTarget(target);
            }
            if (Type.SameLoseTarget && target.IsNull)
            {
                pStand.Ref.SetTarget(target);
                if (target.IsNull && !pStand.Ref.SpawnManager.IsNull)
                {
                    pStand.Ref.SpawnManager.Ref.Destination = target;
                    pStand.Ref.SetTarget(target);
                }
            }
        }

        public void UpdateState(Pointer<TechnoClass> pMaster)
        {
            // Logger.Log($"{Game.CurrentFrame} 单位上的 {AEType.Name} 替身 {Type.Type} {(pStand.Ref.Base.IsAlive ? "存活" : "死亡")}");
            if (pMaster.Ref.IsSinking && Type.RemoveAtSinking)
            {
                // Logger.Log("{0} 船沉了，自爆吧！", Type.Type);
                ExplodesOrDisappear(true);
                return;
            }
            // reset state
            pStand.Ref.Base.Mark(MarkType.UP); // 拔起，不在地图上
            // pStand.Ref.Base.IsOnMap = false;
            // pStand.Ref.Base.NeedsRedraw = true;
            if (pStand.Ref.Base.Base.WhatAmI() != AbstractType.Building)
            {
                pStand.Pointer.Convert<FootClass>().Ref.Locomotor.Lock();
                // pStand.Pointer.Convert<FootClass>().Ref.Jumpjet_LocationClear();
                // Logger.Log("Stand {0} Locomotor {1}", Type.Type, pStand.Pointer.Convert<FootClass>().Ref.Locomotor);
            }

            if (Type.SameHouse)
            {
                // synch Owner
                pStand.Ref.Owner = pMaster.Ref.Owner;
            }


            // synch State
            pStand.Ref.IsSinking = pMaster.Ref.IsSinking;
            pStand.Ref.Shipsink_3CA = pMaster.Ref.Shipsink_3CA;
            pStand.Ref.Base.InLimbo = pMaster.Ref.Base.InLimbo;
            pStand.Ref.Base.OnBridge = pMaster.Ref.Base.OnBridge;
            pStand.Ref.Cloakable = pMaster.Ref.Cloakable;
            pStand.Ref.CloakStates = pMaster.Ref.CloakStates;
            pStand.Ref.BeingWarpedOut = pMaster.Ref.BeingWarpedOut; // 超时空冻结
            pStand.Ref.Deactivated = pMaster.Ref.Deactivated; // 遥控坦克

            pStand.Ref.IronCurtainTimer = pMaster.Ref.IronCurtainTimer;
            pStand.Ref.IdleActionTimer = pMaster.Ref.IdleActionTimer;
            pStand.Ref.IronTintTimer = pMaster.Ref.IronTintTimer;
            // pStand.Ref.CloakDelayTimer = pMaster.Ref.CloakDelayTimer; // 反复进入隐形

            pStand.Ref.Berzerk = pMaster.Ref.Berzerk;
            pStand.Ref.EMPLockRemaining = pMaster.Ref.EMPLockRemaining;
            pStand.Ref.ShouldLoseTargetNow = pMaster.Ref.ShouldLoseTargetNow;

            // synch status
            pStand.Ref.FirepowerMultiplier = pMaster.Ref.FirepowerMultiplier;
            pStand.Ref.ArmorMultiplier = pMaster.Ref.ArmorMultiplier;

            // synch ammo
            if (Type.SameAmmo)
            {
                pStand.Ref.Ammo = pMaster.Ref.Ammo;
            }

            // synch Promote
            if (Type.PromoteFromMaster && pStand.Ref.Type.Ref.Trainable)
            {
                pStand.Ref.Veterancy = pMaster.Ref.Veterancy;
            }

            // synch PrimaryFactory
            pStand.Ref.IsPrimaryFactory = pMaster.Ref.IsPrimaryFactory;

            if (pStand.Pointer.IsInvisible())
            {
                RemoveStandTarget();
                return;
            }

            // get mission
            Mission masterMission = pMaster.Convert<MissionClass>().Ref.CurrentMission;

            // check power off and moving
            bool masterIsBuilding = false;
            bool masterPowerOff = pMaster.Ref.Owner.Ref.NoPower;
            bool masterIsMoving = masterMission == Mission.Move || masterMission == Mission.AttackMove;
            if (masterIsBuilding = (pMaster.Ref.Base.Base.WhatAmI() == AbstractType.Building))
            {
                if (pMaster.Ref.Owner == pStand.Ref.Owner)
                {
                    Pointer<BuildingClass> pBuilding = pMaster.Convert<BuildingClass>();
                    if (!masterPowerOff)
                    {
                        // 关闭当前建筑电源
                        masterPowerOff = !pBuilding.Ref.HasPower;
                    }
                }
            }
            else if (!masterIsMoving)
            {
                Pointer<FootClass> pFoot = pMaster.Convert<FootClass>();
                masterIsMoving = pFoot.Ref.Locomotor.Is_Moving() && pFoot.Ref.GetCurrentSpeed() > 0;
            }

            // check fire
            bool powerOff = Type.Powered && masterPowerOff;
            bool canFire = !powerOff && (Type.MobileFire || !masterIsMoving);
            if (canFire)
            {
                // synch mission
                switch (masterMission)
                {
                    case Mission.Guard:
                    case Mission.Area_Guard:
                        Mission standMission = pStand.Pointer.Convert<MissionClass>().Ref.CurrentMission;
                        if (standMission != Mission.Attack)
                        {
                            pStand.Pointer.Convert<MissionClass>().Ref.QueueMission(masterMission, true);
                        }
                        break;
                }
            }
            else
            {
                RemoveStandTarget();
                onStopCommand = false;
                pStand.Pointer.Convert<MissionClass>().Ref.QueueMission(Mission.Sleep, true);
            }

            // synch target
            if (Type.ForceAttackMaster)
            {
                Pointer<AbstractClass> pTarget = pMaster.Convert<AbstractClass>();
                if (!powerOff && StandCanAttackTarget(pTarget))
                {
                    pStand.Ref.SetTarget(pTarget);
                }
            }
            else
            {
                if (!onStopCommand)
                {
                    // synch Target
                    RemoveStandIllegalTarget();
                    Pointer<AbstractClass> target = pMaster.Ref.Target;
                    if (!target.IsNull)
                    {
                        if (Type.SameTarget && canFire && StandCanAttackTarget(target))
                        {
                            pStand.Ref.SetTarget(target);
                        }
                    }
                    else
                    {
                        if (Type.SameLoseTarget || !canFire)
                        {
                            RemoveStandTarget();
                        }
                    }
                }
                else
                {
                    onStopCommand = false;
                }
            }
        }

        private bool StandCanAttackTarget(Pointer<AbstractClass> pTarget)
        {
            int i = pStand.Ref.SelectWeapon(pTarget);
            FireError fireError = pStand.Ref.GetFireError(pTarget, i, true);
            switch (fireError)
            {
                case FireError.ILLEGAL:
                case FireError.CANT:
                case FireError.MOVING:
                case FireError.RANGE:
                    return false;
            }
            return true;
        }

        private void RemoveStandIllegalTarget()
        {
            Pointer<AbstractClass> pStandTarget;
            if (!(pStandTarget = pStand.Ref.Target).IsNull && !StandCanAttackTarget(pStandTarget))
            {
                pStand.Ref.SetTarget(Pointer<AbstractClass>.Zero);
            }
        }

        private void RemoveStandTarget()
        {
            // Logger.Log("清空替身{0}的目标对象", Type.Type);
            pStand.Ref.Target = IntPtr.Zero;
            pStand.Ref.SetTarget(IntPtr.Zero);
            pStand.Pointer.Convert<MissionClass>().Ref.QueueMission(Mission.Stop, true);
            if (!pStand.Ref.SpawnManager.IsNull)
            {
                pStand.Ref.SpawnManager.Ref.Destination = IntPtr.Zero;
                pStand.Ref.SpawnManager.Ref.Target = IntPtr.Zero;
                pStand.Ref.SpawnManager.Ref.SetTarget(IntPtr.Zero);
            }
        }

        public void UpdateLocation(LocationMark mark)
        {
            SetLocation(mark.Location);
            SetDirection(mark.Direction, false);
        }

        public void SetLocation(CoordStruct location)
        {
            // Logger.Log("{0} - 移动替身[{1}]{2}到位置{3}", Game.CurrentFrame, Type.Type, pStand.Pointer, location);
            pStand.Ref.Base.SetLocation(location);
            pStand.Ref.SetFocus(IntPtr.Zero);
        }

        public void SetDirection(DirStruct direction, bool forceSetTurret)
        {
            if (pStand.Ref.HasTurret() || Type.LockDirection)
            {
                // Logger.Log("设置替身{0}身体的朝向", Type.Type);
                // 替身有炮塔直接转身体
                pStand.Ref.Facing.set(direction);
            }
            // 检查是否需要同步转炮塔
            if ((pStand.Ref.Target.IsNull || Type.LockDirection) && !pStand.Ref.Type.Ref.TurretSpins)
            {
                // Logger.Log("设置替身{0}炮塔的朝向", Type.Type);
                if (forceSetTurret)
                {
                    ForceSetFacing(direction);
                }
                else
                {
                    TurnTurretFacing(direction);
                }
            }

        }

        private void TurnTurretFacing(DirStruct targetDir)
        {
            if (pStand.Ref.HasTurret())
            {
                pStand.Ref.TurretFacing.turn(targetDir);
            }
            else
            {
                pStand.Ref.Facing.turn(targetDir);
            }
        }

        private void ForceSetFacing(DirStruct targetDir)
        {
            pStand.Ref.Facing.set(targetDir);
            pStand.Ref.TurretFacing.set(targetDir);
        }

        public override void OnPut(Pointer<ObjectClass> pOwner, Pointer<CoordStruct> pCoord, short faceDirValue8)
        {
            if (pStand.Ref.Base.InLimbo)
            {
                CoordStruct location = pCoord.Data;
                if (!TryPutStand(location))
                {
                    // Put不出来？
                    Disable(location);
                }
            }
        }

        public override void OnRemove(Pointer<ObjectClass> pOwner)
        {
            if (!pOwner.IsDead())
            {
                pStand.Ref.Base.Remove();
            }
        }

        public override void OnDestroy(Pointer<ObjectClass> pObject)
        {
            // 我不做人了JOJO
            notBeHuman = Type.ExplodesWithMaster;
            // Logger.Log($"{Game.CurrentFrame} - 替身 {pStand.Pointer}[{pStand.Ref.Type.Ref.Base.Base.ID}]的宿主 {pObject}[{pObject.Ref.Type.Ref.Base.ID}]死亡");
            if (pObject.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                // 沉没，坠机，不销毁替身
                pStand.Pointer.Convert<MissionClass>().Ref.QueueMission(Mission.Sleep, true);
            }
            else if (pObject.CastToBullet(out Pointer<BulletClass> pBullet))
            {
                // 抛射体上的宿主直接炸
                Disable(pObject.Ref.Base.GetCoords());
            }
        }

        public override void OnStopCommand()
        {
            // Logger.Log("清空替身{0}的目标对象", Type.Type);
            RemoveStandTarget();
            onStopCommand = true;
            TechnoExt ext = TechnoExt.ExtMap.Find(pStand);
            ext?.OnStopCommand();
            // ext?.Scriptable?.OnStopCommand();
        }

    }

}