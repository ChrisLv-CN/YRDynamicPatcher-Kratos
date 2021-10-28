
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

        // char __stdcall ScrollClass_692300(dwXYZ *arg0, __int16 *a2, Point3D *xyz, _TechnoClass **target, _BYTE *a5, _BOOL1 *a6)
        // {
        //    v27 = sub_6DA380(TacticalMap, arg0);
        // }
        // 
        [Hook(HookType.AresHook, Address = 0x6DA3EB, Size = 6)]
        public static unsafe UInt32 sub_6DA380_Stand(REGISTERS* R)
        {
            // 选择鼠标指向的全部对象，筛选
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                // Logger.Log("sub_6DA380 calling 3, ESI = {0} {1}", R->ESI, pTechno.IsNull ? "NULL" : pTechno.Ref.Type.Ref.Base.Base.ID);
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && ext.Type.VirtualUnit)
                {
                    // Logger.Log("sub_6DA380 calling 3, Skip the Unit {0}", pTechno.Ref.Type.Ref.Base.Base.ID);
                    return 0x6DA491;
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