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
            }
        }
    }


    [Serializable]
    public class AttachStatus : AttachEffectBehaviour
    {
        public AttachStatusType Type;
        public bool Active;
        public TechnoExt technoExt;

        public AttachStatus(AttachStatusType type, AttachEffectType attachEffectType) : base(attachEffectType)
        {
            this.Type = type;
            this.Active = false;
        }

        public override bool IsAlive()
        {
            return this.Active;
        }

        public override void Enable(Pointer<ObjectClass> pObject, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker)
        {
            this.Active = true;
            if (pObject.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                this.technoExt = ext;
                ext?.RecalculateStatus();
            }
        }

        public override void Disable(CoordStruct location)
        {
            this.Active = false;
            technoExt?.RecalculateStatus();
        }

    }

}