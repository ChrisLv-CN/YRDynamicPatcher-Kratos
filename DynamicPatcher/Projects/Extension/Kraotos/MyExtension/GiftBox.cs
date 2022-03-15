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

        private TimerStruct delayTimer;

        public GiftBox(GiftBoxData data)
        {
            this.Enable = data.Enable;
            this.Data = data;
            Reset();
        }

        public bool CanOpen()
        {
            return Enable && !IsOpen && Timeup();
        }

        private bool Timeup()
        {
            if (this.Delay <= 0 || delayTimer.Expired())
            {
                this.IsOpen = true;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            this.IsOpen = false;
            this.Delay = Data.DelayMax == 0 ? Data.Delay : MathEx.Random.Next(Data.DelayMin, Data.DelayMax);
            if (this.Delay > 0)
            {
                delayTimer = new TimerStruct(this.Delay);
            }
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
        public bool OpenWhenDestoryed;

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
            this.OpenWhenDestoryed = false;
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
                    gifts.Add(Gifts[MathEx.Random.Next(0, Gifts.Count)]);
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
            if (null != giftBox && !giftBox.Data.OpenWhenDestoryed && giftBox.CanOpen())
            {
                // 释放礼物
                ReleseGift(giftBox.Data);
                // 销毁或重置盒子
                if (giftBox.Data.Remove)
                {
                    pTechno.Ref.Base.Remove();
                    if (giftBox.Data.Destroy)
                    {
                        pTechno.Ref.Base.TakeDamage(pTechno.Ref.Base.Health + 1, pTechno.Ref.Type.Ref.Crewed);
                        // pTechno.Ref.Base.Destroy();
                    }
                    else
                    {
                        pTechno.Ref.Base.UnInit();
                    }
                }
                else
                {
                    // 重置
                    giftBox.Reset();
                }
            }
        }

        public unsafe void TechnoClass_Destroy_GiftBox()
        {
            if (null != giftBox && giftBox.Data.OpenWhenDestoryed && !giftBox.IsOpen)
            {
                ReleseGift(giftBox.Data);
                giftBox.IsOpen = true;
            }
        }

        private void ReleseGift(GiftBoxData giftBoxData)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            Pointer<HouseClass> pHouse = pTechno.Ref.Owner;
            CoordStruct location = pTechno.Ref.Base.Base.GetCoords();

            // 获取投送单位的位置
            if (MapClass.Instance.TryGetCellAt(location, out Pointer<CellClass> pCell))
            {
                // 投送后需要前往的目的地
                Pointer<AbstractClass> pDest = IntPtr.Zero; // 载具当前的移动目的地
                Pointer<AbstractClass> pFocus = IntPtr.Zero; // 步兵的移动目的地
                // 获取目的地
                if (pTechno.Ref.Base.Base.WhatAmI() != AbstractType.Building)
                {
                    pDest = pTechno.Convert<FootClass>().Ref.Destination;
                    pFocus = pTechno.Ref.Focus;
                }
                // 开始投送单位，每生成一个单位就选择一次位置
                foreach (string id in giftBoxData.GetGiftList())
                {
                    // 随机选择周边的格子
                    if (giftBoxData.RandomRange > 0)
                    {
                        int landTypeCategory = pCell.Ref.LandType.Category();
                        CellStruct cell = MapClass.Coord2Cell(location);
                        CellStruct[] cellOffset = new CellSpreadEnumerator((uint)giftBoxData.RandomRange).ToArray();
                        int max = cellOffset.Count();
                        for (int i = 0; i < max; i++)
                        {
                            int index = MathEx.Random.Next(max - 1);
                            CellStruct offset = cellOffset[index];
                            // Logger.Log("随机获取周围格子索引{0}, 共{1}格, 获取的格子偏移{2}, 单位当前坐标{3}, 第一个格子的坐标{4}, 尝试次数{5}, 当前偏移{6}", index, max, offset, location, MapClass.Cell2Coord(cell + cellOffset[0]), i, cellOffset[i]);
                            if (offset == default)
                            {
                                continue;
                            }
                            if (MapClass.Instance.TryGetCellAt(cell + offset, out Pointer<CellClass> pTargetCell))
                            {
                                if (pTargetCell.Ref.LandType.Category() != landTypeCategory
                                    || (giftBoxData.EmptyCell && !pTargetCell.Ref.GetContent().IsNull))
                                {
                                    // Logger.Log("获取到的格子被占用, 建筑{0}, 步兵{1}, 载具{2}", !pCell.Ref.GetBuilding().IsNull, !pCell.Ref.GetUnit(false).IsNull, !pCell.Ref.GetInfantry(false).IsNull);
                                    continue;
                                }
                                pCell = pTargetCell;
                                location = pCell.Ref.GetCoordsWithBridge();
                                // Logger.Log("获取到的格子坐标{0}", location);
                                break;
                            }
                        }
                    }
                    // 投送单位
                    Pointer<TechnoClass> pGift = ExHelper.CreateAndPutTechno(id, pHouse, location, pCell);
                    if (!pGift.IsNull)
                    {
                        // 开往预定目的地
                        if (pDest.IsNull && pFocus.IsNull)
                        {
                            pGift.Ref.Base.Scatter(CoordStruct.Empty, true, false);
                        }
                        else
                        {
                            if (pGift.Ref.Base.Base.WhatAmI() != AbstractType.Building)
                            {
                                CoordStruct des = pDest.IsNull ? location : pDest.Ref.GetCoords();
                                if (!pFocus.IsNull)
                                {
                                    pGift.Ref.SetFocus(pFocus);
                                    if (pGift.Ref.Base.Base.WhatAmI() == AbstractType.Unit)
                                    {
                                        des = pFocus.Ref.GetCoords();
                                    }
                                }
                                if (MapClass.Instance.TryGetCellAt(des, out Pointer<CellClass> pTargetCell))
                                {
                                    pGift.Ref.SetDestination(pTargetCell, true);
                                    pGift.Convert<MissionClass>().Ref.QueueMission(Mission.Move, false);
                                }
                            }
                        }
                    }
                    else
                    {
                        Logger.LogWarning("Gift box release gift failed, unknown TechnoType [{0}]", id);
                    }
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
        /// GiftBox.RandomToEmptyCell=no
        /// GiftBox.RandomType=no
        /// GiftBox.OpenWhenDestoryed=no
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

                bool openWhenDestoryed = false;
                if (reader.ReadNormal(section, "GiftBox.OpenWhenDestoryed", ref openWhenDestoryed))
                {
                    GiftBoxData.OpenWhenDestoryed = openWhenDestoryed;
                }
            }

        }
    }

}