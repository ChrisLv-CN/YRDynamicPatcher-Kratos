using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [System.Diagnostics.DebuggerDisplay("Value={Value}")]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Pointer<T>
    {
        public static readonly Pointer<T> Zero = new Pointer<T>(0);
        public Pointer(int value)
        {
            Value = (void*)value;
        }
        public Pointer(long value)
        {
            Value = (void*)value;
        }
        public Pointer(void* value)
        {
            Value = value;
        }
        public Pointer(IntPtr value)
        {
            Value = value.ToPointer();
        }

        public void* Value;
        public ref T Ref { get => ref Unsafe.AsRef<T>(Value); }
        public T Data { get => Unsafe.Read<T>(Value); set => Unsafe.Write(Value, value); }
        public ref T this[int index] { get => ref Unsafe.Add(ref Unsafe.AsRef<T>(Value), index); }

        public bool IsNull { get => this == Zero; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pointer<TTo> Convert<TTo>()
        {
            return new Pointer<TTo>(Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> AsPointer(ref T obj)
        {
            return new Pointer<T>(Unsafe.AsPointer(ref obj));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TypeSize()
        {
            return Unsafe.SizeOf<T>();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> operator +(Pointer<T> ptr, int val) => (Pointer<T>)Unsafe.Add<T>(ptr.Value, val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> operator -(Pointer<T> ptr, int val) => (Pointer<T>)Unsafe.Subtract<T>(ptr.Value, val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long operator -(Pointer<T> value1, Pointer<T> value2) => ((long)value1.Value - (long)value2.Value) / TypeSize();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Pointer<T> value1, Pointer<T> value2) => value1.Value == value2.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Pointer<T> value1, Pointer<T> value2) => value1.Value != value2.Value;
        public override int GetHashCode() => ((IntPtr)Value).GetHashCode();
        public override bool Equals(object obj) => Value == ((Pointer<T>)obj).Value;
        public override string ToString() => ((IntPtr)Value).ToString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int(Pointer<T> value) => (int)value.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator void*(Pointer<T> value) => value.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator long(Pointer<T> value) => (long)value.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator IntPtr(Pointer<T> value) => (IntPtr)value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Pointer<T>(void* value) => new Pointer<T>(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Pointer<T>(int value) => new Pointer<T>(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Pointer<T>(long value) => new Pointer<T>(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Pointer<T>(IntPtr value) => new Pointer<T>(value);
    }
}
