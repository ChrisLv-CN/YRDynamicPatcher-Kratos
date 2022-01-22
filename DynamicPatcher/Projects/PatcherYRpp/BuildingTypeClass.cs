using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 6040)]
    [Serializable]
    public struct BuildingTypeClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0xA83C68);

        public static YRPP.GLOBAL_DVC_ARRAY<BuildingTypeClass> ABSTRACTTYPE_ARRAY = new YRPP.GLOBAL_DVC_ARRAY<BuildingTypeClass>(ArrayPointer);

        [FieldOffset(0)] public TechnoTypeClass Base;

        [FieldOffset(3828)] public int Height;
        [FieldOffset(3832)] public int OccupyHeight;
        [FieldOffset(5408)] public int NormalZAdjust;
        [FieldOffset(5490)] public Bool Capturable;
        [FieldOffset(5495)] public Bool CanC4;
        [FieldOffset(5825)] public Bool Hospital;
        [FieldOffset(5835)] public Bool Helipad;
        [FieldOffset(5889)] public Bool InvisibleInGame;
        [FieldOffset(6016)] public int NumberOfDocks;
    }
}
