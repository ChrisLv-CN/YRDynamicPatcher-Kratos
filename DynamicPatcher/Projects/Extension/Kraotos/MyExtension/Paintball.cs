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
        public float BrightMultiplier;
        public int Duration;
        public bool Shining;

        private bool paint;
        private bool infinite;
        private TimerStruct timer;

        private int step;

        public PaintballState()
        {
            this.Color = default;
            this.BrightMultiplier = 1.0f;
            this.Duration = 0;
            this.Shining = false;
            this.paint = false;
            this.infinite = false;
            this.timer = new TimerStruct(0);
            this.step = 0;
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
                timer.Start(duration + 1); // 颜色多持续一帧
            }
        }

        public void Enable(ColorStruct color, float brightMultiplier, int duration)
        {
            Enable(color, duration);
            if (brightMultiplier < 0.0f)
            {
                brightMultiplier = 0.0f;
            }
            else if (brightMultiplier > 2.0f)
            {
                brightMultiplier = 2.0f;
            }
            this.BrightMultiplier = brightMultiplier;
        }

        public bool NeedPaint()
        {
            return NeedPaint(out bool changeColor, out bool changeBright);
        }

        public bool NeedPaint(out bool changeColor, out bool changeBright)
        {
            changeColor = default != Color;
            changeBright = 1.0f != BrightMultiplier;
            return paint && NotDone();
        }

        private bool NotDone()
        {
            paint = infinite || timer.InProgress();
            return paint;
        }

        public uint GetBright(uint bright)
        {
            double b = bright * BrightMultiplier;
            if (b < 0)
            {
                b = 0;
            }
            else if (b > 2000)
            {
                b = 2000;
            }
            return (uint)b;
        }

    }

    public partial class TechnoExt
    {
        public PaintballState PaintballState = new PaintballState();

        public unsafe void TechnoClass_Update_Paintball()
        {
            if (OwnerObject.Convert<AbstractClass>().Ref.WhatAmI() == AbstractType.Building)
            {
                if (PaintballState.NeedPaint())
                {
                    // Logger.Log($"{Game.CurrentFrame} - {OwnerObject.Ref.Type.Ref.Base.Base.ID} change color {PaintballState.Color} {changeColor}, change bright {changeBright}, ForceShilded {OwnerObject.Ref.IsForceShilded}");
                    OwnerObject.Ref.Base.Mark(MarkType.CHANGE);
                }

            }
        }

        public unsafe void TechnoClass_DrawSHP_Paintball(REGISTERS* R)
        {
            // if (OwnerObject.Convert<AbstractClass>().Ref.WhatAmI() == AbstractType.Building)
            // {
            //     return;
            // }
            if (!PaintballState.NeedPaint(out bool changeColor, out bool changeBright) || OwnerObject.Ref.Berzerk || OwnerObject.Ref.IsForceShilded || OwnerObject.Ref.Base.IsIronCurtained())
            {
                return;
            }

            // Logger.Log($"{Game.CurrentFrame} - {OwnerObject} {OwnerObject.Ref.Type.Ref.Base.Base.ID} change color {PaintballState.Color} {changeColor}, change bright {changeBright}");
            if (changeColor)
            {
                ColorStruct colorAdd = ExHelper.Color2ColorAdd(PaintballState.Color);
                // Logger.Log("RGB888 = {0}, RGB565 = {1}, RGB565 = {2}", Paintball.Color, colorAdd, ExHelper.ColorAdd2RGB565(colorAdd));
                R->EAX = ExHelper.ColorAdd2RGB565(colorAdd);
            }
            if (changeBright)
            {
                // uint bright = R->EBP;
                R->EBP = PaintballState.GetBright(R->EBP);
            }
        }

        public unsafe void TechnoClass_DrawSHP_Paintball_BuildAnim(REGISTERS* R)
        {
            if (!PaintballState.NeedPaint(out bool changeColor, out bool changeBright) || OwnerObject.Ref.Berzerk || OwnerObject.Ref.IsForceShilded || OwnerObject.Ref.Base.IsIronCurtained())
            {
                return;
            }

            // Logger.Log($"{Game.CurrentFrame} - {OwnerObject} {OwnerObject.Ref.Type.Ref.Base.Base.ID} change color {PaintballState.Color} {changeColor}, change bright {changeBright}");
            if (changeColor)
            {
                ColorStruct colorAdd = ExHelper.Color2ColorAdd(PaintballState.Color);
                R->EBP = ExHelper.ColorAdd2RGB565(colorAdd);
            }
            if (changeBright)
            {
                uint bright = R->Stack<uint>(0x38);
                R->Stack<uint>(0x38, PaintballState.GetBright(bright));
            }
        }

        public unsafe void TechnoClass_DrawVXL_Paintball(REGISTERS* R, bool isBuilding)
        {

            if (!PaintballState.NeedPaint(out bool changeColor, out bool changeBright) || OwnerObject.Ref.Berzerk || OwnerObject.Ref.IsForceShilded || OwnerObject.Ref.Base.IsIronCurtained())
            {
                return;
            }
            if (changeColor)
            {
                ColorStruct colorAdd = ExHelper.Color2ColorAdd(PaintballState.Color);
                // Logger.Log("RGB888 = {0}, RGB565 = {1}, RGB565 = {2}", Paintball.Color, colorAdd, ExHelper.ColorAdd2RGB565(colorAdd));
                if (isBuilding)
                {
                    // vxl turret
                    R->Stack<uint>(0x24, ExHelper.ColorAdd2RGB565(colorAdd));
                }
                else
                {
                    R->ESI = ExHelper.ColorAdd2RGB565(colorAdd);
                }
            }
            if (changeBright)
            {
                if (isBuilding)
                {
                    // Vxl turret
                    uint bright = R->Stack<uint>(0x20);
                    R->Stack<uint>(0x20, PaintballState.GetBright(bright));
                }
                else
                {
                    uint bright = R->Stack<uint>(0x1E0);
                    R->Stack<uint>(0x1E0, PaintballState.GetBright(bright));
                }
            }
        }

    }

}