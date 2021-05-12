
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using PatcherYRpp.FileFormats;
using Extension.Ext;
using Extension.Script;
using Extension.Utilities;
using System.Threading.Tasks;

namespace Scripts
{
    [Serializable]
    public class MTNK : TechnoScriptable
    {
        public MTNK(TechnoExt owner) : base(owner) { }

        static MTNK()
        {
            // Task.Run(() =>
            // {
            //     while (true)
            //     {
            //         Logger.Log("Ticked.");
            //         Thread.Sleep(1000);
            //     }
            // });
        }

        Random random = new Random();
        static ColorStruct innerColor = new ColorStruct(208, 10, 203);
        static ColorStruct outerColor = new ColorStruct(88, 0, 88);
        static ColorStruct outerSpread = new ColorStruct(10, 10, 10);

        public int duration = 30;
        public int thickness = 10;
        public int distance = 100;

        CoordStruct lastLocation;

        CoordStruct lastFLH;

        List<Pointer<ObjectClass>> targetList = new List<Pointer<ObjectClass>>();

        Dictionary<Pointer<AbstractClass>, int> targetHealthDic = new Dictionary<Pointer<AbstractClass>, int>();

        bool turnFlag = false;

        Pointer<TechnoClass> stand;

        int flameIdx = 0;
        int rate = 0;

        public override void OnUpdate()
        {

            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            TechnoTypeExt extType = ext.Type;
            string ID = pTechno.Ref.Type.Ref.Base.Base.UIName;
            string HouseID = pTechno.Ref.Owner.Ref.Type.Ref.Base.UIName;

            CoordStruct location = pTechno.Ref.Base.Base.GetCoords();

            Point2D pos = new Point2D(0, 30);
            pos += TacticalClass.Global().CoordsToClient(location);
            RectangleStruct bound = new RectangleStruct(0, 0, 1112, 688);

            Pointer<AnimTypeClass> pAnim = AnimTypeClass.ABSTRACTTYPE_ARRAY.Find("MININUKE");
            if (!pAnim.IsNull)
            {
                Pointer<SHPStruct> pSHP = FileSystem.PIPS_SHP;
                if (flameIdx > pSHP.Ref.Frames)
                {
                    flameIdx = 0;
                }
                if (++rate > 30)
                {
                    rate = 0;

                    DSurface.Primary.Ref.DrawSHP(FileSystem.MOUSE_PAL, pSHP, flameIdx, pos, bound, BlitterFlags.Alpha, 1000, 0);
                    flameIdx++;
                }
            }

            if (default == lastLocation)
            {
                lastLocation = location;
            }
            else
            {
                location.Z += 30;
                if (lastLocation.DistanceFrom(location) > distance)
                {
                    Pointer<LaserDrawClass> pLaser = YRMemory.Create<LaserDrawClass>(lastLocation, location, innerColor, outerColor, outerSpread, duration);
                    pLaser.Ref.Thickness = thickness;
                    pLaser.Ref.IsHouseColor = true;

                    // Pointer<CellClass> pCell = MapClass.Instance.GetCellAt(lastLocation);
                    // Pointer<BulletClass> pBullet = pTechno.Ref.Fire(pCell.Convert<ObjectClass>(), 0);

                    lastLocation = location;
                }
            }




            if (pTechno.Ref.Base.IsSelected)
            {
                // Logger.Log("Curret Mission {0}", pTechno.Convert<ObjectClass>().Ref.GetCurrentMission());
                /*
                turnFlag = true;
                // Turn
                ref FacingStruct rFacing = ref pTechno.Ref.Facing;
                // Logger.Log("CurrentFrame = {0}, Timer.StartTime = {1}, Timer.TimeLeft = {2}, Timer.GetTimeLeft = {3}",
                //     Helpers.CurrentFrame, facing.Timer.StartTime, facing.Timer.TimeLeft, facing.Timer.GetTimeLeft());
                if (!rFacing.in_motion())
                {
                    if (!turnFlag)
                    {
                        // turnFlag = true;
                        FacingStruct realFacing = pTechno.Ref.GetRealFacing();
                        // Logger.Log("RealFacing dir={0}", realFacing.target().Value);
                        short dir = realFacing.target().value();
                        // if (dir != 24576 && dir != -24576)
                        //     dir = -24576;
                        // else
                        dir -= 24576;
                        DirStruct tg = new DirStruct(dir);
                        bool flag = rFacing.turn(tg);
                        Logger.Log("Facing = {0}, Turn to {1} {2}. TurnRate = {3} TimeLeft = {4}, Radians = {5}", realFacing.target().value(), tg.value(), flag, rFacing.turn_rate(), rFacing.Timer.GetTimeLeft(), realFacing.target().radians());
                    }
                }
                else
                {
                    // Logger.Log("In motion, Facing={0}, TimeLeft={1}", pTechno.Ref.Facing.Value.Value, pTechno.Ref.Facing.Timer.GetTimeLeft());
                }
                // ref FacingStruct rTurretFacing = ref pTechno.Ref.TurretFacing;
                FacingStruct turretFacing = pTechno.Ref.GetTurretFacing();
                // if(Math.Abs(turretFacing.target().value()) >= 24576)
                // {
                //     short dir = 24576;
                //     if ((int)turretFacing.target().value() < 0)
                //         dir = (short)-dir;
                //     DirStruct tg = new DirStruct(dir);
                //     rTurretFacing.set(ref tg);
                // }
                CoordStruct tFLH = new CoordStruct(1230, 0, 120);
                CoordStruct targetPos = ExHelper.GetFLH(location, tFLH, turretFacing.target());
                CoordStruct sourcePos = location;
                sourcePos.Z += 120;
                // LaserTail.DrawLine(sourcePos, targetPos, 2, 5);
                */
            }
            else
            {
                lastFLH = default;
                turnFlag = false;
            }

            // Stand
            if (!stand.IsNull)
            {
                stand.Ref.Base.SetLocation(location);
                stand.Ref.Base.NeedsRedraw = 1;
            }
        }

