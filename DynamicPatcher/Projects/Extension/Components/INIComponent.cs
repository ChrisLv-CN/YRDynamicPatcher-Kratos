using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Components
{
    /// <summary>
    /// Component used to get ini value
    /// </summary>
    [Serializable]
    public class INIComponent : Component
    {
        static List<INIBuffer> buffers = new List<INIBuffer>();
        class INIBuffer
        {
            public INIBuffer(string name, string section)
            {
                Name = name;
                Section = section;
                Pairs = new Dictionary<string, object>();
            }

            public string Name;
            public string Section;
            public Dictionary<string, object> Pairs;
        }

        INIComponent() : base()
        {

        }

        public INIComponent(string name, string section)
        {
            _name = name;
            _section = section;
            FindBuffer();
        }

        public string ININame
        {
            get => _name;
            set
            {
                _name = value;
                FindBuffer();
            }
        }

        public string INISection
        {
            get => _section;
            set
            {
                _section = value;
                FindBuffer();
            }
        }

        /// <summary>
        /// get key value from ini
        /// </summary>
        /// <remarks>you can only get basic type value</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public T Get<T>(string key, T def = default)
        {
            if (_pairs == null)
            {
                return def;
            }

            if (_pairs.TryGetValue(key, out object val))
            {
                // if the value is not parsed, parse it
                if (typeof(T) != val.GetType())
                {
                    T tmp = def;
                    if(Parser<T>.Parse((string)val, ref tmp) != 0)
                    {
                        _pairs[key] = tmp;
                    }
                    return tmp;
                }
                return (T)val;
            }

            return def;
        }

        /// <summary>
        /// get key values from ini
        /// </summary>
        /// <remarks>you can only get basic type value</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public T[] GetList<T>(string key, T[] def = default)
        {
            if (_pairs == null)
            {
                return def;
            }

            if (_pairs.TryGetValue(key, out object val))
            {
                // if the value is not parsed, parse it
                if (typeof(T[]) != val.GetType())
                {
                    string buffer = (string)val;
                    T[] tmp = new T[buffer.Count(c => c == ',') + 1];
                    if (Parser<T>.Parse((string)val, ref tmp) != 0)
                    {
                        _pairs[key] = tmp;
                    }
                    else
                    {
                        tmp = def;
                    }
                    return tmp;
                }
                return (T[])val;
            }

            return def;
        }

        private void FindBuffer()
        {
            INIBuffer buffer = buffers.Find(b => b.Name == _name && b.Section == _section);

            if (buffer == null)
            {
                buffer = new INIBuffer(_name, _section);

                // read ini section
                var pINI = YRMemory.Create<CCINIClass>();
                var pFile = YRMemory.Create<CCFileClass>(_name);
                INIReader reader = new INIReader(pINI);
                pINI.Ref.ReadCCFile(pFile);
                YRMemory.Delete(pFile);

                // read all pairs as <string, string> first
                int keyCount = pINI.Ref.GetKeyCount(_section);
                for (int i = 0; i < keyCount; i++)
                {
                    string key = pINI.Ref.GetKeyName(_section, i);
                    string val = null;
                    reader.ReadNormal(_section, key, ref val);
                    buffer.Pairs[key] = val;
                }

                YRMemory.Delete(pINI);

                buffers.Add(buffer);
            }

            _pairs = buffer.Pairs;
        }

        public static void ClearBuffer()
        {
            buffers.Clear();
        }

        private string _name;
        private string _section;
        private Dictionary<string, object> _pairs;
    }
}
