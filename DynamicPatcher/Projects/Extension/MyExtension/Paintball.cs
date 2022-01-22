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
        public int Duration;

        private bool paint;
        private bool infinite;
        private TimerStruct timer;

        public PaintballState()
        {
            this.Color = default;
            this.Duration = 0;
            this.paint = false;
            this.infinite = false;
            this.timer = new TimerStruct(0);
        }

        public void Enable(ColorStruct color, int duration)
        {
            this.Color = color;
            this.Duration = duration;
            this.paint = duration != 0;
            if (duration < 0)
            {
                infinite = true;
                timer.Start(0);
            }
            else
            {
                infinite = false;
                timer.Start(duration + 1);
            }
        }

        public bool NeedPaint()
        {
            return paint && NotDone();
        }

        private bool NotDone()
        {
            paint = infinite || timer.InProgress();
            return paint;
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
            if (PaintballState.NeedPaint() && !OwnerObject.Ref.Berzerk)
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
            if (PaintballState.NeedPaint() && !OwnerObject.Ref.Berzerk)
            {
                ColorStruct colorAdd = ExHelper.Color2ColorAdd(PaintballState.Color);
                // Logger.Log("RGB888 = {0}, RGB565 = {1}, RGB565 = {2}", Paintball.Color, colorAdd, ExHelper.ColorAdd2RGB565(colorAdd));
                R->ESI = ExHelper.ColorAdd2RGB565(colorAdd);
            }
        }

    }

}