        public override void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            Pointer<HouseClass> pHouse = pTechno.Ref.Owner;

            string ID = pTechno.Ref.Base.Type.Ref.Base.UIName;
            string HouseID = pHouse.Ref.Type.Ref.Base.UIName;

            TechnoTypeExt extType = Owner.Type;

            AbstractType targetType = pTarget.Ref.WhatAmI();

            // Fire super weapon
            // Pointer<SuperWeaponTypeClass> pSWType = extType.FireSuperWeapon;
            // if (pSWType.IsNull == false) {
            //     Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            //     Pointer<HouseClass> pOwner = pTechno.Ref.Owner;
            //     Pointer<SuperClass> pSuper = pOwner.Ref.FindSuperWeapon(pSWType);

            //     CellStruct targetCell = MapClass.Coord2Cell(pTarget.Ref.GetCoords());
            //     //Logger.Log("FireSuperWeapon({2}):0x({3:X}) -> ({0}, {1})", targetCell.X, targetCell.Y, pSWType.Ref.Base.GetID(), (int)pSuper);
            //     pSuper.Ref.IsCharged = 1;
            //     pSuper.Ref.Launch(targetCell, true);
            //     pSuper.Ref.IsCharged = 0;
            // }

            if (pTarget.Ref.WhatAmI() == AbstractType.Unit)
            {

            }

            if (targetType == AbstractType.Cell)
            {

                // Weapon Size
                // pWeapon.Ref.TurretLocked = false;
                // int index = 0;
                // do
                // {
                //     Pointer<WeaponStruct> pW;
                //     try
                //     {
                //         pW = pTechno.Ref.GetWeapon(index);
                //     }
                //     catch (Exception e)
                //     {
                //         Logger.Log("Has Exception, break. Index={0}", index);
                //         break;
                //     }
                //     if (pW.IsNull)
                //     {
                //         Logger.Log("Weapon {0} is null, pW.IsNull={1}", index, pW.IsNull);
                //         break;
                //     }
                //     Logger.Log("Weapon {0} get. pW.Ref.WeaponType.IsNull={1}", index, pW.Ref.WeaponType.IsNull);
                // } while (index++ < 40);

                // // Create New TechnoType
                // Pointer<TechnoTypeClass> pNewType = TechnoTypeClass.Find("NAPOWR");
                // if (!pNewType.IsNull && stand.IsNull)
                // {
                //     // pTechno.Ref.Base.Remove();
                //     // pTechno.Ref.Base.UnInit();
                //     // create unit put to same location.
                //     CoordStruct ownerLocation = pTechno.Ref.Base.Base.GetCoords();
                //     CoordStruct targetLocation = pTarget.Ref.GetCoords();
                //     Logger.Log("Create new Unit. {0}-{1}", pHouse.Ref.Type.Ref.Base.GetUIName(), pNewType.Ref.Base.Base.GetUIName());
                //     Pointer<ObjectClass> pNewObject = pNewType.Ref.Base.CreateObject(pHouse);
                //     pNewObject.CastToTechno(out stand);
                //     if (pNewObject.Ref.Base.WhatAmI() == AbstractType.Building)
                //     {
                //         pNewObject.Ref.Put(targetLocation, Direction.N);
                //         pNewObject.Ref.SetLocation(ownerLocation);
                //     }
                //     else
                //     {
                //         CoordStruct pos = new CoordStruct(0, 0, 1000);
                //         pNewObject.Ref.Put(pos, Direction.N);
                //         pNewObject.Ref.SetLocation(ownerLocation);
                //         Pointer<FootClass> pFoot = pNewObject.Convert<FootClass>();
                //         pFoot.Ref.MoveTo(ownerLocation);
                //     }
                //     pNewObject.Ref.NeedsRedraw = 1;
                // }

                // CellStruct targetCell = MapClass.Coord2Cell(targetLocation);
                // bool flag = pTechno.Ref.Type.Ref.Base.SpawnAtMapCoords(Pointer<CellStruct>.AsPointer(ref targetCell), pHouse);
                // Logger.Log("SpawnAtMapCoords Flag = {0}", flag);

                // pTechno.Ref.Base.Put(Pointer<CoordStruct>.AsPointer(ref targetLocation), Direction.E);
                // foreach(Pointer<ObjectClass> pObj in targetList)
                // {
                //     targetList.Remove(pObj);
                //     Logger.Log("Ready To Move.");
                //     Pointer<FootClass> pFoot = pObj.Convert<FootClass>();
                //     bool flag = pFoot.Ref.MoveTo(ref targetLocation);
                //     Logger.Log("MoveTo {0}", flag);
                //     break;
                // }

                // if (targetList.Count <= 0)
                // {
                //     Pointer<WeaponStruct> pPrimary = pTechno.Ref.GetWeapon(0);
                //     pPrimary.Ref.WeaponType.Ref.Range = 256 * 2;
                //     pPrimary.Ref.WeaponType.Ref.FireOnce = Convert.ToByte(true);
                //     Pointer<WeaponStruct> pSecondary = pTechno.Ref.GetWeapon(1);
                //     pSecondary.Ref.WeaponType.Ref.Range = 256 * 2;
                //     pSecondary.Ref.WeaponType.Ref.FireOnce = Convert.ToByte(true);
                // }

            }

        }
    }
}