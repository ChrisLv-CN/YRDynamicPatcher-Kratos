using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;


namespace Extension.Ext
{

    public partial class WarheadTypeExt
    {

        public bool CanAffectHouse(Pointer<HouseClass> pOwnerHouse, Pointer<HouseClass> pTargetHouse)
        {
            if (!pOwnerHouse.IsNull && !pTargetHouse.IsNull)
            {
                if (pOwnerHouse == pTargetHouse)
                {
                    return OwnerObject.Ref.AffectsAllies || Ares.AffectsOwner;
                }
                else if (pOwnerHouse.Ref.IsAlliedWith(pTargetHouse))
                {
                    return OwnerObject.Ref.AffectsAllies;
                }
                else
                {
                    return Ares.AffectsEnemies;
                }
            }
            return true;
        }

        [INILoadAction]
        public void LoadINI(Pointer<CCINIClass> pINI)
        {
            INIReader reader = new INIReader(pINI);
            string section = OwnerObject.Ref.Base.ID;

            ReadAffectsFlags(reader, section);
            ReadAresFlags(reader, section);
            ReadAttachEffect(reader, section);
            ReadDamageText(reader, section);
            ReadTauntWarhead(reader, section);
        }

        [LoadAction]
        public void Load(IStream stream) { }

        [SaveAction]
        public void Save(IStream stream) { }
    }
}
