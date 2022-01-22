using Extension.FX.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX
{
    public interface IFXParameter : ICloneable
    {
        public string Name { get; }
        public object Value { get; set; }
    }

    public class FXParameter<T> : IFXParameter
    {
        public FXParameter(string name)
        {
            Name = name;
        }
        public FXParameter(T value) : this("", value)
        {
        }
        public FXParameter(string name, T value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public virtual T Value { get; set; }

        object IFXParameter.Value { get => Value; set => Value = (T)value; }

        public virtual FXParameter<T> Clone()
        {
            return new FXParameter<T>(Name, Value);
        }

        public static implicit operator FXParameter<T>(T value) => new FXParameter<T>(value);
        public static implicit operator T(FXParameter<T> parameter) => parameter.Value;

        public override string ToString()
        {
            return GetType().FullName;
        }
        object ICloneable.Clone()
        {
            return Clone();
        }
    }

    public static class FXParameterHelper
    {
        public static FXParameter<T> Clone<T>(this FXParameter<T> parameter, FXSystem system)
        {
            var _new = parameter.Clone();

            if(_new is FXLinkParameter<T> link)
            {
                link.SetContext(system: system);
            }

            return _new;
        }
        public static FXParameter<T> Clone<T>(this FXParameter<T> parameter, FXEmitter emitter)
        {
            var _new = parameter.Clone();

            if (_new is FXLinkParameter<T> link)
            {
                link.SetContext(emitter.System, emitter);
            }

            return _new;
        }
        public static FXParameter<T> Clone<T>(this FXParameter<T> parameter, FXScript script)
        {
            var _new = parameter.Clone();

            if (_new is FXLinkParameter<T> link)
            {
                link.SetContext(script.System, script.Emitter, script);
            }

            return _new;
        }
    }
}
