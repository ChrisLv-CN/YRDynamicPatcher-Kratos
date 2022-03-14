using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp.Utilities
{
    public struct ObjectBlockID
    {
        public ObjectBlockID(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(ObjectBlockID id1, ObjectBlockID id2) => id1.X == id2.X && id1.Y == id2.Y;
        public static bool operator !=(ObjectBlockID id1, ObjectBlockID id2) => !(id1 == id2);
        public override int GetHashCode() => X.GetHashCode() + Y.GetHashCode();
        public override bool Equals(object obj) => this == (ObjectBlockID)obj;

        public int X;
        public int Y;
    }

    public class ObjectBlockContainer
    {
        /// <summary>
        /// ObjectBlockContainer constructor.
        /// </summary>
        /// <param name="blockLength">the square side length(in cells).</param>
        /// <param name="mapWidth">the whole map width in cells</param>
        /// <param name="mapHeight">the whole map height in cells</param>
        public ObjectBlockContainer(IntPtr objectArrayPointer, int blockLength, int mapWidth, int mapHeight)
        {
            if (blockLength % 2 == 0)
            {
                throw new ArgumentException("block length can not be even.");
            }

            ObjectArrayPointer = objectArrayPointer;
            BlockLength = blockLength;

            Blocks = new ObjectBlock[mapWidth * 2 / blockLength, mapHeight * 2 / blockLength];
            OuterBlock = new ObjectBlock(this, new ObjectBlockID(int.MaxValue, int.MaxValue));
            AllocateBlocks();
        }

        public IntPtr ObjectArrayPointer { get; }
        public ref DynamicVectorClass<Pointer<ObjectClass>> ObjectArray { get => ref DynamicVectorClass<Pointer<ObjectClass>>.GetDynamicVector(ObjectArrayPointer); }

        public ObjectBlock[,] Blocks { get; }
        public ObjectBlock OuterBlock { get; }
        public int BlockLength { get; }
        public int BlockRange => (BlockLength - 1) / 2;

        public int Version { get; private set; }

        private void AllocateBlocks()
        {
            int xLength = Blocks.GetLength(0);
            int yLength = Blocks.GetLength(1);

            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    var block = new ObjectBlock(this, new ObjectBlockID(x - xLength / 2, y - yLength / 2));
                    Blocks[x, y] = block;
                }
            }
        }

        public ObjectBlockID GetIDBy(CoordStruct coord)
        {
            CellStruct location = CellClass.Coord2Cell(coord);
            return GetIDBy(location);
        }
        public ObjectBlockID GetIDBy(CellStruct location)
        {
            return new ObjectBlockID(location.X / BlockLength, location.Y / BlockLength);
        }

        public ObjectBlock GetBlock(CoordStruct coord)
        {
            CellStruct location = CellClass.Coord2Cell(coord);
            return GetBlock(location);
        }
        public ObjectBlock GetBlock(CellStruct location)
        {
            ObjectBlockID id = GetIDBy(location);
            return GetBlock(id);
        }
        public ObjectBlock GetBlock(ObjectBlockID id)
        {
            if (HasChange())
            {
                RefreshBlocks();
            }

            return GetBlockForce(id);
        }
        private ObjectBlock GetBlockForce(ObjectBlockID id)
        {
            int xLength = Blocks.GetLength(0);
            int yLength = Blocks.GetLength(1);

            // notice that we may have negative id
            int x = id.X + xLength / 2;
            int y = id.Y + yLength / 2;

            return x < xLength && y < yLength ? Blocks[x, y] : OuterBlock;
        }

        public void ClearObjects()
        {
            foreach (var block in Blocks)
            {
                block.Clear();
            }
        }


        /// <summary>
        /// clear all blocks and insert objects to blocks.
        /// </summary>
        public void RefreshBlocks()
        {
            ClearObjects();

            ref var objects = ref ObjectArray;

            foreach (Pointer<ObjectClass> pObject in objects)
            {
                CellStruct objectLoc = CellClass.Coord2Cell(pObject.Ref.Base.GetCoords());
                ObjectBlockID id = GetIDBy(objectLoc);
                ObjectBlock block = GetBlockForce(id);
                block.AddObject(pObject);
            }

            Version = Game.CurrentFrame;
        }

        public bool HasChange()
        {   
            // we assume that nothing move in one frame
            // TOCHECK: whether one frame don't change the object array
            if (Version == Game.CurrentFrame)
            {
                return false;
            }

            return true;
        }

        public List<ObjectBlock> GetCoveredBlocks(CoordStruct coord, int range)
        {
            CellStruct location = CellClass.Coord2Cell(coord);
            return GetCoveredBlocks(location, range);
        }
        public List<ObjectBlock> GetCoveredBlocks(CellStruct location, int range)
        {
            ObjectBlock centerBlock = GetBlock(location);
            ObjectBlockID centerId = GetIDBy(location);
            var list = new List<ObjectBlock>() { centerBlock };

            int rangeInCells = range / Game.CellSize;
            int rangeInBlocks = rangeInCells / BlockLength;
            int tryRangeInBlocks = rangeInBlocks + 1;

            for (int x = -tryRangeInBlocks; x <= tryRangeInBlocks; x++)
            {
                for (int y = -tryRangeInBlocks; y <= tryRangeInBlocks; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    // check try block distance
                    if (Math.Abs(x) > rangeInBlocks || Math.Abs(y) > rangeInBlocks)
                    {
                        var offset = new CellStruct(x * rangeInCells / tryRangeInBlocks, y * rangeInCells / tryRangeInBlocks);
                        ObjectBlockID id = GetIDBy(location + offset);
                        if (Math.Abs(id.X - centerId.X) > rangeInBlocks || Math.Abs(id.Y - centerId.Y) > rangeInBlocks)
                        {
                            ObjectBlock block = GetBlock(id);
                            if (!list.Contains(block))
                            {
                                list.Add(block);
                            }
                        }
                    }
                    else
                    {
                        ObjectBlock block = GetBlock(new ObjectBlockID(centerId.X + x, centerId.Y + y));
                        list.Add(block);
                    }
                }
            }


            return list;
        }

        public void PointerExpired(Pointer<ObjectClass> pObject)
        {
            CoordStruct crd = pObject.Ref.Base.GetCoords();
            ObjectBlock block = GetBlock(crd);
            block.RemoveObject(pObject);
        }

        public void ObjectMoved(Pointer<ObjectClass> pObject, CoordStruct oldCoord)
        {
            ObjectBlock oldBlock = GetBlock(oldCoord);
            oldBlock.RemoveObject(pObject);

            CoordStruct newCoord = pObject.Ref.Base.GetCoords();
            ObjectBlock newBlock = GetBlock(newCoord);
            newBlock.AddObject(pObject);
        }
    }

    public class ObjectBlock
    {
        public ObjectBlock(ObjectBlockContainer container, ObjectBlockID id)
        {
            int blockLength = container.BlockLength;
            int blockRange = container.BlockRange;

            Container = container;
            ID = id;
            Center = new CellStruct(id.X * blockLength + blockRange, id.Y * blockLength + blockRange);
            objects = new List<Pointer<ObjectClass>>();
        }

        public ObjectBlockContainer Container { get; }
        public ObjectBlockID ID { get; }
        public CellStruct Center { get; }
        public List<Pointer<ObjectClass>> Objects => objects;
        public bool IsObjectInBlock(Pointer<ObjectClass> pObject)
        {
            CellStruct objectLoc = CellClass.Coord2Cell(pObject.Ref.Base.GetCoords());
            int blockRange = Container.BlockRange;

            return Math.Abs(objectLoc.X - Center.X) <= blockRange
                && Math.Abs(objectLoc.Y - Center.Y) <= blockRange;
        }

        public void AddObject(Pointer<ObjectClass> pObject)
        {
            objects.Add(pObject);
        }

        public void RemoveObject(Pointer<ObjectClass> pObject)
        {
            objects.Remove(pObject);
        }

        public void Clear()
        {
            objects.Clear();
        }

        public List<ObjectBlock> GetNearBlocks()
        {
            var list = new List<ObjectBlock>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    var block = Container.GetBlock(new ObjectBlockID(ID.X + x, ID.Y + y));
                    list.Add(block);
                }
            }

            return list;
        }

        // not good method, we don't want it before every query
        public void FixErrors()
        {
            bool OutOfBlock(Pointer<ObjectClass> pObject)
            {
                var crd = pObject.Ref.Base.GetCoords();
                var block = Container.GetBlock(crd);

                if (block == this)
                {
                    return false;
                }

                block.AddObject(pObject);
                return true;
            }

            objects.RemoveAll(o => o.Ref.Base.Vfptr == IntPtr.Zero || OutOfBlock(o));
        }

        private List<Pointer<ObjectClass>> objects;
    }
}
