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
        public Transform Transform;

        private void InitTransform()
        {
            if (null != Type.TransformType)
            {
                this.Transform = Type.TransformType.CreateObject(Type);
                RegisterAction(Transform);
            }
        }
    }


    [Serializable]
    public class Transform : Effect<TransformType>
    {
        public ExtensionReference<TechnoExt> OwnerExt;

        public Transform()
        {

        }

        public override void OnEnable(Pointer<ObjectClass> pObject)
        {
            if (pObject.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                OwnerExt = TechnoExt.ExtMap.Find(pTechno);

                // Logger.Log("Transform AE enable. Trans to {0}", Type.ToType);
                OwnerExt.Get().TryConvertTypeTo(Type.ToType);
            }
        }

        public override void Disable(CoordStruct location)
        {
            // Logger.Log("Transform AE disable.");
            if (OwnerExt.TryGet(out TechnoExt ext))
            {
                ext.CancelConverType(Type.ToType);
            }
        }

    }

}