using Extension.Ext;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Utilities
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    sealed class INILoadActionAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    sealed class SaveActionAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    sealed class LoadActionAttribute : Attribute
    {
    }

    static class PartialHelper
    {
        public static void PartialLoadINIConfig<T>(this Extension<T> ext, Pointer<CCINIClass> pINI)
        {
            Type type = ext.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach (var method in methods)
            {
                if(method.IsDefined(typeof(INILoadActionAttribute), false))
                {
                    method?.Invoke(ext, new object[] { pINI });
                }
            }
        }
        public static void PartialSaveToStream<T>(this Extension<T> ext, IStream stream)
        {
            Type type = ext.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach (var method in methods)
            {
                if (method.IsDefined(typeof(SaveActionAttribute), false))
                {
                    method?.Invoke(ext, new object[] { stream });
                }
            }
        }
        public static void PartialLoadFromStream<T>(this Extension<T> ext, IStream stream)
        {
            Type type = ext.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach (var method in methods)
            {
                if (method.IsDefined(typeof(LoadActionAttribute), false))
                {
                    method?.Invoke(ext, new object[] { stream });
                }
            }
        }
    }
}
