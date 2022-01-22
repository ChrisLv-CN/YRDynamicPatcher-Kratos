using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Graphic
{
    public class ResourceLoader
    {
        public static BitmapSource LoadBitmap(string filePath)
        {
            ImagingFactory factory = new ImagingFactory();

            var bitmapDecoder = new BitmapDecoder(
                factory,
                filePath,
                DecodeOptions.CacheOnDemand
                );

            var formatConverter = new FormatConverter(factory);

            formatConverter.Initialize(
                bitmapDecoder.GetFrame(0),
                PixelFormat.Format32bppBGRA,
                BitmapDitherType.None,
                null,
                0.0,
                BitmapPaletteType.Custom);

            return formatConverter;
        }

        public static Texture2D CreateTexture2DFromFile(Device device, string filePath)
        {
            BitmapSource bitmapSource = LoadBitmap(filePath);
            // Allocate DataStream to receive the WIC image pixels
            int stride = bitmapSource.Size.Width * 4;
            using (var buffer = new DataStream(bitmapSource.Size.Height * stride, true, true))
            {
                // Copy the content of the WIC to the buffer
                bitmapSource.CopyPixels(stride, buffer);
                return new Texture2D(device, new Texture2DDescription()
                {
                    Width = bitmapSource.Size.Width,
                    Height = bitmapSource.Size.Height,
                    ArraySize = 1,
                    BindFlags = BindFlags.ShaderResource,
                    Usage = ResourceUsage.Immutable,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                }, new DataRectangle(buffer.DataPointer, stride));
            }
        }
    }

}
