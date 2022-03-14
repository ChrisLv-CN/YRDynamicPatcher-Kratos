using Microsoft.VisualStudio.OLE.Interop;
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
    public struct LocomotionClass
    {

        private static IntPtr _pGUIDs = new IntPtr(0x7E9A30);
        private static Pointer<Guid> pGUIDs => _pGUIDs.Convert<Guid>();
        public static Guid Drive { get => pGUIDs[0]; set => pGUIDs[0] = value; }/* 0x7E9A30;*/
        public static Guid Hover { get => pGUIDs[1]; set => pGUIDs[1] = value; }/* 0x7E9A40;*/
        public static Guid Tunnel { get => pGUIDs[2]; set => pGUIDs[2] = value; }/* 0x7E9A50;*/
        public static Guid Walk { get => pGUIDs[3]; set => pGUIDs[3] = value; }/* 0x7E9A60;*/
        public static Guid Droppod { get => pGUIDs[4]; set => pGUIDs[4] = value; }/* 0x7E9A70;*/
        public static Guid Fly { get => pGUIDs[5]; set => pGUIDs[5] = value; }/* 0x7E9A80;*/
        public static Guid Teleport { get => pGUIDs[6]; set => pGUIDs[6] = value; }/* 0x7E9A90;*/
        public static Guid Mech { get => pGUIDs[7]; set => pGUIDs[7] = value; }/* 0x7E9AA0;*/
        public static Guid Ship { get => pGUIDs[8]; set => pGUIDs[8] = value; }/* 0x7E9AB0;*/
        public static Guid Jumpjet { get => pGUIDs[9]; set => pGUIDs[9] = value; }/* 0x7E9AC0;*/
        public static Guid Rocket { get => pGUIDs[10]; set => pGUIDs[10] = value; } /*0x7E9AD0;*/

        public Guid GUID => GetClassID();

        public static void ChangeLocomotorTo(Pointer<FootClass> pFoot, Guid clsid)
        {
            // remember the current one
            ILocomotion Original = pFoot.Ref.Locomotor;

            // create a new locomotor and link it
            ILocomotion NewLoco = COMHelpers.CreateInstance<ILocomotion>(clsid);
            NewLoco.Link_To_Object(pFoot);

            // get piggy interface and piggy original
            if (NewLoco is IPiggyback Piggy)
            {
                Piggy.Begin_Piggyback(Original);
            }

            // replace the current locomotor
            pFoot.Ref.Locomotor = NewLoco;
        }

        public static void ChangeLocomotorTo<TLocomotion>(Pointer<FootClass> pFoot)
        {
            ChangeLocomotorTo(pFoot, typeof(TLocomotion).GUID);
        }

        public Guid GetClassID()
        {
            IPersistStream.GetClassID(out Guid guid);
            return guid;
        }

        /**
        public unsafe Bool Is_Moving()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, Bool>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 4);
            return func(ref this);
        }

        public unsafe CoordStruct Destination(CoordStruct pCoord)
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, IntPtr, IntPtr>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 5);
            Pointer<CoordStruct> res = func(ref this, Pointer<CoordStruct>.AsPointer(ref pCoord));
            return res.Ref;
        }

        public unsafe Pointer<Matrix3DStruct> Draw_Matrix(int key)
        {
            Matrix3DStruct matrix3D = default;
            Pointer<Matrix3DStruct> pMatrix = Pointer<Matrix3DStruct>.AsPointer(ref matrix3D);
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, IntPtr, int, IntPtr>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 9);
            func(ref this, pMatrix, key);
            return pMatrix;
        }

        public unsafe Pointer<Matrix3DStruct> Shadow_Matrix(int key)
        {
            Matrix3DStruct matrix3D = default;
            Pointer<Matrix3DStruct> pMatrix = Pointer<Matrix3DStruct>.AsPointer(ref matrix3D);
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, IntPtr, int, IntPtr>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 10);
            func(ref this, pMatrix, key);
            return pMatrix;
        }

        public unsafe Pointer<Point2D> Draw_Point()
        {
            Point2D point2D = default;
            Pointer<Point2D> pPoint2D = Pointer<Point2D>.AsPointer(ref point2D);
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, IntPtr, IntPtr>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 11);
            func(ref this, pPoint2D);
            return pPoint2D;
        }

        public unsafe Pointer<Point2D> Shadow_Point()
        {
            Point2D point2D = default;
            Pointer<Point2D> pPoint2D = Pointer<Point2D>.AsPointer(ref point2D);
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, IntPtr, IntPtr>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 12);
            func(ref this, pPoint2D);
            return pPoint2D;
        }

        public unsafe VisualType Visual_Character()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, Bool, VisualType>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 13);
            return func(ref this, false);
        }

        public unsafe int Z_Adjust()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, int>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 14);
            return func(ref this);
        }

        public unsafe int Z_Gradient()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, int>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 15);
            return func(ref this);
        }

        public unsafe bool Process()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, Bool>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 16);
            return func(ref this);
        }

        public unsafe void Move_To(CoordStruct to)
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, CoordStruct, void>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 17);
            func(ref this, to);
        }

        public unsafe void Stop_Moving()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, void>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 18);
            func(ref this);
        }

        public unsafe void Do_Turn(DirStruct dir)
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, DirStruct, void>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 19);
            func(ref this, dir);
        }

        public unsafe bool Push(DirStruct dir)
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, DirStruct, Bool>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 26);
            return func(ref this, dir);
        }

        public unsafe bool Shove(DirStruct dir)
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, DirStruct, Bool>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 27);
            return func(ref this, dir);
        }

        public unsafe void ForceTrack(int track, CoordStruct coordStruct)
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, int, CoordStruct, void>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 28);
            func(ref this, track, coordStruct);
        }

        public unsafe Layer In_Which_Layer()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, Layer>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 29);
            return func(ref this);
        }

        public unsafe void Stop_Movement_Animation()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, void>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 43);
            func(ref this);
        }

        public unsafe void Lock()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, void>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 44);
            func(ref this);
        }

        public unsafe void UnLock()
        {
            var func = (delegate* unmanaged[Stdcall]<ref LocomotionClass, void>)
                this.GetVirtualFunctionPointer(Pointer<LocomotionClass>.AsPointer(ref this), 45);
            func(ref this);
        }
        */

        [FieldOffset(0)] public int Vfptr_IPersistStream;
        public IPersistStream IPersistStream => Marshal.GetObjectForIUnknown(this.GetThisPointer()) as IPersistStream;
        [FieldOffset(4)] public int Vfptr_ILocomotion;
        public ILocomotion ILocomotion => Marshal.GetObjectForIUnknown(this.GetThisPointer()) as ILocomotion;
        [FieldOffset(8)] public Pointer<FootClass> Owner;
        [FieldOffset(12)] public Pointer<FootClass> LinkedTo;
        [FieldOffset(16)] public Bool Powered;
        [FieldOffset(17)] public Bool Dirty;
        [FieldOffset(20)] public int RefCount;

    }

    public static class LocomotionHelpers
    {
        public static Pointer<LocomotionClass> ToLocomotionClass(this ILocomotion locomotion)
        {
            Pointer<IntPtr> ptr = Marshal.GetIUnknownForObject(locomotion);
            Marshal.Release(ptr);
            ptr -= 1;

            return ptr.Convert<LocomotionClass>();
        }

        public static Pointer<TLocomotionClass> ToLocomotionClass<TLocomotionClass>(this ILocomotion locomotion)
        {
            return locomotion.ToLocomotionClass().Convert<TLocomotionClass>();
        }

        public static void Load(this IPersistStream persistStream, System.Runtime.InteropServices.ComTypes.IStream stream)
        {
            persistStream.Load(stream as IStream);
        }
        public static void Save(this IPersistStream persistStream, System.Runtime.InteropServices.ComTypes.IStream stream, int fClearDirty)
        {
            persistStream.Save(stream as IStream, fClearDirty);
        }
        public static int SaveSize(this IPersistStream persistStream)
        {
            var tmp = new ULARGE_INTEGER[1];
            persistStream.GetSizeMax(tmp);
            return (int)tmp[0].QuadPart;
        }
    }
}
