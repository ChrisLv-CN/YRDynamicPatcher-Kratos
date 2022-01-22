using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PatcherYRpp.FileFormats;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 212)]
    [Serializable]
    public struct MissionClass
    {

        public unsafe Bool QueueMission(Mission mission, bool startMission)
        {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, Mission, Bool, Bool>)Helpers.GetVirtualFunctionPointer(Pointer<MissionClass>.AsPointer(ref this), 122);
            return func(ref this, mission, startMission);
        }

        public unsafe Bool NextMission()
        {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, Bool>)Helpers.GetVirtualFunctionPointer(Pointer<MissionClass>.AsPointer(ref this), 123);
            return func(ref this);
        }

        public unsafe Bool ForceMission(Mission mission)
        {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, Mission, Bool>)Helpers.GetVirtualFunctionPointer(Pointer<MissionClass>.AsPointer(ref this), 124);
            return func(ref this, mission);
        }

        public unsafe Bool Mission_Revert()
        {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, Bool>)Helpers.GetVirtualFunctionPointer(Pointer<MissionClass>.AsPointer(ref this), 126);
            return func(ref this);
        }

        public unsafe Bool Mission_Guard()
        {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, Bool>)Helpers.GetVirtualFunctionPointer(Pointer<MissionClass>.AsPointer(ref this), 135);
            return func(ref this);
        }

        public unsafe Bool Mission_AreaGuard()
        {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, Bool>)Helpers.GetVirtualFunctionPointer(Pointer<MissionClass>.AsPointer(ref this), 136);
            return func(ref this);
        }

        public unsafe Bool Mission_Hunt()
        {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, Bool>)Helpers.GetVirtualFunctionPointer(Pointer<MissionClass>.AsPointer(ref this), 138);
            return func(ref this);
        }

        [FieldOffset(0)]
        public AbstractClass Base;

        [FieldOffset(172)]
        public Mission CurrentMission;

        [FieldOffset(176)]
        public Mission unknown_mission_B0;

        [FieldOffset(180)]
        public Mission QueuedMission;

        [FieldOffset(188)]
        public int MissionStatus;

    }
}
