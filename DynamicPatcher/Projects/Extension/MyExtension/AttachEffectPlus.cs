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

    public partial class TechnoExt
    {
        public AttachEffectManager AttachEffectManager = new AttachEffectManager();
        public bool AttachEffectOnceFlag = false;


        public unsafe void TechnoClass_Init_AttachEffect()
        {
            // Init阶段，单位未成功生成在地图上，部分AE如Animation会出现异常
            // if (null != Type.AttachEffectData)
            // {
            //     AttachEffectManager.Attach(Type.AttachEffectData, OwnerObject.Convert<ObjectClass>(), OwnerObject.Ref.Owner, false);
            // }
        }

        public unsafe void TechnoClass_Put_AttachEffect(Pointer<CoordStruct> pCoord, Direction faceDir)
        {
            AttachEffectManager.Put(OwnerObject.Convert<ObjectClass>(), pCoord, faceDir);
        }

        public unsafe void TechnoClass_Remove_AttachEffect()
        {
            if (OwnerObject.Convert<AbstractClass>().Ref.WhatAmI() == AbstractType.Building)
            {
                AttachEffectManager.UnInitAll(OwnerObject.Ref.Base.Base.GetCoords());
            }
            else
            {
                AttachEffectManager.Remove(OwnerObject.Convert<ObjectClass>());
            }
        }

        public unsafe void TechnoClass_Update_AttachEffect()
        {
            // if (OwnerObject.IsNull || OwnerObject.Ref.Base.InLimbo || OwnerObject.Ref.IsImmobilized || !OwnerObject.Ref.Transporter.IsNull)
            // {
            //     return;
            // }
            if (null != Type.AttachEffectData && !IsDead && !OwnerObject.Ref.Base.InLimbo && !OwnerObject.Ref.IsImmobilized)
            {
                // 写在ini里的重复赋予
                AttachEffectManager.Attach(Type.AttachEffectData, OwnerObject.Convert<ObjectClass>(), OwnerObject.Ref.Owner, AttachEffectOnceFlag);
                AttachEffectOnceFlag = true;
            }
            AttachEffectManager.Update(OwnerObject.Convert<ObjectClass>(), IsDead);
        }

        public unsafe void TechnoClass_ReceiveDamage_AttachEffect(Pointer<int> pDamage, int distanceFromEpicenter, Pointer<WarheadTypeClass> pWH,
            Pointer<ObjectClass> pAttacker, bool ignoreDefenses, bool preventPassengerEscape, Pointer<HouseClass> pAttackingHouse,
            WarheadTypeExt whExt, int realDamage)
        {
            if (null != whExt && null != whExt.AttachEffectData)
            {
                AttachEffectData aeData = whExt.AttachEffectData;
                bool attached = false;
                if (null != aeData.AttachEffectTypes && aeData.AttachEffectTypes.Count > 0)
                {
                    foreach (string type in aeData.AttachEffectTypes)
                    {
                        // Logger.Log("{0} 收到来自 {1} 的伤害，附加AE {2}", OwnerObject.Ref.Type.Ref.Base.Base.ID, pAttacker.Ref.Type.Ref.Base.ID, type);
                        AttachEffectType aeType = AttachEffectType.FindOrAllocate(type, CCINIClass.INI_Rules) as AttachEffectType;
                        if (aeType.AttachWithDamage)
                        {
                            // 只有跟随伤害赋予的AE类型，才在ReceiveDamage事件中赋予
                            AttachEffect(aeType, pAttacker, pAttackingHouse);
                            attached = true;
                        }
                    }
                }
                if (attached && aeData.CabinLength > 0)
                {
                    AttachEffectManager.SetLocationSpace(aeData.CabinLength);
                }
            }
            // Logger.Log("Techno {0} Receive damage.", OwnerObject.IsNull ? "null" : OwnerObject.Ref.Type.Ref.Base.Base.ID);
            AttachEffectManager.ReceiveDamage(OwnerObject.Convert<ObjectClass>(), pDamage, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse);
        }

        public unsafe void AttachEffect(AttachEffectType aeType, Pointer<ObjectClass> pAttacker, Pointer<HouseClass> pAttackingHouse)
        {
            // 检查是否穿透铁幕
            if (!aeType.PenetratesIronCurtain && OwnerObject.Ref.Base.IsIronCurtained())
            {
                return;
            }
            Pointer<ObjectClass> pObject = OwnerObject.Convert<ObjectClass>();
            // 设定所属
            Pointer<HouseClass> pHouse = aeType.OwnerTarget ? OwnerObject.Ref.Owner : pAttackingHouse;
            if (!pAttacker.IsNull && pAttacker.Ref.IsAlive && pAttacker.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                // Logger.Log("{0} 附加AE {1}, 标记攻击者{2}", pObject.Ref.Type.Ref.Base.ID, aeType.Name, pTechno.Ref.Type.Ref.Base.Base.ID);
                // 乘客附加视为载具
                if (aeType.FromTransporter && !pTechno.Ref.Transporter.IsNull)
                {
                    AttachEffectManager.Attach(aeType, pObject, pHouse, pTechno.Ref.Transporter);
                }
                else
                {
                    AttachEffectManager.Attach(aeType, pObject, pHouse, pTechno);
                }
            }
            else
            {
                // Logger.Log("{0} 附加AE {1}", pObject.Ref.Type.Ref.Base.ID, aeType.Name);
                AttachEffectManager.Attach(aeType, pObject, pHouse, IntPtr.Zero);
            }
        }

        public unsafe void TechnoClass_Destroy_AttachEffect()
        {
            // Logger.Log("Techno {0} Destroy.", OwnerObject.IsNull ? "null" : OwnerObject.Ref.Type.Ref.Base.Base.ID);
            AttachEffectManager.DestroyAll(OwnerObject.Convert<ObjectClass>());
        }

        public unsafe void TechnoClass_UnInit_AttachEffect()
        {
            AttachEffectManager.UnInitAll(LastLocation);
        }

        public unsafe void TechnoClass_StopCommand_AttachEffect()
        {
            AttachEffectManager.StopCommand();
        }

    }

    public partial class TechnoTypeExt
    {
        public AttachEffectData AttachEffectData;

        /// <summary>
        /// [TechnoType]
        /// AttachEffectTypes=AutoWeapon0
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadAttachEffect(INIReader reader, string section)
        {
            if (AttachEffectManager.ReadAttachEffect(reader, section, out AttachEffectData aeData))
            {
                this.AttachEffectData = aeData;
            }
        }

    }

    public partial class BulletExt
    {
        public AttachEffectManager AttachEffectManager = new AttachEffectManager();

        public SwizzleablePointer<ObjectClass> FakeTarget = new SwizzleablePointer<ObjectClass>(IntPtr.Zero);


        public unsafe void BulletClass_Put_AttachEffect(Pointer<CoordStruct> pCoord)
        {
        }

        public unsafe void BulletClass_Update_AttachEffect()
        {
            if (null != Type.AttachEffectData)
            {
                // 写在ini中的重复赋予
                AttachEffectManager.Attach(Type.AttachEffectData, OwnerObject.Convert<ObjectClass>(), pSourceHouse, true);
            }
            AttachEffectManager.Update(OwnerObject.Convert<ObjectClass>(), false);
        }

        public unsafe void AttachEffect(AttachEffectType aeType, Pointer<ObjectClass> pAttacker, Pointer<HouseClass> pAttackingHouse)
        {
            Pointer<ObjectClass> pObject = OwnerObject.Convert<ObjectClass>();
            // 设定所属
            Pointer<HouseClass> pHouse = aeType.OwnerTarget ? pSourceHouse : pAttackingHouse;
            if (!pAttacker.IsNull && pAttacker.Ref.IsAlive && pAttacker.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                // Logger.Log("附加AE {0}, 标记攻击者{1}", aeType.Name, pTechno.Ref.Type.Ref.Base.Base.ID);
                // 乘客附加视为载具
                if (aeType.FromTransporter && !pTechno.Ref.Transporter.IsNull)
                {
                    AttachEffectManager.Attach(aeType, pObject, pHouse, pTechno.Ref.Transporter);
                }
                else
                {
                    AttachEffectManager.Attach(aeType, pObject, pHouse, pTechno);
                }
            }
            else
            {
                // Logger.Log("附加AE {0}", aeType.Name);
                AttachEffectManager.Attach(aeType, pObject, pHouse, IntPtr.Zero);
            }
        }

        public unsafe void BulletClass_Detonate_AttachEffect(Pointer<ObjectClass> pAttacker, TechnoExt targetExt, WarheadTypeExt whExt)
        {
            if (null != whExt && null != whExt.AttachEffectData)
            {
                AttachEffectData aeData = whExt.AttachEffectData;
                bool attached = false;
                if (null != aeData.AttachEffectTypes && aeData.AttachEffectTypes.Count > 0)
                {
                    foreach (string type in aeData.AttachEffectTypes)
                    {
                        // Logger.Log("Attach effect type {0}", type);
                        AttachEffectType aeType = AttachEffectType.FindOrAllocate(type, CCINIClass.INI_Rules) as AttachEffectType;
                        if (!aeType.AttachWithDamage && !aeType.OnlyAffectBullet)
                        {
                            // 只有不跟随伤害进行赋予的AE类型，才在Detonate事件中赋予
                            targetExt.AttachEffect(aeType, pAttacker, pSourceHouse);
                            attached = true;
                        }
                    }
                }
                if (attached && aeData.CabinLength > 0)
                {
                    targetExt.AttachEffectManager.SetLocationSpace(aeData.CabinLength);
                }
            }
        }

        public unsafe void BulletClass_Detonate_AttachEffect_Bullet(Pointer<ObjectClass> pAttacker, BulletExt targetExt, WarheadTypeExt whExt)
        {
            if (null != whExt && null != whExt.AttachEffectData)
            {
                AttachEffectData aeData = whExt.AttachEffectData;
                bool attached = false;
                if (null != aeData.AttachEffectTypes && aeData.AttachEffectTypes.Count > 0)
                {
                    foreach (string type in aeData.AttachEffectTypes)
                    {
                        // Logger.Log("Attach effect type {0}", type);
                        AttachEffectType aeType = AttachEffectType.FindOrAllocate(type, CCINIClass.INI_Rules) as AttachEffectType;
                        if (aeType.AffectBullet || aeType.OnlyAffectBullet)
                        {
                            // Logger.Log("检查抛射体类型，ROT = {0}, Level = {1}, Arcing = {2}, AffectTorpedo = {3}, AffectMissile = {4}, AffectCannon = {5}",
                            //     targetExt.OwnerObject.Ref.Type.Ref.ROT, targetExt.OwnerObject.Ref.Type.Ref.Level, targetExt.OwnerObject.Ref.Type.Ref.Arcing,
                            //     aeType.AffectTorpedo, aeType.AffectMissile, aeType.AffectCannon
                            // );
                            // 只有不跟随伤害进行赋予的AE类型，才在Detonate事件中赋予
                            // 检查类型
                            if (targetExt.OwnerObject.Ref.Type.Ref.ROT > 0)
                            {
                                if ((targetExt.OwnerObject.Ref.Type.Ref.Level && aeType.AffectTorpedo) || aeType.AffectMissile)
                                {
                                    targetExt.AttachEffect(aeType, pAttacker, pSourceHouse);
                                    attached = true;
                                }

                            }
                            else if (aeType.AffectCannon)
                            {
                                // ROT=0 视为 Arcing
                                targetExt.AttachEffect(aeType, pAttacker, pSourceHouse);
                                attached = true;
                            }
                        }
                    }
                }
                if (attached && aeData.CabinLength > 0)
                {
                    targetExt.AttachEffectManager.SetLocationSpace(aeData.CabinLength);
                }
            }
        }

        public unsafe void BulletClass_UnInit_AttachEffect()
        {
            AttachEffectManager.UnInitAll(OwnerObject.Ref.Base.Location);
            if (!FakeTarget.IsNull)
            {
                FakeTarget.Ref.UnInit();
            }
        }
    }


    public partial class BulletTypeExt
    {
        public AttachEffectData AttachEffectData;

        /// <summary>
        /// [TechnoType]
        /// AttachEffectTypes=AutoWeapon0
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadAttachEffect(INIReader reader, string section)
        {
            if (AttachEffectManager.ReadAttachEffect(reader, section, out AttachEffectData aeData))
            {
                this.AttachEffectData = aeData;
            }
        }

    }


    public partial class WarheadTypeExt
    {
        public AttachEffectData AttachEffectData;

        /// <summary>
        /// [WarheadType]
        /// AttachEffectTypes=AutoWeapon0
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadAttachEffect(INIReader reader, string section)
        {
            if (AttachEffectManager.ReadAttachEffect(reader, section, out AttachEffectData aeData))
            {
                this.AttachEffectData = aeData;
            }
        }

    }

}