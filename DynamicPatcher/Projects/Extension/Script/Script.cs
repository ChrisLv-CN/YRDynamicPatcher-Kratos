using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Script
{
    [Serializable]
    public class Script
    {
        public Script(string name)
        {
            Name = name;
        }

        public string Name { get; protected set; }
        public Type ScriptableType { get; internal set; }
    }
}
