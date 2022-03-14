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
            if (AttachStatusType.ReadAttachStatusType(reader, section, out AttachStatusType attachStatusType))
            {
                this.AttachStatusType = attachStatusType;
            }
        }

    }

    /// <summary>
    /// AE属性
    /// </summary>
    [Serializable]
    public class AttachStatusType : CrateMultiplier, IEffectType<AttachStatus>
    {

        public AttachStatus CreateObject(AttachEffectType attachEffectType)
        {
            return new AttachStatus(this, attachEffectType);
        }

        public static bool ReadAttachStatusType(INIReader reader, string section, out AttachStatusType attachStatusType)
        {
            attachStatusType = null;

            double firepowerMultiplier = 1.0;
            if (reader.ReadNormal(section, "Status.FirepowerMultiplier", ref firepowerMultiplier))
            {
                if (null == attachStatusType)
                {
                    attachStatusType = new AttachStatusType();
                }
                attachStatusType.FirepowerMultiplier = firepowerMultiplier;
            }

            double armorMultiplier = 1.0;
            if (reader.ReadNormal(section, "Status.ArmorMultiplier", ref armorMultiplier))
            {
                if (null == attachStatusType)
                {
                    attachStatusType = new AttachStatusType();
                }
                attachStatusType.ArmorMultiplier = armorMultiplier;
            }

            double speedMultiplier = 1.0;
            if (reader.ReadNormal(section, "Status.SpeedMultiplier", ref speedMultiplier))
            {
                if (null == attachStatusType)
                {
                    attachStatusType = new AttachStatusType();
                }
                attachStatusType.SpeedMultiplier = speedMultiplier;
            }

            double rofMultiplier = 1.0;
            if (reader.ReadNormal(section, "Status.ROFMultiplier", ref rofMultiplier))
            {
                if (null == attachStatusType)
                {
                    attachStatusType = new AttachStatusType();
                }
                attachStatusType.ROFMultiplier = rofMultiplier;
            }

            bool cloakable = false;
            if (reader.ReadNormal(section, "Status.Cloakable", ref cloakable))
            {
                if (null == attachStatusType)
                {
                    attachStatusType = new AttachStatusType();
                }
                attachStatusType.Cloakable = cloakable;
            }

            bool forceDecloak = false;
            if (reader.ReadNormal(section, "Status.ForceDecloak", ref forceDecloak))
            {
                if (null == attachStatusType)
                {
                    attachStatusType = new AttachStatusType();
                }
                attachStatusType.ForceDecloak = forceDecloak;
            }

            return null != attachStatusType;
        }

    }

}