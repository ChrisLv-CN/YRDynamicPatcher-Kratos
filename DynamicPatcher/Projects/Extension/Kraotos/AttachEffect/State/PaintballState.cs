using System.Reflection;
using System.Collections;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    [Serializable]
    public class PaintballState : AEState<PaintballType>
    {
        public bool NeedPaint(out bool changeColor, out bool changeBright)
        {
            changeColor = false;
            changeBright = false;
            bool active = IsActive() && null != Data;
            if (active)
            {
                changeColor = default != Data.Color;
                changeBright = 1.0f != Data.BrightMultiplier;
            }
            return active;
        }

        public uint GetColor()
        {
            ColorStruct colorAdd = Data.Color.ToColorAdd();
            return colorAdd.Add2RGB565();
        }

        public uint GetBright(uint bright)
        {
            double b = bright * Data.BrightMultiplier;
            if (b < 0)
            {
                b = 0;
            }
            else if (b > 2000)
            {
                b = 2000;
            }
            return (uint)b;
        }

    }
}