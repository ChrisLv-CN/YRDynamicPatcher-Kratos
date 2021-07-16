
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;

namespace ExtensionHooks
{
    public class LaserDrawExtHooks
    {
        [Hook(HookType.AresHook, Address = 0x550F6A, Size = 8)]
        public static unsafe UInt32 LaserDrawClass_Fade(REGISTERS* R)
        {
            return LaserDrawExt.LaserDrawClass_Fade(R);
        }
    }
}

