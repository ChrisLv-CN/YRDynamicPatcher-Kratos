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
        public TransformType TransformType;

        private void ReadTransformType(INIReader reader, string section)
        {
            TransformType type = new TransformType();
            if (type.TryReadType(reader, section))
            {
                this.TransformType = type;
            }
        }

    }

    /// <summary>
    /// 变身类型
    /// </summary>
    [Serializable]
    public class TransformType : EffectType<Transform>
    {

        public string ToType;

        public TransformType()
        {
            this.ToType = null;
        }

        public override bool TryReadType(INIReader reader, string section)
        {

            string type = null;
            if (reader.ReadNormal(section, "Transform.Type", ref type))
            {
                if (!string.IsNullOrEmpty(type) && !"none".Equals(type.Trim().ToLower()))
                {
                    this.Enable = true;
                    this.ToType = type;
                }
            }

            return this.Enable;
        }

    }

}