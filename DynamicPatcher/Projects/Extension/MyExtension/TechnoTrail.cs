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
        public TrailManager trailManager = new TrailManager();

        public unsafe void TechnoClass_Init_Trail()
        {
            if (null != Type.TrailDatas)
            {
                // ColorStruct houseColor = default;
                // if (!OwnerObject.Ref.Owner.IsNull)
                // {
                //     houseColor = OwnerObject.Ref.Owner.Ref.LaserColor;
                // }
                trailManager.SetTrailData(Type.TrailDatas);
            }
        }

        public unsafe void TechnoClass_Update_Trail()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (null != trailManager && pTechno.Ref.Base.IsVisible && pTechno.Ref.CloakStates == CloakStates.UnCloaked)
            {
                // 绘制尾巴
                trailManager.DrawTrail(pTechno, DrivingState);
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