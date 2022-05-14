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

    [Serializable]
    public class AntiBullet
    {
        public bool Enable;
        public AntiBulletData Data;

        public int Delay;

        public AntiBullet(AntiBulletData data)
        {
            this.Enable = data.Enable;
            this.Data = data;
            this.Delay = 0;
        }

        public bool IsBusy()
        {
            return --this.Delay > 0;
        }

        public void CoolDown()
        {
            this.Delay = this.Data.Rate;
        }

        public override string ToString()
        {
            return string.Format("{{\"Enable\":{0}, \"Delay\":{1}, \"Data\":{2}}}",
                Enable, Delay, Data
            );
        }
    }


    [Serializable]
    public class AntiBulletData
    {
        public bool Enable;
        public bool OneShotOneKill;
        public bool Harmless;
        public bool Self;
        public bool ForPassengers;
        public bool ScanAll;
        public int Range;
        public int EliteRange;
        public int Rate;


        public AntiBulletData(bool enable)
        {
            this.Enable = enable;
            this.OneShotOneKill = true;
            this.Harmless = false;
            this.Self = true;
            this.ForPassengers = false;
            this.ScanAll = false;
            this.Range = 0;
            this.EliteRange = this.Range;
            this.Rate = 0;
        }

        public override string ToString()
        {
            return string.Format("{{\"Enable\":{0}, \"OneShotOneKill\":{1}, \"Harmless\":{2}, \"Self\":{3}, \"ForPassengers\":{4}, \"ScanAll\":{5}, \"Range\":{6}, \"EliteRange\":{7}, \"Rate\":{8}}}",
                Enable, OneShotOneKill, Harmless, Self, ForPassengers, ScanAll, Range, EliteRange, Rate
            );
        }

    }


    public partial class TechnoExt
    {

        public AntiBullet antiBullet;

        public unsafe void TechnoClass_Init_AntiBullet()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (null != Type.AntiBulletData && Type.AntiBulletData.Enable && null == antiBullet)
            {
                ref AntiBulletData antiBulletData = ref Type.AntiBulletData;
                bool disable = false;
                // init anti-bullet data
                if (antiBulletData.Range <= 0)
                {
                    // WWSB only can use Primary weapon, find weapon range.
                    Pointer<WeaponTypeClass> pPrimary = pTechno.Ref.Type.Ref.get_Primary();
                    if (null == pPrimary || pPrimary.IsNull
                        || null == pPrimary.Ref.Projectile || pPrimary.Ref.Projectile.IsNull)
                    {
                        Logger.LogWarning("单位{0}启用了反抛射体，没有主武器，强制关闭", pTechno.Ref.Type.Ref.Base.Base.ID);
                        disable = true;
                    }
                    else
                    {
                        if (antiBulletData.Self && !pPrimary.Ref.Projectile.Ref.AA)
                        {
                            Logger.LogWarning("单位{0}启用了反抛射体，主武器不能对空，强制关闭", pTechno.Ref.Type.Ref.Base.Base.ID);
                            disable = true;
                        }
                        else
                        {
                            antiBulletData.Range = pPrimary.Ref.Range;
                            if (antiBulletData.EliteRange <= 0)
                            {
                                Pointer<WeaponTypeClass> pElite = pTechno.Ref.Type.Ref.get_ElitePrimary();
                                if (null != pElite && !pElite.IsNull)
                                    antiBulletData.EliteRange = pElite.Ref.Range;
                                else
                                    antiBulletData.EliteRange = antiBulletData.Range;
                            }
                        }
                    }
                }
                if (disable)
                {
                    antiBulletData.Enable = false;
                }
                else
                {
                    antiBullet = new AntiBullet(Type.AntiBulletData);

                    OnUpdateAction += TechnoClass_Update_AntiBullet;
                }
            }
        }

        public unsafe void TechnoClass_Update_AntiBullet()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            TechnoTypeExt extType = Type;
            Pointer<HouseClass> pHouse = pTechno.Ref.Owner;

            if (!antiBullet.IsBusy())
            {

                ExHelper.FindBulletTargetHouse(pTechno, (pBullet) =>
                {
                    if (antiBullet.Data.ScanAll || pBullet.Ref.Target == pTechno.Convert<AbstractClass>())
                    {
                            // Scan Target
                        int scanRange = antiBullet.Data.Range;
                        if (pTechno.Ref.Veterancy.IsElite())
                            scanRange = antiBullet.Data.EliteRange;

                        if (pTechno.Ref.Base.DistanceFrom(pBullet.Convert<ObjectClass>()) <= scanRange)
                        {
                            antiBullet.CoolDown();
                            if (antiBullet.Data.ForPassengers)
                            {
                                pTechno.Ref.SetTargetForPassengers(pBullet.Convert<AbstractClass>());
                            }

                            if (antiBullet.Data.Self && (pTechno.Ref.Target.IsNull || pTechno.Ref.Target.Ref.IsDead()))
                            {
                                pTechno.Ref.SetTarget(pBullet.Convert<AbstractClass>());
                            }

                            return true;
                        }
                    }
                    return false;
                });
            }
        }

    }

    public partial class TechnoTypeExt
    {
        public AntiBulletData AntiBulletData;

        /// <summary>
        /// [TechnoType]
        /// AntiMissile.Enable=yes
        /// AntiMissile.OneShotOneKill=yes
        /// AntiMissile.Harmless=no
        /// AntiMissile.Self=yes
        /// AntiMissile.ForPassengers=no
        /// AntiMissile.ScanAll=no
        /// AntiMissile.Range=0
        /// AntiMissile.EliteRange=0
        /// AntiMissile.Rate=15
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        private void ReadAntiBullet(INIReader reader, string section)
        {
            bool enable = false;
            if (reader.ReadNormal(section, "AntiMissile.Enable", ref enable) && enable)
            {
                AntiBulletData = new AntiBulletData(enable);

                bool oneShot = false;
                if (reader.ReadNormal(section, "AntiMissile.OneShotOneKill", ref oneShot))
                {
                    AntiBulletData.OneShotOneKill = oneShot;
                }

                bool harmless = false;
                if (reader.ReadNormal(section, "AntiMissile.Harmless", ref harmless))
                {
                    AntiBulletData.Harmless = harmless;
                }

                bool self = true;
                if (reader.ReadNormal(section, "AntiMissile.Self", ref self))
                {
                    AntiBulletData.Self = self;
                }

                bool forPassengers = false;
                if (reader.ReadNormal(section, "AntiMissile.ForPassengers", ref forPassengers))
                {
                    AntiBulletData.ForPassengers = forPassengers;
                }

                bool scanAll = false;
                if (reader.ReadNormal(section, "AntiMissile.ScanAll", ref scanAll))
                {
                    AntiBulletData.ScanAll = scanAll;
                }

                int range = 0;
                if (reader.ReadNormal(section, "AntiMissile.Range", ref range))
                {
                    AntiBulletData.Range = range * 256;
                }

                int eliteRange = 0;
                if (reader.ReadNormal(section, "AntiMissile.EliteRange", ref eliteRange))
                {
                    AntiBulletData.EliteRange = eliteRange * 256;
                }

                int rate = 0;
                if (reader.ReadNormal(section, "AntiMissile.Rate", ref rate))
                {
                    AntiBulletData.Rate = rate;
                }
            }

        }

    }

    [Serializable]
    public class BulletLifeStatus
    {
        public bool Interceptable; // 可被伤害

        public int Health; // 血量
        public bool IsDetonate; // 已损毁
        public bool IsHarmless; // 和平处置
        public bool SkipAE; // 爆炸不赋予AE


        public BulletLifeStatus(int health, bool interceptable = false)
        {
            this.Interceptable = interceptable;

            this.Health = health;
            this.IsDetonate = false;
            this.IsHarmless = false;
            this.SkipAE = false;
        }

        public void Detonate(bool harmless, bool skipAE = false)
        {
            this.Health = -1;
            this.IsDetonate = true;
            this.IsHarmless = harmless;
            this.SkipAE = skipAE;
        }

        public void TakeDamage(int damage, bool harmless, bool skipAE = false)
        {
            this.Health -= damage;
            this.IsDetonate = this.Health <= 0;
            this.IsHarmless = harmless;
            if (IsDetonate)
            {
                this.SkipAE = skipAE;
            }
        }

        public override string ToString()
        {
            return string.Format("{{\"Interceptable\":{0}, \"Health\":{1}, \"IsDetonate\":{2}, \"IsHarmless\":{3}, \"SkipAE\":{4}}}",
                Interceptable, Health, IsDetonate, IsHarmless, SkipAE
            );
        }
    }

    [Serializable]
    public class BulletDamageStatus
    {
        public int Damage; // 伤害
        public bool Eliminate; // 一击必杀
        public bool Harmless; // 和平处置

        public BulletDamageStatus(int damage)
        {
            this.Damage = damage;
            this.Eliminate = true;
            this.Harmless = false;
        }

        public override string ToString()
        {
            return string.Format("{{\"Damage\":{0}, \"Eliminate\":{1}, \"Harmless\":{2}}}", Damage, Eliminate, Harmless);
        }

    }


    public partial class BulletExt
    {

        public BulletLifeStatus BulletLifeStatus;
        public BulletDamageStatus BulletDamageStatus;

        public unsafe void BulletClass_Put_AntiBullet(Pointer<CoordStruct> pCoord)
        {
            // Logger.Log("抛射体{0}布置在地图上", OwnerObject);
            int health = OwnerObject.Ref.Base.Health;
            // 抛射体武器伤害为负数或者零时的特殊处理
            if (health < 0)
            {
                health = -health;
            }
            else if (health == 0)
            {
                health = 1; // 武器伤害为0，如[NukeCarrier]
            }
            CoordStruct location = OwnerObject.Ref.Base.Location;
            Pointer<TechnoClass> pTechno = OwnerObject.Ref.Owner;
            // 初始化抛射体的生命信息
            if (null == BulletLifeStatus)
            {
                BulletLifeStatus = new BulletLifeStatus(health, Type.Interceptable);
                // Logger.Log("初始化抛射体{0}生存属性{1}", OwnerObject, BulletDamageStatus);
            }

            // 初始化抛射体的伤害信息
            if (null == BulletDamageStatus)
            {
                BulletDamageStatus = new BulletDamageStatus(health);
                if (null != pTechno && !pTechno.IsNull)
                {
                    TechnoExt extOwner = TechnoExt.ExtMap.Find(pTechno);
                    if (null != extOwner && null != extOwner.antiBullet)
                    {
                        BulletDamageStatus.Eliminate = extOwner.antiBullet.Data.OneShotOneKill;
                        BulletDamageStatus.Harmless = extOwner.antiBullet.Data.Harmless;
                    }
                }
                // Logger.Log("初始化抛射体{0}攻击属性{1}", OwnerObject, BulletDamageStatus);
            }

        }

        public unsafe void BulletClass_Update_AntiBullet()
        {
            Pointer<BulletClass> pBullet = OwnerObject;


            // 检查抛射体是否已经被摧毁
            if (null != BulletLifeStatus)
            {
                if (BulletLifeStatus.IsDetonate)
                {
                    // Logger.Log("抛射体{0}死亡, {1}", OwnerObject, BulletLifeStatus);
                    if (!BulletLifeStatus.IsHarmless)
                    {
                        CoordStruct location = OwnerObject.Ref.Base.Base.GetCoords();
                        pBullet.Ref.Detonate(location);
                    }
                    pBullet.Ref.Base.Remove();
                    pBullet.Ref.Base.UnInit();
                    // Logger.Log("抛射体{0}注销", OwnerObject);
                    return;
                }
                // 检查抛射体存活
                if (BulletLifeStatus.Health <= 0)
                {
                    BulletLifeStatus.IsDetonate = true;
                    return;
                }
            }
        }

        public void TakeDamage(BulletDamageStatus damageStatus, bool Interceptable = false)
        {
            if (null != damageStatus && null != BulletLifeStatus && (Interceptable || BulletLifeStatus.Interceptable))
            {
                // Logger.Log("抛射体{0}收到伤害{1}", OwnerObject, damageStatus);
                if (damageStatus.Eliminate)
                {
                    BulletLifeStatus.Detonate(damageStatus.Harmless);
                }
                else
                {
                    BulletLifeStatus.TakeDamage(damageStatus.Damage, damageStatus.Harmless);
                }
            }
        }

        public void BulletClass_Detonate_AntiBullet(CoordStruct location)
        {
            // 检查抛射体是否命中了其他抛射体，并对其造成伤害
            Pointer<BulletClass> pBullet = OwnerObject;
            Pointer<AbstractClass> pTarget = pBullet.Ref.Target;
            if (!pTarget.IsNull && AbstractType.Bullet == pTarget.Ref.WhatAmI())
            {
                // Logger.Log("抛射体{0}引爆自身，目标是{1}", OwnerObject, pTarget);
                Pointer<BulletClass> pTargetBullet = pTarget.Convert<BulletClass>();
                BulletExt targetExt = BulletExt.ExtMap.Find(pTargetBullet);
                if (null != targetExt.BulletLifeStatus && null != BulletDamageStatus)
                {
                    // Logger.Log("抛射体{0}的目标{1}可以被摧毁，伤害{2}", OwnerObject, pTarget, BulletDamageStatus);
                    // 目标可以被摧毁
                    if (pBullet.Ref.Type.Ref.Inviso || location.DistanceFrom(pTarget.Ref.GetCoords()) <= pBullet.Ref.Type.Ref.Arm)
                    {
                        targetExt.TakeDamage(BulletDamageStatus);
                    }
                }

            }
        }

    }

    public partial class BulletTypeExt
    {

        public bool Interceptable = false;

        /// <summary>
        /// [ProjectileType]
        /// Interceptable=yes
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        private void ReadAntiBullet(INIReader reader, string section)
        {

            // Anti-Bullet
            bool interceptable = false;
            if (reader.ReadNormal(section, "Interceptable", ref interceptable))
            {
                Interceptable = interceptable;
            }
        }
    }
}

