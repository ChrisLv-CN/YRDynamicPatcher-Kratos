using DynamicPatcher;
using Extension.Ext;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Script
{
    public partial class ScriptManager
    {
        static Dictionary<string, Script> Scripts = new Dictionary<string, Script>();

        // create script or get a exist script
        public static TScript GetScript<TScript>(string filename) where TScript : Script
        {
            if(filename == null)
                return null; 

            if (Scripts.TryGetValue(filename, out Script script))
            {
                return script as TScript;
            }
            else
            {
                TScript newScript = Activator.CreateInstance(typeof(TScript), filename) as TScript;
                try
                {
                    var pair = Program.Patcher.FileAssembly.First((pair) => Path.GetFileNameWithoutExtension(pair.Key) == filename);
                    Assembly assembly = pair.Value;

                    RefreshScript(newScript, assembly);

                    Scripts.Add(filename, newScript);
                    return newScript;
                }
                catch (Exception e)
                {
                    Logger.LogError("ScriptManager could not find script: {0}", filename);
                    Logger.PrintException(e);
                    return null;
                }
            }
        }

        public static Scriptable<T> GetScriptable<T>(Script script, T owner)
        {
            return script != null ? GetScriptable(script.ScriptableType, owner) : null;
        }

        public static Scriptable<T> GetScriptable<T>(Type scriptableType, T owner)
        {
            var scriptable = Activator.CreateInstance(scriptableType, owner) as Scriptable<T>;
            return scriptable;
        }

        private static void RefreshScript(Script script, Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (typeof(IScriptable).IsAssignableFrom(type))
                {
                    script.SetEvents(type);
                    break;
                }
            }
        }


        private static void Patcher_AssemblyRefresh(object sender, AssemblyRefreshEventArgs args)
        {
            if (Scripts.TryGetValue(args.FileName, out Script script))
            {
                RefreshScript(script, args.RefreshedAssembly);

                // [warning!] unsafe change to scriptable
                unsafe
                {
                    ref var technoArray = ref TechnoClass.Array;
                    for (int i = 0; i < technoArray.Count; i++)
                    {
                        var pItem = technoArray[i];
                        var ext = TechnoExt.ExtMap.Find(pItem);
                        if (ext.Scriptable != null)
                        {
                            var extType = ext.Type;
                            ext.scriptable = new Lazy<TechnoScriptable>(() => GetScriptable(extType.Script, ext) as TechnoScriptable);
                        }
                    }

                    ref var bulletArray = ref BulletClass.Array;
                    for (int i = 0; i < bulletArray.Count; i++)
                    {
                        var pItem = bulletArray[i];
                        var ext = BulletExt.ExtMap.Find(pItem);
                        if (ext.Scriptable != null)
                        {
                            var extType = ext.Type;
                            ext.scriptable = new Lazy<BulletScriptable>(() => GetScriptable(extType.Script, ext) as BulletScriptable);
                        }
                    }
                }
            }
        }

        static ScriptManager()
        {
            Program.Patcher.AssemblyRefresh += Patcher_AssemblyRefresh;
        }
    }
}
