using DynamicPatcher;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extension.Ext;

namespace GeneralHooks
{
    public class General
    {
        [Hook(HookType.AresHook, Address = 0x52BA60, Size = 5)]
        public static unsafe UInt32 YR_Boot(REGISTERS* R)
        {

            return 0;
        }

        // in progress: Initializing Tactical display
        [Hook(HookType.AresHook, Address = 0x6875F3, Size = 6)]
        public static unsafe UInt32 Scenario_Start1(REGISTERS* R)
        {
            // ensure network synchronization
            MathEx.SetRandomSeed(0);
            Kratos.NewGame();
            PrintTextManager.Clear();

            //Logger.Log("set random seed!");

            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x685659, Size = 0xA)]
        public static unsafe UInt32 Scenario_ClearClasses(REGISTERS* R)
        {

            return 0;
        }


        [Hook(HookType.AresHook, Address = 0x7CD8EF, Size = 9)]
        public static unsafe UInt32 ExeTerminate(REGISTERS* R)
        {

            return 0;
        }
    }
}
