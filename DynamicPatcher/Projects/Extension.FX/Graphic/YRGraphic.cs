using PatcherYRpp;
using PatcherYRpp.FileFormats;
using SharpDX.Direct3D11;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using D3D11 = SharpDX.Direct3D11;

namespace Extension.FX.Graphic
{
    public class YRGraphic
    {
        private static ShaderResourceView _yrBufferTextureView;
        public static FXDrawObject drawObject;
        private static Pointer<Surface> additional;

        public static IntPtr WindowHandle => Process.GetCurrentProcess().MainWindowHandle;
        public static ref Surface PrimarySurface => ref Surface.Primary.Ref;
        public static ref ZBufferClass ZBuffer => ref ZBufferClass.ZBuffer.Ref;
        public static ref ABufferClass ABuffer => ref ABufferClass.ABuffer.Ref;
        public static RectangleStruct SurfaceRect => PrimarySurface.GetRect();
        public static ShaderResourceView BufferTextureView => _yrBufferTextureView;
        public static string PrimaryBufferTextureName => "YR_PrimaryBuffer";
        public static ref Surface AdditionalSurface => ref Surface.Composite.Ref;


        public static void Initialize(D3D11.Device d3dDevice)
        {
            var rect = SurfaceRect;

            var desc = new Texture2DDescription
            {
                Width = rect.Width,
                Height = rect.Height,
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.B5G6R5_UNorm,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
            };
            var texture = new Texture2D(d3dDevice, desc);

            _yrBufferTextureView = new ShaderResourceView(d3dDevice, texture);

            drawObject = new FXDrawObject(PrimaryBufferTextureName);

            additional = YRMemory.Create<DSurface>(rect.Width, rect.Height, true, true).Convert<Surface>();
        }
        public static void Dispose()
        {
            renderCTS?.Cancel();

            _yrBufferTextureView?.Dispose();
            drawObject?.Dispose();

            while (drawCells.TryDequeue(out var _)) ;

            YRMemory.Delete(additional.Convert<DSurface>());
        }

        public static void FillTexture()
        {
            Pointer<byte> buffer = PrimarySurface.Lock(0, 0);
            if (buffer.IsNull)
            {
                return;
            }

            var rect = SurfaceRect;
            //var coords = TacticalClass.Instance.Ref.ClientToCoords(new Point2D(rect.Width / 2, rect.Height / 2));

            drawObject.SetLocalBuffer(new Definitions.Vector3(rect.Width / 2, 0, int.MaxValue), new Definitions.Vector3(rect.Width / 2, rect.Height, int.MaxValue));

            int rowPitch = PrimarySurface.GetWidth() * PrimarySurface.GetBytesPerPixel();
            int depthPitch = PrimarySurface.GetHeight() * PrimarySurface.GetPitch();
            Pointer<byte> bufferEnd = buffer + depthPitch;

            using (var tex = _yrBufferTextureView.ResourceAs<Texture2D>())
            {
                //DynamicPatcher.Logger.Log("before map");
                //var map = FXGraphic.ImmediateContext.MapSubresource(tex, 0, MapMode.WriteDiscard, MapFlags.None, out var stream);
                //if (map.IsEmpty)
                //{
                //    return;
                //}
                //DynamicPatcher.Logger.Log("after map");

                //Pointer<byte> dst = map.DataPointer;
                //Pointer<byte> src = buffer;

                //for (int y = 0; y < rect.Height; y++)
                //{
                //    //Helpers.Copy(src, dst, rect.Width * 2);
                //    SharpDX.Utilities.CopyMemory(dst, src, rect.Width * 2);
                //    dst += map.RowPitch;
                //    src += rowPitch;
                //}

                //FXGraphic.ImmediateContext.UnmapSubresource(tex, 0);
                DynamicPatcher.Logger.Log("before UpdateSubresource");
                FXGraphic.ImmediateContext.UpdateSubresource(tex, 0, null, buffer, rowPitch, depthPitch);
                DynamicPatcher.Logger.Log("after UpdateSubresource");
            }

            PrimarySurface.Unlock();
        }


