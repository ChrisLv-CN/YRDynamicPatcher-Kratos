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

    public partial class RulesExt : ITypeExtension
    {
        [INILoadAction]
        public void LoadINI(Pointer<CCINIClass> pINI)
        {
            INIReader reader = new INIReader(pINI);

            ReadAllowAnimDamageTakeOverByKratos(reader);
            ReadAllowDamageIfDebrisHitWater(reader);

            ReadAircraftPut(reader);
            ReadChaosAnim(reader);
            ReadDamageText(reader);
            ReadHealthText(reader);
        }

        public void Save(IStream stream)
        {
        }
        
        public void Load(IStream stream)
        {
        }

    }

}
