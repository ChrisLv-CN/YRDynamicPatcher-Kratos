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
        public LaserType LaserType;

        private void InitLaserType()
        {
            LaserType = new LaserType(true);
            LaserType.IsHouseColor = false;
            LaserType.Fade = true;
        }

        private void ReadLaserTrailType(INIReader reader, string section)
        {
            ColorStruct innerColor = default;
            if (ExHelper.ReadColorStruct(reader, section, "Laser.InnerColor", ref innerColor))
            {
                this.LaserType.InnerColor = innerColor;
                this.LaserType.IsHouseColor = false;
            }

            ColorStruct outerColor = default;
            if (ExHelper.ReadColorStruct(reader, section, "Laser.OuterColor", ref outerColor))
            {
                this.LaserType.OuterColor = outerColor;
            }

            ColorStruct outerSpread = default;
            if (ExHelper.ReadColorStruct(reader, section, "Laser.OuterSpread", ref outerSpread))
            {
                this.LaserType.OuterSpread = outerSpread;
            }

            bool isHouseColor = false;
            if (reader.ReadNormal(section, "Laser.IsHouseColor", ref isHouseColor))
            {
                this.LaserType.IsHouseColor = isHouseColor;
            }

            bool isSupported = false;
            if (reader.ReadNormal(section, "Laser.IsSupported", ref isSupported))
            {
                this.LaserType.IsSupported = isSupported;
            }

            bool fade = false;
            if (reader.ReadNormal(section, "Laser.Fade", ref fade))
            {
                this.LaserType.Fade = fade;
            }

            int duration = 0;
            if (reader.ReadNormal(section, "Laser.Duration", ref duration))
            {
                this.LaserType.Duration = duration;
            }

            int thickness = 0;
            if (reader.ReadNormal(section, "Laser.Thickness", ref thickness))
            {
                this.LaserType.Thickness = thickness;
            }

        }
    }


}