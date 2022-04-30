using System.Reflection;
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

        SwizzleablePointer<AnimClass> pChaosAnim = new SwizzleablePointer<AnimClass>(IntPtr.Zero);

        public unsafe void TechnoClass_Init_ChaosAnim()
        {
            string id = RulesExt.Instance.ChaosAnimID;
            if (!string.IsNullOrEmpty(id))
            {
                Pointer<AnimTypeClass> pAnimType = AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(id);
                if (pAnimType.IsNull)
                {
                    RulesExt.Instance.ChaosAnimID = null;
                    return;
                }
                pChaosAnim.Pointer = YRMemory.Create<AnimClass>(pAnimType, OwnerObject.Ref.Base.Base.GetCoords());
                pChaosAnim.Ref.SetOwnerObject(OwnerObject.Convert<ObjectClass>());
                if (!OwnerObject.Ref.Owner.IsNull)
                {
                    pChaosAnim.Ref.Owner = OwnerObject.Ref.Owner;
                }
                pChaosAnim.Ref.Invisible = true;
                OnRenderAction += TechnoClass_Render_ChaosAnim;
            }
        }

        public unsafe void TechnoClass_Render_ChaosAnim()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (pTechno.Ref.Berzerk)
            {
                if (pChaosAnim.Ref.Invisible)
                {
                    pChaosAnim.Ref.Invisible = false;
                }
            }
            else
            {
                // Logger.Log("取消[{0}]的混乱动画{1}", pTechno.Ref.Type.Convert<AbstractTypeClass>().Ref.ID, Type.pChaosAnimType.IsNull ? "is null" : (Type.pChaosAnimType.Pointer.IsNull ? "pointer is null" : Type.pChaosAnimType.Pointer.Convert<AbstractTypeClass>().Ref.ID));
                pChaosAnim.Ref.Invisible = true;
            }
        }


    }

    public partial class RulesExt
    {
        public SwizzleablePointer<AnimTypeClass> pChaosAnimType = new SwizzleablePointer<AnimTypeClass>(IntPtr.Zero);
        public string ChaosAnimID;

        /// <summary>
        /// [AudioVisual]
        /// BerserkAnim=CHAOSANIM
        /// 
        /// </summary>
        /// <param name="reader"></param>
        private void ReadChaosAnim(INIReader reader)
        {
            string chaosAnimId = default;
            if (reader.ReadNormal(SectionAudioVisual, "BerserkAnim", ref chaosAnimId))
            {
                this.ChaosAnimID = chaosAnimId;
            }
        }
    }

}