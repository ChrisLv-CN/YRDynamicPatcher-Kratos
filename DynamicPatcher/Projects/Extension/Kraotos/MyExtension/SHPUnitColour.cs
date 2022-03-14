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

        public unsafe void TechnoClass_DrawSHP_Colour(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            TechnoTypeExt extType = Type;
            if (!pTechno.IsNull && !pTechno.Ref.IsVoxel() && pTechno.Ref.Base.Base.WhatAmI() == AbstractType.Unit)
            {
                //== Colour ==
                // ForceShield
                // LaserTarget
                // Berzerk
                if (pTechno.Ref.Berzerk)
                {
                    R->EAX = ExHelper.ColorAdd2RGB565(RulesClass.BerserkColor);
                    // R->EAX = Drawing.Color16bit(color);
                    // R->EBP = 200;
                    // R->EAX = 0x42945536;
                }
                // add PowerUp

                //== Darker ==
                // IronCurtain
                // EMP
                // NoPower
            }
        }


    }

    public partial class TechnoTypeExt
    {
    }

}