
using System.Drawing;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Script;
using Extension.Utilities;

namespace ExtensionHooks
{
    public class MapExtHooks
    {

        [Hook(HookType.AresHook, Address = 0x69252D, Size = 6)]
        public static unsafe UInt32 ScrollClass_ProcessClickCoords_VirtualUnit(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && ext.VirtualUnit)
                {
                    return 0x6925E6;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return (uint)0;
        }

    }
}