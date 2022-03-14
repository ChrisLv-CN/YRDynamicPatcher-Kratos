using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 224)]
    public struct TerrainClass
    {
        [FieldOffset(0)] public ObjectClass Base;

        [FieldOffset(172)] public ProgressTimer Animation;
        [FieldOffset(200)] public Pointer<TerrainTypeClass> Type;
        [FieldOffset(204)] public Bool IsBurning; // this terrain object has been ignited
        [FieldOffset(205)] public Bool TimeToDie; // finish the animation and uninit
    }
}
