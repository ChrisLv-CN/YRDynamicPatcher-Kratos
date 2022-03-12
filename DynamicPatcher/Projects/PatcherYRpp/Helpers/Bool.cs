using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace PatcherYRpp
{
    [DebuggerDisplay("{ToBoolean()} ({_Value})")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bool : IComparable, IConvertible, IComparable<Bool>, IEquatable<Bool>
    {
        byte _Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool(byte val)
        {
            _Value = val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool(bool val)
        {
            _Value = Convert.ToByte(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ToBoolean() => Convert.ToBoolean(this._Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Bool(bool val) => new Bool(val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Bool(byte val) => new Bool(val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Bool(sbyte val) => Convert.ToByte(val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Bool(short val) => Convert.ToByte(val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Bool(ushort val) => Convert.ToByte(val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Bool(int val) => Convert.ToByte(val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Bool(uint val) => Convert.ToByte(val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Bool(long val) => Convert.ToByte(val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Bool(ulong val) => Convert.ToByte(val);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(Bool val) => val.ToBoolean();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator byte(Bool val) => Convert.ToByte(val._Value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator sbyte(Bool val) => Convert.ToSByte(val._Value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator short(Bool val) => Convert.ToInt16(val._Value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ushort(Bool val) => Convert.ToUInt16(val._Value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(Bool val) => Convert.ToInt32(val._Value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator uint(Bool val) => Convert.ToUInt32(val._Value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator long(Bool val) => Convert.ToInt64(val._Value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong(Bool val) => Convert.ToUInt64(val._Value);

        public static bool Parse(string value) => bool.Parse(value);
        public static bool TryParse(string value, out bool result) => bool.TryParse(value, out result);
        public int CompareTo(object obj) => this.ToBoolean().CompareTo(obj);
        public int CompareTo(Bool value) => this.ToBoolean().CompareTo(value);
        public override bool Equals(object obj) => this.ToBoolean().Equals(obj);
        public bool Equals(Bool obj) => this.ToBoolean().Equals(obj);
        public override int GetHashCode() => this.ToBoolean().GetHashCode();
        public TypeCode GetTypeCode() => this.ToBoolean().GetTypeCode();
        public override string ToString() => this.ToBoolean().ToString();
        public string ToString(IFormatProvider provider) => this.ToBoolean().ToString(provider);


        public bool ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)this.ToBoolean()).ToBoolean(provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return ((IConvertible)this.ToBoolean()).ToChar(provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)this.ToBoolean()).ToSByte(provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return ((IConvertible)this.ToBoolean()).ToByte(provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)this.ToBoolean()).ToInt16(provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)this.ToBoolean()).ToUInt16(provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)this.ToBoolean()).ToInt32(provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)this.ToBoolean()).ToUInt32(provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)this.ToBoolean()).ToInt64(provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)this.ToBoolean()).ToUInt64(provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)this.ToBoolean()).ToSingle(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)this.ToBoolean()).ToDouble(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)this.ToBoolean()).ToDecimal(provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)this.ToBoolean()).ToDateTime(provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)this.ToBoolean()).ToType(conversionType, provider);
        }
    }
}
