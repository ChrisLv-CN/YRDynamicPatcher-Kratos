using System.Drawing;
using System.Threading;
using PatcherYRpp;
using PatcherYRpp.FileFormats;
using PatcherYRpp.Utilities;
using Extension;
using Extension.Utilities;
using DynamicPatcher;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using Extension.Ext;

namespace Extension.Utilities
{


    public static partial class ExHelper
    {
        private static Regex percentFloat = new Regex(@"^\d?\.\d+$");
        private static Regex percentNumber = new Regex(@"^\d+$");

        public static bool ReadPercent(this INIReader reader, string section, string key, ref double percent, bool allowNegative = false)
        {
            string chanceStr = null;

            if (reader.ReadNormal(section, key, ref chanceStr))
            {
                if (!string.IsNullOrEmpty(chanceStr))
                {
                    chanceStr = chanceStr.Trim();
                    // 写负数等于0
                    if (!allowNegative && chanceStr.IndexOf("-") > -1)
                    {
                        percent = 0;
                        Logger.LogWarning("read ini [{0}]{1} is wrong. value {2} type error.", section, key, chanceStr);
                        return true;
                    }
                    else
                    {
                        percent = PercentStrToDouble(chanceStr);
                        if (percent > 1)
                        {
                            percent = 1;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public static double PercentStrToDouble(string chanceStr, double defVal = 1)
        {
            double result = defVal;

            if (percentFloat.IsMatch(chanceStr))
            {
                // 小数格式
                result = Convert.ToDouble(chanceStr);
            }
            else if (chanceStr.IndexOf("%") > 0)
            {
                // 百分数格式
                string temp = chanceStr.Substring(0, chanceStr.IndexOf("%"));
                result = Convert.ToDouble(temp) / 100;
            }
            else if (percentNumber.IsMatch(chanceStr))
            {
                // 数字格式
                result = Convert.ToDouble(chanceStr) / 100;
            }
            return result;
        }

        public static bool ReadBulletVelocity(this INIReader reader, string section, string key, ref BulletVelocity velocity)
        {
            SingleVector3D vector3D = default;
            if (ReadSingleVector3D(reader, section, key, ref vector3D))
            {
                velocity.X = vector3D.X;
                velocity.Y = vector3D.Y;
                velocity.Z = vector3D.Z;
                return true;
            }
            return false;
        }

        public static bool ReadCoordStruct(this INIReader reader, string section, string key, ref CoordStruct flh)
        {
            SingleVector3D vector3D = default;
            if (ReadSingleVector3D(reader, section, key, ref vector3D))
            {
                flh.X = (int)vector3D.X;
                flh.Y = (int)vector3D.Y;
                flh.Z = (int)vector3D.Z;
                return true;
            }
            return false;
        }

        public static bool ReadColorStruct(this INIReader reader, string section, string key, ref ColorStruct color)
        {
            SingleVector3D vector3D = default;
            if (ReadSingleVector3D(reader, section, key, ref vector3D))
            {
                color.R = Convert.ToByte(vector3D.X);
                color.G = Convert.ToByte(vector3D.Y);
                color.B = Convert.ToByte(vector3D.Z);
                return true;
            }
            return false;
        }

        public static bool ReadSingleVector3D(this INIReader reader, string section, string key, ref SingleVector3D xyz)
        {
            string sXYZ = default;
            if (reader.ReadNormal(section, key, ref sXYZ))
            {
                string[] pos = sXYZ.Split(',');
                if (null != pos && pos.Length > 0)
                {
                    for (int i = 0; i < pos.Length; i++)
                    {
                        float value = Convert.ToSingle(pos[i].Trim());
                        switch (i)
                        {
                            case 0:
                                xyz.X = value;
                                break;
                            case 1:
                                xyz.Y = value;
                                break;
                            case 2:
                                xyz.Z = value;
                                break;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public static bool ReadPoint2D(this INIReader reader, string section, string key, ref Point2D xy)
        {
            string sXY = default;
            if (reader.ReadNormal(section, key, ref sXY))
            {
                string[] pos = sXY.Split(',');
                if (null != pos && pos.Length > 0)
                {
                    for (int i = 0; i < pos.Length; i++)
                    {
                        int value = Convert.ToInt16(pos[i].Trim());
                        switch (i)
                        {
                            case 0:
                                xy.X = value;
                                break;
                            case 1:
                                xy.Y = value;
                                break;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public static bool ReadStringList(this INIReader reader, string section, string key, ref List<string> list, string filter = "none")
        {
            string text = default;
            if (reader.ReadNormal(section, key, ref text))
            {
                if (null == list)
                {
                    list = new List<string>();
                }
                string[] texts = text.Split(',');
                foreach (string t in texts)
                {
                    string tmp = t.Trim();
                    // 过滤 none
                    if (!String.IsNullOrEmpty(tmp) && filter != tmp.ToLower())
                    {
                        list.Add(tmp);
                    }
                }
                return list.Count > 0;
            }
            return false;
        }

        public static bool ReadIntList(this INIReader reader, string section, string key, ref List<int> list)
        {
            string text = default;
            if (reader.ReadNormal(section, key, ref text))
            {
                if (null == list)
                {
                    list = new List<int>();
                }
                string[] texts = text.Split(',');
                foreach (string v in texts)
                {
                    list.Add(Convert.ToInt32(v));
                }
                return true;
            }
            return false;
        }

        public static bool ReadChanceList(this INIReader reader, string section, string key, ref List<double> list)
        {
            string text = default;
            if (reader.ReadNormal(section, key, ref text))
            {
                if (null == list)
                {
                    list = new List<double>();
                }
                string[] texts = text.Split(',');
                foreach (string v in texts)
                {
                    list.Add(PercentStrToDouble(v));
                }
                return true;
            }
            return false;
        }

    }

}