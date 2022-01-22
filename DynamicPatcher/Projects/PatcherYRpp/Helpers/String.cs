using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    public class AnsiString : CriticalFinalizerObject, IDisposable
    {
        IntPtr hGlobal;
        bool allocated;

        public AnsiString(string str)
        {
            hGlobal = Marshal.StringToHGlobalAnsi(str);
            allocated = true;
        }
        public AnsiString(IntPtr buffer, bool allocate = false)
        {
            if (allocate)
            {
                hGlobal = Marshal.StringToHGlobalAnsi(Marshal.PtrToStringAnsi(buffer));
            }
            else
            {
                hGlobal = buffer;
            }
            allocated = allocate;
        }

        public static implicit operator IntPtr(AnsiString ansiStr) => ansiStr.hGlobal;
        public static implicit operator string(AnsiString ansiStr) => Marshal.PtrToStringAnsi((IntPtr)ansiStr);

        public static implicit operator AnsiString(string str) => new AnsiString(str);
        public static implicit operator AnsiString(IntPtr ptr) => new AnsiString(ptr);
        public static implicit operator AnsiString(Pointer<byte> ptr) => new AnsiString(ptr);
        
        public override string ToString() => this;

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                if (hGlobal != IntPtr.Zero && allocated)
                {
                    Marshal.FreeHGlobal(hGlobal);
                }
                disposedValue = true;
            }
        }

        ~AnsiString()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct AnsiStringPointer
    {
        IntPtr buffer;

        public AnsiStringPointer(IntPtr ptr)
        {
            buffer = ptr;
        }
        
        public static implicit operator AnsiString(AnsiStringPointer pointer) => new AnsiString(pointer.buffer);
        public static implicit operator IntPtr(AnsiStringPointer pointer) => pointer.buffer;
        public static implicit operator string(AnsiStringPointer pointer) => (AnsiString)pointer;
        
        public static implicit operator AnsiStringPointer(IntPtr ptr) => new AnsiStringPointer(ptr);
        public static implicit operator AnsiStringPointer(Pointer<byte> ptr) => new AnsiStringPointer(ptr);
        
        
        public override string ToString() => this;
    }
    

    public class UniString : CriticalFinalizerObject, IDisposable
    {
        IntPtr hGlobal;
        bool allocated;

        public UniString(string str)
        {
            hGlobal = Marshal.StringToHGlobalUni(str);
            allocated = true;
        }
        public UniString(IntPtr buffer, bool allocate = false)
        {
            if (allocate)
            {
                hGlobal = Marshal.StringToHGlobalUni(Marshal.PtrToStringUni(buffer));
            }
            else
            {
                hGlobal = buffer;
            }
            allocated = allocate;
        }

        public static implicit operator IntPtr(UniString uniStr) => uniStr.hGlobal;
        public static implicit operator string(UniString uniStr) => Marshal.PtrToStringUni((IntPtr)uniStr);

        public static implicit operator UniString(string str) => new UniString(str);
        public static implicit operator UniString(IntPtr ptr) => new UniString(ptr);
        public static implicit operator UniString(Pointer<char> ptr) => new UniString(ptr);
        
        public override string ToString() => this;

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                if (hGlobal != IntPtr.Zero && allocated)
                {
                    Marshal.FreeHGlobal(hGlobal);
                }
                disposedValue = true;
            }
        }

        ~UniString()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct UniStringPointer
    {
        IntPtr buffer;

        public UniStringPointer(IntPtr ptr)
        {
            buffer = ptr;
        }
        
        public static implicit operator UniString(UniStringPointer pointer) => new UniString(pointer.buffer);
        public static implicit operator IntPtr(UniStringPointer pointer) => pointer.buffer;
        public static implicit operator string(UniStringPointer pointer) => (UniString)pointer;
        
        public static implicit operator UniStringPointer(IntPtr ptr) => new UniStringPointer(ptr);
        public static implicit operator UniStringPointer(Pointer<char> ptr) => new UniStringPointer(ptr);
        
        
        public override string ToString() => this;
    }
}
