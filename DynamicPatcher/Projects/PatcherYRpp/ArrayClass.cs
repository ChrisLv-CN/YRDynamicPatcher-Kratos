using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{

    [StructLayout(LayoutKind.Sequential)]
    public struct DynamicVectorClass<T> : IEnumerable<T>
    {
        public static ref DynamicVectorClass<T> GetDynamicVector(IntPtr ptr)
        {
            return ref Helpers.GetUnmanagedRef<DynamicVectorClass<T>>(ptr);
        }

        public ref T this[int index] { get => ref Get(index); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(int index) => ref Helpers.GetUnmanagedRef<T>(Items, index);

        public Span<T> GetSpan()
        {
            return Helpers.GetSpan<T>(Items, Count);
        }

        class Enumerator : IEnumerator<T>, IEnumerator
        {
            internal Enumerator(Pointer<T> items, int count)
            {
                this.items = items;
                this.count = count;
                Reset();
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                if (index < count)
                {
                    current = items[index];
                    index++;
                    return true;
                }
                return false;
            }

            public T Current => current;

            object IEnumerator.Current => Current;

            public void Reset()
            {
                index = 0;
                current = default(T);
            }

            private Pointer<T> items;
            private int count;
            private int index;
            private T current;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(Items, Count);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private IntPtr Vfptr;
        public IntPtr Items;
        public int Capacity;
        public Bool IsInitialized;
        public Bool IsAllocated;
        public int Count;
        public int CapacityIncrement;
    }
}
