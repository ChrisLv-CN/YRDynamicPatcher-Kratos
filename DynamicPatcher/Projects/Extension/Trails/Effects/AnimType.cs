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
        public string StartDrivingAnim;
        public string WhileDrivingAnim;
        public string StopDrivingAnim;

        private void InitAnimType()
        {
            StartDrivingAnim = null;
            WhileDrivingAnim = null;
            StopDrivingAnim = null;
        }

        private void ReadAnimType(INIReader reader, string section)
        {

            string whileAnim = null;
            if (reader.ReadNormal(section, "Anim.While", ref whileAnim))
            {
                if (!"none".Equals(whileAnim.Trim().ToLower()))
                {
                    this.WhileDrivingAnim = whileAnim;
                }
            }

            string startAnim = null;
            if (reader.ReadNormal(section, "Anim.Start", ref startAnim))
            {
                if (!"none".Equals(startAnim.Trim().ToLower()))
                {
                    this.StartDrivingAnim = startAnim;
                }
            }

            string stopAnim = null;
            if (reader.ReadNormal(section, "Anim.Stop", ref stopAnim))
            {
                if (!"none".Equals(stopAnim.Trim().ToLower()))
                {
                    this.StopDrivingAnim = stopAnim;
                }
            }
        }
    }


}