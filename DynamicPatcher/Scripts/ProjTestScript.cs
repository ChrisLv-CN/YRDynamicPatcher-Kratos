
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
    public class ProjTest : BulletScriptable
    {
        bool flag = false;

        public ProjTest(BulletExt owner) : base(owner) {
            Pointer<BulletClass> pBullet = owner.OwnerObject;
            BulletVelocity initVelocity = pBullet.Ref.Velocity;
            CoordStruct targetPos = pBullet.Ref.TargetCoords;
            CoordStruct sourcePos = pBullet.Convert<AbstractClass>().Ref.GetCoords();
            // LaserHelper.DrawLine(sourcePos, targetPos, 2, 450);
            CoordStruct targetHeightPos = targetPos + new CoordStruct(0, 0, 1280);
            LaserHelper.DrawLine(targetPos, targetHeightPos, 2, 450);
            // Logger.Log("Bullet init velocity {0}", initVelocity);

            // CoordStruct p1 = sourcePos;
            // CoordStruct p2 = targetPos;
            // p1.Z = 0;
            // p2.Z = 0;
            // double distance = p1.DistanceFrom(p2);
            // double speed = pBullet.Ref.Speed;
            // double time = distance / speed;
            // double vZ = ((targetPos.Z - sourcePos.Z) * speed) / distance + (0.5 * 6 * distance) / speed;
            // BulletVelocity v = new BulletVelocity(p2.X - p1.X, p2.Y - p1.Y, 0);
            // v *= 1 / time;
            // v.Z = vZ; // Math.Abs(targetPos.Z - sourcePos.Z) + 6 * time;
            // Logger.Log("Bullet time {0} velocity {1}, init{2}, vZ={3}", time, v, initVelocity, vZ);
            // pBullet.Ref.Velocity = v;
        }

        public override void OnUpdate()
        {
            Pointer<BulletClass> pBullet = Owner.OwnerObject;
            if (!flag)
            {
                flag = true;
                CoordStruct targetPos = pBullet.Ref.TargetCoords;
                CoordStruct sourcePos = pBullet.Convert<AbstractClass>().Ref.GetCoords();
                int zDiff = targetPos.Z - sourcePos.Z;
                targetPos.Z = 0;
                sourcePos.Z = 0;
                double distance = targetPos.DistanceFrom(sourcePos);
                Logger.Log("Speed = {0}, weaponSpeed = {1}", pBullet.Ref.Speed, pBullet.Ref.WeaponType.Ref.Speed);
                pBullet.Ref.Speed = pBullet.Ref.WeaponType.Ref.Speed;
                double speed = pBullet.Ref.Speed;
                double vZ = (zDiff * speed) / distance + (0.5 * RulesClass.Global().Gravity * distance) / speed;
                BulletVelocity v = new BulletVelocity(targetPos.X - sourcePos.X, targetPos.Y - sourcePos.Y, 0);
                v *= speed / distance;
                v.Z = vZ;
                pBullet.Ref.Velocity = v;
            }
        }
    }
}