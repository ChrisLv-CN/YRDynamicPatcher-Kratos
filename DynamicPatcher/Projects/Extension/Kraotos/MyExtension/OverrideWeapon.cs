using System.Net.NetworkInformation;
using System.Threading;
using System.IO;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Extension.Ext
{


    public partial class TechnoExt
    {
        public OverrideWeaponState OverrideWeaponState => AttachEffectManager.OverrideWeaponState;

        
        public unsafe void TechnoClass_Init_OverrideWeapon()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (null != Type.OverrideWeaponData && Type.OverrideWeaponData.Enable)
            {
                OverrideWeaponState.Enable(Type.OverrideWeaponData);
            }

        }

    }

    public partial class TechnoTypeExt
    {

        public OverrideWeaponType OverrideWeaponData;

        /// <summary>
        /// [TechnoType]
        /// OverrideWeapon=0
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadOverrideWeapon(INIReader reader, string section)
        {
            // Override weapon
            OverrideWeaponType temp = new OverrideWeaponType();
            if (temp.TryReadType(reader, section))
            {
                this.OverrideWeaponData = temp;
            }
            else
            {
                temp = null;
            }

        }
    }

}