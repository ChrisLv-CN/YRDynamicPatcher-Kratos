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
    public class SpawnSupportFLHData
    {
        public CoordStruct SpawnSupportFLH;
        public CoordStruct EliteSpawnSupportFLH;

        public CoordStruct SpawnHitFLH;
        public CoordStruct EliteSpawnHitFLH;

        public void SetSpawnSupportFLH(CoordStruct flh)
        {
            SpawnSupportFLH = flh;
            if (default == EliteSpawnSupportFLH)
            {
                EliteSpawnSupportFLH = flh;
            }
        }

        public void SetSpawnHitFLH(CoordStruct flh)
        {
            SpawnHitFLH = flh;
            if (default == EliteSpawnHitFLH)
            {
                EliteSpawnHitFLH = flh;
            }
        }
    }

    [Serializable]
    public class SpawnSupportData
    {
        public bool Enable;
        public string SupportWeapon;
        public string EliteSupportWeapon;
        public bool SwitchFLH;
        public bool Always;

        public SpawnSupportData(bool enable)
        {
            this.Enable = enable;
            this.SupportWeapon = null;
            this.EliteSupportWeapon = null;
            this.SwitchFLH = false;
            this.Always = false;
        }

    }

    public partial class TechnoExt
    {
        private int supportFLHMult = 1;
        private TimerStruct supportFireROF = new TimerStruct(0);


        public unsafe void TechnoClass_Update_SpawnSupport()
        {
            Pointer<TechnoClass> pSpawnOwner = OwnerObject.Ref.SpawnOwner;
            if (!pSpawnOwner.IsNull)
            {
                TechnoTypeExt extType = TechnoExt.ExtMap.Find(pSpawnOwner).Type;
                if (null != extType.SpawnSupportData && extType.SpawnSupportData.Enable && !String.IsNullOrEmpty(extType.SpawnSupportData.SupportWeapon) && extType.SpawnSupportData.Always)
                {
                    FireSupportWeaponToSpawn(pSpawnOwner, extType.SpawnSupportData, FindSupportWeaponFLHData(extType, pSpawnOwner), true);
                }
            }
        }


        public unsafe void TechnoClass_OnFire_SpawnSupport(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            Pointer<TechnoClass> pSpawnOwner = OwnerObject.Ref.SpawnOwner;
            if (!pSpawnOwner.IsNull)
            {
                TechnoTypeExt extType = TechnoExt.ExtMap.Find(pSpawnOwner).Type;
                if (null != extType.SpawnSupportData && extType.SpawnSupportData.Enable && !String.IsNullOrEmpty(extType.SpawnSupportData.SupportWeapon) && !extType.SpawnSupportData.Always)
                {
                    FireSupportWeaponToSpawn(pSpawnOwner, extType.SpawnSupportData, FindSupportWeaponFLHData(extType, pSpawnOwner));
                }
            }

        }

        private SpawnSupportFLHData FindSupportWeaponFLHData(TechnoTypeExt extType, Pointer<TechnoClass> pSpawnOwner)
        {
            Pointer<TechnoClass> pTransporter = pSpawnOwner.Ref.Transporter;
            if (!pTransporter.IsNull)
            {
                TechnoExt ext = TechnoExt.ExtMap.Find(pTransporter);
                if (null != ext)
                {
                    return ext.Type.SpawnSupportFLHData;
                }
            }
            return extType.SpawnSupportFLHData;
        }

        private void FireSupportWeaponToSpawn(Pointer<TechnoClass> pSpawnOwner, SpawnSupportData spawnSupportData, SpawnSupportFLHData spawnSupportFLHData, bool useROF = false)
        {
            String weaponID = spawnSupportData.SupportWeapon;
            CoordStruct flh = spawnSupportFLHData.SpawnSupportFLH;
            CoordStruct hitFLH = spawnSupportFLHData.SpawnHitFLH;
            if (pSpawnOwner.Ref.Veterancy.IsElite())
            {
                weaponID = spawnSupportData.EliteSupportWeapon;
                flh = spawnSupportFLHData.EliteSpawnSupportFLH;
                hitFLH = spawnSupportFLHData.EliteSpawnHitFLH;
            }
            if (spawnSupportData.SwitchFLH)
            {
                flh.Y = flh.Y * supportFLHMult;
                supportFLHMult *= -1;
            }
            Pointer<WeaponTypeClass> pWeapon = WeaponTypeClass.ABSTRACTTYPE_ARRAY.Find(weaponID);
            if (!pWeapon.IsNull)
            {
                if (useROF && supportFireROF.InProgress())
                {
                    return;
                }
                // get source location
                CoordStruct sourcePos = ExHelper.GetFLHAbsoluteCoords(pSpawnOwner, flh, true);
                // Logger.Log("Support Weapon FLH = {0}, hitFLH = {1}", flh, hitFLH);
                // get target location
                CoordStruct targetPos = ExHelper.GetFLHAbsoluteCoords(OwnerObject, hitFLH, true);
                // get bullet velocity
                BulletVelocity bulletVelocity = ExHelper.GetBulletVelocity(sourcePos, targetPos);
                // fire weapon
                ExHelper.FireBulletTo(pSpawnOwner, pSpawnOwner, OwnerObject.Convert<AbstractClass>(), pWeapon, sourcePos, targetPos, bulletVelocity);
                if (useROF)
                {
                    supportFireROF.Start(pWeapon.Ref.ROF);
                }
            }
        }


    }

    public partial class TechnoTypeExt
    {
        public SpawnSupportFLHData SpawnSupportFLHData = new SpawnSupportFLHData();
        public SpawnSupportData SpawnSupportData;

        /// <summary>
        /// [TechnoType]
        /// SupportSpawns=yes
        /// SupportSpawns.Weapon=BotLaserSupport
        /// SupportSpawns.EliteWeapon=BotLaserSupport
        /// SupportSpawns.SwitchFLH=yes
        /// SupportSpawns.AlwaysFire=no
        /// 
        /// [TechnoTypeArt]
        /// SupportWeaponFLH=0,0,0
        /// EliteSupportWeaponFLH=0,0,0
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        private void ReadSpawnSupport(INIReader reader, string section, INIReader artReader, string artSection)
        {
            // read flh from Carrier's Art.ini
            CoordStruct tempFLH = default;
            if (ExHelper.ReadCoordStruct(artReader, artSection, "SupportWeaponFLH", ref tempFLH))
            {
                SpawnSupportFLHData.SetSpawnSupportFLH(tempFLH);
            }

            tempFLH = default;
            if (ExHelper.ReadCoordStruct(artReader, artSection, "EliteSupportWeaponFLH", ref tempFLH))
            {
                SpawnSupportFLHData.EliteSpawnSupportFLH = tempFLH;
            }

            tempFLH = default;
            if (ExHelper.ReadCoordStruct(artReader, artSection, "SupportWeaponHitFLH", ref tempFLH))
            {
                SpawnSupportFLHData.SetSpawnHitFLH(tempFLH);
            }

            tempFLH = default;
            if (ExHelper.ReadCoordStruct(artReader, artSection, "EliteSupportWeaponHitFLH", ref tempFLH))
            {
                SpawnSupportFLHData.EliteSpawnHitFLH = tempFLH;
            }


            // read SupportSetting form Carrier
            bool supportSpawns = false;
            if (reader.ReadNormal(section, "SupportSpawns", ref supportSpawns))
            {
                SpawnSupportData = new SpawnSupportData(supportSpawns);
                string weaponName = null;
                if (reader.ReadNormal(section, "SupportSpawns.Weapon", ref weaponName))
                {
                    SpawnSupportData.SupportWeapon = weaponName;
                    if (reader.ReadNormal(section, "SupportSpawns.EliteWeapon", ref weaponName))
                    {
                        SpawnSupportData.EliteSupportWeapon = weaponName;
                    }
                    else
                    {
                        SpawnSupportData.EliteSupportWeapon = SpawnSupportData.SupportWeapon;
                    }
                    bool switchFLH = false;
                    if (reader.ReadNormal(section, "SupportSpawns.SwitchFLH", ref switchFLH))
                    {
                        SpawnSupportData.SwitchFLH = switchFLH;
                    }
                    bool always = false;
                    if (reader.ReadNormal(section, "SupportSpawns.AlwaysFire", ref always))
                    {
                        SpawnSupportData.Always = always;
                    }
                }
            }
        }
    }
}