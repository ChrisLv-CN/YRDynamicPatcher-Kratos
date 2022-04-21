using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    // // 从箱子中获取的加成
    // [Serializable]
    // public class CrateMultiplier
    // {
    //     public double FirepowerMultiplier;
    //     public double ArmorMultiplier;
    //     public double SpeedMultiplier;
    //     public double ROFMultiplier;
    //     public bool Cloakable;
    //     public bool ForceDecloak;

    //     public CrateMultiplier()
    //     {
    //         this.FirepowerMultiplier = 1.0;
    //         this.ArmorMultiplier = 1.0;
    //         this.SpeedMultiplier = 1.0;
    //         this.ROFMultiplier = 1.0;
    //         this.Cloakable = false;
    //         this.ForceDecloak = false;
    //     }

    //     public override string ToString()
    //     {
    //         return string.Format("{{\"FirepowerMultiplier\":{0}, \"ArmorMultiplier\":{1}, \"SpeedMultiplier\":{2}, \"ROFMultiplier\":{3}, \"Cloakable\":{4}, \"ForceDeclock\":{5}}}",
    //             FirepowerMultiplier, ArmorMultiplier, SpeedMultiplier, ROFMultiplier, Cloakable, ForceDecloak
    //         );
    //     }
    // }

    [Serializable]
    public class RecordBulletStatus
    {
        public int Health;
        public int Speed;
        public BulletVelocity Velocity;
        public bool CourseLocked;

        public RecordBulletStatus(int health, int speed, BulletVelocity velocity, bool courseLocked)
        {
            this.Health = health;
            this.Speed = speed;
            this.Velocity = velocity;
            this.CourseLocked = courseLocked;
        }

        public override string ToString()
        {
            return string.Format("{{\"Health\":{0}, \"Speed\":{1}, \"Velocity\":{2}, \"CourseLocked\":{3}}}", Health, Speed, Velocity, CourseLocked);
        }
    }


    public partial class TechnoExt
    {
        // 记录从箱子中获取的加成
        public AttachStatusType CrateStatus = new AttachStatusType();

        public unsafe void RecalculateStatus()
        {
            if (IsDead)
            {
                return;
            }
            // 获取箱子加成
            double firepowerMult = CrateStatus.FirepowerMultiplier;
            double armorMult = CrateStatus.ArmorMultiplier;
            double speedMult = CrateStatus.SpeedMultiplier;
            double rofMult = CrateStatus.ROFMultiplier;
            bool cloakable = CanICloakByDefault() || CrateStatus.Cloakable;
            // 算上AE加成
            AttachStatusType aeMultiplier = AttachEffectManager.CountAttachStatusMultiplier();
            // 赋予单位
            OwnerObject.Ref.FirepowerMultiplier = firepowerMult * aeMultiplier.FirepowerMultiplier;
            OwnerObject.Ref.ArmorMultiplier = armorMult * aeMultiplier.ArmorMultiplier;
            OwnerObject.Ref.Cloakable = cloakable |= aeMultiplier.Cloakable;
            if (OwnerObject.Ref.Base.Base.WhatAmI() != AbstractType.Building)
            {
                OwnerObject.Convert<FootClass>().Ref.SpeedMultiplier = speedMult * aeMultiplier.SpeedMultiplier;
            }
        }

        public unsafe bool CanICloakByDefault()
        {
            return (!OwnerObject.IsNull && !OwnerObject.Ref.Type.IsNull) && (OwnerObject.Ref.Type.Ref.Cloakable || OwnerObject.Ref.HasAbility(Ability.Cloak));
        }

    }

    public partial class BulletExt
    {

        public RecordBulletStatus RecordBulletStatus;
        public bool SpeedChanged = false;
        public bool LocationLocked = false;

        public unsafe void BulletClass_Update_RecalculateStatus()
        {
            if (!SpeedChanged)
            {
                RecordBulletStatus = new RecordBulletStatus(OwnerObject.Ref.Base.Health, OwnerObject.Ref.Speed, OwnerObject.Ref.Velocity, OwnerObject.Ref.CourseLocked);
            }
            // 计算AE伤害加成
            AttachStatusType aeMultiplier = AttachEffectManager.CountAttachStatusMultiplier();
            int newHealth = RecordBulletStatus.Health;
            if (aeMultiplier.FirepowerMultiplier != 1)
            {
                // 调整参数
                newHealth = (int)Math.Ceiling(newHealth * aeMultiplier.FirepowerMultiplier);
            }
            // 重设伤害值
            OwnerObject.Ref.Base.Health = newHealth;
            if (null != BulletDamageStatus)
            {
                BulletDamageStatus.Damage = newHealth;
            }
            // 计算AE速度加成
            if (aeMultiplier.SpeedMultiplier != 1)
            {
                SpeedChanged = true;
            }
            // Logger.Log("方向向量 - {0}，速度系数{1}，记录向量{2}", OwnerObject.Ref.Velocity, aeMultiplier.SpeedMultiplier, RecordBulletStatus.Velocity);
            // 还原
            if (SpeedChanged && aeMultiplier.SpeedMultiplier == 1.0)
            {
                SpeedChanged = false;
                LocationLocked = false;
                OwnerObject.Ref.Speed = RecordBulletStatus.Speed;
                if (IsStraight())
                {
                    // 恢复直线弹道的向量
                    straightBullet.Velocity = RecalculateBulletVelocity(straightBullet.sourcePos, straightBullet.targetPos);
                }
                else if (OwnerObject.Ref.Type.Ref.Arcing)
                {
                    // 抛物线类型的向量，只恢复方向向量，即X和Y
                    double x = RecordBulletStatus.Velocity.X;
                    double y = RecordBulletStatus.Velocity.Y;
                    BulletVelocity nowVelocity = OwnerObject.Ref.Velocity;
                    if (nowVelocity.X < 0 && x > 0)
                    {
                        x *= -1;
                    }
                    if (nowVelocity.Y < 0 && y > 0)
                    {
                        y *= -1;
                    }
                    OwnerObject.Ref.Velocity.X = x;
                    OwnerObject.Ref.Velocity.Y = y;
                }
                return;
            }

            // 更改运动向量
            if (SpeedChanged)
            {
                double multiplier = aeMultiplier.SpeedMultiplier;
                if (multiplier == 0.0)
                {
                    LocationLocked = true;
                    OwnerObject.Ref.Speed = 1;
                    multiplier = 1E-19;
                }
                // 导弹类需要每帧更改一次运动向量
                if (IsStraight())
                {
                    // 直线导弹用保存的向量覆盖，每次都要重新计算
                    OwnerObject.Ref.Velocity *= multiplier;
                }
                else if (OwnerObject.Ref.Type.Ref.Arcing)
                {
                    // Arcing类，重算方向上向量，即X和Y
                    BulletVelocity recVelocity = RecordBulletStatus.Velocity;
                    recVelocity.Z = OwnerObject.Ref.Velocity.Z;
                    BulletVelocity newVelocity = recVelocity * multiplier;
                    OwnerObject.Ref.Velocity = newVelocity;
                }
                else
                {
                    OwnerObject.Ref.Velocity *= multiplier;
                }

                // Logger.Log(" - 方向向量{0}，速度系数{1}，记录向量{2}", OwnerObject.Ref.Velocity, aeMultiplier.SpeedMultiplier, RecordBulletStatus.Velocity);
            }
        }

    }

}

