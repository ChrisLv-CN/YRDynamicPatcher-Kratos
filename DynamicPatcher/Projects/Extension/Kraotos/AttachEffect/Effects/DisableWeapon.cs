using DynamicPatcher;
using Extension.Ext;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    public partial class AttachEffect
    {
        public DisableWeapon DisableWeapon;

        private void InitDisableWeapon()
        {
            if (null != Type.DisableWeaponType)
            {
                this.DisableWeapon = Type.DisableWeaponType.CreateObject(Type);
                RegisterAction(DisableWeapon);
            }
        }
    }


    [Serializable]
    public class DisableWeapon : StateEffect<DisableWeapon, DisableWeaponType>
    {
        public override IAEState GetState(Pointer<ObjectClass> pOwner, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker)
        {
            return OwnerAEM.DisableWeaponState;
        }

    }

}