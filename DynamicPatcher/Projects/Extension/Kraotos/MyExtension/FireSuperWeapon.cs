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
    public class FireSuperWeaponData
    {
        public bool Enable;
        public List<string> SuperWeapons;
        public bool AnyWeapon;
        public int WeaponIndex;
        public bool ToTarget;
        public bool RealLaunch;

        public FireSuperWeaponData(List<string> superWeapons)
        {
            this.Enable = superWeapons.Count > 0;
            this.SuperWeapons = superWeapons;
            this.AnyWeapon = false;
            this.WeaponIndex = 0;
            this.ToTarget = true;
            this.RealLaunch = false;
        }
    }

    public partial class TechnoExt
    {

        public unsafe void TechnoClass_OnFire_SuperWeapon(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (null != Type.FireSuperWeaponData && Type.FireSuperWeaponData.Enable && (Type.FireSuperWeaponData.AnyWeapon || Type.FireSuperWeaponData.WeaponIndex == weaponIndex))
            {
                foreach (string superWeaponId in Type.FireSuperWeaponData.SuperWeapons)
                {
                    Pointer<SuperWeaponTypeClass> pType = SuperWeaponTypeClass.ABSTRACTTYPE_ARRAY.Find(superWeaponId);
                    if (!pType.IsNull)
                    {
                        Pointer<HouseClass> pHouse = pTechno.Ref.Owner;
                        Pointer<SuperClass> pSuper = pHouse.Ref.FindSuperWeapon(pType);
                        if (pSuper.Ref.IsCharged || !Type.FireSuperWeaponData.RealLaunch)
                        {
                            CoordStruct targetPos = Type.FireSuperWeaponData.ToTarget ? pTarget.Ref.GetCoords() : pTechno.Ref.Base.Base.GetCoords();
                            CellStruct cell = MapClass.Coord2Cell(targetPos);
                            pSuper.Ref.IsCharged = true;
                            pSuper.Ref.Launch(cell, true);
                            pSuper.Ref.IsCharged = false;
                            pSuper.Ref.Reset();
                        }
                    }
                }
            }
        }


    }

    public partial class TechnoTypeExt
    {
        public FireSuperWeaponData FireSuperWeaponData;

        /// <summary>
        /// [TechnoType]
        /// FireSuperWeapon.Types=NukeSpecial,IronCurtainSpecial
        /// FireSuperWeapon.Weapon=0
        /// FireSuperWeapon.AnyWeapon=no
        /// FireSuperWeapon.ToTarget=yes
        /// FireSuperWeapon.RealLaunch=no
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadFireSuperWeapon(INIReader reader, string section)
        {

            List<string> sWeapons = null;
            if (ExHelper.ReadList(reader, section, "FireSuperWeapon.Types", ref sWeapons))
            {
                FireSuperWeaponData = new FireSuperWeaponData(sWeapons);

                bool anyWeapon = false;
                if (reader.ReadNormal(section, "FireSuperWeapon.AnyWeapon", ref anyWeapon))
                {
                    FireSuperWeaponData.AnyWeapon = anyWeapon;
                }

                int weaponIndex = 0;
                if (reader.ReadNormal(section, "FireSuperWeapon.Weapon", ref weaponIndex))
                {
                    FireSuperWeaponData.WeaponIndex = weaponIndex;
                }

                bool toTarget = false;
                if (reader.ReadNormal(section, "FireSuperWeapon.ToTarget", ref toTarget))
                {
                    FireSuperWeaponData.ToTarget = toTarget;
                }

                bool realLaunch = false;
                if (reader.ReadNormal(section, "FireSuperWeapon.RealLaunch", ref realLaunch))
                {
                    FireSuperWeaponData.RealLaunch = realLaunch;
                }
            }

        }
    }

}