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
        public DamageSelfType DamageSelfType;

        private void ReadDamageSelfType(INIReader reader, string section)
        {
            DamageSelfType type = new DamageSelfType();
            if (type.TryReadType(reader, section))
            {
                this.Enable = true;
                this.DamageSelfType = type;
            }
            else
            {
                type = null;
            }
        }

    }

    /// <summary>
    /// 伤害自己
    /// </summary>
    [Serializable]
    public class DamageSelfType :  EffectType<DamageSelf>, IAEStateData
    {

        public int Damage;
        public int ROF;
        public string Warhead;
        public bool WarheadAnim;
        public bool Decloak;
        public bool IgnoreArmor;

        public bool Peaceful;

        public DamageSelfType()
        {
            Damage = 0;
            ROF = 0;
            Warhead = null;
            IgnoreArmor = true;

            Peaceful = false;

            AffectWho = AffectWho.MASTER;
        }


        public override bool TryReadType(INIReader reader, string section)
        {

            ReadCommonType(reader, section, "DamageSelf.");

            int damage = 0;
            if (reader.ReadNormal(section, "DamageSelf.Damage", ref damage) && damage != 0)
            {
                this.Enable = true;

                this.Damage = damage;

                int rof = 0;
                if (reader.ReadNormal(section, "DamageSelf.ROF", ref rof) && rof >= 0)
                {
                    this.ROF = rof;
                }

                string wh = null;
                if (reader.ReadNormal(section, "DamageSelf.Warhead", ref wh))
                {
                    this.Warhead = wh;
                }

                bool warheadAnim = true;
                if (reader.ReadNormal(section, "DamageSelf.WarheadAnim", ref warheadAnim))
                {
                    this.WarheadAnim = warheadAnim;
                }

                bool decloak = true;
                if (reader.ReadNormal(section, "DamageSelf.Decloak", ref decloak))
                {
                    this.Decloak = decloak;
                }

                bool ignoreArmor = true;
                if (reader.ReadNormal(section, "DamageSelf.IgnoreArmor", ref ignoreArmor))
                {
                    this.IgnoreArmor = ignoreArmor;
                }

                bool peaceful = true;
                if (reader.ReadNormal(section, "DamageSelf.Peaceful", ref peaceful))
                {
                    this.Peaceful = peaceful;
                }
            }

            return this.Enable;
        }


    }

}