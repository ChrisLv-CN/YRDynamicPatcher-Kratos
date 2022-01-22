using DynamicPatcher;
using Extension.Ext;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{


    public partial class AttachEffectType
    {
        public AnimationType AnimationType;

        private void ReadAnimationType(INIReader reader, string section)
        {
            if (AnimationType.ReadAnimationType(reader, section, out AnimationType animType))
            {
                this.AnimationType = animType;
            }
        }
    }

    /// <summary>
    /// AE动画
    /// </summary>
    [Serializable]
    public class AnimationType : IEffectType<Animation>
    {
        public string IdleAnim; // 持续动画
        public string ActiveAnim; // 激活时播放的动画
        public string HitAnim; // 被击中时播放的动画
        public string DoneAnim; // 结束时播放的动画

        public AnimationType()
        {
            this.IdleAnim = null;
            this.ActiveAnim = null;
            this.HitAnim = null;
            this.DoneAnim = null;
        }

        public Animation CreateObject(AttachEffectType attachEffectType)
        {
            return new Animation(this, attachEffectType);
        }

        public static bool ReadAnimationType(INIReader reader, string section, out AnimationType animType)
        {
            animType = null;

            string type = null;
            if (reader.ReadNormal(section, "Animation", ref type))
            {
                if (null == animType)
                {
                    animType = new AnimationType();
                }
                animType.IdleAnim = type;
            }

            string act = null;
            if (reader.ReadNormal(section, "ActiveAnim", ref act))
            {
                if (null == animType)
                {
                    animType = new AnimationType();
                }
                animType.ActiveAnim = act;
            }

            string hit = null;
            if (reader.ReadNormal(section, "HitAnim", ref hit))
            {
                if (null == animType)
                {
                    animType = new AnimationType();
                }
                animType.HitAnim = hit;
            }

            string done = null;
            if (reader.ReadNormal(section, "DoneAnim", ref done))
            {
                if (null == animType)
                {
                    animType = new AnimationType();
                }
                animType.DoneAnim = done;
            }

            return null != animType;
        }

    }

}