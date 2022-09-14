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
    public class DamageReactionState : AEState<DamageReactionType>
    {
        public bool forceDone;

        private bool isElite;
        private DamageReactionData data;

        private int count;
        private int delay;
        private TimerStruct delayTimer;

        private int animDelay;
        private TimerStruct animDelayTimer;

        public override void OnEnable()
        {
            this.data = GetDamageReactionData(isElite);
        }

        public void Update(bool isElite)
        {
            if (IsActive())
            {
                this.data = GetDamageReactionData(isElite);
                if (this.isElite != isElite)
                {
                    // 重置计数器
                    if (null != data && data.ResetTimes)
                    {
                        count = 0;
                    }
                }
                if (IsDone() || forceDone)
                {
                    AE.Disable(AE.Location);
                }
            }
            this.isElite = isElite;
        }

        private DamageReactionData GetDamageReactionData(bool isElite)
        {
            if (isElite && null != Data.EliteData)
            {
                return Data.EliteData;
            }
            return Data.Data;
        }

        public bool Reaction(out DamageReactionData reactionData)
        {
            reactionData = data;
            // 检查有效性和冷却
            if (IsActive() && Timeup() && null != data && !IsDone())
            {
                return data.Chance.Bingo();
            }
            return false;
        }

        public void ActionOnce()
        {
            count++;
            if (null != data)
            {
                this.delay = data.Delay;
                forceDone = data.ActiveOnce;
            }
            else
            {
                this.delay = -1;
            }
            if (this.delay > 0)
            {
                delayTimer.Start(delay);
            }
        }

        private bool Timeup()
        {
            return delay <= 0 || delayTimer.Expired();
        }

        private bool IsDone()
        {
            if (data != null)
            {
                return data.TriggeredTimes > 0 && count >= data.TriggeredTimes;
            }
            return false;
        }

        public bool CanPlayAnim()
        {

            return animDelay <= 0 || animDelayTimer.Expired();
        }

        public void AnimPlay()
        {
            this.animDelay = null != data ? data.AnimDelay : -1;
            if (animDelay > 0)
            {
                animDelayTimer.Start(animDelay);
            }
        }

    }
}