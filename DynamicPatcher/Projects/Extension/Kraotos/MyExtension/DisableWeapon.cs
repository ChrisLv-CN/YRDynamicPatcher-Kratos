using System.Threading;
using System.IO;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    public partial class TechnoExt
    {
        public AEState<DisableWeaponType> DisableWeaponState => AttachEffectManager.DisableWeaponState;

        public unsafe bool TechnoClass_CanFire_Disable(Pointer<AbstractClass> pTarget, Pointer<WeaponTypeClass> pWeapon)
        {
            return DisableWeaponState.IsActive();
        }

    }


}