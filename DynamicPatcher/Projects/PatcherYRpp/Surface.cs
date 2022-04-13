using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PatcherYRpp.FileFormats;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 36)]
    public struct Surface
    {
        private static IntPtr ppTile = new IntPtr(0x8872FC);
        private static IntPtr ppSidebar = new IntPtr(0x887300);
        private static IntPtr ppPrimary = new IntPtr(0x887308);
        private static IntPtr ppHidden = new IntPtr(0x88730C);
        private static IntPtr ppAlternate = new IntPtr(0x887310);
        private static IntPtr ppCurrent = new IntPtr(0x887314);
        private static IntPtr ppComposite = new IntPtr(0x88731C);

        public static Pointer<Surface> Tile { get => ((Pointer<Pointer<Surface>>)ppTile).Data; set => ((Pointer<Pointer<Surface>>)ppTile).Ref = value; }
        public static Pointer<Surface> Sidebar { get => ((Pointer<Pointer<Surface>>)ppSidebar).Data; set => ((Pointer<Pointer<Surface>>)ppSidebar).Ref = value; }
        public static Pointer<Surface> Primary { get => ((Pointer<Pointer<Surface>>)ppPrimary).Data; set => ((Pointer<Pointer<Surface>>)ppPrimary).Ref = value; }
        public static Pointer<Surface> Hidden { get => ((Pointer<Pointer<Surface>>)ppHidden).Data; set => ((Pointer<Pointer<Surface>>)ppHidden).Ref = value; }
        public static Pointer<Surface> Alternate { get => ((Pointer<Pointer<Surface>>)ppAlternate).Data; set => ((Pointer<Pointer<Surface>>)ppAlternate).Ref = value; }
        public static Pointer<Surface> Current { get => ((Pointer<Pointer<Surface>>)ppCurrent).Data; set => ((Pointer<Pointer<Surface>>)ppCurrent).Ref = value; }
        public static Pointer<Surface> Composite { get => ((Pointer<Pointer<Surface>>)ppComposite).Data; set => ((Pointer<Pointer<Surface>>)ppComposite).Ref = value; }

        private static IntPtr pViewBound = new IntPtr(0x886FA0);
        public static ref RectangleStruct ViewBound => ref pViewBound.Convert<RectangleStruct>().Ref;

        public unsafe bool BlitWhole(Pointer<Surface> pSrc, bool unk1, bool unk2)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, IntPtr, Bool, Bool, Bool>)this.GetVirtualFunctionPointer(1);
            return func(ref this, pSrc, unk1, unk2);
        }
        public unsafe bool BlitPart(RectangleStruct clipRect, Pointer<Surface> pSrc, RectangleStruct srcRect, bool unk1, bool unk2)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, ref RectangleStruct, IntPtr, ref RectangleStruct, Bool, Bool, Bool>)this.GetVirtualFunctionPointer(2);
            return func(ref this, ref clipRect, pSrc, ref srcRect, unk1, unk2);
        }
        public unsafe bool Blit(RectangleStruct clipRect, RectangleStruct clipRect2, Pointer<Surface> pSrc, RectangleStruct destRect, RectangleStruct srcRect, bool unk1, bool unk2)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, ref RectangleStruct, ref RectangleStruct, IntPtr, ref RectangleStruct, ref RectangleStruct, Bool, Bool, Bool>)this.GetVirtualFunctionPointer(3);
            return func(ref this, ref clipRect, ref clipRect2, pSrc, ref destRect, ref srcRect, unk1, unk2);
        }

        public unsafe bool FillRectEx(RectangleStruct clipRect, RectangleStruct fillRect, int dwColor)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, ref RectangleStruct, ref RectangleStruct, int, Bool>)this.GetVirtualFunctionPointer(4);
            return func(ref this, ref clipRect, ref fillRect, dwColor);
        }
        public unsafe bool FillRect(RectangleStruct fillRect, int dwColor)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, ref RectangleStruct, int, Bool>)this.GetVirtualFunctionPointer(5);
            return func(ref this, ref fillRect, dwColor);
        }
        public unsafe bool FillRect(int dwColor)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, int, Bool>)this.GetVirtualFunctionPointer(6);
            return func(ref this, dwColor);
        }



        public unsafe bool SetPixel(Point2D point, int dwColor)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, ref Point2D, int, Bool>)this.GetVirtualFunctionPointer(9);
            return func(ref this, ref point, dwColor);
        }
        public unsafe int GetPixel(Point2D point)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, ref Point2D, int>)this.GetVirtualFunctionPointer(10);
            return func(ref this, ref point);
        }

        public unsafe bool DrawLineEx(RectangleStruct clipRect, Point2D point1, Point2D point2, int dwColor)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, ref RectangleStruct, ref Point2D, ref Point2D, int, Bool>)this.GetVirtualFunctionPointer(11);
            return func(ref this, ref clipRect, ref point1, ref point2, dwColor);
        }

        public unsafe bool DrawLine(Point2D point1, Point2D point2, int dwColor)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, ref Point2D, ref Point2D, int, Bool>)this.GetVirtualFunctionPointer(12);
            return func(ref this, ref point1, ref point2, dwColor);
        }


        public unsafe bool DrawRibbon(RectangleStruct clipRect, Point2D srcPoint, Point2D destPoint, ColorStruct color, float intensity, int srcZAdjust, int destZAdjust)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, ref RectangleStruct, ref Point2D, ref Point2D, ref ColorStruct, float, int, int, Bool>)this.GetVirtualFunctionPointer(16);
            return func(ref this, ref clipRect, ref srcPoint, ref destPoint, ref color, intensity, srcZAdjust, destZAdjust);
        }


        public unsafe bool DrawRectEx(RectangleStruct clipRect, RectangleStruct drawRect, int dwColor)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, ref RectangleStruct, ref RectangleStruct, int, Bool>)this.GetVirtualFunctionPointer(21);
            return func(ref this, ref clipRect, ref drawRect, dwColor);
        }
        public unsafe bool DrawRect(RectangleStruct drawRect, int dwColor)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, ref RectangleStruct, int, Bool>)this.GetVirtualFunctionPointer(22);
            return func(ref this, ref drawRect, dwColor);
        }

        public unsafe IntPtr Lock(int X, int Y)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, int, int, IntPtr>)this.GetVirtualFunctionPointer(23);
            return func(ref this, X, Y);
        }
        public unsafe bool Unlock()
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, Bool>)this.GetVirtualFunctionPointer(24);
            return func(ref this);
        }


        public unsafe int GetBytesPerPixel()
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, int>)this.GetVirtualFunctionPointer(28);
            return func(ref this);
        }
        public unsafe int GetPitch()
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, int>)this.GetVirtualFunctionPointer(29);
            return func(ref this);
        }
        public unsafe RectangleStruct GetRect()
        {
            RectangleStruct ret = default;
            var func = (delegate* unmanaged[Thiscall]<ref Surface, ref RectangleStruct, IntPtr>)this.GetVirtualFunctionPointer(30);
            func(ref this, ref ret);
            return ret;
        }
        public unsafe int GetWidth()
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, int>)this.GetVirtualFunctionPointer(31);
            return func(ref this);
        }
        public unsafe int GetHeight()
        {
            var func = (delegate* unmanaged[Thiscall]<ref Surface, int>)this.GetVirtualFunctionPointer(32);
            return func(ref this);
        }

        public unsafe void DrawSHP(Pointer<SHPStruct> pSHP, int nFrame, Pointer<ConvertClass> pPalette, int X, int Y)
        {
            DrawSHP(pSHP, nFrame, pPalette, new Point2D(X, Y));
        }

        public unsafe void DrawSHP(Pointer<SHPStruct> pSHP, int nFrame, Pointer<ConvertClass> pPalette, Point2D point)
        {
            RectangleStruct rect = this.GetRect();
            DrawSHP(pPalette, pSHP, nFrame, point, rect, BlitterFlags.None, 0, 0, 0, 0x3E8, 0, IntPtr.Zero, 0, 0, 0);
        }

        public unsafe void DrawSHP(Pointer<ConvertClass> pPalette, Pointer<SHPStruct> pSHP, int frameIdx,
            Point2D pos, RectangleStruct bound, BlitterFlags flags, uint arg7,
            int zAdjust, uint arg9, uint bright, int TintColor, Pointer<SHPStruct> BUILDINGZ_SHA, uint argD, int ZS_X, int ZS_Y)
        {
            CC_Draw_Shape(pPalette, pSHP, frameIdx, Pointer<Point2D>.AsPointer(ref pos), Pointer<RectangleStruct>.AsPointer(ref bound), flags, (int)arg7, zAdjust, (ZGradient)arg9, (int)bright, TintColor, BUILDINGZ_SHA, (int)argD, ZS_X, ZS_Y);
        }


        public unsafe void DrawSHP(Pointer<ConvertClass> pPalette, Pointer<SHPStruct> pSHP, int frameIdx, Point2D pos)
        {
            RectangleStruct bound = this.GetRect();
            DrawSHP(pPalette, pSHP, frameIdx, pos, Pointer<RectangleStruct>.AsPointer(ref bound));
        }

        public unsafe void DrawSHP(Pointer<ConvertClass> pPalette, Pointer<SHPStruct> pSHP, int frameIdx, Point2D pos, RectangleStruct bound)
        {
            DrawSHP(pPalette, pSHP, frameIdx, pos, Pointer<RectangleStruct>.AsPointer(ref bound));
        }

        public unsafe void DrawSHP(Pointer<ConvertClass> pPalette, Pointer<SHPStruct> pSHP, int frameIdx,
            Point2D pos, Pointer<RectangleStruct> pBound, BlitterFlags flags = (BlitterFlags)0x600, int bright = 1000, int tintColor = 0)
        {
            CC_Draw_Shape(pPalette, pSHP, frameIdx,
                Pointer<Point2D>.AsPointer(ref pos), pBound, flags,
                0, 0, ZGradient.Ground, bright, tintColor,
                Pointer<SHPStruct>.Zero, 0, 0, 0);
        }

        // Comments from thomassneddon
        public unsafe void CC_Draw_Shape(Pointer<ConvertClass> pPalette, Pointer<SHPStruct> pSHP, int frameIdx,
            Pointer<Point2D> pLocation, Pointer<RectangleStruct> pBound, BlitterFlags flags,
            int shaperFlags,
            int zAdjust, // + 1 = sqrt(3.0) pixels away from screen
            ZGradient zGradientDescIndex,
            int bright, // 0~2000. Final color = saturate(OriginalColor * Brightness / 1000.0f)
            int tintColor,
            Pointer<SHPStruct> BUILDINGZ_SHA, int ZShapeFrame, int XOffset, int YOffset)
        {
            var func = (delegate* unmanaged[Thiscall]<int, ref Surface, IntPtr, IntPtr, int,
                IntPtr, IntPtr, BlitterFlags,
                int,
                int,
                ZGradient,
                int,
                int,
                IntPtr, int, int, int, void>)ASM.FastCallTransferStation;
            func(0x4AED70, ref this, pPalette, pSHP, frameIdx,
                pLocation, pBound, flags,
                shaperFlags,
                zAdjust,
                zGradientDescIndex,
                bright,
                tintColor,
                BUILDINGZ_SHA, ZShapeFrame, XOffset, YOffset);
        }

        public unsafe void DrawText(string text, Pointer<RectangleStruct> pBound, Pointer<Point2D> pLocation, int foreColor, int backColor, TextPrintType flags)
        {
            Point2D temp = default;
            Fancy_Text_Print_Wide(Pointer<Point2D>.AsPointer(ref temp), text, Pointer<Surface>.AsPointer(ref this), pBound, pLocation, foreColor, backColor, flags);
        }

        public unsafe void DrawText(string text, Pointer<RectangleStruct> pBound, Pointer<Point2D> pLocation, ColorStruct color, TextPrintType flags = TextPrintType.NoShadow)
        {
            DrawText(text, pBound, pLocation, Drawing.RGB2DWORD(color), 0, flags);
        }

        public unsafe void DrawText(string text, Pointer<Point2D> pLocation, ColorStruct color, TextPrintType flags = TextPrintType.NoShadow)
        {
            RectangleStruct bound = GetRect();
            DrawText(text, Pointer<RectangleStruct>.AsPointer(ref bound), pLocation, color, flags);
        }

        public unsafe void DrawText(string text, Point2D pos, ColorStruct color, TextPrintType flags = TextPrintType.NoShadow)
        {
            DrawText(text, Pointer<Point2D>.AsPointer(ref pos), color, flags);
        }

        public unsafe void DrawText(string text, int x, int y, ColorStruct color, TextPrintType flags = TextPrintType.NoShadow)
        {
            Point2D temp = new Point2D(x, y);
            DrawText(text, Pointer<Point2D>.AsPointer(ref temp), color, flags);
        }

        // Comments from thomassneddon
        public static unsafe Pointer<Point2D> Fancy_Text_Print_Wide(Pointer<Point2D> RetVal, UniString pText, Pointer<Surface> pSurface, Pointer<RectangleStruct> pBound,
            Pointer<Point2D> pLocation, int foreColor, int backColor, TextPrintType flags)
        {
            var func = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, int, int, TextPrintType, int, IntPtr>)0x4A60E0;
            return func(RetVal, pText, pSurface, pBound, pLocation, foreColor, backColor, flags, 0);
        }

        [FieldOffset(0)] public int Vfptr;

        [FieldOffset(4)] public int Width;                         /*| BSurface->*/
        [FieldOffset(8)] public int Height;                        /*| BSurface->*/
        [FieldOffset(12)] public int LockLevel;                    /*| BSurface->*/
        [FieldOffset(16)] public int BytesPerPixel;                /*| BSurface->*/
        [FieldOffset(20)] public Pointer<byte> Buffer;             /*| BSurface->*/
        [FieldOffset(24)] public Bool Allocated;                   /*| BSurface->*/ [FieldOffset(24)] public int BufferSize;
        [FieldOffset(25)] public Bool VRAMmed;                     /*| BSurface->*/
        [FieldOffset(26)] public byte unknown_1A;                  /*| BSurface->*/
        [FieldOffset(27)] public byte unknown_1B;                  /*| BSurface->*/
        [FieldOffset(28)] public Pointer<IDirectDrawSurface> Surf; /*| BSurface->*/ [FieldOffset(28)] public Bool BufferAllocated;
        [FieldOffset(32)] public Pointer<DDSURFACEDESC2> SurfDesc; /*| BSurface->*/


    }

    [StructLayout(LayoutKind.Explicit, Size = 36)]
    public struct DSurface
    {
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
