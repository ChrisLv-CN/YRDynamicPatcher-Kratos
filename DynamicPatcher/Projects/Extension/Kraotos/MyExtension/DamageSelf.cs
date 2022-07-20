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
        private AttachEffectType damageSelfAE;

        public unsafe void TechnoClass_Put_DamageSelf(Pointer<CoordStruct> pCoord, short faceDirValue8)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (null != Type.DamageSelfData && Type.DamageSelfData.Enable)
            {
                damageSelfAE = new AttachEffectType("DamageSelf" + OwnerObject);
                damageSelfAE.Enable = true;
                damageSelfAE.DamageSelfType = Type.DamageSelfData;

                OnUpdateAction += TechnoClass_Update_DamageSelf;
            }
        }

        public unsafe void TechnoClass_Update_DamageSelf()
        {
            if (null != damageSelfAE && !IsDead && !OwnerObject.Ref.Base.InLimbo && !OwnerObject.Ref.IsImmobilized)
            {
                AttachEffect(damageSelfAE, OwnerObject.Convert<ObjectClass>(), OwnerObject.Ref.Owner);
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
        /// DamageSelf.WarheadAnim=no ;播放弹头动画
        /// DamageSelf.Decloak=yes ;受伤时隐形单位会显形，如同被炮弹击中
        /// DamageSelf.IgnoreArmor=yes ;无视护甲类型
        /// DamageSelf.Peaceful=no ;如果单位被自伤打死，将平静的消失，不发生爆炸
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

}