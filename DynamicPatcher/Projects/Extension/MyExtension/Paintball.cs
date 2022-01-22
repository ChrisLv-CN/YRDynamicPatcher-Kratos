using System.Drawing;
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

    [Serializable]
    public class PaintballState
    {
        public ColorStruct Color;

        public bool NeedPoint;

        public PaintballState()
        {
            this.Color = default;
            this.NeedPoint = false;
        }

        public void Enable(ColorStruct color)
        {
            this.Color = color;
            this.NeedPoint = true;
        }

        public void Disable()
        {
            NeedPoint = false;
        }

    }

    public partial class TechnoExt
    {
        public PaintballState PaintballState = new PaintballState();

        public unsafe void TechnoClass_DrawSHP_Paintball(REGISTERS* R)
        {
            if (OwnerObject.Convert<AbstractClass>().Ref.WhatAmI() == AbstractType.Building)
            {
                return;
            }
            if (PaintballState.NeedPoint && !OwnerObject.Ref.Berzerk)
            {
                ColorStruct colorAdd = ExHelper.Color2ColorAdd(PaintballState.Color);
                // Logger.Log("RGB888 = {0}, RGB565 = {1}, RGB565 = {2}", Paintball.Color, colorAdd, ExHelper.ColorAdd2RGB565(colorAdd));
                R->EAX = ExHelper.ColorAdd2RGB565(colorAdd);
            }
        }

        public unsafe void TechnoClass_DrawVXL_Paintball(REGISTERS* R)
        {
            if (OwnerObject.Convert<AbstractClass>().Ref.WhatAmI() == AbstractType.Building)
            {
                return;
            }
            if (PaintballState.NeedPoint && !OwnerObject.Ref.Berzerk)
            {
                ColorStruct colorAdd = ExHelper.Color2ColorAdd(PaintballState.Color);
                // Logger.Log("RGB888 = {0}, RGB565 = {1}, RGB565 = {2}", Paintball.Color, colorAdd, ExHelper.ColorAdd2RGB565(colorAdd));
                R->ESI = ExHelper.ColorAdd2RGB565(colorAdd);
            }
        }
    }

}