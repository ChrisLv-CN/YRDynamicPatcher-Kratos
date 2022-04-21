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
    public class AutoFireAreaWeaponData
    {
        public bool Enable;
        public int WeaponIndex;
        public int InitialDelay;
        public bool CheckAmmo;

        public AutoFireAreaWeaponData(int weaponIndex)
        {
            this.Enable = weaponIndex > -1;
            this.WeaponIndex = weaponIndex;
            this.InitialDelay = 0;
            this.CheckAmmo = false;
        }
    }

    [Serializable]
    public class AutoFireAreaWeapon
    {
        public bool Enable;
        public AutoFireAreaWeaponData Data;
        public int Delay;
        private bool canFire;
        public TimerStruct reloadTimer;

        public AutoFireAreaWeapon(AutoFireAreaWeaponData data)
        {
            this.Enable = data.Enable;
            this.Data = data;
            this.Delay = data.InitialDelay;
            if (Delay > 0)
            {
                this.canFire = false;
                reloadTimer.Start(Delay);
            }
            else
            {
                this.canFire = true;
            }

        }

        public bool CanFire()
        {
            if (!canFire)
            {
                canFire = reloadTimer.Expired();
            }
            return canFire;
        }

        public void Reload(int rof)
        {
            canFire = false;
            reloadTimer.Start(rof);
        }
    }

    public partial class TechnoExt
    {
        public AutoFireAreaWeapon autoFireAreaWeapon;

        public unsafe void TechnoClass_Init_AutoFireAreaWeapon()
        {
            if (null != Type.AutoFireAreaWeaponData && Type.AutoFireAreaWeaponData.Enable && null == autoFireAreaWeapon)
            {
                autoFireAreaWeapon = new AutoFireAreaWeapon(Type.AutoFireAreaWeaponData);
            }
        }

        public unsafe void TechnoClass_Update_AutoFireAreaWeapon()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (null != autoFireAreaWeapon && autoFireAreaWeapon.Enable && autoFireAreaWeapon.CanFire() && pTechno.Convert<ObjectClass>().Ref.IsAlive && pTechno.Convert<ObjectClass>().Ref.IsOnMap)
            {
                Pointer<WeaponStruct> pWeapon = pTechno.Ref.GetWeapon(autoFireAreaWeapon.Data.WeaponIndex);
                if (null == pWeapon || pWeapon.IsNull || pWeapon.Ref.WeaponType.IsNull)
                {
                    // no weapon. disable.
                    Type.AutoFireAreaWeaponData.Enable = false;
                    autoFireAreaWeapon.Enable = false;
                    return;
                }
                if (autoFireAreaWeapon.Data.CheckAmmo)
                {
                    int ammo = pTechno.Ref.Ammo;
                    if (ammo == 0)
                    {
                        return;
                    }
                    else if (ammo > 0)
                    {
                        pTechno.Ref.Ammo--;
                    }
                }
                autoFireAreaWeapon.Reload(pWeapon.Ref.WeaponType.Ref.ROF);
                CoordStruct location = pTechno.Ref.Base.Base.GetCoords();
                if (MapClass.Instance.TryGetCellAt(location, out Pointer<CellClass> pCell) && !pCell.IsNull)
                {
                    pTechno.Ref.Fire_IgnoreType(pCell.Convert<AbstractClass>(), autoFireAreaWeapon.Data.WeaponIndex);
                }
            }
        }

    }

    public partial class TechnoTypeExt
    {
        public AutoFireAreaWeaponData AutoFireAreaWeaponData;

        /// <summary>
        /// [TechnoType]
        /// AutoFireAreaWeapon=0
        /// AutoFireAreaWeapon.InitialDelay=0
        /// AutoFireAreaWeapon.CheckAmmo=no
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadAutoFireAreaWeapon(INIReader reader, string section)
        {

            // AutoFireAreaWeapon
            int weaponIndex = 0;
            if (reader.ReadNormal(section, "AutoFireAreaWeapon", ref weaponIndex))
            {
                AutoFireAreaWeaponData = new AutoFireAreaWeaponData(weaponIndex);

                int initDelay = 0;
                if (reader.ReadNormal(section, "AutoFireAreaWeapon.InitialDelay", ref initDelay))
                {
                    AutoFireAreaWeaponData.InitialDelay = initDelay;
                }

                bool checkAmmo = false;
                if (reader.ReadNormal(section, "AutoFireAreaWeapon.CheckAmmo", ref checkAmmo))
                {
                    AutoFireAreaWeaponData.CheckAmmo = checkAmmo;
                }
            }

        }
    }

}