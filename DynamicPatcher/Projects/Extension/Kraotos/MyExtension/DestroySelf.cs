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

        public DestroySelfState DestroySelfState => AttachEffectManager.DestroySelfState;

        public unsafe void TechnoClass_Put_DestroySelf(Pointer<CoordStruct> pCoord, short faceDirValue8)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (null != Type.DestroySelfData && Type.DestroySelfData.Enable)
            {
                DestroySelfState.Enable(Type.DestroySelfData);
            }
        }

        public unsafe void TechnoClass_Update_DestroySelf()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;

            if (DestroySelfState.AmIDead())
            {
                // Logger.Log($"{Game.CurrentFrame} - {pTechno}[{pTechno.Ref.Type.Ref.Base.Base.ID}] 自毁 {DestroySelfState.Data}");
                if (DestroySelfState.Data.Peaceful)
                {
                    pTechno.Ref.Base.Remove();
                    pTechno.Ref.Base.UnInit();
                }
                else
                {
                    SkipDamageText = true;
                    pTechno.Ref.Base.TakeDamage(pTechno.Ref.Base.Health + 1, pTechno.Ref.Type.Ref.Crewed);
                    // pTechno.Ref.Base.Destroy();
                }
            }
        }


    }

    public partial class TechnoTypeExt
    {
        public DestroySelfType DestroySelfData;

        /// <summary>
        /// [TechnoType]
        /// DestroySelf=1500
        /// DestroySelfPeaceful=yes
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadDestroySelf(INIReader reader, string section)
        {
            DestroySelfType temp = new DestroySelfType();
            if (temp.TryReadType(reader, section))
            {
                DestroySelfData = temp;
            }
            else
            {
                temp = null;
            }
        }
    }

    public partial class BulletExt
    {

        public DestroySelfState DestroySelfState => AttachEffectManager.DestroySelfState;

        public unsafe void BulletClass_Update_DestroySelf()
        {
            // if (null != DestroySelfStatus)
            // {
            //     Logger.Log("抛射体[{0}]{1}自毁倒计时{2}", OwnerObject.Ref.Type.Ref.Base.Base.ID, OwnerObject, DestroySelfStatus.LifeTimer.GetTimeLeft());
            // }
            if (DestroySelfState.AmIDead())
            {
                BulletDamageStatus bulletDamageStatus = new BulletDamageStatus(1);
                bulletDamageStatus.Eliminate = true;
                bulletDamageStatus.Harmless = DestroySelfState.Data.Peaceful;
                // Logger.Log("抛射体[{0}]{1}自毁倒计时结束，自毁开始{2}", OwnerObject.Ref.Type.Ref.Base.Base.ID, OwnerObject, bulletDamageStatus);
                TakeDamage(bulletDamageStatus, true);
            }
        }

    }

}