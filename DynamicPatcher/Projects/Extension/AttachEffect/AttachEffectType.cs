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
    [Serializable]
    public enum CumulativeMode
    {
        NO = 0, YES = 1, ATTACKER = 2
    }

    [Serializable]
    public partial class AttachEffectType : Enumerable<AttachEffectType>, INewType<AttachEffect>
    {
        public List<string> AffectTypes; // 可影响的单位
        public int Duration; // 持续时间
        public bool HoldDuration; // 无限时间
        public int Delay; // 不可获得同名的延迟
        public bool RandomDelay; // 随机延迟
        public int MinDelay; // 随机最小值
        public int MaxDelay; // 随机最大值
        public int InitialDelay; // 生效前的初始延迟
        public bool InitialRandomDelay; // 随机初始延迟
        public int InitialMinDelay; // 随机最小值
        public int InitialMaxDelay; // 随机最大值
        public bool DiscardOnEntry; // 离开地图则失效
        public bool PenetratesIronCurtain; // 弹头附加，影响铁幕
        public bool FromTransporter; // 弹头附加，乘客附加时，视为载具
        public bool OwnerTarget; // 弹头附加，属于被赋予对象
        public CumulativeMode Cumulative; // 可叠加
        public int Group; // 分组，同一个分组的效果互相影响，削减或增加持续时间
        public bool OverrideSameGroup; // 是否覆盖同一个分组
        public string Next; // 结束后播放下一个AE

        // 赋予对象过滤
        public bool AttachOnceInTechnoType; // 写在TechnoType上只在创建时赋予一次
        public bool AttachWithDamage; // 弹头附加，随着伤害附加，而不是按弹头爆炸位置附加，如在使用AmbientDamage时
        public bool AffectBullet; // 弹头附加，附加抛射体
        public bool OnlyAffectBullet; // 弹头附加，只附加抛射体
        public bool AffectMissile; // 弹头附加，影响ROT>0
        public bool AffectTorpedo; // 弹头附加，影响Level=yes
        public bool AffectCannon; // 弹头附加，影响Arcing=yes

        public AttachEffectType(string name) : base(name)
        {
            this.AffectTypes = null;
            this.Duration = 1;
            this.HoldDuration = true;
            this.Delay = 0;
            this.RandomDelay = false;
            this.MinDelay = 0;
            this.MaxDelay = 0;
            this.InitialDelay = -1;
            this.InitialRandomDelay = false;
            this.InitialMinDelay = 0;
            this.InitialMaxDelay = 0;
            this.DiscardOnEntry = false;
            this.PenetratesIronCurtain = false;
            this.FromTransporter = true;
            this.OwnerTarget = false;
            this.Cumulative = CumulativeMode.NO;
            this.Group = -1;
            this.OverrideSameGroup = false;
            this.Next = null;
            // 赋予对象过滤
            this.AttachOnceInTechnoType = false;
            this.AttachWithDamage = false;
            this.AffectBullet = false;
            this.OnlyAffectBullet = false;
            this.AffectMissile = true;
            this.AffectTorpedo = true;
            this.AffectCannon = false;
        }

        public AttachEffect CreateObject()
        {
            return new AttachEffect(this);
        }

        public override void LoadFromINI(Pointer<CCINIClass> pINI)
        {
            INIReader reader = new INIReader(pINI);
            string section = Name;

            List<string> affectTypes = null;
            if (ExHelper.ReadList(reader, section, "AffectTypes", ref affectTypes))
            {
                List<string> types = null;
                foreach (string typeName in affectTypes)
                {
                    if (!string.IsNullOrEmpty(typeName) && !"none".Equals(typeName.Trim().ToLower()))
                    {
                        if (null == types)
                        {
                            types = new List<string>();
                        }
                        types.Add(typeName);
                    }
                }
                if (null != types)
                {
                    this.AffectTypes = types;
                }
            }

            int duration = -1;
            if (reader.ReadNormal(section, "Duration", ref duration))
            {
                this.Duration = duration;
                if (duration > 0)
                {
                    this.HoldDuration = false;
                }
            }

            bool holdDuration = false;
            if (reader.ReadNormal(section, "HoldDuration", ref holdDuration))
            {
                this.HoldDuration = holdDuration;
            }

            int delay = 0;
            if (reader.ReadNormal(section, "Delay", ref delay))
            {
                this.Delay = delay;
            }

            List<int> randomDelay = null;
            if (ExHelper.ReadIntList(reader, section, "RandomDelay", ref randomDelay))
            {
                if (null != randomDelay && randomDelay.Count > 1)
                {
                    this.RandomDelay = true;
                    this.MinDelay = randomDelay[0];
                    this.MaxDelay = randomDelay[1];
                    if (this.MaxDelay < this.MinDelay)
                    {
                        int temp = this.MaxDelay;
                        this.MaxDelay = this.MinDelay;
                        this.MinDelay = temp;
                    }
                }
            }

            int initDelay = 0;
            if (reader.ReadNormal(section, "InitialDelay", ref initDelay))
            {
                this.InitialDelay = initDelay;
            }

            List<int> initRandomDelay = null;
            if (ExHelper.ReadIntList(reader, section, "InitialRandomDelay", ref initRandomDelay))
            {
                if (null != initRandomDelay && initRandomDelay.Count > 1)
                {
                    this.InitialRandomDelay = true;
                    this.InitialMinDelay = initRandomDelay[0];
                    this.InitialMaxDelay = initRandomDelay[1];
                    if (this.InitialMaxDelay < this.InitialMinDelay)
                    {
                        int temp = this.InitialMaxDelay;
                        this.InitialMaxDelay = this.InitialMinDelay;
                        this.InitialMinDelay = temp;
                    }
                }
            }

            bool discardOnEntry = false;
            if (reader.ReadNormal(section, "DiscardOnEntry", ref discardOnEntry))
            {
                this.DiscardOnEntry = discardOnEntry;
            }

            bool penetratesIronCurtain = false;
            if (reader.ReadNormal(section, "PenetratesIronCurtain", ref penetratesIronCurtain))
            {
                this.PenetratesIronCurtain = penetratesIronCurtain;
            }

            bool fromTransporter = false;
            if (reader.ReadNormal(section, "FromTransporter", ref fromTransporter))
            {
                this.FromTransporter = fromTransporter;
            }

            bool ownerTarget = false;
            if (reader.ReadNormal(section, "OwnerTarget", ref ownerTarget))
            {
                this.OwnerTarget = ownerTarget;
            }

            string cumulative = "no";
            if (reader.ReadNormal(section, "Cumulative", ref cumulative))
            {
                CumulativeMode cumulativeMode = CumulativeMode.NO;
                string t = cumulative.Substring(0, 1).ToUpper();
                switch (t)
                {
                    case "1":
                    case "T": // true
                    case "Y": // yes
                        cumulativeMode = CumulativeMode.YES;
                        break;
                    case "0":
                    case "F": // false
                    case "N": // no
                        cumulativeMode = CumulativeMode.NO;
                        break;
                    case "A": // attacker
                        cumulativeMode = CumulativeMode.ATTACKER;
                        break;
                }
                this.Cumulative = cumulativeMode;
            }

            int group = 0;
            if (reader.ReadNormal(section, "Group", ref group))
            {
                this.Group = group;
            }

            bool overrideSameGroup = false;
            if (reader.ReadNormal(section, "OverrideSameGroup", ref overrideSameGroup))
            {
                this.OverrideSameGroup = overrideSameGroup;
            }

            string next = null;
            if (reader.ReadNormal(section, "Next", ref next))
            {
                this.Next = next;
            }

            // 赋予出厂单位时只赋予一次
            bool attachOnceInTechnoType = false;
            if (reader.ReadNormal(section, "AttachOnceInTechnoType", ref attachOnceInTechnoType))
            {
                this.AttachOnceInTechnoType = attachOnceInTechnoType;
            }

            // 赋予对象过滤
            bool attachWithDamage = false;
            if (reader.ReadNormal(section, "AttachWithDamage", ref attachWithDamage))
            {
                this.AttachWithDamage = attachWithDamage;
            }

            bool affectBullet = false;
            if (reader.ReadNormal(section, "AffectBullet", ref affectBullet))
            {
                this.AffectBullet = affectBullet;
            }

            bool onlyAffectBullet = false;
            if (reader.ReadNormal(section, "OnlyAffectBullet", ref onlyAffectBullet))
            {
                this.OnlyAffectBullet = onlyAffectBullet;
            }

            bool affectMissile = false;
            if (reader.ReadNormal(section, "AffectMissile", ref affectMissile))
            {
                this.AffectMissile = affectMissile;
            }

            bool affectTorpedo = false;
            if (reader.ReadNormal(section, "AffectTorpedo", ref affectTorpedo))
            {
                this.AffectTorpedo = affectTorpedo;
            }

            bool affectCannon = false;
            if (reader.ReadNormal(section, "AffectCannon", ref affectCannon))
            {
                this.AffectCannon = affectCannon;
            }

            ReadAnimationType(reader, section);
            ReadAttachStatusType(reader, section);
            ReadAutoWeaponType(reader, section);
            ReadDestroySelfType(reader, section);
            ReadPaintballType(reader, section);
            ReadStandType(reader, section);
            ReadTransformType(reader, section);

            base.LoadFromINI(pINI);
        }

    }

}