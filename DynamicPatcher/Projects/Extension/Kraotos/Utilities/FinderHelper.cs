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

    public delegate bool FoundBullet(Pointer<BulletClass> pBullet);
    public delegate bool FoundTechno(Pointer<TechnoClass> pTechno);
    public delegate bool FoundAircraft(Pointer<AircraftClass> pAircraft);

    public static partial class ExHelper
    {
        
        public static void FindBulletTargetHouse(Pointer<TechnoClass> pTechno, FoundBullet func, bool allied = true)
        {
            ref DynamicVectorClass<Pointer<BulletClass>> bullets = ref BulletClass.Array;
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                Pointer<BulletClass> pBullet = bullets.Get(i);
                if (pBullet.IsDeadOrInvisible()
                    || pBullet.Ref.Type.Ref.Inviso
                    || null == pBullet.Ref.Owner || pBullet.Ref.Owner.IsNull || pBullet.Ref.Owner.Ref.Owner == pTechno.Ref.Owner
                    || (allied && pBullet.Ref.Owner.Ref.Owner.Ref.IsAlliedWith(pTechno.Ref.Owner)))
                {
                    continue;
                }

                if (func(pBullet))
                {
                    break;
                }
            }
        }

        public static void FindBulletTargetMe(Pointer<TechnoClass> pTechno, FoundBullet func, bool allied = true)
        {
            FindBulletTargetHouse(pTechno, (pBullet) =>
            {
                if (pBullet.Ref.Target != pTechno.Convert<AbstractClass>())
                {
                    return false;
                }
                return func(pBullet);
            }, allied);
        }

        public static void FindOwnerTechno(Pointer<HouseClass> pHouse, FoundTechno func, bool allied = false, bool enemies = false)
        {
            FindTechno(pHouse, func, true, allied, enemies);
        }

        public static void FindTechno(Pointer<HouseClass> pHouse, FoundTechno func, bool owner = true, bool allied = false, bool enemies = false, bool civilian = false)
        {
            ref DynamicVectorClass<Pointer<TechnoClass>> technos = ref TechnoClass.Array;
            for (int i = technos.Count - 1; i >= 0; i--)
            {
                Pointer<TechnoClass> pTechno = technos.Get(i);
                if (pTechno.IsDeadOrInvisible()
                    // || (pTechno.Ref.Base.IsActive() ? false : !civilian) // ObjectClass.IsActive() 会导致联机不同步
                    || null == pTechno.Ref.Owner || pTechno.Ref.Owner.IsNull
                    || (pTechno.Ref.Owner == pHouse ? !owner : (pTechno.Ref.Owner.Ref.IsAlliedWith(pHouse) ? !allied : !enemies)))
                {
                    continue;
                }

                if (func(pTechno))
                {
                    break;
                }
            }

        }

        public static void FindAircraft(Pointer<HouseClass> pHouse, FoundAircraft func, bool owner = true, bool allied = false, bool enemies = false, bool civilian = false)
        {
            ref DynamicVectorClass<Pointer<AircraftClass>> aircrafts = ref AircraftClass.Array;
            for (int i = aircrafts.Count - 1; i >= 0; i--)
            {
                Pointer<AircraftClass> pAircraft = aircrafts.Get(i);
                if (pAircraft.Convert<TechnoClass>().IsDeadOrInvisible()
                    // || (pAircraft.Ref.Base.Base.Base.IsActive() ? false : !civilian) // ObjectClass.IsActive() 会导致联机不同步
                    || null == pAircraft.Ref.Base.Base.Owner || pAircraft.Ref.Base.Base.Owner.IsNull
                    || (pAircraft.Ref.Base.Base.Owner == pHouse ? !owner : (pAircraft.Ref.Base.Base.Owner.Ref.IsAlliedWith(pHouse) ? !allied : !enemies)))
                {
                    continue;
                }

                if (func(pAircraft))
                {
                    break;
                }
            }

        }

        public static unsafe void FindTechnoInCell(Pointer<CellClass> pCell, FoundTechno found)
        {
            // 获取地面
            Pointer<ObjectClass> pObject = pCell.Ref.GetContent();
            do
            {
                // Logger.Log("Object {0}, Type {1}", pObject, pObject.IsNull ? "is null" : pObject.Ref.Base.WhatAmI());
                if (!pObject.IsNull && pObject.CastToTechno(out Pointer<TechnoClass> pTarget))
                {
                    if (found(pTarget))
                    {
                        break;
                    }
                }
            }
            while (!pObject.IsNull && !(pObject = pObject.Ref.NextObject).IsNull);
        }

        public static List<Pointer<TechnoClass>> GetCellSpreadTechnos(CoordStruct location, double spread, bool includeInAir, bool ignoreBulidingOuter)
        {
            HashSet<Pointer<TechnoClass>> pTechnoSet = new HashSet<Pointer<TechnoClass>>();

            CellStruct cur = MapClass.Coord2Cell(location);
            if (MapClass.Instance.TryGetCellAt(location, out Pointer<CellClass> pCurretCell))
            {
                cur = pCurretCell.Ref.MapCoords;
            }

            uint range = (uint)(spread + 0.99);
            CellSpreadEnumerator enumerator = new CellSpreadEnumerator(range);

            do
            {
                CellStruct offset = enumerator.Current;
                if (MapClass.Instance.TryGetCellAt(cur + offset, out Pointer<CellClass> pCell))
                {
                    FindTechnoInCell(pCell, (pTarget) =>
                    {
                        pTechnoSet.Add(pTarget);
                        return false;
                    });
                }
            } while (enumerator.MoveNext());

            // Logger.Log("range = {0}, pTechnoSet.Count = {1}", range, pTechnoSet.Count);

            if (includeInAir)
            {
                // 获取所有在天上的玩意儿，JJ，飞起来的坦克，包含路过的飞机
                ExHelper.FindTechno(IntPtr.Zero, (pTechno) =>
                {
                    if (pTechno.Ref.Base.GetHeight() > 0 && pTechno.Ref.Base.Location.DistanceFrom(location) <= spread * 256)
                    {
                        pTechnoSet.Add(pTechno.Convert<TechnoClass>());
                    }
                    return false;
                }, true, true, true, true);
            }

            // Logger.Log("includeAir = {0}, pTechnoSet.Count = {1}", includeInAir, pTechnoSet.Count);

            // 筛选并去掉不可用项目
            List<Pointer<TechnoClass>> pTechnoList = new List<Pointer<TechnoClass>>();
            foreach (Pointer<TechnoClass> pTechno in pTechnoSet)
            {
                CoordStruct targetPos = pTechno.Ref.Base.Base.GetCoords();
                double dist = targetPos.DistanceFrom(location);

                bool checkDistance = true;
                AbstractType absType = pTechno.Ref.Base.Base.WhatAmI();
                switch (absType)
                {
                    case AbstractType.Building:
                        if (pTechno.Convert<BuildingClass>().Ref.Type.Ref.InvisibleInGame)
                        {
                            continue;
                        }
                        if (!ignoreBulidingOuter)
                        {
                            checkDistance = false;
                        }
                        break;
                    case AbstractType.Aircraft:
                        if (pTechno.Ref.Base.Base.IsInAir())
                        {
                            dist *= 0.5;
                        }
                        break;
                }

                if (!checkDistance || dist <= spread * 256)
                {
                    pTechnoList.Add(pTechno);
                }
            }
            return pTechnoList;
        }

        public static HashSet<Pointer<BulletClass>> GetCellSpreadBullets(CoordStruct location, double spread)
        {
            HashSet<Pointer<BulletClass>> pBulletSet = new HashSet<Pointer<BulletClass>>();

            double dist = (spread <= 0 ? 1 : Math.Ceiling(spread)) * 256;
            ref DynamicVectorClass<Pointer<BulletClass>> bullets = ref BulletClass.Array;
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                Pointer<BulletClass> pBullet = bullets.Get(i);
                CoordStruct targetLocation = pBullet.Ref.Base.Base.GetCoords();
                if (targetLocation.DistanceFrom(location) <= dist)
                {
                    pBulletSet.Add(pBullet);
                }
            }
            return pBulletSet;
        }



    }

}