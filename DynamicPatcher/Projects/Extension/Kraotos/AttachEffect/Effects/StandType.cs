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
        public StandType StandType;

        private void ReadStandType(INIReader reader, string section)
        {
            StandType type = new StandType();
            if (type.TryReadType(reader, section))
            {
                this.StandType = type;
            }
        }

    }

    /// <summary>
    /// 替身类型
    /// </summary>
    [Serializable]
    public class StandType : EffectType<Stand>
    {
        public string Type; // 替身类型
        public CoordStruct Offset; // 替身相对位置
        // public Direction Direction; // 相对朝向
        public int Direction; // 相对朝向，16分圆，[0-15]
        public bool LockDirection; // 强制朝向，不论替身在做什么
        public bool IsOnTurret; // 相对炮塔或者身体
        public bool IsOnWorld; // 相对世界
        public Layer DrawLayer; // 渲染的层
        public int ZOffset; // ZAdjust偏移值
        public bool Powered; // 是否需要电力支持
        public bool SameHouse; // 与使者同所属
        public bool SameTarget; // 与使者同个目标
        public bool SameLoseTarget; // 使者失去目标时替身也失去
        public bool ForceAttackMaster; // 强制选择使者为目标
        public bool MobileFire; // 移动攻击
        public bool Immune; // 无敌
        public double DamageFromMaster; // 分摊JOJO的伤害
        public double DamageToMaster; // 分摊伤害给JOJO
        public bool Explodes; // 死亡会爆炸
        public bool ExplodesWithMaster; // 使者死亡时强制替身爆炸
        public bool RemoveAtSinking; // 沉船时移除
        public bool PromoteFromMaster; // 与使者同等级
        public double ExperienceToMaster; // 经验给使者
        public bool VirtualUnit; // 虚单位
        public bool SameTilter; // 同步倾斜
        public bool IsTrain; // 火车类型
        public bool CabinHead; // 插入车厢前端
        public int CabinGroup; // 车厢分组

        public StandType()
        {
            this.Type = null;
            this.Offset = default;
            this.Direction = 0;
            this.LockDirection = false;
            this.IsOnTurret = false;
            this.IsOnWorld = false;
            this.DrawLayer = Layer.None;
            this.ZOffset = 12;
            this.SameHouse = true;
            this.SameTarget = true;
            this.SameLoseTarget = false;
            this.ForceAttackMaster = false;
            this.MobileFire = true;
            this.Powered = false;
            this.Immune = true;
            this.DamageFromMaster = 0.0;
            this.DamageToMaster = 0.0;
            this.Explodes = false;
            this.ExplodesWithMaster = false;
            this.RemoveAtSinking = false;
            this.PromoteFromMaster = false;
            this.ExperienceToMaster = 0.0;
            this.VirtualUnit = true;
            this.SameTilter = true;
            this.IsTrain = false;
            this.CabinHead = false;
            this.CabinGroup = -1;
        }

        public override bool TryReadType(INIReader reader, string section)
        {
            // Logger.Log("替身类型 {0} 读取INI配置", section);
            string type = null;
            if (reader.ReadNormal(section, "Stand.Type", ref type) && !string.IsNullOrEmpty(type))
            {
                this.Enable = true;
                this.Type = type.Trim();
                // Logger.Log("替身类型 {0} 名为 {1}", section, type);

                CoordStruct offset = default;
                if (ExHelper.ReadCoordStruct(reader, section, "Stand.Offset", ref offset))
                {
                    this.Offset = offset;
                }

                string dir = "N";
                if (reader.ReadNormal(section, "Stand.Direction", ref dir))
                {
                    Direction direction = (Direction)Enum.Parse(typeof(Direction), dir);
                    this.Direction = (int)direction * 2;
                }

                int dirNum = 0;
                if (reader.ReadNormal(section, "Stand.Direction", ref dirNum))
                {
                    this.Direction = dirNum;
                }

                bool lockDirection = true;
                if (reader.ReadNormal(section, "Stand.LockDirection", ref lockDirection))
                {
                    this.LockDirection = lockDirection;
                }

                bool isOnTurret = true;
                if (reader.ReadNormal(section, "Stand.IsOnTurret", ref isOnTurret))
                {
                    this.IsOnTurret = isOnTurret;
                }

                bool isOnWorld = true;
                if (reader.ReadNormal(section, "Stand.IsOnWorld", ref isOnWorld))
                {
                    this.IsOnWorld = isOnWorld;
                }

                string layerStr = "None";
                if (reader.ReadNormal(section, "Stand.DrawLayer", ref layerStr))
                {
                    Layer layer = Layer.None;
                    string t = layerStr.Substring(0, 1).ToUpper();
                    switch (t)
                    {
                        case "U":
                            layer = Layer.Underground;
                            break;
                        case "S":
                            layer = Layer.Surface;
                            break;
                        case "G":
                            layer = Layer.Ground;
                            break;
                        case "A":
                            layer = Layer.Air;
                            break;
                        case "T":
                            layer = Layer.Top;
                            break;
                    }
                    this.DrawLayer = layer;
                }

                int zOffset = 12;
                if (reader.ReadNormal(section, "Stand.ZOffset", ref zOffset))
                {
                    this.ZOffset = zOffset;
                }

                bool sameHouse = true;
                if (reader.ReadNormal(section, "Stand.SameHouse", ref sameHouse))
                {
                    this.SameHouse = sameHouse;
                }

                bool sameTarget = true;
                if (reader.ReadNormal(section, "Stand.SameTarget", ref sameTarget))
                {
                    this.SameTarget = sameTarget;
                }

                bool sameLoseTarget = true;
                if (reader.ReadNormal(section, "Stand.SameLoseTarget", ref sameLoseTarget))
                {
                    this.SameLoseTarget = sameLoseTarget;
                }

                bool forceAttackMaster = false;
                if (reader.ReadNormal(section, "Stand.ForceAttackMaster", ref forceAttackMaster))
                {
                    this.ForceAttackMaster = forceAttackMaster;
                }

                bool mobileFire = true;
                if (reader.ReadNormal(section, "Stand.MobileFire", ref mobileFire))
                {
                    this.MobileFire = mobileFire;
                }

                bool powered = true;
                if (reader.ReadNormal(section, "Stand.Powered", ref powered))
                {
                    this.Powered = powered;
                }

                bool immune = false;
                if (reader.ReadNormal(section, "Stand.Immune", ref immune))
                {
                    this.Immune = immune;
                }

                double damageFromMaster = 0.0;
                if (reader.ReadNormal(section, "Stand.DamageFromMaster", ref damageFromMaster))
                {
                    if (damageFromMaster > 1.0)
                    {
                        damageFromMaster = 1.0;
                    }
                    else if (damageFromMaster < 0.0)
                    {
                        damageFromMaster = 0.0;
                    }
                    this.DamageFromMaster = damageFromMaster;
                }

                double damageToMaster = 0.0;
                if (reader.ReadNormal(section, "Stand.DamageToMaster", ref damageToMaster))
                {
                    if (damageToMaster > 1.0)
                    {
                        damageToMaster = 1.0;
                    }
                    else if (damageToMaster < 0.0)
                    {
                        damageToMaster = 0.0;
                    }
                    this.DamageToMaster = damageToMaster;
                }

                bool explodes = false;
                if (reader.ReadNormal(section, "Stand.Explodes", ref explodes))
                {
                    this.Explodes = explodes;
                }

                bool explodesWithMaster = false;
                if (reader.ReadNormal(section, "Stand.ExplodesWithMaster", ref explodesWithMaster))
                {
                    this.ExplodesWithMaster = explodesWithMaster;
                }

                bool removeAtSinking = false;
                if (reader.ReadNormal(section, "Stand.RemoveAtSinking", ref removeAtSinking))
                {
                    this.RemoveAtSinking = removeAtSinking;
                }

                bool promoteFromMaster = false;
                if (reader.ReadNormal(section, "Stand.PromoteFromMaster", ref promoteFromMaster))
                {
                    this.PromoteFromMaster = promoteFromMaster;
                }

                double experienceToMaster = 0.0;
                if (reader.ReadNormal(section, "Stand.ExperienceToMaster", ref experienceToMaster))
                {
                    if (experienceToMaster > 1.0)
                    {
                        experienceToMaster = 1.0;
                    }
                    else if (experienceToMaster < 0.0)
                    {
                        experienceToMaster = 0.0;
                    }
                    this.ExperienceToMaster = experienceToMaster;
                }

                bool virtualUnit = false;
                if (reader.ReadNormal(section, "Stand.VirtualUnit", ref virtualUnit))
                {
                    this.VirtualUnit = virtualUnit;
                    if (VirtualUnit)
                    {
                        this.Immune = true;
                    }
                }

                bool sameTilter = false;
                if (reader.ReadNormal(section, "Stand.SameTilter", ref sameTilter))
                {
                    this.SameTilter = sameTilter;
                }

                bool isTrain = false;
                if (reader.ReadNormal(section, "Stand.IsTrain", ref isTrain))
                {
                    this.IsTrain = isTrain;
                }

                bool cabinHead = false;
                if (reader.ReadNormal(section, "Stand.IsCabinHead", ref cabinHead))
                {
                    this.CabinHead = cabinHead;
                }

                int cabinGroup = -1;
                if (reader.ReadNormal(section, "Stand.CabinGroup", ref cabinGroup))
                {
                    this.CabinGroup = cabinGroup;
                }
            }
            return this.Enable;
        }
    }

}