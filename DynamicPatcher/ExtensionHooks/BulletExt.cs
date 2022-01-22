
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;

namespace ExtensionHooks
{
    public class BulletExtHooks
    {
        [Hook(HookType.AresHook, Address = 0x4664BA, Size = 5)]
        public static unsafe UInt32 BulletClass_CTOR(REGISTERS* R)
        {
            return BulletExt.BulletClass_CTOR(R);
        }

        [Hook(HookType.AresHook, Address = 0x4665E9, Size = 0xA)]
        public static unsafe UInt32 BulletClass_DTOR(REGISTERS* R)
        {
            return BulletExt.BulletClass_DTOR(R);
        }

        [Hook(HookType.AresHook, Address = 0x46AFB0, Size = 8)]
        [Hook(HookType.AresHook, Address = 0x46AE70, Size = 5)]
        public static unsafe UInt32 BulletClass_SaveLoad_Prefix(REGISTERS* R)
        {
            return BulletExt.BulletClass_SaveLoad_Prefix(R);
        }

        [Hook(HookType.AresHook, Address = 0x46AF97, Size = 7)]
        [Hook(HookType.AresHook, Address = 0x46AF9E, Size = 7)]
        public static unsafe UInt32 BulletClass_Load_Suffix(REGISTERS* R)
        {
            return BulletExt.BulletClass_Load_Suffix(R);
        }

        [Hook(HookType.AresHook, Address = 0x46AFC4, Size = 5)]
        public static unsafe UInt32 BulletClass_Save_Suffix(REGISTERS* R)
        {
            return BulletExt.BulletClass_Save_Suffix(R);
        }

