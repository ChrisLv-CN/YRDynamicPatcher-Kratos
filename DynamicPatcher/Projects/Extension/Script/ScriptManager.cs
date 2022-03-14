using DynamicPatcher;
using Extension.Components;
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
        // type name -> script
        static Dictionary<string, Script> Scripts = new Dictionary<string, Script>();

        /// <summary>
        /// create script or get a exist script by script name
        /// </summary>
        /// <param name="scriptName"></param>
        /// <returns></returns>
        public static Script GetScript(string scriptName)
        {
            if(scriptName == null)
                return null; 

            if (Scripts.TryGetValue(scriptName, out Script script))
            {
                return script;
            }
            else
            {
                Script newScript = new Script(scriptName);
                try
                {
                    Assembly assembly = FindScriptAssembly(scriptName);

                    RefreshScript(newScript, assembly);

                    Scripts.Add(scriptName, newScript);
                    return newScript;
                }
                catch (Exception e)
                {
                    Logger.LogError("ScriptManager could not find script: {0}", scriptName);
                    Logger.PrintException(e);
                    return null;
                }
            }
        }
        /// <summary>
        /// get all scripts in cs file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<Script> GetScripts(string fileName)
        {
            List<Script> scripts = new List<Script>();

            var pair = Program.Patcher.FileAssembly.First((pair) => pair.Key.EndsWith(fileName));
            Assembly assembly = pair.Value;

            Type[] types = FindScriptTypes(assembly);

            foreach (var type in types)
            {
                Script script = GetScript(type.Name);
                scripts.Add(script);
            }

            return scripts;
        }
        /// <summary>
        /// get scripts by script names or cs file names
        /// </summary>
        /// <param name="scriptList"></param>
        /// <returns></returns>
        public static List<Script> GetScripts(List<string> scriptList)
        {
            List<Script> scripts = new List<Script>();

            foreach (string item in scriptList)
            {
                if (item.EndsWith(".cs"))
                {
                    scripts.AddRange(GetScripts(item));
                }
                else
                {
                    scripts.Add(GetScript(item));
                }
            }

            return scripts;
        }

        public static void CreateScriptableTo<T>(Component root, IEnumerable<Script> scripts, T owner)
        {
            if (scripts == null)
                return;

            foreach (var script in scripts)
            {
                CreateScriptableTo(root, script, owner);
            }
        }
        public static Scriptable<T> CreateScriptableTo<T>(Component root, Script script, T owner)
        {
            if (script == null)
                return null;

            var scriptComponent = CreateScriptable(script, owner);
            scriptComponent.AttachToComponent(root);
            return scriptComponent;
        }
        public static TScriptable CreateScriptable<TScriptable>(Script script, params object[] parameters) where TScriptable : ScriptComponent
        {
            if (script == null)
                return null;

            var scriptable = Activator.CreateInstance(script.ScriptableType, parameters) as TScriptable;
            scriptable.Script = script;
            return scriptable;
        }

        public static Scriptable<T> CreateScriptable<T>(Script script, T owner)
        {
            return CreateScriptable<Scriptable<T>>(script, owner);
        }

        private static Type[] FindScriptTypes(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            if (types == null || types.Length == 0)
                return new Type[0];

            return types.Where(t => typeof(IScriptable).IsAssignableFrom(t)).ToArray();
        }

        private static Assembly FindScriptAssembly(string scriptName)
        {
            foreach (var pair in Program.Patcher.FileAssembly)
            {
                Assembly assembly = pair.Value;
                Type[] types = FindScriptTypes(assembly);
                foreach (Type type in types)
                {
                    if (type.Name == scriptName)
                    {
                        return assembly;
                    }
                }
            }

            return null;
        }

        private static void RefreshScript(Script script, Assembly assembly)
        {
            Type[] types = FindScriptTypes(assembly);
            foreach (Type type in types)
            {
                if (type.Name == script.Name)
                {
                    script.ScriptableType = type;
                    break;
                }
            }
        }

        private static void RefreshScripts(Assembly assembly)
        {
            Type[] types = FindScriptTypes(assembly);

            foreach (Type type in types)
            {
                string scriptName = type.Name;
                if (Scripts.TryGetValue(scriptName, out Script script))
                {
                    RefreshScript(script, assembly);
                }
                else
                {
                    script = GetScript(scriptName);
                }

                Logger.Log("refresh script: {0}", script.Name);
            }

        }


        private static void Patcher_AssemblyRefresh(object sender, AssemblyRefreshEventArgs args)
        {
            Assembly assembly = args.RefreshedAssembly;
            RefreshScripts(assembly);

            Type[] types = FindScriptTypes(assembly);
            foreach (Type type in types)
            {

                // [warning!] unsafe change to scriptable
                unsafe
                {
                    // refresh modified scripts only
                    void RefreshScriptComponents<TExt>(TExt ext) where TExt : IHaveComponent
                    {
                        Component root = ext.AttachedComponent;
                        ScriptComponent[] components = root.GetComponentsInChildren(c => c.GetType().Name == type.Name).Cast<ScriptComponent>().ToArray();
                        if (components.Length > 0)
                        {
                            foreach (var component in components)
                            {
                                component.DetachFromParent();
                            }
                            var scripts = components.Select(c => c.Script);
                            CreateScriptableTo(root, scripts, ext);
                        }
                    }
                    ref var technoArray = ref TechnoClass.Array;
                    for (int i = 0; i < technoArray.Count; i++)
                    {
                        var pItem = technoArray[i];
                        var ext = TechnoExt.ExtMap.Find(pItem);
                        RefreshScriptComponents(ext);
                    }

                    ref var bulletArray = ref BulletClass.Array;
                    for (int i = 0; i < bulletArray.Count; i++)
                    {
                        var pItem = bulletArray[i];
                        var ext = BulletExt.ExtMap.Find(pItem);
                        RefreshScriptComponents(ext);
                    }

                }
            }
        }

        private static void RefreshScriptable<TExt, TBase>() where TExt : IScriptable
        {

        }

        static ScriptManager()
        {
            Program.Patcher.AssemblyRefresh += Patcher_AssemblyRefresh;
        }
    }
}
