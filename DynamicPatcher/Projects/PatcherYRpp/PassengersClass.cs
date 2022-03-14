using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct PassengersClass
    {
        public unsafe void AddPassenger(Pointer<FootClass> pPassenger)
        {
            var func = (delegate* unmanaged[Thiscall]<ref PassengersClass, IntPtr, void>)0x4733A0;
            func(ref this, pPassenger);
        }

        public unsafe int GetTotalSize()
        {
            var func = (delegate* unmanaged[Thiscall]<ref PassengersClass, int>)0x473460;
            return func(ref this);
        }

        public unsafe int IndexOf(Pointer<FootClass> candidate)
        {
            var func = (delegate* unmanaged[Thiscall]<ref PassengersClass, IntPtr, int>)0x473500;
            return func(ref this, candidate);
        }

        public unsafe Pointer<FootClass> RemoveFirstPassenger()
        {
            var func = (delegate* unmanaged[Thiscall]<ref PassengersClass, IntPtr>)0x473430;
            return func(ref this);
        }

        [FieldOffset(0)] public int NumPassengers;

        [FieldOffset(4)] public IntPtr firstPassenger;
        public Pointer<FootClass> FirstPassenger { get => firstPassenger; set => firstPassenger = value; }

    }
}
