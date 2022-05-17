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
            AnimationType type = new AnimationType();
            if (type.TryReadType(reader, section))
            {
                this.Enable = true;
                this.AnimationType = type;
            }
            else
            {
                type = null;
            }
        }
    }

    /// <summary>
    /// AE动画
    /// </summary>
    [Serializable]
    public class AnimationType : EffectType<Animation>
    {
        public string IdleAnim; // 持续动画
        public string ActiveAnim; // 激活时播放的动画
        public string HitAnim; // 被击中时播放的动画
        public string DoneAnim; // 结束时播放的动画

        public bool RemoveInCloak; // 隐形时移除
        public bool TranslucentInCloak; // 隐形时调整透明度为50
        public Relation Visibility; // 谁能看见持续动画

        public AnimationType()
        {
            this.IdleAnim = null;
            this.ActiveAnim = null;
            this.HitAnim = null;
            this.DoneAnim = null;

            this.RemoveInCloak = true;
            this.TranslucentInCloak = false;
            this.Visibility = Relation.All;
        }

        public override bool TryReadType(INIReader reader, string section)
        {
            string type = null;
            if (reader.ReadNormal(section, "Animation", ref type))
            {
                this.Enable = true;
                this.IdleAnim = type;

                bool removeInCloak = false;
                if (reader.ReadNormal(section, "Anim.RemoveInCloak", ref removeInCloak))
                {
                    this.RemoveInCloak = removeInCloak;
                }

                bool translucentInCloak = false;
                if (reader.ReadNormal(section, "Anim.TranslucentInCloak", ref translucentInCloak))
                {
                    this.TranslucentInCloak = translucentInCloak;
                }

                string v = null;
                if (reader.ReadNormal(section, "Anim.Visibility", ref v))
                {
                    Relation relation = Relation.All;
                    string t = v.Substring(0, 1).ToUpper();
                    switch (t)
                    {
                        case "O":
                            relation = Relation.OWNER;
                            break;
                        case "A":
                            relation = Relation.Team;
                            break;
                        case "E":
                            relation = Relation.ENEMIES;
                            break;
                    }
                    this.Visibility = relation;
                }
            }

            string act = null;
            if (reader.ReadNormal(section, "ActiveAnim", ref act))
            {
                this.Enable = true;
                this.ActiveAnim = act;
            }

            string hit = null;
            if (reader.ReadNormal(section, "HitAnim", ref hit))
            {
                this.Enable = true;
                this.HitAnim = hit;
            }

            string done = null;
            if (reader.ReadNormal(section, "DoneAnim", ref done))
            {
                this.Enable = true;
                this.DoneAnim = done;
            }

            return this.Enable;
        }

    }

}