        [Hook(HookType.AresHook, Address = 0x466556, Size = 6)]
        public static unsafe UInt32 BulletClass_Init(REGISTERS* R)
        {
            try
            {
                Pointer<BulletClass> pBullet = (IntPtr)R->ECX;
                BulletExt ext = BulletExt.ExtMap.Find(pBullet);
                // Logger.Log("BulletExt init {0}", ext == null?"Ext is null":"is ready.");
                ext?.OnInit();
                ext?.Scriptable?.OnInit();
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x468B5D, Size = 6)]
        public static unsafe UInt32 BulletClass_Put(REGISTERS* R)
        {
            try
            {
                Pointer<BulletClass> pBullet = (IntPtr)R->EBX;
                BulletExt ext = BulletExt.ExtMap.Find(pBullet);
                Pointer<CoordStruct> pCoord = R->Stack<IntPtr>(-0x20);
                // Logger.Log("BulletExt init {0} {1}", ext == null?"Ext is null":"is ready.", pCoord.Data);
                ext?.OnPut(pCoord);
                ext?.Scriptable?.OnPut(pCoord, default);
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x4666F7, Size = 6)]
        public static unsafe UInt32 BulletClass_Update(REGISTERS* R)
        {
            try
            {
                Pointer<BulletClass> pBullet = (IntPtr)R->EBP;
                BulletExt ext = BulletExt.ExtMap.Find(pBullet);
                ext?.OnUpdate();
                ext?.Scriptable?.OnUpdate();
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return (uint)0;
        }

        // 重新赋值被修改过的导弹类的速度
        // [Hook(HookType.AresHook, Address = 0x466D26, Size = 7)]
        // public static unsafe UInt32 BulletClass_Update_ChangeVelocity1(REGISTERS* R)
        // {
        //     try
        //     {
        //         Pointer<BulletClass> pBullet = (IntPtr)R->EBP;
        //         Pointer<BulletVelocity> pVelocity = R->lea_Stack<IntPtr>(0x0A8);
        //         BulletVelocity velocity = pBullet.Ref.Velocity;
        //         Logger.Log("1当前车速 {0} - {1}, {2}", pBullet.Ref.Speed, velocity, pVelocity.IsNull ? "null" : pVelocity.Data);
        //         pVelocity.Ref.X = velocity.X;
        //         pVelocity.Ref.Y = velocity.Y;
        //         pVelocity.Ref.Z = velocity.Z;
        //     }
        //     catch (Exception e)
        //     {
        //         Logger.PrintException(e);
        //     }
        //     return (uint)0;
        // }

        // 跳过垂直抛射体重新计算向量
        // [Hook(HookType.AresHook, Address = 0x467429, Size = 7)]
        // public static unsafe UInt32 BulletClass_Update_ChangeVelocity2z(REGISTERS* R)
        // {
        //     try
        //     {
        //         Pointer<BulletClass> pBullet = (IntPtr)R->EBP;
        //         Pointer<BulletVelocity> pVelocity = R->lea_Stack<IntPtr>(0x0A0); // 已经算过重力的速度
        //         BulletVelocity velocity = pBullet.Ref.Velocity;
        //         Logger.Log("2当前车速 {0} - {1}, {2}", pBullet.Ref.Speed, velocity, pVelocity.IsNull ? "null" : pVelocity.Data.X);
        //         // pVelocity.Ref.X = velocity.X;
        //         // pVelocity.Ref.Y = velocity.Y;
        //         // pVelocity.Ref.Z = 0;
        //     }
        //     catch (Exception e)
        //     {
        //         Logger.PrintException(e);
        //     }
        //     return (uint)0;
        // }

        // 除 ROT>0 和 Vertical 之外的抛射体会在此根据重力对储存的向量变量进行运算
        // 对Arcing抛射体的重力进行削减
        [Hook(HookType.AresHook, Address = 0x46745C, Size = 7)]
        public static unsafe UInt32 BulletClass_Update_ChangeVelocity(REGISTERS* R)
        {
            try
            {
                Pointer<BulletClass> pBullet = (IntPtr)R->EBP;
                BulletExt ext = BulletExt.ExtMap.Find(pBullet);
                if (pBullet.Ref.Type.Ref.Arcing && null != ext && ext.SpeedChanged)
                {
                    Pointer<BulletVelocity> pVelocity = R->lea_Stack<IntPtr>(0x90); // 已经算过重力的速度
                    BulletVelocity velocity = pBullet.Ref.Velocity;
                    // Logger.Log("Arcing当前车速 {0} - {1}, {2}", pBullet.Ref.Speed, velocity, pVelocity.IsNull ? "null" : pVelocity.Data);
                    // velocity *= 0;
                    pVelocity.Ref.X = velocity.X;
                    pVelocity.Ref.Y = velocity.Y;
                    pVelocity.Ref.Z = ext.LocationLocked ? 0 : velocity.Z; // 锁定状态，竖直方向向量0
                    // Logger.Log(" - Arcing当前车速 {0} - {1}, {2}", pBullet.Ref.Speed, velocity, pVelocity.IsNull ? "null" : pVelocity.Data);
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return (uint)0;
        }

        // 除 ROT>0 和 Vertical 之外的抛射体会在Label_158根据速度向量获取坐标
        // Arcing抛射体即使向量非常小也会试图移动1点
        [Hook(HookType.AresHook, Address = 0x4677C2, Size = 5)]
        public static unsafe UInt32 BulletClass_Update_ChangeVelocity_Locked(REGISTERS* R)
        {
            try
            {
                Pointer<BulletClass> pBullet = (IntPtr)R->EBP;
                BulletExt ext = BulletExt.ExtMap.Find(pBullet);
                if (pBullet.Ref.Type.Ref.Arcing && null != ext && ext.SpeedChanged && ext.LocationLocked)
                {
                    // Logger.Log("Label_158 当前坐标 {0} - 坐标 {{\"X\":{1}, \"Y\":{2}, \"Z\":{3}}}", pBullet.Ref.Base.Location, R->ESI, R->EDI, R->EAX);
                    CoordStruct location = pBullet.Ref.Base.Location;
                    R->ESI = (uint)location.X;
                    R->EDI = (uint)location.Y;
                    R->EAX = (uint)location.Y; // 不要问为什么不是Z
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x46920B, Size = 6)]
        public static unsafe UInt32 BulletClass_Detonate(REGISTERS* R)
        {
            try
            {
                Pointer<BulletClass> pBullet = (IntPtr)R->ESI;
                Pointer<CoordStruct> pCoordsDetonation = R->Base<IntPtr>(0x8);
                BulletExt ext = BulletExt.ExtMap.Find(pBullet);
                ext?.OnDetonate(pCoordsDetonation.Data);
                ext?.Scriptable?.OnDetonate(pCoordsDetonation.Data);
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return (uint)0;
        }

    }
}