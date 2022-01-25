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
            if (StandType.ReadStandType(reader, section, out StandType standType))
            {
                this.StandType = standType;
            }
        }

    }

    /// <summary>
    /// 替身类型
    /// </summary>
    [Serializable]
    public class StandType : IEffectType<Stand>
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
        public bool SameTarget; // 与使者同个目标
        public bool SameLoseTarget; // 使者失去目标时替身也失去
        public bool Explodes; // 死亡会爆炸
        public bool ExplodesWithMaster; // 使者死亡时强制替身爆炸
        public bool RemoveAtSinking; // 沉船时移除
        public bool PromoteFormMaster; // 与使者同等级
        public double ExperienceToMaster; // 经验给使者
        public bool VirtualUnit; // 不可被选择
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
            this.SameTarget = true;
            this.SameLoseTarget = false;
            this.Powered = false;
            this.Explodes = false;
            this.ExplodesWithMaster = false;
            this.RemoveAtSinking = false;
            this.PromoteFormMaster = false;
            this.ExperienceToMaster = 0.0;
            this.VirtualUnit = false;
            this.IsTrain = false;
            this.CabinHead = false;
            this.CabinGroup = -1;
        }

        public Stand CreateObject(AttachEffectType attachEffectType)
        {
            return new Stand(this, attachEffectType);
        }

        public static bool ReadStandType(INIReader reader, string section, out StandType standType)
        {
            standType = null;
            // Logger.Log("替身类型 {0} 读取INI配置", section);
            string type = null;
            if (reader.ReadNormal(section, "Stand.Type", ref type))
            {
                standType = new StandType();

                standType.Type = type;
                // Logger.Log("替身类型 {0} 名为 {1}", section, type);

                CoordStruct offset = default;
                if (ExHelper.ReadCoordStruct(reader, section, "Stand.Offset", ref offset))
                {
                    standType.Offset = offset;
                }

                string dir = "N";
                if (reader.ReadNormal(section, "Stand.Direction", ref dir))
                {
                    Direction direction = (Direction)Enum.Parse(typeof(Direction), dir);
                    standType.Direction = (int)direction * 2;
                }

                int dirNum = 0;
                if (reader.ReadNormal(section, "Stand.Direction", ref dirNum))
                {
                    standType.Direction = dirNum;
                }

                bool lockDirection = true;
                if (reader.ReadNormal(section, "Stand.LockDirection", ref lockDirection))
                {
                    standType.LockDirection = lockDirection;
                }

                bool isOnTurret = true;
                if (reader.ReadNormal(section, "Stand.IsOnTurret", ref isOnTurret))
                {
                    standType.IsOnTurret = isOnTurret;
                }

                bool isOnWorld = true;
                if (reader.ReadNormal(section, "Stand.IsOnWorld", ref isOnWorld))
                {
                    standType.IsOnWorld = isOnWorld;
                }

                string layerStr = "None";
                if (reader.ReadNormal(section, "Stand.DrawLayer", ref layerStr))
                {
                    Layer layer = Layer.None;
                    string t = layerStr.Substring(0, 1).ToUpper();
                    switch(t)
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
                    standType.DrawLayer = layer;
                }

                int zOffset = 12;
                if (reader.ReadNormal(section, "Stand.ZOffset", ref zOffset))
                {
                    standType.ZOffset = zOffset;
                }

                bool sameTarget = true;
                if (reader.ReadNormal(section, "Stand.SameTarget", ref sameTarget))
                {
                    standType.SameTarget = sameTarget;
                }

                bool sameLoseTarget = true;
                if (reader.ReadNormal(section, "Stand.SameLoseTarget", ref sameLoseTarget))
                {
                    standType.SameLoseTarget = sameLoseTarget;
                }

                bool powered = true;
                if (reader.ReadNormal(section, "Stand.Powered", ref powered))
                {
                    standType.Powered = powered;
                }

                bool explodes = false;
                if (reader.ReadNormal(section, "Stand.Explodes", ref explodes))
                {
                    standType.Explodes = explodes;
                }

                bool explodesWithMaster = false;
                if (reader.ReadNormal(section, "Stand.ExplodesWithMaster", ref explodesWithMaster))
                {
                    standType.ExplodesWithMaster = explodesWithMaster;
                }

                bool removeAtSinking = false;
                if (reader.ReadNormal(section, "Stand.RemoveAtSinking", ref removeAtSinking))
                {
                    standType.RemoveAtSinking = removeAtSinking;
                }

                bool promoteFormMaster = false;
                if (reader.ReadNormal(section, "Stand.PromoteFormMaster", ref promoteFormMaster))
                {
                    standType.PromoteFormMaster = promoteFormMaster;
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
                    standType.ExperienceToMaster = experienceToMaster;
                }

                bool virtualUnit = false;
                if (reader.ReadNormal(section, "Stand.VirtualUnit", ref virtualUnit))
                {
                    standType.VirtualUnit = virtualUnit;
                }

                bool isTrain = false;
                if (reader.ReadNormal(section, "Stand.IsTrain", ref isTrain))
                {
                    standType.IsTrain = isTrain;
                }

                bool cabinHead = false;
                if (reader.ReadNormal(section, "Stand.IsCabinHead", ref cabinHead))
                {
                    standType.CabinHead = cabinHead;
                }

                int cabinGroup = -1;
                if (reader.ReadNormal(section, "Stand.CabinGroup", ref cabinGroup))
                {
                    standType.CabinGroup = cabinGroup;
                }
            }
            return null != standType;
        }


    }

}