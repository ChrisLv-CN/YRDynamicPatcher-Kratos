using System.Drawing;
using System.Threading;
using DynamicPatcher;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using Extension.Ext;

namespace Extension.Utilities
{
    public delegate bool FoundBullet(Pointer<BulletClass> pBullet);
    public delegate bool FoundTechno(Pointer<TechnoClass> pTechno);
    public delegate bool FoundAircraft(Pointer<AircraftClass> pAircraft);

    public delegate bool FireBulletToTarget(int index, int burst, Pointer<BulletClass> pBullet, Pointer<AbstractClass> pTarget);

    public static class EXMath
    {
        public const double HalfPI = Math.PI / 2;
        public const double DEG_TO_RAD = Math.PI / 180;
        public const double BINARY_ANGLE_MAGIC = -(360.0 / (65535 - 1)) * DEG_TO_RAD;

        public static double Deg2Rad(double degrees)
        {
            return degrees * DEG_TO_RAD;
        }

        public static double Rad2Deg(double radians)
        {
            return radians / DEG_TO_RAD;
        }
    }

    public static class ExHelper
    {
        public static Random Random = new Random(114514);

        public static bool IsDead(this Pointer<TechnoClass> pTechno)
        {
            return pTechno.IsNull || pTechno.Convert<ObjectClass>().IsDead() || pTechno.Ref.IsCrashing || pTechno.Ref.IsSinking;
        }

        public static bool IsDead(this Pointer<ObjectClass> pObject)
        {
            return pObject.IsNull || pObject.Ref.Health <= 0 || !pObject.Ref.IsAlive;
        }

        public static bool IsInvisible(this Pointer<TechnoClass> pTechno)
        {
            return pTechno.IsNull || pTechno.Convert<ObjectClass>().IsInvisible() || pTechno.Ref.CloakStates == CloakStates.Cloaked || pTechno.Ref.CloakStates == CloakStates.Cloaking;
        }

        public static bool IsInvisible(this Pointer<ObjectClass> pObject)
        {
            return pObject.IsNull || pObject.Ref.InLimbo; // || !pObject.Ref.IsVisible;
        }

        public static bool IsDeadOrInvisible(this Pointer<TechnoClass> pTarget)
        {
            return pTarget.IsDead() || pTarget.IsInvisible();
        }

        public static bool IsDeadOrInvisible(this Pointer<BulletClass> pBullet)
        {
            Pointer<ObjectClass> pObject = pBullet.Convert<ObjectClass>();
            return pObject.IsDead() || pObject.IsInvisible();
        }

        public static int Category(this LandType landType)
        {
            switch (landType)
            {
                case LandType.Rock:
                case LandType.Wall:
                    // 不可用
                    return 0;
                case LandType.Water:
                    // 水
                    return 2;
                case LandType.Beach:
                    // 两栖
                    return 3;
                default:
                    // 陆地
                    return 1;
            }
        }

        public static unsafe Pointer<TechnoClass> CreateAndPutTechno(string id, Pointer<HouseClass> pHouse, CoordStruct location, Pointer<CellClass> pCell = default)
        {
            if (!string.IsNullOrEmpty(id))
            {
                Pointer<TechnoTypeClass> pType = TechnoTypeClass.Find(id);
                if (!pType.IsNull)
                {
                    // 新建单位
                    Pointer<TechnoClass> pTechno = pType.Ref.Base.CreateObject(pHouse).Convert<TechnoClass>();
                    if (!pCell.IsNull || MapClass.Instance.TryGetCellAt(location, out pCell))
                    {
                        // 在目标格子位置刷出单位
                        var occFlags = pCell.Ref.OccupationFlags;
                        pTechno.Ref.Base.OnBridge = pCell.Ref.ContainsBridge();
                        ++Game.IKnowWhatImDoing;
                        pTechno.Ref.Base.Put(pCell.Ref.GetCoordsWithBridge(), Direction.E);
                        --Game.IKnowWhatImDoing;
                        pCell.Ref.OccupationFlags = occFlags;
                        // 单位放到指定的位置
                        pTechno.Ref.Base.SetLocation(location);
                        return pTechno;
                    }
                }
            }
            return IntPtr.Zero;
        }

