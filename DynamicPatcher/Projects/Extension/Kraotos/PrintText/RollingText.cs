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
    public class RollingText : PrintText
    {
        public int RollSpeed;

        public RollingText(string text, CoordStruct location, Point2D offset, int rollSpeed, int duration, PrintTextData data) : base(text, location, offset, duration, data)
        {
            this.Text = text;
            this.Location = location;
            this.Offset = offset;
            this.RollSpeed = rollSpeed;
            this.Duration = duration;
            this.LifeTimer.Start(duration);
            this.Data = data;
        }

        public new bool CanPrint(out Point2D offset, out Point2D pos, out RectangleStruct bound)
        {
            if (base.CanPrint(out offset, out pos, out bound))
            {
                this.Offset -= new Point2D(0, RollSpeed);
                return true;
            }
            return false;
        }

    }

}
