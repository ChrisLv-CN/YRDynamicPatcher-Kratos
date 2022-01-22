using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Decorators
{
    public enum DecoratorSearchTip
    {
        None, Pair, Event
    }
    
    [Serializable]
    public class DecoratorId
    {
        public DecoratorId(int id, DecoratorSearchTip searchTip = DecoratorSearchTip.None)
        {
            this.id = id;
            SearchTip = searchTip;
        }

        public DecoratorId(DecoratorId decoratorId)
        {
            id = decoratorId.id;
            SearchTip = decoratorId.SearchTip;
        }

        public static bool operator ==(DecoratorId left, DecoratorId right) => left.id == right.id;
        public static bool operator !=(DecoratorId left, DecoratorId right) => !(left == right);
        public override bool Equals(object obj) => this == (DecoratorId)obj;
        public override int GetHashCode() => id.GetHashCode();
        public override string ToString() => id.ToString();

        int id;
        public DecoratorSearchTip SearchTip { get; set; }
    }
}
