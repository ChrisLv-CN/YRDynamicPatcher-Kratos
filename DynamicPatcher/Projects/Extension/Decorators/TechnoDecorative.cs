using DynamicPatcher;
using Extension.Ext;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Decorators
{
    public class TechnoDecorative
    {
        //[Hook(HookType.AresHook, Address = 0x6F9E50, Size = 5)]
        static public unsafe UInt32 OnUpdate(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;

            IDecorative<EventDecorator> decorative = TechnoExt.ExtMap.Find(pTechno);
            foreach (var decorator in decorative.GetDecorators())
            {
                decorator.OnUpdate();
            }

            return 0;
        }
        //[Hook(HookType.AresHook, Address = 0x701900, Size = 6)]
        static public unsafe UInt32 OnReceiveDamage(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
            var pDamage = R->Stack<Pointer<int>>(0x4);
            var distanceFromEpicenter = R->Stack<int>(0x8);
            var pWH = R->Stack<Pointer<WarheadTypeClass>>(0xC);
            var pAttacker = R->Stack<Pointer<ObjectClass>>(0x10);
            var ignoreDefenses = R->Stack<bool>(0x14);
            var preventPassengerEscape = R->Stack<bool>(0x18);
            var pAttackingHouse = R->Stack<Pointer<HouseClass>>(0x1C);

            IDecorative<EventDecorator> decorative = TechnoExt.ExtMap.Find(pTechno);
            foreach (var decorator in decorative.GetDecorators())
            {
                decorator.OnReceiveDamage(pDamage, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse);
            }

            return 0;
        }
        //[Hook(HookType.AresHook, Address = 0x6FDD50, Size = 6)]
        static public unsafe UInt32 OnFire(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
            var pTarget = R->Stack<Pointer<AbstractClass>>(0x4);
            var nWeaponIndex = R->Stack<int>(0x8);

            IDecorative<EventDecorator> decorative = TechnoExt.ExtMap.Find(pTechno);
            foreach (var decorator in decorative.GetDecorators())
            {
                decorator.OnFire(pTarget, nWeaponIndex);
            }

            return 0;
        }
    }
}
