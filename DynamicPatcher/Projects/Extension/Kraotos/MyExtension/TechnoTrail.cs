using System.Drawing;
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

    public partial class TechnoExt
    {
        public TrailManager trailManager;

        public unsafe void TechnoClass_Put_Trail(Pointer<CoordStruct> pCoord, DirStruct faceDir)
        {
            if (null != Type.TrailDatas && null == trailManager)
            {
                trailManager = new TrailManager();
                trailManager.SetTrailData(Type.TrailDatas);
            }
            trailManager?.SourceLocation(pCoord.Data, faceDir);
        }

        public unsafe void TechnoClass_Render_Trail()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (null == trailManager || pTechno.IsDeadOrInvisibleOrCloaked())
            {
                return;
            }
            // 绘制尾巴
            trailManager?.DrawTrail(pTechno, DrivingState);
        }

        public unsafe void TechnoClass_Remove_Trail(bool isDead)
        {
            if (!isDead)
            {
                trailManager?.ClearLocation();
            }
        }

    }

    public partial class TechnoTypeExt
    {

        public List<TrailData> TrailDatas;

        private void ReadTrail(INIReader reader, string section, INIReader artReader, string artSection)
        {
            // read form Art.ini
            if (TrailManager.ReadTrailData(artReader, artSection, out List<TrailData> trailDatas))
            {
                // Logger.Log("[{0}] 读取 Image={1} 的Art尾巴参数，共{2}条", section, artSection, trailDatas.Count);
                TrailDatas = trailDatas;
            }
        }

    }

    public partial class BulletExt
    {
        public TrailManager trailManager;

        public unsafe void BulletClass_Put_Trail(Pointer<CoordStruct> pCoord)
        {
            if (null != Type.TrailDatas && null == trailManager)
            {
                trailManager = new TrailManager();
                trailManager.SetTrailData(Type.TrailDatas);
                trailManager.pHouse.Pointer = pSourceHouse;
                trailManager.SourceLocation(OwnerObject.Convert<BulletClass>(), pCoord.Data);
            }
        }

        public unsafe void BulletClass_Render_Trail()
        {
            trailManager?.DrawTrail(OwnerObject);
        }
    }

    public partial class BulletTypeExt
    {
        public List<TrailData> TrailDatas;

        private void ReadTrail(INIReader reader, string section, INIReader artReader, string artSection)
        {
            // read from Art.ini
            if (TrailManager.ReadTrailData(artReader, artSection, out List<TrailData> trailDatas))
            {
                TrailDatas = trailDatas;
            }
        }

    }


}