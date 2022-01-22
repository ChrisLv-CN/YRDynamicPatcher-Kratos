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
            if (TransformType.ReadTransformType(reader, section, out TransformType transformType))
            {
                this.TransformType = transformType;
            }
        }

    }

    /// <summary>
    /// 变身类型
    /// </summary>
    [Serializable]
    public class TransformType : IEffectType<Transform>
    {

        public string ToType;

        public TransformType()
        {
            this.ToType = null;
        }

        public Transform CreateObject(AttachEffectType attachEffectType)
        {
            return new Transform(this, attachEffectType);
        }

        public static bool ReadTransformType(INIReader reader, string section, out TransformType transformType)
        {
            transformType = null;

            string type = null;
            if (reader.ReadNormal(section, "Transform.Type", ref type))
            {
                if (!string.IsNullOrEmpty(type) && !"none".Equals(type.Trim().ToLower()))
                {
                    if (null == transformType)
                    {
                        transformType = new TransformType();
                    }
                    transformType.ToType = type;
                }
            }

            return null != transformType;
        }

    }

}