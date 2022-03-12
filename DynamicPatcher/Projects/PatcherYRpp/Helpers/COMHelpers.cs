using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{

    [DebuggerDisplay("PTR={IUnknown}, OBJ={Interface}")]
    [StructLayout(LayoutKind.Sequential)]
    public struct COMPtr<TObject>
    {
        public static Guid Guid = typeof(TObject).GUID;

        public COMPtr(IntPtr pIUnknown)
        {
            this._IUnknown = pIUnknown;
        }

        public COMPtr(TObject obj) : this()
        {
            Object = obj;
        }
        
        public COMPtr(object obj) : this()
        {
            Object = (TObject)obj;
        }

        public TObject Object
        {
            get => QueryInterface<TObject>();
            set
            {
                Release();
                _IUnknown = Marshal.GetIUnknownForObject(value);
                _IUnknown = COMHelpers.QueryInterface(_IUnknown, Guid);
            }
        }
        public TObject Interface
        {
            get => Object;
            set => Object = value;
        }

        public IntPtr IUnknown
        {
            get => _IUnknown;
            set => _IUnknown = value;
        }

        public IntPtr QueryInterfacePtr<TQueryObject>()
        {
            return _IUnknown == IntPtr.Zero ? IntPtr.Zero : COMHelpers.QueryInterface<TQueryObject>(_IUnknown);
        }

        public IntPtr QueryInterfacePtr(Guid iid)
        {
            if (_IUnknown == IntPtr.Zero)
                return IntPtr.Zero;

            try
            {
                return COMHelpers.QueryInterface(_IUnknown, iid);
            }
            catch (COMException)
            {
                return IntPtr.Zero;
            }
        }

        public TQueryObject QueryInterface<TQueryObject>()
        {
            return _IUnknown == IntPtr.Zero ? default : (TQueryObject)Marshal.GetObjectForIUnknown(_IUnknown);
        }

        public void Release()
        {
            if (_IUnknown != IntPtr.Zero)
                Marshal.Release(_IUnknown);
            _IUnknown = IntPtr.Zero;
        }

        public void CreateInstance()
        {
            CreateInstance(Guid);
        }
        public void CreateInstance(Guid clsid)
        {
            Object = COMHelpers.CreateInstance<TObject>(clsid);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator IntPtr(COMPtr<TObject> ptr) => ptr.IUnknown;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator COMPtr<TObject>(IntPtr iUnknown) => new COMPtr<TObject>(iUnknown);

        private IntPtr _IUnknown;
    }

    public static class COMHelpers
    {
        public static TObject CreateInstance<TObject>(Guid clsid)
        {
            Type type = Type.GetTypeFromCLSID(clsid);
            return (TObject)Activator.CreateInstance(type);
        }
        public static TObject CreateInstance<TObject>()
        {
            return CreateInstance<TObject>(typeof(TObject).GUID);
        }
        public static IntPtr QueryInterface(IntPtr pUnk, Guid iid)
        {
            int hResult = Marshal.QueryInterface(pUnk, ref iid, out IntPtr ppv);
            if (hResult != 0)
                throw new COMException(
                    $"QueryInterface({pUnk}, {iid}) fail! HR={hResult}, Message:{Marshal.GetExceptionForHR(hResult).Message}", hResult);
            return ppv;
        }
        public static IntPtr QueryInterface<TObject>(IntPtr pUnk)
        {
            return QueryInterface(pUnk, typeof(TObject).GUID);
        }

        public static COMPtr<TObject> GetCOMPtr<TObject>(this TObject obj)
        {
            return new COMPtr<TObject>(obj);
        }


        public static void RegisterComObject(Type type)
        {
            ComUtilities.RegisterComObject(ComUtilities.Target.Machine, type);
        }
        public static void RegisterComObject<T>()
        {
            RegisterComObject(typeof(T));
        }
        public static void UnregisterComObject(Type type)
        {
            ComUtilities.UnregisterComObject(ComUtilities.Target.Machine, type);
        }
        public static void UnregisterComObject<T>()
        {
            UnregisterComObject(typeof(T));
        }
    }

    // https://stackoverflow.com/questions/35782404/registering-a-com-without-admin-rights
    static class ComUtilities
    {
        private const string ClsidRegistryKey = @"Software\Classes\CLSID";

        public enum Target
        {
            Machine,    // registers or unregisters a .NET COM object in HKEY_LOCAL_MACHINE, for all users, needs proper rights
            User        // registers or unregisters a .NET COM object in HKEY_CURRENT_USER to avoid UAC prompts
        }

        public static void RegisterComObject(Target target, Type type)
        {
            RegisterComObject(target, type, null);
        }

        public static void RegisterComObject(Target target, Type type, string assemblyPath)
        {
            RegisterComObject(target, type, assemblyPath, null);
        }

        public static void RegisterComObject(Target target, Type type, string assemblyPath, string runtimeVersion)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.Assembly == null)
                throw new ArgumentException(null, nameof(type));

            // note we don't check if the type is marked as ComVisible, maybe we should

            if (assemblyPath == null && !string.IsNullOrEmpty(type.Assembly.Location))
            {
                assemblyPath = new Uri(type.Assembly.Location).LocalPath;
            }

            if (runtimeVersion == null)
            {
                runtimeVersion = GetRuntimeVersion(type.Assembly);
            }

            var root = target == Target.User ? Registry.CurrentUser : Registry.LocalMachine;

            using (RegistryKey key = EnsureSubKey(root, Path.Combine(ClsidRegistryKey, type.GUID.ToString("B"), "InprocServer32")))
            {
                key.SetValue(null, "mscoree.dll");
                key.SetValue("Assembly", type.Assembly.FullName);
                key.SetValue("Class", type.FullName);
                key.SetValue("ThreadingModel", "Both");
                if (assemblyPath != null)
                {
                    key.SetValue("CodeBase", assemblyPath);
                }

                key.SetValue("RuntimeVersion", runtimeVersion);
            }

            using (RegistryKey key = EnsureSubKey(root, Path.Combine(ClsidRegistryKey, type.GUID.ToString("B"))))
            {
                // cf http://stackoverflow.com/questions/2070999/is-the-implemented-categories-key-needed-when-registering-a-managed-com-compon
                using (RegistryKey cats = EnsureSubKey(key, @"Implemented Categories\{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}"))
                {
                    // do nothing special
                }

                var att = type.GetCustomAttribute<ProgIdAttribute>();
                if (att != null && !string.IsNullOrEmpty(att.Value))
                {
                    using (RegistryKey progid = EnsureSubKey(key, "ProgId"))
                    {
                        progid.SetValue(null, att.Value);
                    }
                }
            }
        }

        public static void UnregisterComObject(Target target, Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var root = target == Target.User ? Registry.CurrentUser : Registry.LocalMachine;
            using (RegistryKey key = root.OpenSubKey(ClsidRegistryKey, true))
            {
                if (key == null)
                    return;

                key.DeleteSubKeyTree(type.GUID.ToString("B"), false);
            }
        }

        // kind of hack to determine clr version of an assembly
        private static string GetRuntimeVersion(Assembly asm)
        {
            string def = "v4.0.30319"; // use CLR4 as the default
            try
            {
                var mscorlib = asm.GetReferencedAssemblies().FirstOrDefault(a => a.Name == "mscorlib");
                if (mscorlib != null && mscorlib.Version.Major < 4)
                    return "v2.0.50727"; // use CLR2
            }
            catch
            {
                // too bad, assume CLR4
            }
            return def;
        }

        private static RegistryKey EnsureSubKey(RegistryKey root, string name)
        {
            RegistryKey key = root.OpenSubKey(name, true);
            if (key != null)
                return key;

            string parentName = Path.GetDirectoryName(name);
            if (string.IsNullOrEmpty(parentName))
                return root.CreateSubKey(name);

            using (RegistryKey parentKey = EnsureSubKey(root, parentName))
            {
                return parentKey.CreateSubKey(Path.GetFileName(name));
            }
        }
    }
}
