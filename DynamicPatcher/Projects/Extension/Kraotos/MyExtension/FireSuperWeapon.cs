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
    public class PlaySuperWeaponData : FireSuperWeaponsData
    {
        public PlaySuperWeaponData() : base()
        {

        }

        public void ReadPlaySuperWeapon(INIReader reader, string section)
        {
            ReadSuperWeapons(reader, section);
        }
    }

    [Serializable]
    public class WeaponSuperWeaponData : FireSuperWeaponsData
    {
        public bool AnyWeapon;
        public int WeaponIndex;
        public bool ToTarget;

        public WeaponSuperWeaponData() : base()
        {
            this.AnyWeapon = true;
            this.WeaponIndex = 0;
            this.ToTarget = false;
        }

        public void ReadFireSuperWeapon(INIReader reader, string section)
        {
            ReadSuperWeapons(reader, section);

            bool anyWeapon = false;
            if (reader.ReadNormal(section, "FireSuperWeapon.AnyWeapon", ref anyWeapon))
            {
                this.AnyWeapon = anyWeapon;
            }

            int weaponIndex = 0;
            if (reader.ReadNormal(section, "FireSuperWeapon.Weapon", ref weaponIndex))
            {
                this.WeaponIndex = weaponIndex;
            }

            bool toTarget = false;
            if (reader.ReadNormal(section, "FireSuperWeapon.ToTarget", ref toTarget))
            {
                this.ToTarget = toTarget;
            }

        }
    }

    public partial class TechnoExt
    {
        public FireSuperWeaponManager FireSuperWeaponManager = new FireSuperWeaponManager();

        public unsafe void TechnoClass_Update_SuperWeapon()
        {
            FireSuperWeaponManager.Update();
        }

        public unsafe void TechnoClass_OnFire_SuperWeapon(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (null != Type.FireSuperWeaponData && Type.FireSuperWeaponData.Enable && (Type.FireSuperWeaponData.AnyWeapon || Type.FireSuperWeaponData.WeaponIndex == weaponIndex))
            {
                Pointer<HouseClass> pHouse = pTechno.Ref.Owner;
                CoordStruct targetPos = Type.FireSuperWeaponData.ToTarget ? pTarget.Ref.GetCoords() : pTechno.Ref.Base.Base.GetCoords();

                FireSuperWeaponManager.Launch(pHouse, targetPos, Type.FireSuperWeaponData);

                // foreach (string superWeaponId in Type.FireSuperWeaponData.SuperWeapons)
                // {
                //     Pointer<SuperWeaponTypeClass> pType = SuperWeaponTypeClass.ABSTRACTTYPE_ARRAY.Find(superWeaponId);
                //     if (!pType.IsNull)
                //     {
                //         Pointer<HouseClass> pHouse = pTechno.Ref.Owner;
                //         if (pHouse.IsNull || pHouse.Ref.Defeated)
                //         {
                //             // find civilian
                //             pHouse = HouseClass.FindBySideName("Civilian");
                //             if (pHouse.IsNull)
                //             {
                //                 Logger.LogWarning("Anim [{0}] want to fire a super weapon [{1}], but house is null.", pTechno.Ref.Type.Ref.Base.Base.ID, superWeaponId);
                //                 return;
                //             }
                //         }
                //         Pointer<SuperClass> pSuper = pHouse.Ref.FindSuperWeapon(pType);
                //         if (pSuper.Ref.IsCharged || !Type.FireSuperWeaponData.RealLaunch)
                //         {
                //             CoordStruct targetPos = Type.FireSuperWeaponData.ToTarget ? pTarget.Ref.GetCoords() : pTechno.Ref.Base.Base.GetCoords();
                //             CellStruct cell = MapClass.Coord2Cell(targetPos);
                //             pSuper.Ref.IsCharged = true;
                //             pSuper.Ref.Launch(cell, true);
                //             pSuper.Ref.IsCharged = false;
                //             pSuper.Ref.Reset();
                //         }
                //     }
                // }
            }
        }


    }

    public partial class TechnoTypeExt
    {
        public WeaponSuperWeaponData FireSuperWeaponData;

        /// <summary>
        /// [TechnoType]
        /// FireSuperWeapon.Types=NukeSpecial,IronCurtainSpecial
        /// FireSuperWeapon.InitDelay=0 ;延迟发射超武
        /// FireSuperWeapon.RandomInitDelay=0,15 ;随机延迟发射超武
        /// FireSuperWeapon.Delay=0 ;多次发射之间的延迟
        /// FireSuperWeapon.RandomDelay=0,15 ;多次发射之间的随机延迟
        /// FireSuperWeapon.LaunchCount=1 ;发射几次
        /// FireSuperWeapon.RealLaunch=no ;发射后超级武器进入冷却
        /// FireSuperWeapon.Weapon=0 ;哪个武器发射时发射超级武器，0主武，1副武
        /// FireSuperWeapon.AnyWeapon=no ;任意武器发射时发射超级武器
        /// FireSuperWeapon.ToTarget=yes ;朝目标位置发射超级武器
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadFireSuperWeapon(INIReader reader, string section)
        {

            string fireSuperWeapon = null;
            if (reader.ReadNormal(section, "FireSuperWeapon.Types", ref fireSuperWeapon))
            {
                this.FireSuperWeaponData = new WeaponSuperWeaponData();
                this.FireSuperWeaponData.ReadFireSuperWeapon(reader, section);
            }

        }
    }

    public partial class AnimExt
    {

        public FireSuperWeaponManager FireSuperWeaponManager = new FireSuperWeaponManager();
        private bool playSuperWeaponFlag = false;

        public unsafe void AnimClass_Update_SuperWeapon()
        {
            if (!playSuperWeaponFlag)
            {
                playSuperWeaponFlag = true;
                Pointer<AnimClass> pAnim = OwnerObject;
                CoordStruct targetPos = pAnim.Ref.Base.Base.GetCoords();
                if (null != Type.PlaySuperWeaponData && Type.PlaySuperWeaponData.Enable)
                {
                    FireSuperWeaponManager.Launch(pAnim.Ref.Owner, targetPos, Type.PlaySuperWeaponData);
                }
            }

            FireSuperWeaponManager.Update();
        }

    }

    public partial class AnimTypeExt
    {
        public PlaySuperWeaponData PlaySuperWeaponData;

        /// <summary>
        /// [AnimType]
        /// FireSuperWeapon.Types=NukeSpecial,IronCurtainSpecial
        /// FireSuperWeapon.InitDelay=0 ;延迟发射超武
        /// FireSuperWeapon.RandomInitDelay=0,15 ;随机延迟发射超武
        /// FireSuperWeapon.Delay=0 ;多次发射之间的延迟
        /// FireSuperWeapon.RandomDelay=0,15 ;多次发射之间的随机延迟
        /// FireSuperWeapon.LaunchCount=1 ;发射几次
        /// FireSuperWeapon.RealLaunch=no ;发射后超级武器进入冷却
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadFireSuperWeapon(INIReader reader, string section)
        {
            string fireSuperWeapon = null;
            if (reader.ReadNormal(section, "FireSuperWeapon.Types", ref fireSuperWeapon))
            {
                this.PlaySuperWeaponData = new PlaySuperWeaponData();
                this.PlaySuperWeaponData.ReadPlaySuperWeapon(reader, section);
            }

        }
    }

}