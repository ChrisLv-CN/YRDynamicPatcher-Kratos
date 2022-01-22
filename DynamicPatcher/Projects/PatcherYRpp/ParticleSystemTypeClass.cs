using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 784)]
    [Serializable]
    public struct ParticleSystemTypeClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0xA83D68);

        public static YRPP.GLOBAL_DVC_ARRAY<ParticleSystemTypeClass> ABSTRACTTYPE_ARRAY = new YRPP.GLOBAL_DVC_ARRAY<ParticleSystemTypeClass>(ArrayPointer);

        [FieldOffset(0)] public ObjectTypeClass Base;

    }
}
