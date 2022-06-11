using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using PatcherYRpp.Utilities;
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

        public unsafe void TechnoClass_ReceiveDamage_ClearTarget(Pointer<int> pDamage, int distanceFromEpicenter, Pointer<WarheadTypeClass> pWH,
            Pointer<ObjectClass> pAttacker, bool ignoreDefenses, bool preventPassengerEscape, Pointer<HouseClass> pAttackingHouse,
            WarheadTypeExt whExt, int realDamage)
        {
            if (!OwnerObject.Ref.Target.IsNull && whExt.ClearTarget)
            {
                OwnerObject.Ref.Target = IntPtr.Zero;
                OwnerObject.Ref.SetTarget(IntPtr.Zero);
                // OwnerObject.Convert<MissionClass>().Ref.QueueMission(Mission.Stop, true);
                if (!OwnerObject.Ref.SpawnManager.IsNull)
                {
                    OwnerObject.Ref.SpawnManager.Ref.Destination = IntPtr.Zero;
                    OwnerObject.Ref.SpawnManager.Ref.Target = IntPtr.Zero;
                    OwnerObject.Ref.SpawnManager.Ref.SetTarget(IntPtr.Zero);
                }
            }
        }


    }

    public partial class WarheadTypeExt
    {
        public bool ClearTarget = false;

        /// <summary>
        /// [WarheadType]
        /// ClearTarget=no
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadClearTarget(INIReader reader, string section)
        {
            bool clear = false;
            if (reader.ReadNormal(section, "ClearTarget", ref clear))
            {
                this.ClearTarget = clear;
            }
        }
    }

}