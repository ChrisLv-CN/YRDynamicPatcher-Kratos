using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PatcherYRpp.FileFormats;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 1728)]
    public struct FootClass
    {

        public unsafe Pointer<CoordStruct> GetCoords_Position0(ref CoordStruct coord)
        {
            var func = (delegate* unmanaged[Thiscall]<ref FootClass, IntPtr, IntPtr>)this.GetVirtualFunctionPointer(319);
            return func(ref this, Pointer<CoordStruct>.AsPointer(ref coord));
        }

        public unsafe bool MoveTo(CoordStruct where)
        {
            var func = (delegate* unmanaged[Thiscall]<ref FootClass, IntPtr, bool>)this.GetVirtualFunctionPointer(319);
            return func(ref this, Pointer<CoordStruct>.AsPointer(ref where));
        }

        public unsafe bool StopDrive()
        {
            var func = (delegate* unmanaged[Thiscall]<ref FootClass, bool>)this.GetVirtualFunctionPointer(320);
            return func(ref this);
        }

        public unsafe bool ChronoWarpTo(CoordStruct where)
        {
            var func = (delegate* unmanaged[Thiscall]<ref FootClass, IntPtr, bool>)this.GetVirtualFunctionPointer(322);
            return func(ref this, Pointer<CoordStruct>.AsPointer(ref where));
        }

        public unsafe void Draw_A_SHP(Pointer<SHPStruct> SHP, int idxFacing, Pointer<Point2D> coords, Pointer<RectangleStruct> rectangle,
            int dwUnk5, int dwUnk6, int dwUnk7, ZGradient ZGradient,
            int dwUnk9, int extraLight, int dwUnk11, int dwUnk12,
            int dwUnk13, int dwUnk14, int dwUnk15, int dwUnk16)
        {
            var func = (delegate* unmanaged[Thiscall]<ref FootClass, IntPtr, int, IntPtr, IntPtr,
                int, int, int, ZGradient,
                int, int, int, int,
                int, int, int, int,
                void>)this.GetVirtualFunctionPointer(323);
            func(ref this, SHP, idxFacing, coords, rectangle,
                dwUnk5, dwUnk6, dwUnk7, ZGradient,
                dwUnk9, extraLight, dwUnk11, dwUnk12,
                dwUnk13, dwUnk14, dwUnk15, dwUnk16);
        }

        public unsafe void Draw_A_VXL(Pointer<VoxelStruct> VXL, int HVAFrameIndex, int flags, Pointer<SomeVoxelCache> cache, Pointer<RectangleStruct> rectangle,
            Pointer<Point2D> centerPoint, Pointer<Matrix3DStruct> matrix, int bright, int tint, int dwUnk10)
        {
            var func = (delegate* unmanaged[Thiscall]<ref FootClass, IntPtr, int, int, IntPtr, IntPtr,
                IntPtr, IntPtr, int, int, int, void>)this.GetVirtualFunctionPointer(324);
            func(ref this, VXL, HVAFrameIndex, flags, cache, rectangle, centerPoint, matrix, bright, tint, dwUnk10);
        }

        public unsafe void Panic()
        {
            var func = (delegate* unmanaged[Thiscall]<ref FootClass, void>)this.GetVirtualFunctionPointer(326);
            func(ref this);
        }

        public unsafe void UnPanic()
        {
            var func = (delegate* unmanaged[Thiscall]<ref FootClass, void>)this.GetVirtualFunctionPointer(327);
            func(ref this);
        }

        public unsafe int GetCurrentSpeed()
        {
            var func = (delegate* unmanaged[Thiscall]<ref FootClass, int>)this.GetVirtualFunctionPointer(334);
            return func(ref this);
        }

        public unsafe bool StopMoving()
        {
            var func = (delegate* unmanaged[Thiscall]<ref FootClass, bool>)this.GetVirtualFunctionPointer(338);
            return func(ref this);
        }

        public unsafe int Inf_PlayAnim(SequenceAnimType sequenceAnimType)
        {
            var func = (delegate* unmanaged[Thiscall]<ref FootClass, SequenceAnimType, int, int, int>)this.GetVirtualFunctionPointer(342);
            return func(ref this, sequenceAnimType, 0, 0);
        }

        public unsafe void AbortMotion()
        {
            var func = (delegate* unmanaged[Thiscall]<ref FootClass, void>)0x4DF0D0;
            func(ref this);
        }

        public unsafe bool Jumpjet_LocationClear()
        {
            var func = (delegate* unmanaged[Thiscall]<ref FootClass, Bool>)0x4135A0;
            return func(ref this);
        }

        public unsafe void Jumpjet_OccupyCell(CellStruct cell)
        {
            var func = (delegate* unmanaged[Thiscall]<ref FootClass, CellStruct, void>)0x4E00B0;
            func(ref this, cell);
        }

        [FieldOffset(0)] public TechnoClass Base;

        [FieldOffset(1368)] public CellStruct CurrentMapCoords;
        [FieldOffset(1372)] public CellStruct LastMapCoords;
        [FieldOffset(1376)] public CellStruct LastJumpjetMapCoords;
        [FieldOffset(1384)] public CellStruct CurrentJumpjetMapCoords;
        [FieldOffset(1408)] public double SpeedMultiplier;
        [FieldOffset(1444)] public Pointer<AbstractClass> Destination;
        [FieldOffset(1448)] public Pointer<AbstractClass> LastDestination;
        [FieldOffset(1540)] public int PathDirections;
        [FieldOffset(1600)] public TimerStruct PathDelayTimer;
        [FieldOffset(1616)] public TimerStruct BaseAttackTimer;
        [FieldOffset(1628)] public TimerStruct SightTimer;
        [FieldOffset(1640)] public TimerStruct BlockagePathTimer;
        [FieldOffset(1652)] public COMPtr<ILocomotion> locomotor;
        public ILocomotion Locomotor { get => locomotor.Object; set => locomotor.Object = value; }
        [FieldOffset(1711)] public Bool FacingChanging;
        [FieldOffset(1718)] public Bool FrozenStill;
    }
}
