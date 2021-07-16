
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;

namespace ExtensionHooks
{
    public class WarheadTypeExtHooks
    {
        [Hook(HookType.AresHook, Address = 0x75D1A9, Size = 7)]
        public static unsafe UInt32 WarheadTypeClass_CTOR(REGISTERS* R)
        {
            return WarheadTypeExt.WarheadTypeClass_CTOR(R);
        }

        [Hook(HookType.AresHook, Address = 0x75E5C8, Size = 6)]
        public static unsafe UInt32 WarheadTypeClass_SDDTOR(REGISTERS* R)
        {
            return WarheadTypeExt.WarheadTypeClass_SDDTOR(R);
        }

        [Hook(HookType.AresHook, Address = 0x75DEAF, Size = 5)]
        [Hook(HookType.AresHook, Address = 0x75DEA0, Size = 5)]
        public static unsafe UInt32 WarheadTypeClass_LoadFromINI(REGISTERS* R)
        {
            return WarheadTypeExt.WarheadTypeClass_LoadFromINI(R);
        }

        [Hook(HookType.AresHook, Address = 0x75E2C0, Size = 5)]
        [Hook(HookType.AresHook, Address = 0x75E0C0, Size = 8)]
        public static unsafe UInt32 WarheadTypeClass_SaveLoad_Prefix(REGISTERS* R)
        {
            return WarheadTypeExt.WarheadTypeClass_SaveLoad_Prefix(R);
        }

        [Hook(HookType.AresHook, Address = 0x75E2AE, Size = 7)]
        public static unsafe UInt32 WarheadTypeClass_Load_Suffix(REGISTERS* R)
        {
            return WarheadTypeExt.WarheadTypeClass_Load_Suffix(R);
        }

        [Hook(HookType.AresHook, Address = 0x75E39C, Size = 5)]
        public static unsafe UInt32 WarheadTypeClass_Save_Suffix(REGISTERS* R)
        {
            return WarheadTypeExt.WarheadTypeClass_Save_Suffix(R);
        }
    }
}