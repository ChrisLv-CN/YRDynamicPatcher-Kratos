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
    [StructLayout(LayoutKind.Explicit, Size = 20)]
    [Serializable]
    public struct SlaveControl
    {
        [FieldOffset(0)] public Pointer<InfantryClass> Slave;

        [FieldOffset(4)] public SlaveStatus State;

        [FieldOffset(8)] public TimerStruct RespawnTimer;

    }

    public enum SlaveStatus
    {
        Unknown = 0,
        ScanningForTiberium = 1,
        MovingToTiberium = 2,
        Harvesting = 3,
        BringingItBack = 4,
        Respawning = 5,
        Dead = 6
    }
}
