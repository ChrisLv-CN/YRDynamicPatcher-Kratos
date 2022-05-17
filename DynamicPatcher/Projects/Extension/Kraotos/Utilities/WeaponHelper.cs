using System.Drawing;
using System.Threading;
using PatcherYRpp;
using PatcherYRpp.FileFormats;
using PatcherYRpp.Utilities;
using Extension;
using Extension.Utilities;
using DynamicPatcher;
using System;
using System.Collections.Generic;
using System.Reflection;
using Extension.Ext;

namespace Extension.Utilities
{


    public static partial class ExHelper
    {
        
        public static unsafe BulletVelocity GetBulletVelocity(CoordStruct sourcePos, CoordStruct targetPos)
        {
            CoordStruct bulletFLH = new CoordStruct(1, 0, 0);
            DirStruct bulletDir = ExHelper.Point2Dir(sourcePos, targetPos);
            SingleVector3D bulletV = ExHelper.GetFLHAbsoluteOffset(bulletFLH, bulletDir, default);
            return bulletV.ToBulletVelocity();
        }

        public static unsafe void FireWeaponTo(Pointer<TechnoClass> pShooter, Pointer<TechnoClass> pAttacker, Pointer<AbstractClass> pTarget,
            Pointer<WeaponTypeClass> pWeapon, CoordStruct flh,
            FireBulletToTarget callback = null, CoordStruct bulletSourcePos = default, bool radialFire = false, int splitAngle = 180)
        {
            if (pTarget.IsNull)
            {
                return;
            }
            CoordStruct targetPos = pTarget.Ref.GetCoords();
            // radial fire
            int burst = pWeapon.Ref.Burst;
            RadialFireHelper radialFireHelper = new RadialFireHelper(pShooter, burst, splitAngle);
            int flipY = -1;
            for (int i = 0; i < burst; i++)
            {
                BulletVelocity bulletVelocity = default;
                if (radialFire)
                {
                    flipY = (i < burst / 2f) ? -1 : 1;
                    bulletVelocity = radialFireHelper.GetBulletVelocity(i);
                }
                else
                {
                    flipY *= -1;
                }
                CoordStruct sourcePos = bulletSourcePos;
                if (default == bulletSourcePos)
                {
                    // get flh
                    sourcePos = GetFLHAbsoluteCoords(pShooter, flh, true, flipY, default);
                }
                if (default == bulletVelocity)
                {
                    bulletVelocity = GetBulletVelocity(sourcePos, targetPos);
                }
                Pointer<BulletClass> pBullet = FireBulletTo(pShooter, pAttacker, pTarget, pWeapon, sourcePos, targetPos, bulletVelocity);

                // Logger.Log("发射{0}，抛射体{1}，回调{2}", pWeapon.Ref.Base.ID, pBullet.IsNull ? " is null" : pBullet.Ref.Type.Ref.Base.Base.ID, null == callback);
                if (null != callback && !pBullet.IsNull)
                {
                    callback(i, burst, pBullet, pTarget);
                }
            }
        }

        public static unsafe Pointer<BulletClass> FireBulletTo(Pointer<TechnoClass> pShooter, Pointer<TechnoClass> pAttacker, Pointer<AbstractClass> pTarget,
            Pointer<WeaponTypeClass> pWeapon,
            CoordStruct sourcePos, CoordStruct targetPos,
            BulletVelocity bulletVelocity)
        {
            if (pTarget.IsNull || (pTarget.CastToTechno(out Pointer<TechnoClass> pTechno) && !pTechno.Ref.Base.IsAlive))
            {
                return IntPtr.Zero;
            }
            // Fire weapon
            Pointer<BulletClass> pBullet = FireBullet(pAttacker, pTarget, pWeapon, sourcePos, targetPos, bulletVelocity);
            // Draw bullet effect
            DrawBulletEffect(pWeapon, sourcePos, targetPos, pAttacker, pTarget);
            // Draw particle system
            AttachedParticleSystem(pWeapon, sourcePos, pTarget, pAttacker, targetPos);
            // Play report sound
            PlayReportSound(pWeapon, sourcePos);
            // Draw weapon anim
            DrawWeaponAnim(pShooter, pWeapon, sourcePos, targetPos);
            return pBullet;
        }

