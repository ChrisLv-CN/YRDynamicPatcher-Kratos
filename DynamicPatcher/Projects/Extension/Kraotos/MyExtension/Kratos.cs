using System.Globalization;
using System.Net.Mime;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    public class Kratos
    {

        public static string FileVersion = GetFileVersion();

        private static TimerStruct showTextTimer = new TimerStruct(150);

        private static bool showMsgFlag = false;

        public static void NewGame()
        {
            showMsgFlag = false;
            showTextTimer.Start(150);
        }

        private static string GetFileVersion()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            if (attributes.Length > 0)
            {
                return ((AssemblyFileVersionAttribute)attributes[0]).Version;
            }
            return null;
        }

        public static void DrawActiveMessage()
        {
            if (!showMsgFlag)
            {
                showMsgFlag = true;
                string label = "DP-Kratos";
                string message = "version " + FileVersion + " is active, have fun.";
                MessageListClass.Instance.PrintMessage(label, message, ColorSchemeIndex.Red, 150, true);
            }

            string text = "DP-Kratos v" + FileVersion;
            RectangleStruct textRect = Drawing.GetTextDimensions(text, new Point2D(0, 0), 0, 2, 0);
            RectangleStruct sidebarRect = Surface.Sidebar.Ref.GetRect();
            int x = sidebarRect.Width / 2 - textRect.Width / 2;
            int y = sidebarRect.Height - textRect.Height;
            Point2D pos = new Point2D(x, y);
            // if (showTextTimer.InProgress())
            // {
            Surface.Sidebar.Ref.DrawText(text, Pointer<Point2D>.AsPointer(ref pos), Drawing.TooltipColor);
            // }
        }
    }

}