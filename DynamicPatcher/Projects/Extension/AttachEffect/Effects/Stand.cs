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
            }
        }

    }


    [Serializable]
    public class Stand : AttachEffectBehaviour
    {
        public StandType Type;

        public SwizzleablePointer<TechnoClass> pStand;

        private bool onStopCommand = false;
        private bool notBeHuman = false;

        public Stand(StandType type, AttachEffectType attachEffectType) : base(attachEffectType)
        {
            this.Type = type;
            this.pStand = new SwizzleablePointer<TechnoClass>(IntPtr.Zero);
        }

        public override bool IsAlive()
        {
            if (pStand.IsNull || !pStand.Ref.Base.IsAlive)
            {
                pStand.Pointer = IntPtr.Zero;
                return false;
            }
            return true;
        }

        // 激活
        public override void Enable(Pointer<ObjectClass> pObject, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker)
        {
            CreateAndPutStand(pObject, pHouse);
        }

        private void CreateAndPutStand(Pointer<ObjectClass> pObject, Pointer<HouseClass> pHouse)
        {
            CoordStruct pCoord = pObject.Ref.Location;
            // Pointer<TechnoClass> pStand = ExHelper.CreateTechno(Type.Type, pHouse, pCoord, pCoord);
            Pointer<TechnoTypeClass> pType = TechnoTypeClass.ABSTRACTTYPE_ARRAY.Find(Type.Type);
            if (!pType.IsNull)
            {
                Pointer<ObjectClass> pNew = pType.Ref.Base.CreateObject(pHouse);
                Pointer<TechnoClass> pStand = pNew.Convert<TechnoClass>();
                // reset state
                pStand.Ref.Base.UpdatePlacement(PlacementType.Remove);
                pStand.Ref.Base.IsOnMap = false;

                // Logger.Log("创建替身{0}, {1}", Type.Type, pStand.Ref.Base.InLimbo);
                this.pStand.Pointer = pStand;
                // 同步部分扩展设置
                TechnoExt ext = TechnoExt.ExtMap.Find(pStand);
                if (null != ext)
                {
                    if (this.Type.VirtualUnit)
                    {
                        ext.VirtualUnit = this.Type.VirtualUnit;
                    }
                    Pointer<TechnoClass> pBulletOwner = IntPtr.Zero;
                    if (pObject.Ref.Base.WhatAmI() == AbstractType.Bullet && !(pBulletOwner = pObject.Convert<BulletClass>().Ref.Owner).IsNull)
                    {
                        // 附加在抛射体上的，取抛射体的所有者
                        ext.MyMaster.Pointer = pBulletOwner.Convert<ObjectClass>();
                    }
                    else
                    {
                        ext.MyMaster.Pointer = pObject;
                    }
                    ext.StandType = Type;
                }
                ++Game.IKnowWhatImDoing;
                pNew.Ref.Put(pCoord + new CoordStruct(0, 0, 1024), Direction.N);
                --Game.IKnowWhatImDoing;
                // 直接放置在指定位置
                LocationMark locationMark = AttachEffectHelper.GetLocation(pObject, Type);
                if (default != locationMark.Location)
                {
                    SetLocation(locationMark.Location);
                    // 强扭朝向
                    ForceSetFacing(locationMark.Direction);
                }
                else
                {
                    pNew.Ref.SetLocation(pCoord);
                }
            }
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

        public override void OnUpdate(Pointer<ObjectClass> pObject, bool isDead, AttachEffectManager manager)
        {
            CoordStruct sourcePos = pObject.Ref.Location;
            // 只同步状态，位置和朝向由StandManager控制
            // if (pObject.CastToTechno(out Pointer<TechnoClass> pMaster))
            // {
            //     UpdateState(pMaster);
            // }
            switch (pObject.Ref.Base.WhatAmI())
            {
                case AbstractType.Unit:
                case AbstractType.Aircraft:
                case AbstractType.Building:
                case AbstractType.Infantry:
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
            CancelStandTarget();
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
                pStand.Pointer.Convert<FootClass>().Ref.Locomotor.Ref.Lock();
                // pStand.Pointer.Convert<FootClass>().Ref.Jumpjet_LocationClear();
                // Logger.Log("Stand {0} Locomotor {1}", Type.Type, pStand.Pointer.Convert<FootClass>().Ref.Locomotor);
            }

            // synch Owner
            pStand.Ref.Owner = pMaster.Ref.Owner;

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
            pStand.Ref.Deactivated = pMaster.Ref.Deactivated;

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

            // synch Mission
            Mission mission = pMaster.Convert<MissionClass>().Ref.CurrentMission;
            switch (mission)
            {
                case Mission.Move:
                case Mission.AttackMove:
                    break;
                case Mission.Guard:
                case Mission.Area_Guard:
                    Mission standMission = pStand.Pointer.Convert<MissionClass>().Ref.CurrentMission;
                    if (standMission != Mission.Attack)
                    {
                        pStand.Pointer.Convert<MissionClass>().Ref.QueueMission(mission, true);
                    }
                    break;
            }

            if (!pMaster.Ref.Base.IsActive() || (Type.Powered && !pStand.Ref.Owner.Ref.RecheckPower))
            {
                pStand.Pointer.Convert<MissionClass>().Ref.QueueMission(Mission.Sleep, true);
            }

            if (!onStopCommand)
            {
                // synch Target
                CancelStandTarget();
                Pointer<AbstractClass> target = pMaster.Ref.Target;
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
            else
            {
                onStopCommand = false;
            }
        }

        private void CancelStandTarget()
        {
            Pointer<AbstractClass> pStandTarget;
            if (!(pStandTarget = pStand.Ref.Target).IsNull)
            {
                int weaponCount = pStand.Ref.Type.Ref.WeaponCount;
                if (weaponCount == 0)
                {
                    weaponCount = 2;
                }
                bool canFire = false;
                for (int i = 0; i < weaponCount; i++)
                {
                    FireError fireError = pStand.Ref.GetFireError(pStandTarget, i, true);
                    switch (fireError)
                    {
                        case FireError.ILLEGAL:
                        case FireError.CANT:
                        case FireError.MOVING:
                        case FireError.RANGE:
                            break;
                        default:
                            canFire = true;
                            break;
                    }
                }
                if (!canFire)
                {
                    pStand.Ref.SetTarget(Pointer<AbstractClass>.Zero);
                }
            }
        }

        // public void UpdateLocation(Pointer<TechnoClass> pMaster)
        // {
        //     CoordStruct location = ExHelper.GetFLHAbsoluteCoords(pMaster, Type.Offset, Type.IsOnTurret);
        //     DirStruct targetDir = GetDirection(pMaster);

        //     SetLocation(location);
        //     SetDirection(targetDir);
        // }

        public void UpdateLocation(LocationMark mark)
        {
            SetLocation(mark);
            SetDirection(mark);
        }

        private void SetLocation(LocationMark mark)
        {
            SetLocation(mark.Location);
        }

        private void SetLocation(CoordStruct location)
        {
            // Logger.Log("移动替身{0}的位置到{1}", Type.Type, location);
            CoordStruct sourcePos = pStand.Ref.Base.Base.GetCoords();
            if (sourcePos != location)
            {
                // 播放行动动画
                // switch (pStand.Ref.Base.Base.WhatAmI())
                // {
                //     case AbstractType.Infantry:
                //         // Pointer<InfantryClass> pInf = pStand.Pointer.Convert<InfantryClass>();
                //         // if (SequenceAnimType.WALK != pInf.Ref.SequenceAnim)
                //         // {
                //         //     Logger.Log("丫动起来 {0}, {1}", Type.Type, pInf.Ref.SequenceAnim);
                //         //     pInf.Convert<FootClass>().Ref.Inf_PlayAnim(SequenceAnimType.WALK);
                //         // }
                //         break;
                //     case AbstractType.Unit:
                //         // if (default == pStand.Pointer.Convert<FootClass>().Ref.Locomotor.Ref.DestinationCoord)
                //         // {
                //         //     Logger.Log("丫动起来 {0}", Type.Type);
                //         //     // pStand.Pointer.Convert<FootClass>().Ref.MoveTo(location);
                //         // }
                //         // pStand.Pointer.Convert<FootClass>().Ref.Locomotor.Ref.Move_To(location);
                //         if (!pStand.Pointer.Convert<FootClass>().Ref.Locomotor.Ref.Is_Moving())
                //         {
                //             pStand.Pointer.Convert<FootClass>().Ref.MoveTo(location);
                //         }
                //         break;
                // }
            }
            else
            {
                // if (pStand.Pointer.Convert<FootClass>().Ref.Locomotor.Ref.Is_Moving())
                // {
                //     pStand.Pointer.Convert<FootClass>().Ref.MoveTo(default);
                // }
            }
            pStand.Ref.Base.SetLocation(location);
            pStand.Ref.SetFocus(Pointer<AbstractClass>.Zero);
            pStand.Ref.SetDestination(Pointer<CellClass>.Zero, true);

        }

        private void SetDirection(LocationMark mark)
        {
            SetDirection(mark.Direction, false);
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
                TurnTurretFacing(direction);
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
            CoordStruct location = pCoord.Data;
            ++Game.IKnowWhatImDoing;
            pStand.Ref.Base.Put(location + new CoordStruct(0, 0, 1024), faceDir);
            --Game.IKnowWhatImDoing;
            pStand.Ref.Base.SetLocation(location);
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
            pStand.Ref.Target = IntPtr.Zero;
            pStand.Ref.SetTarget(IntPtr.Zero);
            pStand.Pointer.Convert<MissionClass>().Ref.QueueMission(Mission.Area_Guard, true);
            if (!pStand.Ref.SpawnManager.IsNull)
            {
                pStand.Ref.SpawnManager.Ref.Destination = IntPtr.Zero;
                pStand.Ref.SpawnManager.Ref.Target = IntPtr.Zero;
                pStand.Ref.SpawnManager.Ref.SetTarget(IntPtr.Zero);
            }
            onStopCommand = true;
            TechnoExt ext = TechnoExt.ExtMap.Find(pStand);
            ext?.OnStopCommand();
            ext?.Scriptable?.OnStopCommand();
        }

    }

}