using System.Drawing;
using System.Reflection;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
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
    public class HealthTextTypeControlData
    {
        public bool Hidden;

        public HealthTextTypeData Building;
        public HealthTextTypeData Infantry;
        public HealthTextTypeData Unit;
        public HealthTextTypeData Aircraft;

        public HealthTextTypeControlData()
        {
            this.Hidden = false;
            this.Building = new HealthTextTypeData(AbstractType.Building);
            this.Infantry = new HealthTextTypeData(AbstractType.Infantry);
            this.Unit = new HealthTextTypeData(AbstractType.Unit);
            this.Aircraft = new HealthTextTypeData(AbstractType.Aircraft);
        }

        public void ReadHealthText(INIReader reader, string section)
        {
            bool hidden = false;
            if (reader.ReadNormal(section, "HealthText.Hidden", ref hidden))
            {
                this.Hidden = hidden;
            }

            Building.ReadHealthTextType(reader, section, "HealthText.");
            Infantry.ReadHealthTextType(reader, section, "HealthText.");
            Unit.ReadHealthTextType(reader, section, "HealthText.");
            Aircraft.ReadHealthTextType(reader, section, "HealthText.");

            Building.ReadHealthTextType(reader, section, "HealthText.Building.");
            Infantry.ReadHealthTextType(reader, section, "HealthText.Infantry.");
            Unit.ReadHealthTextType(reader, section, "HealthText.Unit.");
            Aircraft.ReadHealthTextType(reader, section, "HealthText.Aircraft.");


        }

        public HealthTextTypeControlData Clone()
        {
            HealthTextTypeControlData data = new HealthTextTypeControlData();
            data.Hidden = this.Hidden;
            data.Building = this.Building.Clone();
            data.Infantry = this.Infantry.Clone();
            data.Unit = this.Unit.Clone();
            data.Aircraft = this.Aircraft.Clone();
            return data;
        }
    }


    [Serializable]
    public class HealthTextTypeData
    {
        public bool Hidden;

        public HealthTextData Green;
        public HealthTextData Yellow;
        public HealthTextData Red;

        public HealthTextTypeData() { }

        public HealthTextTypeData(AbstractType type)
        {
            this.Hidden = false;
            this.Green = new HealthTextData(HealthState.Green);
            this.Yellow = new HealthTextData(HealthState.Yellow);
            this.Red = new HealthTextData(HealthState.Red);
            switch (type)
            {
                case AbstractType.Building:
                    Green.Style = HealthTextStyle.FULL;
                    Yellow.Style = HealthTextStyle.FULL;
                    Red.Style = HealthTextStyle.FULL;
                    break;
                case AbstractType.Infantry:
                    Green.Style = HealthTextStyle.SHORT;
                    Yellow.Style = HealthTextStyle.SHORT;
                    Red.Style = HealthTextStyle.SHORT;
                    break;
                case AbstractType.Unit:
                    Green.Style = HealthTextStyle.FULL;
                    Yellow.Style = HealthTextStyle.FULL;
                    Red.Style = HealthTextStyle.FULL;
                    break;
                case AbstractType.Aircraft:
                    Green.Style = HealthTextStyle.FULL;
                    Yellow.Style = HealthTextStyle.FULL;
                    Red.Style = HealthTextStyle.FULL;
                    break;
            }
        }

        public void ReadHealthTextType(INIReader reader, string section, string title)
        {
            bool hidden = false;
            if (reader.ReadNormal(section, title + "Hidden", ref hidden))
            {
                this.Hidden = hidden;
            }

            this.Green.ReadHealthText(reader, section, title);
            this.Yellow.ReadHealthText(reader, section, title);
            this.Red.ReadHealthText(reader, section, title);

            this.Green.ReadHealthText(reader, section, title + "Green.");
            this.Yellow.ReadHealthText(reader, section, title + "Yellow.");
            this.Red.ReadHealthText(reader, section, title + "Red.");

        }

        public HealthTextTypeData Clone()
        {
            HealthTextTypeData data = new HealthTextTypeData();
            data.Hidden = this.Hidden;
            data.Green = this.Green.Clone();
            data.Yellow = this.Yellow.Clone();
            data.Red = this.Red.Clone();
            return data;
        }
    }

    [Serializable]
    public class HealthTextData : PrintTextData
    {
        public bool Hidden;
        public bool ShowEnemy;
        public bool ShowHover;
        public HealthTextStyle Style;
        public HealthTextStyle HoverStyle;

        public HealthTextData(HealthState healthState) : base()
        {
            this.Hidden = false;
            this.ShowEnemy = false;
            this.ShowHover = false;
            this.Style = HealthTextStyle.FULL;
            this.HoverStyle = HealthTextStyle.SHORT;

            this.SHPFileName = "pips.shp";
            this.ImageSize = new Point2D(4, 6);
            switch (healthState)
            {
                case HealthState.Green:
                    this.Color = new ColorStruct(0, 252, 0);
                    this.ZeroFrameIndex = 24;
                    break;
                case HealthState.Yellow:
                    this.Color = new ColorStruct(252, 212, 0);
                    this.ZeroFrameIndex = 39;
                    break;
                case HealthState.Red:
                    this.Color = new ColorStruct(252, 0, 0);
                    this.ZeroFrameIndex = 54;
                    break;
            }
        }

        private HealthTextStyle ReadStyle(string text, HealthTextStyle defStyle)
        {
            string t = text.Substring(0, 1).ToUpper();
            switch (t)
            {
                case "F":
                    return HealthTextStyle.FULL;
                case "S":
                    return HealthTextStyle.SHORT;
                case "P":
                    return HealthTextStyle.PERCENT;
            }
            return defStyle;
        }

        public void ReadHealthText(INIReader reader, string section, string title)
        {
            ReadPrintText(reader, section, title);

            bool hidden = false;
            if (reader.ReadNormal(section, title + "Hidden", ref hidden))
            {
                this.Hidden = hidden;
            }

            bool enemy = false;
            if (reader.ReadNormal(section, title + "ShowEnemy", ref enemy))
            {
                this.ShowEnemy = enemy;
            }

            bool hover = false;
            if (reader.ReadNormal(section, title + "ShowHover", ref hover))
            {
                this.ShowHover = hover;
            }

            string s = null;
            if (reader.ReadNormal(section, title + "Style", ref s))
            {
                HealthTextStyle style = ReadStyle(s, HealthTextStyle.FULL);
                this.Style = style;
            }

            string h = null;
            if (reader.ReadNormal(section, title + "HoverStyle", ref h))
            {
                HealthTextStyle style = ReadStyle(h, HealthTextStyle.SHORT);
                this.HoverStyle = style;
            }
        }

        public HealthTextData Clone()
        {
            HealthTextData data = new HealthTextData(HealthState.Green);
            ExHelper.ReflectClone(this, data);
            return data;
        }

    }

    public enum HealthTextStyle
    {
        AUTO = 0, FULL = 1, SHORT = 2, PERCENT = 3
    }

    public partial class TechnoExt
    {
        public unsafe void TechnoClass_DrawHealthBar_Building_Text(int length, Pointer<Point2D> pLocation, Pointer<RectangleStruct> pBound)
        {
            if (!Type.HealthTextControlData.Hidden)
            {
                PrintHealthText(length, pLocation, pBound);
            }
        }

        public unsafe void TechnoClass_DrawHealthBar_Other_Text(int length, Pointer<Point2D> pLocation, Pointer<RectangleStruct> pBound)
        {
            if (!Type.HealthTextControlData.Hidden)
            {
                PrintHealthText(length, pLocation, pBound);
            }
        }

        private void PrintHealthText(int length, Pointer<Point2D> pLocation, Pointer<RectangleStruct> pBound)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            bool isBuilding = false;
            bool isSelected = pTechno.Ref.Base.IsSelected;
            HealthTextTypeData typeData = null;
            switch (pTechno.Ref.Base.Base.WhatAmI())
            {
                case AbstractType.Building:
                    isBuilding = true;
                    typeData = Type.HealthTextControlData.Building;
                    break;
                case AbstractType.Infantry:
                    typeData = Type.HealthTextControlData.Infantry;
                    break;
                case AbstractType.Unit:
                    typeData = Type.HealthTextControlData.Unit;
                    break;
                case AbstractType.Aircraft:
                    typeData = Type.HealthTextControlData.Aircraft;
                    break;
                default:
                    return;
            }
            if (typeData.Hidden)
            {
                return;
            }
            // 根据血量状态获取设置
            HealthTextData data = typeData.Green;
            HealthState healthState = pTechno.Ref.Base.GetHealthStatus();
            switch (healthState)
            {
                case HealthState.Yellow:
                    data = typeData.Yellow;
                    break;
                case HealthState.Red:
                    data = typeData.Red;
                    break;
            }
            // Logger.Log($"{Game.CurrentFrame} - Hidden = {data.Hidden}, ShowEnemy = {(!pTechno.Ref.Owner.Ref.PlayerControl && !data.ShowEnemy)}, ShowHover = {(!isSelected && !data.ShowHover)}");
            if (data.Hidden || (!pTechno.Ref.Owner.Ref.PlayerControl && !data.ShowEnemy) || (!isSelected && !data.ShowHover))
            {
                return;
            }
            // 调整锚点
            Point2D pos = pLocation.Data;
            int xOffset = data.Offset.X; // 锚点向右的偏移值
            int yOffset = data.Offset.Y; // 锚点向下的偏移值

            // Point2D fountSize = data.FontSize; // 使用shp则按照shp图案大小来偏移锚点
            HealthTextStyle style = isSelected ? data.Style : data.HoverStyle; ; // 数值的格式
            if (isBuilding)
            {
                // 算出建筑血条最左边格子的偏移
                CoordStruct dimension = pTechno.Ref.Type.Ref.Base.Dimension2();
                CoordStruct dimension2 = new CoordStruct(-dimension.X / 2, dimension.Y / 2, dimension.Z);
                Point2D pos2 = TacticalClass.Instance.Ref.CoordsToScreen(dimension2);
                // 修正锚点
                pos += pos2;
                pos.X = pos.X + 4 + xOffset;
                pos.Y = pos.Y - 2 + yOffset;
                // 数值格式
                style = isSelected ? data.Style : data.HoverStyle;
            }
            else
            {
                yOffset += pTechno.Ref.Type.Ref.PixelSelectionBracketDelta;
                if (length == 8)
                {
                    // 步兵血条
                    pos.X = pos.X - 7 + xOffset;
                    pos.Y = pos.Y - 28 + yOffset;
                }
                else
                {
                    // 载具血条
                    pos.X = pos.X - 17 + xOffset;
                    pos.Y = pos.Y - 29 + yOffset;
                    // 数值格式
                    style = isSelected ? data.Style : data.HoverStyle;
                }
            }
            // 获得血量数据
            string text = null;
            int health = OwnerObject.Ref.Base.Health;
            switch (style)
            {
                case HealthTextStyle.FULL:
                    int strength = OwnerObject.Ref.Type.Ref.Base.Strength;
                    string s = isBuilding ? "|" : "/";
                    text = string.Format("{0}{1}{2}", health, s, strength);
                    break;
                case HealthTextStyle.PERCENT:
                    double per = OwnerObject.Ref.Base.GetHealthPercentage() * 100;
                    text = string.Format("{0}%", per);
                    break;
                default:
                    text = health.ToString();
                    break;
            }
            if (!string.IsNullOrEmpty(text))
            {
                // 修正锚点
                if (data.UseSHP)
                {
                    // 使用Shp显示数字，SHP锚点在图案中心
                    // 重新调整锚点位置，向上抬起一个半格字的高度
                    pos.Y = pos.Y - data.ImageSize.Y / 2;
                }
                else
                {
                    // 使用文字显示数字，文字的锚点在左上角
                    // 重新调整锚点位置，向上抬起一个半格字的高度
                    pos.X = pos.X - PrintTextManager.FontSize.X / 2; // 左移半个字宽，美观
                    pos.Y = pos.Y - PrintTextManager.FontSize.Y + 5; // 字是20格高，上4中9下7
                }
                PrintTextManager.Print(text, data, pos, pBound, Surface.Current, isBuilding);
            }

        }


    }

    public partial class TechnoTypeExt
    {
        public HealthTextTypeControlData HealthTextControlData;

        /// <summary>
        /// [AudioVisual]
        /// [TechnoType] ;覆盖全局设置
        /// HealthText.Hidden=no ;停用该功能
        /// ; X表示四种单位类型，分别是Building\Infantry\Unit\Aircraft，不写则是全局设置，如HealthText.Hidden=yes，不论何种状态全部隐藏，HealthText.Infantry.Hidden=yes，步兵则隐藏。
        /// ; Y表示三种颜色状态，分别是Green\Yellow\Red，不写则是全局设置，如HealthText.Infantry.Hidden=yes，步兵不论何种状态全部隐藏，HealthText.Infantry.Green.Hidden=yes，则步兵在绿血时隐藏。
        /// HealthText.X.Y.Hidden=no ;隐藏显示
        /// HealthText.X.Y.ShowEnemy=no ;显示给敌方
        /// HealthText.X.Y.ShowHover=no ;鼠标悬停时是否显示
        /// HealthText.X.Y.Offset=0,0 ;锚点向右和向下的偏移位置
        /// ; 数字格式
        /// HealthText.Building.Y.Style=FULL ;数显的类型，FULL\SHORT\PERCENT
        /// HealthText.Building.Y.HoverStyle=SHORT ;鼠标悬停时数显的类型，FULL\SHORT\PERCENT
        /// HealthText.Infantry.Y.Style=SHORT ;数显的类型，FULL\SHORT\PERCENT
        /// HealthText.Infantry.Y.HoverStyle=SHORT ;鼠标悬停时数显的类型，FULL\SHORT\PERCENT
        /// HealthText.Unit.Y.Style=FULL ;数显的类型，FULL\SHORT\PERCENT
        /// HealthText.Unit.Y.HoverStyle=SHORT ;鼠标悬停时数显的类型，FULL\SHORT\PERCENT
        /// HealthText.Aircraft.Y.Style=FULL ;数显的类型，FULL\SHORT\PERCENT
        /// HealthText.Aircraft.Y.HoverStyle=SHORT ;鼠标悬停时数显的类型，FULL\SHORT\PERCENT
        /// ; 字体设置
        /// HealthText.X.Green.Color=0,252,0 ;绿血时字体颜色
        /// HealthText.X.Yellow.Color=252,212,0 ;黄血时字体颜色
        /// HealthText.X.Red.Color=252,0,0 ;红血时字体颜色
        /// HealthText.X.Y.ShadowOffset=1,1 ;阴影的偏移量
        /// HealthText.X.Y.ShadowColor=82,85,82 ;阴影的颜色
        /// ; 使用shp而不是font显示血量数字
        /// HealthText.X.Y.UseSHP=no ;使用shp而不是文字，默认使用pips.shp，每个颜色15帧，每字一帧，顺序“0123456789+-*/%”，图像中心即锚点
        /// HealthText.X.Y.SHP=pips.shp ;血量的shp文件
        /// HealthText.X.Y.ImageSize=4,6 ;血量的图案宽度和高度
        /// HealthText.X.Green.ZeroFrameIndex=24 ;绿色血量的"0"帧所在序号
        /// HealthText.X.Yellow.ZeroFrameIndex=39 ;黄色血量的"0"帧所在序号
        /// HealthText.X.Red.ZeroFrameIndex=54 ;红色血量的"0"帧所在序号
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadHelthText(INIReader reader, string section)
        {
            if (null == HealthTextControlData && null != RulesExt.Instance.GeneralHealthTextTypeControlData)
            {
                HealthTextControlData = RulesExt.Instance.GeneralHealthTextTypeControlData.Clone();
            }

            HealthTextControlData.ReadHealthText(reader, section);
        }
    }

    public partial class RulesExt
    {
        public HealthTextTypeControlData GeneralHealthTextTypeControlData = new HealthTextTypeControlData();

        private void ReadHealthText(INIReader reader)
        {
            GeneralHealthTextTypeControlData.ReadHealthText(reader, SectionAudioVisual);
        }
    }

}