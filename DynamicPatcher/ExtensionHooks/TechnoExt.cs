
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
    public class TechnoExtHooks
    {

        [Hook(HookType.AresHook, Address = 0x6F3260, Size = 5)]
        public static unsafe UInt32 TechnoClass_CTOR(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_CTOR(R);
        }

        [Hook(HookType.AresHook, Address = 0x6F4500, Size = 5)]
        public static unsafe UInt32 TechnoClass_DTOR(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_DTOR(R);
        }

        [Hook(HookType.AresHook, Address = 0x70C250, Size = 8)]
        [Hook(HookType.AresHook, Address = 0x70BF50, Size = 5)]
        public static unsafe UInt32 TechnoClass_SaveLoad_Prefix(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_SaveLoad_Prefix(R);
        }

        [Hook(HookType.AresHook, Address = 0x70C249, Size = 5)]
        public static unsafe UInt32 TechnoClass_Load_Suffix(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_Load_Suffix(R);
        }

        [Hook(HookType.AresHook, Address = 0x70C264, Size = 5)]
        public static unsafe UInt32 TechnoClass_Save_Suffix(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_Save_Suffix(R);
        }

        [Hook(HookType.AresHook, Address = 0x6F42ED, Size = 10)]
        public static unsafe UInt32 TechnoClass_Init(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnInit();
                ext?.Scriptable?.OnInit();
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }


        [Hook(HookType.AresHook, Address = 0x6F9E50, Size = 5)]
        public static unsafe UInt32 TechnoClass_Update(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                // technoClassUpdate?.Invoke(pTechno, ext);
                ext?.OnUpdate();
                ext?.Scriptable?.OnUpdate();
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x71A88D, Size = 0)]
        public static unsafe UInt32 TemporalClass_UpdateA(REGISTERS* R)
        {
            try
            {
                Pointer<TemporalClass> pTemporal = (IntPtr)R->ESI;

                Pointer<TechnoClass> pTechno = pTemporal.Ref.Target;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnTemporalUpdate(pTemporal);
                ext?.Scriptable?.OnTemporalUpdate(pTemporal);

            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            if ((int)R->EAX <= (int)R->EBX)
            {
                return 0x71A895;
            }
            return 0x71AB08;
        }

        /*
        [Hook(HookType.AresHook, Address = 0x71A84E, Size = 5)]
        public static unsafe UInt32 TemporalClass_UpdateA(REGISTERS* R)
        {
            try
            {
                Pointer<TemporalClass> pTemporal = (IntPtr)R->ESI;

                Pointer<TechnoClass> pTechno = pTemporal.Ref.Target;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnTemporalUpdate(pTemporal);
                ext?.Scriptable?.OnTemporalUpdate(pTemporal);

                // pTemporal.Ref.WarpRemaining -= pTemporal.Ref.GetWarpPerStep(0);
                // R->EAX = (uint)pTemporal.Ref.WarpRemaining;
                // return 0x71A88D;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }
        */

        [Hook(HookType.AresHook, Address = 0x6F6CA0, Size = 7)]
        public static unsafe UInt32 TechnoClass_Put(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
            var pCoord = R->Stack<Pointer<CoordStruct>>(0x4);
            var faceDir = R->Stack<Direction>(0x8);

            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            ext?.OnPut(pCoord, faceDir);
            ext?.Scriptable?.OnPut(pCoord, faceDir);

            return 0;
        }

        // [Hook(HookType.AresHook, Address = 0x6F6AC0, Size = 5)]
        [Hook(HookType.AresHook, Address = 0x6F6AC4, Size = 5)]
        public static unsafe UInt32 TechnoClass_Remove(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnRemove();
                ext?.Scriptable?.OnRemove();
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x701900, Size = 6)]
        public static unsafe UInt32 TechnoClass_ReceiveDamage(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
            var pDamage = R->Stack<Pointer<int>>(0x4);
            var distanceFromEpicenter = R->Stack<int>(0x8);
            var pWH = R->Stack<Pointer<WarheadTypeClass>>(0xC);
            var pAttacker = R->Stack<Pointer<ObjectClass>>(0x10);
            var ignoreDefenses = R->Stack<bool>(0x14);
            var preventPassengerEscape = R->Stack<bool>(0x18);
            var pAttackingHouse = R->Stack<Pointer<HouseClass>>(0x1C);

            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            ext?.OnReceiveDamage(pDamage, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse);
            ext?.Scriptable?.OnReceiveDamage(pDamage, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse);

            return 0;
        }


        [Hook(HookType.AresHook, Address = 0x702050, Size = 6)]
        public static unsafe UInt32 TechnoClass_Destroy(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnDestroy();
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x6FC339, Size = 6)]
        public static unsafe UInt32 TechnoClass_CanFire(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                Pointer<WeaponTypeClass> pWeapon = (IntPtr)R->EDI;
                var pTarget = R->Stack<Pointer<AbstractClass>>(0x20 - (-0x4));

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                bool ceaseFire = false;
                ext?.CanFire(pTarget, pWeapon, ref ceaseFire);
                ext?.Scriptable?.CanFire(pTarget, pWeapon, ref ceaseFire);
                if (ceaseFire)
                {
                    return (uint)0x6FCB7E;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x6FDD50, Size = 6)]
        public static unsafe UInt32 TechnoClass_Fire(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
                var pTarget = R->Stack<Pointer<AbstractClass>>(0x4);
                var nWeaponIndex = R->Stack<int>(0x8);

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnFire(pTarget, nWeaponIndex);
                ext?.Scriptable?.OnFire(pTarget, nWeaponIndex);
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }


        // [Hook(HookType.AresHook, Address = 0x6FDD71, Size = 6)]
        public static unsafe UInt32 TechnoClass_Fire_OverrideWeapon(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                // Logger.Log("ESI = {0} Fire, mark weaponIndex = {1}", pTechno.IsNull ? "Is Null" : pTechno.Ref.Type.Convert<AbstractTypeClass>().Ref.ID, null != ext.overrideWeapon ? ext.overrideWeapon.weaponIndex : "no mark");
                if (null != ext.overrideWeapon && ext.overrideWeapon.OverrideThisWeapon())
                {
                    Pointer<WeaponTypeClass> pWeapon = ext.overrideWeapon.pOverrideWeapon;
                    if (!pWeapon.IsNull)
                    {
                        // Logger.Log("Override weapon {0}", pWeapon.Ref.Base.ID);
                        R->EBX = (uint)pWeapon;
                    }
                }
                /*
                Pointer<WeaponTypeClass> pWeapon = (IntPtr)R->EBX;
                if (pWeapon.IsNull)
                {
                    Logger.Log("EBX is null");
                    // Check OverridWeapon
                    pWeapon = WeaponTypeClass.ABSTRACTTYPE_ARRAY.Find("RadBeamWeapon");
                    if (!pWeapon.IsNull)
                    {
                        R->EBX = (uint)pWeapon;
                    }
                }
                */
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x702E9D, Size = 6)]
        public static unsafe UInt32 TechnoClass_RegisterDestruction(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                Pointer<TechnoClass> pKiller = (IntPtr)R->EDI;
                int cost = (int)R->EBP;

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);

                bool skip = false;
                ext?.OnRegisterDestruction(pKiller, cost, ref skip);
                // skip the entire veterancy
                if (skip)
                {
                    return 0x702FF5;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x5F45A0, Size = 5)]
        public static unsafe UInt32 TechnoClass_Select(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->EDI;
                // Logger.Log("{0} Select", pTechno.IsNull ? "Unknow" : pTechno.Ref.Type.Ref.Base.Base.ID);
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                bool selectable = true;
                ext?.OnSelect(ref selectable);
                ext?.Scriptable?.OnSelect(ref selectable);
                if (!selectable)
                {
                    return 0x5F45A9;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }


        [Hook(HookType.AresHook, Address = 0x6F65D1, Size = 6)]
        public static unsafe UInt32 TechnoClass_DrawHealthBar_Building(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;

                int length = (int)R->EBX;
                Pointer<Point2D> pLocation = (IntPtr)R->Stack<IntPtr>(0x4C - (-0x4));
                Pointer<RectangleStruct> pBound = (IntPtr)R->Stack<IntPtr>(0x4C - (0x8));

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.DrawHealthBar_Building(length, pLocation, pBound);
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x6F683C, Size = 7)]
        public static unsafe UInt32 TechnoClass_DrawHealthBar_Other(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;

                int length = pTechno.Ref.Base.Base.WhatAmI() == AbstractType.Infantry ? 8 : 17;
                Pointer<Point2D> pLocation = (IntPtr)R->Stack<IntPtr>(0x4C - (-0x4));
                Pointer<RectangleStruct> pBound = (IntPtr)R->Stack<IntPtr>(0x4C - (0x8));

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.DrawHealthBar_Other(length, pLocation, pBound);
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        // case VISUAL_NORMAL
        [Hook(HookType.AresHook, Address = 0x7063FF, Size = 7)]
        public static unsafe UInt32 TechnoClass_DrawSHP_Colour(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.DrawSHP_Colour(R);
                /*
                if (!pTechno.IsNull && pTechno.Convert<ObjectClass>().Ref.IsSelected)
                {
                    var x = R->EAX;
                    Logger.Log("Unit[{0}] EAX = {1}", pTechno.Ref.Type.Ref.Base.Base.ID, x);

                    ColorStruct color = new ColorStruct(0, 255, 128);
                    ColorStruct colorAdd = ExHelper.Color2ColorAdd(color);
                    Logger.Log("RGB888 = {0}, RGB565 = {1}, RGB565 = {2}", color, colorAdd, ExHelper.ColorAdd2RGB565(colorAdd));
                    R->EAX = ExHelper.ColorAdd2RGB565(colorAdd);
                }
                */
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }


        // change berzerk color
        [Hook(HookType.AresHook, Address = 0x73C15F, Size = 7)]
        public static unsafe UInt32 TechnoClass_DrawVXL_Colour(REGISTERS* R)
        {
            try
            {
                Pointer<UnitClass> pUnit = (IntPtr)R->EBP;
                TechnoExt ext = TechnoExt.ExtMap.Find(pUnit.Convert<TechnoClass>());
                ext?.DrawVXL_Colour(R);
                /*
                if (!pUnit.IsNull && pUnit.Convert<ObjectClass>().Ref.IsSelected)
                {
                    Pointer<TechnoClass> pTechno = pUnit.Convert<TechnoClass>();
                    var x = R->ESI;
                    Logger.Log("Unit[{0}] ESI = {1}", pUnit.Ref.Base.Base.Type.Ref.Base.Base.ID, x);

                    ColorStruct color = new ColorStruct(0, 255, 128);
                    ColorStruct colorAdd = ExHelper.Color2ColorAdd(color);
                    Logger.Log("RGB888 = {0}, RGB565 = {1}, RGB565 = {2}", color, colorAdd, ExHelper.ColorAdd2RGB565(colorAdd));
                    R->ESI = ExHelper.ColorAdd2RGB565(colorAdd);
                }
                */
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x6F9039, Size = 5)]
        public static unsafe UInt32 TechnoClass_Greatest_Threat_HealWeaponRange(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
            int guardRange = pTechno.Ref.Type.Ref.GuardRange;
            Pointer<WeaponStruct> pirmary = pTechno.Ref.GetWeapon(0);
            if (!pirmary.IsNull && !pirmary.Ref.WeaponType.IsNull)
            {
                int range = pirmary.Ref.WeaponType.Ref.Range;
                if (range > guardRange)
                {
                    guardRange = range;
                }
            }
            Pointer<WeaponStruct> secondary = pTechno.Ref.GetWeapon(1);
            if (!secondary.IsNull && !secondary.Ref.WeaponType.IsNull)
            {
                int range = secondary.Ref.WeaponType.Ref.Range;
                if (range > guardRange)
                {
                    guardRange = range;
                }
            }
            R->EDI = (uint)guardRange;
            return 0x6F903E;
        }

        [Hook(HookType.AresHook, Address = 0x730E8F, Size = 6)]
        public static unsafe UInt32 ObjectClass_GuardCommand(REGISTERS* R)
        {
            Pointer<ObjectClass> pObject = (IntPtr)R->ESI;
            if (pObject.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                // Logger.Log("{0} Guard Command", pTechno.IsNull ? "Unknow" : pTechno.Ref.Type.Ref.Base.Base.ID);
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnGuardCommand();
                ext?.Scriptable?.OnGuardCommand();
            }
            return 0;
        }


        [Hook(HookType.AresHook, Address = 0x730F1C, Size = 5)]
        public static unsafe UInt32 ObjectClass_StopCommand(REGISTERS* R)
        {
            Pointer<ObjectClass> pObject = (IntPtr)R->ESI;
            if (pObject.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                // Logger.Log("{0} Stop Command", pTechno.IsNull ? "Unknow" : pTechno.Ref.Type.Ref.Base.Base.ID);
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnStopCommand();
                ext?.Scriptable?.OnStopCommand();
            }
            return 0;
        }


        // [Hook(HookType.AresHook, Address = 0x738801, Size = 6)]
        // public static unsafe UInt32 UnitClass_Destroy(REGISTERS* R)
        // {
        //     try
        //     {
        //         Pointer<UnitClass> pUnit = (IntPtr)R->ESI;
        //         //Pointer<ObjectClass> pKiller = (IntPtr)R->EAX;
        //         //Logger.Log("pKill {0}", pKiller.IsNull ? "is null" : (pKiller.Ref.Type.IsNull ? "type is null" : pKiller.Ref.Type.Convert<AbstractTypeClass>().Ref.ID));
        //         TechnoExt ext = TechnoExt.ExtMap.Find(pUnit.Convert<TechnoClass>());
        //         ext?.OnDestroy_UnitClass();
        //     }
        //     catch (Exception e)
        //     {
        //         Logger.PrintException(e);
        //     }
        //     return 0;
        // }

        // Someone wants to enter the cell where I am
        // [Hook(HookType.AresHook, Address = 0x73F528, Size = 6)]
        // public static unsafe UInt32 UnitClass_CanEnterCell(REGISTERS* R)
        // {
        //     try
        //     {
        //         Pointer<TechnoClass> pUnit = (IntPtr)R->EBX;
        //         Pointer<TechnoClass> pOccupier = (IntPtr)R->ESI;

        //         if (pUnit == pOccupier)
        //         {
        //             return 0x73FC10;
        //         }

        //         TechnoExt ext = TechnoExt.ExtMap.Find(pUnit);
        //         if (null != ext)
        //         {
        //             bool ignoreOccupier = false;
        //             ext?.CanEnterCell_UnitClass(pOccupier, ref ignoreOccupier);
        //             if (ignoreOccupier)
        //             {
        //                 return 0x73FC10;
        //             }
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Logger.PrintException(e);
        //     }
        //     return 0x73F530;
        // }

        // Someone wants to enter the cell where I am
        // [Hook(HookType.AresHook, Address = 0x51C251, Size = 6)]
        // public static unsafe UInt32 InfantryClass_CanEnterCell(REGISTERS* R)
        // {
        //     try
        //     {
        //         Pointer<TechnoClass> pInf = (IntPtr)R->EBP;
        //         Pointer<TechnoClass> pOccupier = (IntPtr)R->ESI;

        //         if (pInf == pOccupier)
        //         {
        //             return 0x51C70F;
        //         }

        //         TechnoExt ext = TechnoExt.ExtMap.Find(pInf);
        //         if (null != ext)
        //         {
        //             bool ignoreOccupier = false;
        //             ext?.CanEnterCell_InfantryClass(pOccupier, ref ignoreOccupier);
        //             if (ignoreOccupier)
        //             {
        //                 return 0x51C70F;
        //             }
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Logger.PrintException(e);
        //     }
        //     return 0x51C259;
        // }


        // [Hook(HookType.AresHook, Address= 0x5F5854, Size = 6)]
        // public static unsafe UInt32 ObjectClass_Mark(REGISTERS* R)
        // {
        //     Logger.Log("{0} OOXX {1}", R->ESI, R->Stack<int>(-0x4));
        //     Pointer<ObjectClass> pTechno = (IntPtr)R->ESI;
        //     Logger.Log("{0} OOXX", pTechno.IsNull ? "Unknow" : pTechno.Ref.Type.Ref.Base.ID);
        //     return 0;
        // }

        // [Hook(HookType.AresHook, Address = 0x707557, Size = 6)]
        // public static unsafe UInt32 TechnoClass_DrawVXL(REGISTERS* R)
        // {
        //     Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
        //     Logger.Log("{0} ooxx", pTechno.Ref.Type.Ref.Base.Base.ID);
        //     return 0;
        // }

        [Hook(HookType.AresHook, Address = 0x5F6B90, Size = 5)]
        public static unsafe UInt32 ObjectClass_InAir_SkipCheckIsOnMap(REGISTERS* R)
        {
            Pointer<ObjectClass> pObject = (IntPtr)R->ECX;
            if (pObject.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && !ext.MyMaster.IsNull)
                {
                    return 0x5F6B97;
                }
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x704363, Size = 5)]
        public static unsafe UInt32 TechnoClass_GetZAdjust(REGISTERS* R)
        {

            Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
            int z = (int)R->EAX;
            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            if (null != ext && !ext.MyMaster.IsNull && (null == ext.StandType || !ext.StandType.IsTrain))
            {
                int offset = null != ext.StandType ? ext.StandType.ZOffset : 12;
                R->ECX = (uint)(z + offset);
                // Logger.Log("ZOffset = {0}, ECX = {1}, EAX = {2}", offset, R->ECX, R->EAX);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x4DB7F7, Size = 6)]
        public static unsafe UInt32 FootClass_In_Which_Layer(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            if (null != ext && !ext.MyMaster.IsNull && null != ext.StandType && !ext.StandType.IsTrain && ext.StandType.ZOffset != 0)
            {
                // Logger.Log("{0} - {1}{2} StandType.DrawLayer = {3}, StandType.ZOffset = {4}", Game.CurrentFrame, pTechno.Ref.Type.Ref.Base.Base.ID, pTechno, ext.StandType.DrawLayer, ext.StandType.ZOffset);
                Layer layer = ext.StandType.DrawLayer;
                if (layer == Layer.None)
                {
                    // Logger.Log(" - ooxx {0}, EAX = {1}, ECX = {2}", pTechno.Ref.Type.Ref.Base.Base.ID, R->EAX, R->ECX);
                    R->EAX = pTechno.Ref.Base.Base.IsInAir() ? (uint)Layer.Top : (uint)Layer.Air;
                }
                else
                {
                    R->EAX = (uint)layer;
                }
                return 0x4DB803;
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x54B8E9, Size = 6)]
        public static unsafe UInt32 JumpjetLocomotionClass_In_Which_Layer_Deviation(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->EAX;
            if (pTechno.Ref.Base.Base.IsInAir())
            {
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext &&  ext.AttachEffectManager.HasStand())
                {
                    // Override JumpjetHeight / CruiseHeight check so it always results in 3 / Layer::Air.
                    R->EDX = Int32.MaxValue;
                    return 0x54B96B;
                }
            }
            return 0;
        }

    }
}