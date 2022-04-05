using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Utilities
{
    [DebuggerDisplay("[{handle.fixedPointer.Value}]={Pointer.Value}")]
    [Serializable]
    public struct SwizzleablePointer<T> : ISerializable
    {
        private PointerHandle<T> handle;
        public ref Pointer<T> Pointer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref handle.Pointer;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SwizzleablePointer(Pointer<T> ptr)
        {
            handle = new PointerHandle<T>(ptr);
        }

        [SecurityPermission(SecurityAction.LinkDemand,
            Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (handle != null)
            {
                info.AddValue("Pointer", (int)Pointer);
            }
        }
        private SwizzleablePointer(SerializationInfo info, StreamingContext context)
        {
            try
            {
                handle = new PointerHandle<T>((IntPtr)info.GetInt32("Pointer"));
                SwizzleManagerClass.Instance.Swizzle(ref Pointer);
            }
            catch (SerializationException)
            {
                handle = null;
            }
        }

        public ref T Ref
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Pointer.Ref;
        }
        public T Data
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Pointer.Ref;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Pointer.Ref = value;
        }
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Pointer[index];
        }

        public bool IsNull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Pointer == Pointer<T>.Zero;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Pointer<T>(SwizzleablePointer<T> obj) => obj.Pointer;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator SwizzleablePointer<T>(Pointer<T> ptr) => new SwizzleablePointer<T>(ptr);
    }
}
