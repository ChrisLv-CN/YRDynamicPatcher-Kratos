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
                List<string> types = Data.Types;
                bool isRandomType = Data.RandomType;
                List<int> weights = Data.Weights;
                int overrideIndex = Data.Index;
                double chance = Data.Chance;
                if (isElite)
                {
                    types = Data.EliteTypes;
                    isRandomType = Data.EliteRandomType;
                    weights = Data.EliteWeights;
                    overrideIndex = Data.EliteIndex;
                    chance = Data.EliteChance;
                }
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
            return false;
        }

        private string FinedWeaponType(bool isElite)
        {
            List<string> types = Data.Types;
            bool isRandomType = Data.RandomType;
            List<int> weights = Data.Weights;
            if (isElite)
            {
                types = Data.EliteTypes;
                isRandomType = Data.EliteRandomType;
                weights = Data.EliteWeights;
            }
            if (isRandomType)
            {
                // 算权重
                int typeCount = types.Count;

                int weightCount = null != weights ? weights.Count : 0;
                Dictionary<Point2D, int> targetPad = weights.MakeTargetPad(typeCount, out int maxValue);
                int index = MathEx.Random.Next(0, maxValue);
                return types[index];
            }
            return types[0];
        }

    }
}