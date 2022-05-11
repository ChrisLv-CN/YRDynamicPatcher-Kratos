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
                this.Enable = true;
                this.AutoWeaponType = type;
            }
            else
            {
                type = null;
            }
        }
    }

    [Serializable]
    public class AutoWeaponData
    {
        public int WeaponIndex; // 使用单位自身的武器
        public List<string> WeaponTypes; // 武器类型
        public int RandomTypesNum; // 随机使用几个武器
        public CoordStruct FireFLH; // 开火相对位置
        public CoordStruct TargetFLH; // 目标相对位置
        public CoordStruct MoveTo; // 以开火位置为坐标0点，计算TargetFLH

        public AutoWeaponData()
        {
            this.WeaponIndex = -1;
            this.WeaponTypes = null;
            this.RandomTypesNum = 0;
            this.FireFLH = default;
            this.TargetFLH = default;
            this.MoveTo = default;
        }

        public AutoWeaponData Clone()
        {
            AutoWeaponData data = new AutoWeaponData();
            data.WeaponIndex = this.WeaponIndex;
            data.WeaponTypes = this.WeaponTypes;
            data.RandomTypesNum = this.RandomTypesNum;
            data.FireFLH = this.FireFLH;
            data.TargetFLH = this.TargetFLH;
            data.MoveTo = this.MoveTo;
            return data;
        }

        public bool TryReadType(INIReader reader, string section, string title)
        {
            bool isRead = false;


            int weaponIdx = -1;
            if (reader.ReadNormal(section, title + "WeaponIndex", ref weaponIdx))
            {
                if (weaponIdx > -1)
                {
                    isRead = true;
                    this.WeaponIndex = weaponIdx;
                }
            }

            List<string> weaponTypes = null;
            if (reader.ReadStringList(section, title + "Types", ref weaponTypes))
            {
                isRead = true;
                this.WeaponTypes = weaponTypes;
            }

            int randomTypesNum = 0;
            if (reader.ReadNormal(section, title + "RandomTypesNum", ref randomTypesNum))
            {
                if (randomTypesNum > 0)
                {
                    isRead = true;
                    this.RandomTypesNum = randomTypesNum;
                }
            }

            CoordStruct fireFLH = default;
            if (reader.ReadCoordStruct(section, title + "FireFLH", ref fireFLH))
            {
                isRead = true;
                this.FireFLH = fireFLH;
            }

            CoordStruct targetFLH = default;
            if (reader.ReadCoordStruct(section, title + "TargetFLH", ref targetFLH))
            {
                isRead = true;
                this.TargetFLH = targetFLH;
            }

            CoordStruct moveTo = default;
            if (reader.ReadCoordStruct(section, title + "MoveTo", ref moveTo))
            {
                isRead = true;
                this.MoveTo = moveTo;
                this.TargetFLH = this.FireFLH + moveTo;
            }


            return isRead;
        }

    }

    /// <summary>
    /// 自动武器类型
    /// </summary>
    [Serializable]
    public class AutoWeaponType : EffectType<AutoWeapon>
    {

        public AutoWeaponData Data; // 普通
        public AutoWeaponData EliteData; // 精英

        // public int WeaponIndex; // 使用单位自身的武器
        // public int EliteWeaponIndex; // 精英时使用单位自身的武器
        // public List<string> WeaponTypes; // 武器类型
        // public List<string> EliteWeaponTypes; // 精英武器类型
        // public int RandomTypesNum; // 随机使用几个武器
        // public int EliteRandomTypesNum; // 精英时随机使用几个武器
        // public CoordStruct FireFLH; // 开火相对位置
        // public CoordStruct EliteFireFLH; // 精英开火相对位置
        // public CoordStruct TargetFLH; // 目标相对位置
        // public CoordStruct EliteTargetFLH; // 精英目标相对位置
        // public CoordStruct MoveTo; // 以开火位置为坐标0点，计算TargetFLH
        // public CoordStruct EliteMoveTo; // 以开火位置为坐标0点，计算EliteTargetFLH

        public bool FireOnce; // 发射后销毁
        public bool FireToTarget; // 朝附加对象的目标开火，如果附加的对象没有目标，不开火
        public bool IsOnTurret; // 相对炮塔或者身体
        public bool IsOnWorld; // 相对世界

        // 攻击者标记
        public bool IsAttackerMark; // 允许附加对象和攻击者进行交互
        public bool ReceiverAttack; // 武器由AE的接受者发射
        public bool ReceiverOwnBullet; // 武器所属是AE的接受者

        public AutoWeaponType()
        {
            this.Data = null;
            this.EliteData = null;
            this.FireOnce = false;
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

            AutoWeaponData data = new AutoWeaponData();
            if (data.TryReadType(reader, section, "AutoWeapon."))
            {
                this.Data = data;
                this.EliteData = data;
            }

            AutoWeaponData eliteData = null != Data ? Data.Clone() : new AutoWeaponData();
            if (eliteData.TryReadType(reader, section, "AutoWeapon.Elite"))
            {
                this.EliteData = eliteData;
            }

            if (this.Enable = (null != Data || null != EliteData))
            {

                bool fireOnce = false;
                if (reader.ReadNormal(section, "AutoWeapon.FireOnce", ref fireOnce))
                {
                    this.FireOnce = fireOnce;
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