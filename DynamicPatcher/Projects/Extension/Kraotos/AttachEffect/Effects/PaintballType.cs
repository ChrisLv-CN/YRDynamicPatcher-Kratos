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
            PaintballType type = new PaintballType();
            if (type.TryReadType(reader, section))
            {
                this.PaintballType = type;
            }
        }

    }

    /// <summary>
    /// 染色弹
    /// </summary>
    [Serializable]
    public class PaintballType : EffectType<Paintball>, IAEStateData
    {
        public ColorStruct Color; // 颜色
        public bool IsHouseColor; // 使用所属色
        public float BrightMultiplier; // 亮度系数

        public PaintballType()
        {
            this.Color = default;
            this.IsHouseColor = false;
            this.BrightMultiplier = 1.0f;
        }

        public override bool TryReadType(INIReader reader, string section)
        {

            ReadCommonType(reader, section, "Paintball.");

            ColorStruct color = default;
            if (ExHelper.ReadColorStruct(reader, section, "Paintball.Color", ref color))
            {
                this.Enable = true;
                this.Color = color;

                bool isHouseColor = false;
                if (reader.ReadNormal(section, "Paintball.IsHouseColor", ref isHouseColor))
                {
                    this.IsHouseColor = isHouseColor;
                }
            }

            float bright = 1;
            if (reader.ReadNormal(section, "Paintball.BrightMultiplier", ref bright))
            {
                this.Enable = true;
                if (bright < 0.0f)
                {
                    bright = 0.0f;
                }
                else if (bright > 2.0f)
                {
                    bright = 2.0f;
                }
                this.BrightMultiplier = bright;
            }

            return this.Enable;
        }


    }

}