        public static IntPtr CopyZBuffer(Pointer<byte> buffer)
        {
            var pZBuffer = ZBuffer.AdjustedGetBufferAt(new Point2D(0, 0));

            int size = (int)(ZBuffer.BufferEndpoint - pZBuffer.Convert<byte>());

            Helpers.Copy(pZBuffer, buffer, size);

            if (size < ZBuffer.BufferSize)
            {
                var pRealZBuffer = pZBuffer + size / 2 - ZBuffer.BufferSize / 2;
                Helpers.Copy(pRealZBuffer, buffer + size, ZBuffer.BufferSize - size);
            }

            return pZBuffer;
        }
        public static IntPtr GetABuffer(short[] aBuffer)
        {
            var pABuffer = ABuffer.AdjustedGetBufferAt(new Point2D(0, 0));

            int size = (int)(ABuffer.BufferEndpoint - pABuffer.Convert<byte>());

            Marshal.Copy(pABuffer, aBuffer, 0, size / 2);

            if (size < ABuffer.BufferSize)
            {
                var pRealABuffer = pABuffer + size / 2 - ABuffer.BufferSize / 2;
                Marshal.Copy(pRealABuffer, aBuffer, size / 2, ZBuffer.BufferSize - size);
            }

            return pABuffer;
        }

        public static string HLSL_zbuffer_vertex =>
            "//render target 1                                                                               " +
            "struct VSOutput                                                                                 " +
            "{                                                                                               " +
            "    vector position : POSITION;                                                                 " +
            "    float3 coords : TEXCOORD1;                                                                  " +
            "    float2 uv : TEXCOORD;                                                                       " +
            "};                                                                                              " +
            "                                                                                                " +
            "VSOutput main(in vector coords : POSITION0, in float2 uv : TEXCOORD0)                           " +
            "{                                                                                               " +
            "    VSOutput output;                                                                            " +
            "                                                                                                " +
            "    output.position = vector((coords.xy - float2(0.5, 0.5)) * float2(2.0, -2.0), 0.0, 1.0);     " +
            "    output.coords = coords.xyz;                                                                 " +
            "    output.uv = uv.yx;                                                                          " +
            "    return output;                                                                              " +
            "}                                                                                               "
            ;

        public static string HLSL_zbuffer_pixel =>
            "Texture2D tex_draw : register(t1);                                                              " +
            "Texture2D tex_zbuffer : register(t0);                                                           " +
            "SamplerState tex_sampler {                                                                      " +
            "    Filter = MIN_MAG_MIP_LINEAR;                                                                " +
            "    AddressU = Wrap;                                                                            " +
            "    AddressV = Wrap;                                                                            " +
            "};                                                                                              " +
            "                                                                                                " +
            "//render target 1                                                                               " +
            "struct VSOutput                                                                                 " +
            "{                                                                                               " +
            "    vector position : POSITION;                                                                 " +
            "    float3 coords : TEXCOORD1;                                                                  " +
            "    float2 uv : TEXCOORD;                                                                       " +
            "};                                                                                              " +
            "                                                                                                " +
            "                                                                                                " +
            "vector main(in VSOutput input) : SV_TARGET                                                      " +
            "{                                                                                               " +
            "    float2 zbuffer_uv = input.coords.xy;                                                        " +
            "    float zvalue = input.coords.z;                                                              " +
            "                                                                                                " +
            "    // vector zcolor = tex2D(tex_zbuffer, zbuffer_uv);                                          " +
            "    vector zcolor = tex_zbuffer.Sample(tex_sampler, zbuffer_uv);                                " +
            "    int rvalue = ceil(zcolor.r * 31) * 32 * 64;                                                 " +
            "    int gvalue = ceil(zcolor.g * 63) * 32;                                                      " +
            "    int bvalue = zcolor.b * 31;                                                                 " +
            "                                                                                                " +
            "    float zbuffer_val = rvalue + gvalue + bvalue;                                               " +
            "    if (zbuffer_val < zvalue)                                                                   " +
            "        discard;                                                                                " +
            "                                                                                                " +
            "    //float u = (input.uv.x - floor(input.uv.x)) * (1 - (input.uv.x - floor(input.uv.x)));      " +
            "    //float v = (input.uv.y - floor(input.uv.y)) * (1 - (input.uv.y - floor(input.uv.y)));      " +
            "    //return vector(saturate((u + v) / 2), 0, 0, 1);                                            " +
            "    return vector(saturate(tex_draw.Sample(tex_sampler, input.uv).rgb), 1.0);                   " +
            "}                                                                                               "
            ;
        
        private static List<SharpDX.Rectangle> rectangles = new List<SharpDX.Rectangle>();
        private static SharpDX.Rectangle ConvertRect(RectangleStruct rect) => new SharpDX.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        public static void LockRect(RectangleStruct rect)
        {
            if(rect.Height == 0 || rect.Width == 0)
            {
                return;
            }

            var _rect = ConvertRect(rect);
            while (true)
            {
                System.Threading.Monitor.Enter(rectangles);
                {
                    if (rectangles.Any(r => r.Intersects(_rect)))
                    {
                        System.Threading.Monitor.Exit(rectangles);
                        Task.Yield();
                        continue;
                    }

                    rectangles.Add(_rect);
                }
                System.Threading.Monitor.Exit(rectangles);
                break;
            }
        }

