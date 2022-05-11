using System.Threading;
using System.IO;
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
        public AEState<DeselectType> DeselectState => AttachEffectManager.DeselectState;

        public unsafe void TechnoClass_Init_Deselect()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (null != Type.DeselectData && Type.DeselectData.Enable)
            {
                DeselectState.Enable(Type.DeselectData);
            }

        }

        public unsafe void TechnoClass_Update_Deselect()
        {
            if (OwnerObject.Ref.Base.IsSelected && DeselectState.IsActive())
            {
                OwnerObject.Ref.Base.Deselect();
            }
        }

        public unsafe bool TechnoClass_Select_Deselect()
        {
            return !DeselectState.IsActive();
        }

    }


    public partial class TechnoTypeExt
    {
        public DeselectType DeselectData;

        /// <summary>
        /// [TechnoType]
        /// Select.Disable=HTNK
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadDeselect(INIReader reader, string section)
        {
            // Deselect
            DeselectType temp = new DeselectType();
            if (temp.TryReadType(reader, section))
            {
                this.DeselectData = temp;
            }
            else
            {
                temp = null;
            }

        }
    }

}