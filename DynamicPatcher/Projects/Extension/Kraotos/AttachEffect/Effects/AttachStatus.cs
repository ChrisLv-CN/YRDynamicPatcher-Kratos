using DynamicPatcher;
using Extension.Ext;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    public partial class AttachEffect
    {
        public AttachStatus AttachStatus;

        private void InitAttachStatus()
        {
            if (null != Type.AttachStatusType)
            {
                this.AttachStatus = Type.AttachStatusType.CreateObject(Type);
                RegisterAction(AttachStatus);
            }
        }
    }


    [Serializable]
    public class AttachStatus : Effect<AttachStatusType>
    {
        public bool Active;
        public ExtensionReference<TechnoExt> technoExt;

        public AttachStatus()
        {
            this.Active = false;
        }

        public override bool IsAlive()
        {
            return this.Active;
        }

        public override void OnEnable(Pointer<ObjectClass> pObject)
        {
            this.Active = true;
            if (pObject.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                technoExt = TechnoExt.ExtMap.Find(pTechno);
                technoExt.Get().RecalculateStatus();
            }
        }

        public override void Disable(CoordStruct location)
        {
            this.Active = false;
            if (technoExt.TryGet(out TechnoExt ext))
            {
                ext.RecalculateStatus();
            }
        }

    }

}