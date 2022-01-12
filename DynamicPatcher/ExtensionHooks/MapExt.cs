
using System.Collections;
using System.Drawing;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Script;
using Extension.Utilities;

namespace ExtensionHooks
{
    public class MapExtHooks
    {

        [Hook(HookType.AresHook, Address = 0x69252D, Size = 6)]
        public static unsafe UInt32 ScrollClass_ProcessClickCoords_VirtualUnit(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (true == ext?.VirtualUnit)
                {
                    // Logger.Log("ScrollClass_ClickCoords {0} is virtual unit", pTechno.Ref.Type.Ref.Base.Base.ID);
                    return 0x6925E6;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return (uint)0;
        }

        /*
            generic crate-handler file
            currently used only to shim crates into TechnoExt
            since Techno fields are used by AttachEffect

            Graion Dilach, 2013-05-31
        */
        //overrides for crate checks
        //481D52 - pass
        //481C86 - override with Money
        [Hook(HookType.AresHook, Address = 0x481D0E, Size = 6)]
        public static unsafe UInt32 CellClass_CrateBeingCollected_Firepower1(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->EDI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && ext.CrateMultiplier.FirepowerMultiplier == 1.0)
                {
                    return 0x481D52;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0x481C86;
        }

        [Hook(HookType.AresHook, Address = 0x481C6C, Size = 6)]
        public static unsafe UInt32 CellClass_CrateBeingCollected_Armor1(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->EDI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && ext.CrateMultiplier.ArmorMultiplier == 1.0)
                {
                    return 0x481D52;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0x481C86;
        }

        [Hook(HookType.AresHook, Address = 0x481CE1, Size = 6)]
        public static unsafe UInt32 CellClass_CrateBeingCollected_Speed1(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->EDI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && ext.CrateMultiplier.SpeedMultiplier == 1.0)
                {
                    return 0x481D52;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0x481C86;
        }

        //overrides on actual crate effect applications
        [Hook(HookType.AresHook, Address = 0x483226, Size = 6)]
        public static unsafe UInt32 CellClass_CrateBeingCollected_Firepower2(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
                double pow_FirepowerMultiplier = R->Stack<double>(0x20);
                // Logger.Log("{0}踩箱子获得火力加成{1}", pTechno.Ref.Type.Ref.Base.Base.ID, pow_FirepowerMultiplier);
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && ext.CrateMultiplier.FirepowerMultiplier == 1.0)
                {
                    ext.CrateMultiplier.FirepowerMultiplier = pow_FirepowerMultiplier;
                    ext.RecalculateStatus();
                    R->AL = Convert.ToByte(pTechno.Ref.Owner.Ref.PlayerControl);
                    return 0x483258;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0x483261;
        }

        [Hook(HookType.AresHook, Address = 0x482E57, Size = 6)]
        public static unsafe UInt32 CellClass_CrateBeingCollected_Armor2(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
                double pow_ArmorMultiplier = R->Stack<double>(0x20);
                // Logger.Log("{0}踩箱子获得装甲加成{1}", pTechno.Ref.Type.Ref.Base.Base.ID, pow_ArmorMultiplier);
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && ext.CrateMultiplier.ArmorMultiplier == 1.0)
                {
                    ext.CrateMultiplier.ArmorMultiplier = pow_ArmorMultiplier;
                    ext.RecalculateStatus();
                    R->AL = Convert.ToByte(pTechno.Ref.Owner.Ref.PlayerControl);
                    return 0x482E89;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0x482E92;
        }

        [Hook(HookType.AresHook, Address = 0x48303A, Size = 6)]
        public static unsafe UInt32 CellClass_CrateBeingCollected_Speed2(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->EDI;
                double pow_SpeedMultiplier = R->Stack<double>(0x20);
                // Logger.Log("{0}踩箱子获得速度加成{1}", pTechno.Ref.Type.Ref.Base.Base.ID, pow_SpeedMultiplier);
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && ext.CrateMultiplier.SpeedMultiplier == 1.0)
                {
                    ext.CrateMultiplier.SpeedMultiplier = pow_SpeedMultiplier;
                    ext.RecalculateStatus();
                    R->CL = Convert.ToByte(pTechno.Ref.Owner.Ref.PlayerControl);
                    return 0x483078;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0x483081;
        }

    }
}