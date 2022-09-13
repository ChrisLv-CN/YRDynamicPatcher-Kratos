using System.Collections;
using DynamicPatcher;
using Extension.Ext;
using Extension.Utilities;
using PatcherYRpp;
using PatcherYRpp.FileFormats;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    [Serializable]
    public enum LongText
    {
        NONE = 0, HIT = 1, MISS = 2, CRIT = 3, GLANCING = 4, BLOCK = 5
    }

    [Serializable]
    public class PrintText
    {
        public string Text;
        public CoordStruct Location;
        public Point2D Offset;
        public int Duration;
        public TimerStruct LifeTimer;
        public PrintTextData Data;

        public PrintText(string text, CoordStruct location, Point2D offset, int duration, PrintTextData data)
        {
            this.Text = text;
            this.Location = location;
            this.Offset = offset;
            this.Duration = duration;
            this.LifeTimer.Start(duration);
            this.Data = data;
        }

        public bool CanPrint(out Point2D offset, out Point2D pos, out RectangleStruct bound)
        {
            offset = this.Offset;
            pos = default;
            bound = default;
            if (LifeTimer.InProgress() && !LocationOutOfViewOrHiddenInFog(out pos, out bound))
            {
                return true;
            }
            return false;
        }

        public bool LocationOutOfViewOrHiddenInFog(out Point2D pos, out RectangleStruct bound)
        {
            // 视野外
            pos = TacticalClass.Instance.Ref.CoordsToClient(Location);
            bound = Surface.Composite.Ref.GetRect();
            if (pos.X < 0 || pos.Y < 0 || pos.X > bound.Width || pos.Y > bound.Height - 32)
            {
                return true;
            }
            // 迷雾下
            if (MapClass.Instance.TryGetCellAt(Location, out Pointer<CellClass> pCell))
            {
                return !pCell.Ref.Flags.HasFlag(CellFlags.Revealed);
            }
            return false;
        }
    }

    [Serializable]
    public class PrintTextData
    {
        public Point2D Offset;
        public Point2D ShadowOffset;
        public ColorStruct Color;
        public ColorStruct ShadowColor;
        public bool UseSHP;
        public bool CustomSHP; // 强制使用shp文件的某一帧来渲染
        public string SHPFileName;
        public int ZeroFrameIndex;
        public Point2D ImageSize;

        public bool NoNumbers; // 不使用数字
        // long text
        public string HitSHP;
        public int HitIndex;
        public string MissSHP;
        public int MissIndex;
        public string CritSHP;
        public int CritIndex;
        public string GlancingSHP;
        public int GlancingIndex;
        public string BlockSHP;
        public int BlockIndex;

        public PrintTextData()
        {
            this.Offset = new Point2D(0, 0);
            this.ShadowOffset = new Point2D(1, 1);
            this.Color = new ColorStruct(252, 252, 252);
            this.ShadowColor = new ColorStruct(82, 85, 82);
            this.UseSHP = false;
            this.CustomSHP = false;
            this.SHPFileName = "pipsnum.shp";
            this.ZeroFrameIndex = 0;
            this.ImageSize = new Point2D(5, 8);

            this.NoNumbers = false;
            // long text
            this.HitSHP = "pipstext.shp";
            this.HitIndex = 0;
            this.MissSHP = "pipstext.shp";
            this.MissIndex = 2;
            this.CritSHP = "pipstext.shp";
            this.CritIndex = 3;
            this.GlancingSHP = "pipstext.shp";
            this.GlancingIndex = 4;
            this.BlockSHP = "pipstext.shp";
            this.BlockIndex = 5;
        }

        public PrintTextData CopyTo(PrintTextData data)
        {
            data.Offset = this.Offset;
            data.ShadowOffset = this.ShadowOffset;
            data.Color = this.Color;
            data.ShadowColor = this.ShadowColor;
            data.UseSHP = this.UseSHP;
            data.CustomSHP = this.CustomSHP;
            data.SHPFileName = this.SHPFileName;
            data.ZeroFrameIndex = this.ZeroFrameIndex;
            data.ImageSize = this.ImageSize;

            data.NoNumbers = this.NoNumbers;
            data.HitSHP = this.HitSHP;
            data.HitIndex = this.HitIndex;
            data.MissSHP = this.MissSHP;
            data.MissIndex = this.MissIndex;
            data.CritSHP = this.CritSHP;
            data.CritIndex = this.CritIndex;
            data.GlancingSHP = this.GlancingSHP;
            data.GlancingIndex = this.GlancingIndex;
            data.BlockSHP = this.BlockSHP;
            data.BlockIndex = this.BlockIndex;
            return data;
        }

        protected bool TryReadPrintText(INIReader reader, string section, string title)
        {
            bool isRead = false;

            Point2D offset = default;
            if (reader.ReadPoint2D(section, title + "Offset", ref offset))
            {
                isRead = true;
                this.Offset = offset;
            }

            // 文字设置
            Point2D shadowOffset = default;
            if (reader.ReadPoint2D(section, title + "ShadowOffset", ref shadowOffset))
            {
                isRead = true;
                this.ShadowOffset = shadowOffset;
            }

            ColorStruct color = default;
            if (reader.ReadColorStruct(section, title + "Color", ref color))
            {
                isRead = true;
                this.Color = color;
            }

            ColorStruct shadowColor = default;
            if (reader.ReadColorStruct(section, title + "ShadowColor", ref shadowColor))
            {
                isRead = true;
                this.ShadowColor = shadowColor;
            }

            // SHP设置
            bool useSHP = false;
            if (reader.ReadNormal(section, title + "UseSHP", ref useSHP))
            {
                isRead = true;
                this.UseSHP = useSHP;
            }

            string fileName = null;
            if (reader.ReadNormal(section, title + "SHP", ref fileName))
            {
                string file = fileName;
                if (!string.IsNullOrEmpty(fileName) && (file = fileName.ToLower()) != this.SHPFileName)
                {
                    isRead = true;
                    this.SHPFileName = file;
                }
            }

            int idx = 0;
            if (reader.ReadNormal(section, title + "ZeroFrameIndex", ref idx))
            {
                isRead = true;
                this.ZeroFrameIndex = idx;
            }

            Point2D imgSize = default;
            if (reader.ReadPoint2D(section, title + "ImageSize", ref imgSize))
            {
                isRead = true;
                this.ImageSize = imgSize;
            }

            bool noNumbers = false;
            if (reader.ReadNormal(section, title + "NoNumbers", ref noNumbers))
            {
                isRead = true;
                this.NoNumbers = noNumbers;
            }

            // Long text
            string longTextSHP = null;
            if (reader.ReadNormal(section, title + "HIT.SHP", ref longTextSHP))
            {
                string file = longTextSHP;
                if (!string.IsNullOrEmpty(fileName) && (file = longTextSHP.ToLower()) != this.HitSHP)
                {
                    isRead = true;
                    this.HitSHP = file;
                }
            }

            int longTextIndex = 0;
            if (reader.ReadNormal(section, title + "HIT.Index", ref longTextIndex))
            {
                isRead = true;
                this.HitIndex = longTextIndex;
            }

            if (reader.ReadNormal(section, title + "MISS.SHP", ref longTextSHP))
            {
                string file = longTextSHP;
                if (!string.IsNullOrEmpty(fileName) && (file = longTextSHP.ToLower()) != this.MissSHP)
                {
                    isRead = true;
                    this.MissSHP = file;
                }
            }

            longTextIndex = 0;
            if (reader.ReadNormal(section, title + "MISS.Index", ref longTextIndex))
            {
                isRead = true;
                this.MissIndex = longTextIndex;
            }

            if (reader.ReadNormal(section, title + "CRIT.SHP", ref longTextSHP))
            {
                string file = longTextSHP;
                if (!string.IsNullOrEmpty(fileName) && (file = longTextSHP.ToLower()) != this.CritSHP)
                {
                    isRead = true;
                    this.CritSHP = file;
                }
            }

            longTextIndex = 0;
            if (reader.ReadNormal(section, title + "CRIT.Index", ref longTextIndex))
            {
                isRead = true;
                this.CritIndex = longTextIndex;
            }

            if (reader.ReadNormal(section, title + "GLANCING.SHP", ref longTextSHP))
            {
                string file = longTextSHP;
                if (!string.IsNullOrEmpty(fileName) && (file = longTextSHP.ToLower()) != this.GlancingSHP)
                {
                    isRead = true;
                    this.GlancingSHP = file;
                }
            }

            longTextIndex = 0;
            if (reader.ReadNormal(section, title + "GLANCING.Index", ref longTextIndex))
            {
                isRead = true;
                this.GlancingIndex = longTextIndex;
            }

            if (reader.ReadNormal(section, title + "BLOCK.SHP", ref longTextSHP))
            {
                string file = longTextSHP;
                if (!string.IsNullOrEmpty(fileName) && (file = longTextSHP.ToLower()) != this.BlockSHP)
                {
                    isRead = true;
                    this.BlockSHP = file;
                }
            }

            longTextIndex = 0;
            if (reader.ReadNormal(section, title + "BLOCK.Index", ref longTextIndex))
            {
                isRead = true;
                this.BlockIndex = longTextIndex;
            }



            return isRead;
        }

    }

}
