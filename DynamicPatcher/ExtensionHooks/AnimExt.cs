
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;

namespace ExtensionHooks
{
    public class AnimExtHooks
    {
        
        [Hook(HookType.AresHook, Address = 0x0042312A, Size = 6)]
        static public unsafe UInt32 AnimClass_Draw_Remap(REGISTERS* R)
        {
            //Logger.Log("Hook 0x00423130 calling...");
            Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
            if (!pAnim.IsNull && pAnim.Ref.Type.Ref.AltPalette && !pAnim.Ref.Owner.IsNull)
            {
                // string id = pAnim.Ref.Owner.IsNull ? "NULL" : pAnim.Ref.Owner.Ref.Type.Ref.Base.ID;
                // ColorStruct color = pAnim.Ref.Owner.IsNull ? default : pAnim.Ref.Owner.Ref.Color;
                // Logger.Log("Anim[{0}] 从所属中{1}获取颜色. Colour={2}, Anim.RemapColor={3}", pAnim.Ref.Type.Convert<AbstractTypeClass>().Ref.ID, id, color, pAnim.Ref.RemapColour);
                return 0x00423130;
            }
            return 0x004231F3;
        }

    }
}