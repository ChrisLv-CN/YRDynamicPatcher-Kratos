using DynamicPatcher;
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
    public partial class AnimExt
    {

        public string MyExtensionTest = nameof(MyExtensionTest);

        public unsafe void OnInit()
        {

        }

        public unsafe void OnUpdate()
        {
            AnimClass_Update_SuperWeapon();
        }

        public unsafe void OnRender()
        {
            
        }

        public unsafe void OnUnInit()
        {

        }

    }

    public partial class AnimTypeExt
    {

        [INILoadAction]
        public void LoadINI(Pointer<CCINIClass> pINI)
        {
            // rules reader
            INIReader reader = new INIReader(pINI);
            string section = OwnerObject.Ref.Base.Base.ID;

            ReadFireSuperWeapon(reader, section);
        }

    }

}
