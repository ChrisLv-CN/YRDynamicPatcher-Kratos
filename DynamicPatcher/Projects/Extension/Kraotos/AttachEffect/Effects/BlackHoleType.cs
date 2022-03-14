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
            if (BlackHoleType.ReadBlackHoleType(reader, section, out BlackHoleType blackHoleType))
            {
                this.BlackHoleType = blackHoleType;
            }
        }

    }

    /// <summary>
    /// 黑洞类型
    /// </summary>
    [Serializable]
    public class BlackHoleType : IEffectType<BlackHole>
    {
        // 基础设置
        public bool Enable;
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

        public BlackHole CreateObject(AttachEffectType attachEffectType)
        {
            return new BlackHole(this, attachEffectType);
        }

        public static bool ReadBlackHoleType(INIReader reader, string section, out BlackHoleType blackHoleType)
        {
            blackHoleType = null;

            int range = 0;
            if (reader.ReadNormal(section, "BlackHole.Range", ref range))
            {
                if (range > 0)
                {
                    blackHoleType = new BlackHoleType();
                    blackHoleType.Range = range;
                    blackHoleType.EliteRange = range;
                }
            }

            int eliteRange = 0;
            if (reader.ReadNormal(section, "BlackHole.EliteRange", ref eliteRange))
            {
                if (eliteRange > 0)
                {
                    if (null == blackHoleType)
                    {
                        blackHoleType = new BlackHoleType();
                    }
                    blackHoleType.EliteRange = eliteRange;
                }
            }

            if (null != blackHoleType)
            {

                int rate = 0;
                if (reader.ReadNormal(section, "BlackHole.Rate", ref rate))
                {
                    blackHoleType.Rate = rate;
                    blackHoleType.EliteRate = rate;
                }

                int eliteRate = 0;
                if (reader.ReadNormal(section, "BlackHole.EliteRate", ref eliteRate))
                {
                    blackHoleType.EliteRate = eliteRate;
                }

                // 过滤器
                List<string> affectTypes = null;
                if (ExHelper.ReadList(reader, section, "BlackHole.AffectTypes", ref affectTypes))
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
                        blackHoleType.AffectTypes = types;
                    }
                }

                List<string> notAffectTypes = null;
                if (ExHelper.ReadList(reader, section, "BlackHole.NotAffectTypes", ref notAffectTypes))
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
                        blackHoleType.NotAffectTypes = types;
                    }
                }

                bool affectTechno = false;
                if (reader.ReadNormal(section, "BlackHole.AffectTechno", ref affectTechno))
                {
                    blackHoleType.AffectTechno = affectTechno;
                }

                bool onlyAffectTechno = false;
                if (reader.ReadNormal(section, "BlackHole.OnlyAffectTechno", ref onlyAffectTechno))
                {
                    blackHoleType.OnlyAffectTechno = onlyAffectTechno;
                }

                bool affectMissile = false;
                if (reader.ReadNormal(section, "BlackHole.AffectMissile", ref affectMissile))
                {
                    blackHoleType.AffectMissile = affectMissile;
                }

                bool affectTorpedo = false;
                if (reader.ReadNormal(section, "BlackHole.AffectTorpedo", ref affectTorpedo))
                {
                    blackHoleType.AffectTorpedo = affectTorpedo;
                }

                bool affectCannon = false;
                if (reader.ReadNormal(section, "BlackHole.AffectCannon", ref affectCannon))
                {
                    blackHoleType.AffectCannon = affectCannon;
                }

                // 敌我识别
                bool affectsOwner = false;
                if (reader.ReadNormal(section, "BlackHole.AffectsOwner", ref affectsOwner))
                {
                    blackHoleType.AffectsOwner = affectsOwner;
                }

                bool affectsAllies = false;
                if (reader.ReadNormal(section, "BlackHole.AffectsAllies", ref affectsAllies))
                {
                    blackHoleType.AffectsAllies = affectsAllies;
                }

                bool affectsEnemies = false;
                if (reader.ReadNormal(section, "BlackHole.AffectsEnemies", ref affectsEnemies))
                {
                    blackHoleType.AffectsEnemies = affectsEnemies;
                }
            }

            return null != blackHoleType;
        }

    }

}