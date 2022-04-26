using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct DamageGroup
    {
        [FieldOffset(0)] public Pointer<TechnoClass> Target;

        [FieldOffset(4)] public double Distance;

    }
}
