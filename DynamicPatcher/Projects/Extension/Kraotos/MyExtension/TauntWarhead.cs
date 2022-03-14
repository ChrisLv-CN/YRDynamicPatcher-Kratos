using System.Drawing;
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
        public unsafe void TechnoClass_ReceiveDamage_TauntWarhead(Pointer<int> pDamage, int DistanceFromEpicenter, Pointer<WarheadTypeClass> pWH,
            Pointer<ObjectClass> pAttacker, bool IgnoreDefenses, bool PreventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        {
            WarheadTypeExt whExt = WarheadTypeExt.ExtMap.Find(pWH);
            if (null != whExt && whExt.TauntWarhead && !pAttacker.IsNull)
            {
                int weaponCount = OwnerObject.Ref.Type.Ref.WeaponCount;
                if (weaponCount == 0)
                {
                    weaponCount = 2;
                }
                bool canFire = false;
                for (int i = 0; i < weaponCount; i++)
                {
                    FireError fireError = OwnerObject.Ref.GetFireErrorWithoutRange(pAttacker.Convert<AbstractClass>(), i);
                    if (fireError != FireError.ILLEGAL && fireError != FireError.CANT)
                    {
                        canFire = true;
                        break;
                    }
                }
                if (canFire)
                {
                    OwnerObject.Ref.SetTarget(pAttacker.Convert<AbstractClass>());
                }
            }
        }

    }

    public partial class WarheadTypeExt
    {

        public bool TauntWarhead;

        /// <summary>
        /// [WarheadType]
        /// Taunt=no
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadTauntWarhead(INIReader reader, string section)
        {
            bool taunt = false;
            if (reader.ReadNormal(section, "Taunt", ref taunt))
            {
                TauntWarhead = taunt;
            }

        }
    }

}