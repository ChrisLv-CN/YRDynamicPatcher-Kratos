using System.Collections;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using PatcherYRpp.Utilities;
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
    public partial class AttachEffectManager
    {
        public List<AttachEffect> AttachEffects; // 所有有效的AE
        public Dictionary<string, TimerStruct> DisableDelayTimers; // 同名AE失效后再赋予的计时器

        public List<LocationMark> LocationMarks;
        private CoordStruct lastLocation; // 使者的上一次位置
        private int locationMarkDistance; // 多少格记录一个位置
        private double totleMileage; // 总里程

        private bool renderFlag = false; // Render比Update先执行，在附着对象Render时先调整替身位置，Update就不用调整

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
            switch (pOwner.Ref.Base.WhatAmI())
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
            if (!aeType.Enable)
            {
                Logger.LogWarning("Attempt to attach an invalid AE [{0}] for [{1}]", aeType.Name, pOwner.Ref.Type.Ref.Base.ID);
                return;
            }
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
            if (aeType.NotAffectTypes != null && aeType.NotAffectTypes.Count > 0)
            {
                // 启用黑名单
                if (aeType.NotAffectTypes.Contains(pOwner.Ref.Type.Ref.Base.ID))
                {
                    // Logger.Log("ID {0} 在黑名单内，不能赋予", pOwner.Ref.Type.Ref.Base.ID);
                    // 存在于黑名单内
                    return;
                }
            }
            CoordStruct location = pOwner.Ref.Base.GetCoords();
            // 检查叠加
            bool add = aeType.Cumulative == CumulativeMode.YES;
            if (!add)
            {
                // 不同攻击者是否叠加
                bool isAttackMark = aeType.Cumulative == CumulativeMode.ATTACKER && !pAttacker.IsNull && pAttacker.Ref.Base.IsAlive;
                // 攻击者标记AE名称相同，但可以来自不同的攻击者，可以叠加，不检查Delay
                // 检查冷却计时器
                if (!isAttackMark && DisableDelayTimers.TryGetValue(aeType.Name, out TimerStruct delayTimer) && delayTimer.InProgress())
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
                        // 找同名
                        if (temp.Type.Name.Equals(aeType.Name))
                        {
                            // 找到了
                            find = true;
                            if (isAttackMark)
                            {
                                if (temp.pSource.Pointer == pAttacker)
                                {
                                    // 是攻击者标记，且相同的攻击者，重置持续时间
                                    if (temp.Type.ResetDurationOnReapply)
                                    {
                                        temp.ResetDuration();
                                        AttachEffects[i] = temp;
                                    }
                                    break;
                                }
                            }
                            else
                            {
                                // 不是攻击者标记，重置持续时间
                                if (temp.Type.ResetDurationOnReapply)
                                {
                                    temp.ResetDuration();
                                    AttachEffects[i] = temp;
                                }
                            }
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
                                // Logger.Log("待添加新的{0}，发现同组已存在{1}，关闭", aeType.Name, temp.Name);
                                // 关闭发现的同组
                                temp.Disable(location);
                                add = true;
                                continue; // 全部替换
                            }
                            else
                            {
                                // Logger.Log("待添加新的{0}，发现同组已存在{1}，调整持续时间{2}", aeType.Name, temp.Name, aeType.Duration);
                                // 调整持续时间
                                temp.MergeDuation(aeType.Duration);
                                AttachEffects[i] = temp;
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
                int index = FindInsertIndex(ae);
                // Logger.Log("添加AE类型{0}进入队列，插入位置{1}", aeType.Name, index);
                AttachEffects.Insert(index, ae);
                // 激活
                ae.Enable(pOwner, pHouse, pAttacker);
            }
        }

        public int FindInsertIndex(AttachEffect ae)
        {
            if (null != ae.Stand && ae.Stand.Type.IsTrain)
            {
                StandType type = ae.Stand.Type;
                int index = -1;
                // 插入头还是尾
                if (type.CabinHead)
                {
                    // 插入队列末位
                    // 检查是否有分组
                    if (type.CabinGroup > -1)
                    {
                        // 倒着找自己的分组
                        for (int j = Count() - 1; j >= 0; j--)
                        {
                            AttachEffect temp = AttachEffects[j];
                            Stand tempStand = null;
                            if (null != (tempStand = temp.Stand))
                            {
                                if (type.CabinGroup == tempStand.Type.CabinGroup)
                                {
                                    // 找到组员
                                    index = j;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // 插入队列首位
                    index = 0;
                    // 检查是否有分组
                    if (type.CabinGroup > -1)
                    {
                        // 顺着找自己的分组
                        for (int j = 0; j < Count(); j++)
                        {
                            AttachEffect temp = AttachEffects[j];
                            Stand tempStand = null;
                            if (null != (tempStand = temp.Stand))
                            {
                                if (type.CabinGroup == tempStand.Type.CabinGroup)
                                {
                                    // 找到组员
                                    index = j;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (index > -1)
                {
                    return index;
                }
            }
            return 0;
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
                LocationMark locationMark = StandHelper.GetLocation(pOwner, new StandType());

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
            foreach (AttachEffect ae in AttachEffects)
            {
                if (null != ae.Stand && ae.IsActive())
                {
                    return true;
                }
            }
            return false;
        }

        public AttachStatusType CountAttachStatusMultiplier()
        {
            AttachStatusType multiplier = new AttachStatusType();
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

        public unsafe void Render2(Pointer<ObjectClass> pOwner, bool isDead)
        {
            renderFlag = !isDead;
            if (renderFlag)
            {
                // 记录下位置
                CoordStruct location = MarkLocation(pOwner);
                // 更新替身的位置
                int markIndex = 0;
                for (int i = Count() - 1; i >= 0; i--)
                {
                    AttachEffect ae = AttachEffects[i];
                    if (ae.IsActive())
                    {
                        // 如果是替身，额外执行替身的定位操作
                        if (null != ae.Stand && ae.Stand.IsAlive())
                        {
                            StandHelper.UpdateStandLocation(this, pOwner, ae.Stand, ref markIndex); // 调整位置
                        }
                        ae.OnRender2(pOwner, location);
                    }
                }
            }
        }

        public void Update(Pointer<ObjectClass> pOwner, bool isDead)
        {
            // 记录下位置
            CoordStruct location = pOwner.Ref.Base.GetCoords();

            if (!renderFlag)
            {
                location = MarkLocation(pOwner);
            }
            // 逐个触发有效的AEbuff，并移除无效的AEbuff
            int markIndex = 0;
            for (int i = Count() - 1; i >= 0; i--)
            {
                AttachEffect ae = AttachEffects[i];
                if (ae.IsActive())
                {
                    if (!renderFlag && null != ae.Stand && ae.Stand.IsAlive())
                    {
                        // 替身不需要渲染时，在update中调整替身的位置
                        StandHelper.UpdateStandLocation(this, pOwner, ae.Stand, ref markIndex);
                    }
                    // Logger.Log($"{Game.CurrentFrame} - {pOwner} [{pOwner.Ref.Type.Ref.Base.ID}] {ae.Type.Name} 执行更新");
                    ae.OnUpdate(pOwner, location, isDead);
                }
                else
                {
                    // Logger.Log($"{Game.CurrentFrame} - {pOwner} [{pOwner.Ref.Type.Ref.Base.ID}] {ae.Type.Name} 失效，从列表中移除，不可再赋予延迟 {ae.Type.Delay}");
                    int delay = ae.Type.Delay;
                    if (ae.Type.RandomDelay)
                    {
                        delay = MathEx.Random.Next(ae.Type.MinDelay, ae.Type.MaxDelay);
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
                        Attach(nextAE, pOwner, ae.pSourceHouse, ae.pSource, false);
                    }
                }
            }
            renderFlag = false;
            // if (pOwner.Ref.Type.Ref.Base.ID.ToString().StartsWith("SHOGUN"))
            //     Logger.Log($"{Game.CurrentFrame} - {pOwner} [{pOwner.Ref.Type.Ref.Base.ID}] Update, pos {pOwner.Ref.Base.GetCoords()}");
        }

        public unsafe void TemporalUpdate(TechnoExt ext, Pointer<TemporalClass> pTemporal)
        {
            for (int i = Count() - 1; i >= 0; i--)
            {
                AttachEffect ae = AttachEffects[i];
                if (ae.IsActive())
                {
                    ae.OnTemporalUpdate(ext, pTemporal);
                }
            }
        }

        public void Put(Pointer<ObjectClass> pOwner, Pointer<CoordStruct> pCoord, short faceDirValue8)
        {
            foreach (AttachEffect ae in AttachEffects)
            {
                if (ae.IsActive())
                {
                    ae.OnPut(pOwner, pCoord, faceDirValue8);
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
                if (ae.IsAnyAlive())
                {
                    // Logger.Log($"{Game.CurrentFrame} - {ae.Type.Name} 注销，执行关闭");
                    ae.Disable(location);
                }
                // Logger.Log($"{Game.CurrentFrame} - {ae.Type.Name} 注销，移出列表");
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
            if (reader.ReadStringList(section, "AttachEffectTypes", ref aeTypes))
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