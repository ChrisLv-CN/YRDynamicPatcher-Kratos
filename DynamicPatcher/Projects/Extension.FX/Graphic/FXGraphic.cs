//using Alea;
using Extension.FX.Definitions;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using D3D11 = SharpDX.Direct3D11;
using Color = Extension.FX.Definitions.Color;
using SharpDX.D3DCompiler;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;

namespace Extension.FX.Graphic
{
    class FXGraphic
    {
        public static bool RenderToNewWindow { get; set; } = false;
        public static string GraphicDirectory { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "Graphic");
        public static string ShaderDirectory => Path.Combine(GraphicDirectory, "Shaders");
        public static string TextureDirectory => Path.Combine(GraphicDirectory, "Textures");

        //private static Gpu gpu = Gpu.Default;
        private const int zBufferSlot = 0;
        private const int texSlot = 1;

        private static DeviceContext d3dImmediateContext;
        private static SwapChain swapChain;
        private static D3D11.Device d3dDevice;
        private static Texture2D _backBuffer;
        private static Texture2D _renderBuffer;
        private static RenderTargetView _renderView;
        private static DepthStencilView _depthView;
        private static D3D11.Buffer _surfaceSizeBuffer;
        private static D3D11.Buffer _zbufferSizeBuffer;
        private static BlendState _blendState;
        private static RasterizerState _rasterizerState;
        private static DepthStencilState _depthStencilState;


        private static Dictionary<string, ShaderResourceView> textures = new Dictionary<string, ShaderResourceView>();
        private static BlockingCollection<DeviceContext> deferredContexts = new BlockingCollection<DeviceContext>();
        private static ConcurrentQueue<CommandList> commandLists = new ConcurrentQueue<CommandList>();

        public static DeviceContext ImmediateContext => d3dImmediateContext;
        public static Texture2D BackBuffer => _backBuffer;
        //public static Texture2D BackBuffer => _renderBuffer;

        public static void Initialize()
        {
            InitializeDevice();
            ZBuffer.Initialize(d3dDevice);
            YRGraphic.Initialize(d3dDevice);
            textures[YRGraphic.PrimaryBufferTextureName] = YRGraphic.BufferTextureView;
        }

        public static void Dispose()
        {
            ZBuffer.Dispose();
            YRGraphic.Dispose();

            Array.ForEach(textures.ToArray(), p => p.Value.Dispose());
            textures.Clear();

            while (deferredContexts.TryTake(out var deviceContext))
            {
                deviceContext.Dispose();
            }
            while (commandLists.TryDequeue(out var commandList))
            {
                commandList.Dispose();
            }

            _depthStencilState?.Dispose();
            _rasterizerState?.Dispose();
            _blendState?.Dispose();
            _zbufferSizeBuffer?.Dispose();
            _surfaceSizeBuffer?.Dispose();
            _depthView?.Dispose();
            _renderView?.Dispose();
            _backBuffer?.Dispose();
            _renderBuffer?.Dispose();
            d3dDevice?.Dispose();
            swapChain?.Dispose();
            d3dImmediateContext?.Dispose();
            d3dImmediateContext = null;
        }

