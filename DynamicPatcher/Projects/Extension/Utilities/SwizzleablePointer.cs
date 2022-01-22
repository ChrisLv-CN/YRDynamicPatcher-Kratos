using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Utilities
{
    [Serializable]
    public struct SwizzleablePointer<T> : ISerializable
    {
        private PointerHandle<T> handle;
        public ref Pointer<T> Pointer { get => ref handle.Pointer; }
        public SwizzleablePointer(Pointer<T> ptr)
        {
            handle = new PointerHandle<T>(ptr);
        }

        [SecurityPermission(SecurityAction.LinkDemand,
            Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Pointer", (int)Pointer);
        }
        private SwizzleablePointer(SerializationInfo info, StreamingContext context)
        {
            handle = new PointerHandle<T>((IntPtr)info.GetInt32("Pointer"));
            SwizzleManagerClass.Instance.Swizzle(ref Pointer);
        }

        public ref T Ref { get => ref Pointer.Ref; }
        public T Data { get => Pointer.Ref; set => Pointer.Ref = value; }
        public ref T this[int index] { get => ref Pointer[index]; }

        public bool IsNull { get => Pointer == Pointer<T>.Zero; }
        public static implicit operator Pointer<T>(SwizzleablePointer<T> obj) => obj.Pointer;
    }
}
