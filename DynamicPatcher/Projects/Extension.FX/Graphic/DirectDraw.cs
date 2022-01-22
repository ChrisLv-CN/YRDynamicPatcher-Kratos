using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Graphic
{
    public class DirectDraw
    {
        private const int DDSD_PIXELFORMAT = 0x00001000;

        private static IDirectDrawSurface _surface;


        public readonly static IntPtr ppDD = new IntPtr(0x8A0094);
        public static ref Pointer<IDirectDraw> lpDD => ref ppDD.Convert<Pointer<IDirectDraw>>().Ref;


        public static void Initialize()
        {
            var rect = YRGraphic.SurfaceRect;

            var _directDraw = Marshal.GetObjectForIUnknown(lpDD) as IDirectDraw;

            IntPtr _pSurface = IntPtr.Zero;
            DDSURFACEDESC2 desc = new DDSURFACEDESC2
            {
                dwSize = 108,
                dwFlags = 7,
                dwWidth = (uint)rect.Width,
                dwHeight = (uint)rect.Height,
                ddsCaps = new DDSCAPS2 { dwCaps = 2048 }
            };


            _directDraw.CreateSurface(
                    Pointer<DDSURFACEDESC2>.AsPointer(ref desc),
                    Pointer<IntPtr>.AsPointer(ref _pSurface).Convert<Pointer<IDirectDrawSurface>>(),
                    IntPtr.Zero);
        }
        public static void Dispose()
        {
            _surface = null;
        }
    }
}
