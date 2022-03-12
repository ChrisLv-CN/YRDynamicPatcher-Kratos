using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using DynamicPatcher;
using System.Runtime.Caching;
using System.Runtime.ConstrainedExecution;

namespace PatcherYRpp
{
    public static class Helpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ref T GetUnmanagedRef<T>(IntPtr ptr, int offset = 0)
        {
            return ref ptr.Convert<T>()[offset];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Span<T> GetSpan<T>(IntPtr ptr, int length)
        {
            return new Span<T>(ptr.ToPointer(), length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TTo ForceConvert<TFrom, TTo>(TFrom obj)
        {
            return Unsafe.As<TFrom, TTo>(ref obj);
            //    var ptr = new Pointer<TTo>(Pointer<TFrom>.AsPointer(ref obj));
            //    return ptr.Ref;
        }

        static MemoryCache VirtualFunctionCache = new MemoryCache("virtual functions");
        public static T GetVirtualFunction<T>(IntPtr pThis, int index) where T : Delegate
        {
            IntPtr address = GetVirtualFunctionPointer(pThis, index);

            string key = address.ToString();

            var ret = VirtualFunctionCache.Get(key);
            if (ret == null)
            {
                var policy = new CacheItemPolicy
                {
                    SlidingExpiration = TimeSpan.FromSeconds(60.0)
                };
                VirtualFunctionCache.Set(key, Marshal.GetDelegateForFunctionPointer<T>(address), policy);
                ret = VirtualFunctionCache.Get(key);
            }

            return ret as T;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr GetVirtualFunctionTable(IntPtr pThis, int tableIndex)
        {
            Pointer<Pointer<IntPtr>> pVfptr = pThis;
            Pointer<IntPtr> vfptr = pVfptr[tableIndex];
            return vfptr;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr GetVirtualFunctionTable<T>(Pointer<T> pThis, int tableIndex)
        {
            return GetVirtualFunctionTable((IntPtr)pThis, tableIndex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr GetVirtualFunctionTable<T>(ref this T pThis, int tableIndex) where T : struct
        {
            return GetVirtualFunctionTable(Pointer<T>.AsPointer(ref pThis), tableIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr GetVirtualFunctionPointer(IntPtr pThis, int index, int tableIndex = 0)
        {
            Pointer<IntPtr> vfptr = GetVirtualFunctionTable(pThis, tableIndex);
            IntPtr address = vfptr[index];
            return address;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr GetVirtualFunctionPointer<T>(Pointer<T> pThis, int index, int tableIndex = 0)
        {
            return GetVirtualFunctionPointer((IntPtr)pThis, index, tableIndex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr GetVirtualFunctionPointer<T>(ref this T pThis, int index, int tableIndex = 0) where T : struct
        {
            return GetVirtualFunctionPointer(Pointer<T>.AsPointer(ref pThis), index, tableIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetVirtualFunctionPointer(IntPtr pThis, int index, IntPtr functionAddress, int tableIndex = 0)
        {
            Pointer<IntPtr> vfptr = GetVirtualFunctionTable(pThis, tableIndex);
            vfptr[index] = functionAddress;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetVirtualFunctionPointer<T>(Pointer<T> pThis, int index, IntPtr functionAddress, int tableIndex = 0)
        {
            SetVirtualFunctionPointer((IntPtr)pThis, index, functionAddress, tableIndex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetVirtualFunctionPointer<T>(ref this T pThis, int index, IntPtr functionAddress, int tableIndex = 0) where T : struct
        {
            SetVirtualFunctionPointer(Pointer<T>.AsPointer(ref pThis), index, functionAddress, tableIndex);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> Convert<T>(this IntPtr ptr)
        {
            return new Pointer<T>(ptr);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(IntPtr from, IntPtr to, int byteCount)
        {
            Unsafe.CopyBlock(to.ToPointer(), from.ToPointer(), (uint)byteCount);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> GetThisPointer<T>(ref this T pThis) where T : struct
        {
            return Pointer<T>.AsPointer(ref pThis);
        }
    }
}