        public static CoordStruct GetFLH(CoordStruct source, CoordStruct flh, DirStruct dir, bool flip = false)
        {
            CoordStruct res = source;
            if (null != flh && default != flh && null != dir)
            {
                double radians = dir.radians();

                double rF = flh.X;
                double xF = rF * Math.Cos(-radians);
                double yF = rF * Math.Sin(-radians);
                CoordStruct offsetF = new CoordStruct((int)xF, (int)yF, 0);

                double rL = flip ? flh.Y : -flh.Y;
                double xL = rL * Math.Sin(radians);
                double yL = rL * Math.Cos(radians);
                CoordStruct offsetL = new CoordStruct((int)xL, (int)yL, 0);

                res = source + offsetF + offsetL + new CoordStruct(0, 0, flh.Z);
            }
            return res;
        }

        public static unsafe CoordStruct GetFLHAbsoluteCoords(CoordStruct source, CoordStruct flh, DirStruct dir, CoordStruct turretOffset = default)
        {
            CoordStruct res = source;
            if (null != flh && default != flh)
            {
                SingleVector3D offset = GetFLHAbsoluteOffset(flh, dir, turretOffset);
                res += offset.ToCoordStruct();
            }
            return res;
        }

        public static unsafe SingleVector3D GetFLHAbsoluteOffset(CoordStruct flh, DirStruct dir, CoordStruct turretOffset)
        {
            SingleVector3D offset = default;
            if (null != flh && default != flh)
            {
                Matrix3DStruct matrix3D = new Matrix3DStruct(true);
                matrix3D.Translate(turretOffset.X, turretOffset.Y, turretOffset.Z);
                matrix3D.RotateZ((float)dir.radians());
                offset = GetFLHOffset(ref matrix3D, flh);
            }
            return offset;
        }

        public static unsafe CoordStruct GetFLHAbsoluteCoords(Pointer<TechnoClass> pTechno, CoordStruct flh, bool isOnTurret = true, int flipY = 1, bool nextFrame = true)
        {
            CoordStruct turretOffset = default;
            if (isOnTurret)
            {
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext)
                {
                    turretOffset = ext.Type.TurretOffset;
                }
                else
                {
                    turretOffset.X = pTechno.Ref.Type.Ref.TurretOffset;
                }
            }
            return GetFLHAbsoluteCoords(pTechno, flh, isOnTurret, flipY, turretOffset, nextFrame);
        }

        public static unsafe CoordStruct GetFLHAbsoluteCoords(Pointer<TechnoClass> pTechno, CoordStruct flh, bool isOnTurret, int flipY, CoordStruct turretOffset, bool nextFrame)
        {
            if (pTechno.Ref.Base.Base.WhatAmI() == AbstractType.Building)
            {
                // 建筑不能使用矩阵方法测算FLH
                return GetFLHAbsoluteCoords(pTechno.Ref.Base.Base.GetCoords(), flh, pTechno.Ref.Facing.current(), turretOffset);
            }
            else
            {
                SingleVector3D res = pTechno.Ref.Base.Base.GetCoords().ToVector3D();
                // get turretOffset location offset
                CoordStruct sourceOffset = turretOffset;
                if (nextFrame)
                {
                    // get nextframe location offset
                    Pointer<FootClass> pFoot = pTechno.Convert<FootClass>();
                    int speed = 0;
                    if (pFoot.Ref.Locomotor.Is_Moving() && (speed = pFoot.Ref.GetCurrentSpeed()) > 0)
                    {
                        sourceOffset += new CoordStruct(speed, 0, 0);
                    }
                }
                if (null != flh && default != flh)
                {
                    // Step 1: get body transform matrix
                    Matrix3DStruct matrix3D = GetMatrix3D(pTechno);
                    // Step 2: move to turrretOffset
                    matrix3D.Translate(turretOffset.X, turretOffset.Y, turretOffset.Z);
                    // Step 3: rotation
                    RotateMatrix3D(ref matrix3D, pTechno, isOnTurret, nextFrame);
                    // Step 4: apply FLH offset
                    CoordStruct tempFLH = flh;
                    if (pTechno.Convert<AbstractClass>().Ref.WhatAmI() == AbstractType.Building)
                    {
                        tempFLH.Z += Game.LevelHeight;
                    }
                    tempFLH.Y *= flipY;
                    SingleVector3D offset = GetFLHOffset(ref matrix3D, tempFLH);
                    // Step 5: offset techno location
                    res += offset;
                }
                return res.ToCoordStruct();
            }
        }

