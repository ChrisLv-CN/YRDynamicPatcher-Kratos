
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;

namespace ExtensionHooks
{
    public class GScreenExtHook
    {
        [Hook(HookType.AresHook, Address = 0x4F4583, Size = 6)]
        public static unsafe UInt32 GScreenClass_Render(REGISTERS* R)
        {
            Kratos.DrawActiveMessage();
            PrintTextManager.PrintText();
            return 0;
        }
    }
}

