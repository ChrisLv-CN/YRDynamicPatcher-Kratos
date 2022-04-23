using DynamicPatcher;
using Extension.Ext;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            // Type type = ext.GetType();
            // MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);


            // Stopwatch stopwatch = new Stopwatch();
            // stopwatch.Start();

            // foreach (var method in methods)
            // {
            //     if (method.IsDefined(typeof(INILoadActionAttribute), false))
            //     {
            //         method?.Invoke(ext, new object[] { pINI });
            //     }
            // }

            // stopwatch.Stop();
            // Logger.Log($"反射读取 [{typeof(T).Name}] 的ini耗时 {stopwatch.Elapsed}");

            // stopwatch.Reset();
            // stopwatch.Start();
            if (ext is ITypeExtension)
            {
                ((ITypeExtension)ext).LoadINI(pINI);
            }
            // stopwatch.Stop();
            // Logger.Log($" - 直接读取 [{typeof(T).Name}] 的ini耗时 {stopwatch.Elapsed}");

        }
        public static void PartialSaveToStream<T>(this Extension<T> ext, IStream stream)
        {
            // Type type = ext.GetType();
            // MethodInfo[] methods = type.GetMethods();

            // foreach (var method in methods)
            // {
            //     if (method.IsDefined(typeof(SaveActionAttribute), false))
            //     {
            //         method?.Invoke(ext, new object[] { stream });
            //     }
            // }
            if (ext is ITypeExtension)
            {
                ((ITypeExtension)ext).Save(stream);
            }
        }
        public static void PartialLoadFromStream<T>(this Extension<T> ext, IStream stream)
        {
            // Type type = ext.GetType();
            // MethodInfo[] methods = type.GetMethods();

            // foreach (var method in methods)
            // {
            //     if (method.IsDefined(typeof(LoadActionAttribute), false))
            //     {
            //         method?.Invoke(ext, new object[] { stream });
            //     }
            // }
            if (ext is ITypeExtension)
            {
                ((ITypeExtension)ext).Load(stream);
            }
        }
    }
}
