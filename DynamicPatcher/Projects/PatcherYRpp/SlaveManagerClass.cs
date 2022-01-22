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
    [StructLayout(LayoutKind.Explicit, Size = 100)]
    [Serializable]
    public struct SlaveManagerClass
    {

        public unsafe void LostSlave(Pointer<InfantryClass> pSlave)
        {
            var func = (delegate* unmanaged[Thiscall]<ref SlaveManagerClass, IntPtr, void>)0x6B0A20;
            func(ref this, pSlave);
        }

        [FieldOffset(36)] public IntPtr owner;
        public Pointer<TechnoClass> Owner { get => owner; set => owner = value; }

        [FieldOffset(40)] public IntPtr slaveType;
        public Pointer<InfantryTypeClass> SlaveType { get => slaveType; set => slaveType = value; }

        [FieldOffset(44)] public int SpawnCount;

        [FieldOffset(48)] public int RegenRate;

        [FieldOffset(52)] public int ReloadRate;

        [FieldOffset(56)] public byte slaveNodes;
        public ref DynamicVectorClass<Pointer<SlaveControl>> SlaveNodes => ref Pointer<byte>.AsPointer(ref slaveNodes).Convert<DynamicVectorClass<Pointer<SlaveControl>>>().Ref;

        [FieldOffset(80)] public TimerStruct RespawnTimer;

        [FieldOffset(92)] public SlaveManagerState State;

        [FieldOffset(96)] public int LastScanFrame;


        public enum SlaveManagerState
        {
            Ready = 0,
            Scanning = 1,
            Travelling = 2,
            Deploying = 3,
            Working = 4,
            ScanningAgain = 5,
            PackingUp = 6
        }

    }
}
