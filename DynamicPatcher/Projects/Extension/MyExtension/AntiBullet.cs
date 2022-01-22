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
                        disable = true;
                    }
                    else
                    {
                        if (antiBulletData.Self && !pPrimary.Ref.Projectile.Ref.AA)
                        {
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
                }
            }
        }

        public unsafe void TechnoClass_Update_AntiBullet()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            TechnoTypeExt extType = Type;
            Pointer<HouseClass> pHouse = pTechno.Ref.Owner;

            if (null != antiBullet && antiBullet.Enable)
            {
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

                    /*
                    // find target
                    DynamicVectorClass<Pointer<BulletClass>> bullets = BulletClass.Array;
                    for (int i = 0; i < bullets.Count; i++)
                    {
                        Pointer<BulletClass> pBullet = bullets.Get(i);
                        BulletExt bulletExt = BulletExt.ExtMap.Find(pBullet);
                        if (null == bulletExt || !bulletExt.bulletLifeData.Interceptable
                            || pBullet.Ref.Type.Ref.Inviso == YES
                            || (null != pBullet.Ref.Owner && !pBullet.Ref.Owner.IsNull && pBullet.Ref.Owner.Ref.Owner == pHouse)
                            || (!antiBullet.Data.ScanAll && pBullet.Ref.Target != pTechno.Convert<AbstractClass>()))
                            continue;

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

                            break;
                        }
                    }
                    */
                }
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
        public bool Interceptable;

        public int Health;
        public bool IsDetonate;
        public bool IsHarmless;


        public BulletLifeStatus(int health, bool interceptable = false)
        {
            this.Interceptable = interceptable;

            this.Health = health;
            this.IsDetonate = false;
            this.IsHarmless = false;
        }

        public void Detonate(bool harmless)
        {
            this.Health = -1;
            this.IsDetonate = true;
            this.IsHarmless = harmless;
        }

        public void TakeDamage(int damage, bool harmless)
        {
            this.Health -= damage;
            this.IsDetonate = this.Health <= 0;
            this.IsHarmless = harmless;
        }

        public override string ToString()
        {
            return string.Format("{{\"Interceptable\":{0}, \"Health\":{1}, \"IsDetonate\":{2}, \"IsHarmless\":{3}}}",
                Interceptable, Health, IsDetonate, IsHarmless
            );
        }
    }

    [Serializable]
    public class BulletDamageStatus
    {
        public int Damage;
        public bool Eliminate;
        public bool Harmless;

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

        public unsafe void BulletClass_Update_AntiBullet()
        {
            Pointer<BulletClass> pBullet = OwnerObject;

            int health = pBullet.Ref.Base.Health;
            CoordStruct location = pBullet.Ref.Base.Location;
            Pointer<TechnoClass> pTechno = pBullet.Ref.Owner;

            // 初始化抛射体的生命信息
            if (null == BulletLifeStatus)
            {
                BulletLifeStatus = new BulletLifeStatus(health, Type.Interceptable);
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
            }

            // 检查抛射体是否已经被摧毁
            if (null != BulletLifeStatus)
            {
                if (BulletLifeStatus.IsDetonate)
                {
                    // Logger.Log("抛射体{0}死亡, {1}", OwnerObject, BulletLifeStatus);
                    if (!BulletLifeStatus.IsHarmless)
                    {
                        pBullet.Ref.Detonate(location);
                    }
                    pBullet.Ref.Base.Remove();
                    pBullet.Ref.Base.UnInit();
                    // Logger.Log("抛射体{0}注销", OwnerObject);
                    return;
                }
                if (BulletLifeStatus.Health <= 0)
                {
                    BulletLifeStatus.IsDetonate = true;
                    return;
                }
            }


            // 检查抛射体是否命中了其他抛射体，并对其造成伤害
            Pointer<AbstractClass> pTarget = pBullet.Ref.Target;
            if (!pTarget.IsNull && AbstractType.Bullet == pTarget.Ref.WhatAmI())
            {
                Pointer<BulletClass> pTargetBullet = pTarget.Convert<BulletClass>();
                BulletExt targetExt = BulletExt.ExtMap.Find(pTargetBullet);
                if (null != targetExt.BulletLifeStatus && null != BulletDamageStatus)
                {
                    // 目标可以被摧毁
                    if (pBullet.Ref.Type.Ref.Inviso || location.DistanceFrom(pTarget.Ref.GetCoords()) <= pBullet.Ref.Type.Ref.Arm)
                    {
                        targetExt.TakeDamage(BulletDamageStatus);
                    }
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

