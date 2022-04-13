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
            this.LifeTimer = new TimerStruct(duration);
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
        public bool CustomSHP;
        public string SHPFileName;
        public int ZeroFrameIndex;
        public Point2D ImageSize;

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
        }

    }
}
