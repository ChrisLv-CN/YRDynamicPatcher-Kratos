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
        public DisableWeaponType DisableWeaponType;

        private void ReadDisableWeaponType(INIReader reader, string section)
        {
            DisableWeaponType type = new DisableWeaponType();
            if (type.TryReadType(reader, section))
            {
                this.DisableWeaponType = type;
            }
        }

    }

    /// <summary>
    /// 禁用武器
    /// </summary>
    [Serializable]
    public class DisableWeaponType : EffectType<DisableWeapon>, IAEStateData
    {
        public bool WeaponDisable;


        public DisableWeaponType()
        {
            this.WeaponDisable = false;
        }

        public override bool TryReadType(INIReader reader, string section)
        {
            ReadCommonType(reader, section, "Weapon.");

            bool disable = false;
            if (reader.ReadNormal(section, "Weapon.Disable", ref disable))
            {
                this.WeaponDisable = disable;
            }

            return this.WeaponDisable;
        }

    }

}