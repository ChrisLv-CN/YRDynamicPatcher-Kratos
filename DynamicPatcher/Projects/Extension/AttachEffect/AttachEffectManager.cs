using System.Collections;
using DynamicPatcher;
using Extension.Ext;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    [Serializable]
    public class AttachEffectData
    {
        public List<string> AttachEffectTypes;
        public int CabinLength;
    }

    [Serializable]
    public class LocationMark
    {
        public CoordStruct Location;
        public DirStruct Direction;

        public LocationMark(CoordStruct location, DirStruct direction)
        {
            this.Location = location;
            this.Direction = direction;
        }
    }

    [Serializable]
    public class AttachEffectManager
    {
        public List<AttachEffect> AttachEffects; // 所有有效的AE
        public Dictionary<string, TimerStruct> DisableDelayTimers; // 同名AE失效后再赋予的计时器

        public List<LocationMark> LocationMarks;
        private CoordStruct lastLocation; // 使者的上一次位置
        private int locationMarkDistance; // 多少格记录一个位置
        private double totleMileage; // 总里程

        public int LocationSpace; // 替身火车的车厢间距

        public AttachEffectManager()
        {
            this.AttachEffects = new List<AttachEffect>();
            this.DisableDelayTimers = new Dictionary<string, TimerStruct>();
            this.LocationMarks = new List<LocationMark>();
            this.lastLocation = default;
            this.locationMarkDistance = 16;
            this.totleMileage = 0;
            this.LocationSpace = 512;
        }

        public int Count()
        {
            return AttachEffects.Count;
        }

        public void SetLocationSpace(int cabinLenght)
        {
            this.LocationSpace = cabinLenght;

            if (cabinLenght < locationMarkDistance)
            {
                this.locationMarkDistance = cabinLenght;
            }
        }

        public void Attach(AttachEffectData aeData, Pointer<ObjectClass> pOwner, Pointer<HouseClass> pHouse, bool attachOnceFlag)
        {
            // 写在type上附加的AE，攻击者是自己
            Pointer<TechnoClass> pAttacker = IntPtr.Zero;
            switch(pOwner.Ref.Base.WhatAmI())
            {
                case AbstractType.Unit:
                case AbstractType.Aircraft:
                case AbstractType.Building:
                case AbstractType.Infantry:
                    pAttacker = pOwner.Convert<TechnoClass>();
                    break;
                case AbstractType.Bullet:
                    pAttacker = pOwner.Convert<BulletClass>().Ref.Owner;
                    break;
            }
            if (null != aeData.AttachEffectTypes && aeData.AttachEffectTypes.Count > 0)
            {
                foreach (string type in aeData.AttachEffectTypes)
                {
                    // Logger.Log("事件{0}添加AE类型{1}", onUpdate ? "OnUpdate" : "OnInit", type);
                    Attach(type, pOwner, pHouse, pAttacker, attachOnceFlag);
                }
            }

            if (aeData.CabinLength > 0)
            {
                SetLocationSpace(aeData.CabinLength);
            }
        }

        public void Attach(string type, Pointer<ObjectClass> pOwner, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker, bool attachOnceFlag = true)
        {
            AttachEffectType aeType = AttachEffectType.FindOrAllocate(type, CCINIClass.INI_Rules) as AttachEffectType;
            if (attachOnceFlag && aeType.AttachOnceInTechnoType)
            {
                return;
            }
            // Logger.Log("AE {0} AttachOnceInTechnoType = {1}, AttachOnceFlag = {2}", aeType.Name, aeType.AttachOnceInTechnoType, attachOnceFlag);
            Attach(aeType, pOwner, pHouse, pAttacker);
        }

        public void Attach(AttachEffectType aeType, Pointer<ObjectClass> pOwner, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker)
        {
            if (aeType.AffectTypes != null && aeType.AffectTypes.Count > 0)
            {
                // 启用白名单
                if (!aeType.AffectTypes.Contains(pOwner.Ref.Type.Ref.Base.ID))
                {
                    // Logger.Log("ID {0} 不在白名单内，不能赋予", pOwner.Ref.Type.Ref.Base.ID);
                    // 不存在于白名单内
                    return;
                }
            }
            CoordStruct location = pOwner.Ref.Base.GetCoords();
            // 检查叠加
            bool add = aeType.Cumulative;
            if (!add)
            {
                // 不允许叠加

                // 攻击者标记
                bool isAttackerMark = (aeType.CumulativeByDifferentAttacker || (null !=aeType.AutoWeaponType && aeType.AutoWeaponType.IsAttackerMark)) && !pAttacker.IsNull && pAttacker.Ref.Base.IsAlive;
                // 攻击者标记AE名称相同，但可以来自不同的攻击者，可以叠加，不检查Delay
                // 检查冷却计时器
                if (!isAttackerMark && DisableDelayTimers.TryGetValue(aeType.Name, out TimerStruct delayTimer) && delayTimer.InProgress())
                {
                    // Logger.Log("类型[0]尚在冷却中，无法添加", autoWeaponType.Name);
                    return;
                }
                bool find = false;
                // 检查持续时间，增减Duration
                for (int i = Count() - 1; i >= 0; i--)
                {
                    AttachEffect temp = AttachEffects[i];
                    if (aeType.Group < 0)
                    {
                        // 找同名，如果是攻击者标记，同名且为同一攻击者，不附加
                        if (temp.Type.Name.Equals(aeType.Name) && (!isAttackerMark || temp.pAttacker.Pointer == pAttacker))
                        {
                            // 找到了
                            // Logger.Log("类型{0}已经存在，无法添加", aeType.Name);
                            find = true;
                            break;
                        }
                    }
                    else
                    {
                        // 找同组
                        if (temp.IsSameGroup(aeType))
                        {
                            // 找到了同组
                            find = true;
                            if (aeType.OverrideSameGroup)
                            {
                                // 替换
                                // 关闭发现的同组
                                temp.Disable(location);
                                add = true;
                                continue; // 全部替换
                            }
                            else
                            {
                                // 调整持续时间
                                temp.MergeDuation(aeType.Duration);
                            }
                        }
                    }
                }
                // 没找到同类或同组，可以添加新的实例
                add = add || !find;
            }
            if (add && aeType.Duration > 0)
            {
                AttachEffect ae = aeType.CreateObject();
                // 入队
                int index = AttachEffectHelper.FindInsertIndex(this, ae);
                // Logger.Log("添加AE类型{0}进入队列，插入位置{1}", type, index);
                AttachEffects.Insert(index, ae);
                // 激活
                ae.Enable(pOwner, pHouse, pAttacker);
            }
        }

        private CoordStruct MarkLocation(Pointer<ObjectClass> pOwner)
        {
            CoordStruct location = pOwner.Ref.Base.GetCoords();
            if (default == lastLocation)
            {
                lastLocation = location;
            }
            double mileage = location.DistanceFrom(lastLocation);
            if (mileage > locationMarkDistance)
            {
                lastLocation = location;
                double tempMileage = totleMileage + mileage;
                // 记录下当前的位置信息
                LocationMark locationMark = AttachEffectHelper.GetLocation(pOwner, new StandType());

                // 入队
                LocationMarks.Insert(0, locationMark);

                // 检查数量超过就删除最后一个
                if (tempMileage > (Count() + 1) * LocationSpace)
                {
                    LocationMarks.RemoveAt(LocationMarks.Count - 1);
                }
                else
                {
                    totleMileage = tempMileage;
                }
            }
            return location;
        }

        public bool HasSpace()
        {
            return totleMileage > Count() * LocationSpace;
        }

        public bool HasStand()
        {
            foreach(AttachEffect ae in AttachEffects)
            {
                if (null != ae.Stand && ae.IsActive())
                {
                    return true;
                }
            }
            return false;
        }

        public CrateMultiplier CountAttachStatusMultiplier()
        {
            CrateMultiplier multiplier = new CrateMultiplier();
            foreach (AttachEffect ae in AttachEffects)
            {
                if (null != ae.AttachStatus && ae.AttachStatus.Active)
                {
                    multiplier.FirepowerMultiplier *= ae.AttachStatus.Type.FirepowerMultiplier;
                    multiplier.ArmorMultiplier *= ae.AttachStatus.Type.ArmorMultiplier;
                    multiplier.SpeedMultiplier *= ae.AttachStatus.Type.SpeedMultiplier;
                    multiplier.ROFMultiplier *= ae.AttachStatus.Type.ROFMultiplier;
                    multiplier.Cloakable |= ae.AttachStatus.Type.Cloakable;
                    multiplier.ForceDecloak |= ae.AttachStatus.Type.ForceDecloak;
                    // Logger.Log("Count {0}, ae {1}", multiplier, ae.AttachStatus.Type);
                }
            }
            return multiplier;
        }

        public void Put(Pointer<ObjectClass> pOwner, Pointer<CoordStruct> pCoord, Direction faceDir)
        {
            foreach (AttachEffect ae in AttachEffects)
            {
                if (ae.IsActive())
                {
                    ae.OnPut(pOwner, pCoord, faceDir);
                }
            }
        }

        public void Remove(Pointer<ObjectClass> pOwner)
        {
            CoordStruct location = pOwner.Ref.Base.GetCoords();
            foreach (AttachEffect ae in AttachEffects)
            {
                if (ae.Type.DiscardOnEntry)
                {
                    ae.Disable(location);
                }
                else
                {
                    if (ae.IsActive())
                    {
                        ae.OnRemove(pOwner);
                    }
                }
            }
        }

        public void Update(Pointer<ObjectClass> pOwner, bool isDead)
        {
            // 记录下位置
            CoordStruct location = MarkLocation(pOwner);
            // 逐个触发有效的AEbuff，并移除无效的AEbuff
            int markIndex = 0;
            for (int i = Count() - 1; i >= 0; i--)
            {
                AttachEffect ae = AttachEffects[i];
                if (ae.IsActive())
                {
                    // Logger.Log("{0}更新AE类型{1}", pOwner, ae.Type.Name);
                    ae.OnUpdate(pOwner, isDead, this);
                    // 如果是替身，额外执行替身的定位操作
                    if (null != ae.Stand && ae.IsActive())
                    {
                        AttachEffectHelper.UpdateStandLocation(this, pOwner, ae.Stand, ref markIndex);
                    }
                }
                else
                {
                    // Logger.Log("{0}移除失效AE类型{1}, 设置不可再获取的延迟时间{2}", pOwner, ae.Type.Name, ae.Type.Delay);
                    int delay = ae.Type.Delay;
                    if (ae.Type.RandomDelay)
                    {
                        delay = ExHelper.Random.Next(ae.Type.MinDelay, ae.Type.MaxDelay);
                    }
                    if (delay > 0)
                    {
                        DisableDelayTimers[ae.Type.Name] = new TimerStruct(delay);
                    }
                    ae.Disable(location);
                    AttachEffects.Remove(ae);
                    // 如果有Next，则赋予新的AE
                    string nextAE = ae.Type.Next;
                    if (!string.IsNullOrEmpty(nextAE))
                    {
                        // Logger.Log("赋予NextAE {0}", nextAE);
                        Attach(nextAE, pOwner, ae.pHouse, IntPtr.Zero);
                    }
                }
            }
        }

        public unsafe void ReceiveDamage(Pointer<ObjectClass> pOwner, Pointer<int> pDamage, int distanceFromEpicenter, Pointer<WarheadTypeClass> pWH,
            Pointer<ObjectClass> pAttacker, bool ignoreDefenses, bool preventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        {
            foreach (AttachEffect ae in AttachEffects)
            {
                if (ae.IsActive())
                {
                    ae.OnReceiveDamage(pOwner, pDamage, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse);
                }
            }

        }

        public void DestroyAll(Pointer<ObjectClass> pOwner)
        {
            foreach (AttachEffect ae in AttachEffects)
            {
                if (ae.IsActive())
                {
                    ae.OnDestroy(pOwner);
                }
            }
        }

        public void UnInitAll(CoordStruct location)
        {
            for (int i = Count() - 1; i >= 0; i--)
            {
                AttachEffect ae = AttachEffects[i];
                if (ae.IsActive())
                {
                    ae.Disable(location);
                }
                AttachEffects.Remove(ae);
            }
            AttachEffects.Clear();
        }

        public void GuardCommand()
        {
            foreach (AttachEffect ae in AttachEffects)
            {
                if (ae.IsActive())
                {
                    ae.OnGuardCommand();
                }
            }
        }

        public void StopCommand()
        {
            foreach (AttachEffect ae in AttachEffects)
            {
                if (ae.IsActive())
                {
                    ae.OnStopCommand();
                }
            }
        }

        public static bool ReadAttachEffect(INIReader reader, string section, out AttachEffectData attachEffectData)
        {
            attachEffectData = null;

            List<string> aeTypes = null;
            if (ExHelper.ReadList(reader, section, "AttachEffectTypes", ref aeTypes))
            {
                if (null == attachEffectData)
                {
                    attachEffectData = new AttachEffectData();
                }
                attachEffectData.AttachEffectTypes = aeTypes;
            }

            int cabinLength = 0;
            if (reader.ReadNormal(section, "StandTrainCabinLength", ref cabinLength))
            {
                if (null == attachEffectData)
                {
                    attachEffectData = new AttachEffectData();
                }
                attachEffectData.CabinLength = cabinLength;
            }

            return null != attachEffectData;
        }

    }

}