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
    public class AttachEffectState
    {
        public string Token;

        protected bool active;
        protected bool infinite;
        protected TimerStruct timer;

        public AttachEffectState()
        {
            this.active = false;
            this.infinite = false;
            this.timer = new TimerStruct(0);
        }

        public void Enable(int duration, string token)
        {
            this.Token = token;
            this.active = duration != 0;
            if (duration < 0)
            {
                infinite = true;
                timer.Start(0);
            }
            else
            {
                infinite = false;
                timer.Start(duration);
            }
        }

        public void Disable(string token)
        {
            if (this.Token != null && this.Token.Equals(token))
            {
                this.active = false;
                this.infinite = false;
                this.timer.Start(0);
            }
        }

        public bool IsActive()
        {
            return infinite || timer.InProgress();
        }

    }
}