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

        public unsafe void TechnoClass_Update_FixGattlingStage()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            Pointer<TechnoTypeClass> pType = pTechno.Ref.Type;
            if (pType.Ref.IsGattling && pType.Ref.WeaponStages > 1)
            {
                for (int i = 0; i < pType.Ref.WeaponStages; i++)
                {
                    int minStage = 0;
                    int maxStage = 0;
                    if (!pTechno.Ref.Veterancy.IsElite())
                    {
                        minStage = i == 0 ? 0 : pType.Ref.WeaponStage[i - 1];
                        maxStage = pType.Ref.WeaponStage[i];
                    }
                    else
                    {
                        minStage = i == 0 ? 0 : pType.Ref.EliteStage[i - 1];
                        maxStage = pType.Ref.EliteStage[i];
                    }
                    if (pTechno.Ref.GattlingValue >= minStage && pTechno.Ref.GattlingValue < maxStage && pTechno.Ref.CurrentGattlingStage != i)
                    {
                        // Logger.Log("盖特计数{0}, 盖特阶段{1}, {2}[{3} - {4}]", pTechno.Ref.GattlingValue, pTechno.Ref.CurrentGattlingStage, i, minStage, maxStage);
                        pTechno.Ref.SetCurrentWeaponStage(i);
                        break;
                    }
                }
            }
        }


    }


}