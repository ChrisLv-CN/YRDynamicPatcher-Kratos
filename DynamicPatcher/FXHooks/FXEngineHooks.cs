using DynamicPatcher;
using Extension.FX;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FXHooks
{
#if FX_ENABLE
    [RunClassConstructorFirst]
    public class FXEngineHooks
    {
        [Hook(HookType.AresHook, Address = 0x55B294, Size = 6)]
        public static unsafe UInt32 FXEngine_Update(REGISTERS* R)
        {
            FXEngine.Update();
            return 0;
        }
        [Hook(HookType.AresHook, Address = 0x4F45A3, Size = 5)]
        public static unsafe UInt32 FXEngine_Render(REGISTERS* R)
        {
            FXEngine.Render();
            return 0;
        }

        public static unsafe UInt32 FXEngine_Present(REGISTERS* R)
        {
            FXEngine.Present();
            return 0;
        }

#if FX_CNCDDRAW
        [DllImport("d3d9.dll")]
        extern public static IntPtr Direct3DCreate9(uint SDKVersion);

        delegate UInt32 IDirect3D9_CreateDeviceDelegate(IntPtr pD3D, uint Adapter, uint DeviceType, uint hFocusWindow, uint BehaviorFlags, IntPtr pPresentationParameters, IntPtr ppReturnedDeviceInterface);
        static IntPtr IDirect3D9_CreateDevice;
        static IDirect3D9_CreateDeviceDelegate myIDirect3D9_CreateDevice = D3D9_IDirect3D9_CreateDevice;

        delegate UInt32 IDirect3DDevice9_PresentDelegate(IntPtr pDevice, IntPtr pSourceRect, IntPtr pDestRect, uint hDestWindowOverride, IntPtr pDirtyRegion);
        static IntPtr IDirect3DDevice9_Present;
        static IDirect3DDevice9_PresentDelegate myIDirect3DDevice9_Present = D3D9_IDirect3DDevice9_Present;

        // get d3d9.dll's function address before export table hook.
        static FXEngineHooks()
        {
            Logger.LogWarning("experimental feature FX is working.");
            Logger.LogWarning("FX need CNC-DDRAW and D3D9 && D3D11 installed.");

            var type = typeof(FXEngineHooks);
            System.Runtime.CompilerServices.RuntimeHelpers.PrepareMethod(type.GetMethod(nameof(Direct3DCreate9)).MethodHandle);
        }

        [Hook(HookType.ExportTableHook, "d3d9.dll", TargetName = "Direct3DCreate9")]
        public static IntPtr D3D9_Direct3DCreate9(uint SDKVersion)
        {
            Pointer<IntPtr> pDirect9 = Direct3DCreate9(SDKVersion);
            Pointer<IntPtr> pDirect9Impl = pDirect9[0];
            System.Threading.Interlocked.CompareExchange(ref IDirect3D9_CreateDevice, pDirect9Impl[16], IntPtr.Zero);
            MemoryHelper.Write((int)(pDirect9Impl + 16), Marshal.GetFunctionPointerForDelegate(myIDirect3D9_CreateDevice));

            return pDirect9;
        }

        public static unsafe UInt32 D3D9_IDirect3D9_CreateDevice(IntPtr pD3D, uint Adapter, uint DeviceType, uint hFocusWindow, uint BehaviorFlags, IntPtr pPresentationParameters, IntPtr ppReturnedDeviceInterface)
        {
            var func = (delegate* unmanaged[Stdcall]<IntPtr, uint, uint, uint, uint, IntPtr, IntPtr, uint>)IDirect3D9_CreateDevice;
            var ret = func(pD3D, Adapter, DeviceType, hFocusWindow, BehaviorFlags, pPresentationParameters, ppReturnedDeviceInterface);

            Pointer<IntPtr> pDevice = ppReturnedDeviceInterface.Convert<IntPtr>().Data;
            Pointer<IntPtr> pDeviceImpl = pDevice[0];
            System.Threading.Interlocked.CompareExchange(ref IDirect3DDevice9_Present, pDeviceImpl[17], IntPtr.Zero);
            MemoryHelper.Write((int)(pDeviceImpl + 17), Marshal.GetFunctionPointerForDelegate(myIDirect3DDevice9_Present));

            return ret;
        }

        public static unsafe UInt32 D3D9_IDirect3DDevice9_Present(IntPtr pDevice, IntPtr pSourceRect, IntPtr pDestRect, uint hDestWindowOverride, IntPtr pDirtyRegion)
        {
            var func = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, uint, IntPtr, uint>)IDirect3DDevice9_Present;
            uint ret = 0;

            //if (FXEngine.WorkSystems.Any() && FXEngine.Rendered)
            //{
            //    FXEngine.WorkListRWLock.EnterWriteLock();
            //    ret = (uint)FXEngine.Present();
            //    FXEngine.WorkListRWLock.ExitWriteLock();
            //}
            //else
            //{
            //    ret = func(pDevice, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion);
            //}

            //if (FXEngine.WorkSystems.Any() && FXEngine.Rendered)
            //{
            //    FXEngine.WorkListRWLock.EnterWriteLock();
            //    FXEngine.OnPresent(pDevice);
            //    FXEngine.WorkListRWLock.ExitWriteLock();
            //}

            //ret = func(pDevice, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion);

            //return ret;

            if (FXEngine.Rendered)
            {
                return 0;
            }

            //if (FXEngine.WorkSystems.Any())
            //{
            //    FXEngine.WorkListRWLock.EnterWriteLock();
            //    FXEngine.OnPresent(pDevice);
            //    FXEngine.WorkListRWLock.ExitWriteLock();
            //    return 0;
            //}

            return func(pDevice, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion);
        }
#endif
    }
#endif
}
