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
        public bool IsAlternateColor;

        private void InitElectricType()
        {
            IsAlternateColor = false;
        }

        private void ReadElectricType(INIReader reader, string section)
        {

            bool isAlternateColor = false;
            if (reader.ReadNormal(section, "Electric.IsAlternateColor", ref isAlternateColor))
            {
                this.IsAlternateColor = isAlternateColor;
            }
        }
    }


}