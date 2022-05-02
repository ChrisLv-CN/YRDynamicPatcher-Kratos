using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    [Serializable]
    public class TrailData
    {

        public bool Enable;

        public string TrailType;

        public CoordStruct FLH;

        public bool IsOnTurret;

        public List<LandType> OnLands;
        public List<TileType> OnTiles;

        public TrailData(string trailType)
        {
            this.Enable = !string.IsNullOrEmpty(trailType);
            this.TrailType = trailType;
            this.FLH = default;
            this.IsOnTurret = false;
        }

    }


    [Serializable]
    public class TrailManager
    {
        public List<Trail> Trails = new List<Trail>();
        public SwizzleablePointer<HouseClass> pHouse = new SwizzleablePointer<HouseClass>(IntPtr.Zero);

        public void SetTrailData(List<TrailData> trailDatas)
        {
            foreach (TrailData trailData in trailDatas)
            {
                string typeName = trailData.TrailType;
                TrailType trailType = TrailType.FindOrAllocate(typeName, CCINIClass.INI_Art) as TrailType;
                // Logger.Log("获得尾巴类型{0}的实例{1}，创建尾巴实体对象", typeName, trailType != null);
                Trail trail = trailType.CreateObject();
                trail.FLH = trailData.FLH;
                trail.IsOnTurret = trailData.IsOnTurret;
                trail.OnLandTypes = trailData.OnLands;
                trail.OnTileTypes = trailData.OnTiles;
                Trails.Add(trail);
            }
        }

        public void Put(CoordStruct location, DirStruct faceDir)
        {
            foreach (Trail trail in Trails)
            {
                CoordStruct sourcePos = ExHelper.GetFLHAbsoluteCoords(location, trail.FLH, faceDir);
                trail.SetLastLocation(sourcePos);
            }
        }

        public void Put(CoordStruct location, short faceDirValue8)
        {
            DirStruct facing = new DirStruct();
            facing.value8(faceDirValue8);
            Put(location, facing);
        }

        public void Put(Pointer<BulletClass> pBullet, CoordStruct location)
        {
            CoordStruct forwardLocation = location + pBullet.Ref.Velocity.ToCoordStruct();
            DirStruct bulletFacing = ExHelper.Point2Dir(location, forwardLocation);
            Put(location, bulletFacing);
        }

        public void Update(Pointer<TechnoClass> pTechno, DrivingState drivingState = DrivingState.Moving)
        {
            foreach (Trail trail in Trails)
            {
                // 检查动画尾巴
                if (trail.Type.Mode == TrailMode.ANIM)
                {
                    switch (drivingState)
                    {
                        case DrivingState.Start:
                        case DrivingState.Stop:
                            trail.SetDrivingState(drivingState);
                            break;
                    }
                }
                CoordStruct sourcePos = ExHelper.GetFLHAbsoluteCoords(pTechno, trail.FLH, trail.IsOnTurret);
                trail.UpdateLastLocation(sourcePos);
            }
        }

        public void Update(Pointer<BulletClass> pBullet)
        {
            CoordStruct location = pBullet.Ref.Base.Base.GetCoords();
            CoordStruct forwardLocation = location + pBullet.Ref.Velocity.ToCoordStruct();
            DirStruct bulletFacing = ExHelper.Point2Dir(location, forwardLocation);
            foreach (Trail trail in Trails)
            {
                CoordStruct sourcePos = ExHelper.GetFLHAbsoluteCoords(location, trail.FLH, bulletFacing);
                trail.UpdateLastLocation(sourcePos);
            }
        }

        public void Remove()
        {
            foreach (Trail trail in Trails)
            {
                trail.ClearLastLocation();
            }
        }

        public void DrawTrail(Pointer<TechnoClass> pTechno, DrivingState drivingState = DrivingState.Moving)
        {
            foreach (Trail trail in Trails)
            {
                // 检查动画尾巴
                if (trail.Type.Mode == TrailMode.ANIM)
                {
                    switch (drivingState)
                    {
                        case DrivingState.Start:
                        case DrivingState.Stop:
                            trail.SetDrivingState(drivingState);
                            break;
                    }
                }
                CoordStruct sourcePos = ExHelper.GetFLHAbsoluteCoords(pTechno, trail.FLH, trail.IsOnTurret);
                trail.DrawTrail(pTechno.Ref.Owner, sourcePos);
            }
        }

        public void DrawTrail(Pointer<BulletClass> pBullet)
        {
            CoordStruct location = pBullet.Ref.Base.Base.GetCoords();
            CoordStruct forwardLocation = location + pBullet.Ref.Velocity.ToCoordStruct();
            DirStruct bulletFacing = ExHelper.Point2Dir(location, forwardLocation);
            foreach (Trail trail in Trails)
            {
                CoordStruct sourcePos = ExHelper.GetFLHAbsoluteCoords(location, trail.FLH, bulletFacing);
                trail.DrawTrail(pHouse, sourcePos);
            }
        }

        public static bool ReadTrailData(INIReader reader, string section, out List<TrailData> trailDatas)
        {
            trailDatas = null;
            for (int i = 0; i < 12; i++)
            {
                TrailData data = null;
                string type = null;
                if (reader.ReadNormal(section, "Trail" + i + ".Type", ref type))
                {
                    data = new TrailData(type);

                    CoordStruct flh = default;
                    if (ExHelper.ReadCoordStruct(reader, section, "Trail" + i + ".FLH", ref flh))
                    {
                        data.FLH = flh;
                    }

                    bool isOnTurret = false;
                    if (reader.ReadNormal(section, "Trail" + i + ".IsOnTurret", ref isOnTurret))
                    {
                        data.IsOnTurret = isOnTurret;
                    }

                    List<string> onLands = null;
                    if (reader.ReadStringList(section, "Trail" + i + ".OnLands", ref onLands))
                    {
                        List<LandType> landTypes = null;
                        foreach (string tile in onLands)
                        {
                            if (Enum.TryParse<LandType>(tile, out LandType landType))
                            {
                                if (null == landTypes)
                                {
                                    landTypes = new List<LandType>();
                                }
                                landTypes.Add(landType);
                            }
                        }
                        if (null != landTypes)
                        {
                            data.OnLands = landTypes;
                        }
                    }

                    List<string> onTiles = null;
                    if (reader.ReadStringList(section, "Trail" + i + ".OnTiles", ref onTiles))
                    {
                        List<TileType> tileTypes = null;
                        foreach (string tile in onTiles)
                        {
                            if (Enum.TryParse<TileType>(tile, out TileType tileType))
                            {
                                if (null == tileTypes)
                                {
                                    tileTypes = new List<TileType>();
                                }
                                tileTypes.Add(tileType);
                            }
                        }
                        if (null != tileTypes)
                        {
                            data.OnTiles = tileTypes;
                        }
                    }
                }
                if (null != data)
                {
                    if (null == trailDatas)
                    {
                        trailDatas = new List<TrailData>();
                    }
                    trailDatas.Add(data);
                }
            }
            return null != trailDatas;
        }

    }

}