        private static unsafe Pointer<BulletClass> FireBullet(Pointer<TechnoClass> pAttacker, Pointer<AbstractClass> pTarget,
            Pointer<WeaponTypeClass> pWeapon,
            CoordStruct sourcePos, CoordStruct targetPos,
            BulletVelocity bulletVelocity)
        {
            double fireMult = 1;
            if (!pAttacker.IsNull && pAttacker.Ref.Base.IsAlive)
            {
                // check spawner
                Pointer<SpawnManagerClass> pSpawn = pAttacker.Ref.SpawnManager;
                if (pWeapon.Ref.Spawner && !pSpawn.IsNull)
                {
                    pSpawn.Ref.SetTarget(pTarget);
                    return Pointer<BulletClass>.Zero;
                }

                // check Abilities FIREPOWER
                fireMult = pAttacker.GetDamageMult(); //GetDamageMult(pAttacker);
            }
            int damage = (int)(pWeapon.Ref.Damage * fireMult);
            Pointer<WarheadTypeClass> pWH = pWeapon.Ref.Warhead;
            int speed = pWeapon.Ref.Speed;
            bool bright = pWeapon.Ref.Bright; // 原游戏中弹头上的bright是无效的

            Pointer<BulletClass> pBullet = IntPtr.Zero;
            // 自己不能发射武器朝向自己
            // Pointer<TechnoClass> pRealAttacker = pTarget.Value != pAttacker.Value ? pAttacker : IntPtr.Zero;
            pBullet = pWeapon.Ref.Projectile.Ref.CreateBullet(pTarget, pAttacker, damage, pWH, speed, bright);
            pBullet.Ref.WeaponType = pWeapon;
            // Logger.Log("{0}发射武器{1}，创建抛射体，目标类型{2}", pAttacker, pWeapon.Ref.Base.ID, pTarget.Ref.WhatAmI());

            // pBullet.Ref.SetTarget(pTarget);
            pBullet.Ref.MoveTo(sourcePos, bulletVelocity);
            if (pWeapon.Ref.Projectile.Ref.Inviso && !pWeapon.Ref.Projectile.Ref.Airburst)
            {
                pBullet.Ref.Detonate(targetPos);
                pBullet.Ref.Base.UnInit();
            }
            return pBullet;
        }

