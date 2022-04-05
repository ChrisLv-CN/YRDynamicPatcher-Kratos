using Extension.Script;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Utilities
{
    public class INIReader
    {
        INI_EX parser;

        public INIReader(INI_EX exINI)
        {
            parser = exINI;
        }

        public INIReader(Pointer<CCINIClass> pINI)
        {
            parser = new INI_EX(pINI);
        }

        //public bool Read<T>(string section, string key, ref T buffer)
        //{
        //    Pointer<T> pOutValue = Pointer<T>.AsPointer(ref buffer);

        //    switch (buffer)
        //    {
        //        case string:
        //        case bool:
        //        case int:
        //        case byte:
        //        case float:
        //        case double:
        //            return ReadNormal<T>(section, key, ref buffer);
        //        case Pointer<SuperWeaponTypeClass>:
        //            return ReadSuperWeapon(section, key, ref pOutValue.Convert<Pointer<SuperWeaponTypeClass>>().Ref);
        //        default:
        //            return false;
        //    }
        //}

        public bool ReadNormal<T>(string section, string key, ref T buffer)
        {
            return parser.Read(section, key, ref buffer);
        }
        public bool ReadArray<T>(string section, string key, ref T[] buffer)
        {
            return parser.ReadArray(section, key, ref buffer);
        }
        public bool ReadList<T>(string section, string key, ref List<T> buffer)
        {
            return parser.ReadList(section, key, ref buffer);
        }

        public bool ReadSuperWeapon(string section, string key, ref Pointer<SuperWeaponTypeClass> buffer)
        {
            string val = default;
            if(ReadNormal(section, key, ref val))
            {
                buffer = SuperWeaponTypeClass.ABSTRACTTYPE_ARRAY.Find(val);
                return true;
            }
            return false;
        }

        public bool ReadScripts(string section, string key, ref List<Script.Script> buffer)
        {
            List<string> list = new List<string>();
            if (ReadList(section, key, ref list))
            {
                buffer = ScriptManager.GetScripts(list);
                return true;
            }
            return false;
        }
    }
}
