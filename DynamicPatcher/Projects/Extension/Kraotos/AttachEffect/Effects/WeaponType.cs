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
        public WeaponType WeaponType;

        private void ReadWeaponType(INIReader reader, string section)
        {
            if (WeaponType.ReadWeaponType(reader, section, out WeaponType weaponType))
            {
                this.WeaponType = weaponType;
            }
        }

    }

    /// <summary>
    /// 覆盖武器
    /// </summary>
    [Serializable]
    public class WeaponType : OverrideWeaponData, IEffectType<Weapon>
    {
        public bool WeaponDisable;

        public WeaponType() : base()
        {
            this.WeaponDisable = false;
        }

        public Weapon CreateObject(AttachEffectType attachEffectType)
        {
            return new Weapon(this, attachEffectType);
        }

        public static bool ReadWeaponType(INIReader reader, string section, out WeaponType weaponType)
        {
            weaponType = null;

            // 替换武器
            WeaponType temp = new WeaponType();
            temp.ReadOverrideWeapon(reader, section);
            if (temp.Enable)
            {
                if (null == weaponType)
                {
                    weaponType = temp;
                }
            }

            // 关闭武器
            bool disable = false;
            if (reader.ReadNormal(section, "Weapon.Disable", ref disable))
            {
                if (null == weaponType)
                {
                    weaponType = new WeaponType();
                }
                weaponType.WeaponDisable = disable;
            }
            
            return null != weaponType;
        }

    }

}