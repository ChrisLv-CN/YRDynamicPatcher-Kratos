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
            DestroySelfType type = new DestroySelfType();
            if (type.TryReadType(reader, section))
            {
                this.Enable = true;
                this.DestroySelfType = type;
            }
            else
            {
                type = null;
            }
        }

    }

    /// <summary>
    /// 自毁类型
    /// </summary>
    [Serializable]
    public class DestroySelfType : EffectType<DestroySelf>, IAEStateData
    {
        public int Delay;
        public bool Peaceful;

        public DestroySelfType()
        {
            this.Enable = false;
            this.Delay = 0;
            this.Peaceful = false;
        }

        public override bool TryReadType(INIReader reader, string section)
        {

            bool enable = false;
            if (reader.ReadNormal(section, "DestroySelf", ref enable))
            {
                this.Enable = enable;
                this.Delay = 0;
            }

            int life = 0;
            if (reader.ReadNormal(section, "DestroySelf", ref life))
            {
                this.Enable = true;
                this.Delay = life;
            }

            enable = false;
            if (reader.ReadNormal(section, "DestroySelf.Delay", ref enable))
            {
                this.Enable = enable;
                this.Delay = 0;
            }

            life = 0;
            if (reader.ReadNormal(section, "DestroySelf.Delay", ref life))
            {
                this.Enable = true;
                this.Delay = life;
            }

            if (this.Enable)
            {
                bool peaceful = false;
                if (reader.ReadNormal(section, "DestroySelf.Peaceful", ref peaceful))
                {
                    this.Peaceful = peaceful;
                }
            }

            return this.Enable;
        }

    }

}