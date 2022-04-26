using System.Drawing;
using System.Threading;
using PatcherYRpp;
using PatcherYRpp.FileFormats;
using PatcherYRpp.Utilities;
using Extension;
using Extension.Utilities;
using DynamicPatcher;
using System;
using System.Collections.Generic;
using System.Reflection;
using Extension.Ext;

namespace Extension.Utilities
{

    public static partial class ExHelper
    {

        // Finder all stand, check distance and blown it up.
        public static void FindAndDamageStand(CoordStruct location, int damage, Pointer<ObjectClass> pAttacker,
           Pointer<WarheadTypeClass> pWH, bool affectsTiberium, Pointer<HouseClass> pAttackingHouse)
        {

            double spread = pWH.Ref.CellSpread * 256;

            HashSet<DamageGroup> stands = new HashSet<DamageGroup>();
            ExHelper.FindTechno(IntPtr.Zero, (pTechno) =>
            {
                if (!pTechno.Ref.Base.IsOnMap && !(pTechno.Ref.Base.IsIronCurtained() || pTechno.Ref.IsForceShilded)) // Stand always not on map.
                {
                    TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                    if (null != ext && !ext.MyMaster.IsNull && null != ext.StandType)
                    {
                        // 检查距离
                        CoordStruct targetPos = pTechno.Ref.Base.Base.GetCoords();
                        double dist = targetPos.DistanceFrom(location);
                        if (pTechno.Ref.Base.Base.WhatAmI() == AbstractType.Aircraft && pTechno.InAir(true))
                        {
                            dist *= 0.5;
                        }
                        if (dist <= spread)
                        {
                            // 找到一个最近的替身，检查替身是否可以受伤，以及弹头是否可以影响该替身
                            if (!ext.StandType.Immune
                                && ext.AffectMe(pAttacker, pWH, pAttackingHouse, out WarheadTypeExt whExt) // 检查所属权限
                                && ext.DamageMe(damage, (int)dist, whExt, out int realDamage) // 检查护甲
                            )
                            {
                                DamageGroup damageGroup = new DamageGroup();
                                damageGroup.Target = pTechno;
                                damageGroup.Distance = dist;
                                stands.Add(damageGroup);
                            }
                        }
                    }
                }
                return false;
            }, true, true, true, true);

            foreach (DamageGroup damageGroup in stands)
            {
                damageGroup.Target.Ref.Base.ReceiveDamage(damage, (int)damageGroup.Distance, pWH, pAttacker, false, false, pAttackingHouse);
            }
        }

    }

}