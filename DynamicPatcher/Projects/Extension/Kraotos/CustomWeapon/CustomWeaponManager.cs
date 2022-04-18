using System.Collections;
using DynamicPatcher;
using Extension.Ext;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    // 单次发射的数据
    [Serializable]
    public class SimulateBurst
    {
        public SwizzleablePointer<WeaponTypeClass> pWeaponType; // 武器指针
        public SwizzleablePointer<TechnoClass> pShooter; // 发射武器的对象指针
        public SwizzleablePointer<AbstractClass> pTarget; // 目标指针
        public CoordStruct FLH; // 发射的位置
        public AttachFireData FireData; // 武器发射的设置
        public FireBulletToTarget Callback; // 武器发射后回调函数
        public int Burst; // 总数
        public int Range; // 射程

        public int FlipY; // 左右发射位点标签
        public int Index; // 当前发射的序号

        private TimerStruct timer;
        private int flag;

        public SimulateBurst(Pointer<WeaponTypeClass> pWeaponType, Pointer<TechnoClass> pShooter, Pointer<AbstractClass> pTarget, CoordStruct flh, int burst, int range, AttachFireData fireData, int flipY, FireBulletToTarget callback)
        {
            this.pWeaponType = new SwizzleablePointer<WeaponTypeClass>(pWeaponType);
            this.pShooter = new SwizzleablePointer<TechnoClass>(pShooter);
            this.pTarget = new SwizzleablePointer<AbstractClass>(pTarget);
            this.FLH = flh;
            this.Burst = burst;
            this.Range = range;
            this.FireData = fireData;
            this.Callback = callback;
            this.FlipY = flipY;
            this.flag = flipY;
            this.Index = 0;
            this.timer = new TimerStruct(fireData.SimulateBurstDelay);
        }

        public SimulateBurst Clone()
        {
            SimulateBurst newObj = new SimulateBurst(pWeaponType, pShooter, pTarget, FLH, Burst, Range, FireData, FlipY, Callback);
            newObj.Index = Index;
            return newObj;
        }

        public bool CanFire()
        {
            if (timer.Expired())
            {
                timer.Start(FireData.SimulateBurstDelay);
                return true;
            }
            return false;
        }

        public void CountOne()
        {
            Index++;
            switch (FireData.SimulateBurstMode)
            {
                case 1:
                    // 左右切换
                    FlipY *= -1;
                    break;
                case 2:
                    // 左一半右一半
                    FlipY = (Index < Burst / 2f) ? flag : -flag;
                    break;
                default:
                    break;
            }
        }

        // public override string ToString()
        // {
        //     return string.Format("{{\"WeaponId\":{0}, \"FLH\":{1}, \"Index\":{2}, \"Burst\":{3}, \"Delay\":{4}}}", WeaponId, FLH, Index, Burst, Delay);
        // }

    }

    [Serializable]
    public class CustomWeaponManager
    {
        // Burst发射模式下剩余待发射的队列
        private Queue<SimulateBurst> simulateBurstQueue = new Queue<SimulateBurst>();

        public void Update(TechnoExt ext)
        {
            Pointer<TechnoClass> pAttacker = ext.OwnerObject;
            // 失去目标时清空所有待发射队列
            if (pAttacker.IsNull || pAttacker.Ref.Target.IsNull || ext.IsDead)
            {
                // if (simulateBurstQueue.Count > 0)
                // {
                //     Logger.Log("{0} - {1}{2}失去目标清空所有待发射队列，目标类型{3}", Game.CurrentFrame, pAttacker.IsNull ? "null" : pAttacker.Ref.Type.Ref.Base.Base.ID, ext.IsDead ? " is Dead" : " " + pAttacker, pAttacker.Ref.Target.IsNull ? "null" : pAttacker.Ref.Target.Ref.WhatAmI());
                // }
                simulateBurstQueue.Clear();
            }
            else
            {
                // 模拟Burst发射
                for (int i = 0; i < simulateBurstQueue.Count; i++)
                {
                    SimulateBurst burst = simulateBurstQueue.Dequeue();
                    // 检查是否还需要发射
                    if (burst.Index < burst.Burst)
                    {
                        // 检查延迟
                        if (burst.CanFire())
                        {
                            Pointer<TechnoClass> pShooter = burst.pShooter;
                            Pointer<AbstractClass> pTarget = burst.pTarget;
                            Pointer<WeaponTypeClass> pWeaponType = burst.pWeaponType;
                            // 检查目标幸存和射程
                            if (!pWeaponType.IsNull
                                && !pShooter.IsNull && pShooter.Ref.Base.IsAlive
                                && !pTarget.IsNull && (!pTarget.CastToTechno(out Pointer<TechnoClass> pTemp) || !pTemp.IsDeadOrInvisible())
                                && (!burst.FireData.CheckRange || pShooter.Ref.Base.Base.GetCoords().DistanceFrom(pTarget.Ref.GetCoords()) <= burst.Range)
                                && (pAttacker.Ref.Transporter.IsNull || (pWeaponType.Ref.FireInTransport || burst.FireData.OnlyFireInTransport))
                            )
                            {
                                // 发射
                                SimulateBurstFire(pShooter, pAttacker, pTarget, pWeaponType, ref burst);
                            }
                            else
                            {
                                // 武器失效
                                continue;
                            }
                        }
                        // 归队
                        simulateBurstQueue.Enqueue(burst);
                    }
                }
            }
        }

        public bool FireCustomWeapon(Pointer<TechnoClass> pShooter, Pointer<TechnoClass> pAttacker, Pointer<AbstractClass> pTarget, string weaponId, CoordStruct flh, CoordStruct bulletSourcePos, double rofMult, FireBulletToTarget callback)
        {
            bool isFire = false;
            pShooter = WhoIsShooter(pShooter);
            Pointer<WeaponTypeClass> pWeapon = WeaponTypeClass.ABSTRACTTYPE_ARRAY.Find(weaponId);
            if (!pWeapon.IsNull && (pAttacker.Ref.Transporter.IsNull || pWeapon.Ref.FireInTransport))
            {
                WeaponTypeExt typeExt = WeaponTypeExt.ExtMap.Find(pWeapon);
                if (null != typeExt)
                {
                    AttachFireData fireData = typeExt.AttachFireData;
                    // 在载具内的特殊情况，武器是否可以使用以及使用的是FLH
                    CoordStruct fireFLH = flh;
                    Pointer<TechnoClass> pTransporter = pAttacker.Ref.Transporter;
                    if (!pTransporter.IsNull)
                    {
                        if (fireData.UseAlternateFLH)
                        {
                            // 获取开或者位于载具的序号，再获取载具的开火坐标，先进后出，序号随着乘客数量增长
                            int index = pTransporter.Ref.Passengers.IndexOf(pAttacker.Convert<FootClass>());
                            // 有效序号1 - 5, 对应FLH 6 - 10
                            if (index < 6)
                            {
                                fireFLH = pTransporter.Ref.Type.Ref.TurretWeaponFLH[index + 5];
                            }
                            // Logger.Log("Use AlternateFLH, passengers index {0}, fireFLH = {1}", index, fireFLH);
                        }
                    }
                    else if (fireData.OnlyFireInTransport)
                    {
                        // 仅有在载具内才可以发射这个武器
                        return isFire;
                    }
                    int burst = pWeapon.Ref.Burst;
                    int range = pWeapon.Ref.Range;
                    if (pTarget.Ref.IsInAir())
                    {
                        range += pShooter.Ref.Type.Ref.AirRangeBonus;
                    }
                    if (burst > 1 && fireData.SimulateBurst)
                    {
                        // 模拟burst发射武器
                        // 抛射体如果反向发射，翻转L
                        int flipY = 1;
                        Pointer<BulletTypeClass> pBulletType = pWeapon.Ref.Projectile;
                        if (!pBulletType.IsNull)
                        {
                            BulletTypeExt bulletTypeExt = BulletTypeExt.ExtMap.Find(pBulletType);
                            if (null != bulletTypeExt && bulletTypeExt.MissileBulletData.ReverseVelocity)
                            {
                                flipY = -1;
                            }
                        }
                        SimulateBurst newBurst = new SimulateBurst(pWeapon, pShooter, pTarget, fireFLH, burst, range, fireData, flipY, callback);
                        // Logger.Log("{0} - {1}{2}添加订单模拟Burst发射{3}发，目标类型{4}，入队", Game.CurrentFrame, pAttacker.IsNull ? "null" : pAttacker.Ref.Type.Ref.Base.Base.ID, pAttacker, burst, pAttacker.Ref.Target.IsNull ? "null" : pAttacker.Ref.Target.Ref.WhatAmI());
                        // 发射武器
                        SimulateBurstFire(pShooter, pAttacker, pTarget, pWeapon, ref newBurst);
                        // 入队
                        simulateBurstQueue.Enqueue(newBurst);
                        isFire = true;
                    }
                    else
                    {
                        // 检查射程
                        if (!fireData.CheckRange || pShooter.Ref.Base.Base.GetCoords().DistanceFrom(pTarget.Ref.GetCoords()) <= range)
                        {
                            // 直接发射武器
                            // Logger.Log("{0} - {1}{2}添加订单发射自定义武器{3}，目标类型{4}，入队", Game.CurrentFrame, pAttacker.IsNull ? "null" : pAttacker.Ref.Type.Ref.Base.Base.ID, pAttacker, pWeapon.Ref.Base.ID, pAttacker.Ref.Target.IsNull ? "null" : pAttacker.Ref.Target.Ref.WhatAmI());
                            ExHelper.FireWeaponTo(pShooter, pAttacker, pTarget, pWeapon, fireFLH, callback, bulletSourcePos, fireData.RadialFire, fireData.RadialAngle);
                            isFire = true;
                        }
                    }
                }
                else
                {
                    // 直接发射武器
                    // Logger.Log("{0} - {1}{2}添加订单发射自定义武器{3}，目标类型{4}，入队", Game.CurrentFrame, pAttacker.IsNull ? "null" : pAttacker.Ref.Type.Ref.Base.Base.ID, pAttacker, pWeapon.Ref.Base.ID, pAttacker.Ref.Target.IsNull ? "null" : pAttacker.Ref.Target.Ref.WhatAmI());
                    ExHelper.FireWeaponTo(pShooter, pAttacker, pTarget, pWeapon, flh, callback, bulletSourcePos);
                    isFire = true;
                }
            }
            return isFire;
        }

        private void SimulateBurstFire(Pointer<TechnoClass> pShooter, Pointer<TechnoClass> pAttacker, Pointer<AbstractClass> pTarget, Pointer<WeaponTypeClass> pWeapon, ref SimulateBurst burst)
        {
            // 模拟Burst发射武器
            if (burst.FireData.SimulateBurstMode == 3)
            {
                // 模式3，双发
                SimulateBurst b2 = burst.Clone();
                b2.FlipY *= -1;
                SimulateBurstFireOnce(pShooter, pAttacker, pTarget, pWeapon, ref b2);
            }
            // 单发
            SimulateBurstFireOnce(pShooter, pAttacker, pTarget, pWeapon, ref burst);

        }

        private void SimulateBurstFireOnce(Pointer<TechnoClass> pShooter, Pointer<TechnoClass> pAttacker, Pointer<AbstractClass> pTarget, Pointer<WeaponTypeClass> pWeapon, ref SimulateBurst burst)
        {
            // Pointer<TechnoClass> pShooter = WhoIsShooter(pShooter);
            CoordStruct sourcePos = ExHelper.GetFLHAbsoluteCoords(pShooter, burst.FLH, true, burst.FlipY);
            CoordStruct targetPos = pTarget.Ref.GetCoords();
            BulletVelocity bulletVelocity = default;
            // 扇形攻击
            if (burst.FireData.RadialFire)
            {
                RadialFireHelper radialFireHelper = new RadialFireHelper(pShooter, burst.Burst, burst.FireData.RadialAngle);
                bulletVelocity = radialFireHelper.GetBulletVelocity(burst.Index);
            }
            else
            {
                bulletVelocity = ExHelper.GetBulletVelocity(sourcePos, targetPos);
            }
            // 发射武器
            Pointer<BulletClass> pBullet = ExHelper.FireBulletTo(pShooter, pAttacker, pTarget, pWeapon, sourcePos, targetPos, bulletVelocity);
            if (null != burst.Callback)
            {
                burst.Callback(burst.Index, burst.Burst, pBullet, pTarget);
            }
            burst.CountOne();
        }

        private Pointer<TechnoClass> WhoIsShooter(Pointer<TechnoClass> pAttacker)
        {
            Pointer<TechnoClass> pTransporter = pAttacker.Ref.Transporter;
            if (!pTransporter.IsNull)
            {
                // I'm a passengers
                pAttacker = pTransporter;
            }
            return pAttacker;
        }


        public void UnInitAll()
        {
            simulateBurstQueue.Clear();
        }

    }

}