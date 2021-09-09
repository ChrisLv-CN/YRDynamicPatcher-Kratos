
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;

namespace ExtensionHooks
{
    public class WeaponTypeExtHooks
    {
        [Hook(HookType.AresHook, Address = 0x771EE9, Size = 5)]
        public static unsafe UInt32 WeaponTypeClass_CTOR(REGISTERS* R)
        {
            return WeaponTypeExt.WeaponTypeClass_CTOR(R);
        }

        [Hook(HookType.AresHook, Address = 0x77311D, Size = 6)]
        public static unsafe UInt32 WeaponTypeClass_SDDTOR(REGISTERS* R)
        {
            return WeaponTypeExt.WeaponTypeClass_SDDTOR(R);
        }

        [Hook(HookType.AresHook, Address = 0x7729C7, Size = 5)]
        [Hook(HookType.AresHook, Address = 0x7729D6, Size = 5)]
        [Hook(HookType.AresHook, Address = 0x7729B0, Size = 5)]
        public static unsafe UInt32 WeaponTypeClass_LoadFromINI(REGISTERS* R)
        {
            return WeaponTypeExt.WeaponTypeClass_LoadFromINI(R);
        }

        [Hook(HookType.AresHook, Address = 0x772EB0, Size = 5)]
        [Hook(HookType.AresHook, Address = 0x772CD0, Size = 7)]
        public static unsafe UInt32 WeaponTypeClass_SaveLoad_Prefix(REGISTERS* R)
        {
            return WeaponTypeExt.WeaponTypeClass_SaveLoad_Prefix(R);
        }

        [Hook(HookType.AresHook, Address = 0x772EA6, Size = 6)]
        public static unsafe UInt32 WeaponTypeClass_Load_Suffix(REGISTERS* R)
        {
            return WeaponTypeExt.WeaponTypeClass_Load_Suffix(R);
        }

        [Hook(HookType.AresHook, Address = 0x772F8C, Size = 5)]
        public static unsafe UInt32 WeaponTypeClass_Save_Suffix(REGISTERS* R)
        {
            return WeaponTypeExt.WeaponTypeClass_Save_Suffix(R);
        }
    }
}