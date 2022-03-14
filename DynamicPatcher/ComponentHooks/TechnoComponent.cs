using DynamicPatcher;
using Extension.Ext;
using Extension.Script;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentHooks
{
    public class TechnoComponentHooks
    {
        // Kratos moved to ExtensionHook
        // [Hook(HookType.AresHook, Address = 0x6F9E50, Size = 5)]
        static public unsafe UInt32 TechnoClass_Update_Components(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext.AttachedComponent.Foreach(c => c.OnUpdate());

                return 0;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return 0;
            }
        }

        // Kratos moved to ExtensionHook
        // [Hook(HookType.AresHook, Address = 0x6F6CA0, Size = 7)]
        static public unsafe UInt32 TechnoClass_Put_Script(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
                var pCoord = R->Stack<Pointer<CoordStruct>>(0x4);
                var faceDir = R->Stack<Direction>(0x8);

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext.AttachedComponent.Foreach(c => (c as IObjectScriptable)?.OnPut(pCoord.Data, faceDir));

                return 0;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return 0;
            }
        }

        // avoid hook conflict with phobos feature -- shield
        //[Hook(HookType.AresHook, Address = 0x6F6AC0, Size = 5)]
        // Kratos moved to ExtensionHook
        // [Hook(HookType.AresHook, Address = 0x6F6AC4, Size = 5)]
        static public unsafe UInt32 TechnoClass_Remove_Script(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext.AttachedComponent.Foreach(c => (c as IObjectScriptable)?.OnRemove());

                return 0;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return 0;
            }
        }

        // Kratos moved to ExtensionHook
        // [Hook(HookType.AresHook, Address = 0x701900, Size = 6)]
        static public unsafe UInt32 TechnoClass_ReceiveDamage_Script(REGISTERS* R)
        {
            try
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
                ext.AttachedComponent.Foreach(c => (c as IObjectScriptable)?.OnReceiveDamage(pDamage, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse));

                return 0;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return 0;
            }
        }

        // Kratos moved to ExtensionHook
        // [Hook(HookType.AresHook, Address = 0x6FDD50, Size = 6)]
        static public unsafe UInt32 TechnoClass_Fire(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
                var pTarget = R->Stack<Pointer<AbstractClass>>(0x4);
                var nWeaponIndex = R->Stack<int>(0x8);

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext.AttachedComponent.Foreach(c => (c as ITechnoScriptable)?.OnFire(pTarget, nWeaponIndex));

                return 0;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return 0;
            }
        }
    }
}
