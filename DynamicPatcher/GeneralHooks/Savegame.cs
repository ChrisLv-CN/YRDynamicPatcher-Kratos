using DynamicPatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHooks
{
    public class Savegame
    {
        [Hook(HookType.AresHook, Address = 0x67D300, Size = 5)]
        public static unsafe UInt32 SaveGame_Start(REGISTERS* R)
        {

            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x67E42E, Size = 0xD)]
        public static unsafe UInt32 SaveGame(REGISTERS* R)
        {

            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x67E730, Size = 5)]
        public static unsafe UInt32 LoadGame_Start(REGISTERS* R)
        {

            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x67F7C8, Size = 5)]
        public static unsafe UInt32 LoadGame_End(REGISTERS* R)
        {

            return 0;
        }
    }
}
