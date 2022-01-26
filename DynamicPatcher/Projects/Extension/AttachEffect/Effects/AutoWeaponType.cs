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
            if (AutoWeaponType.ReadAutoWeaponType(reader, section, out AutoWeaponType autoWeaponType))
            {
                this.AutoWeaponType = autoWeaponType;
            }
        }
    }

    /// <summary>
    /// 自动武器类型
    /// </summary>
    [Serializable]
    public class AutoWeaponType : IEffectType<AutoWeapon>
    {
        public int WeaponIndex; // 使用单位自身的武器
        public int EliteWeaponIndex; // 精英时使用单位自身的武器
        public List<string> WeaponTypes; // 武器类型
        public List<string> EliteWeaponTypes; // 精英武器类型
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

        public AutoWeapon CreateObject(AttachEffectType attachEffectType)
        {
            if (WeaponIndex > -1 || EliteWeaponIndex > -1 || null != WeaponTypes || null != EliteWeaponTypes)
            {
                return new AutoWeapon(this, attachEffectType);
            }
            return null;
        }

        public static bool ReadAutoWeaponType(INIReader reader, string section, out AutoWeaponType autoWeaponType)
        {
            autoWeaponType = null;

            int weaponIdx = -1;
            if (reader.ReadNormal(section, "AutoWeapon.WeaponIndex", ref weaponIdx))
            {
                if (weaponIdx > -1)
                {
                    if (null == autoWeaponType)
                    {
                        autoWeaponType = new AutoWeaponType();
                    }
                    autoWeaponType.WeaponIndex = weaponIdx;
                    autoWeaponType.EliteWeaponIndex = weaponIdx;
                }
            }

            int eliteWeaponIdx = -1;
            if (reader.ReadNormal(section, "AutoWeapon.EliteWeaponIndex", ref eliteWeaponIdx))
            {
                if (eliteWeaponIdx > -1)
                {
                    if (null == autoWeaponType)
                    {
                        autoWeaponType = new AutoWeaponType();
                    }
                    autoWeaponType.EliteWeaponIndex = eliteWeaponIdx;
                }
            }

            List<string> weaponTypes = null;
            if (ExHelper.ReadList(reader, section, "AutoWeapon.Types", ref weaponTypes))
            {
                // 排除掉none
                if (weaponTypes.Count > 0 && !weaponTypes[0].ToLower().Equals("none"))
                {
                    if (null == autoWeaponType)
                    {
                        autoWeaponType = new AutoWeaponType();
                    }
                    autoWeaponType.WeaponTypes = weaponTypes;
                    autoWeaponType.EliteWeaponTypes = weaponTypes;
                }
            }

            List<string> eliteTypes = null;
            if (ExHelper.ReadList(reader, section, "AutoWeapon.EliteTypes", ref eliteTypes))
            {
                // 排除掉none
                if (weaponTypes.Count > 0 && !weaponTypes[0].ToLower().Equals("none"))
                {
                    if (null == autoWeaponType)
                    {
                        autoWeaponType = new AutoWeaponType();
                    }
                    autoWeaponType.EliteWeaponTypes = eliteTypes;
                }
            }

            if (null != autoWeaponType)
            {
                bool fireOnce = false;
                if (reader.ReadNormal(section, "AutoWeapon.FireOnce", ref fireOnce))
                {
                    autoWeaponType.FireOnce = fireOnce;
                }

                CoordStruct fireFLH = default;
                if (ExHelper.ReadCoordStruct(reader, section, "AutoWeapon.FireFLH", ref fireFLH))
                {
                    autoWeaponType.FireFLH = fireFLH;
                    autoWeaponType.EliteFireFLH = fireFLH;
                }

                CoordStruct eliteFireFLH = default;
                if (ExHelper.ReadCoordStruct(reader, section, "AutoWeapon.EliteFireFLH", ref eliteFireFLH))
                {
                    autoWeaponType.EliteFireFLH = eliteFireFLH;
                }

                CoordStruct targetFLH = default;
                if (ExHelper.ReadCoordStruct(reader, section, "AutoWeapon.TargetFLH", ref targetFLH))
                {
                    autoWeaponType.TargetFLH = targetFLH;
                    autoWeaponType.EliteTargetFLH = targetFLH;
                }

                CoordStruct eliteTargetFLH = default;
                if (ExHelper.ReadCoordStruct(reader, section, "AutoWeapon.EliteTargetFLH", ref eliteTargetFLH))
                {
                    autoWeaponType.EliteTargetFLH = eliteTargetFLH;
                }

                CoordStruct moveTo = default;
                if (ExHelper.ReadCoordStruct(reader, section, "AutoWeapon.MoveTo", ref moveTo))
                {
                    autoWeaponType.MoveTo = moveTo;
                    autoWeaponType.EliteMoveTo = moveTo;
                    autoWeaponType.TargetFLH = autoWeaponType.FireFLH + moveTo;
                    autoWeaponType.EliteTargetFLH = autoWeaponType.EliteFireFLH + moveTo;
                }

                CoordStruct eliteMoveTo = default;
                if (ExHelper.ReadCoordStruct(reader, section, "AutoWeapon.EliteMoveTo", ref eliteMoveTo))
                {
                    autoWeaponType.EliteMoveTo = eliteMoveTo;
                    autoWeaponType.EliteTargetFLH = autoWeaponType.EliteFireFLH + eliteMoveTo;
                }

                bool fireToTarget = false;
                if (reader.ReadNormal(section, "AutoWeapon.FireToTarget", ref fireToTarget))
                {
                    autoWeaponType.FireToTarget = fireToTarget;
                }

                bool isOnTurret = true;
                if (reader.ReadNormal(section, "AutoWeapon.IsOnTurret", ref isOnTurret))
                {
                    autoWeaponType.IsOnTurret = isOnTurret;
                }

                bool isOnWorld = true;
                if (reader.ReadNormal(section, "AutoWeapon.IsOnWorld", ref isOnWorld))
                {
                    autoWeaponType.IsOnWorld = isOnWorld;
                }

                // 攻击者标记
                bool isAttackerMark = false;
                if (reader.ReadNormal(section, "AutoWeapon.IsAttackerMark", ref isAttackerMark))
                {
                    autoWeaponType.IsAttackerMark = isAttackerMark;
                }

                bool receiverAttack = false;
                if (reader.ReadNormal(section, "AutoWeapon.ReceiverAttack", ref receiverAttack))
                {
                    autoWeaponType.ReceiverAttack = receiverAttack;
                    if (!receiverAttack)
                    {
                        autoWeaponType.ReceiverOwnBullet = false;
                    }
                }

                bool receiverOwnBullet = false;
                if (reader.ReadNormal(section, "AutoWeapon.ReceiverOwnBullet", ref receiverOwnBullet))
                {
                    autoWeaponType.ReceiverOwnBullet = receiverOwnBullet;
                }

            }

            return null != autoWeaponType;
        }

    }

}