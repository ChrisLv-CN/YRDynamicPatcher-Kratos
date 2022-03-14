using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp.Utilities.Clusters
{
    [Serializable]
    public abstract class ClusterDiscoverer<T> where T : ICanCluster<T>
    {
        public abstract int ClusterRange { get; }
        public abstract int ClusterCapacity { get; }
        public abstract int ClusterStartNum { get; }

        public abstract List<T> ObjectList { get; }

        public abstract ICluster<T> CreateCluster(IEnumerable<T> objects);

        protected virtual void UpdateClusters()
        {
            List<T> list = ObjectList;

            foreach (T obj in list)
            {
                obj.Update();

                ICluster<T> cluster = obj.Cluster;
                if (cluster != null && cluster.Leader.Equals(obj))
                {
                    cluster?.Update();
                }
            }
        }

        protected virtual void CleanClusters()
        {
            List<T> list = ObjectList;
            foreach (T obj in list)
            {
                ICluster<T> cluster = obj.Cluster;

                if (cluster != null)
                {
                    if (cluster.IsOutOfRange(obj, ClusterRange))
                    {
                        cluster.Remove(obj);
                    }

                    // clear small cluster
                    if (cluster.Objects.Count < ClusterStartNum)
                    {
                        cluster.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// discover cluster by leader method:
        /// 1. pick first object as leader
        /// 2. group nearest objects in cluster or throw all near points
        /// 3. loop until no points
        /// </summary>
        protected virtual void DiscoverClusters()
        {
            List<T> list = ObjectList;
            List<CoordStruct> points = list.Select(obj => obj.Point).ToList();
            // use indexes
            List<int> restPoints = Enumerable.Range(0, list.Count).ToList();

            while (restPoints.Count > 0)
            {
                // pick first point as leader
                int leaderIdx = restPoints[0];
                Pointer<HouseClass> leaderOwner = list[leaderIdx].Owner;
                // find points near leader
                List<int> nearPoints = restPoints.Where(
                    idx => points[idx].DistanceFrom(points[leaderIdx]) <= ClusterRange
                    && leaderOwner.Ref.IsAlliedWith(list[idx].Owner)).ToList();

                // sort and pick nearest points
                nearPoints = nearPoints.OrderBy(idx => points[idx].DistanceFrom(points[leaderIdx])).ToList();

                if (nearPoints.Count >= ClusterStartNum)
                {
                    if (nearPoints.Count > ClusterCapacity)
                    {
                        // too large, take range [0, ClusterCapacity)
                        nearPoints = nearPoints.GetRange(0, ClusterCapacity);
                    }

                    T[] objects = nearPoints.Select(p => list[p]).ToArray();
                    ICluster<T> cluster = CreateCluster(objects);
                }
                else
                {
                    // clean cluster for small group
                    foreach (var idx in nearPoints)
                    {
                        list[idx].Cluster = null;
                    }
                }

                // restPoints = restPoints - nearPoints
                restPoints = restPoints.Except(nearPoints).ToList();
            }
        }

        public virtual void Update()
        {
            UpdateClusters();
            CleanClusters();
            DiscoverClusters();
        }
    }
}
