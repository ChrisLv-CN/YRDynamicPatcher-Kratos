using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Graphic
{
    public static class D3D9
    {
        private static bool _inited = false;
        private static VertexBuffer vertices;
        private static Effect effect;
        private static VertexDeclaration vertexDecl;
        private static Texture additionalTexture;
        private static Texture sharedTexture;

        public static void Initialize(Device device)
        {
            vertices = new VertexBuffer(device, Marshal.SizeOf<Vector4>() * 2 * 4, Usage.None, VertexFormat.None, Pool.Managed);
            vertices.Lock(0, 0, LockFlags.None).WriteRange(
                new[]
                {
                    new Vector4(-1.0f,  1.0f, 0.0f, 0.0f), new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
                    new Vector4( 1.0f,  1.0f, 0.0f, 0.0f), new Vector4(1.0f, 0.0f, 0.0f, 0.0f),
                    new Vector4(-1.0f, -1.0f, 0.0f, 0.0f), new Vector4(0.0f, 1.0f, 0.0f, 0.0f),
                    new Vector4( 1.0f, -1.0f, 0.0f, 0.0f), new Vector4(1.0f, 1.0f, 0.0f, 0.0f)

                });
            vertices.Unlock();

            effect = Effect.FromString(device, D3D9_shader, ShaderFlags.None);

            VertexElement[] vertexElems = new[] {
                new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                new VertexElement(0, 16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                VertexElement.VertexDeclarationEnd
            };

            // Creates and sets the Vertex Declaration
            vertexDecl = new VertexDeclaration(device, vertexElems);

            var rect = YRGraphic.SurfaceRect;
            additionalTexture = new Texture(device, rect.Width, rect.Height, 1, Usage.Dynamic, Format.A8B8G8R8, Pool.Default);

            //using (var res = FXGraphic.BackBuffer.QueryInterface<SharpDX.DXGI.Resource>())
            //{
            //    var sharedHandle = res.SharedHandle;
            //    sharedTexture = new Texture(device, rect.Width, rect.Height, 1, Usage.RenderTarget , Format.A8B8G8R8, Pool.Default, ref sharedHandle);
            //}
        }
        public static void Dispose()
        {
            vertices?.Dispose();
            effect?.Dispose();
            vertexDecl?.Dispose();
            additionalTexture?.Dispose();
            sharedTexture?.Dispose();
        }
        public static void CopyBuffer()
        {
            var rect = YRGraphic.SurfaceRect;

            var map9 = additionalTexture.LockRectangle(0, LockFlags.None);
            var map11 = FXGraphic.ImmediateContext.MapSubresource(FXGraphic.BackBuffer, 0, SharpDX.Direct3D11.MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

            PatcherYRpp.Pointer<byte> dst = map9.DataPointer;
            PatcherYRpp.Pointer<byte> src = map11.DataPointer;

            for (int y = 0; y < rect.Height; y++)
            {
                SharpDX.Utilities.CopyMemory(dst, src, rect.Width * 4);
                dst += map9.Pitch;
                src += map11.RowPitch;
            }

            FXGraphic.ImmediateContext.UnmapSubresource(FXGraphic.BackBuffer, 0);
            additionalTexture.UnlockRectangle(0);
        }
        public static void CopyBuffer(Device device)
        {
            var rect = YRGraphic.SurfaceRect;

            var surface = Surface.CreateOffscreenPlain(device, rect.Width, rect.Height, Format.R5G6B5, Pool.SystemMemory);
            device.GetRenderTargetData(device.GetBackBuffer(0, 0), surface);

            var map9 = surface.LockRectangle(LockFlags.ReadOnly);
            var map11 = FXGraphic.ImmediateContext.MapSubresource(YRGraphic.BufferTextureView.Resource, 0, SharpDX.Direct3D11.MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None);

            PatcherYRpp.Pointer<byte> dst = map11.DataPointer;
            PatcherYRpp.Pointer<byte> src = map9.DataPointer;

            for (int y = 0; y < rect.Height; y++)
            {
                SharpDX.Utilities.CopyMemory(dst, src, rect.Width * 2);
                dst += map11.RowPitch;
                src += map9.Pitch;
            }

            FXGraphic.ImmediateContext.UnmapSubresource(YRGraphic.BufferTextureView.Resource, 0);
            surface.UnlockRectangle();
            surface.Dispose();
        }
        public static void OnPresent(Device device)
        {
            //if (!_inited)
            //{
            //    Initialize(device);
            //    _inited = true;
            //}

            //device.SetStreamSource(0, vertices, 0, Marshal.SizeOf<Vector4>() * 2);
            //device.VertexDeclaration = vertexDecl;
            ////device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            ////device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);

            //CopyBuffer();

            //device.BeginScene();
            //effect.Begin();

            //device.SetTexture(0, additionalTexture);
            //device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

            //effect.End();
            //device.EndScene();

            CopyBuffer(device);
        }

        public static string D3D9_shader =>
            "sampler additional_sampler : register(s0);                     " +
            "                                                               " +
            "struct VS_IN                                                   " +
            "{                                                              " +
            "    float4 pos : POSITION;                                     " +
            "	float4 uv : TEXCOORD;                                       " +
            "};                                                             " +
            "                                                               " +
            "struct PS_IN                                                   " +
            "{                                                              " +
            "    float4 pos : POSITION;                                     " +
            "	float4 uv : TEXCOORD;                                       " +
            "};                                                             " +
            "                                                               " +
            "PS_IN VS(VS_IN input)                                          " +
            "{                                                              " +
            "    PS_IN output = (PS_IN)0;                                   " +
            "                                                               " +
            "    output.pos = input.pos;                                    " +
            "    output.uv = input.uv;                                      " +
            "                                                               " +
            "    return output;                                             " +
            "}                                                              " +
            "                                                               " +
            "float4 PS(PS_IN input) : COLOR                                 " +
            "{                                                              " +
            "    return tex2D(additional_sampler, input.uv).rgba;           " +
            "}                                                              " +
            "                                                               " +
            "technique Main                                                 " +
            "{                                                              " +
            "    pass P0                                                    " +
            "    {                                                          " +
            "        VertexShader = compile vs_3_0 VS();                    " +
            "        PixelShader = compile ps_3_0 PS();                     " +
            "    }                                                          " +
            "}                                                              " +
            "                                                               " +
            "                                                               "
            ;
    }
}





































