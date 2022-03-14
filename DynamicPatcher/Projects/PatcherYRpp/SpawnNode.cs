using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PatcherYRpp.FileFormats;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 24)]
    public struct SpawnNode
    {
        [FieldOffset(0)] public Pointer<TechnoClass> Unit;

        [FieldOffset(4)] public int Status;

        [FieldOffset(20)] public Bool IsSpawnMissile;

    }
}
