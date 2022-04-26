using System.Drawing;
using DynamicPatcher;
using Extension.Decorators;
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
        public bool VirtualUnit;

        public unsafe void TechnoClass_Init_VirtualUnit()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            this.VirtualUnit = Type.VirtualUnit;
            // Logger.Log("{0}, 从Type读入虚单位标签{1}", pTechno.Ref.Type.Ref.Base.Base.ID, Type.VirtualUnit);
            if (VirtualUnit)
            {
                pTechno.Ref.Base.Mark(MarkType.UP);
                pTechno.Ref.Base.IsOnMap = false;

                pTechno.Ref.Type.Ref.DontScore = true;
                pTechno.Ref.Type.Ref.Base.Insignificant = true;
                pTechno.Ref.Type.Ref.Base.Selectable = false;
                pTechno.Ref.Type.Ref.Base.Immune = true;
            }
        }

        public unsafe bool TechnoClass_Select_VirtualUnit()
        {
            return !VirtualUnit;
        }

    }

    public partial class TechnoTypeExt
    {
        public bool VirtualUnit;

        /// <summary>
        /// [TechnoType]
        /// VirtualUnit=no
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        private void ReadVirtualUnit(INIReader reader, string section)
        {
            bool virtualUnit = false;
            if (reader.ReadNormal(section, "VirtualUnit", ref virtualUnit))
            {
                VirtualUnit = virtualUnit;
            }
        }
    }
}