        private static unsafe void DrawBulletEffect(Pointer<WeaponTypeClass> pWeapon, CoordStruct sourcePos, CoordStruct targetPos, Pointer<TechnoClass> pAttacker, Pointer<AbstractClass> pTarget)
        {
            // IsLaser
            if (pWeapon.Ref.IsLaser)
            {
                LaserType laserType = new LaserType(false);
                ColorStruct houseColor = default;
                if (pWeapon.Ref.IsHouseColor && !pAttacker.IsNull)
                {
                    houseColor = pAttacker.Ref.Owner.Ref.LaserColor;
                }
                laserType.InnerColor = pWeapon.Ref.LaserInnerColor;
                laserType.OuterColor = pWeapon.Ref.LaserOuterColor;
                laserType.OuterSpread = pWeapon.Ref.LaserOuterSpread;
                laserType.IsHouseColor = pWeapon.Ref.IsHouseColor;
                laserType.Duration = pWeapon.Ref.LaserDuration;
                // get thickness and fade
                WeaponTypeExt ext = WeaponTypeExt.ExtMap.Find(pWeapon);
                if (null != ext)
                {
                    if (ext.LaserThickness > 0)
                    {
                        laserType.Thickness = ext.LaserThickness;
                    }
                    laserType.Fade = ext.LaserFade;
                    laserType.IsSupported = ext.IsSupported;
                }
                BulletEffectHelper.DrawLine(sourcePos, targetPos, laserType, houseColor);
            }
            // IsRadBeam
            if (pWeapon.Ref.IsRadBeam)
            {
                RadBeamType radBeamType = RadBeamType.RadBeam;
                if (!pWeapon.Ref.Warhead.IsNull && pWeapon.Ref.Warhead.Ref.Temporal)
                {
                    radBeamType = RadBeamType.Temporal;
                }
                BeamType beamType = new BeamType(radBeamType);
                BulletEffectHelper.DrawBeam(sourcePos, targetPos, beamType);
                // RadBeamType beamType = RadBeamType.RadBeam;
                // ColorStruct beamColor = RulesClass.Global().RadColor;
                // if (!pWeapon.Ref.Warhead.IsNull && pWeapon.Ref.Warhead.Ref.Temporal)
                // {
                //     beamType = RadBeamType.Temporal;
                //     beamColor = RulesClass.Global().ChronoBeamColor;
                // }
                // Pointer<RadBeam> pRadBeam = RadBeam.Allocate(beamType);
                // if (!pRadBeam.IsNull)
                // {
                //     pRadBeam.Ref.SetCoordsSource(sourcePos);
                //     pRadBeam.Ref.SetCoordsTarget(targetPos);
                //     pRadBeam.Ref.Color = beamColor;
                //     pRadBeam.Ref.Period = 15;
                //     pRadBeam.Ref.Amplitude = 40.0;
                // }
            }
            //IsElectricBolt
            if (pWeapon.Ref.IsElectricBolt)
            {
                if (!pAttacker.IsNull && !pTarget.IsNull)
                {
                    BulletEffectHelper.DrawBolt(pAttacker, pTarget, pWeapon, sourcePos);
                }
                else
                {
                    BulletEffectHelper.DrawBolt(sourcePos, targetPos, pWeapon.Ref.IsAlternateColor);
                }
            }
        }

        private static unsafe void AttachedParticleSystem(Pointer<WeaponTypeClass> pWeapon, CoordStruct sourcePos, Pointer<AbstractClass> pTarget, Pointer<TechnoClass> pAttacker, CoordStruct targetPos)
        {
            //ParticleSystem
            Pointer<ParticleSystemTypeClass> psType = pWeapon.Ref.AttachedParticleSystem;
            if (!psType.IsNull)
            {
                BulletEffectHelper.DrawParticele(psType, sourcePos, pTarget, pAttacker, targetPos);
            }
        }

        private static unsafe void PlayReportSound(Pointer<WeaponTypeClass> pWeapon, CoordStruct sourcePos)
        {
            if (pWeapon.Ref.Report.Count > 0)
            {
                int index = MathEx.Random.Next(0, pWeapon.Ref.Report.Count - 1);
                int soundIndex = pWeapon.Ref.Report.Get(index);
                if (soundIndex != -1)
                {
                    VocClass.PlayAt(soundIndex, sourcePos, IntPtr.Zero);
                }
            }
        }

        private static unsafe void DrawWeaponAnim(Pointer<TechnoClass> pShooter, Pointer<WeaponTypeClass> pWeapon, CoordStruct sourcePos, CoordStruct targetPos)
        {
            // Anim
            if (pWeapon.Ref.Anim.Count > 0)
            {
                int facing = pWeapon.Ref.Anim.Count;
                int index = 0;
                if (facing % 8 == 0)
                {
                    index = Dir2FacingIndex(Point2Dir(sourcePos, targetPos), facing);
                    index = (int)(facing / 8) + index;
                    if (index >= facing)
                    {
                        index = 0;
                    }
                }
                Pointer<AnimTypeClass> pAnimType = pWeapon.Ref.Anim.Get(index);
                // Logger.Log("获取到动画{0}", pAnimType.IsNull ? "不存在" : pAnimType.Convert<AbstractTypeClass>().Ref.ID);
                if (!pAnimType.IsNull)
                {
                    Pointer<AnimClass> pAnim = YRMemory.Create<AnimClass>(pAnimType, sourcePos);
                    pAnim.Ref.SetOwnerObject(pShooter.Convert<ObjectClass>());
                }
            }
        }


    }

}