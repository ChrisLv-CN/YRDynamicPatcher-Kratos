using DynamicPatcher;
using Extension.Ext;
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


    public partial class AttachEffectType
    {
        public DeselectType DeselectType;

        private void ReadDeselectType(INIReader reader, string section)
        {
            DeselectType type = new DeselectType();
            if (type.TryReadType(reader, section))
            {
                this.DeselectType = type;
            }
        }

    }

    /// <summary>
    /// 礼物盒
    /// </summary>
    [Serializable]
    public class DeselectType : EffectType<Deselect>, IAEStateData
    {


        public override bool TryReadType(INIReader reader, string section)
        {

            ReadCommonType(reader, section, "Select.");

            
            bool disable = false;
            if (reader.ReadNormal(section, "Select.Disable", ref disable))
            {
                this.Enable = disable;
            }

            return this.Enable;
        }


    }

}