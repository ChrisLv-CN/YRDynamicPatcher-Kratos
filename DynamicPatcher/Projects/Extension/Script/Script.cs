using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Script
{
    public enum ScriptEventType
    {
        // Object
        OnUpdate, OnRemove, OnPut, OnReceiveDamage,
        // Techno
        OnFire
    }

    public class ScriptEvent
    {
        public ScriptEvent(MethodInfo methodInfo)
        {
            method = methodInfo;
        }

        public void ResetMethod(MethodInfo methodInfo)
        {
            method = methodInfo;
        }

        public object Invoke(object obj, params object[] parameters)
        {
            return method.Invoke(obj, parameters);
        }

        MethodInfo method;
    }

    public abstract class Script
    {
        public string Name { get; protected set; }
        public Type ScriptableType { get; protected set; }
        public IDictionary<string, ScriptEvent> Events { get; protected set; }

        public virtual ScriptEvent GetEvent(string eventName) => Events[eventName];
        public ScriptEvent this[string eventName] => GetEvent(eventName);
        public ScriptEvent this[ScriptEventType eventType] => GetEvent(eventType.ToString());

        public virtual IEnumerable<string> EventNames { get; }
        public void SetEvents(Type type)
        {
            ScriptableType = type;
            foreach (string eventName in EventNames)
            {
                MethodInfo method = ScriptableType.GetMethod(eventName);
                SetEvent(eventName, method);
            }
        }

        protected virtual void SetEvent(string eventName, MethodInfo method)
        {
            if (Events.TryGetValue(eventName, out ScriptEvent scriptEvent))
            {
                scriptEvent.ResetMethod(method);
            }
            else
            {
                Events.Add(eventName, new ScriptEvent(method));
            }
        }
    }

    public class TechnoScript : Script
    {
        public TechnoScript(string name)
        {
            Name = name;
            Events = new Dictionary<string, ScriptEvent>();
        }

        static readonly List<string> eventNames = new ScriptEventType[] {
            ScriptEventType.OnUpdate, ScriptEventType.OnRemove, ScriptEventType.OnPut,
            ScriptEventType.OnReceiveDamage,
            ScriptEventType.OnFire
        }.Select(type => type.ToString()).ToList();
        public override IEnumerable<string> EventNames => eventNames;
    }

    public class BulletScript : Script
    {
        public BulletScript(string name)
        {
            Name = name;
            Events = new Dictionary<string, ScriptEvent>();
        }

        static readonly List<string> eventNames = new ScriptEventType[] {
            ScriptEventType.OnUpdate, ScriptEventType.OnRemove, ScriptEventType.OnPut,
        }.Select(type => type.ToString()).ToList();
        public override IEnumerable<string> EventNames => eventNames;
    }
}
