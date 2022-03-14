using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using HRESULT = System.UInt32;
namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SwizzlePointerClass
    {
        uint unknown_0; //no idea, only found it being zero
        void* pAnything; //the pointer, to literally any object type

        public static bool operator ==(SwizzlePointerClass a, SwizzlePointerClass b)
        {
            return

                (a.unknown_0 == b.unknown_0) &&
                (a.pAnything == b.pAnything);
        }
        public static bool operator !=(SwizzlePointerClass a, SwizzlePointerClass b) => !(a == b);
        public override bool Equals(object obj) => this == (SwizzlePointerClass)obj;
        public override int GetHashCode() => base.GetHashCode();
    };


    [StructLayout(LayoutKind.Explicit, Size = 52)]
    public struct SwizzleManagerClass
    {
        private static IntPtr instance = new IntPtr(0xB0C110);
        static public ref SwizzleManagerClass Instance { get => ref instance.Convert<SwizzleManagerClass>().Ref; }
        public unsafe HRESULT Reset()
        {
            var func = (delegate* unmanaged[Stdcall]<ref SwizzleManagerClass, HRESULT>)this.GetVirtualFunctionPointer(3);
            return func(ref this);
        }
        public unsafe HRESULT Swizzle(Pointer<IntPtr> pointer)
        {
            var func = (delegate* unmanaged[Stdcall]<ref SwizzleManagerClass, IntPtr, HRESULT>)this.GetVirtualFunctionPointer(4);
            return func(ref this, pointer);
        }

        public unsafe HRESULT Fetch_Swizzle_ID(IntPtr pointer, Pointer<int> id)
        {
            var func = (delegate* unmanaged[Stdcall]<ref SwizzleManagerClass, IntPtr, IntPtr, HRESULT>)this.GetVirtualFunctionPointer(5);
            return func(ref this, pointer, id);
        }

        public unsafe HRESULT Here_I_Am(int id, IntPtr pointer)
        {
            var func = (delegate* unmanaged[Stdcall]<ref SwizzleManagerClass, int, IntPtr, HRESULT>)this.GetVirtualFunctionPointer(6);
            return func(ref this, id, pointer);
        }

        public unsafe HRESULT Save_Interface(IStream stream, IntPtr pointer)
        {
            var func = (delegate* unmanaged[Stdcall]<ref SwizzleManagerClass, IStream, IntPtr, HRESULT>)this.GetVirtualFunctionPointer(7);
            return func(ref this, stream, pointer);
        }

        public unsafe HRESULT Load_Interface(IStream stream, Guid riid, Pointer<IntPtr> pointer)
        {
            var func = (delegate* unmanaged[Stdcall]<ref SwizzleManagerClass, IStream, IntPtr, IntPtr, HRESULT>)this.GetVirtualFunctionPointer(8);
            return func(ref this, stream, Pointer<Guid>.AsPointer(ref riid), pointer);
        }
        public unsafe HRESULT Get_Save_Size(Pointer<int> psize)
        {
            var func = (delegate* unmanaged[Stdcall]<ref SwizzleManagerClass, IntPtr, HRESULT>)this.GetVirtualFunctionPointer(9);
            return func(ref this, psize);
        }


        [FieldOffset(4)]
        public DynamicVectorClass<SwizzlePointerClass> Swizzles_Old;
        [FieldOffset(28)]
        public DynamicVectorClass<SwizzlePointerClass> Swizzles_New;
    }

    public static class SwizzleManagerHelpers
    {
        public static void Swizzle<T>(this SwizzleManagerClass @this, ref T obj)
        {
            SwizzleManagerClass.Instance.Swizzle(Pointer<T>.AsPointer(ref obj).Convert<IntPtr>());
        }
        public static void Here_I_Am<T>(this SwizzleManagerClass @this, Pointer<T> oldPtr, ref T obj)
        {
            SwizzleManagerClass.Instance.Here_I_Am((int)oldPtr, Pointer<T>.AsPointer(ref obj).Convert<IntPtr>());
        }
    }
}
