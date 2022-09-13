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
        public string ParticleSystem;

        private void InitParticleType()
        {
            ParticleSystem = null;
        }

        private void ReadParticleType(INIReader reader, string section)
        {

            string particleSystem = null;
            if (reader.ReadNormal(section, "ParticleSystem", ref particleSystem))
            {
                if ("none" != particleSystem.Trim().ToLower())
                {
                    this.ParticleSystem = particleSystem;
                }
            }

        }
    }


}