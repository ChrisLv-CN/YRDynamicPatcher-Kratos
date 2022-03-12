using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    public class PointerHandle<T> : CriticalFinalizerObject, IDisposable
    {
        private Pointer<Pointer<T>> fixedPointer;
        public virtual ref Pointer<T> Pointer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref fixedPointer.Ref;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PointerHandle()
        {
            fixedPointer = Marshal.AllocHGlobal(Pointer<Pointer<T>>.TypeSize());
        }
        public PointerHandle(Pointer<T> ptr) : this()
        {
            fixedPointer.Ref = ptr;
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
