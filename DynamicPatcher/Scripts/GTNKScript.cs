
using System.Net;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Script;
using Extension.Decorators;
using Extension.Utilities;
using System.Threading.Tasks;

namespace Scripts
{
    [Serializable]
    public class GTNKScript : TechnoScriptable
    {
        public GTNKScript(TechnoExt owner) : base(owner) { }

        int rof = 150;
        public override void OnUpdate()
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            if (--rof < 0)
            {
                rof = 150;
                Pointer<WeaponStruct> pSec = pTechno.Ref.GetWeapon(1);
                if (!pSec.IsNull)
                {
                    CoordStruct sourcePos = pTechno.Convert<AbstractClass>().Ref.GetCoords();
                    CoordStruct flh = new CoordStruct(5 * 256, 0, 0);
                    CoordStruct targetPos = ExHelper.GetFLHAbsoluteCoords(pTechno, flh, true);
                    CellStruct cur = MapClass.Coord2Cell(sourcePos);
                    CellSpreadEnumerator cells = new CellSpreadEnumerator(6);
                    List<Pointer<UnitClass>> targets = new List<Pointer<UnitClass>>();
                    foreach (CellStruct offset in cells)
                    {
                        CoordStruct where = MapClass.Cell2Coord(cur + offset);
                        if (MapClass.Instance.TryGetCellAt(where, out Pointer<CellClass> pCell))
                        {
                            Pointer<UnitClass> pUnit = pCell.Ref.GetUnit(false);
                            if (!pUnit.IsNull)
                            {
                                targets.Add(pUnit);
                            }
                        }
                    }
                    for (int i = 0; i < targets.Count; i++)
                    {
                        Pointer<UnitClass> pUnit = targets[i];
                        Logger.Log("朝目标{0}开火, 0 = {1}, 1 = {2}", i, pTechno.Ref.GetROF(0), pTechno.Ref.GetROF(1));
                        pTechno.Ref.Fire_IgnoreType(pUnit.Convert<AbstractClass>(), 1);
                    }
                    Logger.Log("");
                    // pTechno.Ref.Fire(pCell.Convert<AbstractClass>(), 1);
                }
            }

            if (pTechno.Ref.Base.IsSelected)
            {

            }
        }

        public override void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            Logger.Log("[{0}]{1} Fire. 武器ROF: 0 = {2}, 1 = {3}, ReloadTimer = {4}", pTechno.Ref.Owner.Ref.Type.Ref.Base.ID, pTechno.Ref.Type.Convert<AbstractTypeClass>().Ref.ID, pTechno.Ref.GetROF(0), pTechno.Ref.GetROF(1), pTechno.Ref.ReloadTimer);
        }

        public override void OnRemove()
        {
            // Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            // Logger.Log("Techno IsAlive={0} IsActive={1}, IsOnMap={2}", pTechno.Ref.Base.IsAlive, pTechno.Ref.Base.IsActive(), pTechno.Ref.Base.IsOnMap);
        }
    }

}

