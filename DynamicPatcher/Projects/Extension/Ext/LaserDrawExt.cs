using DynamicPatcher;
using Extension.Script;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    [Serializable]
    public class LaserDrawExt
    {

        // [Hook(HookType.AresHook, Address = 0x550D1F, Size = 6)]
        // public static unsafe UInt32 LaserDrawClass_Context(REGISTERS* R)
        // {
        //     var maxColor = ((Pointer<ColorStruct>)R->lea_Stack(0x14)).Data;
        //     return 0;
        // }

        // [Hook(HookType.AresHook, Address = 0x550F6A, Size = 8)]
        public static unsafe UInt32 LaserDrawClass_Fade(REGISTERS* R)
        {
            // Logger.Log("LaserDrawClass_Fade call...");

            ref LaserDrawClass pThis = ref ((Pointer<LaserDrawClass>)R->EBX).Ref;
            int thickness = pThis.Thickness;

            // Test
            // ColorStruct tInnerColor = pThis.InnerColor;
            // ColorStruct tOuterColor = pThis.OuterColor;
            // ColorStruct tOuterSpread = pThis.OuterSpread;
            // Logger.Log("Draw Laser IsHouseColor={0} IsSupported={1} - inner={2}; outer={3}; spread={4}; thickness={5}; duration={6}; Blinks={7}, BlikState={8}, Fades={9} ",
            //         pThis.IsHouseColor,
            //         pThis.IsSupported,
            //         tInnerColor,
            //         tOuterColor,
            //         tOuterSpread,
            //         thickness,
            //         pThis.Duration,
            //         pThis.Blinks,
            //         pThis.BlinkState,
            //         pThis.Fades
            // );

            var curColor = ((Pointer<ColorStruct>)R->lea_Stack(0x14)).Data;

            bool doNot_quickDraw = R->Stack<bool>(0x13);
            R->ESI = doNot_quickDraw ? 8u : 64u;

            // faster
            if (thickness <= 5)
            {
                R->EAX = (uint)(curColor.R >> 1);
                R->ECX = (uint)(curColor.G >> 1);
                R->EDX = (uint)(curColor.B >> 1);
            }
            else
            {
                int currentThickness = R->Stack<int>(0x5C);
                // Another type code - Kerbiter
                ColorStruct maxColor = curColor;
                // Map value from range of [1, Thickness] to [0, pi]
                double x = Math.PI * (currentThickness - 1) / (thickness - 1);
                double mult = Math.Cos(x);

                // ColorStruct innerColor = pThis.InnerColor;
                // ColorStruct maxColor;
                // if (pThis.IsSupported)
                // {
                //     maxColor = innerColor;
                //     thickness--;
                // }
                // else
                // {
                //     maxColor = new ColorStruct((byte)(innerColor.R >> 1), (byte)(innerColor.G >> 1), (byte)(innerColor.B >> 1));
                // }
                // byte max = Math.Max(maxColor.R, Math.Max(maxColor.G, maxColor.B));

                // double w = Math.PI * ((double)(max - 8) / (double)(2 * thickness * max));
                // double mult = Math.Cos(w * currentThickness);

                R->EAX = (uint)(maxColor.R * mult);
                R->ECX = (uint)(maxColor.G * mult);
                R->EDX = (uint)(maxColor.B * mult);
                // Logger.Log("LaserDrawClass_Fade::RGB:{0},{1},{2}, currentThickness:{3}\n", curColor.R, curColor.G, curColor.B, currentThickness);
            }
            return 0x550F9D;
        }
    }
}