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
        public BeamType BeamType;

        private void InitBeamType()
        {
            BeamType = new BeamType(RadBeamType.RadBeam);
        }

        private void ReadBeamTrailType(INIReader reader, string section)
        {

            ColorStruct customColor = default;
            if (ExHelper.ReadColorStruct(reader, section, "Beam.Color", ref customColor))
            {
                this.BeamType.BeamColor = customColor;
            }

            int period = 0;
            if (reader.ReadNormal(section, "Beam.Period", ref period))
            {
                this.BeamType.Period = period;
            }

            double amplitude = 0;
            if (reader.ReadNormal(section, "Beam.Amplitude", ref amplitude))
            {
                this.BeamType.Amplitude = amplitude;
            }

        }

    }



}