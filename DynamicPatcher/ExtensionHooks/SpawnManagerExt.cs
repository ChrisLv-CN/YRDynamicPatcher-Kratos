
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
    public class SpawnManagerExtHooks
    {

        // [Hook(HookType.AresHook, Address = 0x6B7B90, Size = 7)]
        // public static unsafe UInt32 SpawnManagerClass_Assign_Target(REGISTERS* R)
        // {
        //     Pointer<AbstractClass> pTarget = R->Stack<IntPtr>(0x4);
        //     if (!pTarget.IsNull)
        //     {
        //         Logger.Log($"{Game.CurrentFrame} 子机管理器分配目标，目标 {pTarget} 类型 {pTarget.Ref.WhatAmI()}");
        //     }
        //     else
        //     {
        //         Logger.Log($"{Game.CurrentFrame} 子机管理器分配目标，目标 {pTarget}");
        //     }
        //     return 0;
        // }

        // [Hook(HookType.AresHook, Address = 0x6B7392, Size = 6)]
        // public static unsafe UInt32 SpawnManagerClass_AI(REGISTERS* R)
        // {
        //     Logger.Log($"{Game.CurrentFrame} - SpawnManagerClass_AI 子机停车？");
        //     return 0;
        // }

        [Hook(HookType.AresHook, Address = 0x6B7A32, Size = 5)]
        public static unsafe UInt32 SpawnManagerClass_AI_Add_Missile_Target(REGISTERS* R)
        {
            Pointer<TechnoClass> pRocket = (IntPtr)R->ECX;
            Pointer<AbstractClass> pTarget = (IntPtr)R->EAX;
            // Logger.Log($"{Game.CurrentFrame} 子机管理器为导弹 {pRocket} 分配目标，目标 {pTarget} 类型 {pTarget.Ref.WhatAmI()}");
            // 如果目标在天上，则自动开启跟踪模式
            if (pTarget.Ref.IsInAir())
            {
                TechnoExt ext = TechnoExt.ExtMap.Find(pRocket);
                ext.IsHoming = true;
            }
            return 0;
        }

        // [Hook(HookType.AresHook, Address = 0x6B7BB0, Size = 6)]
        // public static unsafe UInt32 SpawnManagerClass_Kamikaze_AI(REGISTERS* R)
        // {
        //     Logger.Log($"{Game.CurrentFrame} - SpawnManagerClass_Kamikaze_AI");
        //     return 0;
        // }

        // [Hook(HookType.AresHook, Address = 0x6B7BEB, Size = 5)]
        // public static unsafe UInt32 SpawnManagerClass_Kamikaze_AI2(REGISTERS* R)
        // {
        //     Logger.Log($"{Game.CurrentFrame} - SpawnManagerClass_Kamikaze_AI_SpawnControl_State=2");
        //     return 0;
        // }

        [Hook(HookType.AresHook, Address = 0x54E42B, Size = 6)]
        public static unsafe UInt32 KamikazeTrackerClass_Add_Missile_Has_Target(REGISTERS* R)
        {
            // Pointer<AircraftClass> pRocket = R->Stack<IntPtr>(0x1C);
            Pointer<AbstractClass> pTarget = (IntPtr)R->ECX;
            // TechnoExt ext = TechnoExt.ExtMap.Find(pRocket.Convert<TechnoClass>());
            // ext.pHomingTarget.Pointer = pTarget;
            // Logger.Log($"{Game.CurrentFrame} 导弹 {pRocket} {pRocket.Ref.Type.Ref.Base.Base.Base.ID} 获得目标 {pTarget} 类型 {pTarget.Ref.WhatAmI()}");
            R->EAX = (uint)pTarget;
            return 0x54E475;
        }

        // [Hook(HookType.AresHook, Address = 0x54E5A2, Size = 7)]
        // public static unsafe UInt32 KamikazeTrackerClass_Detach(REGISTERS* R)
        // {
        //     Logger.Log($"{Game.CurrentFrame} - KamikazeTrackerClass_Detach");
        //     return 0;
        // }

        [Hook(HookType.AresHook, Address = 0x6622C0, Size = 6)]
        public static unsafe UInt32 RocketLocomotionClass_Process(REGISTERS* R)
        {
            Pointer<FootClass> pFoot = (IntPtr)R->ESI;
            Pointer<RocketLocomotionClass> pLoco = pFoot.Ref.Locomotor.ToLocomotionClass<RocketLocomotionClass>();
            // Logger.Log($"{Game.CurrentFrame} - RocketLocomotionClass_Process {pLoco.Ref.Timer34}");
            // If missile try to AA, it will block on ground. step is 0, can't move.
            if (pLoco.Ref.Timer34.Step == 0)
            {
                pLoco.Ref.Timer34.Step = 1;
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x662CAC, Size = 6)]
        public static unsafe UInt32 RocketLocomotionClass_Process_Step5_To_Lazy_4(REGISTERS* R)
        {
            // Only "Lazy = no" missile has Step5, "Lazy = yes"'s Step4 moving logic is the best.
            // Logger.Log($"{Game.CurrentFrame} - RocketLocomotionClass_Process_Step5 {R->ESI} {R->Stack<IntPtr>(-0x4)}");
            Pointer<RocketLocomotionClass> pLoco = (IntPtr)(R->ESI - 4); // ╮(╯_╰)╭
            Pointer<FootClass> pFoot = pLoco.Convert<LocomotionClass>().Ref.LinkedTo;
            TechnoExt ext = TechnoExt.ExtMap.Find(pFoot.Convert<TechnoClass>());
            if (ext.IsHoming)
            {
                return 0x662A32;
            }
            return 0;
        }

        // [Hook(HookType.AresHook, Address = 0x6621A0, Size = 5)]
        // public static unsafe UInt32 RocketLocomotionClass_6620F0(REGISTERS* R)
        // {
        //     try
        //     {
        //         Pointer<RocketLocomotionClass> pLoco = (IntPtr)R->ESI;
        //         Pointer<FootClass> pFoot = pLoco.Convert<LocomotionClass>().Ref.LinkedTo;
        //         Pointer<AircraftClass> pRocket = pFoot.Convert<AircraftClass>();
        //         int v6 = (int)R->EDI;
        //         CoordStruct sourcePos = pRocket.Ref.Base.Base.Base.Base.GetCoords();
        //         CoordStruct targetPos = pLoco.Ref.Destination;
        //         Surface.Primary.Ref.DrawText(v6.ToString(), sourcePos.ToClientPos() + new Point2D(0, -30), ColorStruct.Blue);
        //         // BulletEffectHelper.BlueCrosshair(targetPos, 1024);
        //         Logger.Log($"{Game.CurrentFrame} - Loco {pLoco} 绑定的子机导弹 {pRocket} [{pRocket.Ref.Type.Ref.Base.Base.Base.ID}] 距离目标距离 {R->EDI} {pLoco.Ref.Destination.Z}");

        //         // return 0x662214;
        //     }
        //     catch (Exception e)
        //     {
        //         Logger.PrintException(e);
        //     }
        //     return 0;
        // }

        // [Hook(HookType.AresHook, Address = 0x66220A, Size = 6)]
        // public static unsafe UInt32 RocketLocomotionClass_6620F02(REGISTERS* R)
        // {
        //     try
        //     {
        //         Pointer<AircraftClass> pRocket = (IntPtr)R->ECX;
        //         Logger.Log($"{Game.CurrentFrame} - 子机导弹 {pRocket} [{pRocket.Ref.Type.Ref.Base.Base.Base.ID}] 高度爆炸检查 {pRocket.Ref.Base.Base.Base.GetHeight()}");
        //     }
        //     catch (Exception e)
        //     {
        //         Logger.PrintException(e);
        //     }
        //     return 0;
        // }


        // [Hook(HookType.AresHook, Address = 0x66304F, Size = 5)]
        // public static unsafe UInt32 RocketLocomotionClass_663030(REGISTERS* R)
        // {
        //     Logger.Log($"{Game.CurrentFrame} - 子机导弹 {R->EDX} 爆炸");
        //     return 0;
        // }


    }
}

