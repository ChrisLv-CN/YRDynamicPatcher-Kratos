using DynamicPatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extension.Decorators;

namespace DecoratorHooks
{
    public class BulletDecorativeHooks
    {
        [Hook(HookType.AresHook, Address = 0x4666F7, Size = 6)]
        static public unsafe UInt32 OnUpdate(REGISTERS* R)
        {
            try
            {
                return BulletDecorative.OnUpdate(R);
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return (uint)0;
            }
        }
    }
}
