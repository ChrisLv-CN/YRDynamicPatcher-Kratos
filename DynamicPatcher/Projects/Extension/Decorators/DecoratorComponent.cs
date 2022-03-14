using Extension.Components;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Decorators
{
    [Serializable]
    public class DecoratorComponent : Component, IDecorative, IDecorative<PairDecorator>
    {
        public DecoratorComponent() : base(true)
        {

        }

        public TDecorator CreateDecorator<TDecorator>(DecoratorId id, string description, params object[] parameters) where TDecorator : Decorator
        {
            TDecorator decorator = decoratorMap.CreateDecorator<TDecorator>(id, description, parameters);
            decorator.Decorative = this;
            return decorator;
        }
        public TDecorator CreateDecorator<TDecorator>(string description, params object[] parameters) where TDecorator : Decorator
        {
            DecoratorId id = new DecoratorId(GetRandomID());
            return this.CreateDecorator<TDecorator>(id, description, parameters);
        }

        public Decorator Get(DecoratorId id) => decoratorMap.Get(id);

        public void Remove(DecoratorId id) => decoratorMap.Remove(id);

        public void Remove(Decorator decorator) => decoratorMap.Remove(decorator);

        IEnumerable<PairDecorator> IDecorative<PairDecorator>.GetDecorators() => GetPairDecorators();
        IEnumerable<PairDecorator> GetPairDecorators() => decoratorMap.GetPairDecorators();

        public TDecorator Get<TDecorator>(DecoratorId id) where TDecorator : Decorator
        {
            return (TDecorator)this.Get(id);
        }

        public object GetValue(DecoratorId id)
        {
            return this.Get<PairDecorator>(id).Value;
        }
        public void SetValue(DecoratorId id, object value)
        {
            this.Get<PairDecorator>(id).Value = value;
        }
        public TValue GetValue<TValue>(DecoratorId id)
        {
            return (TValue)this.Get<PairDecorator>(id).Value;
        }
        public TValue GetValue<TKey, TValue>(DecoratorId id)
        {
            return this.Get<PairDecorator<TKey, TValue>>(id).Value;
        }
        public void SetValue<TKey, TValue>(DecoratorId id, TValue value)
        {
            this.Get<PairDecorator<TKey, TValue>>(id).Value = value;
        }

        public PairDecorator GetPairDecorator(object key)
        {
            foreach (var decorator in this.GetPairDecorators())
            {
                if (key.Equals(decorator.Key))
                {
                    return decorator;
                }
            }

            return null;
        }
        public PairDecorator<TKey, TValue> GetPairDecorator<TKey, TValue>(TKey key)
        {
            foreach (var decorator in this.GetPairDecorators())
            {
                if (key.Equals(decorator.Key))
                {
                    return decorator as PairDecorator<TKey, TValue>;
                }
            }

            return null;
        }

        public object GetValue(object key)
        {
            var decorator = this.GetPairDecorator(key);
            return decorator?.Value;
        }
        public void SetValue(object key, object value)
        {
            var decorator = this.GetPairDecorator(key);
            if (decorator != null)
            {
                decorator.Value = value;
            }
        }

        public TValue GetValue<TValue>(object key)
        {
            return (TValue)this.GetValue(key);
        }

        public TValue GetValue<TKey, TValue>(TKey key)
        {
            var decorator = this.GetPairDecorator<TKey, TValue>(key);
            return decorator != null ? decorator.Value : default;
        }
        public void SetValue<TKey, TValue>(TKey key, TValue value)
        {
            var decorator = this.GetPairDecorator<TKey, TValue>(key);
            if (decorator != null)
            {
                decorator.Value = value;
            }
        }

        private DecoratorId GetRandomID()
        {
            DecoratorId id;
            do
            {
                id = new DecoratorId(MathEx.Random.Next());
                if (!decoratorMap.ContainsKey(id))
                    break;
            } while (true);
            return id;
        }


        private DecoratorMap decoratorMap = new DecoratorMap();
    }
}
