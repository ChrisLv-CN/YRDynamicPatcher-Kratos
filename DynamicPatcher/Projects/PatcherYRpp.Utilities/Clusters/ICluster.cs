using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp.Utilities.Clusters
{
    public interface ICluster<T>
    {
        T Leader { get; set; }
        List<T> Objects { get; }
        CoordStruct Mean { get; }
        List<CoordStruct> Points { get; }

        void Add(T obj);
        void Add(IEnumerable<T> list);
        void Remove(T obj);
        void Clear();
        bool IsOutOfRange(T obj, double range);
        void Update();
    }
}
