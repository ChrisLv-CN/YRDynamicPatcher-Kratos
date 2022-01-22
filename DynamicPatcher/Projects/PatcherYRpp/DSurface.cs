using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PatcherYRpp.FileFormats;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 36)]
    [Serializable]
    public struct DSurface
    {
        static private IntPtr tile = new IntPtr(0x8872FC);
        public static ref Pointer<Surface> Tile { get => ref tile.Convert<Pointer<Surface>>().Ref; }

        static private IntPtr sidebar = new IntPtr(0x887300);
        public static ref Pointer<Surface> Sidebar { get => ref sidebar.Convert<Pointer<Surface>>().Ref; }
        static private IntPtr primary = new IntPtr(0x887308);
        public static ref Pointer<Surface> Primary { get => ref primary.Convert<Pointer<Surface>>().Ref; }
        static private IntPtr hidden = new IntPtr(0x88730C);
        public static ref Pointer<Surface> Hidden { get => ref hidden.Convert<Pointer<Surface>>().Ref; }

        static private IntPtr alternate = new IntPtr(0x887310);
        public static ref Pointer<Surface> Alternate { get => ref alternate.Convert<Pointer<Surface>>().Ref; }

        static private IntPtr temp = new IntPtr(0x887314);
        public static ref Pointer<Surface> Temp { get => ref temp.Convert<Pointer<Surface>>().Ref; }

        static private IntPtr composite = new IntPtr(0x88731C);
        public static ref Pointer<Surface> Composite { get => ref composite.Convert<Pointer<Surface>>().Ref; }

        public static unsafe void Constructor(Pointer<DSurface> pThis, int Width, int Height, bool BackBuffer, bool Force3D)
        {
            var func = (delegate* unmanaged[Thiscall]<ref DSurface, int, int, Bool, Bool, IntPtr>)0x4BA5A0;
            func(ref pThis.Ref, Width, Height, BackBuffer, Force3D);
        }


        public static unsafe void Destructor(Pointer<DSurface> pThis)
        {
            var func = (delegate* unmanaged[Thiscall]<ref DSurface, void>)Helpers.GetVirtualFunctionPointer(pThis, 0);
            func(ref pThis.Ref);
        }

        [FieldOffset(0)] public Surface Base;
    }
}
