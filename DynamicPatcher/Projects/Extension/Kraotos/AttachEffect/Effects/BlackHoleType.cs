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
        public BlackHoleType BlackHoleType;

        private void ReadBlackHoleType(INIReader reader, string section)
        {
            BlackHoleType type = new BlackHoleType();
            if (type.TryReadType(reader, section))
            {
                this.BlackHoleType = type;
            }
        }

    }

    /// <summary>
    /// 黑洞类型
    /// </summary>
    [Serializable]
    public class BlackHoleType : EffectType<BlackHole>
    {
        // 基础设置
        public int Range;
        public int EliteRange;
        public int Rate;
        public int EliteRate;
        // 类型过滤
        public List<string> AffectTypes;
        public List<string> NotAffectTypes;
        public bool AffectTechno;
        public bool OnlyAffectTechno;
        public bool AffectMissile;
        public bool AffectTorpedo;
        public bool AffectCannon;
        // 敌我识别
        public bool AffectsOwner;
        public bool AffectsAllies;
        public bool AffectsEnemies;

        public BlackHoleType()
        {
            // 基础设置
            this.Enable = false;
            this.Range = 0;
            this.EliteRange = 0;
            this.Rate = 15;
            this.EliteRate = 15;
            // 类型过滤
            this.AffectTypes = null;
            this.NotAffectTypes = null;
            this.AffectTechno = false;
            this.OnlyAffectTechno = false;
            this.AffectMissile = true;
            this.AffectTorpedo = true;
            this.AffectCannon = false;
            // 敌我识别
            this.AffectsOwner = false;
            this.AffectsAllies = false;
            this.AffectsEnemies = true;
        }

        public override bool TryReadType(INIReader reader, string section)
        {

            ReadCommonType(reader, section, "BlackHole.");

            int range = 0;
            if (reader.ReadNormal(section, "BlackHole.Range", ref range))
            {
                if (range > 0)
                {
                    this.Enable = true;
                    this.Range = range;
                    this.EliteRange = range;
                }
            }

            int eliteRange = 0;
            if (reader.ReadNormal(section, "BlackHole.EliteRange", ref eliteRange))
            {
                if (eliteRange > 0)
                {
                    this.Enable = true;
                    this.EliteRange = eliteRange;
                }
            }

            if (this.Enable)
            {

                int rate = 0;
                if (reader.ReadNormal(section, "BlackHole.Rate", ref rate))
                {
                    this.Rate = rate;
                    this.EliteRate = rate;
                }

                int eliteRate = 0;
                if (reader.ReadNormal(section, "BlackHole.EliteRate", ref eliteRate))
                {
                    this.EliteRate = eliteRate;
                }

                // 过滤器
                List<string> affectTypes = null;
                if (reader.ReadStringList(section, "BlackHole.AffectTypes", ref affectTypes))
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

                List<string> notAffectTypes = null;
                if (reader.ReadStringList(section, "BlackHole.NotAffectTypes", ref notAffectTypes))
                {
                    List<string> types = null;
                    foreach (string typeName in notAffectTypes)
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
                        this.NotAffectTypes = types;
                    }
                }

                bool affectTechno = false;
                if (reader.ReadNormal(section, "BlackHole.AffectTechno", ref affectTechno))
                {
                    this.AffectTechno = affectTechno;
                }

                bool onlyAffectTechno = false;
                if (reader.ReadNormal(section, "BlackHole.OnlyAffectTechno", ref onlyAffectTechno))
                {
                    this.OnlyAffectTechno = onlyAffectTechno;
                }

                bool affectMissile = false;
                if (reader.ReadNormal(section, "BlackHole.AffectMissile", ref affectMissile))
                {
                    this.AffectMissile = affectMissile;
                }

                bool affectTorpedo = false;
                if (reader.ReadNormal(section, "BlackHole.AffectTorpedo", ref affectTorpedo))
                {
                    this.AffectTorpedo = affectTorpedo;
                }

                bool affectCannon = false;
                if (reader.ReadNormal(section, "BlackHole.AffectCannon", ref affectCannon))
                {
                    this.AffectCannon = affectCannon;
                }

                // 敌我识别
                bool affectsOwner = false;
                if (reader.ReadNormal(section, "BlackHole.AffectsOwner", ref affectsOwner))
                {
                    this.AffectsOwner = affectsOwner;
                }

                bool affectsAllies = false;
                if (reader.ReadNormal(section, "BlackHole.AffectsAllies", ref affectsAllies))
                {
                    this.AffectsAllies = affectsAllies;
                }

                bool affectsEnemies = false;
                if (reader.ReadNormal(section, "BlackHole.AffectsEnemies", ref affectsEnemies))
                {
                    this.AffectsEnemies = affectsEnemies;
                }
            }

            return this.Enable;
        }
    }

}