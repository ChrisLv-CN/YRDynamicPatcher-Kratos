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

        public bool TryReadHealthText(INIReader reader, string section)
        {
            bool isRead = false;

            bool hidden = false;
            if (reader.ReadNormal(section, "HealthText.Hidden", ref hidden))
            {
                isRead = true;
                this.Hidden = hidden;
            }

            if (!Hidden)
            {
                if (Building.TryReadHealthTextType(reader, section, "HealthText."))
                {
                    isRead = true;
                }
                if (Infantry.TryReadHealthTextType(reader, section, "HealthText."))
                {
                    isRead = true;
                }
                if (Unit.TryReadHealthTextType(reader, section, "HealthText."))
                {
                    isRead = true;
                }
                if (Aircraft.TryReadHealthTextType(reader, section, "HealthText."))
                {
                    isRead = true;
                }

                if (Building.TryReadHealthTextType(reader, section, "HealthText.Building."))
                {
                    isRead = true;
                }
                if (Infantry.TryReadHealthTextType(reader, section, "HealthText.Infantry."))
                {
                    isRead = true;
                }
                if (Unit.TryReadHealthTextType(reader, section, "HealthText.Unit."))
                {
                    isRead = true;
                }
                if (Aircraft.TryReadHealthTextType(reader, section, "HealthText.Aircraft."))
                {
                    isRead = true;
                }
            }

            return isRead;
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

        public HealthTextTypeData Clone()
        {
            HealthTextTypeData data = new HealthTextTypeData();
            data.Hidden = this.Hidden;
            data.Green = this.Green.Clone();
            data.Yellow = this.Yellow.Clone();
            data.Red = this.Red.Clone();
            return data;
        }

        public bool TryReadHealthTextType(INIReader reader, string section, string title)
        {
            bool isRead = false;

            bool hidden = false;
            if (reader.ReadNormal(section, title + "Hidden", ref hidden))
            {
                isRead = true;
                this.Hidden = hidden;
            }

            if (!Hidden)
            {

                if (this.Green.TryReadHealthText(reader, section, title))
                {
                    isRead = true;
                }
                if (this.Yellow.TryReadHealthText(reader, section, title))
                {
                    isRead = true;
                }
                if (this.Red.TryReadHealthText(reader, section, title))
                {
                    isRead = true;
                }

                if (this.Green.TryReadHealthText(reader, section, title + "Green."))
                {
                    isRead = true;
                }
                if (this.Yellow.TryReadHealthText(reader, section, title + "Yellow."))
                {
                    isRead = true;
                }
                if (this.Red.TryReadHealthText(reader, section, title + "Red."))
                {
                    isRead = true;
                }
            }
            
            return isRead;
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
        public HealthTextAlign Align;

        public HealthTextData(HealthState healthState) : base()
        {
            this.Hidden = false;
            this.ShowEnemy = false;
            this.ShowHover = false;
            this.Style = HealthTextStyle.FULL;
            this.HoverStyle = HealthTextStyle.SHORT;
            this.Align = HealthTextAlign.LEFT;

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

        public HealthTextData Clone()
        {
            HealthTextData data = new HealthTextData(HealthState.Green);
            CopyTo(data);
            data.Hidden = this.Hidden;
            data.ShowEnemy = this.ShowEnemy;
            data.ShowHover = this.ShowHover;
            data.Style = this.Style;
            data.HoverStyle = this.HoverStyle;
            data.Align = this.Align;
            return data;
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

        public bool TryReadHealthText(INIReader reader, string section, string title)
        {
            bool isRead = false;


            bool hidden = false;
            if (reader.ReadNormal(section, title + "Hidden", ref hidden))
            {
                isRead = true;
                this.Hidden = hidden;
            }

            if (!Hidden)
            {

                isRead = TryReadPrintText(reader, section, title);

                bool enemy = false;
                if (reader.ReadNormal(section, title + "ShowEnemy", ref enemy))
                {
                    isRead = true;
                    this.ShowEnemy = enemy;
                }

                bool hover = false;
                if (reader.ReadNormal(section, title + "ShowHover", ref hover))
                {
                    isRead = true;
                    this.ShowHover = hover;
                }

                string s = null;
                if (reader.ReadNormal(section, title + "Style", ref s))
                {
                    isRead = true;
                    HealthTextStyle style = ReadStyle(s, HealthTextStyle.FULL);
                    this.Style = style;
                }

                string h = null;
                if (reader.ReadNormal(section, title + "HoverStyle", ref h))
                {
                    isRead = true;
                    HealthTextStyle style = ReadStyle(h, HealthTextStyle.SHORT);
                    this.HoverStyle = style;
                }

                string a = null;
                if (reader.ReadNormal(section, title + "Align", ref a))
                {
                    isRead = true;
                    string t = a.Substring(0, 1).ToUpper();
                    switch (t)
                    {
                        case "L":
                            this.Align = HealthTextAlign.LEFT;
                            break;
                        case "C":
                            this.Align = HealthTextAlign.CENTER;
                            break;
                        case "R":
                            this.Align = HealthTextAlign.RIGHT;
                            break;
                    }
                }
            }

            return isRead;
        }

    }

    public enum HealthTextStyle
    {
        AUTO = 0, FULL = 1, SHORT = 2, PERCENT = 3
    }

    public enum HealthTextAlign
    {
        LEFT = 0, CENTER = 1, RIGHT = 2
    }

    public partial class TechnoExt
    {
        private bool hiddenHealthText = false;
        private HealthTextTypeData healthTextTypeData;

        public unsafe void TechnoClass_Init_HealthBarText()
        {
            this.hiddenHealthText = Type.HealthTextHidden;
            if (!hiddenHealthText)
            {
                this.healthTextTypeData = Type.HealthTextTypeData;
                this.hiddenHealthText = healthTextTypeData.Hidden;
            }
        }

        public unsafe void TechnoClass_DrawHealthBar_Building_Text(int length, Pointer<Point2D> pLocation, Pointer<RectangleStruct> pBound)
        {
            if (!hiddenHealthText)
            {
                PrintHealthText(length, pLocation, pBound);
            }
        }

        public unsafe void TechnoClass_DrawHealthBar_Other_Text(int length, Pointer<Point2D> pLocation, Pointer<RectangleStruct> pBound)
        {
            if (!hiddenHealthText)
            {
                PrintHealthText(length, pLocation, pBound);
            }
        }

        private void PrintHealthText(int barLength, Pointer<Point2D> pLocation, Pointer<RectangleStruct> pBound)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            bool isSelected = pTechno.Ref.Base.IsSelected;
            // 根据血量状态获取设置
            HealthTextData data = healthTextTypeData.Green;
            HealthState healthState = pTechno.Ref.Base.GetHealthStatus();
            switch (healthState)
            {
                case HealthState.Yellow:
                    data = healthTextTypeData.Yellow;
                    break;
                case HealthState.Red:
                    data = healthTextTypeData.Red;
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
            int barWidth = barLength * 2; // 血条显示的个数，单位是半条，建筑是满条

            // Point2D fountSize = data.FontSize; // 使用shp则按照shp图案大小来偏移锚点
            HealthTextStyle style = isSelected ? data.Style : data.HoverStyle; ; // 数值的格式
            if (IsBuilding)
            {
                // 算出建筑血条最左边格子的偏移
                CoordStruct dimension = pTechno.Ref.Type.Ref.Base.Dimension2();
                CoordStruct dimension2 = new CoordStruct(-dimension.X / 2, dimension.Y / 2, dimension.Z);
                Point2D pos2 = TacticalClass.Instance.Ref.CoordsToScreen(dimension2);
                // 修正锚点
                pos += pos2;
                pos.X = pos.X - 2 + xOffset;
                pos.Y = pos.Y - 2 + yOffset;
                barWidth = barLength * 4 + 6; // 建筑是满条，每个块是10像素宽，每个4像素绘制一个，头边距2，尾边距4
            }
            else
            {
                yOffset += pTechno.Ref.Type.Ref.PixelSelectionBracketDelta;
                pos.X += -barLength + 3 + xOffset;
                pos.Y += -28 + yOffset;
                if (barLength == 8)
                {
                    // 步兵血条 length = 8
                    pos.X += 1;
                }
                else
                {
                    // 载具血条 length = 17
                    pos.Y += -1;
                }
            }
            // 获得血量数据
            string text = null;
            int health = OwnerObject.Ref.Base.Health;
            switch (style)
            {
                case HealthTextStyle.FULL:
                    int strength = OwnerObject.Ref.Type.Ref.Base.Strength;
                    string s = IsBuilding ? "|" : "/";
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

                    // 按对齐方式再次调整锚点
                    if (data.Align != HealthTextAlign.LEFT)
                    {
                        int x = data.ImageSize.X % 2 == 0 ? data.ImageSize.X : data.ImageSize.X + 1;
                        int textWidth = text.ToCharArray().Count() * x;
                        OffsetPosAlign(ref pos, textWidth, barWidth, data.Align, IsBuilding, true);
                    }
                    else
                    {
                        if (IsBuilding)
                        {
                            pos.X += data.ImageSize.X; // 右移一个字宽，美观
                        }
                    }
                }
                else
                {
                    // 使用文字显示数字，文字的锚点在左上角
                    // 重新调整锚点位置，向上抬起一个半格字的高度
                    pos.Y = pos.Y - PrintTextManager.FontSize.Y + 5; // 字是20格高，上4中9下7

                    // 按对齐方式再次调整锚点
                    if (data.Align != HealthTextAlign.LEFT)
                    {
                        RectangleStruct textRect = Drawing.GetTextDimensions(text, new Point2D(0, 0), 0, 2, 0);
                        int textWidth = textRect.Width;
                        OffsetPosAlign(ref pos, textWidth, barWidth, data.Align, IsBuilding, false);
                    }
                    else
                    {
                        if (IsBuilding)
                        {
                            pos.X += PrintTextManager.FontSize.X; // 右移一个字宽，美观
                        }
                        else
                        {
                            pos.X -= PrintTextManager.FontSize.X / 2; // 左移半个字宽，美观
                        }
                    }
                }
                PrintTextManager.Print(text, data, pos, pBound, Surface.Current, IsBuilding);
            }

        }

        private void OffsetPosAlign(ref Point2D pos, int textWidth, int barWidth, HealthTextAlign align, bool isBuilding, bool useSHP)
        {
            // Logger.Log($"{Game.CurrentFrame} textWidth = {textWidth}, barWidth = {barWidth}, align = {align}, isBuilding = {isBuilding}");
            int offset = barWidth - textWidth;
            switch (align)
            {
                case HealthTextAlign.CENTER:
                    pos.X += offset / 2;
                    if (isBuilding)
                    {
                        pos.Y -= offset / 4;
                    }
                    break;
                case HealthTextAlign.RIGHT:
                    pos.X += offset;
                    if (!useSHP)
                    {
                        pos.X += PrintTextManager.FontSize.X / 2; // 右移半个字宽，补偿Margin
                    }
                    if (isBuilding)
                    {
                        pos.Y -= offset / 2;
                    }
                    break;
            }
        }


    }

    public partial class TechnoTypeExt
    {
        // public HealthTextTypeControlData HealthTextControlData = new HealthTextTypeControlData();
        public bool HealthTextHidden;
        public HealthTextTypeData HealthTextTypeData;

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
        /// HealthText.X.Y.Align=LEFT ;对齐方式，LEFT\CENTER\RIGHT，建筑恒定为左对齐
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
        private void ReadHelthText(INIReader reader, string section, AbstractType absType)
        {
            HealthTextHidden = RulesExt.Instance.GeneralHealthTextTypeControlData.Hidden;
            if (!HealthTextHidden)
            {
                switch (absType)
                {
                    case AbstractType.BuildingType:
                        if (null == HealthTextTypeData || RulesExt.Instance.GeneralHealthTextTypeControlDataHasChanged)
                        {
                            HealthTextTypeData = RulesExt.Instance.GeneralHealthTextTypeControlData.Building.Clone();
                        }
                        HealthTextTypeData.TryReadHealthTextType(reader, section, "HealthText.");
                        HealthTextTypeData.TryReadHealthTextType(reader, section, "HealthText.Building.");
                        break;
                    case AbstractType.InfantryType:
                        if (null == HealthTextTypeData || RulesExt.Instance.GeneralHealthTextTypeControlDataHasChanged)
                        {
                            HealthTextTypeData = RulesExt.Instance.GeneralHealthTextTypeControlData.Infantry.Clone();
                        }
                        HealthTextTypeData.TryReadHealthTextType(reader, section, "HealthText.");
                        HealthTextTypeData.TryReadHealthTextType(reader, section, "HealthText.Infantry.");
                        break;
                    case AbstractType.UnitType:
                        if (null == HealthTextTypeData || RulesExt.Instance.GeneralHealthTextTypeControlDataHasChanged)
                        {
                            HealthTextTypeData = RulesExt.Instance.GeneralHealthTextTypeControlData.Unit.Clone();
                        }
                        HealthTextTypeData.TryReadHealthTextType(reader, section, "HealthText.");
                        HealthTextTypeData.TryReadHealthTextType(reader, section, "HealthText.Unit.");
                        break;
                    case AbstractType.AircraftType:
                        if (null == HealthTextTypeData || RulesExt.Instance.GeneralHealthTextTypeControlDataHasChanged)
                        {
                            HealthTextTypeData = RulesExt.Instance.GeneralHealthTextTypeControlData.Aircraft.Clone();
                        }
                        HealthTextTypeData.TryReadHealthTextType(reader, section, "HealthText.");
                        HealthTextTypeData.TryReadHealthTextType(reader, section, "HealthText.Aircraft.");
                        break;
                }
            }
            // HealthTextControlData.ReadHealthText(reader, RulesExt.SectionAudioVisual);
            // HealthTextControlData.ReadHealthText(reader, section);
        }

    }

    public partial class RulesExt
    {
        public HealthTextTypeControlData GeneralHealthTextTypeControlData = new HealthTextTypeControlData();
        public bool GeneralHealthTextTypeControlDataHasChanged = false;

        private void ReadHealthText(INIReader reader)
        {
            HealthTextTypeControlData temp = new HealthTextTypeControlData();
            if (temp.TryReadHealthText(reader, SectionAudioVisual))
            {
                GeneralHealthTextTypeControlDataHasChanged = true;
                GeneralHealthTextTypeControlData = temp;
            }
            else
            {
                GeneralHealthTextTypeControlDataHasChanged = false;
                temp = null;
            }
        }
    }

}