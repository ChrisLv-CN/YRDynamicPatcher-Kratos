
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Script;

namespace ExtensionHooks
{
    public class TechnoExtHooks
    {

        [Hook(HookType.AresHook, Address = 0x6F3260, Size = 5)]
        static public unsafe UInt32 TechnoClass_CTOR(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_CTOR(R);
        }

        [Hook(HookType.AresHook, Address = 0x6F4500, Size = 5)]
        static public unsafe UInt32 TechnoClass_DTOR(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_DTOR(R);
        }
        
        [Hook(HookType.AresHook, Address = 0x70C250, Size = 8)]
        [Hook(HookType.AresHook, Address = 0x70BF50, Size = 5)]
        static public unsafe UInt32 TechnoClass_SaveLoad_Prefix(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_SaveLoad_Prefix(R);
        }

        [Hook(HookType.AresHook, Address = 0x70C249, Size = 5)]
        static public unsafe UInt32 TechnoClass_Load_Suffix(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_Load_Suffix(R);
        }

        [Hook(HookType.AresHook, Address = 0x70C264, Size = 5)]
        static public unsafe UInt32 TechnoClass_Save_Suffix(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_Save_Suffix(R);
        }
        
        
        [Hook(HookType.AresHook, Address = 0x6F9E50, Size = 5)]
        static public unsafe UInt32 TechnoClass_Update(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                // technoClassUpdate?.Invoke(pTechno, ext);
                ext?.OnUpdate();
                ext.Scriptable?.OnUpdate();
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x6F6CA0, Size = 7)]
        static public unsafe UInt32 TechnoClass_Put_Script(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
            var pCoord = R->Stack<Pointer<CoordStruct>>(0x4);
            var faceDir = R->Stack<Direction>(0x8);

            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            ext?.OnPut(pCoord, faceDir);
            ext.Scriptable?.OnPut(pCoord, faceDir);

            return (uint)0;
        }

        // [Hook(HookType.AresHook, Address = 0x6F6AC0, Size = 5)]
        [Hook(HookType.AresHook, Address = 0x6F6AC4, Size = 5)]
        static public unsafe UInt32 TechnoClass_Remove_Script(REGISTERS* R)
        {
            try 
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnRemove();
                ext.Scriptable?.OnRemove();
            }
            catch(Exception e)
            {
                Logger.PrintException(e);
            }
            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x701900, Size = 6)]
        static public unsafe UInt32 TechnoClass_ReceiveDamage_Script(REGISTERS* R)
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
            ext.Scriptable?.OnReceiveDamage(pDamage, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse);

            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x6FC339, Size = 6)]
        static public unsafe UInt32 TechnoClass_CanFire(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                Pointer<WeaponTypeClass> pWeapon = (IntPtr)R->EDI;
                var pTarget = R->Stack<Pointer<AbstractClass>>(0x20 - (-0x4));

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                bool ceaseFire = false;
                ext?.CanFire(pTarget, pWeapon, ref ceaseFire);
                if (ceaseFire)
                {
                    return (uint)0x6FCB7E;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x6FDD50, Size = 6)]
        static public unsafe UInt32 TechnoClass_Fire(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
                var pTarget = R->Stack<Pointer<AbstractClass>>(0x4);
                var nWeaponIndex = R->Stack<int>(0x8);

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnFire(pTarget, nWeaponIndex);
                ext.Scriptable?.OnFire(pTarget, nWeaponIndex);;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x6F65D1, Size = 6)]
        static public unsafe UInt32 TechnoClass_DrawHealthBar_Building(REGISTERS* R)
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
            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x6F683C, Size = 7)]
        static public unsafe UInt32 TechnoClass_DrawHealthBar_Other(REGISTERS* R)
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
            return (uint)0;
        }

        [Hook(HookType.AresHook, Address = 0x738801, Size = 6)]
        static public unsafe UInt32 UnitClass_Destory(REGISTERS* R)
        {
            try {
                Pointer<UnitClass> pUnit = (IntPtr)R->ESI;
                //Pointer<ObjectClass> pKiller = (IntPtr)R->EAX;
                //Logger.Log("pKill {0}", pKiller.IsNull ? "is null" : (pKiller.Ref.Type.IsNull ? "type is null" : pKiller.Ref.Type.Convert<AbstractTypeClass>().Ref.ID));
                TechnoExt ext = TechnoExt.ExtMap.Find(pUnit.Convert<TechnoClass>());
                ext?.OnDestory_UnitClass();
            }
            catch(Exception e)
            {
                Logger.PrintException(e);
            }
            return (uint)0;
        }

        // Test

        [Hook(HookType.AresHook, Address = 0x0073CC03, Size = 7)]
        static public unsafe UInt32 UnitClass_DrawSHP_Berzerk(REGISTERS* R)
        {
            try {

            }
            catch(Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x0042312A, Size = 6)]
        static public unsafe UInt32 AnimClass_Draw_Remap(REGISTERS* R)
        {
            //Logger.Log("Hook 0x00423130 calling...");
            Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
            if (!pAnim.IsNull && pAnim.Ref.Type.Ref.AltPalette && !pAnim.Ref.Owner.IsNull)
            {
                // string id = pAnim.Ref.Owner.IsNull ? "NULL" : pAnim.Ref.Owner.Ref.Type.Ref.Base.ID;
                // ColorStruct color = pAnim.Ref.Owner.IsNull ? default : pAnim.Ref.Owner.Ref.Color;
                // Logger.Log("Anim[{0}] 从所属中{1}获取颜色. Colour={2}, Anim.RemapColor={3}", pAnim.Ref.Type.Convert<AbstractTypeClass>().Ref.ID, id, color, pAnim.Ref.RemapColour);
                return 0x00423130;
            }
            return 0x004231F3;
        }


        // [Hook(HookType.AresHook, Address = 0x6F9E50, Size = 5)]
        static public unsafe void TechnoClass_Update_SHP_Berzerk(Pointer<TechnoClass> pTechno, TechnoExt ext)
        {
            ref TechnoClass rTechno = ref pTechno.Ref;
            ref TechnoTypeClass rType = ref rTechno.Type.Ref;
            if (rTechno.Berzerk && !rTechno.IsVoxel() && rTechno.Base.Base.WhatAmI() == AbstractType.Unit)
            {
                Logger.Log("{0}", rTechno.Base.Base.WhatAmI());
                ref HouseClass rHouse = ref rTechno.Owner.Ref;
                string ID = rType.Base.Base.UIName;
                string HouseID = rHouse.Type.Ref.Base.UIName;
                Logger.Log("{0}-{1} is SHP Unit.", HouseID, ID);
            }
        }

    }
}