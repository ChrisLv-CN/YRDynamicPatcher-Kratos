
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
        // [Hook(HookType.AresHook, Address = 0x4F4497, Size = 6)] // GScreenClass_Render
        [Hook(HookType.AresHook, Address = 0x4F4583, Size = 6)] // GScreenClass_Render
        public static unsafe UInt32 GScreenClass_Render(REGISTERS* R)
        {
            // Logger.Log($"{Game.CurrentFrame} GScreenClass_Render call");
            PrintTextManager.PrintText();
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x6A70AE, Size = 5)] // SidebarClass_Draw_It
        public static unsafe UInt32 SidebarClass_Draw_It(REGISTERS* R)
        {
            // Logger.Log($"{Game.CurrentFrame} SidebarClass_Draw_It call"); // before GScreen
            Kratos.DrawActiveMessage();
            return 0;
        }

    }
}

