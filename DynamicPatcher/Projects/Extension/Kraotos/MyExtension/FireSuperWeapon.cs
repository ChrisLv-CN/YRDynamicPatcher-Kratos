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
    public partial class TechnoExt
    {
        public FireSuperWeaponManager FireSuperWeaponManager = new FireSuperWeaponManager();
        public AEState<FireSuperType> FireSuperState => AttachEffectManager.FireSuperState;

        public unsafe void TechnoClass_Init_SuperWeapon()
        {
            if (null != Type.FireSuperData && Type.FireSuperData.Enable)
            {
                FireSuperState.Enable(Type.FireSuperData);
            }
        }

        public unsafe void TechnoClass_Update_SuperWeapon()
        {
            FireSuperWeaponManager.Update();
        }

        public unsafe void TechnoClass_OnFire_SuperWeapon(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (FireSuperState.IsActive() && null != FireSuperState.Data)
            {
                FireSuperData data = FireSuperState.Data.Data;
                if (pTechno.Ref.Veterancy.IsElite())
                {
                    data = FireSuperState.Data.EliteData;
                }
                if (null != data && (data.WeaponIndex < 0 || data.WeaponIndex == weaponIndex))
                {
                    Pointer<HouseClass> pHouse = pTechno.Ref.Owner;
                    // 检查平民
                    if (!FireSuperState.Data.DeactiveWhenCivilian || !pHouse.IsCivilian())
                    {
                        // Logger.Log($"{Game.CurrentFrame} - 发射超武 检查平民 = {FireSuperState.Data.DeactiveWhenCivilian}, 我是 {pHouse.Ref.Type.Ref.Base.ID} 平民 = {pHouse.IsCivilian()}");
                        CoordStruct targetPos = data.ToTarget ? pTarget.Ref.GetCoords() : pTechno.Ref.Base.Base.GetCoords();
                        FireSuperWeaponManager.Launch(pHouse, targetPos, data);
                    }
                }
            }
        }


    }

    public partial class TechnoTypeExt
    {
        public FireSuperType FireSuperData;

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

            FireSuperType temp = new FireSuperType();
            if (temp.TryReadType(reader, section))
            {
                FireSuperData = temp;
            }
            else
            {
                temp = null;
            }

        }
    }


    public enum PlaySuperWeaponMode
    {
        CUSTOM = 0, DONE = 1, LOOP = 2
    }

    [Serializable]
    public class PlaySuperData : FireSuperData
    {
        public PlaySuperWeaponMode LaunchMode;

        public PlaySuperData()
        {
            this.LaunchMode = PlaySuperWeaponMode.DONE;
        }

        public bool ReadPlaySuperWeapon(INIReader reader, string section)
        {
            if (TryReadType(reader, section))
            {
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
            return this.Enable;
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
                PlaySuperData data = Type.PlaySuperData;
                if (null != data && data.Enable && data.LaunchMode == PlaySuperWeaponMode.CUSTOM)
                {
                    CoordStruct targetPos = OwnerObject.Ref.Base.Base.GetCoords();
                    FireSuperWeaponManager.Order(OwnerObject.Ref.Owner, targetPos, data);
                    // Logger.Log($"{Game.CurrentFrame} - 动画 {OwnerObject} [{OwnerObject.Ref.Type.Ref.Base.Base.ID}] Update 下单投放超武 {data.Supers[0]} 所属 {OwnerObject.Ref.Owner}");
                }
            }

            FireSuperWeaponManager.Update();
        }

        public unsafe void AnimClass_Loop_SuperWeapon()
        {
            PlaySuperData data = Type.PlaySuperData;
            if (null != data && data.Enable && data.LaunchMode == PlaySuperWeaponMode.LOOP)
            {
                CoordStruct targetPos = OwnerObject.Ref.Base.Base.GetCoords();
                FireSuperWeaponManager.Launch(OwnerObject.Ref.Owner, targetPos, data);
                // Logger.Log($"{Game.CurrentFrame} - 动画 {OwnerObject} [{OwnerObject.Ref.Type.Ref.Base.Base.ID}] Loop 投放超武 {data.Supers[0]} 所属 {OwnerObject.Ref.Owner}");
            }
        }

        public unsafe void AnimClass_Done_SuperWeapon()
        {
            FireSuperWeaponManager.Reset();
            playSuperWeaponFlag = false;

            PlaySuperData data = Type.PlaySuperData;
            if (null != data && data.Enable && data.LaunchMode == PlaySuperWeaponMode.DONE)
            {
                switch (data.LaunchMode)
                {
                    case PlaySuperWeaponMode.LOOP:
                    case PlaySuperWeaponMode.DONE:
                        CoordStruct targetPos = OwnerObject.Ref.Base.Base.GetCoords();
                        FireSuperWeaponManager.Launch(OwnerObject.Ref.Owner, targetPos, data);
                        // Logger.Log($"{Game.CurrentFrame} - 动画 {OwnerObject} [{OwnerObject.Ref.Type.Ref.Base.Base.ID}] Done 投放超武 {data.Supers[0]} 所属 {OwnerObject.Ref.Owner}");
                        break;
                }
            }
        }

    }

    public partial class AnimTypeExt
    {
        public PlaySuperData PlaySuperData;

        /// <summary>
        /// [AnimType]
        /// FireSuperWeapon.LaunchMode=DONE ;发射超武的模式，DONE\LOOP\CUSTOM，DONE=动画播放结束时释放，LOOP=动画每次Loop结束时释放，CUSTOM=按照下方的控制项目来执行释放和循环
        /// FireSuperWeapon.Types=NukeSpecial,IronCurtainSpecial
        /// FireSuperWeapon.Chances=1.0,1.0 ;发射该超武的概率
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
            PlaySuperData temp = new PlaySuperData();
            if (temp.ReadPlaySuperWeapon(reader, section))
            {
                this.PlaySuperData = temp;
            }
            else
            {
                temp = null;
            }

        }
    }

}