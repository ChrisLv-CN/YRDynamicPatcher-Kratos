
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Script;
using Extension.Utilities;
using System.Threading.Tasks;

namespace Scripts
{
    [Serializable]
    public class ORCA : TechnoScriptable
    {

        bool flag = false;
        int dir = 0;

        public ORCA(TechnoExt owner) : base(owner)
        {

        }

        public override void OnPut(Pointer<CoordStruct> pCoord, Direction faceDir)
        {

        }

        public override void OnUpdate()
        {
            /*
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            TechnoTypeExt extType = ext.Type;
            string ID = pTechno.Ref.Type.Ref.Base.Base.UIName;
            string HouseID = pTechno.Ref.Owner.Ref.Type.Ref.Base.UIName;

            CoordStruct location = pTechno.Ref.Base.Base.GetCoords();

            if (pTechno.Ref.Base.IsSelected)
            {
                if (flag)
                {
                    return;
                }
                flag = true;
                Pointer<FootClass> pFoot = pTechno.Convert<FootClass>();
                pFoot.Ref.StopMoving();
                CoordStruct offset = default;
                switch ((Direction)dir)
                {
                    case Direction.N:
                        offset = new CoordStruct(0, -1, 0); // N
                        break;
                    case Direction.NE:
                        offset = new CoordStruct(1, -1, 0); // NE
                        break;
                    case Direction.E:
                        offset = new CoordStruct(1, 0, 0); // E
                        break;
                    case Direction.SE:
                        offset = new CoordStruct(1, 1, 0); // SE
                        break;
                    case Direction.S:
                        offset = new CoordStruct(0, 1, 0); // S
                        break;
                    case Direction.SW:
                        offset = new CoordStruct(-1, 1, 0); // SW
                        break;
                    case Direction.W:
                        offset = new CoordStruct(-1, 0, 0); // W
                        break;
                    case Direction.NW:
                        offset = new CoordStruct(-1, -1, 0); // NW
                        break;
                }
                if (++dir == 8) {
                    dir = 0;
                }
                pFoot.Ref.MoveTo(location + offset);
            }
            else
            {
                flag = false;
            }
            */
        }

        public override void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            CoordStruct location = pTechno.Ref.Base.Base.GetCoords();
            CoordStruct targetPos = pTarget.Ref.GetCoords();
            Point2D p1 = new Point2D(location.X, location.Y);
            Point2D p2 = new Point2D(targetPos.X, targetPos.Y);
            Direction targetDir = Direction.N;
            var angle = Math.Atan2((p2.X - p1.X), (p2.Y - p1.Y));
            var theta = angle * (180 / Math.PI);
            if (theta >= 0)
            {
                if (theta <= 22.5)
                    targetDir = Direction.S;
                else if (theta <= 67.5)
                    targetDir = Direction.SE;
                else if (theta <= 112.5)
                    targetDir = Direction.E;
                else if (theta <= 157.5)
                    targetDir = Direction.NE;
                else
                    targetDir = Direction.N;
            }
            else
            {
                if (theta > -22.5)
                    targetDir = Direction.S;
                else if (theta > -67.5)
                    targetDir = Direction.SW;
                else if (theta > -112.5)
                    targetDir = Direction.W;
                else if (theta > -157.5)
                    targetDir = Direction.NW;
                else
                    targetDir = Direction.N;
            }
            var dirValue = pTechno.Ref.Facing.target().value();
            Direction selfDir = Direction.S;
            if (dirValue >= 0)
            {
                if (dirValue <= 4096)
                    selfDir = Direction.N;
                else if (dirValue <= 12288)
                    selfDir = Direction.NE;
                else if (dirValue <= 20480)
                    selfDir = Direction.E;
                else if (dirValue <= 28672)
                    selfDir = Direction.SE;
                else
                    selfDir = Direction.S;
            }
            else
            {
                if (dirValue > -4096)
                    selfDir = Direction.N;
                else if (dirValue > -12288)
                    selfDir = Direction.NW;
                else if (dirValue > -20480)
                    selfDir = Direction.W;
                else if (dirValue > -28672)
                    selfDir = Direction.SW;
                else
                    selfDir = Direction.S;
            }

            Logger.Log("目标方位{0}，本体方向{1}", targetDir, selfDir);
            if (selfDir != targetDir)
            {
                Pointer<FootClass> pFoot = pTechno.Convert<FootClass>();
                pFoot.Ref.StopMoving();
                CoordStruct offset = default;
                switch (targetDir)
                {
                    case Direction.N:
                        offset = new CoordStruct(0, -1, 0); // N
                        break;
                    case Direction.NE:
                        offset = new CoordStruct(1, -1, 0); // NE
                        break;
                    case Direction.E:
                        offset = new CoordStruct(1, 0, 0); // E
                        break;
                    case Direction.SE:
                        offset = new CoordStruct(1, 1, 0); // SE
                        break;
                    case Direction.S:
                        offset = new CoordStruct(0, 1, 0); // S
                        break;
                    case Direction.SW:
                        offset = new CoordStruct(-1, 1, 0); // SW
                        break;
                    case Direction.W:
                        offset = new CoordStruct(-1, 0, 0); // W
                        break;
                    case Direction.NW:
                        offset = new CoordStruct(-1, -1, 0); // NW
                        break;
                }
                pFoot.Ref.MoveTo(location + offset);
                Logger.Log("需要转向，目标方向{0}，本体方向{1}", targetDir, selfDir);
            }
        }

    }
}

