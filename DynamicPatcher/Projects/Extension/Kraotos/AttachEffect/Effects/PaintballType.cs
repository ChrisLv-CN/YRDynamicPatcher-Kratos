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
        public PaintballType PaintballType;

        private void ReadPaintballType(INIReader reader, string section)
        {
            if (PaintballType.ReadPaintballType(reader, section, out PaintballType paintballType))
            {
                this.PaintballType = paintballType;
            }
        }

    }

    /// <summary>
    /// 染色弹
    /// </summary>
    [Serializable]
    public class PaintballType : IEffectType<Paintball>
    {
        public ColorStruct Color; // 颜色
        public bool IsHouseColor; // 使用所属色

        public PaintballType()
        {
            this.Color = default;
            this.IsHouseColor = false;
        }

        public Paintball CreateObject(AttachEffectType attachEffectType)
        {
            return new Paintball(this, attachEffectType);
        }

        public static bool ReadPaintballType(INIReader reader, string section, out PaintballType paintballType)
        {
            paintballType = null;

            ColorStruct color = default;
            if (ExHelper.ReadColorStruct(reader, section, "Paintball.Color", ref color))
            {
                if (null == paintballType)
                {
                    paintballType = new PaintballType();
                }
                paintballType.Color = color;

                bool isHouseColor = false;
                if (reader.ReadNormal(section, "Paintball.IsHouseColor", ref isHouseColor))
                {
                    paintballType.IsHouseColor = isHouseColor;
                }
            }

            return null != paintballType;
        }

    }

}