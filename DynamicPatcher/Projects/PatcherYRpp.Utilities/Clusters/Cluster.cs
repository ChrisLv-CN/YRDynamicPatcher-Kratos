using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp.Utilities.Clusters
{
    [Serializable]
    public abstract class Cluster<T> : ICluster<T> where T : ICanCluster<T>
    {
        public virtual T Leader
        {
            get => Objects.FirstOrDefault();
            set
            {
                if (Objects.Remove(value))
                {
                    Objects.Insert(0, value);
                }
            }
        }
        public virtual List<T> Objects { get; } = new List<T>();
        public virtual CoordStruct Mean
        {
            get
            {
                var points = Points;
                return points.Aggregate((sum, next) => sum + next) * (1.0 / points.Count);
            }
        }

        public virtual List<CoordStruct> Points => Objects.Select(o => o.Point).ToList();

        public virtual void Add(T obj)
        {
            Objects.Add(obj);
            obj.Cluster = this;
        }
        public virtual void Add(IEnumerable<T> list)
        {
            Objects.AddRange(list);
            foreach (T obj in list)
            {
                obj.Cluster = this;
            }
        }

        public virtual void Remove(T obj)
        {
            Objects.Remove(obj);
            obj.Cluster = null;
        }

        public virtual void Clear()
        {
            foreach (T obj in Objects)
            {
                obj.Cluster = null;
            }

            Objects.Clear();
        }

        public virtual bool IsOutOfRange(T obj, double range)
        {
            return Objects.Count > 0 && obj.Point.DistanceFrom(Mean) > range;
        }

        public abstract void Update();
    }
}
