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
                OverrideWeaponData data = Data.Data;
                if (isElite)
                {
                    data = Data.EliteData;
                }
                if (null != data)
                {
                    List<string> types = data.Types;
                    bool isRandomType = data.RandomType;
                    List<int> weights = data.Weights;
                    int overrideIndex = data.Index;
                    double chance = data.Chance;
                    weaponType = types[0];
                    if (isRandomType)
                    {
                        // 算权重
                        int typeCount = types.Count;
                        // 获取权重标靶
                        Dictionary<Point2D, int> targetPad = weights.MakeTargetPad(typeCount, out int maxValue);
                        // 中
                        int i = targetPad.Hit(maxValue);
                        weaponType = types[i];
                    }
                    if (!string.IsNullOrEmpty(weaponType) && (overrideIndex < 0 || overrideIndex == index))
                    {
                        // 算概率
                        return chance >= 1 || chance >= MathEx.Random.NextDouble();
                    }
                }
            }
            return false;
        }

    }
}