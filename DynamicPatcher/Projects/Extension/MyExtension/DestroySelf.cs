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
    public class DestroySelfStatus
    {
        public DestroySelfData Data;
        public bool Dead;
        public TimerStruct LifeTimer;

        public DestroySelfStatus(DestroySelfData data)
        {
            this.Data = data;
            if (data.Life > 0)
            {
                this.Dead = false;
                this.LifeTimer = new TimerStruct(data.Life);
            }
            else
            {
                this.Dead = true;
            }
        }

        public bool IsDead()
        {
            if (!Dead)
            {
                Dead = LifeTimer.Expired();
            }
            return Dead;
        }

    }

    [Serializable]
    public class DestroySelfData
    {
        public bool Enable;
        public bool Peaceful;
        public int Life;

        public DestroySelfData(int life, bool enable = true)
        {
            if (enable && life < 0)
            {
                this.Enable = false;
            }
            else
            {
                this.Enable = enable;
            }
            this.Peaceful = false;
            this.Life = life;

        }

        public override string ToString()
        {
            return string.Format("{{\"Enable\":{0}, \"Life\":{1}, \"Peaceful\":{2}}}", Enable, Life, Peaceful);
        }
    }

    public partial class TechnoExt
    {

        public DestroySelfStatus DestroySelfStatus;

        public unsafe void TechnoClass_Put_DestroySelf(Pointer<CoordStruct> pCoord, Direction faceDir)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (null != Type.DestroySelfData && Type.DestroySelfData.Enable && null == DestroySelfStatus)
            {
                DestroySelfStatus = new DestroySelfStatus(Type.DestroySelfData);
            }
        }

        public unsafe void TechnoClass_Update_DestroySelf()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            // if (null != DestroySelfStatus)
            // {
            //     Logger.Log("单位[{0}]{1}自毁倒计时{2}", pTechno.Ref.Type.Ref.Base.Base.ID, pTechno, DestroySelfStatus.LifeTimer.GetTimeLeft());
            // }
            if (null != DestroySelfStatus && DestroySelfStatus.IsDead())
            {
                if (DestroySelfStatus.Data.Peaceful)
                {
                    pTechno.Ref.Base.Remove();
                    pTechno.Ref.Base.UnInit();
                }
                else
                {
                    pTechno.Ref.Base.TakeDamage(pTechno.Ref.Base.Health + 1, pTechno.Ref.Type.Ref.Crewed);
                    // pTechno.Ref.Base.Destroy();
                }
                DestroySelfStatus = null;
            }
        }


    }

    public partial class TechnoTypeExt
    {
        public DestroySelfData DestroySelfData;

        /// <summary>
        /// [TechnoType]
        /// DestroySelf=1500
        /// DestroySelfPeaceful=yes
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadDestroySelf(INIReader reader, string section)
        {
            bool destory = false;
            if (reader.ReadNormal(section, "DestroySelf", ref destory))
            {
                DestroySelfData = new DestroySelfData(0, destory);
            }

            int life = 0;
            if (reader.ReadNormal(section, "DestroySelf", ref life))
            {
                DestroySelfData = new DestroySelfData(life);
            }

            destory = false;
            if (reader.ReadNormal(section, "DestroySelf.Delay", ref destory))
            {
                DestroySelfData = new DestroySelfData(0, destory);
            }

            life = 0;
            if (reader.ReadNormal(section, "DestroySelf.Delay", ref life))
            {
                DestroySelfData = new DestroySelfData(life);
            }

            if (null != DestroySelfData)
            {
                bool peaceful = false;
                if (reader.ReadNormal(section, "DestroySelf.Peaceful", ref peaceful))
                {
                    DestroySelfData.Peaceful = peaceful;
                }
            }
        }
    }

    public partial class BulletExt
    {

        public DestroySelfStatus DestroySelfStatus;

        public unsafe void BulletClass_Update_DestroySelf()
        {
            // if (null != DestroySelfStatus)
            // {
            //     Logger.Log("抛射体[{0}]{1}自毁倒计时{2}", OwnerObject.Ref.Type.Ref.Base.Base.ID, OwnerObject, DestroySelfStatus.LifeTimer.GetTimeLeft());
            // }
            if (null != DestroySelfStatus && DestroySelfStatus.IsDead())
            {
                BulletDamageStatus bulletDamageStatus = new BulletDamageStatus(1);
                bulletDamageStatus.Eliminate = true;
                bulletDamageStatus.Harmless = DestroySelfStatus.Data.Peaceful;
                // Logger.Log("抛射体[{0}]{1}自毁倒计时结束，自毁开始{2}", OwnerObject.Ref.Type.Ref.Base.Base.ID, OwnerObject, bulletDamageStatus);
                TakeDamage(bulletDamageStatus, true);
                DestroySelfStatus = null;
            }
        }

    }

}