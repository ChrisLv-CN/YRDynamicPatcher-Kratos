using System.Drawing;
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
    public class PrintTextManager
    {
        private static Point2D fontSize = default;
        public static Point2D FontSize
        {
            get
            {
                if (default == fontSize)
                {
                    string temp = "0123456789+-*/%";
                    RectangleStruct fontRect = Drawing.GetTextDimensions(temp, default, 0, 0, 0);
                    int x = fontRect.Width / 15;
                    fontSize.X = x % 2 == 0 ? x : x + 1;
                    fontSize.Y = fontRect.Height;
                    // Logger.Log($"{Game.CurrentFrame}, FontSize = {fontSize}");
                }
                return fontSize;
            }
        }

        public static Queue<RollingText> RollingTextQueue = new Queue<RollingText>();

        public static void RollingText(string text, CoordStruct location, Point2D offset, int rollSpeed, int duration, PrintTextData data)
        {
            RollingText rollingText = new RollingText(text, location, offset, rollSpeed, duration, data);
            PrintTextManager.RollingTextQueue.Enqueue(rollingText);
        }

        public static void PrintText()
        {
            // 打印滚动文字
            for (int i = 0; i < RollingTextQueue.Count; i++)
            {
                RollingText rollingText = RollingTextQueue.Dequeue();
                // 检查存活然后渲染
                if (rollingText.CanPrint(out Point2D offset, out Point2D pos, out RectangleStruct bound))
                {
                    // 获得锚点位置
                    Point2D pos2 = pos + offset;
                    Print(rollingText.Text, rollingText.Data, pos2, Pointer<RectangleStruct>.AsPointer(ref bound), Surface.Current, false);
                    RollingTextQueue.Enqueue(rollingText);
                }
            }
        }

        public static void Print(string text, PrintTextData data, Point2D pos, Pointer<RectangleStruct> pBound, Pointer<Surface> pSurface, bool isBuilding)
        {
            // 渲染
            if (data.UseSHP)
            {
                // 使用Shp显示数字
                int zeroFrameIndex = data.ZeroFrameIndex; // shp时的起始帧序号
                Point2D imageSize = data.ImageSize; // shp时的图案大小
                // 获取字体横向位移值，即图像宽度，同时计算阶梯高度偏移
                int x = imageSize.X % 2 == 0 ? imageSize.X : imageSize.X + 1;
                int y = isBuilding ? x / 2 : 0;
                // 拆成单个字符
                char[] t = text.ToCharArray();
                foreach (char c in t)
                {
                    int frameIndex = zeroFrameIndex;
                    int frameOffset = 0;
                    // 找到数字或者字符对应的图像帧
                    switch (c)
                    {
                        case '0':
                            frameOffset = 0;
                            break;
                        case '1':
                            frameOffset = 1;
                            break;
                        case '2':
                            frameOffset = 2;
                            break;
                        case '3':
                            frameOffset = 3;
                            break;
                        case '4':
                            frameOffset = 4;
                            break;
                        case '5':
                            frameOffset = 5;
                            break;
                        case '6':
                            frameOffset = 6;
                            break;
                        case '7':
                            frameOffset = 7;
                            break;
                        case '8':
                            frameOffset = 8;
                            break;
                        case '9':
                            frameOffset = 9;
                            break;
                        case '+':
                            frameOffset = 10;
                            break;
                        case '-':
                            frameOffset = 11;
                            break;
                        case '*':
                            frameOffset = 12;
                            break;
                        case '/':
                        case '|':
                            frameOffset = 13;
                            break;
                        case '%':
                            frameOffset = 14;
                            break;
                    }
                    // Logger.Log("{0} - frameIdx = {1}, frameOffset = {2}", Game.CurrentFrame, frameIndex, frameOffset);
                    // 找到对应的帧序号
                    frameIndex += frameOffset;
                    Pointer<SHPStruct> pSHP = FileSystem.PIPS_SHP;
                    if (data.CustomSHP && FileSystem.TyrLoadSHPFile(data.SHPFileName, out Pointer<SHPStruct> pCustomSHP))
                    {
                        pSHP = pCustomSHP;
                        // Logger.Log("{0} - 使用自定义SHP {1}, {2}", Game.CurrentFrame, data.SHPFileName, pSHP);
                    }
                    // 显示对应的帧
                    pSurface.Ref.DrawSHP(FileSystem.PALETTE_PAL, pSHP, frameIndex, pos, pBound);
                    // 调整下一个字符锚点
                    pos.X += x;
                    pos.Y -= y;
                }
            }
            else
            {
                // 使用文字显示数字
                ColorStruct textColor = data.Color; // 文字时渲染颜色
                int x = FontSize.X;
                int y = isBuilding ? FontSize.X / 2 : 0;
                // 拆成单个字符
                char[] t = text.ToCharArray();
                foreach (char c in t)
                {
                    // 画阴影
                    if (default != data.ShadowOffset)
                    {
                        Point2D shadow = pos + data.ShadowOffset;
                        pSurface.Ref.DrawText(c.ToString(), pBound, Pointer<Point2D>.AsPointer(ref shadow), data.ShadowColor);
                    }
                    pSurface.Ref.DrawText(c.ToString(), pBound, Pointer<Point2D>.AsPointer(ref pos), textColor);
                    // 获取字体横向位移值，即图像宽度，同时计算阶梯高度偏移
                    pos.X += x;
                    pos.Y -= y;
                }
            }
        }
    }
}