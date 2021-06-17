
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Script;
using System.Threading.Tasks;

namespace Scripts
{
    [Serializable]
    public class AAHeatSeeker2 : BulletScriptable
    {
        public AAHeatSeeker2(BulletExt owner) : base(owner) {}
        
        Random random = new Random();
        static ColorStruct innerColor = new ColorStruct(208,10,20);
        static ColorStruct outerColor = new ColorStruct(88, 0, 20);
        static ColorStruct outerSpread = new ColorStruct(10, 10, 10);

        CoordStruct lastLocation;
        int angle;

        public override void OnUpdate()
        {
            Pointer<BulletClass> pBullet = Owner.OwnerObject;
            BulletTypeExt extType = Owner.Type;
            CoordStruct location = pBullet.Ref.Base.Base.GetCoords();
            // if (lastLocation == location)
            // {
            int arm = pBullet.Ref.Type.Ref.Arm;
            Pointer<AbstractClass> pTarget = pBullet.Ref.Target;
            CoordStruct targetLocation = pTarget.Ref.GetCoords();
            CoordStruct targetPos = pBullet.Ref.TargetCoords;
            CellStruct targetPosCell = MapClass.Coord2Cell(targetPos);
            CellStruct targetCell = MapClass.Coord2Cell(targetLocation);
            CellStruct bulletCell = MapClass.Coord2Cell(location);
            Logger.Log("bulletPos {0}, target {1}, Distance {2}, Arm {3}", location, targetLocation, location.DistanceFrom(targetLocation), arm);
            Logger.Log("bulletCell pos {0}, targetCell pos {1}, Distance {2}, Arm {3}", bulletCell, targetCell, bulletCell.DistanceFrom(targetCell), arm);
            Logger.Log("bulletPos {0}, targetPos {1}, Distance {2}, Arm {3}", location, targetPos, location.DistanceFrom(targetPos), arm);
            Logger.Log("bulletCell pos {0}, targetPosCell ois {1}, Distance {2}, Arm {3}", bulletCell, targetPosCell, bulletCell.DistanceFrom(targetPosCell), arm);
            Logger.Log("-");
            // }
            // lastLocation = location;

            // location.Z += 0;
            // if (lastLocation == default(CoordStruct)) {
            //     lastLocation = location;
            // }
            // if (lastLocation.DistanceFrom(location) > 50)
            // {
            //     Pointer<LaserDrawClass> pLaser = YRMemory.Create<LaserDrawClass>(lastLocation, location, innerColor, outerColor, outerSpread, 10);
            //     pLaser.Ref.Thickness = 6;
            //     pLaser.Ref.IsHouseColor = true;
            //     lastLocation = location;
            // }

            // const int radius = 100;
            // pBullet.Ref.Base.Location +=
            //     new CoordStruct((int)(Math.Cos(angle * Math.PI / 180) * radius), (int)(Math.Sin(angle * Math.PI / 180) * radius), 100)
            //      * (pBullet.Ref.Velocity.Z > -20 ? 1 : -1);
            // angle = (angle + 25) % 360;

        }
    }
}