        public static unsafe Matrix3DStruct GetMatrix3D(Pointer<TechnoClass> pTechno)
        {
            // Step 1: get body transform matrix
            Matrix3DStruct matrix3D = new Matrix3DStruct(true);
            ILocomotion loco = pTechno.Convert<FootClass>().Ref.Locomotor;
            if (null != loco)
            {
                loco.Draw_Matrix(Pointer<Matrix3DStruct>.AsPointer(ref matrix3D), IntPtr.Zero);
            }
            return matrix3D;
        }

        public static unsafe Matrix3DStruct RotateMatrix3D(ref Matrix3DStruct matrix3D, Pointer<TechnoClass> pTechno, bool isOnTurret, bool nextFrame)
        {
            // Step 2: rotation
            if (isOnTurret)
            {
                // 旋转到炮塔相同角度
                if (pTechno.Ref.HasTurret())
                {
                    DirStruct turretDir = nextFrame ? pTechno.Ref.TurretFacing.next() : pTechno.Ref.TurretFacing.current();
                    /*
                    double turretRad = (pTechno.Ref.GetTurretFacing().current(false).value32() - 8) * -(Math.PI / 16);
                    double bodyRad = (pTechno.Ref.GetRealFacing().current(false).value32() - 8) * -(Math.PI / 16);
                    float angle = (float)(turretRad - bodyRad);
                    matrix3D.RotateZ(angle);
                    */
                    if (pTechno.Convert<AbstractClass>().Ref.WhatAmI() == AbstractType.Building)
                    {
                        double turretRad = turretDir.radians();
                        matrix3D.RotateZ((float)turretRad);
                    }
                    else
                    {
                        // 旋转到0点，再转到炮塔的角度
                        matrix3D.RotateZ(-matrix3D.GetZRotation());
                        matrix3D.RotateZ((float)turretDir.radians());
                    }
                }
            }
            else if (nextFrame)
            {
                // 旋转到0点，再转到身体下一帧的角度
                matrix3D.RotateZ(-matrix3D.GetZRotation());
                matrix3D.RotateZ((float)pTechno.Ref.Facing.next().radians());
            }
            return matrix3D;
        }

        public static unsafe SingleVector3D GetFLHOffset(ref Matrix3DStruct matrix3D, CoordStruct flh)
        {
            // Step 4: apply FLH offset
            matrix3D.Translate(flh.X, flh.Y, flh.Z);
            SingleVector3D result = Game.MatrixMultiply(matrix3D);
            // Resulting FLH is mirrored along X axis, so we mirror it back - Kerbiter
            result.Y *= -1;
            return result;
        }

        public static unsafe BulletVelocity GetBulletVelocity(CoordStruct sourcePos, CoordStruct targetPos)
        {
            CoordStruct bulletFLH = new CoordStruct(1, 0, 0);
            DirStruct bulletDir = ExHelper.Point2Dir(sourcePos, targetPos);
            SingleVector3D bulletV = ExHelper.GetFLHAbsoluteOffset(bulletFLH, bulletDir, default);
            return bulletV.ToBulletVelocity();
        }

        public static bool ReadBulletVelocity(INIReader reader, string section, string key, ref BulletVelocity velocity)
        {
            SingleVector3D vector3D = default;
            if (ReadSingleVector3D(reader, section, key, ref vector3D))
            {
                velocity.X = vector3D.X;
                velocity.Y = vector3D.Y;
                velocity.Z = vector3D.Z;
                return true;
            }
            return false;
        }

        public static bool ReadCoordStruct(INIReader reader, string section, string key, ref CoordStruct flh)
        {
            SingleVector3D vector3D = default;
            if (ReadSingleVector3D(reader, section, key, ref vector3D))
            {
                flh.X = (int)vector3D.X;
                flh.Y = (int)vector3D.Y;
                flh.Z = (int)vector3D.Z;
                return true;
            }
            return false;
        }

        public static bool ReadColorStruct(INIReader reader, string section, string key, ref ColorStruct color)
        {
            SingleVector3D vector3D = default;
            if (ReadSingleVector3D(reader, section, key, ref vector3D))
            {
                color.R = Convert.ToByte(vector3D.X);
                color.G = Convert.ToByte(vector3D.Y);
                color.B = Convert.ToByte(vector3D.Z);
                return true;
            }
            return false;
        }

