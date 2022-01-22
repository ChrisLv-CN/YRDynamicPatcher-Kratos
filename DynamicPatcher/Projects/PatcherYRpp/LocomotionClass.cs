using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{


    [StructLayout(LayoutKind.Explicit, Size = 24)]
    [Serializable]
    public struct LocomotionClass
    {

        // public unsafe long GetClassID(ref Guid clsID)
        // {

        //     var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, Guid, long>)
        //         Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 3);
        //     return func(ref this, clsID);
        // }

        public unsafe Bool Is_Moving()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, Bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 4);
            return func(ref this);
        }

        public unsafe CoordStruct Destination(CoordStruct pCoord)
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, IntPtr, IntPtr>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 5);
            Pointer<CoordStruct> res = func(ref this, Pointer<CoordStruct>.AsPointer(ref pCoord));
            return res.Ref;
        }

        public unsafe Pointer<Matrix3DStruct> Draw_Matrix(int key)
        {
            Matrix3DStruct matrix3D = default;
            Pointer<Matrix3DStruct> pMatrix = Pointer<Matrix3DStruct>.AsPointer(ref matrix3D);
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, IntPtr, int, IntPtr>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 9);
            func(ref this, pMatrix, key);
            return pMatrix;
        }

        public unsafe Pointer<Matrix3DStruct> Shadow_Matrix(int key)
        {
            Matrix3DStruct matrix3D = default;
            Pointer<Matrix3DStruct> pMatrix = Pointer<Matrix3DStruct>.AsPointer(ref matrix3D);
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, IntPtr, int, IntPtr>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 10);
            func(ref this, pMatrix, key);
            return pMatrix;
        }

        public unsafe Pointer<Point2D> Draw_Point()
        {
            Point2D point2D = default;
            Pointer<Point2D> pPoint2D = Pointer<Point2D>.AsPointer(ref point2D);
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, IntPtr, IntPtr>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 11);
            func(ref this, pPoint2D);
            return pPoint2D;
        }

        public unsafe Pointer<Point2D> Shadow_Point()
        {
            Point2D point2D = default;
            Pointer<Point2D> pPoint2D = Pointer<Point2D>.AsPointer(ref point2D);
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, IntPtr, IntPtr>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 12);
            func(ref this, pPoint2D);
            return pPoint2D;
        }

        public unsafe VisualType Visual_Character()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, Bool, VisualType>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 13);
            return func(ref this, false);
        }

        public unsafe int Z_Adjust()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, int>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 14);
            return func(ref this);
        }

        public unsafe int Z_Gradient()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, int>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 15);
            return func(ref this);
        }

        public unsafe bool Process()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, Bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 16);
            return func(ref this);
        }

        public unsafe void Move_To(CoordStruct to)
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, CoordStruct, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 17);
            func(ref this, to);
        }

        public unsafe void Stop_Moving()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 18);
            func(ref this);
        }

        public unsafe void Do_Turn(DirStruct dir)
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, DirStruct, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 19);
            func(ref this, dir);
        }

        public unsafe bool Push(DirStruct dir)
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, DirStruct, Bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 26);
            return func(ref this, dir);
        }

        public unsafe bool Shove(DirStruct dir)
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, DirStruct, Bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 27);
            return func(ref this, dir);
        }

        public unsafe void ForceTrack(int track, CoordStruct coordStruct)
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, int, CoordStruct, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 28);
            func(ref this, track, coordStruct);
        }

        public unsafe Layer In_Which_Layer()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, Layer>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 29);
            return func(ref this);
        }

        public unsafe void Stop_Movement_Animation()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 43);
            func(ref this);
        }

        public unsafe void Lock()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 44);
            func(ref this);
        }

        public unsafe void UnLock()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 45);
            func(ref this);
        }

        [FieldOffset(12)] public IntPtr linkedTo;
        public Pointer<FootClass> LinkedTo { get => linkedTo; set => linkedTo = value; }

    }

    [StructLayout(LayoutKind.Explicit, Size = 1)]
    [Serializable]
    public struct CLSIDs
    {

    }
}