        private static void InitializeDevice()
        {
            var rect = YRGraphic.SurfaceRect;

            //var factory2 = new Factory2();
            //d3dDevice = new D3D11.Device(DriverType.Hardware, DeviceCreationFlags.None);
            //var swapChainDescription1 = new SwapChainDescription1()
            //{
            //    Width = rect.Width,
            //    Height = rect.Height,
            //    Format = Format.R8G8B8A8_UNorm,
            //    Stereo = false,
            //    SampleDescription = new SampleDescription(1, 0),
            //    Usage = Usage.RenderTargetOutput,
            //    BufferCount = 1,
            //    Scaling = Scaling.Stretch,
            //    AlphaMode = AlphaMode.Ignore,
            //    SwapEffect = SwapEffect.Discard,
            //    Flags = SwapChainFlags.None
            //};
            //swapChain = new SwapChain1(factory2, d3dDevice, GetWindowHandle(), ref swapChainDescription1);

            SwapChainDescription swapChainDesc = new SwapChainDescription()
            {
                ModeDescription = new ModeDescription(rect.Width, rect.Height, new Rational(0, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Usage = Usage.RenderTargetOutput,
                BufferCount = 1,
                OutputHandle = GetWindowHandle(),
                IsWindowed = true,
                SwapEffect = SwapEffect.Discard
            };

            D3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapChainDesc, out d3dDevice, out swapChain);
            d3dImmediateContext = d3dDevice.ImmediateContext;

            var factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(swapChainDesc.OutputHandle, WindowAssociationFlags.IgnoreAll);

            // Create render target view for texture
            _backBuffer = swapChain.GetBackBuffer<Texture2D>(0);

            _renderBuffer = new Texture2D(d3dDevice, new Texture2DDescription()
            {
                Format = Format.R8G8B8A8_UNorm,
                ArraySize = 1,
                MipLevels = 1,
                Width = rect.Width,
                Height = rect.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.Shared
            });

            _renderView = new RenderTargetView(d3dDevice, _backBuffer);
            //_renderView = new RenderTargetView(d3dDevice, _renderBuffer);

            // Create Depth Buffer & View 
            var depthBuffer = new Texture2D(d3dDevice, new Texture2DDescription()
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = rect.Width,
                Height = rect.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            _depthView = new DepthStencilView(d3dDevice, depthBuffer);

            var blendStateDesc = new BlendStateDescription();
            blendStateDesc.AlphaToCoverageEnable = false;
            blendStateDesc.IndependentBlendEnable = false;
            blendStateDesc.RenderTarget[0].IsBlendEnabled = true;
            blendStateDesc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            blendStateDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            blendStateDesc.RenderTarget[0].BlendOperation = BlendOperation.Add;
            blendStateDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
            blendStateDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
            blendStateDesc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
            blendStateDesc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

            _blendState = new BlendState(d3dDevice, blendStateDesc);

            _rasterizerState = new RasterizerState(d3dDevice, new RasterizerStateDescription()
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.Back,
                IsFrontCounterClockwise = true,
                DepthBias = 0,
                DepthBiasClamp = 0,
                SlopeScaledDepthBias = 0,
                IsDepthClipEnabled = true,
                IsScissorEnabled = false,
                IsMultisampleEnabled = true,
                IsAntialiasedLineEnabled = true
            });

            var depthStencilStateDesc = new DepthStencilStateDescription()
            {
                IsDepthEnabled = false,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Less,
                IsStencilEnabled = true,
                StencilReadMask = 0xFF,
                StencilWriteMask = 0xFF,
                // Stencil operation if pixel front-facing.
                FrontFace = new DepthStencilOperationDescription()
                {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Increment,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Always
                },
                // Stencil operation if pixel is back-facing.
                BackFace = new DepthStencilOperationDescription()
                {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Decrement,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Always
                }
            };

            // Create the depth stencil state.
            _depthStencilState = new DepthStencilState(d3dDevice, depthStencilStateDesc);

            for (int idx = 0; idx < 16; idx++)
            {
                var context = new DeviceContext(d3dDevice);
                deferredContexts.Add(context);
            }

            _surfaceSizeBuffer = new D3D11.Buffer(d3dDevice, Marshal.SizeOf<SharpDX.Vector4>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            var surfaceSize = new SharpDX.Vector2(rect.Width, rect.Height);
            d3dImmediateContext.UpdateSubresource(ref surfaceSize, _surfaceSizeBuffer);

            _zbufferSizeBuffer = new D3D11.Buffer(d3dDevice, Marshal.SizeOf<SharpDX.Vector4>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            var zbufferSize = new SharpDX.Vector2(ZBuffer.Rect.Width, ZBuffer.Rect.Height);
            d3dImmediateContext.UpdateSubresource(ref zbufferSize, _zbufferSizeBuffer);
        }

        private static IntPtr GetWindowHandle()
        {
            if (RenderToNewWindow)
            {
                return CreateWindow();
            }

            return YRGraphic.WindowHandle;
        }

        private static IntPtr CreateWindow()
        {
            var form = new System.Windows.Forms.Form();
            form.Text = "FX Draw Buffer";
            form.Show();
            return form.Handle;
        }

        public static D3D11.Buffer CreateBuffer<T>(BindFlags bindFlags, T[] data, int sizeInBytes = 0, ResourceUsage usage = ResourceUsage.Default, CpuAccessFlags accessFlags = CpuAccessFlags.None, ResourceOptionFlags optionFlags = ResourceOptionFlags.None, int structureByteStride = 0) where T : struct
        {
            return D3D11.Buffer.Create(d3dDevice, bindFlags, data, sizeInBytes, usage, accessFlags, optionFlags, structureByteStride);
        }
        public static ShaderResourceView GetTexture(string filePath)
        {
            lock (textures)
            {
                if (textures.TryGetValue(filePath, out var texture))
                {
                    return texture;
                }

                var texture2D = ResourceLoader.CreateTexture2DFromFile(d3dDevice, Path.Combine(TextureDirectory, filePath));
                textures[filePath] = texture = new ShaderResourceView(d3dDevice, texture2D);
                return texture;
            }
        }


        public static void DrawTexture(ShaderResourceView texture, Definitions.Vector3 clientPosition, Definitions.Vector2 clientRotaion, Definitions.Vector2 clientScale)
        {

        }

        private static void SetupPipeline(DeviceContext renderingContext)
        {
            var rect = YRGraphic.SurfaceRect;
            var zBufferInputLayout = ZBuffer.GetInputLayout();
            var zBufferTexture = ZBuffer.GetTexture();
            var zBufferVertexShader = ZBuffer.GetVertexShader();
            var zBufferPixelShader = ZBuffer.GetPixelShader();

            // Prepare All the stages 
            renderingContext.InputAssembler.InputLayout = zBufferInputLayout;
            renderingContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            renderingContext.VertexShader.Set(zBufferVertexShader);
            renderingContext.VertexShader.SetConstantBuffer(0, _surfaceSizeBuffer);
            renderingContext.VertexShader.SetConstantBuffer(1, _zbufferSizeBuffer);

            renderingContext.Rasterizer.SetViewport(0, 0, rect.Width, rect.Height);
            //renderingContext.Rasterizer.State = _rasterizerState;

            renderingContext.PixelShader.Set(zBufferPixelShader);
            renderingContext.PixelShader.SetShaderResource(zBufferSlot, zBufferTexture);

            renderingContext.OutputMerger.SetTargets(_depthView, _renderView);
            renderingContext.OutputMerger.SetBlendState(_blendState);
            renderingContext.OutputMerger.SetDepthStencilState(_depthStencilState);
        }

        #region ImmediateContextLock
        //SharpDX.Windows.RenderLoop renderLoop = new SharpDX.Windows.RenderLoop();
        //private static CancellationTokenSource renderCTS;
        //private static Task renderTask;

        //public struct ImmediateContextLocker : IDisposable
        //{
        //    private static object locker = new object();
        //    private static int useCount = 0;
        //    private static ReaderWriterLockSlim _RWLock = new ReaderWriterLockSlim();

        //    public ImmediateContextLocker(bool drawThread)
        //    {
        //        _drawThread = drawThread;
        //    }

        //    public void Lock()
        //    {
        //        //while (Interlocked.CompareExchange(ref useCount, 1, 0) == 1)
        //        //{
        //        //}
        //        //Monitor.Enter(locker);
        //        if (_drawThread)
        //        {
        //            _RWLock.EnterWriteLock();
        //        }
        //        else
        //        {
        //            _RWLock.EnterUpgradeableReadLock();
        //        }
        //    }

        //    public void Dispose()
        //    {
        //        //if (Monitor.IsEntered(locker))
        //        //{
        //        //    Monitor.Exit(locker);
        //        //}
        //        //Interlocked.Decrement(ref useCount);
        //        if (_drawThread)
        //        {
        //            _RWLock.ExitWriteLock();
        //        }
        //        else
        //        {
        //            _RWLock.ExitUpgradeableReadLock();
        //        }
        //    }

        //    private bool _drawThread;
        //}
        //public static ImmediateContextLocker GetImmediateContextLock(bool drawThread = false)
        //{
        //    var locker = new ImmediateContextLocker(drawThread);
        //    locker.Lock();
        //    return locker;
        //}

        //public static void BeginDraw()
        //{
        //    renderCTS = new CancellationTokenSource();
        //    renderTask = new Task(RenderLoop);
        //    renderTask.Start();
        //}

        //public static void RenderLoop()
        //{
        //    d3dImmediateContext.ClearDepthStencilView(_depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
        //    d3dImmediateContext.ClearRenderTargetView(_renderView, SharpDX.Color.Black);

        //    SetupPipeline(d3dImmediateContext);

        //    while (renderCTS.IsCancellationRequested == false)
        //    {
        //        if (commandLists.TryDequeue(out var commandList))
        //        {
        //            using (GetImmediateContextLock(true))
        //            {
        //                d3dImmediateContext.ExecuteCommandList(commandList, false);
        //            }

        //            commandList.Dispose();
        //        }
        //        else
        //        {
        //            Thread.Sleep(10);
        //        }
        //    }

        //    d3dImmediateContext.OutputMerger.SetBlendState(null);
        //}

        //public static void EndDraw()
        //{
        //    renderCTS.Cancel();
        //    renderTask.Wait();
        //    Present();
        //}

        #endregion


        public static void Render()
        {
            d3dImmediateContext.ClearDepthStencilView(_depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            d3dImmediateContext.ClearRenderTargetView(_renderView, SharpDX.Color.Black);

            SetupPipeline(d3dImmediateContext);

            while (commandLists.TryDequeue(out var commandList))
            {
                d3dImmediateContext.ExecuteCommandList(commandList, false);

                commandList.Dispose();
            }
        }

        public static Result Present()
        {
            // Swap front and back buffer
            return swapChain.Present(0, PresentFlags.None);
            //return (swapChain as SwapChain1).Present(0, PresentFlags.DoNotWait, new PresentParameters());
        }

        private static DeviceContext TakeDeferredContext()
        {
            //if(deferredContexts.TryTake(out var deviceContext))
            //{
            //    return deviceContext;
            //}

            //deviceContext = new DeviceContext(d3dDevice);
            //return deviceContext;

            return deferredContexts.Take();
        }



        public static void DrawObject(FXDrawObject drawObject)
        {
            var deviceContext = TakeDeferredContext();

            drawObject.SetBuffer();

            SetupPipeline(deviceContext);

            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(drawObject.VertexBuffer, Marshal.SizeOf<VS_INPUT>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(drawObject.IndexBuffer, Format.R32_UInt, 0);

            deviceContext.PixelShader.SetShaderResource(texSlot, drawObject.TextureView);

            deviceContext.DrawIndexed(drawObject.IndexCount, 0, 0);

            var commandList = deviceContext.FinishCommandList(false);

            deferredContexts.Add(deviceContext);

            commandLists.Enqueue(commandList);
        }
    }
}
