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
        public Weapon Weapon;

        private void InitWeapon()
        {
            if (null != Type.WeaponType)
            {
                this.Weapon = Type.WeaponType.CreateObject(Type);
            }
        }
    }


    [Serializable]
    public class Weapon : AttachEffectBehaviour
    {
        public WeaponType Type;

        private string token;
        private int duration;
        private TechnoExt OwnerExt;

        public Weapon(WeaponType type, AttachEffectType attachEffectType) : base(attachEffectType)
        {
            this.Type = type;
            this.token = Guid.NewGuid().ToString();
            this.duration = attachEffectType.Duration;
        }

        public override void Enable(Pointer<ObjectClass> pObject, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker)
        {
            if (pObject.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                OwnerExt = TechnoExt.ExtMap.Find(pTechno);
                ResetDuration();
            }
        }

        public override void Disable(CoordStruct location)
        {
            if (Type.WeaponDisable)
            {
                OwnerExt.DisableWeaponState.Disable(token);
            }
            if (Type.Enable)
            {
                OwnerExt.OverrideWeaponState.Disable(token);
            }
        }

        public override void ResetDuration()
        {
            if (null != OwnerExt)
            {
                int duration = AttachEffectType.HoldDuration ? -1 : AttachEffectType.Duration;
                if (Type.WeaponDisable)
                {
                    OwnerExt.DisableWeaponState.Enable(duration, token);
                }
                if (Type.Enable)
                {
                    OwnerExt.OverrideWeaponState.Enable(duration, token, Type);
                }
            }
        }

    }

}