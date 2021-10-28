
using System.Net;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Script;
using Extension.Decorators;
using Extension.Utilities;
using System.Threading.Tasks;

namespace Scripts
{
    [Serializable]
    public class ParaScript : TechnoScriptable
    {
        private bool flag = false;

        public ParaScript(TechnoExt owner) : base(owner) { }

        public override void OnUpdate()
        {
            
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            Mission mission = pTechno.Convert<ObjectClass>().Ref.GetCurrentMission();
            if (!flag && mission == Mission.Hunt)
            {
                flag = true;
                pTechno.Convert<MissionClass>().Ref.QueueMission(Mission.Area_Guard, false);
            }
        }

    }

}

