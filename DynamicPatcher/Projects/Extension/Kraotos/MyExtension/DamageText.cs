using System.Net.Mime;
using System.Reflection;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using PatcherYRpp.FileFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    [Serializable]
    public class DamageTextControlData
    {
        public bool Hidden;

        public DamageTextData DamageData;
        public DamageTextData RepairData;

        public DamageTextControlData()
        {
            this.Hidden = false;
            this.DamageData = new DamageTextData(true);
            this.RepairData = new DamageTextData(false);
        }

        public void ReadDamageText(INIReader reader, string section)
        {
            bool hidden = false;
            if (reader.ReadNormal(section, "DamageText.Hidden", ref hidden))
            {
                this.Hidden = hidden;
                this.DamageData.Hidden = hidden;
                this.RepairData.Hidden = hidden;
            }
            hidden = false;
            if (reader.ReadNormal(section, "DamageText.Damage.Hidden", ref hidden))
            {
                this.DamageData.Hidden = hidden;
            }
            hidden = false;
            if (reader.ReadNormal(section, "DamageText.Repair.Hidden", ref hidden))
            {
                this.RepairData.Hidden = hidden;
            }

            Point2D xOffset = default;
            if (ExHelper.ReadPoint2D(reader, section, "DamageText.XOffset", ref xOffset))
            {
                this.DamageData.XOffset = xOffset;
                this.RepairData.XOffset = xOffset;
            }
            xOffset = default;
            if (ExHelper.ReadPoint2D(reader, section, "DamageText.Damage.XOffset", ref xOffset))
            {
                this.DamageData.XOffset = xOffset;
            }
            xOffset = default;
            if (ExHelper.ReadPoint2D(reader, section, "DamageText.Repair.XOffset", ref xOffset))
            {
                this.RepairData.XOffset = xOffset;
            }

            Point2D yOffset = default;
            if (ExHelper.ReadPoint2D(reader, section, "DamageText.YOffset", ref yOffset))
            {
                Point2D offset = yOffset;
                if (yOffset.X > yOffset.Y)
                {
                    offset.X = yOffset.Y;
                    offset.Y = yOffset.X;
                }
                this.DamageData.YOffset = offset;
                this.RepairData.YOffset = offset;
            }
            yOffset = default;
            if (ExHelper.ReadPoint2D(reader, section, "DamageText.Damage.YOffset", ref yOffset))
            {
                Point2D offset = yOffset;
                if (yOffset.X > yOffset.Y)
                {
                    offset.X = yOffset.Y;
                    offset.Y = yOffset.X;
                }
                this.DamageData.YOffset = offset;
            }
            yOffset = default;
            if (ExHelper.ReadPoint2D(reader, section, "DamageText.Repair.YOffset", ref yOffset))
            {
                Point2D offset = yOffset;
                if (yOffset.X > yOffset.Y)
                {
                    offset.X = yOffset.Y;
                    offset.Y = yOffset.X;
                }
                this.RepairData.YOffset = offset;
            }

            int roll = 1;
            if (reader.ReadNormal(section, "DamageText.RollSpeed", ref roll))
            {
                this.DamageData.RollSpeed = roll;
                this.RepairData.RollSpeed = roll;
            }
            roll = 1;
            if (reader.ReadNormal(section, "DamageText.Damage.RollSpeed", ref roll))
            {
                this.DamageData.RollSpeed = roll;
            }
            roll = 1;
            if (reader.ReadNormal(section, "DamageText.Repair.RollSpeed", ref roll))
            {
                this.RepairData.RollSpeed = roll;
            }

            int duration = 0;
            if (reader.ReadNormal(section, "DamageText.Duration", ref duration))
            {
                this.DamageData.Duration = duration;
                this.RepairData.Duration = duration;
            }
            duration = 0;
            if (reader.ReadNormal(section, "DamageText.Damage.Duration", ref duration))
            {
                this.DamageData.Duration = duration;
            }
            duration = 0;
            if (reader.ReadNormal(section, "DamageText.Repair.Duration", ref duration))
            {
                this.RepairData.Duration = duration;
            }

            // 文字设置
            Point2D shadowOffset = default;
            if (ExHelper.ReadPoint2D(reader, section, "DamageText.ShadowOffset", ref shadowOffset))
            {
                this.DamageData.ShadowOffset = shadowOffset;
                this.RepairData.ShadowOffset = shadowOffset;
            }
            shadowOffset = default;
            if (ExHelper.ReadPoint2D(reader, section, "DamageText.Damage.ShadowOffset", ref shadowOffset))
            {
                this.DamageData.ShadowOffset = shadowOffset;
            }
            shadowOffset = default;
            if (ExHelper.ReadPoint2D(reader, section, "DamageText.Repair.ShadowOffset", ref shadowOffset))
            {
                this.RepairData.ShadowOffset = shadowOffset;
            }

            ColorStruct color = default;
            if (ExHelper.ReadColorStruct(reader, section, "DamageText.Color", ref color))
            {
                this.DamageData.Color = color;
                this.RepairData.Color = color;
            }
            color = default;
            if (ExHelper.ReadColorStruct(reader, section, "DamageText.Damage.Color", ref color))
            {
                this.DamageData.Color = color;
            }
            color = default;
            if (ExHelper.ReadColorStruct(reader, section, "DamageText.Repair.Color", ref color))
            {
                this.RepairData.Color = color;
            }

            ColorStruct shadowColor = default;
            if (ExHelper.ReadColorStruct(reader, section, "DamageText.ShadowColor", ref shadowColor))
            {
                this.DamageData.ShadowColor = shadowColor;
                this.RepairData.ShadowColor = shadowColor;
            }
            color = default;
            if (ExHelper.ReadColorStruct(reader, section, "DamageText.Damage.ShadowColor", ref shadowColor))
            {
                this.DamageData.ShadowColor = shadowColor;
            }
            color = default;
            if (ExHelper.ReadColorStruct(reader, section, "DamageText.Repair.ShadowColor", ref shadowColor))
            {
                this.RepairData.ShadowColor = shadowColor;
            }

            // SHP设置
            bool useSHP = false;
            if (reader.ReadNormal(section, "DamageText.UseSHP", ref useSHP))
            {
                this.DamageData.UseSHP = useSHP;
                this.RepairData.UseSHP = useSHP;
            }
            useSHP = false;
            if (reader.ReadNormal(section, "DamageText.Damage.UseSHP", ref useSHP))
            {
                this.DamageData.UseSHP = useSHP;
            }
            useSHP = false;
            if (reader.ReadNormal(section, "DamageText.Repair.UseSHP", ref useSHP))
            {
                this.RepairData.UseSHP = useSHP;
            }

            string fileName = null;
            if (reader.ReadNormal(section, "DamageText.SHP", ref fileName))
            {
                string file = fileName;
                if (!string.IsNullOrEmpty(fileName) && !(file = fileName.ToLower()).Equals("pips.shp"))
                {
                    this.DamageData.CustomSHP = true;
                    this.DamageData.SHPFileName = file;
                    this.RepairData.CustomSHP = true;
                    this.RepairData.SHPFileName = file;
                }
            }
            fileName = null;
            if (reader.ReadNormal(section, "DamageText.Damage.SHP", ref fileName))
            {
                string file = fileName;
                if (!string.IsNullOrEmpty(fileName) && !(file = fileName.ToLower()).Equals("pips.shp"))
                {
                    this.DamageData.CustomSHP = true;
                    this.DamageData.SHPFileName = file;
                }
            }
            fileName = null;
            if (reader.ReadNormal(section, "DamageText.Repair.SHP", ref fileName))
            {
                string file = fileName;
                if (!string.IsNullOrEmpty(fileName) && !(file = fileName.ToLower()).Equals("pips.shp"))
                {
                    this.RepairData.CustomSHP = true;
                    this.RepairData.SHPFileName = file;
                }
            }

            Point2D imgSize = default;
            if (ExHelper.ReadPoint2D(reader, section, "DamageText.ImageSize", ref imgSize))
            {
                this.DamageData.ImageSize = imgSize;
                this.RepairData.ImageSize = imgSize;
            }
            imgSize = default;
            if (ExHelper.ReadPoint2D(reader, section, "DamageText.Damage.ImageSize", ref imgSize))
            {
                this.DamageData.ImageSize = imgSize;
            }
            imgSize = default;
            if (ExHelper.ReadPoint2D(reader, section, "DamageText.Repair.ImageSize", ref imgSize))
            {
                this.RepairData.ImageSize = imgSize;
            }

            int frameIdx = 0;
            if (reader.ReadNormal(section, "DamageText.ZeroFrameIndex", ref frameIdx))
            {
                this.DamageData.ZeroFrameIndex = frameIdx;
                this.RepairData.ZeroFrameIndex = frameIdx;
            }
            frameIdx = 0;
            if (reader.ReadNormal(section, "DamageText.Damage.ZeroFrameIndex", ref frameIdx))
            {
                this.DamageData.ZeroFrameIndex = frameIdx;
            }
            frameIdx = 0;
            if (reader.ReadNormal(section, "DamageText.Repair.ZeroFrameIndex", ref frameIdx))
            {
                this.RepairData.ZeroFrameIndex = frameIdx;
            }

        }

        public DamageTextControlData Clone()
        {
            DamageTextControlData data = new DamageTextControlData();
            data.Hidden = this.Hidden;
            data.DamageData = this.DamageData.Clone();
            data.RepairData = this.RepairData.Clone();
            return data;
        }
    }

    [Serializable]
    public class DamageTextData : HealthTextData
    {
        public Point2D XOffset;
        public Point2D YOffset;
        public int RollSpeed;
        public int Duration;

        public DamageTextData() : this(true) { }

        public DamageTextData(bool isDamage) : base(isDamage ? HealthState.Red : HealthState.Green)
        {
            this.Hidden = false;
            this.XOffset = new Point2D(-15, 15);
            this.YOffset = new Point2D(-12, 12);
            this.RollSpeed = 1;
            this.Duration = 75;
        }

        public new DamageTextData Clone()
        {
            DamageTextData data = new DamageTextData();
            ExHelper.ReflectClone(this, data);
            return data;
        }
    }

    public partial class TechnoExt
    {

        public unsafe void TechnoClass_ReceiveDamage2_DamageText(Pointer<int> pRealDamage, Pointer<WarheadTypeClass> pWH, DamageState damageState)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (Type.DamageTextControlData.Hidden || pTechno.IsInvisible() || pTechno.IsCloaked())
            {
                return;
            }
            string text = null;
            DamageTextData data = null;
            int damage = pRealDamage.Data;
            if (damage > 0)
            {
                data = Type.DamageTextControlData.DamageData;
                if (!data.Hidden)
                {
                    text = "-" + damage;
                }
            }
            else if (damage < 0)
            {
                data = Type.DamageTextControlData.RepairData;
                if (!data.Hidden)
                {
                    text = "+" + -damage;
                }
            }
            if (!string.IsNullOrEmpty(text))
            {
                int x = MathEx.Random.Next(data.XOffset.X, data.XOffset.Y);
                int y = MathEx.Random.Next(data.YOffset.X, data.YOffset.Y) - 15; // 离地高度
                Point2D offset = new Point2D(x, y);
                // 横向锚点修正
                int length = text.Length / 2;
                if (data.UseSHP)
                {
                    offset.X -= data.ImageSize.X * length;
                }
                else
                {
                    offset.X -= PrintTextManager.FontSize.X * length;
                }
                CoordStruct location = pTechno.Ref.Base.Base.GetCoords();
                PrintTextManager.RollingText(text, location, offset, data.RollSpeed, data.Duration, data);
            }
        }

    }

    public partial class TechnoTypeExt
    {
        public DamageTextControlData DamageTextControlData;

        /// <summary>
        /// [AudioVisual]
        /// [TechnoType] ;覆盖全局设置
        /// DamageText.Hidden=no ;停用显示伤害数字
        /// ; X表示两种伤害状态，分别是Damage\Repair，不写则是全局设置，如DamageText.Hidden=yes，停用显示，DamageText.Damage.Hidden=yes，不显示收到的伤害
        /// DamageText.X.Hidden=no ;隐藏显示
        /// DamageText.X.XOffset=-15,15 ;锚点随机横向范围
        /// DamageText.X.YOffset=-12,12 ;锚点随机纵向范围
        /// DamageText.X.RollSpeed=2 ;数字向上滚动的速度
        /// DamageText.X.Duration=50 ;数字存在的时间
        /// DamageText.Damage.Color=252,0,0 ;伤害数字的颜色
        /// DamageText.Repair.Color=0,252,0 ;修复数字的颜色
        /// ; 使用shp而不是font显示血量数字
        /// DamageText.X.UseSHP=no ;使用SHP显示伤害数字
        /// DamageText.X.SHP=pips.shp ;使用shp而不是文字，默认使用pips.shp，每个颜色15帧，每字一帧，顺序“0123456789+-*/%”，图像中心即锚点在字体的左上角
        /// DamageText.X.ImageSize=4,6 ;血量的图案宽度和高度
        /// DamageText.Damage.ZeroFrameIndex=54 ;伤害数字的"0"帧所在序号
        /// DamageText.Repair.ZeroFrameIndex=24 ;修复数字的"0"帧所在序号
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadDamageText(INIReader reader, string section)
        {
            if (null == DamageTextControlData && null != RulesExt.Instance.GeneralDamageTextControlData)
            {
                DamageTextControlData = RulesExt.Instance.GeneralDamageTextControlData.Clone();
            }

            DamageTextControlData.ReadDamageText(reader, section);
        }
    }

    public partial class RulesExt
    {
        public DamageTextControlData GeneralDamageTextControlData = new DamageTextControlData();

        private void ReadDamageText(INIReader reader)
        {
            GeneralDamageTextControlData.ReadDamageText(reader, SectionAV);
        }

    }

}