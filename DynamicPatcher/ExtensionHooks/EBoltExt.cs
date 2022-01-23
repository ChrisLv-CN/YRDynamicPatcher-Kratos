
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Utilities;

namespace ExtensionHooks
{
    public class EBoltExtHooks
    {
        [Hook(HookType.AresHook, Address = 0x4C1E42, Size = 5)]
        public static unsafe UInt32 EBolt_CTOR(REGISTERS* R)
        {
            Pointer<EBolt> pEBolt = (IntPtr)R->EAX;
            EBoltExt.ExtMap.FindOrAllocate(pEBolt);
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x4C2951, Size = 5)]
        public static unsafe UInt32 EBolt_DTOR(REGISTERS* R)
        {
            Pointer<EBolt> pEBolt = (IntPtr)R->ECX;
            EBoltExt.ExtMap.Remove(pEBolt);
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x4C24BE, Size = 5)]
        public static unsafe UInt32 EBolt_Draw_Color1(REGISTERS* R)
        {
            Pointer<EBolt> pEBolt = R->Stack<IntPtr>(0x40);
            EBoltExt ext = EBoltExt.ExtMap.Find(pEBolt);
            if (null != ext && ext.Color1 != default)
            {
                R->EAX = (uint)Drawing.Color16bit(ext.Color1);
                return 0x4C24E4;
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x4C25CB, Size = 5)]
        public static unsafe UInt32 EBolt_Draw_Color2(REGISTERS* R)
        {
            Pointer<EBolt> pEBolt = R->Stack<IntPtr>(0x40);
            EBoltExt ext = EBoltExt.ExtMap.Find(pEBolt);
            if (null != ext && ext.Color2 != default)
            {
                R->Stack<int>(0x18, Drawing.Color16bit(ext.Color2));
                return 0x4C25FD;
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x4C26C7, Size = 5)]
        public static unsafe UInt32 EBolt_Draw_Color3(REGISTERS* R)
        {
            Pointer<EBolt> pEBolt = R->Stack<IntPtr>(0x40);
            EBoltExt ext = EBoltExt.ExtMap.Find(pEBolt);
            if (null != ext && ext.Color3 != default)
            {
                R->EBX = R->EBX - 2;
                R->EAX = (uint)Drawing.Color16bit(ext.Color3);
                return 0x4C26EE;
            }
            return 0;
        }

        // [Hook(HookType.AresHook, Address = 0x4C2AFF, Size = 5)]
        // public static unsafe UInt32 EBolt_Fire_ParticleSystem(REGISTERS* R)
        // {
        //     Pointer<EBolt> pEBolt = (IntPtr)R->ESI;
        //     EBoltExt ext = EBoltExt.ExtMap.Find(pEBolt);
        //     Logger.Log("1 pEBolt {0}, ext {1}, R->EAX = {2}", pEBolt, ext == null ? "is null" : "not null", R->EAX);
        //     // if (null != ext)
        //     // {
        //     //     BulletEffectHelper.BlueLine(pEBolt.Ref.Point1, pEBolt.Ref.Point2, 1 ,75);
        //     //     return 0x4C2B35;
        //     // }
        //     return 0;
        // }


        // [Hook(HookType.AresHook, Address = 0x4C2B09, Size = 5)]
        // public static unsafe UInt32 EBolt_Fire_ParticleSystem2(REGISTERS* R)
        // {
        //     Pointer<EBolt> pEBolt = (IntPtr)R->EAX;
        //     EBoltExt ext = EBoltExt.ExtMap.Find(pEBolt);
        //     Logger.Log("2 pEBolt {0}, ext {1}, R->EAX = {2}", pEBolt, ext == null ? "is null" : "not null", R->EAX);
        //     // if (null != ext)
        //     // {
        //     //     BulletEffectHelper.BlueLine(pEBolt.Ref.Point1, pEBolt.Ref.Point2, 1 ,75);
        //     //     return 0x4C2B35;
        //     // }
        //     return 0x4C2B35;
        // }


        // [Hook(HookType.AresHook, Address = 0x62DC6A, Size = 6)]
        // public static unsafe UInt32 ParticleSystem_DTCR(REGISTERS* R)
        // {
        //     Logger.Log("OOXX {0}", R->EAX);
        //     // if (null != ext)
        //     // {
        //     //     BulletEffectHelper.BlueLine(pEBolt.Ref.Point1, pEBolt.Ref.Point2, 1 ,75);
        //     //     return 0x4C2B35;
        //     // }
        //     return 0;
        // }

        


        [Hook(HookType.AresHook, Address = 0x4C24E4, Size = 0x8)]
        public static unsafe UInt32 Ebolt_Draw_Disable1(REGISTERS* R)
        {
            Pointer<EBolt> pEBolt = R->Stack<IntPtr>(0x40);
            EBoltExt ext = EBoltExt.ExtMap.Find(pEBolt);
            if (null != ext && ext.Disable1)
            {
                return 0x4C2515;
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x4C25FD, Size = 0xA)]
        public static unsafe UInt32 Ebolt_Draw_Disable2(REGISTERS* R)
        {
            Pointer<EBolt> pEBolt = R->Stack<IntPtr>(0x40);
            EBoltExt ext = EBoltExt.ExtMap.Find(pEBolt);
            if (null != ext && ext.Disable2)
            {
                return 0x4C262A;
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x4C26EE, Size = 0x8)]
        public static unsafe UInt32 Ebolt_Draw_Disable3(REGISTERS* R)
        {
            Pointer<EBolt> pEBolt = R->Stack<IntPtr>(0x40);
            EBoltExt ext = EBoltExt.ExtMap.Find(pEBolt);
            if (null != ext && ext.Disable3)
            {
                return 0x4C2710;
            }
            return 0;
        }


    }
}