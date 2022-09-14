using System.Drawing;
using System.Threading;
using System.IO;
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


    public partial class AnimExt
    {

        private TimerStruct weaponDelay;
        private bool initDelayFlag = false;

        public SwizzleablePointer<TechnoClass> Creater = new SwizzleablePointer<TechnoClass>(IntPtr.Zero);
        public bool CreaterIsDeadth = false;

        public void AnimClass_Update_Damage()
        {
            if (!initDelayFlag)
            {
                initDelayFlag = true;
                weaponDelay.Start(Type.InitDelay);
            }
            if (!CreaterIsDeadth)
            {
                if (Creater.IsNull)
                {
                    if (Type.KillByCreater && !OwnerObject.Ref.OwnerObject.IsNull && OwnerObject.Ref.OwnerObject.CastToTechno(out Pointer<TechnoClass> pTechno) && !pTechno.IsDead())
                    {
                        Creater.Pointer = pTechno;
                    }
                    else
                    {
                        CreaterIsDeadth = true;
                    }
                }
                else if (Creater.Pointer.IsDead())
                {
                    Creater.Pointer = IntPtr.Zero;
                    CreaterIsDeadth = true;
                }
            }
        }

        public void Explosion_Damage(bool isBounce = false, bool bright = false)
        {
            Pointer<AnimClass> pAnim = OwnerObject.Convert<AnimClass>();
            Pointer<AnimTypeClass> pAnimType = pAnim.Ref.Type;
            if (!pAnimType.IsNull)
            {
                CoordStruct location = pAnim.Ref.Base.Base.GetCoords();
                if (isBounce)
                {
                    location = pAnim.Ref.Bounce.GetCoords();
                }
                AnimTypeExt typeExt = AnimTypeExt.ExtMap.Find(pAnimType);
                int damage = (int)pAnimType.Ref.Damage;
                if (damage != 0)
                {
                    // 制造伤害
                    string weaponType = typeExt.Ares.Weapon;
                    Pointer<WarheadTypeClass> pWH = pAnimType.Ref.Warhead;
                    // 检查动画类型有没有写弹头
                    if (!string.IsNullOrEmpty(weaponType) && "none" != weaponType.ToLower())
                    {
                        // 用武器
                        Pointer<WeaponTypeClass> pWeapon = WeaponTypeClass.ABSTRACTTYPE_ARRAY.Find(weaponType);
                        if (!pWeapon.IsNull)
                        {
                            if (weaponDelay.Expired())
                            {
                                // Logger.Log($"{Game.CurrentFrame} - 动画 {pAnim} [{pAnimType.Ref.Base.Base.ID}] 用武器播放伤害 TypeDamage = {damage}, AnimDamage = {pAnim.Ref.Damage}, Weapon = {weaponType}");
                                pWH = pWeapon.Ref.Warhead;
                                bool isBright = bright || pWeapon.Ref.Bright; // 原游戏中弹头上的bright是无效的
                                Pointer<BulletTypeClass> pBulletType = pWeapon.Ref.Projectile;
                                Pointer<BulletClass> pBullet = pBulletType.Ref.CreateBullet(IntPtr.Zero, Creater, damage, pWH, pWeapon.Ref.Speed, isBright);
                                pBullet.Ref.WeaponType = pWeapon;
                                BulletExt ext = BulletExt.ExtMap.Find(pBullet);
                                ext.pSourceHouse = pAnim.Ref.Owner;
                                pBullet.Ref.Detonate(location);
                                pBullet.Ref.Base.UnInit();
                                weaponDelay.Start(typeExt.Ares.WeaponDelay);
                            }
                        }
                    }
                    else if (!pWH.IsNull)
                    {
                        // 用弹头
                        if (weaponDelay.Expired())
                        {
                            // Logger.Log($"{Game.CurrentFrame} - 动画 {pAnim} [{pAnimType.Ref.Base.Base.ID}] 用弹头播放伤害 TypeDamage = {damage}, AnimDamage = {pAnim.Ref.Damage}, Warhead = {pAnimType.Ref.Warhead}");
                            MapClass.DamageArea(location, damage, Creater, pWH, true, pAnim.Ref.Owner);
                            weaponDelay.Start(typeExt.Ares.WeaponDelay);
                            if (bright)
                            {
                                MapClass.FlashbangWarheadAt(damage, pWH, location);
                            }
                            // 播放弹头动画
                            if (Type.PlayWarheadAnim)
                            {
                                LandType landType = LandType.Clear;
                                if (MapClass.Instance.TryGetCellAt(location, out Pointer<CellClass> pCell))
                                {
                                    landType = pCell.Ref.LandType;
                                }
                                Pointer<AnimTypeClass> pWHAnimType = MapClass.SelectDamageAnimation(damage, pWH, landType, location);
                                // Logger.Log($"{Game.CurrentFrame} - Anim {pAnim} [{pAnimType.Ref.Base.Base.ID}] play warhead's Anim [{(pWHAnimType.IsNull ? "null" : pWHAnimType.Ref.Base.Base.ID)}]");
                                if (!pWHAnimType.IsNull)
                                {
                                    Pointer<AnimClass> pWHAnim = YRMemory.Create<AnimClass>(pWHAnimType, location);
                                    pWHAnim.Ref.Owner = pAnim.Ref.Owner;
                                }
                            }
                        }
                    }

                }

            }
        }


    }

    public partial class AnimTypeExt
    {
        public bool PlayWarheadAnim;
        public int InitDelay;
        public bool KillByCreater;

        private void ReadAnimDamage(INIReader reader, string section)
        {

            bool play = false;
            if (reader.ReadNormal(section, "Warhead.PlayAnim", ref play))
            {
                PlayWarheadAnim = play;
            }

            int delay = 1;
            if (reader.ReadNormal(section, "Damage.Delay", ref delay))
            {
                InitDelay = delay;
            }

            int initDelay = 1;
            if (reader.ReadNormal(section, "Damage.InitDelay", ref initDelay))
            {
                InitDelay = initDelay;
            }

            bool killByCreater = false;
            if (reader.ReadNormal(section, "Damage.KillByCreater", ref killByCreater))
            {
                KillByCreater = killByCreater;
            }
        }

    }

    public partial class RulesExt
    {
        public bool AllowAnimDamageTakeOverByKratos = true;
        public bool PlayWarheadAnim = false;

        /// <summary>
        /// [CombatDamage]
        /// AllowDamageIfDebrisHitWater=yes ;允许碎片\流星落到水上时产生伤害
        /// AllowAnimDamageTakeOverByKratos=yes ;允许Kratos接管由动画产生的伤害机制，包括Ares的Weapon标签，产生的伤害和弹头动画获得所属传递
        /// 
        /// </summary>
        /// <param name="reader"></param>
        private void ReadAllowAnimDamageTakeOverByKratos(INIReader reader)
        {
            bool allowed = true;
            if (reader.ReadNormal(SectionCombatDamage, "AllowAnimDamageTakeOverByKratos", ref allowed))
            {
                AllowAnimDamageTakeOverByKratos = allowed;
            }
        }
    }

}