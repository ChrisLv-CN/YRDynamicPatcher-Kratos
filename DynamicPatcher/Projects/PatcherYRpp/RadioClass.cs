using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 240)]
    public struct RadioClass
    {

        public unsafe RadioCommand SendToFirstLink(RadioCommand command)
        {
            var func = (delegate* unmanaged[Thiscall]<ref RadioClass, RadioCommand, RadioCommand>)this.GetVirtualFunctionPointer(157);
            return func(ref this, command);
        }
        public unsafe RadioCommand SendCommand(RadioCommand command, Pointer<TechnoClass> pRecipient)
        {
            var func = (delegate* unmanaged[Thiscall]<ref RadioClass, RadioCommand, IntPtr, RadioCommand>)this.GetVirtualFunctionPointer(158);
            return func(ref this, command, pRecipient);
        }
        public unsafe RadioCommand SendCommandWithData(RadioCommand command, ref Pointer<AbstractClass> pInOut, Pointer<TechnoClass> pRecipient)
        {
            var func = (delegate* unmanaged[Thiscall]<ref RadioClass, RadioCommand, IntPtr, IntPtr, RadioCommand>)this.GetVirtualFunctionPointer(159);
            return func(ref this, command, pInOut.GetThisPointer(), pRecipient);
        }
        public unsafe void SendToEachLink(RadioCommand command)
        {
            var func = (delegate* unmanaged[Thiscall]<ref RadioClass, RadioCommand, void>)this.GetVirtualFunctionPointer(160);
            func(ref this, command);
        }


        [FieldOffset(0)] public MissionClass Base;
        [FieldOffset(0)] public ObjectClass BaseObject;
        [FieldOffset(0)] public AbstractClass BaseAbstract;


    }
}
