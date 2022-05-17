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
    public class DestroySelfState : AEState<DestroySelfType>
    {
        public bool GoDie;

        private int timeLeft;
        private TimerStruct CountdownTimer;

        public void DestroyNow(bool peaceful)
        {
            DestroySelfType data = new DestroySelfType();
            data.Peaceful = peaceful;
            Enable(data);
        }

        public override void OnEnable()
        {
            Reset();
        }

        public void Reset()
        {
            this.GoDie = false;
            this.timeLeft = Data.Delay;
            if (timeLeft > 0)
            {
                CountdownTimer.Start(timeLeft);
            }
        }

        public bool AmIDead()
        {
            return IsActive() && !GoDie && Timeup();
        }

        private bool Timeup()
        {
            GoDie = timeLeft <= 0 || CountdownTimer.Expired();
            return GoDie;
        }

    }
}