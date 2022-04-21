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
        public AttachStatusType AttachStatusType;

        private void ReadAttachStatusType(INIReader reader, string section)
        {
            AttachStatusType type = new AttachStatusType();
            if (type.TryReadType(reader, section))
            {
                this.AttachStatusType = type;
            }
        }

    }

    /// <summary>
    /// AE属性
    /// </summary>
    [Serializable]
    public class AttachStatusType : EffectType<AttachStatus>
    {

        public double FirepowerMultiplier;
        public double ArmorMultiplier;
        public double SpeedMultiplier;
        public double ROFMultiplier;
        public bool Cloakable;
        public bool ForceDecloak;

        public AttachStatusType()
        {
            this.Enable = false;

            this.FirepowerMultiplier = 1.0;
            this.ArmorMultiplier = 1.0;
            this.SpeedMultiplier = 1.0;
            this.ROFMultiplier = 1.0;
            this.Cloakable = false;
            this.ForceDecloak = false;
        }

        public override bool TryReadType(INIReader reader, string section)
        {

            double firepowerMultiplier = 1.0;
            if (reader.ReadNormal(section, "Status.FirepowerMultiplier", ref firepowerMultiplier))
            {
                this.Enable = true;
                this.FirepowerMultiplier = firepowerMultiplier;
            }

            double armorMultiplier = 1.0;
            if (reader.ReadNormal(section, "Status.ArmorMultiplier", ref armorMultiplier))
            {
                this.Enable = true;
                this.ArmorMultiplier = armorMultiplier;
            }

            double speedMultiplier = 1.0;
            if (reader.ReadNormal(section, "Status.SpeedMultiplier", ref speedMultiplier))
            {
                this.Enable = true;
                this.SpeedMultiplier = speedMultiplier;
            }

            double rofMultiplier = 1.0;
            if (reader.ReadNormal(section, "Status.ROFMultiplier", ref rofMultiplier))
            {
                this.Enable = true;
                this.ROFMultiplier = rofMultiplier;
            }

            bool cloakable = false;
            if (reader.ReadNormal(section, "Status.Cloakable", ref cloakable))
            {
                this.Enable = true;
                this.Cloakable = cloakable;
            }

            bool forceDecloak = false;
            if (reader.ReadNormal(section, "Status.ForceDecloak", ref forceDecloak))
            {
                this.Enable = true;
                this.ForceDecloak = forceDecloak;
            }

            return this.Enable;
        }

    }

}