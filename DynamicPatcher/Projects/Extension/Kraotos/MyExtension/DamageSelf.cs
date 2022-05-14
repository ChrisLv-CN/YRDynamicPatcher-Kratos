using System.Data.Common;
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

        public DamageSelfState DamageSelfState => AttachEffectManager.DamageSelfState;

        public unsafe void TechnoClass_Put_DamageSelf(Pointer<CoordStruct> pCoord, short faceDirValue8)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (null != Type.DamageSelfData && Type.DamageSelfData.Enable)
            {
                DamageSelfState.Enable(Type.DamageSelfData);
            }
        }

        public unsafe void TechnoClass_Update_DamageSelf()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;

            if (DamageSelfState.CanHitSelf())
            {
                Pointer<HouseClass> pHouse = pTechno.Ref.Owner;
                // 检查平民
                if (!DamageSelfState.Data.DeactiveWhenCivilian || !pHouse.IsCivilian())
                {
                    DamageSelfType data = DamageSelfState.Data;
                    int realDamage = data.Damage;
                    if (data.Damage < 0 || pTechno.Ref.CloakStates == CloakStates.UnCloaked || data.Decloak)
                    {
                        // 维修或者显形直接炸
                        pTechno.Ref.Base.ReceiveDamage(data.Damage, 0, DamageSelfState.pWH, IntPtr.Zero, data.IgnoreArmor, pTechno.Ref.Type.Ref.Crewed, pTechno.Ref.Owner);
                    }
                    else
                    {
                        // 不显形不能使用ReceiveDamage，改成直接扣血
                        if (!data.IgnoreArmor)
                        {
                            // 计算实际伤害
                            realDamage = MapClass.GetTotalDamage(data.Damage, DamageSelfState.pWH, OwnerObject.Ref.Base.Type.Ref.Armor, 0);
                        }
                        // 扣血
                        if (realDamage >= pTechno.Ref.Base.Health)
                        {
                            // 本次伤害足够打死目标
                            pTechno.Ref.Base.ReceiveDamage(realDamage, 0, DamageSelfState.pWH, IntPtr.Zero, true, pTechno.Ref.Type.Ref.Crewed, pTechno.Ref.Owner);
                        }
                        else
                        {
                            // 血量可以减到负数不死
                            pTechno.Ref.Base.Health -= realDamage;
                        }
                    }
                    // 播放弹头动画
                    if (data.WarheadAnim)
                    {
                        CoordStruct location = pTechno.Ref.Base.Base.GetCoords(); LandType landType = LandType.Clear;
                        if (MapClass.Instance.TryGetCellAt(location, out Pointer<CellClass> pCell))
                        {
                            landType = pCell.Ref.LandType;
                        }
                        Pointer<AnimTypeClass> pWHAnimType = MapClass.SelectDamageAnimation(realDamage, DamageSelfState.pWH, landType, location);
                        if (!pWHAnimType.IsNull)
                        {
                            Pointer<AnimClass> pWHAnim = YRMemory.Create<AnimClass>(pWHAnimType, location);
                            pWHAnim.Ref.Owner = pTechno.Ref.Owner;
                        }
                    }
                    DamageSelfState.Reset();
                }
            }
        }


    }

    public partial class TechnoTypeExt
    {
        public DamageSelfType DamageSelfData;

        /// <summary>
        /// [TechnoType]
        /// DamageSelf.Damage=1 ;持续期间对附着对象产生伤害
        /// DamageSelf.ROF=0 ;伤害间隔
        /// DamageSelf.Warhead=C4Warhead ;伤害使用的弹头
        /// DamageSelf.IgnoreArmor=yes ;无视护甲类型
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadDamageSelf(INIReader reader, string section)
        {
            DamageSelfType temp = new DamageSelfType();
            if (temp.TryReadType(reader, section))
            {
                DamageSelfData = temp;
            }
            else
            {
                temp = null;
            }
        }
    }

    public partial class BulletExt
    {

        public DamageSelfState DamageSelfState => AttachEffectManager.DamageSelfState;

        public unsafe void BulletClass_Update_DamageSelf()
        {
            if (DamageSelfState.CanHitSelf())
            {
                TakeDamage(DamageSelfState.BulletDamageStatus, true);
            }
        }

    }

}