        public static void UnlockRect(RectangleStruct rect)
        {
            if (rect.Height == 0 || rect.Width == 0)
            {
                return;
            }

            var _rect = ConvertRect(rect);
            lock (rectangles)
            {
                rectangles.Remove(_rect);
            }
        }

        public static unsafe void DrawSHP(Pointer<ConvertClass> Palette, Pointer<SHPStruct> SHP, int frameIdx,
            Point2D pos, RectangleStruct boundingRect, BlitterFlags flags, uint arg7,
            int zAdjust, uint arg9, uint bright, int TintColor, Pointer<SHPStruct> BUILDINGZ_SHA, uint argD, int ZS_X, int ZS_Y)
        {
            //ref var surface = ref Surface.Current.Ref;
            //YRGraphic.LockRect(boundingRect);
            //surface.DrawSHP(Palette, SHP, frameIdx, pos, boundingRect, flags, arg7, zAdjust, arg9, bright, TintColor, BUILDINGZ_SHA, argD, ZS_X, ZS_Y);
            //YRGraphic.UnlockRect(boundingRect);

            drawCells.Enqueue(new SHPDrawCell
            {
                Palette = Palette,
                SHP = SHP,
                FrameIdx = frameIdx,
                Position = pos,
                ClipRect = boundingRect,
                Flags = flags,
                DrawSHP_Arg7 = arg7,
                ZAdjust = zAdjust,
                DrawSHP_Arg9 = arg9,
                Bright = bright,
                TintColor = TintColor,
                BUILDINGZ_SHA = BUILDINGZ_SHA,
                DrawSHP_ArgD = argD,
                ZS_X = ZS_X,
                ZS_Y = ZS_Y
            });
        }

        private static ConcurrentQueue<DrawCell> drawCells = new ConcurrentQueue<DrawCell>();
        private abstract class DrawCell
        {
            public abstract void Draw();
        }
        private class SHPDrawCell : DrawCell
        {
            public Pointer<ConvertClass> Palette;
            public Pointer<SHPStruct> SHP;
            public int FrameIdx;
            public Point2D Position;
            public RectangleStruct ClipRect;
            public BlitterFlags Flags;
            public uint DrawSHP_Arg7;
            public int ZAdjust;
            public uint DrawSHP_Arg9;
            public uint Bright;
            public int TintColor;
            public Pointer<SHPStruct> BUILDINGZ_SHA;
            public uint DrawSHP_ArgD;
            public int ZS_X;
            public int ZS_Y;

            public override void Draw()
            {
                if(ClipRect.X < 0)
                {
                    ClipRect.Width -= ClipRect.X;
                    ClipRect.X = 0;
                }
                if (ClipRect.Y < 0)
                {
                    ClipRect.Height -= ClipRect.Y;
                    ClipRect.Y = 0;
                }
                AdditionalSurface.DrawSHP(Palette, SHP, FrameIdx, Position, ClipRect, Flags, DrawSHP_Arg7, ZAdjust, DrawSHP_Arg9, Bright, TintColor, BUILDINGZ_SHA, DrawSHP_ArgD, ZS_X, ZS_Y);
            }
        }

        private static CancellationTokenSource renderCTS;
        private static Task renderTask;
        public static void RenderLoop()
        {
            //var rect = AdditionalSurface.GetRect();
            //var gameRect = ZBuffer.Rect;
            //AdditionalSurface.BlitPart(gameRect, Surface.Tile, gameRect, false, true);
            //AdditionalSurface.BlitPart(new RectangleStruct(gameRect.Width, rect.Y, rect.Width - gameRect.Width, rect.Height),
            //    Surface.Sidebar, Surface.Sidebar.Ref.GetRect(), false, true);
            //additional.Ref.FillRect(additional.Ref.GetRect(), 0);

            while (renderCTS.IsCancellationRequested == false)
            {
                if (drawCells.TryDequeue(out var drawCell))
                {
                    drawCell.Draw();
                }
                else
                {
                    Console.WriteLine($"dequeue fail, count: {drawCells.Count}");
                }
            }
        }
        public static void BeginDraw()
        {
            renderCTS = new CancellationTokenSource();
            renderTask = new Task(RenderLoop);
            renderTask.Start();
        }
        public static void EndDraw()
        {
            renderCTS.Cancel();
            renderTask.Wait();

            //var rect = additional.Ref.GetRect();
            //Surface.Primary.Ref.BlitPart(rect, additional, rect, false, true);
        }
    }
}




























