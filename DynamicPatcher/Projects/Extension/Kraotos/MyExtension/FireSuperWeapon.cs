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
    public enum PlaySuperWeaponMode
    {
        CUSTOM = 0, DONE = 1, LOOP = 2
    }

    [Serializable]
    public class PlaySuperWeaponData : FireSuperWeaponsData
    {
        public PlaySuperWeaponMode LaunchMode;

        public PlaySuperWeaponData() : base()
        {
            this.LaunchMode = PlaySuperWeaponMode.DONE;
        }

        public void ReadPlaySuperWeapon(INIReader reader, string section)
        {
            ReadSuperWeapons(reader, section);

            string mode = null;
            if (reader.ReadNormal(section, "FireSuperWeapon.LaunchMode", ref mode))
            {
                string t = mode.Substring(0, 1).ToUpper();
                switch (t)
                {
                    case "D":
                        this.LaunchMode = PlaySuperWeaponMode.DONE;
                        break;
                    case "L":
                        this.LaunchMode = PlaySuperWeaponMode.LOOP;
                        break;
                    case "C":
                        this.LaunchMode = PlaySuperWeaponMode.CUSTOM;
                        break;
                }
            }
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
                PlaySuperWeaponData data = Type.PlaySuperWeaponData;
                if (null != data && data.Enable && data.LaunchMode == PlaySuperWeaponMode.CUSTOM)
                {
                    CoordStruct targetPos = OwnerObject.Ref.Base.Base.GetCoords();
                    FireSuperWeaponManager.Order(OwnerObject.Ref.Owner, targetPos, data);
                }
            }

            FireSuperWeaponManager.Update();
        }

        public unsafe void AnimClass_Loop_SuperWeapon()
        {
            PlaySuperWeaponData data = Type.PlaySuperWeaponData;
            if (null != data && data.Enable && data.LaunchMode == PlaySuperWeaponMode.LOOP)
            {
                CoordStruct targetPos = OwnerObject.Ref.Base.Base.GetCoords();
                FireSuperWeaponManager.Launch(OwnerObject.Ref.Owner, targetPos, data);
            }
        }

        public unsafe void AnimClass_Done_SuperWeapon()
        {
            FireSuperWeaponManager.Reset();
            playSuperWeaponFlag = false;

            PlaySuperWeaponData data = Type.PlaySuperWeaponData;
            if (null != data && data.Enable && data.LaunchMode == PlaySuperWeaponMode.DONE)
            {
                switch (data.LaunchMode)
                {
                    case PlaySuperWeaponMode.LOOP:
                    case PlaySuperWeaponMode.DONE:
                        CoordStruct targetPos = OwnerObject.Ref.Base.Base.GetCoords();
                        FireSuperWeaponManager.Launch(OwnerObject.Ref.Owner, targetPos, data);
                        break;
                }
            }
        }

    }

    public partial class AnimTypeExt
    {
        public PlaySuperWeaponData PlaySuperWeaponData;

        /// <summary>
        /// [AnimType]
        /// FireSuperWeapon.LaunchMode=DONE ;发射超武的模式，DONE\LOOP\CUSTOM，DONE=动画播放结束时释放，LOOP=动画每次Loop结束时释放，CUSTOM=按照下方的控制项目来执行释放和循环
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