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
    public partial class AttachEffectTypeExt : Extension<AttachEffectType>
    {

        public static string MainSection = "AttachEffectTypes";

        public AttachEffectTypeExt(Pointer<AttachEffectType> OwnerObject) : base(OwnerObject) { }

        public static void LoadFromINIList(Pointer<CCINIClass> pINI)
        {
            TrailType.LoadFromINIList(pINI, MainSection);
        }

    }
}
