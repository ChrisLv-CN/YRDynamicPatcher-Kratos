using DynamicPatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miscellaneous
{
#if OOXX
    [RunClassConstructorFirst]
    public class Preprocess
    {
        static Preprocess()
        {
            // add 500MB pressure
            GC.AddMemoryPressure(500 * 1024 * 1024);
            Logger.Log("Add 500MB pressure to GC.");
        }
    }
#endif
}
