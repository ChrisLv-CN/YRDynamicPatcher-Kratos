using DynamicPatcher;
using Extension.Script;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    [Serializable]
    public partial class TrailTypeExt : Extension<TrailType>
    {

        public static string MainSection = "TrailTypes";

        public TrailTypeExt(Pointer<TrailType> OwnerObject) : base(OwnerObject) { }

        public static void LoadFromINIList(Pointer<CCINIClass> pINI)
        {
            TrailType.LoadFromINIList(pINI, MainSection);
        }

    }
}
