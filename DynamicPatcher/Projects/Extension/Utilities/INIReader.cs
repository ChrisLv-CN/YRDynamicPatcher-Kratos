using Extension.Script;
using Extension.Ext;
using PatcherYRpp;
using DynamicPatcher;
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

        //public T Read<T>(string section, string key)
        //{
        //    T buffer = default;

        //    switch (buffer)
        //    {
        //        case string:
        //        case bool:
        //        case int:
        //        case byte:
        //        case float:
        //        case double:
        //            return ReadNormal<T>(section, key);
        //        case Pointer<SuperWeaponTypeClass>:
        //            return ReadSuperWeapon(section, key);
        //        default:
        //            break;
        //    }
        //}

        public bool ReadNormal<T>(string section, string key, ref T buffer)
        {
            return parser.Read(section, key, ref buffer);
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

        public bool ReadScript<TScript>(string section, string key, ref TScript buffer) where TScript : Script.Script
        {
            string val = default;
            if (ReadNormal(section, key, ref val))
            {
                buffer = ScriptManager.GetScript<TScript>(val);
                return true;
            }
            return false;
        }


    }
}
