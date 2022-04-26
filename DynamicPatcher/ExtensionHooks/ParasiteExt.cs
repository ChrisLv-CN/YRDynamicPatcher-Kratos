
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;

namespace ExtensionHooks
{
    public class ParasiteExtHooks
    {

        // Take over create Anim by ParasiteEat
        [Hook(HookType.AresHook, Address = 0x62A13F, Size = 5)]
        public static unsafe UInt32 ParasiteClass_Update_Anim_Remap(REGISTERS* R)
        {
            try
            {
                Pointer<ParasiteClass> pParasite = (IntPtr)R->ESI;
                Pointer<AnimTypeClass> pAnimType = (IntPtr)R->EBP;
                CoordStruct location = R->Stack<CoordStruct>(0x34);
                // Logger.Log($"{Game.CurrentFrame} - 播放寄生动画 {pAnimType} [{pAnimType.Ref.Base.Base.ID}], 位置 {location}, 所属 {pParasite.Ref.Owner}, 受害者 {pParasite.Ref.Victim}");
                Pointer<AnimClass> pAnim = YRMemory.Create<AnimClass>(pAnimType, location);
                pAnim.Ref.SetOwnerObject(pParasite.Ref.Victim.Convert<ObjectClass>());
                pAnim.Ref.Owner = pParasite.Ref.Owner.Ref.Base.Owner;
                return 0x62A16A;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        // Eat by Ginormous Squid
        [Hook(HookType.AresHook, Address = 0x6297F0, Size = 6)]
        public static unsafe UInt32 ParasiteClass_GrappleUpdate_Anim_Remap(REGISTERS* R)
        {
            try
            {

                Pointer<ParasiteClass> pParasite = (IntPtr)R->ESI;
                // Logger.Log($"{Game.CurrentFrame} - 大鱿鱼咬人，所属 {pParasite.Ref.Owner}, 受害者 {pParasite.Ref.Victim}");

            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;

        }
    }
}

