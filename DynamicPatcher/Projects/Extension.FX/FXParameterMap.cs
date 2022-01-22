using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX
{
    public class FXParameterMap : Dictionary<string, IFXParameter>, ICloneable
    {
        public FXParameterMap() : base()
        {
        }

        public FXParameterMap(IDictionary<string, IFXParameter> dictionary) : base(dictionary)
        {
        }

        protected FXParameterMap(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public FXParameterMap Clone()
        {
            var map = new FXParameterMap();

            foreach (var pair in this)
            {
                map.Add(pair.Key, (IFXParameter)pair.Value.Clone());
            }

            return map;
        }

        public FXParameter<TValue> GetOrDefault<TValue>(string name, TValue def = default)
        {
            if (TryGetValue(name, out IFXParameter parameter) == false)
            {
                parameter = new FXParameter<TValue>(name) { Value = def };
                this[name] = parameter;
            }
            return (FXParameter<TValue>)parameter;
        }

        public TValue GetValueOrDefault<TValue>(string name, TValue def = default)
        {
            return GetOrDefault<TValue>(name, def).Value;
        }

        public FXParameter<TValue> GetOrDefault<TValue>(string name, Func<TValue> def)
        {
            if (TryGetValue(name, out IFXParameter parameter) == false)
            {
                parameter = new FXParameter<TValue>(name) { Value = def() };
                this[name] = parameter;
            }
            return (FXParameter<TValue>)parameter;
        }

        public TValue GetValueOrDefault<TValue>(string name, Func<TValue> def)
        {
            return GetOrDefault<TValue>(name, def).Value;
        }

        public void SetValue<T>(string name, T value)
        {
            this[name].Value = value;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
