using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 212)]
    public struct MissionClass
    {
        public unsafe bool QueueMission(Mission mission, bool start_mission)
        {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, Mission, Bool, Bool>)this.GetVirtualFunctionPointer(122);
            return func(ref this, mission, start_mission);
        }
        public unsafe bool NextMission()
        {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, Bool>)this.GetVirtualFunctionPointer(123);
            return func(ref this);
        }
        public unsafe bool ForceMission(Mission mission)
        {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, Mission, Bool>)this.GetVirtualFunctionPointer(124);
            return func(ref this, mission);
        }
        public unsafe void StartMission(Mission mission, Pointer<AbstractClass> pTarget, Pointer<AbstractClass> pDestination = default)
        {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, Mission, IntPtr, IntPtr, void>)this.GetVirtualFunctionPointer(125);
            func(ref this, mission, pTarget, pDestination);
        }

        public unsafe bool Mission_Revert()
        {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, Bool>)this.GetVirtualFunctionPointer(126);
            return func(ref this);
        }
        public unsafe bool HasForcedMission()
        {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, Bool>)this.GetVirtualFunctionPointer(127);
            return func(ref this);
        }
        public unsafe bool CanDoNextMission() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, Bool>)this.GetVirtualFunctionPointer(128);
            return func(ref this);
        }
        public unsafe int Mission_Sleep() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(129);
            return func(ref this);
        }
        public unsafe int Mission_Harmless() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(130);
            return func(ref this);
        }
        public unsafe int Mission_Ambush() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(131);
            return func(ref this);
        }
        public unsafe int Mission_Attack() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(132);
            return func(ref this);
        }
        public unsafe int Mission_Capture() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(133);
            return func(ref this);
        }
        public unsafe int Mission_Eaten() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(134);
            return func(ref this);
        }
        public unsafe int Mission_Guard() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(135);
            return func(ref this);
        }
        public unsafe int Mission_AreaGuard() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(136);
            return func(ref this);
        }
        public unsafe int Mission_Harvest() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(137);
            return func(ref this);
        }
        public unsafe int Mission_Hunt() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(138);
            return func(ref this);
        }
        public unsafe int Mission_Move() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(139);
            return func(ref this);
        }
        public unsafe int Mission_Retreat() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(140);
            return func(ref this);
        }
        public unsafe int Mission_Return() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(141);
            return func(ref this);
        }
        public unsafe int Mission_Stop() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(142);
            return func(ref this);
        }
        public unsafe int Mission_Unload() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(143);
            return func(ref this);
        }
        public unsafe int Mission_Enter() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(144);
            return func(ref this);
        }
        public unsafe int Mission_Construction() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(145);
            return func(ref this);
        }
        public unsafe int Mission_Selling() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(146);
            return func(ref this);
        }
        public unsafe int Mission_Repair() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(147);
            return func(ref this);
        }
        public unsafe int Mission_Missile() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(148);
            return func(ref this);
        }
        public unsafe int Mission_Open() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(149);
            return func(ref this);
        }
        public unsafe int Mission_Rescue() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(150);
            return func(ref this);
        }
        public unsafe int Mission_Patrol() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(151);
            return func(ref this);
        }
        public unsafe int Mission_ParaDropApproach() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(152);
            return func(ref this);
        }
        public unsafe int Mission_ParaDropOverfly() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(153);
            return func(ref this);
        }
        public unsafe int Mission_Wait() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(154);
            return func(ref this);
        }
        public unsafe int Mission_SpyPlaneApproach() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(155);
            return func(ref this);
        }
        public unsafe int Mission_SpyPlaneOverfly() {
            var func = (delegate* unmanaged[Thiscall]<ref MissionClass, int>)this.GetVirtualFunctionPointer(156);
            return func(ref this);
        }

        [FieldOffset(0)] public ObjectClass Base;
        [FieldOffset(0)] public AbstractClass BaseAbstract;

        [FieldOffset(172)] public Mission CurrentMission;
        [FieldOffset(176)] public Mission unknown_mission_B0;
        [FieldOffset(180)] public Mission QueuedMission;

        [FieldOffset(188)] public int MissionStatus;
        [FieldOffset(192)] public int CurrentMissionStartTime;    //in frames

        [FieldOffset(200)] public TimerStruct UpdateTimer;
    }
}



