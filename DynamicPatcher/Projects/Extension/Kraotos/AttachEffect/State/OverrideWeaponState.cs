using System.Reflection;
using System.Collections;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    [Serializable]
    public class OverrideWeaponState : AEState<OverrideWeaponType>
    {
        public bool TryGetOverrideWeapon(int index, bool isElite, out Pointer<WeaponTypeClass> pOverrideWeapon)
        {
            pOverrideWeapon = IntPtr.Zero;
            if (IsActive() && CanOverride(index, isElite, out string weaponType))
            {
                pOverrideWeapon = WeaponTypeClass.ABSTRACTTYPE_ARRAY.Find(weaponType);
                return !pOverrideWeapon.IsNull;
            }
            return false;
        }

        private bool CanOverride(int index, bool isElite, out string weaponType)
        {
            weaponType = null;
            if (null != Data)
            {
                weaponType = Data.Type;
                int overrideIndex = Data.Index;
                double chance = Data.Chance;
                if (isElite)
                {
                    weaponType = Data.EliteType;
                    overrideIndex = Data.EliteIndex;
                    chance = Data.EliteChance;
                }
                if (!string.IsNullOrEmpty(weaponType) && (overrideIndex < 0 || overrideIndex == index))
                {
                    // 算概率
                    return chance >= 1 || chance >= MathEx.Random.NextDouble();
                }
            }
            return false;
        }

    }
}