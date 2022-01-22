using System.Threading;
using System.IO;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    [Serializable]
    public class GiftBox
    {
        public bool Enable;
        public GiftBoxData Data;
        public bool IsOpen;
        public int Delay;
        public List<string> Gifts = new List<string>();

        public GiftBox(GiftBoxData data)
        {
            this.Enable = data.Enable;
            this.Data = data;
            this.IsOpen = false;
            this.Delay = data.DelayMax == 0 ? data.Delay : ExHelper.Random.Next(data.DelayMin, data.DelayMax);
            if (data.Gifts.Count == 0)
            {
                this.Enable = false;
            }
        }

        public bool Open()
        {
            return Enable && (IsOpen ? false : CheckDelay());
        }

        private bool CheckDelay()
        {
            if (--this.Delay <= 0)
            {
                this.IsOpen = true;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            this.Delay = Data.DelayMax == 0 ? Data.Delay : ExHelper.Random.Next(Data.DelayMin, Data.DelayMax);
            this.IsOpen = false;
        }
    }

    [Serializable]
    public class GiftBoxData
    {
        public bool Enable;
        public List<string> Gifts;
        public List<int> Nums;
        public bool Remove;
        public bool Destroy;
        public int Delay;
        public int DelayMin;
        public int DelayMax;
        public int RandomRange;
        public bool EmptyCell;
        public bool RandomType;

        public GiftBoxData(List<string> gifts)
        {
            this.Enable = gifts.Count > 0;
            this.Gifts = gifts;
            this.Nums = new List<int>();
            this.Remove = true;
            this.Destroy = false;
            this.Delay = 0;
            this.DelayMin = 0;
            this.DelayMax = 0;
            this.RandomRange = 0;
            this.EmptyCell = false;
            this.RandomType = false;
        }

        public List<string> GetGiftList()
        {
            List<string> gifts = new List<string>();

            if (RandomType)
            {
                List<string> result = new List<string>();
                int nums = Nums.Count < 1 ? 1 : 0;
                foreach (int num in Nums)
                {
                    nums += num;
                }
                for (int i = 0; i < nums; i++)
                {
                    gifts.Add(Gifts[ExHelper.Random.Next(0, Gifts.Count)]);
                }
            }
            else
            {
                foreach (string id in Gifts)
                {
                    for (int i = 0; i < GetGiftNum(id); i++)
                    {
                        gifts.Add(id);
                    }
                }
            }
            return gifts;
        }

        private int GetGiftNum(string gift)
        {
            int index = Gifts.FindIndex((giftName) => { return giftName == gift; });
            return index < Nums.Count ? Nums[index] : 1;
        }
    }

    public partial class TechnoExt
    {

        private GiftBox giftBox;

        public unsafe void TechnoClass_Init_GiftBox()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (null != Type.GiftBoxData && Type.GiftBoxData.Enable && null == giftBox)
            {
                giftBox = new GiftBox(Type.GiftBoxData);
            }

        }

        public unsafe void TechnoClass_Update_GiftBox()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (null != giftBox && giftBox.Open())
            {
                Pointer<HouseClass> pHouse = pTechno.Ref.Owner;
                CoordStruct location = pTechno.Ref.Base.Base.GetCoords();
                if (giftBox.Data.RandomRange > 0)
                {
                    CellStruct cell = MapClass.Coord2Cell(location);
                    CellStruct[] cellOffset = new CellSpreadEnumerator((uint)giftBox.Data.RandomRange).ToArray();
                    int max = cellOffset.Count();
                    for (int i = 0; i < max; i++)
                    {
                        int index = ExHelper.Random.Next(max - 1);
                        CellStruct offset = cellOffset[index];
                        // Logger.Log("随机获取周围格子索引{0}, 共{1}格, 获取的格子偏移{2}, 单位当前坐标{3}, 第一个格子的坐标{4}, 尝试次数{5}, 当前偏移{6}", index, max, offset, location, MapClass.Cell2Coord(cell + cellOffset[0]), i, cellOffset[i]);
                        if (offset == default)
                        {
                            continue;
                        }
                        CoordStruct where = MapClass.Cell2Coord(cell + offset);
                        if (MapClass.Instance.TryGetCellAt(where, out Pointer<CellClass> pCell) && !pCell.IsNull)
                        {
                            if (giftBox.Data.EmptyCell && (!pCell.Ref.GetBuilding().IsNull || !pCell.Ref.GetUnit(false).IsNull || !pCell.Ref.GetInfantry(false).IsNull))
                            {
                                // Logger.Log("获取到的格子被占用, 建筑{0}, 步兵{1}, 载具{2}", !pCell.Ref.GetBuilding().IsNull, !pCell.Ref.GetUnit(false).IsNull, !pCell.Ref.GetInfantry(false).IsNull);
                                continue;
                            }
                            location = pCell.Ref.Base.GetCoords();
                            // Logger.Log("获取到的格子坐标{0}", location);
                            break;
                        }
                    }
                }
                CoordStruct destination = location;
                Pointer<AbstractClass> pFocus = default;
                if (pTechno.Ref.Base.Base.WhatAmI() != AbstractType.Building)
                {
                    Pointer<AbstractClass> dest = pTechno.Convert<FootClass>().Ref.Destination;
                    if (!dest.IsNull)
                    {
                        destination = dest.Ref.GetCoords();
                    }
                    pFocus = pTechno.Ref.Focus;
                }

                foreach (string id in giftBox.Data.GetGiftList())
                {
                    Pointer<TechnoClass> pGift = ExHelper.CreateTechno(id, pHouse, location, destination, pFocus);
                }
                if (giftBox.Data.Remove)
                {
                    if (giftBox.Data.Destroy)
                    {
                        pTechno.Ref.Base.TakeDamage(pTechno.Ref.Base.Health + 1, pTechno.Ref.Type.Ref.Crewed);
                        // pTechno.Ref.Base.Destroy();
                    }
                    else
                    {
                        pTechno.Ref.Base.Remove();
                        pTechno.Ref.Base.UnInit();
                    }
                }
                else
                {
                    giftBox.Reset();
                }
            }
        }


    }

    public partial class TechnoTypeExt
    {
        public GiftBoxData GiftBoxData;

        /// <summary>
        /// [TechnoType]
        /// GiftBox.Types=HTNK
        /// GiftBox.Nums=1
        /// GiftBox.Remove=yes
        /// GiftBox.Destroy=no
        /// GiftBox.Delay=0
        /// GiftBox.RandomDelay=0,300
        /// GiftBox.RandomRange=0
        /// GiftBox.EmptyCell=no
        /// GiftBox.RandomType=no
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadGiftBox(INIReader reader, string section)
        {
            // GiftBox
            List<string> giftTypes = default;
            if (ExHelper.ReadList(reader, section, "GiftBox.Types", ref giftTypes))
            {
                GiftBoxData = new GiftBoxData(giftTypes);

                List<int> giftNums = default;
                if (ExHelper.ReadIntList(reader, section, "GiftBox.Nums", ref giftNums))
                {
                    GiftBoxData.Nums = giftNums;
                }

                bool giftBoxRemove = false;
                if (reader.ReadNormal(section, "GiftBox.Remove", ref giftBoxRemove))
                {
                    GiftBoxData.Remove = giftBoxRemove;
                }

                bool giftBoxDestroy = false;
                if (reader.ReadNormal(section, "GiftBox.Explodes", ref giftBoxDestroy))
                {
                    GiftBoxData.Destroy = giftBoxDestroy;
                }

                int giftBoxDelay = 0;
                if (reader.ReadNormal(section, "GiftBox.Delay", ref giftBoxDelay))
                {
                    GiftBoxData.Delay = giftBoxDelay;
                }

                List<int> randomDelay = default;
                if (ExHelper.ReadIntList(reader, section, "GiftBox.RandomDelay", ref randomDelay))
                {
                    if (null != randomDelay && randomDelay.Count > 1)
                    {
                        GiftBoxData.DelayMin = randomDelay[0];
                        GiftBoxData.DelayMax = randomDelay[1];
                    }
                }

                int randomRange = 0;
                if (reader.ReadNormal(section, "GiftBox.RandomRange", ref randomRange))
                {
                    GiftBoxData.RandomRange = randomRange;
                }

                bool emptyCell = false;
                if (reader.ReadNormal(section, "GiftBox.RandomToEmptyCell", ref emptyCell))
                {
                    GiftBoxData.EmptyCell = emptyCell;
                }

                bool randomType = false;
                if (reader.ReadNormal(section, "GiftBox.RandomType", ref randomType))
                {
                    GiftBoxData.RandomType = randomType;
                }
            }

        }
    }

}