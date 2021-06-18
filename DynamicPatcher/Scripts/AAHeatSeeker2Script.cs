
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
        int angle;

        public override void OnUpdate()
        {
            Pointer<BulletClass> pBullet = Owner.OwnerObject;
            
            const int radius = 100;
            pBullet.Ref.Base.Location +=
                new CoordStruct((int)(Math.Cos(angle * Math.PI / 180) * radius), (int)(Math.Sin(angle * Math.PI / 180) * radius), 100)
                 * (pBullet.Ref.Velocity.Z > -20 ? 1 : -1);
            angle = (angle + 25) % 360;

        }
    }
}