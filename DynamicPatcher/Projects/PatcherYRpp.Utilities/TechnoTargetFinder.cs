using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp.Utilities
{
    public static class TechnoTargetFinder
    {
        public static bool IsAttackable(ref this ObjectClass obj)
        {
            if (!obj.IsOnMap)
                return false;

            var pObject = Pointer<ObjectClass>.AsPointer(ref obj);

            if (pObject.CastToTechno(out var pTechno))
            {
                if (!pTechno.Ref.IsInPlayfield)
                    return false;

            }

            return true;
        }

        public static bool CanAttack(ref this TechnoClass pThis, Pointer<AbstractClass> pTarget)
        {
            int idxWeapon = pThis.SelectWeapon(pTarget);

            FireError fireError = pThis.GetFireError(pTarget, idxWeapon, true);
            switch (fireError)
            {
                case FireError.ILLEGAL:
                case FireError.CANT:
                    return false;
            }

            return true;
        }

        public static bool CanAttack(ref this TechnoClass pThis, Pointer<ObjectClass> pObject)
        {
            return pThis.CanAttack(pObject.Convert<AbstractClass>());
        }
        public static bool CanAttack(ref this TechnoClass pThis, Pointer<TechnoClass> pTechno)
        {
            return pThis.CanAttack(pTechno.Convert<AbstractClass>());
        }
        public static bool CanAttack(ref this TechnoClass pThis, Pointer<FootClass> pFoot)
        {
            return pThis.CanAttack(pFoot.Convert<AbstractClass>());
        }
        public static bool CanAttack(ref this TechnoClass pThis, Pointer<BuildingClass> pBuilding)
        {
            return pThis.CanAttack(pBuilding.Convert<AbstractClass>());
        }
        public static bool CanAttack(ref this TechnoClass pThis, Pointer<InfantryClass> pInfantry)
        {
            return pThis.CanAttack(pInfantry.Convert<AbstractClass>());
        }
        public static bool CanAttack(ref this TechnoClass pThis, Pointer<UnitClass> pUnit)
        {
            return pThis.CanAttack(pUnit.Convert<AbstractClass>());
        }
        public static bool CanAttack(ref this TechnoClass pThis, Pointer<TerrainClass> pTerrain)
        {
            return pThis.CanAttack(pTerrain.Convert<AbstractClass>());
        }

        public static void Attack(ref this TechnoClass pThis, Pointer<AbstractClass> pTarget)
        {
            //Mission mission = pThis.CanAttackOnTheMove() ? Mission.AttackMove : Mission.Attack;

            pThis.SetTarget(pTarget);
            pThis.BaseMission.QueueMission(Mission.Attack, false);
            pThis.BaseMission.NextMission();
        }

        public static void AttackMove(ref this FootClass pThis, Pointer<AbstractClass> pTarget, Pointer<AbstractClass> pToMove)
        {
            pThis.Base.SetDestination(pToMove);
            pThis.Base.Attack(pTarget);
        }
        public static void AttackMove(ref this FootClass pThis, Pointer<AbstractClass> pTarget, Pointer<CellClass> pCellToMove)
        {
            pThis.AttackMove(pTarget, pCellToMove.Convert<AbstractClass>());
        }

        // not really right
        public static bool CanAttackMove(ref this TechnoClass pThis)
        {
            if (pThis.Base.Base.WhatAmI() == AbstractType.Aircraft)
            {
                return true;
            }

            return pThis.CanAttackOnTheMove();
        }

        public static List<Pointer<TechnoClass>> FindAttackTechnos(ref this TechnoClass pThis, int range, bool includeFriends = false)
        {
            CoordStruct location = pThis.BaseAbstract.GetCoords();
            return pThis.FindAttackTechnos(location, range, includeFriends);
        }
        public static List<Pointer<TechnoClass>> FindAttackTechnos(ref this TechnoClass pThis, CoordStruct location, int range, bool includeFriends = false)
        {
            var list = new List<Pointer<TechnoClass>>();

            Pointer<HouseClass> pOwner = pThis.Owner;

            var objects = ObjectFinder.FindTechnosNear(location, range);
            if (!includeFriends)
            {
                objects = objects.Where(o => !pOwner.Ref.IsAlliedWith(o.Ref.Base.GetOwningHouse())).ToList();
            }

            foreach (var o in objects)
            {
                if (pThis.CanAttack(o))
                {
                    list.Add(o.Convert<TechnoClass>());
                }
            }

            return list;
        }
    }
}
