using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Decorators
{
    [Serializable]
    class DecoratorMap : Dictionary<DecoratorId, Decorator>
    {
        public DecoratorMap() : base()
        {
            pairs = new EnumerableBuffer<PairDecorator>(this);
        }

        protected DecoratorMap(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand,
            Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public TDecorator CreateDecorator<TDecorator>(DecoratorId id, string description, params object[] parameters) where TDecorator : Decorator
        {
            var decorator = Activator.CreateInstance(typeof(TDecorator), parameters) as TDecorator;
            decorator.Description = description;
            decorator.ID = id;

            this.Add(decorator);

            return decorator;
        }

        public Decorator Get(DecoratorId id)
        {
            if (this.TryGetValue(id, out Decorator decorator))
            {
                return decorator;
            }
            return null;
        }

        public void Add(Decorator decorator)
        {
            base.Add(decorator.ID, decorator);
            NotifyChanged();
        }

        public void Remove(Decorator decorator)
        {
            base.Remove(decorator.ID);
            NotifyChanged();
        }

        private Action NotifyChanged;

        public IEnumerable<PairDecorator> GetPairDecorators() => pairs.Get();

        // convenient to remove when enumerating
        [Serializable]
        class EnumerableBuffer<TDecorator> where TDecorator : Decorator
        {
            DecoratorMap map;
            List<TDecorator> list;
            public bool hasChanged = true;

            public EnumerableBuffer(DecoratorMap map)
            {
                this.map = map;
                map.NotifyChanged += () => hasChanged = true;
            }

            public IEnumerable<TDecorator> Get()
            {
                if (hasChanged)
                {
                    list = (from d in map.Values where d is TDecorator select d as TDecorator).ToList();
                    hasChanged = false;
                }
                return list;
            }
        }

        EnumerableBuffer<PairDecorator> pairs;
    }
}
