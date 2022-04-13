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
    public class HealthTextControlData
    {
        public bool Hidden;
        public bool ShowEnemy;
        public bool ShowHover;

        public HealthTextData GreenData;
        public HealthTextData YellowData;
        public HealthTextData RedData;

        public HealthTextControlData()
        {
            this.Hidden = false;
            this.ShowEnemy = false;
            this.ShowHover = false;
            this.GreenData = new HealthTextData(HealthState.Green);
            this.YellowData = new HealthTextData(HealthState.Yellow);
            this.RedData = new HealthTextData(HealthState.Red);
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

        public bool IsNotPips(string fileName, out Pointer<SHPStruct> pSHP)
        {
            pSHP = IntPtr.Zero;
            string temp = fileName;
            if (!string.IsNullOrEmpty(fileName) && !(temp = fileName.ToLower()).Equals("pips.shp"))
            {
                pSHP = FileSystem.LoadSHPFile(temp);
                Logger.Log("{0} - 尝试载入shp文件{1}，{2}", Game.CurrentFrame, temp, !pSHP.IsNull);
                return !pSHP.IsNull;
            }
            return false;
        }

        public void ReadHealthText(INIReader reader, string section)
        {
            bool hidden = false;
            if (reader.ReadNormal(section, "HealthText.Hidden", ref hidden))
            {
                this.Hidden = hidden;
                this.GreenData.Hidden = hidden;
                this.YellowData.Hidden = hidden;
                this.RedData.Hidden = hidden;
            }
            hidden = false;
            if (reader.ReadNormal(section, "HealthText.Green.Hidden", ref hidden))
            {
                this.GreenData.Hidden = hidden;
            }
            hidden = false;
            if (reader.ReadNormal(section, "HealthText.Yellow.Hidden", ref hidden))
            {
                this.YellowData.Hidden = hidden;
            }
            hidden = false;
            if (reader.ReadNormal(section, "HealthText.Red.Hidden", ref hidden))
            {
                this.RedData.Hidden = hidden;
            }

            bool showEnemy = false;
            if (reader.ReadNormal(section, "HealthText.ShowEnemy", ref showEnemy))
            {
                this.ShowEnemy = showEnemy;
            }
            bool showHover = false;
            if (reader.ReadNormal(section, "HealthText.ShowHover", ref showHover))
            {
                this.ShowHover = showHover;
            }

            // 格式
            string infantry = null;
            if (reader.ReadNormal(section, "HealthText.InfantryStyle", ref infantry))
            {
                HealthTextStyle style = ReadStyle(infantry, HealthTextStyle.SHORT);
                this.GreenData.InfantryStyle = style;
                this.YellowData.InfantryStyle = style;
                this.RedData.InfantryStyle = style;
            }
            infantry = null;
            if (reader.ReadNormal(section, "HealthText.Green.InfantryStyle", ref infantry))
            {
                HealthTextStyle style = ReadStyle(infantry, HealthTextStyle.SHORT);
                this.GreenData.InfantryStyle = style;
            }
            infantry = null;
            if (reader.ReadNormal(section, "HealthText.Yellow.InfantryStyle", ref infantry))
            {
                HealthTextStyle style = ReadStyle(infantry, HealthTextStyle.SHORT);
                this.YellowData.InfantryStyle = style;
            }
            infantry = null;
            if (reader.ReadNormal(section, "HealthText.Red.InfantryStyle", ref infantry))
            {
                HealthTextStyle style = ReadStyle(infantry, HealthTextStyle.SHORT);
                this.RedData.InfantryStyle = style;
            }

            string infantryH = null;
            if (reader.ReadNormal(section, "HealthText.InfantryHoverStyle", ref infantryH))
            {
                HealthTextStyle style = ReadStyle(infantryH, HealthTextStyle.SHORT);
                this.GreenData.InfantryHoverStyle = style;
                this.YellowData.InfantryHoverStyle = style;
                this.RedData.InfantryHoverStyle = style;
            }
            infantryH = null;
            if (reader.ReadNormal(section, "HealthText.Green.InfantryHoverStyle", ref infantryH))
            {
                HealthTextStyle style = ReadStyle(infantryH, HealthTextStyle.SHORT);
                this.GreenData.InfantryHoverStyle = style;
            }
            infantryH = null;
            if (reader.ReadNormal(section, "HealthText.Yellow.InfantryHoverStyle", ref infantryH))
            {
                HealthTextStyle style = ReadStyle(infantryH, HealthTextStyle.SHORT);
                this.YellowData.InfantryHoverStyle = style;
            }
            infantryH = null;
            if (reader.ReadNormal(section, "HealthText.Red.InfantryHoverStyle", ref infantryH))
            {
                HealthTextStyle style = ReadStyle(infantryH, HealthTextStyle.SHORT);
                this.RedData.InfantryHoverStyle = style;
            }

            string vehicle = null;
            if (reader.ReadNormal(section, "HealthText.VehicleStyle", ref vehicle))
            {
                HealthTextStyle style = ReadStyle(vehicle, HealthTextStyle.FULL);
                this.GreenData.VehicleStyle = style;
                this.YellowData.VehicleStyle = style;
                this.RedData.VehicleStyle = style;
            }
            vehicle = null;
            if (reader.ReadNormal(section, "HealthText.Green.VehicleStyle", ref vehicle))
            {
                HealthTextStyle style = ReadStyle(vehicle, HealthTextStyle.FULL);
                this.GreenData.VehicleStyle = style;
            }
            vehicle = null;
            if (reader.ReadNormal(section, "HealthText.Yellow.VehicleStyle", ref vehicle))
            {
                HealthTextStyle style = ReadStyle(vehicle, HealthTextStyle.FULL);
                this.YellowData.VehicleStyle = style;
            }
            vehicle = null;
            if (reader.ReadNormal(section, "HealthText.Red.VehicleStyle", ref vehicle))
            {
                HealthTextStyle style = ReadStyle(vehicle, HealthTextStyle.FULL);
                this.RedData.VehicleStyle = style;
            }

            string vehicleH = null;
            if (reader.ReadNormal(section, "HealthText.VehicleHoverStyle", ref vehicleH))
            {
                HealthTextStyle style = ReadStyle(vehicleH, HealthTextStyle.SHORT);
                this.GreenData.VehicleHoverStyle = style;
                this.YellowData.VehicleHoverStyle = style;
                this.RedData.VehicleHoverStyle = style;
            }
            vehicleH = null;
            if (reader.ReadNormal(section, "HealthText.Green.VehicleHoverStyle", ref vehicleH))
            {
                HealthTextStyle style = ReadStyle(vehicleH, HealthTextStyle.SHORT);
                this.GreenData.VehicleHoverStyle = style;
            }
            vehicleH = null;
            if (reader.ReadNormal(section, "HealthText.Yellow.VehicleHoverStyle", ref vehicleH))
            {
                HealthTextStyle style = ReadStyle(vehicleH, HealthTextStyle.SHORT);
                this.YellowData.VehicleHoverStyle = style;
            }
            vehicleH = null;
            if (reader.ReadNormal(section, "HealthText.Red.VehicleHoverStyle", ref vehicleH))
            {
                HealthTextStyle style = ReadStyle(vehicleH, HealthTextStyle.SHORT);
                this.RedData.VehicleHoverStyle = style;
            }

            string building = null;
            if (reader.ReadNormal(section, "HealthText.BuildingStyle", ref building))
            {
                HealthTextStyle style = ReadStyle(building, HealthTextStyle.FULL);
                this.GreenData.BuildingStyle = style;
                this.YellowData.BuildingStyle = style;
                this.RedData.BuildingStyle = style;
            }
            building = null;
            if (reader.ReadNormal(section, "HealthText.Green.BuildingStyle", ref building))
            {
                HealthTextStyle style = ReadStyle(building, HealthTextStyle.FULL);
                this.GreenData.BuildingStyle = style;
            }
            building = null;
            if (reader.ReadNormal(section, "HealthText.Yellow.BuildingStyle", ref building))
            {
                HealthTextStyle style = ReadStyle(building, HealthTextStyle.FULL);
                this.YellowData.BuildingStyle = style;
            }
            building = null;
            if (reader.ReadNormal(section, "HealthText.Red.BuildingStyle", ref building))
            {
                HealthTextStyle style = ReadStyle(building, HealthTextStyle.FULL);
                this.RedData.BuildingStyle = style;
            }

            string buildingH = null;
            if (reader.ReadNormal(section, "HealthText.BuildingHoverStyle", ref buildingH))
            {
                HealthTextStyle style = ReadStyle(buildingH, HealthTextStyle.SHORT);
                this.GreenData.BuildingHoverStyle = style;
                this.YellowData.BuildingHoverStyle = style;
                this.RedData.BuildingHoverStyle = style;
            }
            buildingH = null;
            if (reader.ReadNormal(section, "HealthText.Green.BuildingHoverStyle", ref buildingH))
            {
                HealthTextStyle style = ReadStyle(buildingH, HealthTextStyle.SHORT);
                this.GreenData.BuildingHoverStyle = style;
            }
            buildingH = null;
            if (reader.ReadNormal(section, "HealthText.Yellow.BuildingHoverStyle", ref buildingH))
            {
                HealthTextStyle style = ReadStyle(buildingH, HealthTextStyle.SHORT);
                this.YellowData.BuildingHoverStyle = style;
            }
            buildingH = null;
            if (reader.ReadNormal(section, "HealthText.Red.BuildingHoverStyle", ref buildingH))
            {
                HealthTextStyle style = ReadStyle(buildingH, HealthTextStyle.SHORT);
                this.RedData.BuildingHoverStyle = style;
            }

            Point2D offset = default;
            if (ExHelper.ReadPoint2D(reader, section, "HealthText.Offset", ref offset))
            {
                this.GreenData.Offset = offset;
                this.YellowData.Offset = offset;
                this.RedData.Offset = offset;
            }
            offset = default;
            if (ExHelper.ReadPoint2D(reader, section, "HealthText.Green.Offset", ref offset))
            {
                this.GreenData.Offset = offset;
            }
            offset = default;
            if (ExHelper.ReadPoint2D(reader, section, "HealthText.Yellow.Offset", ref offset))
            {
                this.YellowData.Offset = offset;
            }
            offset = default;
            if (ExHelper.ReadPoint2D(reader, section, "HealthText.Red.Offset", ref offset))
            {
                this.RedData.Offset = offset;
            }

            // 文字设置
            Point2D shadowOffset = default;
            if (ExHelper.ReadPoint2D(reader, section, "HealthText.ShadowOffset", ref shadowOffset))
            {
                this.GreenData.ShadowOffset = shadowOffset;
                this.YellowData.ShadowOffset = shadowOffset;
                this.RedData.ShadowOffset = shadowOffset;
            }
            shadowOffset = default;
            if (ExHelper.ReadPoint2D(reader, section, "HealthText.Green.ShadowOffset", ref shadowOffset))
            {
                this.GreenData.ShadowOffset = shadowOffset;
            }
            shadowOffset = default;
            if (ExHelper.ReadPoint2D(reader, section, "HealthText.Yellow.ShadowOffset", ref shadowOffset))
            {
                this.YellowData.ShadowOffset = shadowOffset;
            }
            shadowOffset = default;
            if (ExHelper.ReadPoint2D(reader, section, "HealthText.Red.ShadowOffset", ref shadowOffset))
            {
                this.RedData.ShadowOffset = shadowOffset;
            }

            ColorStruct color = default;
            if (ExHelper.ReadColorStruct(reader, section, "HealthText.Color", ref color))
            {
                this.GreenData.Color = color;
                this.YellowData.Color = color;
                this.RedData.Color = color;
            }
            color = default;
            if (ExHelper.ReadColorStruct(reader, section, "HealthText.Green.Color", ref color))
            {
                this.GreenData.Color = color;
            }
            color = default;
            if (ExHelper.ReadColorStruct(reader, section, "HealthText.Yellow.Color", ref color))
            {
                this.YellowData.Color = color;
            }
            color = default;
            if (ExHelper.ReadColorStruct(reader, section, "HealthText.Red.Color", ref color))
            {
                this.RedData.Color = color;
            }

            ColorStruct shadowColor = default;
            if (ExHelper.ReadColorStruct(reader, section, "HealthText.ShadowColor", ref shadowColor))
            {
                this.GreenData.ShadowColor = shadowColor;
                this.YellowData.ShadowColor = shadowColor;
                this.RedData.ShadowColor = shadowColor;
            }
            shadowColor = default;
            if (ExHelper.ReadColorStruct(reader, section, "HealthText.Green.ShadowColor", ref shadowColor))
            {
                this.GreenData.ShadowColor = shadowColor;
            }
            shadowColor = default;
            if (ExHelper.ReadColorStruct(reader, section, "HealthText.Yellow.ShadowColor", ref shadowColor))
            {
                this.YellowData.ShadowColor = shadowColor;
            }
            shadowColor = default;
            if (ExHelper.ReadColorStruct(reader, section, "HealthText.Red.ShadowColor", ref shadowColor))
            {
                this.RedData.ShadowColor = shadowColor;
            }

            // SHP设置
            bool useSHP = false;
            if (reader.ReadNormal(section, "HealthText.UseSHP", ref useSHP))
            {
                this.GreenData.UseSHP = useSHP;
                this.YellowData.UseSHP = useSHP;
                this.RedData.UseSHP = useSHP;
            }
            useSHP = false;
            if (reader.ReadNormal(section, "HealthText.Green.UseSHP", ref useSHP))
            {
                this.GreenData.UseSHP = useSHP;
            }
            useSHP = false;
            if (reader.ReadNormal(section, "HealthText.Yellow.UseSHP", ref useSHP))
            {
                this.YellowData.UseSHP = useSHP;
            }
            useSHP = false;
            if (reader.ReadNormal(section, "HealthText.Red.UseSHP", ref useSHP))
            {
                this.RedData.UseSHP = useSHP;
            }

            string fileName = null;
            if (reader.ReadNormal(section, "HealthText.SHP", ref fileName))
            {
                string file = fileName;
                if (!string.IsNullOrEmpty(fileName) && !(file = fileName.ToLower()).Equals("pips.shp"))
                {
                    this.GreenData.CustomSHP = true;
                    this.GreenData.SHPFileName = file;
                    this.YellowData.CustomSHP = true;
                    this.YellowData.SHPFileName = file;
                    this.RedData.CustomSHP = true;
                    this.RedData.SHPFileName = file;
                }
            }
            fileName = null;
            if (reader.ReadNormal(section, "HealthText.Green.SHP", ref fileName))
            {
                string file = fileName;
                if (!string.IsNullOrEmpty(fileName) && !(file = fileName.ToLower()).Equals("pips.shp"))
                {
                    this.GreenData.CustomSHP = true;
                    this.GreenData.SHPFileName = file;
                }
            }
            fileName = null;
            if (reader.ReadNormal(section, "HealthText.Yellow.SHP", ref fileName))
            {
                string file = fileName;
                if (!string.IsNullOrEmpty(fileName) && !(file = fileName.ToLower()).Equals("pips.shp"))
                {
                    this.YellowData.CustomSHP = true;
                    this.YellowData.SHPFileName = file;
                }
            }
            fileName = null;
            if (reader.ReadNormal(section, "HealthText.Red.SHP", ref fileName))
            {
                string file = fileName;
                if (!string.IsNullOrEmpty(fileName) && !(file = fileName.ToLower()).Equals("pips.shp"))
                {
                    this.RedData.CustomSHP = true;
                    this.RedData.SHPFileName = file;
                }
            }

            int idx = 0;
            if (reader.ReadNormal(section, "HealthText.ZeroFrameIndex", ref idx))
            {
                this.GreenData.ZeroFrameIndex = idx;
                this.YellowData.ZeroFrameIndex = idx;
                this.RedData.ZeroFrameIndex = idx;
            }
            idx = 0;
            if (reader.ReadNormal(section, "HealthText.Green.ZeroFrameIndex", ref idx))
            {
                this.GreenData.ZeroFrameIndex = idx;
            }
            idx = 0;
            if (reader.ReadNormal(section, "HealthText.Yellow.ZeroFrameIndex", ref idx))
            {
                this.YellowData.ZeroFrameIndex = idx;
            }
            idx = 0;
            if (reader.ReadNormal(section, "HealthText.Red.ZeroFrameIndex", ref idx))
            {
                this.RedData.ZeroFrameIndex = idx;
            }

            Point2D imgSize = default;
            if (ExHelper.ReadPoint2D(reader, section, "HealthText.ImageSize", ref imgSize))
            {
                this.GreenData.ImageSize = imgSize;
                this.YellowData.ImageSize = imgSize;
                this.RedData.ImageSize = imgSize;
            }
            imgSize = default;
            if (ExHelper.ReadPoint2D(reader, section, "HealthText.Green.ImageSize", ref imgSize))
            {
                this.GreenData.ImageSize = imgSize;
            }
            imgSize = default;
            if (ExHelper.ReadPoint2D(reader, section, "HealthText.Yellow.ImageSize", ref imgSize))
            {
                this.YellowData.ImageSize = imgSize;
            }
            imgSize = default;
            if (ExHelper.ReadPoint2D(reader, section, "HealthText.Red.ImageSize", ref imgSize))
            {
                this.RedData.ImageSize = imgSize;
            }
        }

        public HealthTextControlData Clone()
        {
            HealthTextControlData data = new HealthTextControlData();
            data.GreenData = this.GreenData.Clone();
            data.YellowData = this.YellowData.Clone();
            data.RedData = this.RedData.Clone();
            return data;
        }
    }

    [Serializable]
    public class HealthTextData : PrintTextData
    {
        public bool Hidden;
        public HealthTextStyle InfantryStyle;
        public HealthTextStyle InfantryHoverStyle;
        public HealthTextStyle VehicleStyle;
        public HealthTextStyle VehicleHoverStyle;
        public HealthTextStyle BuildingStyle;
        public HealthTextStyle BuildingHoverStyle;

        public HealthTextData() : this(HealthState.Green) { }

        public HealthTextData(HealthState healthState) : base()
        {
            this.Hidden = false;
            this.InfantryStyle = HealthTextStyle.SHORT;
            this.InfantryHoverStyle = HealthTextStyle.SHORT;
            this.VehicleStyle = HealthTextStyle.FULL;
            this.VehicleHoverStyle = HealthTextStyle.SHORT;
            this.BuildingStyle = HealthTextStyle.FULL;
            this.BuildingHoverStyle = HealthTextStyle.SHORT;
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
            Pointer<TechnoClass> pTechno = OwnerObject;
            bool isSelected = pTechno.Ref.Base.IsSelected;
            if (!Type.HealthTextControlData.Hidden && (Type.HealthTextControlData.ShowEnemy || pTechno.Ref.Owner.Ref.PlayerControl) && (isSelected || Type.HealthTextControlData.ShowHover))
            {
                PrintHealthText(length, pLocation, pBound, true, isSelected);
            }
        }

        public unsafe void TechnoClass_DrawHealthBar_Other_Text(int length, Pointer<Point2D> pLocation, Pointer<RectangleStruct> pBound)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            bool isSelected = pTechno.Ref.Base.IsSelected;
            if (!Type.HealthTextControlData.Hidden && (Type.HealthTextControlData.ShowEnemy || pTechno.Ref.Owner.Ref.PlayerControl) && (isSelected || Type.HealthTextControlData.ShowHover))
            {
                PrintHealthText(length, pLocation, pBound, false, isSelected);
            }
        }

        private void PrintHealthText(int length, Pointer<Point2D> pLocation, Pointer<RectangleStruct> pBound, bool isBuilding, bool isSelected)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            // 根据血量状态获取设置
            HealthTextData data = Type.HealthTextControlData.GreenData;
            HealthState healthState = pTechno.Ref.Base.GetHealthStatus();
            switch (healthState)
            {
                case HealthState.Yellow:
                    data = Type.HealthTextControlData.YellowData;
                    break;
                case HealthState.Red:
                    data = Type.HealthTextControlData.RedData;
                    break;
            }
            if (data.Hidden)
            {
                return;
            }

            // 调整锚点
            Point2D pos = pLocation.Data;
            int xOffset = data.Offset.X; // 锚点向右的偏移值
            int yOffset = data.Offset.Y; // 锚点向下的偏移值

            // Point2D fountSize = data.FontSize; // 使用shp则按照shp图案大小来偏移锚点

            HealthTextStyle style = HealthTextStyle.FULL; // 数值的格式
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
                style = isSelected ? data.BuildingStyle : data.BuildingHoverStyle;
            }
            else
            {
                yOffset += pTechno.Ref.Type.Ref.PixelSelectionBracketDelta;
                if (length == 8)
                {
                    // 步兵血条
                    pos.X = pos.X - 7 + xOffset;
                    pos.Y = pos.Y - 28 + yOffset;
                    // 数值格式
                    style = isSelected ? data.InfantryStyle : data.InfantryHoverStyle;
                }
                else
                {
                    // 载具血条
                    pos.X = pos.X - 17 + xOffset;
                    pos.Y = pos.Y - 29 + yOffset;
                    // 数值格式
                    style = isSelected ? data.VehicleStyle : data.VehicleHoverStyle;
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
        public HealthTextControlData HealthTextControlData;

        /// <summary>
        /// [AudioVisual]
        /// [TechnoType] ;覆盖全局设置
        /// HealthText.Hidden=no ;停用该功能
        /// HealthText.ShowEnemy=no ;显示给敌方
        /// HealthText.ShowHover=no ;鼠标悬停时是否显示
        /// ; X表示三种颜色状态，分别是Green\Yellow\Red，不写则是全局设置，如HealthText.Hidden=yes，不论何种状态全部隐藏，HealthText.Green.Hidden=yes，则只在绿血时隐藏。
        /// HealthText.X.Hidden=no ;隐藏显示
        /// HealthText.X.Offset=0,0 ;锚点向右和向下的偏移位置
        /// HealthText.Green.Color=0,252,0 ;绿血时字体颜色
        /// HealthText.Yellow.Color=252,212,0 ;黄血时字体颜色
        /// HealthText.Red.Color=252,0,0 ;红血时字体颜色
        /// ; 数字格式
        /// HealthText.X.InfantryStyle=SHORT ;数显的类型，FULL\SHORT\PERCENT
        /// HealthText.X.InfantryHoverStyle=SHORT ;鼠标悬停时数显的类型，FULL\SHORT\PERCENT
        /// HealthText.X.VehicleStyle=FULL ;数显的类型，FULL\SHORT\PERCENT
        /// HealthText.X.VehicleHoverStyle=SHORT ;鼠标悬停时数显的类型，FULL\SHORT\PERCENT
        /// HealthText.X.BuildingStyle=FULL ;数显的类型，FULL\SHORT\PERCENT
        /// HealthText.X.BuildingHoverStyle=SHORT ;鼠标悬停时数显的类型，FULL\SHORT\PERCENT
        /// ; 使用shp而不是font显示血量数字
        /// HealthText.X.UseSHP=no ;使用shp而不是文字，默认使用pips.shp，每个颜色15帧，每字一帧，顺序“0123456789+-*/%”，图像中心即锚点在字体的左上角
        /// HealthText.X.SHP=pips.shp ;绿色血量的shp文件
        /// HealthText.X.ImageSize=4,6 ;绿色血量的图案宽度和高度
        /// HealthText.Green.ZeroFrameIndex=24 ;绿色血量的0帧所在序号
        /// HealthText.Yellow.ZeroFrameIndex=39 ;黄色血量的0帧所在序号
        /// HealthText.Red.ZeroFrameIndex=54 ;红色血量的0帧所在序号
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadHelthText(INIReader reader, string section)
        {
            if (null == HealthTextControlData && null != RulesExt.Instance.GeneralHealthTextControlData)
            {
                HealthTextControlData = RulesExt.Instance.GeneralHealthTextControlData.Clone();
            }

            HealthTextControlData.ReadHealthText(reader, section);
        }
    }

    public partial class RulesExt
    {
        public HealthTextControlData GeneralHealthTextControlData = new HealthTextControlData();

        private void ReadHealthText(INIReader reader)
        {
            GeneralHealthTextControlData.ReadHealthText(reader, SectionAV);
        }
    }

}