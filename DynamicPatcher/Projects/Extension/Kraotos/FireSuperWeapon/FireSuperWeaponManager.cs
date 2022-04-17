using System.Collections;
using DynamicPatcher;
using Extension.Ext;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{



    [Serializable]
    public class FireSuperWeaponManager
    {
        // Burst发射模式下剩余待发射的队列
        private Queue<FireSuperWeapon> fireSuperWeaponQueue = new Queue<FireSuperWeapon>();


        public void Launch(Pointer<HouseClass> pHouse, CoordStruct location, FireSuperWeaponsData data)
        {
            CellStruct cellStruct = MapClass.Coord2Cell(location);
            FireSuperWeapon fireSuperWeapon = new FireSuperWeapon(pHouse, cellStruct, data);
            fireSuperWeaponQueue.Enqueue(fireSuperWeapon);
        }

        public void Update()
        {
            for (int i = 0; i < fireSuperWeaponQueue.Count; i++)
            {
                FireSuperWeapon fireSuperWeapon = fireSuperWeaponQueue.Dequeue();
                if (fireSuperWeapon.CanLaunch())
                {
                    LaunchSuperWeapons(fireSuperWeapon.House.Pointer, fireSuperWeapon.Location, fireSuperWeapon.Data);
                    fireSuperWeapon.Cooldown();
                }
                if (!fireSuperWeapon.IsDone())
                {
                    fireSuperWeaponQueue.Enqueue(fireSuperWeapon);
                }
            }
        }

        private void LaunchSuperWeapons(Pointer<HouseClass> pHouse, CellStruct targetPos, FireSuperWeaponsData data)
        {
            if (null != data && data.Enable)
            {
                // Check House alive
                if (pHouse.IsNull || pHouse.Ref.Defeated)
                {
                    // find civilian
                    pHouse = HouseClass.FindBySideName("Civilian");
                    if (pHouse.IsNull)
                    {
                        Logger.LogWarning("Want to fire a super weapon {0}, but house is null.", data.SuperWeapons.ToArray());
                        return;
                    }
                }
                // Get the TargetCell location
                foreach (string superWeaponId in data.SuperWeapons)
                {
                    Pointer<SuperWeaponTypeClass> pType = SuperWeaponTypeClass.ABSTRACTTYPE_ARRAY.Find(superWeaponId);
                    if (!pType.IsNull)
                    {
                        Pointer<SuperClass> pSuper = pHouse.Ref.FindSuperWeapon(pType);
                        if (pSuper.Ref.IsCharged || !data.RealLaunch)
                        {
                            pSuper.Ref.IsCharged = true;
                            pSuper.Ref.Launch(targetPos, true);
                            pSuper.Ref.IsCharged = false;
                            pSuper.Ref.Reset();
                        }
                    }
                }
            }
        }


    }

}