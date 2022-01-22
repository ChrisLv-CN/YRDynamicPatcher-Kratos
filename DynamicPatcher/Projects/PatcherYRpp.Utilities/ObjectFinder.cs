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
            ObjectBlock
        }

        private static ObjectBlockContainer container = new ObjectBlockContainer(ObjectClass.ArrayPointer, 10 + 1, 500, 500);

        private static List<Pointer<ObjectClass>> BruteFindObjectsNear(CoordStruct location, int range)
        {
            ref var objects = ref ObjectClass.Array;
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


        private static List<Pointer<ObjectClass>> BlockFindObjectsNear(CoordStruct location, int range)
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


        /// <summary>
        /// find object by specified method.
        /// </summary>
        /// <param name="location">the center to find object.</param>
        /// <param name="range">distance from location.</param>
        /// <returns>the list of objects in range.</returns>
        public static List<Pointer<ObjectClass>> FindObjectsNear(CoordStruct location, int range, FindMethod method = FindMethod.ObjectBlock)
        {
            switch (method)
            {
                case FindMethod.BruteForce:
                    return BruteFindObjectsNear(location, range);
                case FindMethod.ObjectBlock:
                    return BlockFindObjectsNear(location, range);
                default:
                    return null;
            }
        }
    }
}
