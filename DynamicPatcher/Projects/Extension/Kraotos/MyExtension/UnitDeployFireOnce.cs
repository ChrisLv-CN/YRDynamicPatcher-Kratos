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
        public unsafe void TechnoClass_Init_UnitDeployFireOnce()
        {
            if (OwnerObject.CastIf(AbstractType.Unit, out Pointer<UnitClass> pUnit) && pUnit.Ref.Type.Ref.Base.DeployFire)
            {
                OnFireOnceAction += TechnoClass_Fire_UnitDeployFireOnce;
            }
        }

        public unsafe void TechnoClass_Fire_UnitDeployFireOnce()
        {
            Pointer<MissionClass> pMission = OwnerObject.Convert<MissionClass>();
            if (pMission.Ref.CurrentMission == Mission.Unload)
            {
                pMission.Ref.QueueMission(Mission.Stop, true);
            }
        }


    }

}