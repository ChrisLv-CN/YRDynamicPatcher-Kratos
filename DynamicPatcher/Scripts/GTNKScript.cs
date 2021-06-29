
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

        DirStruct toDir;

        public override void OnUpdate()
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            if (pTechno.Ref.Base.IsSelected)
            {
                // Logger.Log("Techno {0}", pTechno.Convert<AbstractClass>().Ref.IsInAir() ? "Is InAir" : (pTechno.Convert<AbstractClass>().Ref.IsOnFloor() ? "Is OnFloor":"unknow"));
                //FacingStruct face = pTechno.Ref.GetRealFacing();
                //if (!face.in_motion())
                //{
                //    if (ExHelper.Dir2FacingIndex(toDir, 8) != ExHelper.Dir2FacingIndex(face.current(), 8))
                //    {
                //        Logger.Log("当前面向{0}, 转向{1}", face.target().value8(), toDir.value8());
                //        face.turn(toDir);
                //        pTechno.Convert<FootClass>().Ref.Locomotor.Ref.Do_Turn(toDir);
                //    }
                //}
            }
        }

        public override void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            CoordStruct sourcePos = pTechno.Ref.Base.Base.GetCoords();
            CoordStruct targetPos = pTarget.Ref.GetCoords();
            Pointer<WeaponTypeClass> pWeapon = WeaponTypeClass.ABSTRACTTYPE_ARRAY.Find("RedEye");

            Pointer<BulletTypeClass> pBulletType = BulletTypeClass.ABSTRACTTYPE_ARRAY.Find("AAHeatSeeker2");

            CellStruct targetCell = MapClass.Coord2Cell(targetPos);
            CellSpreadEnumerator cells = new CellSpreadEnumerator(6);
            foreach (CellStruct offset in cells)
            {
                CoordStruct where = MapClass.Cell2Coord(targetCell + offset) + new CoordStruct(0, 0, targetPos.Z);
                if (MapClass.Instance.TryGetCellAt(where, out Pointer<CellClass> pCell))
                {
                    //CoordStruct cellPos = pCell.Convert<AbstractClass>().Ref.GetCoords();
                    //LaserHelper.DrawLine(targetPos, where, 2, 150);
                    //LaserHelper.DrawLine(cellPos, cellPos + new CoordStruct(0, 0, 1000), 2, 150);
                    Pointer<BulletClass> pBullet = pBulletType.Ref.CreateBullet(pCell.Convert<AbstractClass>(), pTechno, pWeapon.Ref.Damage, pWeapon.Ref.Warhead, pWeapon.Ref.Speed, pWeapon.Ref.Bright);
                    pBullet.Ref.WeaponType = pWeapon;
                    BulletVelocity velocity = new BulletVelocity(0, 0, 0);
                    pBullet.Ref.MoveTo(sourcePos, velocity);
                    // pBullet.Ref.SetTarget(pCell.Convert<AbstractClass>());
                }
            }
        }

        public override void OnRemove()
        {
            // Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            // Logger.Log("Techno IsAlive={0} IsActive={1}, IsOnMap={2}", pTechno.Ref.Base.IsAlive, pTechno.Ref.Base.IsActive(), pTechno.Ref.Base.IsOnMap);
        }
    }

}

