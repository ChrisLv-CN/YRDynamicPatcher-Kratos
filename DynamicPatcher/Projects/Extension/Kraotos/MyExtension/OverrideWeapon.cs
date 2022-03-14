using System.Threading;
using System.IO;
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
    public class OverrideWeapon
    {
        public bool Enable;
        public OverrideWeaponData Data;
        public bool Locked;
        public SwizzleablePointer<WeaponTypeClass> pOverrideWeapon;
        public int weaponIndex;

        public OverrideWeapon(OverrideWeaponData data)
        {
            this.Enable = data.Enable;
            this.Data = data;
            this.Locked = false;
            this.pOverrideWeapon = new SwizzleablePointer<WeaponTypeClass>(IntPtr.Zero);
            this.weaponIndex = -1;
        }

        public bool OverrideThisWeapon()
        {
            return weaponIndex == Data.WeaponIndex;
        }
    }

    [Serializable]
    public class OverrideWeaponData
    {
        public bool Enable;
        public int WeaponIndex;

        public OverrideWeaponData(int weaponIndex)
        {
            this.Enable = weaponIndex > -1;
            this.WeaponIndex = weaponIndex;
        }
    }

    public partial class TechnoExt
    {
        public OverrideWeapon overrideWeapon;

        public void TechnoClass_Init_OverrideWeapon()
        {
            if (null != Type.OverrideWeaponData && Type.OverrideWeaponData.Enable && null == overrideWeapon)
            {
                overrideWeapon = new OverrideWeapon(Type.OverrideWeaponData);
            }
        }

        public void TechnoClass_OnFire_OverrideWeapon(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            if (null != overrideWeapon)
            {
                // Logger.Log("{0}记录下发射武器序号{1}", OwnerObject.Ref.Type.Convert<AbstractTypeClass>().Ref.ID, weaponIndex);
                overrideWeapon.weaponIndex = weaponIndex;
            }
        }

        public bool TechnoClass_RegisterDestruction_OverrideWeapon(Pointer<TechnoClass> pKiller, int cost)
        {
            // override killer's weapon
            Pointer<TechnoClass> pTechno = OwnerObject;
            // Logger.Log("被{0}杀死了", pKill.Ref.Type.Convert<AbstractTypeClass>().Ref.ID);
            TechnoExt ext = TechnoExt.ExtMap.Find(pKiller);
            if (null != ext && null != ext.overrideWeapon && !ext.overrideWeapon.Locked)
            {
                Pointer<WeaponStruct> pWeapon = pTechno.Ref.GetWeapon(0);
                if (!pWeapon.IsNull && !pWeapon.Ref.WeaponType.IsNull)
                {
                    // Logger.Log("被{0}杀死了，转移武器{1}给{0}", pKill.Ref.Type.Convert<AbstractTypeClass>().Ref.ID, pWeapon.Ref.WeaponType.Ref.Base.ID);
                    ext.overrideWeapon.pOverrideWeapon.Pointer = pWeapon.Ref.WeaponType;
                }
            }
            return false;
        }

    }

    public partial class TechnoTypeExt
    {

        public OverrideWeaponData OverrideWeaponData;

        /// <summary>
        /// [TechnoType]
        /// OverrideWeapon=0
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadOverrideWeapon(INIReader reader, string section)
        {
            // Override weapon
            int weaponIndex = -1;
            if (reader.ReadNormal(section, "OverrideWeapon", ref weaponIndex))
            {
                OverrideWeaponData = new OverrideWeaponData(weaponIndex);
            }

        }
    }

}