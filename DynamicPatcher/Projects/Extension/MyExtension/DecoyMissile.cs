using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    [Serializable]
    public class DecoyBullet
    {
        public SwizzleablePointer<BulletClass> Bullet;

        public CoordStruct LaunchPort;

        public int Life;

        public DecoyBullet(Pointer<BulletClass> pBullet, CoordStruct launchPort, int life = 150)
        {
            this.Bullet = new SwizzleablePointer<BulletClass>(pBullet);
            this.LaunchPort = launchPort;
            this.Life = life;
        }

        public bool IsNotDeath()
        {
            if (--Life <= 0)
            {
                // Is death
                if (!Bullet.IsNull)
                {
                    CoordStruct location = Bullet.Ref.Base.Location;
                    Bullet.Ref.Detonate(location);
                    Bullet.Ref.Base.Remove();
                    Bullet.Ref.Base.UnInit();
                }
                Bullet = default;
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return string.Format("{{\"Bullet\": {0}, \"LaunchPort\": {1}, \"Life\": {2}}}", Bullet.Pointer, LaunchPort, Life);
        }

    }

    [Serializable]
    public class DecoyMissile
    {
        public bool Enable;

        // data
        public DecoyMissileData Data;

        public SwizzleablePointer<WeaponTypeClass> Weapon;
        public SwizzleablePointer<WeaponTypeClass> EliteWeapon;

        public SwizzleablePointer<WeaponTypeClass> UseWeapon;
        // public int Damage;
        // public int Burst;
        // public int ROF;
        // public int Distance;

        // control
        public int Delay;
        public int Bullets;
        public int Reloading;
        public bool Fire;

        public bool Elite;

        public List<DecoyBullet> Decoys;

        public DecoyMissile(DecoyMissileData data, Pointer<WeaponTypeClass> pWeapon, Pointer<WeaponTypeClass> pEliteWeapon, bool elite = false)
        {
            this.Enable = true;
            this.Data = data;
            this.Weapon = new SwizzleablePointer<WeaponTypeClass>(pWeapon);
            this.EliteWeapon = new SwizzleablePointer<WeaponTypeClass>(pEliteWeapon);
            if (elite)
            {
                this.UseWeapon = this.EliteWeapon;
            }
            else
            {
                this.UseWeapon = this.Weapon;
            }
            this.Delay = data.Delay;
            this.Bullets = UseWeapon.Ref.Burst;
            this.Reloading = 0;
            this.Fire = data.AlwaysFire;
            this.Elite = elite;
            Decoys = new List<DecoyBullet>();
        }

        public Pointer<WeaponTypeClass> FindWeapon(bool elite)
        {
            if (this.Elite != elite)
            {
                if (elite)
                {
                    this.UseWeapon = this.EliteWeapon;
                }
                else
                {
                    this.UseWeapon = this.Weapon;
                }
                this.Elite = elite;
            }
            return this.UseWeapon;
        }

        public bool DropOne()
        {
            if (--this.Reloading <= 0 && --this.Delay <= 0)
            {
                if (--this.Bullets >= 0)
                {
                    this.Delay = Data.Delay;
                    return true;
                }
                Reload();
            }
            return false;
        }

        public void Reload()
        {
            this.Bullets = this.UseWeapon.Ref.Burst;
            this.Reloading = this.UseWeapon.Ref.ROF;
            this.Fire = this.Data.AlwaysFire;
        }

        public void AddDecoy(Pointer<BulletClass> pDecoy, CoordStruct launchPort, int life)
        {
            if (null == Decoys)
                Decoys = new List<DecoyBullet>();
            DecoyBullet decoy = new DecoyBullet(pDecoy, launchPort, life);
            Decoys.Add(decoy);
        }

        public void ClearDecoy()
        {
            Decoys.RemoveAll((deocy) =>
            {
                return deocy.Life <= 0 || deocy.Bullet.IsNull || !deocy.Bullet.Ref.Base.IsAlive;
            });
        }

        public Pointer<BulletClass> RandomDecoy()
        {
            int count = 0;
            if (null != Decoys && (count = Decoys.Count) > 0)
            {
                int ans = ExHelper.Random.Next(count);
                DecoyBullet decoy = Decoys[ans == count ? ans - 1 : ans];
                Decoys.Remove(decoy);
                return decoy.Bullet;
            }
            return Pointer<BulletClass>.Zero;
        }

        public Pointer<BulletClass> CloseEnoughDecoy(CoordStruct pos, double min)
        {
            int index = -1;
            double distance = min;
            for (int i = 0; i < Decoys.Count; i++)
            {
                DecoyBullet decoy = Decoys[i];
                CoordStruct location = pos;
                double x = 0;
                if (!decoy.Bullet.IsNull
                    && (x = pos.DistanceFrom(decoy.Bullet.Ref.Base.Location)) < distance)
                {
                    distance = x;
                    index = i;
                }
            }
            return index >= 0 ? Decoys[index].Bullet.Pointer : default;
        }
    }

    [Serializable]
    public class DecoyMissileData
    {
        public bool Enable;
        public string Weapon;
        public string EliteWeapon;
        public CoordStruct FLH;
        public CoordStruct Velocity;
        public int Delay;
        public int Life;
        public bool AlwaysFire;

        public DecoyMissileData(string weapon)
        {
            this.Enable = true;
            this.Weapon = weapon;
            this.EliteWeapon = weapon;
            this.FLH = default;
            this.Velocity = default;
            this.Delay = 4;
            this.Life = 99999;
            this.AlwaysFire = false;
        }
    }

    public partial class TechnoExt
    {
        public DecoyMissile decoyMissile;

        public unsafe void TechnoClass_Init_DecoyMissile()
        {
            ref DecoyMissileData data = ref Type.DecoyMissileData;
            if (null != data && data.Enable && null == decoyMissile)
            {
                Pointer<WeaponTypeClass> pWeapon = WeaponTypeClass.ABSTRACTTYPE_ARRAY.Find(data.Weapon);
                Pointer<WeaponTypeClass> pEliteWeapon = WeaponTypeClass.ABSTRACTTYPE_ARRAY.Find(data.Weapon);
                if (default == data.Velocity && pWeapon.Ref.Projectile.Ref.Arcing)
                {
                    data.Velocity = data.FLH;
                }
                decoyMissile = new DecoyMissile(data, pWeapon, pEliteWeapon, OwnerObject.Ref.Veterancy.IsElite());
            }
        }

        public unsafe void TechnoClass_Update_DecoyMissile()
        {
            if (null != decoyMissile && decoyMissile.Enable && !decoyMissile.Weapon.IsNull && !decoyMissile.EliteWeapon.IsNull)
            {
                Pointer<TechnoClass> pTechno = OwnerObject;
                CoordStruct location = pTechno.Ref.Base.Base.GetCoords();
                Pointer<HouseClass> pHouse = pTechno.Ref.Owner;
                Pointer<WeaponTypeClass> pWeapon = decoyMissile.FindWeapon(pTechno.Ref.Veterancy.IsElite());
                int distance = pWeapon.Ref.Range;
                // change decoy speed and target.
                for (int i = 0; i < decoyMissile.Decoys.Count; i++)
                {
                    // Check life count down;
                    DecoyBullet decoy = decoyMissile.Decoys[i];
                    if (decoy.IsNotDeath())
                    {
                        // Check distance to Change speed and target point
                        Pointer<BulletClass> pBullet = decoy.Bullet;
                        if (null != pBullet && !pBullet.IsNull)
                        {
                            int speed = pBullet.Ref.Speed - 5;
                            pBullet.Ref.Speed = speed < 10 ? 10 : speed;
                            if (speed > 10 && decoy.LaunchPort.DistanceFrom(pBullet.Ref.Base.Location) <= distance)
                            {
                                pBullet.Ref.Base.Location += new CoordStruct(0, 0, 64);
                            }

                        }
                    }
                    decoyMissile.Decoys[i] = decoy;
                }

                // remove dead decoy
                decoyMissile.ClearDecoy();

                // Fire decoy
                if (decoyMissile.Fire)
                {

                    if (decoyMissile.DropOne())
                    {
                        FacingStruct facing = pTechno.Ref.GetRealFacing();

                        CoordStruct flhL = decoyMissile.Data.FLH;
                        if (flhL.Y > 0)
                        {
                            flhL.Y = -flhL.Y;
                        }
                        CoordStruct flhR = decoyMissile.Data.FLH;
                        if (flhR.Y < 0)
                        {
                            flhR.Y = -flhR.Y;
                        }

                        CoordStruct portL = ExHelper.GetFLH(location, flhL, facing.target());
                        CoordStruct portR = ExHelper.GetFLH(location, flhR, facing.target());

                        CoordStruct targetFLHL = flhL + new CoordStruct(0, -distance * 2, 0);
                        CoordStruct targetFLHR = flhR + new CoordStruct(0, distance * 2, 0);
                        CoordStruct targetL = ExHelper.GetFLH(location, targetFLHL, facing.target());
                        CoordStruct targetR = ExHelper.GetFLH(location, targetFLHR, facing.target());

                        CoordStruct vL = decoyMissile.Data.Velocity;
                        if (vL.Y > 0)
                        {
                            vL.Y = -vL.Y;
                        }
                        vL.Z *= 2;
                        CoordStruct vR = decoyMissile.Data.Velocity;
                        if (vR.Y < 0)
                        {
                            vR.Y = -vR.Y;
                        }
                        vR.Z *= 2;
                        CoordStruct velocityL = ExHelper.GetFLH(new CoordStruct(), vL, facing.target());
                        CoordStruct velocityR = ExHelper.GetFLH(new CoordStruct(), vR, facing.target());

                        for (int i = 0; i < 2; i++)
                        {
                            CoordStruct initTarget = targetL;
                            CoordStruct port = portL;
                            BulletVelocity velocity = new BulletVelocity(velocityL.X, velocityL.Y, velocityL.Z);
                            if (i > 0)
                            {
                                initTarget = targetR;
                                port = portR;
                                velocity = new BulletVelocity(velocityR.X, velocityR.Y, velocityR.Z);
                            }
                            Pointer<CellClass> pCell = MapClass.Instance.GetCellAt(initTarget);
                            Pointer<BulletClass> pBullet = pWeapon.Ref.Projectile.Ref.CreateBullet(pCell.Convert<AbstractClass>(), pTechno, pWeapon.Ref.Damage, pWeapon.Ref.Warhead, pWeapon.Ref.Speed, pWeapon.Ref.Bright);
                            pBullet.Ref.WeaponType = pWeapon;
                            pBullet.Ref.MoveTo(port, velocity);
                            decoyMissile.AddDecoy(pBullet, port, decoyMissile.Data.Life);
                        }
                    }
                    ExHelper.FindBulletTargetMe(pTechno, (pBullet) =>
                    {
                        CoordStruct pos = pBullet.Ref.Base.Location;
                        Pointer<BulletClass> pDecoy = decoyMissile.CloseEnoughDecoy(pos, location.DistanceFrom(pos));
                        if (null != pDecoy && !pDecoy.IsNull && pDecoy.Ref.Base.IsActive() && pDecoy.Ref.Base.IsAlive
                            && pDecoy.Ref.Base.Location.DistanceFrom(pBullet.Ref.Base.Location) <= distance * 2)
                        {
                            pBullet.Ref.SetTarget(pDecoy.Convert<AbstractClass>());
                            return true;
                        }
                        return false;
                    });

                }
                else
                {
                    ExHelper.FindBulletTargetMe(pTechno, (pBullet) =>
                    {
                        if (location.DistanceFrom(pBullet.Ref.Base.Location) <= distance)
                        {
                            decoyMissile.Fire = true;
                            CoordStruct pos = pBullet.Ref.Base.Location;
                            Pointer<BulletClass> pDecoy = decoyMissile.CloseEnoughDecoy(pos, location.DistanceFrom(pos));
                            if (null != pDecoy && !pDecoy.IsNull && pDecoy.Ref.Base.IsActive() && pDecoy.Ref.Base.IsAlive)
                            {
                                pBullet.Ref.SetTarget(pDecoy.Convert<AbstractClass>());
                            }
                            return true;
                        }
                        return false;
                    });

                }
            }
        }

    }

    public partial class TechnoTypeExt
    {
        public DecoyMissileData DecoyMissileData;

        /// <summary>
        /// [TechnoType]
        /// DecoyMissile.Weapon=DecoyMissile
        /// DecoyMissile.EliteWeapon=DecoyMissile
        /// DecoyMissile.FLH=0,10,150
        /// DecoyMissile.Velocity=0,0,0
        /// DecoyMissile.Delay=4
        /// DecoyMissile.Life=75
        /// DecoyMissile.AlwaysFire=no
        /// 
        /// [DecoyMissile]
        /// Damage=1
        /// ROF=100
        /// Range=3
        /// Speed=40
        /// Burst=10
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadDecoyMissile(INIReader reader, string section)
        {
            string weapon = null;
            if (reader.ReadNormal(section, "DecoyMissile.Weapon", ref weapon))
            {
                DecoyMissileData = new DecoyMissileData(weapon);

                string eliteWeapon = default;
                if (reader.ReadNormal(section, "DecoyMissile.EliteWeapon", ref eliteWeapon))
                {
                    DecoyMissileData.EliteWeapon = eliteWeapon;
                }

                CoordStruct flh = default;
                if (ExHelper.ReadCoordStruct(reader, section, "DecoyMissile.FLH", ref flh))
                {
                    DecoyMissileData.FLH = flh;
                }

                CoordStruct velocity = default;
                if (ExHelper.ReadCoordStruct(reader, section, "DecoyMissile.Velocity", ref velocity))
                {
                    DecoyMissileData.Velocity = velocity;
                }

                int delay = 0;
                if (reader.ReadNormal(section, "DecoyMissile.Delay", ref delay))
                {
                    DecoyMissileData.Delay = delay;
                }

                int life = 0;
                if (reader.ReadNormal(section, "DecoyMissile.Life", ref life))
                {
                    DecoyMissileData.Life = life;
                }

                bool alwaysFire = false;
                if (reader.ReadNormal(section, "DecoyMissile.AlwaysFire", ref alwaysFire))
                {
                    DecoyMissileData.AlwaysFire = alwaysFire;
                }
            }
        }
    }

}