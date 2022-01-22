using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    public class PointerHandle<T> : CriticalFinalizerObject, IDisposable
    {
        private Pointer<Pointer<T>> fixedPointer;
        public virtual ref Pointer<T> Pointer { get => ref fixedPointer.Ref; }
        public PointerHandle()
        {
            fixedPointer = Marshal.AllocHGlobal(Pointer<Pointer<T>>.TypeSize());
        }
        public PointerHandle(Pointer<T> ptr) : this()
        {
            fixedPointer.Ref = ptr;
        }

        public ref T Ref { get => ref Pointer.Ref; }
        public T Data { get => Pointer.Ref; set => Pointer.Ref = value; }
        public ref T this[int index] { get => ref Pointer[index]; }

        public bool IsNull { get => Pointer == Pointer<T>.Zero; }
        public static implicit operator Pointer<T>(PointerHandle<T> obj) => obj.Pointer;

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                Marshal.FreeHGlobal(fixedPointer);
                disposedValue = true;
            }
        }

        ~PointerHandle()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
