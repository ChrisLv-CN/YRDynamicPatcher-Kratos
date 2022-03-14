using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Decorators
{
    [Serializable]
    public abstract class Decorator
    {
        public string Description { get; set; }
        public DecoratorId ID { get; set; }
        public IDecorative Decorative { get; internal set; }
    }

    [Serializable]
    public abstract class PairDecorator : Decorator
    {
        public object Key { get => GetKey(); set => SetKey(value); }
        public object Value { get => GetValue(); set => SetValue(value); }
        public PairDecorator(object key, object val)
        {
            Key = key;
            Value = val;
        }

        protected abstract object GetKey();
        protected abstract object SetKey(object key);
        protected abstract object GetValue();
        protected abstract object SetValue(object value);
    }

    [Serializable]
    public class PairDecorator<TKey, TValue> : PairDecorator
    {
        public new TKey Key { get => pair.Item1; set => pair.Item1 = value; }
        public new TValue Value { get => pair.Item2; set => pair.Item2 = value; }
        public PairDecorator(TKey key, TValue val) : base(key, val)
        {
        }

        protected override object GetKey() => pair.Item1;
        protected override object SetKey(object key) => pair.Item1 = (TKey)key;
        protected override object GetValue() => pair.Item2;
        protected override object SetValue(object value) => pair.Item2 = (TValue)value;

        private ValueTuple<TKey, TValue> pair;
    }

}
