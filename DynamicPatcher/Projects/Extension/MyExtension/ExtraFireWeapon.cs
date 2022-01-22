using System.Threading;
using System.IO;
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
    public class ExtraFireROF
    {
        private TimerStruct timer;

        public bool CanFire(int ROF)
        {
            if (timer.Expired())
            {
                timer.Start(ROF);
                return true;
            }
            return false;
        }
    }

    [Serializable]
    public class ExtraFireFLHData
    {

        public CoordStruct PrimaryWeaponFLH;
        public CoordStruct ElitePrimaryWeaponFLH;
        public CoordStruct SecondaryWeaponFLH;
        public CoordStruct EliteSecondaryWeaponFLH;

        public Dictionary<int, CoordStruct> WeaponXFLH = new Dictionary<int, CoordStruct>();
        public Dictionary<int, CoordStruct> EliteWeaponXFLH = new Dictionary<int, CoordStruct>();

        public void SetAttachPrimaryFLH(CoordStruct flh)
        {
            PrimaryWeaponFLH = flh;
            if (default == ElitePrimaryWeaponFLH)
            {
                ElitePrimaryWeaponFLH = flh;
            }
        }

        public void SetAttachSecondaryFLH(CoordStruct flh)
        {
            SecondaryWeaponFLH = flh;
            if (default == EliteSecondaryWeaponFLH)
            {
                EliteSecondaryWeaponFLH = flh;
            }
        }

        public void AddWeaponXFLH(int index, CoordStruct flh)
        {
            if (WeaponXFLH.ContainsKey(index))
            {
                WeaponXFLH[index] = flh;
            }
            else
            {
                WeaponXFLH.Add(index, flh);
            }
            if (!EliteWeaponXFLH.ContainsKey(index))
            {
                EliteWeaponXFLH.Add(index, flh);
            }
        }

        public void AddEliteWeaponXFLH(int index, CoordStruct flh)
        {
            if (EliteWeaponXFLH.ContainsKey(index))
            {
                EliteWeaponXFLH[index] = flh;
            }
            else
            {
                EliteWeaponXFLH.Add(index, flh);
            }
        }
    }


    [Serializable]
    public class ExtraFireData
    {
        public bool Enable;
        public List<string> PrimaryWeapons;
        // public CoordStruct PrimaryWeaponFLH;
        public List<string> SecondaryWeapons;
        // public CoordStruct SecondaryWeaponFLH;
        public List<string> ElitePrimaryWeapons;
        // public CoordStruct ElitePrimaryWeaponFLH;
        public List<string> EliteSecondaryWeapons;
        // public CoordStruct EliteSecondaryWeaponFLH;

        public Dictionary<int, List<string>> WeaponX;
        public Dictionary<int, List<string>> EliteWeaponX;

        public ExtraFireData(bool enable)
        {
            this.Enable = enable;
        }
    }

    public partial class TechnoExt
    {
        Dictionary<string, TimerStruct> extraFireROF = new Dictionary<string, TimerStruct>();

        public unsafe void TechnoClass_OnFire_ExtraFireWeapon(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            if (null != Type.ExtraFireData && Type.ExtraFireData.Enable)
            {
                // 获取武器清单和FLH设置
                List<string> weapons = null;
                CoordStruct customFLH = default;
                ExtraFireFLHData flhData = Type.ExtraFireFLHData;
                // in Transport
                Pointer<TechnoClass> pTransporter = OwnerObject.Ref.Transporter;
                if (!pTransporter.IsNull)
                {
                    TechnoExt transporterExt = TechnoExt.ExtMap.Find(pTransporter);
                    if (null != transporterExt)
                    {
                        flhData = transporterExt.Type.ExtraFireFLHData;
                    }
                }
                if (!OwnerObject.Ref.Veterancy.IsElite())
                {
                    // 检查WeaponX
                    if (OwnerObject.Ref.Type.Ref.WeaponCount > 0)
                    {
                        if (null != Type.ExtraFireData.WeaponX)
                        {
                            Type.ExtraFireData.WeaponX.TryGetValue(weaponIndex, out weapons);
                            flhData.WeaponXFLH.TryGetValue(weaponIndex, out customFLH);
                        }

                    }
                    else if (weaponIndex == 0)
                    {
                        weapons = Type.ExtraFireData.PrimaryWeapons;
                        customFLH = flhData.PrimaryWeaponFLH;
                    }
                    else if (weaponIndex == 1)
                    {
                        weapons = Type.ExtraFireData.SecondaryWeapons;
                        customFLH = flhData.SecondaryWeaponFLH;
                    }
                }
                else
                {
                    // 检查WeaponX
                    if (OwnerObject.Ref.Type.Ref.WeaponCount > 0)
                    {
                        if (null != Type.ExtraFireData.EliteWeaponX)
                        {
                            Type.ExtraFireData.WeaponX.TryGetValue(weaponIndex, out weapons);
                            flhData.WeaponXFLH.TryGetValue(weaponIndex, out customFLH);
                        }
                    }
                    else if (weaponIndex == 0)
                    {
                        weapons = Type.ExtraFireData.ElitePrimaryWeapons;
                        customFLH = flhData.ElitePrimaryWeaponFLH;
                    }
                    else if (weaponIndex == 1)
                    {
                        weapons = Type.ExtraFireData.EliteSecondaryWeapons;
                        customFLH = flhData.EliteSecondaryWeaponFLH;
                    }
                }

                if (null != weapons)
                {

                    // bool rofAbility = false;
                    // if (OwnerObject.Ref.Veterancy.IsElite())
                    // {
                    //     rofAbility = OwnerObject.Ref.Type.Ref.VeteranAbilities.ROF || OwnerObject.Ref.Type.Ref.EliteAbilities.ROF;
                    // }
                    // else if (OwnerObject.Ref.Veterancy.IsVeteran())
                    // {
                    //     rofAbility = OwnerObject.Ref.Type.Ref.VeteranAbilities.ROF;
                    // }
                    // double rofMult = !rofAbility ? 1.0 : RulesClass.Global().VeteranROF * ((OwnerObject.Ref.Owner.IsNull || OwnerObject.Ref.Owner.Ref.Type.IsNull) ? 1.0 : OwnerObject.Ref.Owner.Ref.Type.Ref.ROFMult);

                    double rofMult = ExHelper.GetROFMult(OwnerObject);

                    CoordStruct flh = OwnerObject.Ref.GetWeapon(weaponIndex).Ref.FLH;
                    if (customFLH != default)
                    {
                        flh = customFLH;
                    }
                    // 循环武器清单并发射
                    foreach (string weaponId in weapons)
                    {
                        // 进行ROF检查
                        Pointer<WeaponTypeClass> pWeapon = WeaponTypeClass.ABSTRACTTYPE_ARRAY.Find(weaponId);
                        if (!pWeapon.IsNull)
                        {
                            bool canFire = true;
                            WeaponTypeExt typeExt = WeaponTypeExt.ExtMap.Find(pWeapon);
                            if (null != typeExt)
                            {
                                AttachFireData fireData = typeExt.AttachFireData;
                                // 进行ROF检查
                                canFire = !fireData.UseROF;
                                if (!canFire)
                                {
                                    // 本次发射的rof
                                    int rof = (int)(pWeapon.Ref.ROF * rofMult);
                                    if (extraFireROF.TryGetValue(weaponId, out TimerStruct rofTimer))
                                    {
                                        if (rofTimer.Expired())
                                        {
                                            canFire = true;
                                            rofTimer.Start(rof);
                                            extraFireROF[weaponId] = rofTimer;
                                        }
                                    }
                                    else
                                    {
                                        canFire = true;
                                        extraFireROF.Add(weaponId, new TimerStruct(rof));
                                    }
                                }
                            }
                            if (canFire)
                            {
                                FireCustomWeapon(OwnerObject, OwnerObject, pTarget, weaponId, flh, default, rofMult);
                            }
                        }
                    }
                }
            }
        }

    }

    public partial class TechnoTypeExt
    {

        public ExtraFireFLHData ExtraFireFLHData = new ExtraFireFLHData();
        public ExtraFireData ExtraFireData;

        /// <summary>
        /// [TechnoType]
        /// ExtraFire.Primary=RedEye2
        /// ExtraFire.ElitePrimary=RedEye2
        /// ExtraFire.Secondary=RedEye2
        /// ExtraFire.EliteSecondary=RedEye2
        /// ExtraFire.WeaponX=RedEye2
        /// ExtraFire.EliteWeaponX=RedEye2
        /// 
        /// [TechnoTypeArt]
        /// ExtraFire.PrimaryFLH=0,0,0
        /// ExtraFire.ElitePrimaryFLH=0,0,0
        /// ExtraFire.SecondaryFLH=0,0,0
        /// ExtraFire.EliteSecondaryFLH=0,0,0
        /// ExtraFire.WeaponXFLH=0,0,0
        /// ExtraFire.EliteWeaponXFLH=0,0,0
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadExtraFireWeapon(INIReader reader, string section)
        {

            // read flh
            INIReader artReader = reader;
            if (null != CCINIClass.INI_Art && !CCINIClass.INI_Art.IsNull)
            {
                artReader = new INIReader(CCINIClass.INI_Art);
            }
            string artSection = section;
            string image = default;
            if (reader.ReadNormal(section, "Image", ref image))
            {
                artSection = image;
            }
            CoordStruct tempFLH = default;
            if (ExHelper.ReadCoordStruct(artReader, artSection, "ExtraFire.PrimaryFLH", ref tempFLH))
            {
                ExtraFireFLHData.SetAttachPrimaryFLH(tempFLH);
            }

            tempFLH = default;
            if (ExHelper.ReadCoordStruct(artReader, artSection, "ExtraFire.ElitePrimaryFLH", ref tempFLH))
            {
                ExtraFireFLHData.ElitePrimaryWeaponFLH = tempFLH;
            }

            tempFLH = default;
            if (ExHelper.ReadCoordStruct(artReader, artSection, "ExtraFire.SecondaryFLH", ref tempFLH))
            {
                ExtraFireFLHData.SetAttachSecondaryFLH(tempFLH);
            }

            tempFLH = default;
            if (ExHelper.ReadCoordStruct(artReader, artSection, "ExtraFire.EliteSecondaryFLH", ref tempFLH))
            {
                ExtraFireFLHData.EliteSecondaryWeaponFLH = tempFLH;
            }

            for (int i = 0; i < 127; i++)
            {
                CoordStruct flh = default;
                if (ExHelper.ReadCoordStruct(artReader, artSection, "ExtraFire.Weapon" + (i + 1) + "FLH", ref flh))
                {
                    ExtraFireFLHData.AddWeaponXFLH(i, flh);
                }
                flh = default;
                if (ExHelper.ReadCoordStruct(artReader, artSection, "ExtraFire.EliteWeapon" + (i + 1) + "FLH", ref flh))
                {
                    ExtraFireFLHData.AddEliteWeaponXFLH(i, flh);
                }
            }

            // read attach weapons
            List<string> primary = null;
            if (ExHelper.ReadList(reader, section, "ExtraFire.Primary", ref primary))
            {
                if (null == ExtraFireData)
                {
                    ExtraFireData = new ExtraFireData(true);
                }
                ExtraFireData.PrimaryWeapons = primary;
            }

            List<string> secondary = null;
            if (ExHelper.ReadList(reader, section, "ExtraFire.Secondary", ref secondary))
            {
                if (null == ExtraFireData)
                {
                    ExtraFireData = new ExtraFireData(true);
                }
                ExtraFireData.SecondaryWeapons = secondary;
            }

            List<string> elitePrimary = null;
            if (ExHelper.ReadList(reader, section, "ExtraFire.ElitePrimary", ref elitePrimary))
            {
                if (null == ExtraFireData)
                {
                    ExtraFireData = new ExtraFireData(true);
                }
                ExtraFireData.ElitePrimaryWeapons = elitePrimary;
            }

            List<string> eliteSecondary = null;
            if (ExHelper.ReadList(reader, section, "ExtraFire.EliteSecondary", ref eliteSecondary))
            {
                if (null == ExtraFireData)
                {
                    ExtraFireData = new ExtraFireData(true);
                }
                ExtraFireData.EliteSecondaryWeapons = eliteSecondary;
            }

            Dictionary<int, List<string>> weaponIndex = null;
            Dictionary<int, List<string>> eliteWeaponIndex = null;
            for (int i = 0; i < 127; i++)
            {
                List<string> weapons = null;
                if (ExHelper.ReadList(reader, section, "ExtraFire.Weapon" + (i + 1), ref weapons))
                {
                    if (null == ExtraFireData)
                    {
                        ExtraFireData = new ExtraFireData(true);
                    }
                    if (null == weaponIndex)
                    {
                        weaponIndex = new Dictionary<int, List<string>>();
                    }
                    weaponIndex.Add(i, weapons);
                }

                weapons = null;
                if (ExHelper.ReadList(reader, section, "ExtraFire.EliteWeapon" + (i + 1), ref weapons))
                {
                    if (null == ExtraFireData)
                    {
                        ExtraFireData = new ExtraFireData(true);
                    }
                    if (null == eliteWeaponIndex)
                    {
                        eliteWeaponIndex = new Dictionary<int, List<string>>();
                    }
                    eliteWeaponIndex.Add(i, weapons);
                }
            }
            if (null != weaponIndex)
            {
                ExtraFireData.WeaponX = weaponIndex;
            }
            if (null != eliteWeaponIndex)
            {
                ExtraFireData.EliteWeaponX = eliteWeaponIndex;
            }
        }
    }

}