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
        public OverrideWeapon OverrideWeapon;

        private void InitOverrideWeapon()
        {
            if (null != Type.OverrideWeaponType)
            {
                this.OverrideWeapon = Type.OverrideWeaponType.CreateObject(Type);
                RegisterAction(OverrideWeapon);
            }
        }
    }


    [Serializable]
    public class OverrideWeapon : StateEffect<OverrideWeapon, OverrideWeaponType>
    {
        public override IAEState GetState()
        {
            return OwnerAEM.OverrideWeaponState;
        }

    }

}