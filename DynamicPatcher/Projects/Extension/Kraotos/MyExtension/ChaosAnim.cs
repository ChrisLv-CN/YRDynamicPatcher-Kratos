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
        private SwizzleablePointer<AnimClass> pChaosAnim = new SwizzleablePointer<AnimClass>(IntPtr.Zero);

        public unsafe void TechnoClass_Update_ChaosAnim()
        {
            if (!RulesExt.Instance.pChaosAnimType.IsNull)
            {
                Pointer<TechnoClass> pTechno = OwnerObject;
                if (pTechno.Ref.Berzerk)
                {
                    CoordStruct pos = pTechno.Ref.Base.Base.GetCoords();
                    if (pChaosAnim.IsNull)
                    {
                        // Logger.Log("为[{0}]创建混乱动画{1}", pTechno.Ref.Type.Convert<AbstractTypeClass>().Ref.ID, Type.pChaosAnimType.IsNull ? "is null" : (Type.pChaosAnimType.Pointer.IsNull ? "pointer is null" : Type.pChaosAnimType.Pointer.Convert<AbstractTypeClass>().Ref.ID));
                        Pointer<AnimClass> pAnim = YRMemory.Create<AnimClass>(RulesExt.Instance.pChaosAnimType.Pointer, pos);
                        pAnim.Ref.SetOwnerObject(pTechno.Convert<ObjectClass>());
                        if (!OwnerObject.Ref.Owner.IsNull)
                        {
                            pAnim.Ref.Owner = OwnerObject.Ref.Owner;
                        }
                        pChaosAnim.Pointer = pAnim;
                    }
                    if (pChaosAnim.Ref.Invisible)
                    {
                        pChaosAnim.Ref.Invisible = false;
                    }
                }
                else
                {
                    if (!pChaosAnim.IsNull)
                    {
                        // Logger.Log("取消[{0}]的混乱动画{1}", pTechno.Ref.Type.Convert<AbstractTypeClass>().Ref.ID, Type.pChaosAnimType.IsNull ? "is null" : (Type.pChaosAnimType.Pointer.IsNull ? "pointer is null" : Type.pChaosAnimType.Pointer.Convert<AbstractTypeClass>().Ref.ID));
                        pChaosAnim.Ref.Invisible = true;
                    }
                }
            }

        }


    }

    public partial class RulesExt
    {
        public SwizzleablePointer<AnimTypeClass> pChaosAnimType = new SwizzleablePointer<AnimTypeClass>(IntPtr.Zero);

        /// <summary>
        /// [AudioVisual]
        /// BerserkAnim=CHAOSANIM
        /// 
        /// </summary>
        /// <param name="reader"></param>
        private void ReadChaosAnim(INIReader reader)
        {
            string chaosAnimId = default;
            if (pChaosAnimType.IsNull && reader.ReadNormal(SectionAudioVisual, "BerserkAnim", ref chaosAnimId))
            {
                pChaosAnimType.Pointer = AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(chaosAnimId);
            }
        }
    }

}