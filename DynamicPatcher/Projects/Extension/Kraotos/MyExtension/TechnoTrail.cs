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

        public unsafe void TechnoClass_Init_Trail()
        {
            if (null != Type.TrailDatas && null == trailManager)
            {
                trailManager = new TrailManager();
                trailManager.SetTrailData(Type.TrailDatas);

                OnPutAction += TechnoClass_Put_Trail;
                OnUpdateAction += TechnoClass_Update_Trail;
                OnRemoveAction += TechnoClass_Remove_Trail;
            }
        }

        public unsafe void TechnoClass_Put_Trail(Pointer<CoordStruct> pCoord, short faceDirValue8)
        {
            // Logger.Log($"{Game.CurrentFrame} - {OwnerObject} [{OwnerObject.Ref.Type.Ref.Base.Base.ID}] put. {OwnerObject.Ref.Base.Base.GetCoords()} {pCoord.Data}");
            trailManager?.Put(pCoord.Data, faceDirValue8);
        }

        public unsafe void TechnoClass_Update_Trail()
        {
            // Logger.Log($"{Game.CurrentFrame} - {OwnerObject} [{OwnerObject.Ref.Type.Ref.Base.Base.ID}] update. {OwnerObject.Ref.Base.Base.GetCoords()}");
            if (OwnerObject.IsDeadOrInvisibleOrCloaked())
            {
                trailManager?.Update(OwnerObject, DrivingState);
            }
            else
            {
                // 绘制尾巴
                trailManager?.DrawTrail(OwnerObject, DrivingState);
            }
        }

        public unsafe void TechnoClass_Remove_Trail()
        {
            trailManager?.Remove();
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
                trailManager.Put(OwnerObject.Convert<BulletClass>(), pCoord.Data);
            }
        }

        public unsafe void BulletClass_Update_Trail()
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