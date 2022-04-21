using System.Reflection;
using System.Collections;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    [Serializable]
    public class GiftBoxState : AEState<GiftBoxType>
    {
        public bool IsOpen;

        private int delay;
        private TimerStruct delayTimer;

        public override void OnEnable()
        {
            Reset();
        }

        public void Reset()
        {
            this.IsOpen = false;
            this.delay = Data.RandomDelay.GetRandomValue(Data.Delay);
            if (this.delay > 0)
            {
                delayTimer.Start(this.delay);
            }
        }

        public bool CanOpen()
        {
            return IsActive() && !IsOpen && Timeup();
        }

        private bool Timeup()
        {
            return this.delay <= 0 || delayTimer.Expired();
        }

        public List<string> GetGiftList()
        {
            List<string> gifts = new List<string>();
            if (null != Data)
            {
                int giftCount = Data.Gifts.Count;
                int numsCount = null != Data.Nums ? Data.Nums.Count : 0;
                if (Data.RandomType)
                {
                    int times = 1;
                    if (numsCount > 0)
                    {
                        times = 0;
                        foreach (int num in Data.Nums)
                        {
                            times += num;
                        }
                    }
                    int weightCount = null != Data.RandomWeights ? Data.RandomWeights.Count : 0;
                    Dictionary<Point2D, int> targetPad = new Dictionary<Point2D, int>();
                    int flag = 0;
                    // 将所有的概率加起来，获得上游指标
                    for (int index = 0; index < giftCount; index++)
                    {
                        Point2D target = new Point2D();
                        target.X = flag;
                        int weight = 1;
                        if (weightCount > 0 && index < weightCount)
                        {
                            int w = Data.RandomWeights[index];
                            if (w > 0)
                            {
                                weight = w;
                            }
                        }
                        flag += weight;
                        target.Y = flag;
                        targetPad.Add(target, index);
                    }
                    // 算出随机值，确认位置，取得序号，选出单位
                    for (int i = 0; i < times; i++)
                    {
                        // 选出类型的序号
                        int index = 0;
                        // 产生标靶
                        int p = MathEx.Random.Next(0, flag);
                        // 检查命中
                        foreach (var target in targetPad)
                        {
                            Point2D tKey = target.Key;
                            if (p >= tKey.X && p < tKey.Y)
                            {
                                // 中
                                index = target.Value;
                            }
                        }
                        // 计算概率
                        if (Data.Chances.Bingo(index))
                        {
                            gifts.Add(Data.Gifts[index]);
                        }
                    }
                }
                else
                {
                    for (int index = 0; index < giftCount; index++)
                    {
                        string id = Data.Gifts[index];
                        int times = 1;
                        if (numsCount > 0 && index < numsCount)
                        {
                            times = Data.Nums[index];
                        }
                        for (int i = 0; i < times; i++)
                        {
                            // 计算概率
                            if (Data.Chances.Bingo(index))
                            {
                                gifts.Add(id);
                            }
                        }
                    }
                }
            }
            return gifts;
        }

    }
}