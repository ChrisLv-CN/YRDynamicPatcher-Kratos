using DynamicPatcher;
using Extension.Ext;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    public partial class AttachEffect
    {
        public AutoWeapon AutoWeapon;

        private void InitAutoWeapon()
        {
            if (null != Type.AutoWeaponType)
            {
                this.AutoWeapon = Type.AutoWeaponType.CreateObject(Type);
            }
        }
    }


    [Serializable]
    public class AutoWeapon : AttachEffectBehaviour
    {
        public AutoWeaponType Type;
        public SwizzleablePointer<TechnoClass> pAttacker; // AE来源

        private Dictionary<string, TimerStruct> weaponsROF;

        private bool Active;

        public AutoWeapon(AutoWeaponType type, AttachEffectType attachEffectType) : base(attachEffectType)
        {
            this.Type = type;
            this.pAttacker = new SwizzleablePointer<TechnoClass>(IntPtr.Zero);
            this.weaponsROF = new Dictionary<string, TimerStruct>();
            this.Active = false;
        }

        public override void Enable(Pointer<ObjectClass> pObject, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker)
        {
            // 创建假想敌
            // if (pTarget.IsNull)
            // {
            //     Pointer<OverlayTypeClass> pOverlayType = OverlayTypeClass.ABSTRACTTYPE_ARRAY.Array[0];
            //     if (!pOverlayType.IsNull)
            //     {
            //         Pointer<ObjectClass> pTargetObject = pOverlayType.Ref.Base.CreateObject(pHouse);
            //         // Logger.Log("创建一个假想敌, {0}", pTargetObject);
            //         pTarget.Pointer = pTargetObject;
            //     }
            // }
            this.pAttacker.Pointer = pAttacker;
            this.Active = true;
        }

        public override void Disable(CoordStruct location)
        {
            this.Active = false;
        }

        public override bool IsAlive()
        {
            return this.Active;
        }

        public override void OnUpdate(Pointer<ObjectClass> pObject, bool isDead, AttachEffectManager manager)
        {
            if (!Active)
            {
                return;
            }
            if (isDead)
            {
                Disable(default);
                return;
            }

            int weaponIndex = -1;
            List<string> weaponTypes = Type.WeaponTypes;
            CoordStruct fireFLH = Type.FireFLH;
            CoordStruct targetFLH = Type.TargetFLH;
            double rofMult = 1;
            CoordStruct sourcePos = pObject.Ref.Location;
            CoordStruct targetPos = default;

            Pointer<TechnoClass> pTechno = IntPtr.Zero; // 附着的对象
            Pointer<HouseClass> pHouse = IntPtr.Zero;

            CoordStruct bulletSourcePos = default;

            bool isOnBullet = false;

            switch (pObject.Ref.Base.WhatAmI())
            {
                case AbstractType.Unit:
                case AbstractType.Aircraft:
                case AbstractType.Building:
                case AbstractType.Infantry:
                    // 以单位获得参考，取目标的位点
                    pTechno = pObject.Convert<TechnoClass>();
                    pHouse = pTechno.Ref.Owner;
                    weaponIndex = Type.WeaponIndex;
                    if (pTechno.Ref.Veterancy.IsElite())
                    {
                        weaponIndex = Type.EliteWeaponIndex;
                        weaponTypes = Type.EliteWeaponTypes;
                        fireFLH = Type.EliteFireFLH;
                        targetFLH = Type.EliteTargetFLH;
                    }
                    rofMult = ExHelper.GetROFMult(pTechno);
                    if (Type.IsOnWorld)
                    {
                        // 绑定世界坐标
                        DirStruct dir = new DirStruct();
                        bulletSourcePos = ExHelper.GetFLHAbsoluteCoords(sourcePos, fireFLH, dir);
                        targetPos = ExHelper.GetFLHAbsoluteCoords(sourcePos, targetFLH, dir);
                    }
                    else
                    {
                        // 绑定单位身上或炮塔
                        targetPos = ExHelper.GetFLHAbsoluteCoords(pTechno, targetFLH, Type.IsOnTurret);
                    }
                    break;
                case AbstractType.Bullet:
                    isOnBullet = true;
                    // 以抛射体作为参考，取抛射体当前位置和目标位置获得方向，按照方向获取发射位点和目标位点
                    Pointer<BulletClass> pBullet = pObject.Convert<BulletClass>();

                    pTechno = pBullet.Ref.Owner;
                    if (!pTechno.IsNull)
                    {
                        pHouse = pTechno.Ref.Owner;
                    }
                    else
                    {
                        BulletExt bulletExt = BulletExt.ExtMap.Find(pBullet);
                        if (null != bulletExt)
                        {
                            pHouse = bulletExt.pSourceHouse;
                        }
                    }

                    DirStruct bulletDir = new DirStruct();
                    if (!Type.IsOnWorld)
                    {
                        bulletDir = ExHelper.Point2Dir(pBullet.Ref.SourceCoords, pBullet.Ref.TargetCoords);
                    }
                    bulletSourcePos = ExHelper.GetFLHAbsoluteCoords(sourcePos, fireFLH, bulletDir);
                    targetPos = ExHelper.GetFLHAbsoluteCoords(sourcePos, targetFLH, bulletDir);
                    break;
                default:
                    return;
            }

            bool attackerInvisible = pAttacker.IsNull || pAttacker.Ref.Base.Health <= 0 || !pAttacker.Ref.Base.IsAlive || pAttacker.Ref.Base.InLimbo || pAttacker.Ref.IsImmobilized || !pAttacker.Ref.Transporter.IsNull;
            // 攻击者标记下，攻击者死亡或不存在或AE被赋予抛射体，AE结束
            if (Type.IsAttackerMark && (isOnBullet || attackerInvisible))
            {
                Disable(default);
                return;
            }

            // 进入发射流程
            bool weaponLaunch = false;
            // 发射武器是单位本身的武器还是自定义武器
            if (weaponIndex >= 0 && !isOnBullet)
            {
                // 发射单位自身的武器
                if (ReadyToFire(pTechno, pHouse, targetPos, out Pointer<TechnoClass> pShooter, out Pointer<TechnoClass> pWeaponOwner, out Pointer<ObjectClass> pTarget, out FireBulletToTarget callback))
                {
                    // Logger.Log("{0}朝{1}发射自身的武器{2}", pShooter.Ref.Type.Ref.Base.Base.ID, pTarget.Ref.Type.Ref.Base.ID, weaponIndex);
                    // 检查武器是否存在，是否ROF结束
                    Pointer<WeaponStruct> pWeaponStruct = pShooter.Ref.GetWeapon(weaponIndex);
                    if (!pWeaponStruct.IsNull && !pWeaponStruct.Ref.WeaponType.IsNull && CheckAndResetROF(pWeaponStruct.Ref.WeaponType, rofMult))
                    {
                        Logger.Log("{0}朝{1}发射自身的武器{2}", pShooter.Ref.Type.Ref.Base.Base.ID, pTarget.Ref.Type.Ref.Base.ID, weaponIndex);
                        // 射手发射武器，忽略武器所属
                        // pShooter.Ref.Fire_IgnoreType(pTarget.Convert<AbstractClass>(), weaponIndex);
                        TechnoExt ext = TechnoExt.ExtMap.Find(pShooter);
                        ext?.EnqueueDelayFireWeapon(weaponIndex, pTarget.Convert<AbstractClass>());
                        weaponLaunch = true;
                    }
                }
            }
            else if (null != weaponTypes && weaponTypes.Count > 0)
            {
                // 发射自定义的武器
                foreach (string weaponId in weaponTypes)
                {

                    if (!string.IsNullOrEmpty(weaponId) && !pTechno.IsNull)
                    {
                        // 进行ROF检查
                        Pointer<WeaponTypeClass> pWeapon = WeaponTypeClass.ABSTRACTTYPE_ARRAY.Find(weaponId);
                        if (!pWeapon.IsNull)
                        {
                            if (CheckAndResetROF(pWeapon, rofMult) && !pHouse.IsNull)
                            {
                                if (ReadyToFire(pTechno, pHouse, targetPos, out Pointer<TechnoClass> pShooter, out Pointer<TechnoClass> pWeaponOwner, out Pointer<ObjectClass> pTarget, out FireBulletToTarget callback))
                                {
                                    // 可以发射武器，发射自定义武器
                                    TechnoExt ext = TechnoExt.ExtMap.Find(pShooter);
                                    ext?.FireCustomWeapon(pShooter, pWeaponOwner, pTarget.Convert<AbstractClass>(), weaponId, fireFLH, bulletSourcePos, rofMult, callback);
                                    weaponLaunch = true;
                                }
                                else
                                {
                                    Disable(default);
                                    return;
                                }

                            }
                        }
                    }
                }
            }
            if (weaponLaunch && Type.FireOnce)
            {
                // 武器已成功发射，销毁AE
                Disable(default);
            }
        }

        public bool CheckAndResetROF(Pointer<WeaponTypeClass> pWeapon, double rofMult)
        {
            bool canFire = false;
            string weaponId = pWeapon.Ref.Base.ID;
            WeaponTypeExt typeExt = WeaponTypeExt.ExtMap.Find(pWeapon);
            if (null != typeExt)
            {
                AttachFireData fireData = typeExt.AttachFireData;
                // 进行ROF检查
                canFire = !fireData.UseROF;
                if (!canFire)
                {
                    // 本次发射的rof
                    int rof = (int)(pWeapon.Ref.ROF * rofMult);
                    if (weaponsROF.TryGetValue(weaponId, out TimerStruct rofTimer))
                    {
                        if (rofTimer.Expired())
                        {
                            canFire = true;
                            rofTimer.Start(rof);
                            weaponsROF[weaponId] = rofTimer;
                        }
                    }
                    else
                    {
                        canFire = true;
                        weaponsROF.Add(weaponId, new TimerStruct(rof));
                    }
                }
            }
            return canFire;
        }

        // 发射武器前设定攻击者和目标
        public bool ReadyToFire(Pointer<TechnoClass> pOwner, Pointer<HouseClass> pHouse, CoordStruct targetPos, out Pointer<TechnoClass> pShooter, out Pointer<TechnoClass> pWeaponOwner, out Pointer<ObjectClass> pTarget, out FireBulletToTarget callback)
        {
            // 默认情况下，由标记持有者朝向预设位置开火
            pShooter = pOwner;
            pWeaponOwner = pOwner;
            pTarget = IntPtr.Zero;
            callback = null;

            // 更改射手
            if (!Type.ReceiverAttack)
            {
                // IsAttackerMark=yes时ReceiverAttack和ReceiverOwnBullet默认值为no
                // 若无显式修改，此时应为攻击者朝AE附属对象进行攻击
                // 由攻击者开火，朝向AE附属对象进行攻击
                pShooter = pAttacker;
                pWeaponOwner = pAttacker;
                pTarget = pOwner.Convert<ObjectClass>();
                // Logger.Log("由攻击者{0}朝向持有者{1}开火", pShooter.Ref.Type.Ref.Base.Base.ID, pTarget.Ref.Type.Ref.Base.ID);
            }

            // 更改所属
            if (Type.ReceiverOwnBullet)
            {
                pWeaponOwner = pOwner;
            }
            else
            {
                pWeaponOwner = pAttacker;
                // Logger.Log("武器所属变更为攻击者{0}", pShooter.Ref.Type.Ref.Base.Base.ID);
            }

            // 设定目标
            if (Type.IsAttackerMark)
            {
                // IsAttackerMark=yes时ReceiverAttack和ReceiverOwnBullet默认值为no
                // 若无显式修改，此时应为攻击者朝AE附属对象进行攻击
                // 只有显式修改 ReceiverAttack时，说明是由AE附属对象朝向攻击者攻击
                // 修改目标为攻击者
                if (Type.ReceiverAttack)
                {
                    pTarget = pAttacker.Pointer.Convert<ObjectClass>();
                }
            }
            else
            {
                // 不是攻击标记，目标只能虚拟目标
                // 朝向一个目标点虚空开火
                // 创建假想敌
                Pointer<OverlayTypeClass> pOverlayType = OverlayTypeClass.ABSTRACTTYPE_ARRAY.Array[0];
                if (!pOverlayType.IsNull)
                {
                    pTarget = pOverlayType.Ref.Base.CreateObject(pHouse);
                    // Logger.Log("创建一个假想敌, {0}", pTarget.Ref.Base.WhatAmI());
                    pTarget.Ref.SetLocation(targetPos);
                    pTarget.Ref.InLimbo = false;
                    pTarget.Ref.IsVisible = false;
                    callback = this.FireBulletToTarget;
                }
                // Pointer<ObjectTypeClass> pObjectType = pTechno.Ref.Type.Convert<ObjectTypeClass>();
                // if (!pObjectType.IsNull)
                // {
                //     pTarget = pObjectType.Ref.CreateObject(pHouse);
                //     Logger.Log("创建一个假想敌, {0}", pTarget.Ref.Base.WhatAmI());
                //     pTarget.Ref.SetLocation(targetPos);
                //     callback = this.FireBulletToTarget;
                // }
            }
            return !pTarget.IsNull && pTarget.Ref.IsAlive;
        }


        // 将假想敌设置在抛射体扩展上，以便在抛射体注销时销毁假想敌
        public bool FireBulletToTarget(int index, int burst, Pointer<BulletClass> pBullet, Pointer<AbstractClass> pTarget)
        {
            BulletExt ext = BulletExt.ExtMap.Find(pBullet);
            if (null != ext && (index + 1) >= burst)
            {
                ext.FakeTarget.Pointer = pTarget.Convert<ObjectClass>();
            }
            return false;
        }

        public override void OnPut(Pointer<ObjectClass> pObject, Pointer<CoordStruct> pCoord, Direction faceDir)
        {
            this.Active = true;
        }

        public override void OnRemove(Pointer<ObjectClass> pObject)
        {
            Disable(default);
        }

        public override void OnDestroy(Pointer<ObjectClass> pObject)
        {
            Disable(default);
        }

    }

}