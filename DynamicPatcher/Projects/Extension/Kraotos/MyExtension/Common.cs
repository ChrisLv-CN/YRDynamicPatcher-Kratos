using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    [Serializable]
    public class AresTechnoExt
    {
        public bool Cloakable_Allowed = true;

    }

    public partial class TechnoTypeExt
    {
        public AresTechnoExt Ares = new AresTechnoExt();

        private void ReadAresFlags(INIReader reader, string section)
        {
            bool cloakable_Allowed = false;
            if (reader.ReadNormal(section, "Cloakable.Allowed", ref cloakable_Allowed))
            {
                Ares.Cloakable_Allowed = cloakable_Allowed;
            }
        }

    }

    [Serializable]
    public class AresBulletExt
    {
        public int BallisticScatterMin = 0;
        public int BallisticScatterMax = 0;
    }

    public partial class BulletTypeExt
    {

        public AresBulletExt Ares = new AresBulletExt();

        private void ReadAresFlags(INIReader reader, string section)
        {

            float min = 0;
            if (reader.ReadNormal(section, "BallisticScatter.Min", ref min))
            {
                Ares.BallisticScatterMin = (int)(min * 256);
            }

            float max = 0;
            if (reader.ReadNormal(section, "BallisticScatter.Max", ref max))
            {
                Ares.BallisticScatterMax = (int)(max * 256);
            }
        }
    }

    public partial class WeaponTypeExt
    {

        public int LaserThickness;
        public bool LaserFade;
        public bool IsSupported;

        private void ReadLaser(INIReader reader, string section)
        {
            int thickness = 0;
            if (reader.ReadNormal(section, "LaserThickness", ref thickness))
            {
                this.LaserThickness = thickness;
            }

            bool isFade = false;
            if (reader.ReadNormal(section, "LaserFade", ref isFade))
            {
                this.LaserFade = isFade;
            }

            bool isSupported = false;
            if (reader.ReadNormal(section, "IsSupported", ref isSupported))
            {
                this.IsSupported = isSupported;
            }
        }
    }

    [Serializable]
    public class AresWarheadExt
    {
        public bool AffectsOwner; // 是否伤害持有者，默认同AffectsAllies
        public bool AffectsEnemies; // 是否伤害敌人，默认yes
        public bool EffectsRequireDamage; // 至少伤害1，默认no
        public bool EffectsRequireVerses; // 只影响弹头比例大于0%的目标，默认yes
        public bool AllowZeroDamage; // 伤害0也能影响，默认no

        public AresWarheadExt()
        {
            AffectsOwner = true;
            AffectsEnemies = true;
            EffectsRequireDamage = false;
            EffectsRequireVerses = true;
            AllowZeroDamage = false;
        }
    }

    public partial class WarheadTypeExt
    {

        public bool AffectsAir = true;
        public bool AffectsBullet = false;

        public AresWarheadExt Ares = new AresWarheadExt();


        private void ReadAffectsFlags(INIReader reader, string section)
        {
            bool affectsAir = true;
            if (reader.ReadNormal(section, "AffectsAir", ref affectsAir))
            {
                this.AffectsAir = affectsAir;
            }

            bool affectsBullet = true;
            if (reader.ReadNormal(section, "AffectsBullet", ref affectsBullet))
            {
                this.AffectsBullet = affectsBullet;
            }

        }

        private void ReadAresFlags(INIReader reader, string section)
        {

            bool affectsAllies = false;
            if (reader.ReadNormal(section, "AffectsAllies", ref affectsAllies))
            {
                Ares.AffectsOwner = affectsAllies;
            }

            bool affectsOwner = false;
            if (reader.ReadNormal(section, "AffectsOwner", ref affectsOwner))
            {
                Ares.AffectsOwner = affectsOwner;
            }

            bool affectsEnemies = false;
            if (reader.ReadNormal(section, "AffectsEnemies", ref affectsEnemies))
            {
                Ares.AffectsEnemies = affectsEnemies;
            }

            bool effectsRequireDamage = false;
            if (reader.ReadNormal(section, "EffectsRequireDamage", ref effectsRequireDamage))
            {
                Ares.EffectsRequireDamage = effectsRequireDamage;
            }

            bool effectsRequireVerses = false;
            if (reader.ReadNormal(section, "EffectsRequireVerses", ref effectsRequireVerses))
            {
                Ares.EffectsRequireVerses = effectsRequireVerses;
            }

            bool allowZeroDamage = false;
            if (reader.ReadNormal(section, "AllowZeroDamage", ref allowZeroDamage))
            {
                Ares.AllowZeroDamage = allowZeroDamage;
            }

        }
    }

    [Serializable]
    public class AresAnimExt
    {
        public string Weapon;
        public int WeaponDelay;

        public AresAnimExt()
        {
            this.Weapon = null;
            this.WeaponDelay = 0;
        }
    }

    public partial class AnimTypeExt
    {

        public AresAnimExt Ares = new AresAnimExt();

        private void ReadAresFlags(INIReader reader, string section)
        {
            string weapon = null;
            if (reader.ReadNormal(section, "Weapon", ref weapon))
            {
                Ares.Weapon = weapon;
            }

            int weaponDelay = 0;
            if (reader.ReadNormal(section, "Damage.Delay", ref weaponDelay))
            {
                Ares.WeaponDelay = weaponDelay;
            }
        }
    }
}

