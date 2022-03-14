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


    public partial class AttachEffectType
    {
        public DestroySelfType DestroySelfType;

        private void ReadDestroySelfType(INIReader reader, string section)
        {
            if (DestroySelfType.ReadDestroySelfType(reader, section, out DestroySelfType destroySelfType))
            {
                this.DestroySelfType = destroySelfType;
            }
        }

    }

    /// <summary>
    /// 自毁类型
    /// </summary>
    [Serializable]
    public class DestroySelfType : IEffectType<DestroySelf>
    {
        public bool Enable;
        public int Delay;
        public bool Peaceful;

        public DestroySelfType()
        {
            this.Enable = true;
            this.Delay = 0;
            this.Peaceful = false;
        }

        public DestroySelf CreateObject(AttachEffectType attachEffectType)
        {
            return new DestroySelf(this, attachEffectType);
        }

        public static bool ReadDestroySelfType(INIReader reader, string section, out DestroySelfType destroySelfType)
        {
            destroySelfType = null;

            bool enable = false;
            if (reader.ReadNormal(section, "DestroySelf", ref enable))
            {
                if (null == destroySelfType)
                {
                    destroySelfType = new DestroySelfType();
                }
                destroySelfType.Delay = 0;
                destroySelfType.Enable = enable;
            }

            int life = 0;
            if (reader.ReadNormal(section, "DestroySelf", ref life))
            {
                if (null == destroySelfType)
                {
                    destroySelfType = new DestroySelfType();
                }
                destroySelfType.Delay = life;
                destroySelfType.Enable = true;
            }

            enable = false;
            if (reader.ReadNormal(section, "DestroySelf.Delay", ref enable))
            {
                if (null == destroySelfType)
                {
                    destroySelfType = new DestroySelfType();
                }
                destroySelfType.Delay = 0;
                destroySelfType.Enable = enable;
            }

            life = 0;
            if (reader.ReadNormal(section, "DestroySelf.Delay", ref life))
            {
                if (null == destroySelfType)
                {
                    destroySelfType = new DestroySelfType();
                }
                destroySelfType.Delay = life;
                destroySelfType.Enable = true;
            }

            if (null != destroySelfType && destroySelfType.Enable)
            {
                bool peaceful = false;
                if (reader.ReadNormal(section, "DestroySelf.Peaceful", ref peaceful))
                {
                    destroySelfType.Peaceful = peaceful;
                }
            }
            else
            {
                destroySelfType = null;
            }

            return null != destroySelfType;
        }

    }

}