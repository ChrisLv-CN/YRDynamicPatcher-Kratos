using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using HRESULT = System.UInt32;
using WORD = System.UInt16;
using DWORD = System.UInt32;
using LONG = System.UInt32;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DDCOLORKEY
    {
        public DWORD dwColorSpaceLowValue;   // low boundary of color space that is to
                                             // be treated as Color Key, inclusive
        public DWORD dwColorSpaceHighValue;  // high boundary of color space that is
                                             // to be treated as Color Key, inclusive
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct DDSCAPS2
    {
        [FieldOffset(0)] public DWORD dwCaps;         // capabilities of surface wanted
        [FieldOffset(4)] public DWORD dwCaps2;
        [FieldOffset(8)] public DWORD dwCaps3;

        [FieldOffset(12)] public DWORD dwCaps4;
        [FieldOffset(12)] public DWORD dwVolumeDepth;
    }

    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct DDPIXELFORMAT
    {
        [FieldOffset(0)] public DWORD dwSize;                 // size of structure
        [FieldOffset(4)] public DWORD dwFlags;                // pixel format flags
        [FieldOffset(8)] public DWORD dwFourCC;               // (FOURCC code)

        [FieldOffset(12)] public DWORD dwRGBBitCount;          // how many bits per pixel
        [FieldOffset(12)] public DWORD dwYUVBitCount;          // how many bits per pixel
        [FieldOffset(12)] public DWORD dwZBufferBitDepth;      // how many total bits/pixel in z buffer (including any stencil bits)
        [FieldOffset(12)] public DWORD dwAlphaBitDepth;        // how many bits for alpha channels
        [FieldOffset(12)] public DWORD dwLuminanceBitCount;    // how many bits per pixel
        [FieldOffset(12)] public DWORD dwBumpBitCount;         // how many bits per "buxel", total
        [FieldOffset(12)] public DWORD dwPrivateFormatBitCount;// Bits per pixel of private driver formats. Only valid in texture
                                                               // format list and if DDPF_D3DFORMAT is set

        [FieldOffset(16)] public DWORD dwRBitMask;             // mask for red bit
        [FieldOffset(16)] public DWORD dwYBitMask;             // mask for Y bits
        [FieldOffset(16)] public DWORD dwStencilBitDepth;      // how many stencil bits (note: dwZBufferBitDepth-dwStencilBitDepth is total Z-only bits)
        [FieldOffset(16)] public DWORD dwLuminanceBitMask;     // mask for luminance bits
        [FieldOffset(16)] public DWORD dwBumpDuBitMask;        // mask for bump map U delta bits
        [FieldOffset(16)] public DWORD dwOperations;           // DDPF_D3DFORMAT Operations

        [FieldOffset(20)] public DWORD dwGBitMask;             // mask for green bits
        [FieldOffset(20)] public DWORD dwUBitMask;             // mask for U bits
        [FieldOffset(20)] public DWORD dwZBitMask;             // mask for Z bits
        [FieldOffset(20)] public DWORD dwBumpDvBitMask;        // mask for bump map V delta bits
        [StructLayout(LayoutKind.Sequential)]
        public struct _TMultiSampleCaps
        {
            public WORD wFlipMSTypes;       // Multisample methods supported via flip for this D3DFORMAT
            public WORD wBltMSTypes;        // Multisample methods supported via blt for this D3DFORMAT
        }
        [FieldOffset(20)] public _TMultiSampleCaps MultiSampleCaps;

        [FieldOffset(24)] public DWORD dwBBitMask;             // mask for blue bits
        [FieldOffset(24)] public DWORD dwVBitMask;             // mask for V bits
        [FieldOffset(24)] public DWORD dwStencilBitMask;       // mask for stencil bits
        [FieldOffset(24)] public DWORD dwBumpLuminanceBitMask; // mask for luminance in bump map

        [FieldOffset(28)] public DWORD dwRGBAlphaBitMask;      // mask for alpha channel
        [FieldOffset(28)] public DWORD dwYUVAlphaBitMask;      // mask for alpha channel
        [FieldOffset(28)] public DWORD dwLuminanceAlphaBitMask;// mask for alpha channel
        [FieldOffset(28)] public DWORD dwRGBZBitMask;          // mask for Z channel
        [FieldOffset(28)] public DWORD dwYUVZBitMask;          // mask for Z channel
    }

    [StructLayout(LayoutKind.Explicit, Size = 124)]
    public struct DDSURFACEDESC2
    {
        [FieldOffset(0)] public DWORD dwSize;                 // size of the DDSURFACEDESC structure
        [FieldOffset(4)] public DWORD dwFlags;                // determines what fields are valid
        [FieldOffset(8)] public DWORD dwHeight;               // height of surface to be created
        [FieldOffset(12)] public DWORD dwWidth;                // width of input surface

        [FieldOffset(16)] public LONG lPitch;                 // distance to start of next line (return value only)
        [FieldOffset(16)] public DWORD dwLinearSize;           // Formless late-allocated optimized surface size


        [FieldOffset(20)] public DWORD dwBackBufferCount;      // number of back buffers requested
        [FieldOffset(20)] public DWORD dwDepth;                // the depth if this is a volume texture 

        [FieldOffset(24)] public DWORD dwMipMapCount;          // number of mip-map levels requestde
                                                               // dwZBufferBitDepth removed, use ddpfPixelFormat one instead
        [FieldOffset(24)] public DWORD dwRefreshRate;          // refresh rate (used when display mode is described)
        [FieldOffset(24)] public DWORD dwSrcVBHandle;          // The source used in VB::Optimize


        [FieldOffset(28)] public DWORD dwAlphaBitDepth;        // depth of alpha buffer requested
        [FieldOffset(32)] public DWORD dwReserved;             // reserved
        [FieldOffset(36)] public IntPtr lpSurface;              // pointer to the associated surface memory

        [FieldOffset(40)] public DDCOLORKEY ddckCKDestOverlay;      // color key for destination overlay use
        [FieldOffset(40)] public DWORD dwEmptyFaceColor;       // Physical color for empty cubemap faces

        [FieldOffset(48)] public DDCOLORKEY ddckCKDestBlt;          // color key for destination blt use
        [FieldOffset(56)] public DDCOLORKEY ddckCKSrcOverlay;       // color key for source overlay use
        [FieldOffset(64)] public DDCOLORKEY ddckCKSrcBlt;           // color key for source blt use

        [FieldOffset(72)] public DDPIXELFORMAT ddpfPixelFormat;        // pixel format description of the surface
        [FieldOffset(72)] public DWORD dwFVF;                  // vertex format description of vertex buffers

        [FieldOffset(104)] public DDSCAPS2 ddsCaps;                // direct draw surface capabilities
        [FieldOffset(120)] public DWORD dwTextureStage;         // stage in multitexture cascade
    };

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirectDraw
    {
        HRESULT Compact();
        HRESULT CreateClipper(IntPtr _1, IntPtr _2);
        HRESULT CreatePalette(uint _1, IntPtr _2, IntPtr _3, IntPtr _4);
        HRESULT CreateSurface(Pointer<DDSURFACEDESC2> lpDDSurfaceDesc, Pointer<Pointer<IDirectDrawSurface>> lpDDSurface, IntPtr unkOuter);

        // ...more
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirectDrawSurface
    {
        HRESULT AddAttachedSurface(Pointer<IDirectDrawSurface> lpDDSAttachedSurface);
        HRESULT AddOverlayDirtyRect(Pointer<Rect> lpDirtyRect);
        HRESULT Blt(Pointer<Rect> lpDestRect, Pointer<IDirectDrawSurface> lpDDSrcSurface, Pointer<Rect> lpSrcRect, uint dwFlags, IntPtr lpDDBltFx);
        HRESULT BltBatch(IntPtr lpDDBltBatch, uint dwCount, uint dwFlags);
        HRESULT BltFast(uint dwX, uint dwY, Pointer<IDirectDrawSurface> lpDDSrcSurface, Pointer<Rect> lpSrcRect, uint dwTrans);
        HRESULT DeleteAttachedSurface(uint dwFlags, Pointer<IDirectDrawSurface> lpDDSAttachedSurface);
        HRESULT EnumAttachedSurfaces(IntPtr lpContext, IntPtr lpEnumSurfacesCallback);
        HRESULT EnumOverlayZOrders(uint dwFlags, IntPtr lpContext, IntPtr lpfnCallback);
        HRESULT Flip(Pointer<IDirectDrawSurface> lpDDSurfaceTargetOverride, uint dwFlags);
        HRESULT GetAttachedSurface(IntPtr lpDDSCaps, Pointer<IDirectDrawSurface> lplpDDAttachedSurface);
        HRESULT GetBltStatus(uint dwFlags);
        HRESULT GetCaps(IntPtr lpDDCaps);
        HRESULT GetClipper(IntPtr lplpDDClipper);
        HRESULT GetColorKey(uint dwFlags, IntPtr lpDDColorKey);
        HRESULT GetDC(IntPtr lphDC);
        HRESULT GetFlipStatus(uint dwFlags);
        HRESULT GetOverlayPosition(Pointer<int> lplX, Pointer<int> lplY);
        HRESULT GetPalette(IntPtr lplpDDPalette);
        HRESULT GetPixelFormat(IntPtr lpDDPixelFormat);
        HRESULT GetSurfaceDesc(IntPtr lpDDSurfaceDesc);
        HRESULT Initialize(IntPtr lpDD, IntPtr lpDDSurfaceDesc);
        HRESULT IsLost();
        HRESULT Lock(Pointer<Rect> lpDestRect, IntPtr lpDDSurfaceDesc, uint dwFlags, IntPtr hEvent);
        HRESULT ReleaseDC(IntPtr hDC);
        HRESULT Restore();
        HRESULT SetClipper(IntPtr lpDirectDrawClipper);
        HRESULT SetColorKey(uint dwFlags, IntPtr lpDDColorKey);
        HRESULT SetOverlayPosition(int lX, int lY);
        HRESULT SetPalette(IntPtr lpDDPalette);
        HRESULT Unlock(IntPtr lpSurfaceData);
        HRESULT UpdateOverlay(Pointer<Rect> lpSrcRect, Pointer<IDirectDrawSurface> lpDDDestSurface, Pointer<Rect> lpDestRect, uint dwFlags, IntPtr lpDDOverlayFx);
        HRESULT UpdateOverlayDisplay(uint dwFlags);
        HRESULT UpdateOverlayZOrder(uint dwFlags, Pointer<IDirectDrawSurface> lpDDSReference);
    }

}
