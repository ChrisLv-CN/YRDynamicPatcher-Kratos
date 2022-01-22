using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Parameters
{
    public struct FXLinkParameterContext
    {
        public FXLinkParameterContext(FXSystem system, FXEmitter emitter, FXScript script)
        {
            System = system;
            Emitter = emitter;
            Script = script;
        }

        public FXSystem System;
        public FXEmitter Emitter;
        public FXScript Script;
    }

    public class FXLinkParameter<T> : FXParameter<T>
    {
        public FXLinkParameter(string path) : this("", path)
        {
        }
        public FXLinkParameter(string name, string path) : base(name)
        {
            Path = path;

            var linkList = path.Split('.');

            if(linkList.Length != 2)
            {
                throw new ArgumentException("FXLinkParameter's path not valid.");
            }

            SelectedContext = linkList[0];
            SelectedParameter = linkList[1];
        }

        public string Path { get; }
        public string SelectedContext { get; }
        public string SelectedParameter { get; }

        public FXLinkParameterContext Context { get; set; }

        public void SetContext(FXSystem system = null, FXEmitter emitter = null, FXScript script = null)
        {
            Context = new FXLinkParameterContext(
                system ?? Context.System,
                emitter ?? Context.Emitter,
                script ?? Context.Script
                );
        }

        public object GetContext()
        {
            switch (SelectedContext)
            {
                case "System":
                    return Context.System;
                case "Emitter":
                    return Context.Emitter;
                case "Script":
                    return Context.Script;
                case "Particle":
                    throw new NotSupportedException("FXLinkParameter does not support Particle link.");
                default:
                    throw new InvalidOperationException($"FXLinkParameter can not link {SelectedContext}.");
            }
        }

        public MemberInfo GetParameterInfo()
        {
            Type contextType = GetContext().GetType();
            MemberInfo info = contextType.GetMember(SelectedParameter)[0];
            return info;
        }

        private bool IsFXParameter(Type type)
        {
            return typeof(FXParameter<T>).IsAssignableFrom(type);
        }

        public T GetValue()
        {
            MemberInfo info = GetParameterInfo();
            var context = GetContext();

            Type type = null;
            object value = null;

            if(info is PropertyInfo property)
            {
                type = property.PropertyType;
                value = property.GetValue(context);
            }
            else if(info is FieldInfo field)
            {
                type = field.FieldType;
                value = field.GetValue(context);
            }

            if (IsFXParameter(type))
            {
                var parameter = value as FXParameter<T>;
                return parameter.Value;
            }

            return (T)value;
        }

        public void SetValue(T value)
        {
            MemberInfo info = GetParameterInfo();
            var context = GetContext();

            Type type = null;

            if (info is PropertyInfo property)
            {
                type = property.PropertyType;

                if (IsFXParameter(type))
                {
                    var parameter = value as FXParameter<T>;
                    parameter.Value = value;
                }
                else
                {
                    property.SetValue(context, value);
                }
            }
            else if (info is FieldInfo field)
            {
                type = field.FieldType;
                if (IsFXParameter(type))
                {
                    var parameter = value as FXParameter<T>;
                    parameter.Value = value;
                }
                else
                {
                    field.SetValue(context, value);
                }
            }
        }

        public override T Value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public override FXParameter<T> Clone()
        {
            return new FXLinkParameter<T>(Name, Path);
        }
    }
}

