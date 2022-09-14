using DynamicPatcher;
using Extension.Script;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    public interface IAEState
    {
        public void Enable(int duration, string token, IAEStateData data);

        public void Disable(string token);

        public bool IsActive();

    }

    [Serializable]
    public class AEState<T> : IAEState where T : IAEStateData, new()
    {
        public string Token;
        public AttachEffect AE;

        public T Data;

        protected bool active;
        protected bool infinite;
        protected TimerStruct timer;

        public AEState()
        {
            this.active = false;
            this.infinite = false;
            this.timer.Start(0);
        }

        public void EnableAndReplace<TT>(Effect<TT> effect) where TT : IEffectType, IAEStateData, new()
        {
            // 激活新的效果，关闭旧的效果
            if (!string.IsNullOrEmpty(Token) && Token != effect.Token && null != AE && AE.IsActive())
            {
                AE.Disable(AE.Location);
            }
            this.AE = effect.AE;
            Enable(effect.AEType.GetDuration(), effect.Token, effect.Type);
        }

        public void Enable(IAEStateData data)
        {
            Enable(-1, null, data);
        }

        public void Enable(int duration, string token, IAEStateData data)
        {
            this.Token = token;
            this.Data = (T)data;
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
            // Logger.Log($"{Game.CurrentFrame}, Enable AE State {Data.GetType().Name}, token {Token}");
            OnEnable();
        }

        public virtual void OnEnable() { }

        public void Disable()
        {
            Disable(this.Token);
        }

        public void Disable(string token)
        {
            if (this.Token == token)
            {
                this.active = false;
                this.infinite = false;
                this.timer.Start(0);

                // Logger.Log($"{Game.CurrentFrame}, Disable AE State {Data.GetType().Name}, token {Token}");
                OnDisable();
            }
        }

        public virtual void OnDisable() { }

        public bool IsActive()
        {
            return infinite || timer.InProgress();
        }

    }

}
