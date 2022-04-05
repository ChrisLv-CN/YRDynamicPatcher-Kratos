using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Utilities
{
    public class Parser<T>
    {
        private static string GetString(byte[] buffer)
        {
            string str = Encoding.UTF8.GetString(buffer);
            str = str.Substring(0, str.IndexOf('\0'));
            str = str.Trim();

            return str;
        }
        public static int Parse(byte[] buffer, ref T outValue)
        {
            string str = GetString(buffer);
            return Parse(str, ref outValue);
        }

        public static int Parse(byte[] buffer, ref T[] outValue)
        {
            string str = GetString(buffer);
            return Parse(str, ref outValue);
        }

        public static int Parse(string str, ref T outValue)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }

            if (TryParse(str, ref outValue))
            {
                return 1;
            }

            return 0;
        }
        public static int Parse(string str, ref T[] outValue)
        {
            string[] strs = str.Split(',');
            int i;
            for (i = 0; i < strs.Length; i++)
            {
                string s = strs[i].Trim();
                if (TryParse(s, ref outValue[i]) == false)
                {
                    return i;
                }
            }

            return i;
        }

        static bool TryParse(string str, ref T outValue)
        {
            Pointer<T> pOutValue = Pointer<T>.AsPointer(ref outValue);

            Type type = typeof(T);

            if (type == typeof(string))
            {
                return TryParseString(str, ref pOutValue.Convert<string>().Ref);
            }
            else if(type== typeof(bool))
            {
                return TryParseBool(str, ref pOutValue.Convert<bool>().Ref);
            }
            else if (type == typeof(int))
            {
                return TryParseInt(str, ref pOutValue.Convert<int>().Ref);
            }
            else if (type == typeof(uint))
            {
                return TryParseUInt(str, ref pOutValue.Convert<uint>().Ref);
            }
            else if (type == typeof(sbyte))
            {
                return TryParseSByte(str, ref pOutValue.Convert<sbyte>().Ref);
            }
            else if (type == typeof(byte))
            {
                return TryParseByte(str, ref pOutValue.Convert<byte>().Ref);
            }
            else if (type == typeof(long))
            {
                return TryParseLong(str, ref pOutValue.Convert<long>().Ref);
            }
            else if (type == typeof(ulong))
            {
                return TryParseULong(str, ref pOutValue.Convert<ulong>().Ref);
            }
            else if (type == typeof(short))
            {
                return TryParseShort(str, ref pOutValue.Convert<short>().Ref);
            }
            else if (type == typeof(ushort))
            {
                return TryParseUShort(str, ref pOutValue.Convert<ushort>().Ref);
            }
            else if (type == typeof(float))
            {
                return TryParseFloat(str, ref pOutValue.Convert<float>().Ref);
            }
            else if (type == typeof(double))
            {
                return TryParseDouble(str, ref pOutValue.Convert<double>().Ref);
            }

            //switch (outValue)
            //{
            //    case string:
            //        return TryParseString(str, ref pOutValue.Convert<string>().Ref);
            //    case bool:
            //        return TryParseBool(str, ref pOutValue.Convert<bool>().Ref);
            //    case int:
            //        return TryParseInt(str, ref pOutValue.Convert<int>().Ref);
            //    case byte:
            //        return TryParseByte(str, ref pOutValue.Convert<byte>().Ref);
            //    case float:
            //        return TryParseFloat(str, ref pOutValue.Convert<float>().Ref);
            //    case double:
            //        return TryParseDouble(str, ref pOutValue.Convert<double>().Ref);
            //    default:
            //        break;
            //}
            return false;
        }

        static bool TryParseString(string str, ref string outValue)
        {
            outValue = str;
            return true;
        }

        static bool TryParseBool(string str, ref bool outValue)
        {
            switch (str.ToUpper()[0])
            {
                case '1':
                case 'T':
                case 'Y':
                    outValue = true;
                    return true;
                case '0':
                case 'F':
                case 'N':
                    outValue = false;
                    return true;
                default:
                    return false;
            }
        }

        static bool TryParseSByte(string str, ref sbyte outValue)
        {
            return sbyte.TryParse(str, out outValue);
        }
        static bool TryParseByte(string str, ref byte outValue)
        {
            return byte.TryParse(str, out outValue);
        }

        static bool TryParseInt(string str, ref int outValue)
        {
            return int.TryParse(str, out outValue);
        }
        static bool TryParseUInt(string str, ref uint outValue)
        {
            return uint.TryParse(str, out outValue);
        }

        static bool TryParseShort(string str, ref short outValue)
        {
            return short.TryParse(str, out outValue);
        }
        static bool TryParseUShort(string str, ref ushort outValue)
        {
            return ushort.TryParse(str, out outValue);
        }

        static bool TryParseLong(string str, ref long outValue)
        {
            return long.TryParse(str, out outValue);
        }
        static bool TryParseULong(string str, ref ulong outValue)
        {
            return ulong.TryParse(str, out outValue);
        }

        static bool TryParseFloat(string str, ref float outValue)
        {
            return float.TryParse(str, out outValue);
        }

        static bool TryParseDouble(string str, ref double outValue)
        {
            return double.TryParse(str, out outValue);
        }
    }

    public class INI_EX
    {
        Pointer<CCINIClass> IniFile;
        static byte[] readBuffer = new byte[2048];

        public INI_EX(Pointer<CCINIClass> pIniFile)
        {
            IniFile = pIniFile;
        }

        public byte[] value()
        {
            return readBuffer;
        }

        public int max_size()
        {
            return readBuffer.Length;
        }

        public bool empty()
        {
            return readBuffer[0] == 0;
        }

        // basic string reader
        public int ReadString(string section, string key)
        {
            return IniFile.Ref.ReadString(section, key, "", value(), max_size());
        }

        public bool Read<T>(string section, string key, ref T pBuffer, int Count = 1)
        {
            if (ReadString(section, key) > 0)
            {
                return Parser<T>.Parse(value(), ref pBuffer) == Count;
            }
            return false;
        }
        public bool Read<T>(string section, string key, ref T[] pBuffer, int Count = 1)
        {
            if (ReadString(section, key) > 0)
            {
                return Parser<T>.Parse(value(), ref pBuffer) == Count;
            }
            return false;
        }

        public bool ReadBool(string section, string key, ref bool bBuffer)
        {
            return Read(section, key, ref bBuffer, 1);
        }

        public bool ReadInteger(string section, string key, ref int nBuffer)
        {
            return Read(section, key, ref nBuffer, 1);
        }

        public bool ReadDouble(string section, string key, ref double nBuffer)
        {
            return Read(section, key, ref nBuffer, 1);
        }

        public bool ReadArray<T>(string section, string key, ref T[] aBuffer)
        {
            return Read(section, key, ref aBuffer, aBuffer.Length);
        }

        public bool ReadList<T>(string section, string key, ref List<T> list)
        {
            string str = null;
            if (Read(section, key, ref str))
            {
                string[] strs = str.Split(',');
                T[] buffer = new T[strs.Length];
                if (ReadArray(section,key, ref buffer))
                {
                    list.Clear();
                    list.AddRange(buffer);
                    return true;
                }
            }

            return false;
        }
    }
}
