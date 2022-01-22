using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp.Utilities
{
    public static class BulletFactory
    {
        public static bool TryCreateBullet(CoordStruct targetCoord, Pointer<WeaponTypeClass> pWeapon, Pointer<TechnoClass> pOwner, out Pointer<BulletClass> pBullet)
        {
            pBullet = Pointer<BulletClass>.Zero;

            if (MapClass.Instance.TryGetCellAt(targetCoord, out var pCell))
            {
                pBullet = CreateBullet(pCell.Convert<AbstractClass>(), pWeapon, pOwner);
            }

            return pBullet != Pointer<BulletClass>.Zero;
        }

        public static Pointer<BulletClass> CreateBullet(Pointer<AbstractClass> pTarget, Pointer<WeaponTypeClass> pWeapon, Pointer<TechnoClass> pOwner)
        {
            Pointer<BulletClass> pBullet = CreateBullet(
                pTarget, pOwner, pWeapon.Ref.projectile,
                pWeapon.Ref.Damage, pWeapon.Ref.Warhead,
                pWeapon.Ref.Speed, pWeapon.Ref.Bright);

            pBullet.Ref.WeaponType = pWeapon;

            return pBullet;
        }

        public static Pointer<BulletClass> CreateBullet(Pointer<AbstractClass> pTarget, Pointer<TechnoClass> pOwner, Pointer<BulletTypeClass> pBulletType, int damage, Pointer<WarheadTypeClass> pWarhead, int speed, bool bright)
        {
            if (pTarget.IsNull)
                throw new ArgumentNullException(nameof(pTarget));
            if (pWarhead.IsNull)
                throw new ArgumentNullException(nameof(pWarhead));

            Pointer<BulletClass> pBullet = pBulletType.Ref.CreateBullet(pTarget, pOwner, damage, pWarhead, speed, bright);
            return pBullet;
        }


    }
}
