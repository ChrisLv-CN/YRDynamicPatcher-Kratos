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
    public class CrawlingFLHData
    {
        public CoordStruct PrimaryFireFLH;
        public CoordStruct PrimaryCrawlingFLH;
        public CoordStruct ElitePrimaryFireFLH;
        public CoordStruct ElitePrimaryCrawlingFLH;
        public CoordStruct SecondaryFireFLH;
        public CoordStruct SecondaryCrawlingFLH;
        public CoordStruct EliteSecondaryFireFLH;
        public CoordStruct EliteSecondaryCrawlingFLH;

        public void SetPrimaryFireFLH(CoordStruct primaryFLH)
        {
            this.PrimaryFireFLH = primaryFLH;
            if (default == ElitePrimaryFireFLH)
            {
                this.ElitePrimaryFireFLH = primaryFLH;
            }
        }

        public void SetPrimaryCrawlingFireFLH(CoordStruct primaryCrawlingFLH)
        {
            this.PrimaryCrawlingFLH = primaryCrawlingFLH;
            if (default == ElitePrimaryCrawlingFLH)
            {
                this.ElitePrimaryCrawlingFLH = primaryCrawlingFLH;
            }
        }

        public void SetSecondaryFireFLH(CoordStruct secondaryFLH)
        {
            this.SecondaryFireFLH = secondaryFLH;
            if (default == EliteSecondaryFireFLH)
            {
                this.EliteSecondaryFireFLH = secondaryFLH;
            }
        }

        public void SetSecondaryCrawlingFireFLH(CoordStruct secondaryCrawlingFLH)
        {
            this.SecondaryCrawlingFLH = secondaryCrawlingFLH;
            if (default == EliteSecondaryCrawlingFLH)
            {
                this.EliteSecondaryCrawlingFLH = secondaryCrawlingFLH;
            }
        }

        public override string ToString()
        {
            return String.Format("{{\"PrimaryFireFLH\":{0}, \"PrimaryCrawlingFLH\":{1}, \"ElitePrimaryFireFLH\":{2}, \"ElitePrimaryCrawlingFLH\":{3}, \"SecondaryFireFLH\":{4}, \"SecondaryCrawlingFLH\":{5}, \"EliteSecondaryFireFLH\":{6}, \"EliteSecondaryCrawlingFLH\":{7}}}",
                PrimaryFireFLH, PrimaryCrawlingFLH,
                ElitePrimaryFireFLH, ElitePrimaryCrawlingFLH,
                SecondaryFireFLH, SecondaryCrawlingFLH,
                EliteSecondaryFireFLH, EliteSecondaryCrawlingFLH
            );
        }
    }

    public partial class TechnoExt
    {
        public unsafe void InfantryClass_Init_CrawlingFLH()
        {
            if (null != Type.CrawlingFLHData && OwnerObject.Convert<InfantryClass>().Ref.Crawling)
            {
                OnUpdateAction += InfantryClass_Update_CrawlingFLH;
            }
        }

        public unsafe void InfantryClass_Update_CrawlingFLH()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            Pointer<WeaponStruct> primary = pTechno.Ref.GetWeapon(0);
            Pointer<WeaponStruct> secondary = pTechno.Ref.GetWeapon(1);

            CrawlingFLHData crawlingFLHData = Type.CrawlingFLHData;
            // Logger.Log("CrawlingFLHData = {0}", crawlingFLHData);

            if (pTechno.Convert<InfantryClass>().Ref.Crawling)
            {
                if (pTechno.Ref.Veterancy.IsElite())
                {
                    if (null != primary && !primary.IsNull)
                    {
                        primary.Ref.FLH = crawlingFLHData.ElitePrimaryCrawlingFLH;
                    }
                    if (null != secondary && !secondary.IsNull)
                    {
                        secondary.Ref.FLH = crawlingFLHData.EliteSecondaryCrawlingFLH;
                    }
                }
                else
                {
                    if (null != primary && !primary.IsNull)
                    {
                        primary.Ref.FLH = crawlingFLHData.PrimaryCrawlingFLH;
                    }
                    if (null != secondary && !secondary.IsNull)
                    {
                        secondary.Ref.FLH = crawlingFLHData.SecondaryCrawlingFLH;
                    }
                }
            }
            else
            {
                if (pTechno.Ref.Veterancy.IsElite())
                {
                    if (null != primary && !primary.IsNull)
                    {
                        primary.Ref.FLH = crawlingFLHData.ElitePrimaryFireFLH;
                    }
                    if (null != secondary && !secondary.IsNull)
                    {
                        secondary.Ref.FLH = crawlingFLHData.EliteSecondaryFireFLH;
                    }
                }
                else
                {
                    if (null != primary && !primary.IsNull)
                    {
                        primary.Ref.FLH = crawlingFLHData.PrimaryFireFLH;
                    }
                    if (null != secondary && !secondary.IsNull)
                    {
                        secondary.Ref.FLH = crawlingFLHData.SecondaryFireFLH;
                    }
                }
            }
        }


    }

    public partial class TechnoTypeExt
    {

        public CrawlingFLHData CrawlingFLHData;

        /// <summary>
        /// [TechnoTypeArt]
        /// PrimaryCrawlingFLH=150,0,225
        /// SecondaryCrawlingFLH=150,0,225
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadCrawlingFLH(INIReader reader, string section, INIReader artReader, string artSection)
        {
            bool isRead = false;
            CrawlingFLHData temp = new CrawlingFLHData();

            // CrawlingFLH from Art.ini
            CoordStruct tempFLH = default;
            if (ExHelper.ReadCoordStruct(artReader, artSection, "PrimaryFireFLH", ref tempFLH))
            {
                temp.SetPrimaryFireFLH(tempFLH);

                tempFLH = default;
                if (ExHelper.ReadCoordStruct(artReader, artSection, "PrimaryCrawlingFLH", ref tempFLH))
                {
                    temp.SetPrimaryCrawlingFireFLH(tempFLH);
                }
                else
                {
                    CoordStruct flh = temp.PrimaryFireFLH;
                    flh.Z = 20;
                    temp.SetPrimaryCrawlingFireFLH(flh);
                }
                isRead = true;
            }

            tempFLH = default;
            if (ExHelper.ReadCoordStruct(artReader, artSection, "ElitePrimaryFireFLH", ref tempFLH))
            {
                temp.ElitePrimaryFireFLH = tempFLH;

                tempFLH = default;
                if (ExHelper.ReadCoordStruct(artReader, artSection, "ElitePrimaryCrawlingFLH", ref tempFLH))
                {
                    temp.ElitePrimaryCrawlingFLH = tempFLH;
                }
                else
                {
                    CoordStruct flh = temp.ElitePrimaryFireFLH;
                    flh.Z = 20;
                    temp.ElitePrimaryCrawlingFLH = flh;
                }
                isRead = true;
            }

            tempFLH = default;
            if (ExHelper.ReadCoordStruct(artReader, artSection, "SecondaryFireFLH", ref tempFLH))
            {
                temp.SetSecondaryFireFLH(tempFLH);

                tempFLH = default;
                if (ExHelper.ReadCoordStruct(artReader, artSection, "SecondaryCrawlingFLH", ref tempFLH))
                {
                    temp.SetSecondaryCrawlingFireFLH(tempFLH);
                }
                else
                {
                    CoordStruct flh = temp.SecondaryFireFLH;
                    flh.Z = 20;
                    temp.SetSecondaryCrawlingFireFLH(flh);
                }
                isRead = true;
            }


            tempFLH = default;
            if (ExHelper.ReadCoordStruct(artReader, artSection, "EliteSecondaryFireFLH", ref tempFLH))
            {
                temp.EliteSecondaryFireFLH = tempFLH;

                tempFLH = default;
                if (ExHelper.ReadCoordStruct(artReader, artSection, "EliteSecondaryCrawlingFLH", ref tempFLH))
                {
                    temp.EliteSecondaryCrawlingFLH = tempFLH;
                }
                else
                {
                    CoordStruct flh = temp.EliteSecondaryFireFLH;
                    flh.Z = 20;
                    temp.EliteSecondaryCrawlingFLH = flh;
                }
                isRead = true;
            }
            if (isRead)
            {
                this.CrawlingFLHData = temp;
            }

        }
    }

}