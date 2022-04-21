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
        public AutoWeaponType AutoWeaponType;

        private void ReadAutoWeaponType(INIReader reader, string section)
        {
            AutoWeaponType type = new AutoWeaponType();
            if (type.TryReadType(reader, section))
            {
                this.AutoWeaponType = type;
            }
        }
    }

    /// <summary>
    /// 自动武器类型
    /// </summary>
    [Serializable]
    public class AutoWeaponType : EffectType<AutoWeapon>
    {
        public int WeaponIndex; // 使用单位自身的武器
        public int EliteWeaponIndex; // 精英时使用单位自身的武器
        public List<string> WeaponTypes; // 武器类型
        public List<string> EliteWeaponTypes; // 精英武器类型
        public int RandomTypesNum; // 随机使用几个武器
        public int EliteRandomTypesNum; // 精英时随机使用几个武器
        public bool FireOnce; // 发射后销毁
        public CoordStruct FireFLH; // 开火相对位置
        public CoordStruct EliteFireFLH; // 精英开火相对位置
        public CoordStruct TargetFLH; // 目标相对位置
        public CoordStruct EliteTargetFLH; // 精英目标相对位置
        public CoordStruct MoveTo; // 以开火位置为坐标0点，计算TargetFLH
        public CoordStruct EliteMoveTo; // 以开火位置为坐标0点，计算EliteTargetFLH
        public bool FireToTarget; // 朝附加对象的目标开火，如果附加的对象没有目标，不开火
        public bool IsOnTurret; // 相对炮塔或者身体
        public bool IsOnWorld; // 相对世界

        // 攻击者标记
        public bool IsAttackerMark; // 允许附加对象和攻击者进行交互
        public bool ReceiverAttack; // 武器由AE的接受者发射
        public bool ReceiverOwnBullet; // 武器所属是AE的接受者

        public AutoWeaponType()
        {
            this.WeaponIndex = -1;
            this.EliteWeaponIndex = -1;
            this.WeaponTypes = null;
            this.EliteWeaponTypes = null;
            this.RandomTypesNum = 0;
            this.EliteRandomTypesNum = 0;
            this.FireOnce = false;
            this.FireFLH = default;
            this.EliteFireFLH = default;
            this.TargetFLH = default;
            this.EliteTargetFLH = default;
            this.MoveTo = default;
            this.EliteMoveTo = default;
            this.FireToTarget = false;
            this.IsOnTurret = true;
            this.IsOnWorld = false;

            this.IsAttackerMark = false;
            this.ReceiverAttack = true;
            this.ReceiverOwnBullet = true;
        }


        public override bool TryReadType(INIReader reader, string section)
        {

            ReadCommonType(reader, section, "AutoWeapon.");

            int weaponIdx = -1;
            if (reader.ReadNormal(section, "AutoWeapon.WeaponIndex", ref weaponIdx))
            {
                if (weaponIdx > -1)
                {
                    this.Enable = true;
                    this.WeaponIndex = weaponIdx;
                    this.EliteWeaponIndex = weaponIdx;
                }
            }

            int eliteWeaponIdx = -1;
            if (reader.ReadNormal(section, "AutoWeapon.EliteWeaponIndex", ref eliteWeaponIdx))
            {
                if (eliteWeaponIdx > -1)
                {
                    this.Enable = true;
                    this.EliteWeaponIndex = eliteWeaponIdx;
                }
            }

            List<string> weaponTypes = null;
            if (reader.ReadStringList(section, "AutoWeapon.Types", ref weaponTypes))
            {
                // 排除掉none
                if (weaponTypes.Count > 0 && !weaponTypes[0].ToLower().Equals("none"))
                {
                    this.Enable = true;
                    this.WeaponTypes = weaponTypes;
                    this.EliteWeaponTypes = weaponTypes;
                }
            }

            List<string> eliteTypes = null;
            if (reader.ReadStringList(section, "AutoWeapon.EliteTypes", ref eliteTypes))
            {
                // 排除掉none
                if (weaponTypes.Count > 0 && !weaponTypes[0].ToLower().Equals("none"))
                {
                    this.Enable = true;
                    this.EliteWeaponTypes = eliteTypes;
                }
            }

            if (this.Enable)
            {
                int randomTypesNum = 0;
                if (reader.ReadNormal(section, "AutoWeapon.RandomTypesNum", ref randomTypesNum))
                {
                    if (randomTypesNum > 0)
                    {
                        this.RandomTypesNum = randomTypesNum;
                        this.EliteRandomTypesNum = randomTypesNum;
                    }
                }

                int eliteRandomTypesNum = 0;
                if (reader.ReadNormal(section, "AutoWeapon.EliteRandomTypesNum", ref eliteRandomTypesNum))
                {
                    if (eliteRandomTypesNum > 0)
                    {
                        this.EliteRandomTypesNum = eliteRandomTypesNum;
                    }
                }

                bool fireOnce = false;
                if (reader.ReadNormal(section, "AutoWeapon.FireOnce", ref fireOnce))
                {
                    this.FireOnce = fireOnce;
                }

                CoordStruct fireFLH = default;
                if (ExHelper.ReadCoordStruct(reader, section, "AutoWeapon.FireFLH", ref fireFLH))
                {
                    this.FireFLH = fireFLH;
                    this.EliteFireFLH = fireFLH;
                }

                CoordStruct eliteFireFLH = default;
                if (ExHelper.ReadCoordStruct(reader, section, "AutoWeapon.EliteFireFLH", ref eliteFireFLH))
                {
                    this.EliteFireFLH = eliteFireFLH;
                }

                CoordStruct targetFLH = default;
                if (ExHelper.ReadCoordStruct(reader, section, "AutoWeapon.TargetFLH", ref targetFLH))
                {
                    this.TargetFLH = targetFLH;
                    this.EliteTargetFLH = targetFLH;
                }

                CoordStruct eliteTargetFLH = default;
                if (ExHelper.ReadCoordStruct(reader, section, "AutoWeapon.EliteTargetFLH", ref eliteTargetFLH))
                {
                    this.EliteTargetFLH = eliteTargetFLH;
                }

                CoordStruct moveTo = default;
                if (ExHelper.ReadCoordStruct(reader, section, "AutoWeapon.MoveTo", ref moveTo))
                {
                    this.MoveTo = moveTo;
                    this.EliteMoveTo = moveTo;
                    this.TargetFLH = this.FireFLH + moveTo;
                    this.EliteTargetFLH = this.EliteFireFLH + moveTo;
                }

                CoordStruct eliteMoveTo = default;
                if (ExHelper.ReadCoordStruct(reader, section, "AutoWeapon.EliteMoveTo", ref eliteMoveTo))
                {
                    this.EliteMoveTo = eliteMoveTo;
                    this.EliteTargetFLH = this.EliteFireFLH + eliteMoveTo;
                }

                bool fireToTarget = false;
                if (reader.ReadNormal(section, "AutoWeapon.FireToTarget", ref fireToTarget))
                {
                    this.FireToTarget = fireToTarget;
                }

                bool isOnTurret = true;
                if (reader.ReadNormal(section, "AutoWeapon.IsOnTurret", ref isOnTurret))
                {
                    this.IsOnTurret = isOnTurret;
                }

                bool isOnWorld = true;
                if (reader.ReadNormal(section, "AutoWeapon.IsOnWorld", ref isOnWorld))
                {
                    this.IsOnWorld = isOnWorld;
                }

                // 攻击者标记
                bool isAttackerMark = false;
                if (reader.ReadNormal(section, "AutoWeapon.IsAttackerMark", ref isAttackerMark))
                {
                    this.IsAttackerMark = isAttackerMark;
                }

                bool receiverAttack = false;
                if (reader.ReadNormal(section, "AutoWeapon.ReceiverAttack", ref receiverAttack))
                {
                    this.ReceiverAttack = receiverAttack;
                    if (!receiverAttack)
                    {
                        this.ReceiverOwnBullet = false;
                    }
                }

                bool receiverOwnBullet = false;
                if (reader.ReadNormal(section, "AutoWeapon.ReceiverOwnBullet", ref receiverOwnBullet))
                {
                    this.ReceiverOwnBullet = receiverOwnBullet;
                }
            }
            return this.Enable;
        }

    }

}