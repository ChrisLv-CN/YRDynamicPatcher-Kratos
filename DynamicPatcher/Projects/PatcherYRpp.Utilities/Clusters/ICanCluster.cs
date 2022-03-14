using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp.Utilities.Clusters
{
    public interface ICanCluster<T>
    {
        ICluster<T> Cluster { get; set; }
        CoordStruct Point { get; }
        Pointer<HouseClass> Owner { get; }

        void Update();
    }
}
