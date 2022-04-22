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
                                }
                                break;
                        }
                        ext.StandType = Type;
                    }

                    // 初始化替身
                    pStand.Ref.Base.UpdatePlacement(PlacementType.Remove);
                    pStand.Ref.Base.IsOnMap = false;
                    pStand.Ref.Base.NeedsRedraw = true;
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

                    // 在格子位置刷出替身单位
                    if (!TryPutStand(location))
                    {
                        // 刷不出来？
                        Disable(location);
                        return;
                    }

                    // 放置到指定位置
                    LocationMark locationMark = StandHelper.GetLocation(pObject, Type);
                    if (default != locationMark.Location)
                    {
                        SetLocation(locationMark.Location);
                        // 强扭朝向
                        ForceSetFacing(locationMark.Direction);
                    }

                    // Logger.Log("{0} - 创建替身[{1}]{2}", Game.CurrentFrame, Type.Type, pStand.Pointer);
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
                pStand.Ref.Base.Put(xyz, Direction.E);
                --Game.IKnowWhatImDoing;
                pCell.Ref.OccupationFlags = occFlags;
                return true;
            }
            return false;
        }

        // 销毁
        public override void Disable(CoordStruct location)
        {
            if (pStand.IsNull)
            {
                return;
            }
            ExplodesOrDisappear(false);
        }

        private void ExplodesOrDisappear(bool remove)
        {
            // Logger.Log("替身{0}注销", Type.Type);
            if (Type.Explodes || notBeHuman)
            {
                pStand.Ref.Base.TakeDamage(pStand.Ref.Base.Health + 1, pStand.Ref.Type.Ref.Crewed);
                if (remove)
                {
                    pStand.Ref.Base.Remove();
                }
            }
            else
            {
                // pStand.Ref.Base.Remove();
                pStand.Ref.Base.UnInit();
            }
            pStand.Pointer = IntPtr.Zero;
        }

        public override void OnUpdate(Pointer<ObjectClass> pObject, bool isDead)
        {
            CoordStruct sourcePos = pObject.Ref.Base.GetCoords();

            // 只同步状态，位置和朝向由StandManager控制
            switch (pObject.Ref.Base.WhatAmI())
            {
                case AbstractType.Unit:
                case AbstractType.Aircraft:
                case AbstractType.Infantry:
                case AbstractType.Building:
                    UpdateState(pObject.Convert<TechnoClass>());
                    break;
                case AbstractType.Bullet:
                    UpdateState(pObject.Convert<BulletClass>());
                    break;
            }
        }

        public void UpdateState(Pointer<BulletClass> pBullet)
        {
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
            // Logger.Log("Stand {0} {1}", Type.Name, pStand.Ref.Base.IsAlive ? "is Alive" : "is Dead");
            if (pMaster.Ref.IsSinking && Type.RemoveAtSinking)
            {
                // Logger.Log("{0} 船沉了，自爆吧！", Type.Type);
                ExplodesOrDisappear(true);
                return;
            }
            // reset state
            pStand.Ref.Base.UpdatePlacement(PlacementType.Remove);
            pStand.Ref.Base.IsOnMap = false;
            pStand.Ref.Base.NeedsRedraw = true;
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

            // synch Tilt
            if (!Type.IsTrain)
            {
                pStand.Ref.AngleRotatedForwards = pMaster.Ref.AngleRotatedForwards;
                pStand.Ref.AngleRotatedSideways = pMaster.Ref.AngleRotatedSideways;
            }

            // synch State
            pStand.Ref.IsSinking = pMaster.Ref.IsSinking;
            pStand.Ref.Shipsink_3CA = pMaster.Ref.Shipsink_3CA;
            pStand.Ref.Base.InLimbo = pMaster.Ref.Base.InLimbo;
            pStand.Ref.Base.OnBridge = pMaster.Ref.Base.OnBridge;
            pStand.Ref.CloakStates = pMaster.Ref.CloakStates;
            pStand.Ref.BeingWarpedOut = pMaster.Ref.BeingWarpedOut;
            pStand.Ref.Deactivated = pMaster.Ref.Deactivated; // 遥控坦克

            pStand.Ref.IronCurtainTimer = pMaster.Ref.IronCurtainTimer;
            pStand.Ref.IdleActionTimer = pMaster.Ref.IdleActionTimer;
            pStand.Ref.IronTintTimer = pMaster.Ref.IronTintTimer;
            pStand.Ref.CloakDelayTimer = pMaster.Ref.CloakDelayTimer;

            pStand.Ref.Berzerk = pMaster.Ref.Berzerk;
            pStand.Ref.EMPLockRemaining = pMaster.Ref.EMPLockRemaining;
            pStand.Ref.ShouldLoseTargetNow = pMaster.Ref.ShouldLoseTargetNow;

            // synch Promote
            if (Type.PromoteFormMaster && pStand.Ref.Type.Ref.Trainable)
            {
                pStand.Ref.Veterancy = pMaster.Ref.Veterancy;
            }

            // synch PrimaryFactory
            pStand.Ref.IsPrimaryFactory = pMaster.Ref.IsPrimaryFactory;

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

            ILocomotion masterLoco = null;
            ILocomotion standLoco = null;
            if (!masterIsBuilding)
            {
                masterLoco = pMaster.Convert<FootClass>().Ref.Locomotor;
            }
            if (!isBuilding)
            {
                standLoco = pStand.Pointer.Convert<FootClass>().Ref.Locomotor;
            }

            // synch Moving anim
            if (Type.IsTrain && !isBuilding)
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
            else if (Type.SameTilter && null != masterLoco && null != standLoco)
            {
                // synch Tilter
                Guid masterLocoId = masterLoco.ToLocomotionClass().Ref.GetClassID();
                Guid standLocoId = standLoco.ToLocomotionClass().Ref.GetClassID();
                if (LocomotionClass.Drive == masterLocoId && LocomotionClass.Drive == standLocoId)
                {
                    Pointer<DriveLocomotionClass> pMasterLoco = masterLoco.ToLocomotionClass<DriveLocomotionClass>();
                    Pointer<DriveLocomotionClass> pStandLoco = standLoco.ToLocomotionClass<DriveLocomotionClass>();
                    pStandLoco.Ref.Ramp1 = pMasterLoco.Ref.Ramp1;
                    pStandLoco.Ref.Ramp2 = pMasterLoco.Ref.Ramp2;
                }
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
                if (!powerOff)
                {
                    pStand.Ref.SetTarget(pMaster.Convert<AbstractClass>());
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
                        if (Type.SameTarget && canFire)
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

        private void RemoveStandIllegalTarget()
        {
            Pointer<AbstractClass> pStandTarget;
            if (!(pStandTarget = pStand.Ref.Target).IsNull)
            {
                int i = pStand.Ref.SelectWeapon(pStandTarget);
                FireError fireError = pStand.Ref.GetFireError(pStandTarget, i, true);
                switch (fireError)
                {
                    case FireError.ILLEGAL:
                    case FireError.CANT:
                    case FireError.MOVING:
                    case FireError.RANGE:
                        pStand.Ref.SetTarget(Pointer<AbstractClass>.Zero);
                        break;
                }
            }
        }

        private void RemoveStandTarget()
        {
            // Logger.Log("清空替身{0}的目标对象", Type.Type);
            pStand.Ref.Target = IntPtr.Zero;
            pStand.Ref.SetTarget(IntPtr.Zero);
            pStand.Pointer.Convert<MissionClass>().Ref.QueueMission(Mission.Area_Guard, true);
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

        private void SetLocation(CoordStruct location)
        {
            // Logger.Log("{0} - 移动替身[{1}]{2}到位置{3}", Game.CurrentFrame, Type.Type, pStand.Pointer, location);
            pStand.Ref.Base.SetLocation(location);
            pStand.Ref.SetFocus(IntPtr.Zero);
        }

        private void SetDirection(DirStruct direction, bool forceSetTurret)
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

        public override void OnPut(Pointer<ObjectClass> pOwner, Pointer<CoordStruct> pCoord, Direction faceDir)
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
            pStand.Ref.Base.Remove();
        }

        public override void OnDestroy(Pointer<ObjectClass> pObject)
        {
            // 我不做人了JOJO
            notBeHuman = Type.ExplodesWithMaster;
            // Logger.Log("替身{0}死亡，{1}", pStand.Ref.Type.Ref.Base.Base.ID, Type.Explodes ? "爆炸" : "什么都不做");
            pStand.Pointer.Convert<MissionClass>().Ref.QueueMission(Mission.Sleep, true);
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