using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Decorators
{
    public interface IDecorative
    {
        public TDecorator CreateDecorator<TDecorator>(DecoratorId id, string description, params object[] parameters) where TDecorator : Decorator;
        public Decorator Get(DecoratorId id);
        public void Remove(DecoratorId id);
        public void Remove(Decorator decorator);
    }

    public interface IDecorative<TDecorator> : IDecorative where TDecorator : Decorator
    {
        public IEnumerable<TDecorator> GetDecorators();
    }
}
