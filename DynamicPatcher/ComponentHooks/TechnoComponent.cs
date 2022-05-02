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
        public static unsafe UInt32 TechnoClass_Update_Components(REGISTERS* R)
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
        public static unsafe UInt32 TechnoClass_Put_Components(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
                var pCoord = R->Stack<Pointer<CoordStruct>>(0x4);
                var faceDirValue8 = R->Stack<short>(0x8);

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext.AttachedComponent.Foreach(c => (c as IObjectScriptable)?.OnPut(pCoord.Data, faceDirValue8));

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
        public static unsafe UInt32 TechnoClass_Remove_Components(REGISTERS* R)
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
        public static unsafe UInt32 TechnoClass_ReceiveDamage_Components(REGISTERS* R)
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
        public static unsafe UInt32 TechnoClass_Fire_Components(REGISTERS* R)
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

        #region Render
        public static UInt32 TechnoClass_Render_Components(Pointer<TechnoClass> pTechno)
        {
            try
            {
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext.OnRender();
                ext.AttachedComponent.Foreach(c => c.OnRender());

                return 0;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return 0;
            }
        }
        [Hook(HookType.AresHook, Address = 0x4144B0, Size = 5)]
        public static unsafe UInt32 AircraftClass_Render_Components(REGISTERS* R)
        {
            Pointer<AircraftClass> pAircraft = (IntPtr)R->ECX;
            return TechnoClass_Render_Components(pAircraft.Convert<TechnoClass>());
        }
        [Hook(HookType.AresHook, Address = 0x43D290, Size = 5)]
        public static unsafe UInt32 BuildingClass_Render_Components(REGISTERS* R)
        {
            Pointer<BuildingClass> pBuilding = (IntPtr)R->ECX;
            return TechnoClass_Render_Components(pBuilding.Convert<TechnoClass>());
        }
        [Hook(HookType.AresHook, Address = 0x518F90, Size = 7)]
        public static unsafe UInt32 InfantryClass_Render_Components(REGISTERS* R)
        {
            Pointer<InfantryClass> pInfantry = (IntPtr)R->ECX;
            return TechnoClass_Render_Components(pInfantry.Convert<TechnoClass>());
        }
        [Hook(HookType.AresHook, Address = 0x73CEC0, Size = 5)]
        public static unsafe UInt32 UnitClass_Render_Components(REGISTERS* R)
        {
            Pointer<UnitClass> pUnit = (IntPtr)R->ECX;
            return TechnoClass_Render_Components(pUnit.Convert<TechnoClass>());
        }
        #endregion

    }
}
