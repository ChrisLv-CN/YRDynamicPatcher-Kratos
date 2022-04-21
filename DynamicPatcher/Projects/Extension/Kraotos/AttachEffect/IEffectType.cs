using DynamicPatcher;
using Extension.Script;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    public interface IEffectType
    {

        bool TryReadType(INIReader reader, string section);

    }

    [Serializable]
    public class EffectType<E> : IEffectType where E : IEffect, new()
    {
        public bool Enable;

        public AffectWho AffectWho;

        public bool DeactiveWhenCivilian;

        public EffectType()
        {
            this.Enable = false;
            this.AffectWho = AffectWho.ALL;
            this.DeactiveWhenCivilian = false;
        }

        public E CreateObject(AttachEffectType aeType)
        {
            E effect = new E();
            effect.InitEffect(this, aeType);
            return effect;
        }

        public virtual bool TryReadType(INIReader reader, string section)
        {
            return false;
        }

        protected bool ReadCommonType(INIReader reader, string section, string title)
        {
            bool flag = false;

            bool enable = false;
            if (reader.ReadNormal(section, title + "Enable", ref enable))
            {
                flag = true;
                this.Enable = enable;
            }

            string who = null;
            if (reader.ReadNormal(section, title + "AffectWho", ref who))
            {
                flag = true;
                string t = who.Substring(0, 1).ToUpper();
                switch (t)
                {
                    case "A":
                        this.AffectWho = AffectWho.ALL;
                        break;
                    case "M":
                        this.AffectWho = AffectWho.MASTER;
                        break;
                    case "S":
                        this.AffectWho = AffectWho.STAND;
                        break;
                }
            }

            bool DeactiveWhenCivilian = false;
            if (reader.ReadNormal(section, title + "DeactiveWhenCivilian", ref DeactiveWhenCivilian))
            {
                flag = true;
                this.DeactiveWhenCivilian = DeactiveWhenCivilian;
            }

            return flag;
        }
    }

    public enum AffectWho
    {
        ALL = 0, MASTER = 1, STAND = 2
    }

}
