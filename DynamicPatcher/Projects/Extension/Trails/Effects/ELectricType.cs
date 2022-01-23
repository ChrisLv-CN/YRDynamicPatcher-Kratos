using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    public partial class TrailType
    {
        public BoltType BoltType;

        private void InitElectricType()
        {
            BoltType = new BoltType(false);
        }

        private void ReadElectricType(INIReader reader, string section)
        {

            bool isAlternateColor = false;
            if (reader.ReadNormal(section, "Electric.IsAlternateColor", ref isAlternateColor))
            {
                this.BoltType.IsAlternateColor = isAlternateColor;
            }

            ColorStruct color1 = default;
            if (ExHelper.ReadColorStruct(reader, section, "Bolt.Color1", ref color1))
            {
                this.BoltType.Color1 = color1;
            }

            ColorStruct color2 = default;
            if (ExHelper.ReadColorStruct(reader, section, "Bolt.Color2", ref color2))
            {
                this.BoltType.Color2 = color2;
            }

            ColorStruct color3 = default;
            if (ExHelper.ReadColorStruct(reader, section, "Bolt.Color3", ref color3))
            {
                this.BoltType.Color3 = color3;
            }

            bool disable1 = false;
            if (reader.ReadNormal(section, "Bolt.Disable1", ref disable1))
            {
                this.BoltType.Disable1 = disable1;
            }

            bool disable2 = false;
            if (reader.ReadNormal(section, "Bolt.Disable2", ref disable2))
            {
                this.BoltType.Disable2 = disable2;
            }

            bool disable3 = false;
            if (reader.ReadNormal(section, "Bolt.Disable3", ref disable3))
            {
                this.BoltType.Disable3 = disable3;
            }
        }
    }


}