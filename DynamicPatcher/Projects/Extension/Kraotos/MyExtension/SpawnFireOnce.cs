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
    public class SpawnFireOnceData
    {
        public bool FireOnce;
        public int Delay;

        public SpawnFireOnceData(bool fireOnce)
        {
            this.FireOnce = fireOnce;
            this.Delay = 0;
        }

    }

    public partial class TechnoExt
    {
        private TimerStruct spawnFireOnceDelay = new TimerStruct(0);
        private bool spawnFireFlag = false;

        public unsafe void TechnoClass_Update_SpawnFireOnce()
        {
            CoordStruct sourcePos = OwnerObject.Ref.Base.Base.GetCoords();
            // I'm spawn
            Pointer<TechnoClass> pSpawnOwner = OwnerObject.Ref.SpawnOwner;
            if (!pSpawnOwner.IsNull)
            {
                TechnoExt ext = TechnoExt.ExtMap.Find(pSpawnOwner);
                TechnoTypeExt extType = ext.Type;
                if (null != extType.SpawnFireOnceData && extType.SpawnFireOnceData.FireOnce)
                {
                    ext.CancelSpawnDest();
                }
            }

            // I'm carrier
            Pointer<SpawnManagerClass> pSpawn = OwnerObject.Ref.SpawnManager;
            if (!pSpawn.IsNull)
            {
                if (pSpawn.Ref.Destination.IsNull && pSpawn.Ref.Target.IsNull)
                {
                    spawnFireFlag = false;
                }
            }
        }

        public unsafe void CancelSpawnDest()
        {
            if (!spawnFireFlag)
            {
                spawnFireOnceDelay.Start(Type.SpawnFireOnceData.Delay);
                spawnFireFlag = true;
            }
            else
            {
                if (!spawnFireOnceDelay.InProgress())
                {
                    Pointer<SpawnManagerClass> pSpawn = OwnerObject.Ref.SpawnManager;
                    pSpawn.Ref.Destination = Pointer<AbstractClass>.Zero;
                    pSpawn.Ref.SetTarget(Pointer<AbstractClass>.Zero);
                }
            }
        }

    }

    public partial class TechnoTypeExt
    {
        public SpawnFireOnceData SpawnFireOnceData;

        /// <summary>
        /// [TechnoType]
        /// SpawnFireOnce=no
        /// SpawnFireOnceDelay=0
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        private void ReadSpawnFireOnce(INIReader reader, string section)
        {
            // read flh form Carrier
            bool fireOnce = false;
            if (reader.ReadNormal(section, "SpawnFireOnce", ref fireOnce))
            {
                if (null == SpawnFireOnceData)
                {
                    SpawnFireOnceData = new SpawnFireOnceData(fireOnce);
                }

                int delay = 0;
                if (reader.ReadNormal(section, "SpawnFireOnceDelay", ref delay))
                {
                    SpawnFireOnceData.Delay = delay;
                }
            }
        }
    }
}