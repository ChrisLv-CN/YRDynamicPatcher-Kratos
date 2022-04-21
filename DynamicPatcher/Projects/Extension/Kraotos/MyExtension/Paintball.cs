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


    public partial class TechnoExt
    {

        private PaintballState PaintballState => AttachEffectManager.PaintballState;

        public unsafe void TechnoClass_Update_Paintball()
        {
            if (OwnerObject.Convert<AbstractClass>().Ref.WhatAmI() == AbstractType.Building)
            {
                if (PaintballState.IsActive())
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
                // Logger.Log("RGB888 = {0}, RGB565 = {1}, RGB565 = {2}", Paintball.Color, colorAdd, ExHelper.ColorAdd2RGB565(colorAdd));
                R->EAX = PaintballState.GetColor();
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
                R->EBP = PaintballState.GetColor();
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
                uint color = PaintballState.GetColor();
                // Logger.Log("RGB888 = {0}, RGB565 = {1}, RGB565 = {2}", Paintball.Color, colorAdd, ExHelper.ColorAdd2RGB565(colorAdd));
                if (isBuilding)
                {
                    // vxl turret
                    R->Stack<uint>(0x24, color);
                }
                else
                {
                    R->ESI = color;
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