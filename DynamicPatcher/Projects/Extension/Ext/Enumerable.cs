using System.Reflection;
using System.Collections;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
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
    public class Enumerable<T>
    {
        public String Name;

        public Enumerable(string name)
        {
            this.Name = name;
        }

        ~Enumerable() { }

        public virtual void LoadFromINI(Pointer<CCINIClass> pINI) { }



        public static Dictionary<string, Enumerable<T>> Items = new Dictionary<string, Enumerable<T>>();

        public static Enumerable<T> Find(string section)
        {
            if (Items.TryGetValue(section, out Enumerable<T> item))
            {
                return item;
            }
            return null;
        }

        public static Enumerable<T> FindOrAllocate(string section, Pointer<CCINIClass> pINI)
        {
            if (string.IsNullOrEmpty(section))
            {
                Logger.Log("CTOR of {0} attempted for a NULL pointer! WTF!\n", section);
                return null;
            }
            Enumerable<T> find = Find(section);
            if (null == find)
            {
                // Logger.Log("创建 {0} 的实例 {1}, INI指针 {2}", typeof(T).Name, section, pINI.Pointer);
                // 创建类型的实例
                find = Activator.CreateInstance(typeof(T), section) as Enumerable<T>;
                Items.Add(section, find);
                // 读取参数
                if (!pINI.IsNull)
                {
                    // Logger.Log("从INI中读取 {0} 实例 {1} 的参数", typeof(T).Name, section);
                    find.LoadFromINI(pINI);
                }
            }
            return find;
        }

        public static void Clear()
        {
            if (Items.Count > 0)
            {
                Logger.Log("Cleared {0} items from {1}.\n", Items.Count, typeof(Enumerable<T>).ToString());
                Items.Clear();
            }
        }

        public static void LoadFromINIList(Pointer<CCINIClass> pINI, string mainSection)
        {
            if (string.IsNullOrEmpty(mainSection))
            {
                return;
            }

            int len = pINI.Ref.GetKeyCount(mainSection);
            // Logger.Log("载入注册表 {0} ,有 {1} 项", mainSection, len);
            INIReader reader = new INIReader(pINI);
            for (int i = 0; i < len; i++)
            {
                string key = pINI.Ref.GetKeyName(mainSection, i);
                string val = null;
                if (reader.ReadNormal(mainSection, key, ref val))
                {
                    FindOrAllocate(val, pINI);
                }
            }

        }

    }


}
