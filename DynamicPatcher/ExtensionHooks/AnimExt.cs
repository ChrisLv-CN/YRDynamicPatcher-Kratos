
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
        

        // [Hook(HookType.AresHook, Address = 0x42310A, Size = 6)]
        // public static unsafe UInt32 AnimClass_Draw_RemapX(REGISTERS* R)
        // {
        //     //Logger.Log("Hook 0x00423130 calling...");
        //     Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
        //     if (!pAnim.IsNull && pAnim.Ref.Type.Ref.AltPalette && !pAnim.Ref.Owner.IsNull)
        //     {
        //         string id = pAnim.Ref.Owner.IsNull ? "NULL" : pAnim.Ref.Owner.Ref.Type.Ref.Base.ID;
        //         int index = pAnim.Ref.Owner.IsNull ? -1 : pAnim.Ref.Owner.Ref.ArrayIndex;
        //         ColorStruct color = pAnim.Ref.Owner.IsNull ? default : pAnim.Ref.Owner.Ref.Color;
        //         var edx = R->EDX;
        //         var ecx = R->ECX;
        //         Logger.Log("Anim[{0}] 从所属中{1}-{2}获取颜色. Colour={3} ECX={4}, EDX={5}, pHouse={6}", pAnim.Ref.Type.Convert<AbstractTypeClass>().Ref.ID, index, id, color, ecx, edx, pAnim.Ref.Owner);
        //     }
        //     return 0;
        // }

        [Hook(HookType.AresHook, Address = 0x42312A, Size = 6)]
        public static unsafe UInt32 AnimClass_Draw_Remap(REGISTERS* R)
        {
            //Logger.Log("Hook 0x00423130 calling...");
            Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
            if (!pAnim.IsNull && pAnim.Ref.Type.Ref.AltPalette && !pAnim.Ref.Owner.IsNull)
            {
                // string id = pAnim.Ref.Owner.IsNull ? "NULL" : pAnim.Ref.Owner.Ref.Type.Ref.Base.ID;
                // int index = pAnim.Ref.Owner.IsNull ? -1 : pAnim.Ref.Owner.Ref.ArrayIndex;
                // ColorStruct color = pAnim.Ref.Owner.IsNull ? default : pAnim.Ref.Owner.Ref.Color;
                // Logger.Log(" 强转所属 Anim[{0}] 从所属中{1}-{2}获取颜色. Colour={3}, pHouse={4}", pAnim.Ref.Type.Convert<AbstractTypeClass>().Ref.ID, index, id, color, pAnim.Ref.Owner);
                return 0x423130;
            }
            return 0x4231F3;
        }

        [Hook(HookType.AresHook, Address = 0x423136, Size = 6)]
        public static unsafe UInt32 AnimClass_Draw_Remap2(REGISTERS* R)
        {
            Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
            if (!pAnim.IsNull && pAnim.Ref.Type.Ref.AltPalette && !pAnim.Ref.Owner.IsNull)
            {
                // var edx = R->EDX;
                // var ecx = R->ECX;
                R->ECX = (uint)pAnim.Ref.Owner;
                // Logger.Log(" - Anim[{0}] ECX={1}, EDX={2}", pAnim.Ref.Type.Convert<AbstractTypeClass>().Ref.ID, ecx, edx);
            }
            return 0;
        }

    }
}