        public static bool ReadSingleVector3D(INIReader reader, string section, string key, ref SingleVector3D xyz)
        {
            string sXYZ = default;
            if (reader.ReadNormal(section, key, ref sXYZ))
            {
                string[] pos = sXYZ.Split(',');
                if (null != pos && pos.Length > 0)
                {
                    for (int i = 0; i < pos.Length; i++)
                    {
                        float value = Convert.ToSingle(pos[i].Trim());
                        switch (i)
                        {
                            case 0:
                                xyz.X = value;
                                break;
                            case 1:
                                xyz.Y = value;
                                break;
                            case 2:
                                xyz.Z = value;
                                break;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public static bool ReadList(INIReader reader, string section, string key, ref List<string> list)
        {
            string text = default;
            if (reader.ReadNormal(section, key, ref text))
            {
                if (null == list)
                {
                    list = new List<string>();
                }
                string[] texts = text.Split(',');
                foreach (string t in texts)
                {
                    string tmp = t.Trim();
                    if (!String.IsNullOrEmpty(tmp))
                    {
                        list.Add(tmp);
                    }
                }
                return true;
            }
            return false;
        }

        public static bool ReadIntList(INIReader reader, string section, string key, ref List<int> list)
        {
            List<string> values = default;
            if (ReadList(reader, section, key, ref values))
            {
                if (null == list)
                {
                    list = new List<int>();
                }
                foreach (string v in values)
                {
                    list.Add(Convert.ToInt32(v));
                }
                return true;
            }
            return false;
        }

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

        public static unsafe void FireWeaponTo(Pointer<TechnoClass> pShooter, Pointer<TechnoClass> pAttacker, Pointer<AbstractClass> pTarget, Pointer<WeaponTypeClass> pWeapon, CoordStruct flh, FireBulletToTarget callback = null, CoordStruct bulletSourcePos = default, bool radialFire = false, int splitAngle = 180)
        {
            if (pTarget.IsNull)
            {
                return;
            }
            CoordStruct targetPos = pTarget.Ref.GetCoords();
            // radial fire
            int burst = pWeapon.Ref.Burst;
            RadialFireHelper radialFireHelper = new RadialFireHelper(pShooter, burst, splitAngle);
            int flipY = -1;
            for (int i = 0; i < burst; i++)
            {
                BulletVelocity bulletVelocity = default;
                if (radialFire)
                {
                    flipY = (i < burst / 2f) ? -1 : 1;
                    bulletVelocity = radialFireHelper.GetBulletVelocity(i);
                }
                else
                {
                    flipY *= -1;
                }
                CoordStruct sourcePos = bulletSourcePos;
                if (default == bulletSourcePos)
                {
                    // get flh
                    sourcePos = GetFLHAbsoluteCoords(pShooter, flh, true, flipY, default);
                }
                if (default == bulletVelocity)
                {
                    bulletVelocity = GetBulletVelocity(sourcePos, targetPos);
                }
                Pointer<BulletClass> pBullet = FireBulletTo(pAttacker, pTarget, pWeapon, sourcePos, targetPos, bulletVelocity);

                // Logger.Log("发射{0}，抛射体{1}，回调{2}", pWeapon.Ref.Base.ID, pBullet.IsNull ? " is null" : pBullet.Ref.Type.Ref.Base.Base.ID, null == callback);
                if (null != callback && !pBullet.IsNull)
                {
                    callback(i, burst, pBullet, pTarget);
                }
            }
        }

        public static unsafe Pointer<BulletClass> FireBulletTo(Pointer<TechnoClass> pAttacker, Pointer<AbstractClass> pTarget, Pointer<WeaponTypeClass> pWeapon, CoordStruct sourcePos, CoordStruct targetPos, BulletVelocity bulletVelocity)
        {
            if (pTarget.IsNull || (pTarget.CastToTechno(out Pointer<TechnoClass> pTechno) && !pTechno.Ref.Base.IsAlive))
            {
                return IntPtr.Zero;
            }
            // Fire weapon
            Pointer<BulletClass> pBullet = FireBullet(pAttacker, pTarget, pWeapon, sourcePos, targetPos, bulletVelocity);
            // Draw bullet effect
            DrawBulletEffect(pWeapon, sourcePos, targetPos, pAttacker, pTarget);
            // Draw particle system
            AttachedParticleSystem(pWeapon, sourcePos, pTarget, pAttacker, targetPos);
            // Play report sound
            PlayReportSound(pWeapon, sourcePos);
            // Draw weapon anim
            DrawWeaponAnim(pWeapon, sourcePos, targetPos);
            return pBullet;
        }

        public static unsafe Pointer<BulletClass> FireBullet(Pointer<TechnoClass> pAttacker, Pointer<AbstractClass> pTarget, Pointer<WeaponTypeClass> pWeapon, CoordStruct sourcePos, CoordStruct targetPos, BulletVelocity bulletVelocity)
        {
            double fireMult = 1;
            if (!pAttacker.IsNull && pAttacker.Ref.Base.IsAlive)
            {
                // check spawner
                Pointer<SpawnManagerClass> pSpawn = pAttacker.Ref.SpawnManager;
                if (pWeapon.Ref.Spawner && !pSpawn.IsNull)
                {
                    pSpawn.Ref.SetTarget(pTarget);
                    return Pointer<BulletClass>.Zero;
                }

                // check Abilities FIREPOWER
                fireMult = GetDamageMult(pAttacker);
            }
            int damage = (int)(pWeapon.Ref.Damage * fireMult);
            Pointer<WarheadTypeClass> pWH = pWeapon.Ref.Warhead;
            int speed = pWeapon.Ref.Speed;
            bool bright = pWeapon.Ref.Bright || pWH.Ref.Bright;

            Pointer<BulletClass> pBullet = IntPtr.Zero;
            // 自己不能发射武器朝向自己
            // Pointer<TechnoClass> pRealAttacker = pTarget.Value != pAttacker.Value ? pAttacker : IntPtr.Zero;
            pBullet = pWeapon.Ref.Projectile.Ref.CreateBullet(pTarget, pAttacker, damage, pWH, speed, bright);
            pBullet.Ref.WeaponType = pWeapon;
            // Logger.Log("{0}发射武器{1}，创建抛射体，目标类型{2}", pAttacker, pWeapon.Ref.Base.ID, pTarget.Ref.WhatAmI());

            // pBullet.Ref.SetTarget(pTarget);
            pBullet.Ref.MoveTo(sourcePos, bulletVelocity);
            if (pWeapon.Ref.Projectile.Ref.Inviso && !pWeapon.Ref.Projectile.Ref.Airburst)
            {
                pBullet.Ref.Detonate(targetPos);
                pBullet.Ref.Base.UnInit();
            }
            return pBullet;
        }

        public static unsafe void DrawBulletEffect(Pointer<WeaponTypeClass> pWeapon, CoordStruct sourcePos, CoordStruct targetPos, Pointer<TechnoClass> pAttacker, Pointer<AbstractClass> pTarget)
        {
            // IsLaser
            if (pWeapon.Ref.IsLaser)
            {
                LaserType laserType = new LaserType(false);
                ColorStruct houseColor = default;
                if (pWeapon.Ref.IsHouseColor && !pAttacker.IsNull)
                {
                    houseColor = pAttacker.Ref.Owner.Ref.LaserColor;
                }
                laserType.InnerColor = pWeapon.Ref.LaserInnerColor;
                laserType.OuterColor = pWeapon.Ref.LaserOuterColor;
                laserType.OuterSpread = pWeapon.Ref.LaserOuterSpread;
                laserType.IsHouseColor = pWeapon.Ref.IsHouseColor;
                laserType.Duration = pWeapon.Ref.LaserDuration;
                // get thickness and fade
                WeaponTypeExt ext = WeaponTypeExt.ExtMap.Find(pWeapon);
                if (null != ext)
                {
                    if (ext.LaserThickness > 0)
                    {
                        laserType.Thickness = ext.LaserThickness;
                    }
                    laserType.Fade = ext.LaserFade;
                    laserType.IsSupported = ext.IsSupported;
                }
                BulletEffectHelper.DrawLine(sourcePos, targetPos, laserType, houseColor);
            }
            // IsRadBeam
            if (pWeapon.Ref.IsRadBeam)
            {
                RadBeamType radBeamType = RadBeamType.RadBeam;
                if (!pWeapon.Ref.Warhead.IsNull && pWeapon.Ref.Warhead.Ref.Temporal)
                {
                    radBeamType = RadBeamType.Temporal;
                }
                BeamType beamType = new BeamType(radBeamType);
                BulletEffectHelper.DrawBeam(sourcePos, targetPos, beamType);
                // RadBeamType beamType = RadBeamType.RadBeam;
                // ColorStruct beamColor = RulesClass.Global().RadColor;
                // if (!pWeapon.Ref.Warhead.IsNull && pWeapon.Ref.Warhead.Ref.Temporal)
                // {
                //     beamType = RadBeamType.Temporal;
                //     beamColor = RulesClass.Global().ChronoBeamColor;
                // }
                // Pointer<RadBeam> pRadBeam = RadBeam.Allocate(beamType);
                // if (!pRadBeam.IsNull)
                // {
                //     pRadBeam.Ref.SetCoordsSource(sourcePos);
                //     pRadBeam.Ref.SetCoordsTarget(targetPos);
                //     pRadBeam.Ref.Color = beamColor;
                //     pRadBeam.Ref.Period = 15;
                //     pRadBeam.Ref.Amplitude = 40.0;
                // }
            }
            //IsElectricBolt
            if (pWeapon.Ref.IsElectricBolt)
            {
                if (!pAttacker.IsNull && !pTarget.IsNull)
                {
                    BulletEffectHelper.DrawBolt(pAttacker, pTarget, pWeapon, sourcePos);
                }
                else
                {
                    BulletEffectHelper.DrawBolt(sourcePos, targetPos, pWeapon.Ref.IsAlternateColor);
                }
            }
        }

        public static unsafe void AttachedParticleSystem(Pointer<WeaponTypeClass> pWeapon, CoordStruct sourcePos, Pointer<AbstractClass> pTarget, Pointer<TechnoClass> pAttacker, CoordStruct targetPos)
        {
            //ParticleSystem
            Pointer<ParticleSystemTypeClass> psType = pWeapon.Ref.AttachedParticleSystem;
            if (!psType.IsNull)
            {
                BulletEffectHelper.DrawParticele(psType, sourcePos, pTarget, pAttacker, targetPos);
            }
        }

        public static unsafe void PlayReportSound(Pointer<WeaponTypeClass> pWeapon, CoordStruct sourcePos)
        {
            if (pWeapon.Ref.Report.Count > 0)
            {
                int index = Random.Next(0, pWeapon.Ref.Report.Count - 1);
                int soundIndex = pWeapon.Ref.Report.Get(index);
                if (soundIndex != -1)
                {
                    VocClass.PlayAt(soundIndex, sourcePos, IntPtr.Zero);
                }
            }
        }

        public static unsafe void DrawWeaponAnim(Pointer<WeaponTypeClass> pWeapon, CoordStruct sourcePos, CoordStruct targetPos)
        {
            // Anim
            if (pWeapon.Ref.Anim.Count > 0)
            {
                int facing = pWeapon.Ref.Anim.Count;
                int index = 0;
                if (facing % 8 == 0)
                {
                    index = Dir2FacingIndex(Point2Dir(sourcePos, targetPos), facing);
                    index = (int)(facing / 8) + index;
                    if (index >= facing)
                    {
                        index = 0;
                    }
                }
                Pointer<AnimTypeClass> pAnimType = pWeapon.Ref.Anim.Get(index);
                // Logger.Log("获取到动画{0}", pAnimType.IsNull ? "不存在" : pAnimType.Convert<AbstractTypeClass>().Ref.ID);
                if (!pAnimType.IsNull)
                {
                    Pointer<AnimClass> pAnim = YRMemory.Create<AnimClass>(pAnimType, sourcePos);
                }
            }
        }

        public static double GetROFMult(Pointer<TechnoClass> pTechno)
        {
            bool rofAbility = false;
            if (pTechno.Ref.Veterancy.IsElite())
            {
                rofAbility = pTechno.Ref.Type.Ref.VeteranAbilities.ROF || pTechno.Ref.Type.Ref.EliteAbilities.ROF;
            }
            else if (pTechno.Ref.Veterancy.IsVeteran())
            {
                rofAbility = pTechno.Ref.Type.Ref.VeteranAbilities.ROF;
            }
            return !rofAbility ? 1.0 : RulesClass.Global().VeteranROF * ((pTechno.Ref.Owner.IsNull || pTechno.Ref.Owner.Ref.Type.IsNull) ? 1.0 : pTechno.Ref.Owner.Ref.Type.Ref.ROFMult);
        }

        public static double GetDamageMult(Pointer<TechnoClass> pTechno)
        {
            if (pTechno.IsNull || !pTechno.Ref.Base.IsAlive)
            {
                return 1;
            }
            bool firepower = false;
            if (pTechno.Ref.Veterancy.IsElite())
            {
                firepower = pTechno.Ref.Type.Ref.VeteranAbilities.FIREPOWER || pTechno.Ref.Type.Ref.EliteAbilities.FIREPOWER;
            }
            else if (pTechno.Ref.Veterancy.IsVeteran())
            {
                firepower = pTechno.Ref.Type.Ref.VeteranAbilities.FIREPOWER;
            }
            return (!firepower ? 1.0 : RulesClass.Global().VeteranCombat) * pTechno.Ref.FirepowerMultiplier * ((pTechno.Ref.Owner.IsNull || pTechno.Ref.Owner.Ref.Type.IsNull) ? 1.0 : pTechno.Ref.Owner.Ref.Type.Ref.FirepowerMult);
        }

        public static int CountAircraft(Pointer<HouseClass> pHouse, List<String> padList)
        {
            int count = 0;
            FindAircraft(pHouse, (pTarget) =>
            {
                if (padList.Contains(pTarget.Ref.Type.Ref.Base.Base.Base.ID)
                    && pTarget.Ref.Type.Ref.AirportBound)
                {
                    count++;
                }
                return false;
            });
            return count;
        }

        public static CoordStruct OneCellOffsetToTarget(CoordStruct sourcePos, CoordStruct targetPos)
        {
            double angle = Math.Atan2((targetPos.Y - sourcePos.Y), (targetPos.X - sourcePos.X));
            int y = (int)(256 * Math.Tan(angle));
            int x = (int)(256 / Math.Tan(angle));
            CoordStruct offset = new CoordStruct();
            if (y == 0)
            {
                offset.Y = 0;
                if (angle < Math.PI)
                {
                    offset.X = 256;
                }
                else
                {
                    offset.X = -256;
                }
            }
            else if (x == 0)
            {
                offset.X = 0;
                if (angle < 0)
                {
                    offset.Y = -256;
                }
                else
                {
                    offset.Y = 256;
                }
            }
            else
            {
                if (Math.Abs(x) <= 256)
                {
                    offset.X = x;
                    if (angle > 0)
                    {
                        offset.Y = 256;
                    }
                    else
                    {
                        offset.X = -offset.X;
                        offset.Y = -256;
                    }
                }
                else
                {
                    offset.Y = y;
                    if (Math.Abs(angle) < 0.5 * Math.PI)
                    {
                        offset.X = 256;
                    }
                    else
                    {
                        offset.X = -256;
                        offset.Y = -offset.Y;
                    }
                }
            }
            return offset;
        }

        public static ColorStruct Color2ColorAdd(ColorStruct color)
        {
            int B = color.B >> 3;
            int G = color.G >> 2;
            int R = color.R >> 3;
            return new ColorStruct(R, G, B);
        }

        public static uint ColorAdd2RGB565(ColorStruct colorAdd)
        {
            string R2 = Convert.ToString(colorAdd.R, 2).PadLeft(5, '0');
            string G2 = Convert.ToString(colorAdd.G, 2).PadLeft(6, '0');
            string B2 = Convert.ToString(colorAdd.B, 2).PadLeft(5, '0');
            string c2 = R2 + G2 + B2;
            return Convert.ToUInt32(c2, 2);
        }

        public static DirStruct DirNormalized(int index, int facing)
        {
            double radians = EXMath.Deg2Rad((-360 / facing * index));
            DirStruct dir = new DirStruct();
            dir.SetValue((short)(radians / EXMath.BINARY_ANGLE_MAGIC));
            return dir;
        }

        public static int Dir2FacingIndex(DirStruct dir, int facing)
        {
            uint bits = (uint)Math.Round(Math.Sqrt(facing), MidpointRounding.AwayFromZero);
            double face = dir.GetValue(bits);
            double x = (face / (1 << (int)bits)) * facing;
            int index = (int)Math.Round(x, MidpointRounding.AwayFromZero);
            return index;
        }

        public static DirStruct Point2Dir(CoordStruct sourcePos, CoordStruct targetPos)
        {
            // get angle
            double radians = Math.Atan2(sourcePos.Y - targetPos.Y, targetPos.X - sourcePos.X);
            // Magic form tomsons26
            radians -= EXMath.Deg2Rad(90);
            return Radians2Dir(radians);
        }

        public static DirStruct Radians2Dir(double radians)
        {
            short d = (short)(radians / EXMath.BINARY_ANGLE_MAGIC);
            return new DirStruct(d);
        }
    }
}
