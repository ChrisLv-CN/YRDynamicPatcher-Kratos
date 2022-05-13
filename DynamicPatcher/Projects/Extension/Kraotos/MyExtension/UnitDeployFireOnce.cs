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
        public unsafe void UnitClass_Init_UnitDeployFireOnce()
        {
            if (OwnerObject.Ref.Type.Ref.DeployFire)
            {
                OnFireOnceAction += UnitClass_Fire_UnitDeployFireOnce;
            }
        }

        public unsafe void UnitClass_Fire_UnitDeployFireOnce()
        {
            Pointer<MissionClass> pMission = OwnerObject.Convert<MissionClass>();
            if (pMission.Ref.CurrentMission == Mission.Unload)
            {
                pMission.Ref.QueueMission(Mission.Stop, true);
            }
        }


    }

}