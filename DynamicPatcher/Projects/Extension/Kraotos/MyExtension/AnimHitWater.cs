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


    public partial class AnimExt
    {

        public bool OverrideExpireAnimOnWater()
        {
            string animType = Type.ExpireAnimOnWater;

            if (!string.IsNullOrEmpty(animType))
            {
                // Logger.Log($"{Game.CurrentFrame} 试图接管 落水动画 {animType}");
                if (!"none".Equals(animType.ToLower()))
                {
                    Pointer<AnimTypeClass> pNewType = AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(animType);
                    if (!pNewType.IsNull)
                    {
                        // Logger.Log($"{Game.CurrentFrame} 试图创建新的落水动画 {animType}");
                        Pointer<AnimClass> pNewAnim = YRMemory.Create<AnimClass>(pNewType, OwnerObject.Ref.Base.Base.GetCoords());
                        pNewAnim.Ref.Owner = OwnerObject.Ref.Owner;
                    }
                }
                return true; // skip create anim
            }
            return false;
        }
    }

    public partial class AnimTypeExt
    {

        public string ExpireAnimOnWater;

        private void ReadExpireAnimOnWater(INIReader reader, string section)
        {
            string expireAnimOnWater = null;
            if (reader.ReadNormal(section, "ExpireAnimOnWater", ref expireAnimOnWater))
            {
                ExpireAnimOnWater = expireAnimOnWater;
            }
        }
    }

}