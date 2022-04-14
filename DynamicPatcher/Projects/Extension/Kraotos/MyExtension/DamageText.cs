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
    public class DamageTextTypeControlData
    {
        public bool Hidden;

        public Dictionary<int, DamageTextTypeData> Types;

        public DamageTextTypeControlData(bool init)
        {
            this.Hidden = false;
            this.Types = new Dictionary<int, DamageTextTypeData>();
            if (init)
            {
                for (int i = 0; i <= 10; i++)
                {
                    // 0 是unknow类型，默认设置
                    Types.Add(i, new DamageTextTypeData());
                }
            }
        }

        public void ReadDamageText(INIReader reader, string section)
        {
            bool hidden = false;
            if (reader.ReadNormal(section, "DamageText.Hidden", ref hidden))
            {
                this.Hidden = hidden;
            }

            foreach (KeyValuePair<int, DamageTextTypeData> type in Types)
            {
                type.Value.ReadDamageTextType(reader, section, "DamageText.");

                type.Value.ReadDamageTextType(reader, section, "DamageText." + type.Key + ".");
            }
        }
    }

    [Serializable]
    public class DamageTextTypeData
    {
        public bool Hidden;

        public DamageTextData Damage;
        public DamageTextData Repair;

        public DamageTextTypeData()
        {
            this.Hidden = false;
            this.Damage = new DamageTextData(true);
            this.Repair = new DamageTextData(false);
        }

        public void ReadDamageTextType(INIReader reader, string section, string title)
        {

            bool hidden = false;
            if (reader.ReadNormal(section, title + "Hidden", ref hidden))
            {
                this.Hidden = hidden;
            }

            this.Damage.ReadDamageText(reader, section, title);
            this.Repair.ReadDamageText(reader, section, title);

            this.Damage.ReadDamageText(reader, section, title + "Damage.");
            this.Repair.ReadDamageText(reader, section, title + "Repair.");
        }

        public DamageTextTypeData Clone()
        {
            DamageTextTypeData data = new DamageTextTypeData();
            data.Hidden = this.Hidden;
            data.Damage = this.Damage.Clone();
            data.Repair = this.Repair.Clone();
            return data;
        }
    }


    [Serializable]
    public class DamageTextData : PrintTextData
    {
        public bool Hidden;
        public Point2D XOffset;
        public Point2D YOffset;
        public int RollSpeed;
        public int Duration;

        public DamageTextData(bool isDamage) : base()
        {
            this.Hidden = false;
            this.XOffset = new Point2D(-15, 15);
            this.YOffset = new Point2D(-12, 12);
            this.RollSpeed = 1;
            this.Duration = 75;

            this.SHPFileName = "pips.shp";
            this.ImageSize = new Point2D(4, 6);
            if (isDamage)
            {
                this.Color = new ColorStruct(252, 0, 0);
                this.ZeroFrameIndex = 54;
            }
            else
            {
                this.Color = new ColorStruct(0, 252, 0);
                this.ZeroFrameIndex = 24;
            }
        }

        public void ReadDamageText(INIReader reader, string section, string title)
        {
            ReadPrintText(reader, section, title);

            bool hidden = false;
            if (reader.ReadNormal(section, title + "Hidden", ref hidden))
            {
                this.Hidden = hidden;
            }

            Point2D xOffset = default;
            if (ExHelper.ReadPoint2D(reader, section, title + "XOffset", ref xOffset))
            {
                Point2D offset = xOffset;
                if (xOffset.X > xOffset.Y)
                {
                    offset.X = xOffset.Y;
                    offset.Y = xOffset.X;
                }
                this.XOffset = offset;
            }

            Point2D yOffset = default;
            if (ExHelper.ReadPoint2D(reader, section, title + "YOffset", ref yOffset))
            {
                Point2D offset = yOffset;
                if (yOffset.X > yOffset.Y)
                {
                    offset.X = yOffset.Y;
                    offset.Y = yOffset.X;
                }
                this.YOffset = offset;
            }

            int roll = 1;
            if (reader.ReadNormal(section, title + "RollSpeed", ref roll))
            {
                this.RollSpeed = roll;
            }

            int duration = 0;
            if (reader.ReadNormal(section, title + "Duration", ref duration))
            {
                this.Duration = duration;
            }
        }

        public DamageTextData Clone()
        {
            DamageTextData data = new DamageTextData(true);
            ExHelper.ReflectClone(this, data);
            return data;
        }
    }

    public partial class TechnoExt
    {

        public unsafe void TechnoClass_ReceiveDamage2_DamageText(Pointer<int> pRealDamage, Pointer<WarheadTypeClass> pWH, DamageState damageState)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            WarheadTypeExt whExt = WarheadTypeExt.ExtMap.Find(pWH);
            if (pTechno.IsInvisible() || pTechno.IsCloaked() || null == whExt || null == whExt.DamageTextTypeData || whExt.DamageTextTypeData.Hidden)
            {
                return;
            }
            string text = null;
            DamageTextData data = null;
            int damage = pRealDamage.Data;
            if (damage > 0)
            {
                data = whExt.DamageTextTypeData.Damage;
                if (!data.Hidden)
                {
                    text = "-" + damage;
                }
            }
            else if (damage < 0)
            {
                data = whExt.DamageTextTypeData.Repair;
                if (!data.Hidden)
                {
                    text = "+" + -damage;
                }
            }
            if (null == data || data.Hidden)
            {
                return;
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

    public partial class WarheadTypeExt
    {
        public DamageTextTypeData DamageTextTypeData;
        private int DamageTextTypeNum;

        /// <summary>
        /// [AudioVisual]
        /// [WarheadType] ;覆盖全局设置
        /// DamageText.Hidden=no ;停用显示伤害数字
        /// ; X表示若干种伤害类型，分别是0\1\2\3\4\5..10，对应弹头的InfDeath=X，0表示未知，不写则是全局设置，如DamageText.Color=255,0,0不管弹头是什么伤害类型都是红色，DamageText.5.Color=255,0,0，则只有火焰伤害是红色
        /// ; Y表示两种伤害状态，分别是Damage\Repair，不写则是全局设置，如DamageText.Hidden=yes，停用显示，DamageText.Damage.Hidden=yes，不显示收到的伤害
        /// DamageText.X.Y.Hidden=no ;隐藏显示
        /// DamageText.X.Y.XOffset=-15,15 ;锚点随机横向范围
        /// DamageText.X.Y.YOffset=-12,12 ;锚点随机纵向范围
        /// DamageText.X.Y.RollSpeed=1 ;数字向上滚动的速度
        /// DamageText.X.Y.Duration=75 ;数字存在的时间
        /// ; 字体设置
        /// DamageText.X.Damage.Color=252,0,0 ;伤害数字的颜色
        /// DamageText.X.Repair.Color=0,252,0 ;修复数字的颜色
        /// DamageText.X.Y.ShadowOffset=1,1 ;阴影的偏移量
        /// DamageText.X.Y.ShadowColor=82,85,82 ;阴影的颜色
        /// ; 使用shp而不是font显示血量数字
        /// DamageText.X.Y.UseSHP=no ;使用SHP显示伤害数字
        /// DamageText.X.Y.SHP=pips.shp ;使用shp而不是文字，默认使用pips.shp，每个颜色15帧，每字一帧，顺序“0123456789+-*/%”，图像中心即锚点
        /// DamageText.X.Y.ImageSize=4,6 ;血量的图案宽度和高度
        /// DamageText.X.Damage.ZeroFrameIndex=54 ;伤害数字的"0"帧所在序号
        /// DamageText.X.Repair.ZeroFrameIndex=24 ;修复数字的"0"帧所在序号
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadDamageText(INIReader reader, string section)
        {

            int infDeath = 0;
            if (reader.ReadNormal(section, "InfDeath", ref infDeath))
            {
                if (infDeath > 0 && infDeath <= 10)
                {
                    this.DamageTextTypeNum = infDeath;
                }
            }
            if (null == DamageTextTypeData)
            {
                if (null != RulesExt.Instance.GeneralDamageTextTypeControlData)
                {
                    DamageTextTypeData = RulesExt.Instance.GeneralDamageTextTypeControlData.Types[DamageTextTypeNum].Clone();
                }
                else
                {
                    DamageTextTypeData = new DamageTextTypeData();
                }
            }
            if (null != DamageTextTypeData)
            {
                DamageTextTypeData.ReadDamageTextType(reader, section, "DamageText.");
                DamageTextTypeData.ReadDamageTextType(reader, section, "DamageText." + DamageTextTypeNum + ".");
            }
        }
    }

    public partial class RulesExt
    {
        public DamageTextTypeControlData GeneralDamageTextTypeControlData = new DamageTextTypeControlData(true);

        private void ReadDamageText(INIReader reader)
        {
            GeneralDamageTextTypeControlData.ReadDamageText(reader, SectionAV);
        }

    }

}