
using System.Runtime.InteropServices.ComTypes;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Utilities;
using Extension.Script;

namespace ExtensionHooks
{

    public class RulesExtHooks
    {
        [Hook(HookType.AresHook, Address = 0x667A1D, Size = 5)]
        public static unsafe UInt32 RulesClass_CTOR(REGISTERS* R)
        {
            Pointer<RulesClass> pRules = (IntPtr)R->ESI;
            RulesExt.Allocate(pRules);
            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x667A30, Size = 5)]
        public static unsafe UInt32 RulesClass_DTOR(REGISTERS* R)
        {
            Pointer<RulesClass> pRules = (IntPtr)R->ECX;
            RulesExt.Remove(pRules);
            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x674730, Size = 6)]
        [Hook(HookType.AresHook, Address = 0x675210, Size = 5)]
        public static unsafe UInt32 RulesClass_SaveLoad_Prefix(REGISTERS* R)
        {
            Pointer<IStream> pStm = R->Stack<IntPtr>(0x4);

            IStream stream = Marshal.GetObjectForIUnknown(pStm) as IStream;
            RulesExt.RulesClass_SaveLoad_Prefix(stream);
            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x678841, Size = 7)]
        public static unsafe UInt32 RulesClass_Load_Suffix(REGISTERS* R)
        {
            RulesExt.RulesClass_Load_Suffix();
            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x675205, Size = 8)]
        public static unsafe UInt32 RulesClass_Save_Suffix(REGISTERS* R)
        {
            RulesExt.RulesClass_Save_Suffix();
            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x668BF0, Size = 5)]
        public static unsafe UInt32 RulesClass_Addition(REGISTERS* R)
        {
            Pointer<RulesClass> pRules = (IntPtr)R->ECX;
            Pointer<CCINIClass> pINI = R->Stack<IntPtr>(0x4);

            RulesExt.LoadFromINIFile(pRules, pINI);
            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x679A15, Size = 6)]
        public static unsafe UInt32 RulesData_LoadBeforeTypeData(REGISTERS* R)
        {
            Pointer<RulesClass> pRules = (IntPtr)R->ECX;
            Pointer<CCINIClass> pINI = R->Stack<IntPtr>(0x4);

            RulesExt.RulesData_LoadBeforeTypeData(pRules, pINI);
            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x679CAF, Size = 5)]
        public static unsafe UInt32 RulesData_LoadAfterTypeData(REGISTERS* R)
        {
            Pointer<RulesClass> pRules = RulesClass.Instance;
            Pointer<CCINIClass> pINI = (IntPtr)R->ESI;

            RulesExt.RulesData_LoadAfterTypeData(pRules, pINI);
            return (uint)0;
        }

    }
}