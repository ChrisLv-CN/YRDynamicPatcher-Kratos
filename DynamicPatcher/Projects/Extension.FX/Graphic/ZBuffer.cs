using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using D3D11 = SharpDX.Direct3D11;
namespace Extension.FX.Graphic
{
    public static class ZBuffer
    {
        //private static short[] zBuffer;
        private static PatcherYRpp.Pointer<short> _zBuffer;

        private static VertexShader vertexShader;
        private static PixelShader pixelShader;
        private static ShaderSignature inputSignature;
        private static InputLayout inputLayout;
        private static ShaderResourceView zBufferTextureView;


        public static PatcherYRpp.RectangleStruct Rect
        {
            get
            {
                var zBuffer = YRGraphic.ZBuffer;
                return new PatcherYRpp.RectangleStruct(0, 0, zBuffer.Width, zBuffer.Height);
            }
        }

        public static void Initialize(D3D11.Device d3dDevice)
        {
            var rect = Rect;

            //buffer = gpu.Allocate<Color>(rect.Width * rect.Height);
            //zBuffer = gpu.Allocate<short>(rect.Width * rect.Height);

            var pSurface = YRGraphic.ZBuffer.BSurface;
            var zBufferSize = pSurface.Ref.GetPitch() * pSurface.Ref.Height;
            _zBuffer = System.Runtime.InteropServices.Marshal.AllocHGlobal(zBufferSize);

            // Compile the vertex shader code
            using (var vertexShaderByteCode = ShaderBytecode.CompileFromFile(Path.Combine(FXGraphic.ShaderDirectory, "zbuffer.hlsl"), "vmain", "vs_4_0", ShaderFlags.Debug))
            {
                // Read input signature from shader code
                inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);

                vertexShader = new VertexShader(d3dDevice, vertexShaderByteCode);
            }

            // Compile the pixel shader code
            using (var pixelShaderByteCode = ShaderBytecode.CompileFromFile(Path.Combine(FXGraphic.ShaderDirectory, "zbuffer.hlsl"), "pmain", "ps_4_0", ShaderFlags.Debug))
            {
                pixelShader = new PixelShader(d3dDevice, pixelShaderByteCode);
            }


            inputLayout = new InputLayout(d3dDevice, inputSignature, new InputElement[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 12, 0, InputClassification.PerVertexData, 0)
            });

            var desc = new Texture2DDescription
            {
                Width = rect.Width,
                Height = rect.Height,
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B5G6R5_UNorm,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
            };
            var texture = new Texture2D(d3dDevice, desc);

            zBufferTextureView = new ShaderResourceView(d3dDevice, texture);
        }

        public static void Dispose()
        {
            inputSignature?.Dispose();
            vertexShader?.Dispose();
            pixelShader?.Dispose();
            inputLayout?.Dispose();
            zBufferTextureView?.Dispose();

            if(_zBuffer.IsNull == false)
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(_zBuffer);
            }
        }

        public static void FillZTexture()
        {
            YRGraphic.CopyZBuffer(_zBuffer.Convert<byte>());
            var texture = ZBuffer.GetTexture();
            //FXGraphic.ImmediateContext.UpdateSubresource(ref zBuffer[0], texture.Resource, 0);

            ref var surface = ref YRGraphic.ZBuffer.BSurface.Ref;

            int rowPitch = surface.GetWidth() * surface.GetBytesPerPixel();
            int depthPitch = surface.GetHeight() * surface.GetPitch();

            //FXGraphic.ImmediateContext.UpdateSubresource(texture.Resource, 0, null, _zBuffer, rowPitch, depthPitch);

            //using (var surface = texture.QueryInterface<Surface>())
            //{
            //    var map = surface.Map(SharpDX.DXGI.MapFlags.Write);
            //    YRGraphic.CopyZBuffer(map.DataPointer);
            //    surface.Unmap();
            //}
        }

        public static ShaderResourceView GetTexture()
        {
            return zBufferTextureView;
        }
        public static VertexShader GetVertexShader()
        {
            return vertexShader;
        }
        public static PixelShader GetPixelShader()
        {
            return pixelShader;
        }
        public static InputLayout GetInputLayout()
        {
            return inputLayout;
        }
    }
}
