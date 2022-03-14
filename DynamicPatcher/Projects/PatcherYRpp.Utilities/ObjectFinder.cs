using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp.Utilities
{

    /// <summary>
    /// A class to find objects, technos, buildings and so on with high performence method.
    /// </summary>
    public static class ObjectFinder
    {
        public enum FindMethod
        {
            BruteForce,
            ObjectBlock,
            Degenerate
        }

        public static ObjectBlockContainer ObjectContainer { get; } = new ObjectBlockContainer(ObjectClass.ArrayPointer, 10 + 1, 500, 500);
        public static ObjectBlockContainer TechnoContainer { get; } = new ObjectBlockContainer(TechnoClass.ArrayPointer, 10 + 1, 500, 500);


        private static List<Pointer<ObjectClass>> BruteFindObjectsNear(ref DynamicVectorClass<Pointer<ObjectClass>> objects, CoordStruct location, int range)
        {
            var list = new List<Pointer<ObjectClass>>();

            foreach (Pointer<ObjectClass> pObject in objects)
            {
                if (pObject.Ref.Base.GetCoords().DistanceFrom(location) <= range)
                {
                    list.Add(pObject);
                }
            }

            return list;
        }

        private static List<Pointer<ObjectClass>> BlockFindObjectsNear(ObjectBlockContainer container, CoordStruct location, int range)
        {
            var blocks = container.GetCoveredBlocks(location, range);
            var list = new List<Pointer<ObjectClass>>();

            foreach (var block in blocks)
            {
                foreach (var pObject in block.Objects)
                {
                    if (pObject.Ref.Base.GetCoords().DistanceFrom(location) <= range)
                    {
                        list.Add(pObject);
                    }
                }
            }

            return list;
        }

        public static List<Pointer<ObjectClass>> BruteFindObjectsNear(CoordStruct location, int range)
        {
            return BruteFindObjectsNear(ref ObjectContainer.ObjectArray, location, range);
        }

        public static List<Pointer<ObjectClass>> BlockFindObjectsNear(CoordStruct location, int range)
        {
            return BlockFindObjectsNear(ObjectContainer, location, range);
        }


        /// <summary>
        /// degenerate find objects until meet demand.
        /// </summary>
        /// <param name="location">the center to find object.</param>
        /// <param name="range">distance from location.</param>
        /// <param name="needCount">number of objects required.</param>
        /// <returns>the list of objects in range.</returns>
        public static List<Pointer<ObjectClass>> DegenerateFindObjectsNear(CoordStruct location, int range, int needCount)
        {
            var list = new List<Pointer<ObjectClass>>();
            int count = 0;

            bool Enough() => count >= needCount;

            CellStruct center = CellClass.Coord2Cell(location);
            CellSpreadEnumerator enumerator = new CellSpreadEnumerator((uint)(range / Game.CellSize));
            foreach (CellStruct offset in enumerator)
            {
                if (MapClass.Instance.TryGetCellAt(center + offset, out Pointer<CellClass> pCell))
                {

                }

                if (Enough())
                    return list;
            }

            return list;
        }


        /// <summary>
        /// find objects by method automatively choiced.
        /// </summary>
        /// <param name="location">the center to find object.</param>
        /// <param name="range">distance from location.</param>
        /// <returns>the list of objects in range.</returns>
        public static List<Pointer<ObjectClass>> FindObjectsNear(CoordStruct location, int range)
        {
            if (range <= Game.CellSize * 64 /*&& container.ObjectArray.Count >= 500*/)
            {
                return BlockFindObjectsNear(location, range);
            }
            else
            {
                return BruteFindObjectsNear(location, range);
            }
        }

        public static List<Pointer<ObjectClass>> BruteFindTechnosNear(CoordStruct location, int range)
        {
            return BruteFindObjectsNear(ref TechnoContainer.ObjectArray, location, range);
        }

        public static List<Pointer<ObjectClass>> BlockFindTechnosNear(CoordStruct location, int range)
        {
            return BlockFindObjectsNear(TechnoContainer, location, range);
        }
        /// <summary>
        /// find technos by method automatively choiced.
        /// </summary>
        /// <param name="location">the center to find object.</param>
        /// <param name="range">distance from location.</param>
        /// <returns>the list of technos in range.</returns>
        public static List<Pointer<ObjectClass>> FindTechnosNear(CoordStruct location, int range)
        {
            if (range <= Game.CellSize * 64 /*&& container.TechnoArray.Count >= 500*/)
            {
                return BlockFindTechnosNear(location, range);
            }
            else
            {
                return BruteFindTechnosNear(location, range);
            }
        }
    }
}
