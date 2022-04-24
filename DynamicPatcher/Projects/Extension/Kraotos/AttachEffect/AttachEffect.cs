using System.Diagnostics;
using System.Reflection;
using System.Collections;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    [Serializable]
    public partial class AttachEffect : IAttachEffectBehaviour
    {

        public string Name;
        public AttachEffectType Type;

        public SwizzleablePointer<HouseClass> pHouse;
        public SwizzleablePointer<TechnoClass> pAttacker;
        public bool Active;
        private int duration; // 寿命
        private bool immortal; // 永生
        private TimerStruct lifeTimer;
        private TimerStruct initialDelayTimer;
        private bool delayToEnable; // 延迟激活中

        private List<IAttachEffectBehaviour> effects = new List<IAttachEffectBehaviour>();

        // AE激活，开始生效
        private event System.Action<Pointer<ObjectClass>, Pointer<HouseClass>, Pointer<TechnoClass>> EnableAction;
        // AE关闭，销毁相关资源
        private event System.Action<CoordStruct> DisableAction;
        // 重置计时器
        private event System.Action ResetDurationAction;
        // 更新
        private event System.Action<Pointer<ObjectClass>, bool> OnUpdateAction;
        // 被超时空冻结更新
        private event System.Action<TechnoExt, Pointer<TemporalClass>> OnTemporalUpdateAction;
        // 挂载AE的单位出现在地图上
        private event System.Action<Pointer<ObjectClass>, Pointer<CoordStruct>, DirStruct> OnPutAction;
        // 挂载AE的单位从地图隐藏
        private event System.Action<Pointer<ObjectClass>> OnRemoveAction;
        // 收到伤害
        private event System.Action<Pointer<ObjectClass>, Pointer<int>, int, Pointer<WarheadTypeClass>, Pointer<ObjectClass>, bool, bool, Pointer<HouseClass>> OnReceiveDamageAction;
        // 收到伤害导致死亡
        private event System.Action<Pointer<ObjectClass>> OnDestroyAction;
        // 按下G键
        private event System.Action OnGuardCommandAction;
        // 按下S键
        private event System.Action OnStopCommandAction;


        public AttachEffect(AttachEffectType type)
        {
            this.Name = type.Name;
            this.Type = type;
            this.pHouse = new SwizzleablePointer<HouseClass>(IntPtr.Zero);
            this.pAttacker = new SwizzleablePointer<TechnoClass>(IntPtr.Zero);
            int initDelay = type.InitialDelay;
            this.delayToEnable = false;
            if (type.InitialRandomDelay)
            {
                initDelay = MathEx.Random.Next(type.InitialMinDelay, type.InitialMaxDelay);
            }
            if (initDelay > 0)
            {
                this.initialDelayTimer.Start(initDelay);
                this.delayToEnable = true;
            }
            this.duration = type.Duration;
            this.immortal = Type.HoldDuration;
            // if (!this.immortal)
            // {
            //     this.lifeTimer.Start(this.duration);
            // }

            // Stopwatch stopwatch = new Stopwatch();
            // stopwatch.Start();
            // foreach(MethodInfo m in InitMethods)
            // {
            //     m.Invoke(this, null);
            // }


            InitStand(); // 替身需要第一个初始化

            InitAnimation();
            InitAttachStatus();
            InitAutoWeapon();
            InitBlackHole();
            InitDestroySelf();
            InitFireSuper(); // AffectWho
            InitGiftBox(); // AffectWho
            InitPaintball();
            InitTransform();
            InitDisableWeapon(); // AffectWho
            InitDeselect(); // AffectWho
            InitOverrideWeapon(); // AffectWho

            // stopwatch.Stop();
            // Logger.Log($"{Game.CurrentFrame} 初始化 {Name} 耗时 {stopwatch.Elapsed}");
        }

        public void SetupLifeTimer()
        {
            if (!this.immortal)
            {
                this.lifeTimer.Start(this.duration);
            }
        }

        private void RegisterAction(IAttachEffectBehaviour behaviour)
        {
            // Logger.Log($"{Game.CurrentFrame}, 注册AE系统 {Type.Name}, 模块 {behaviour.GetType().Name}");
            effects.Add(behaviour);
            // AE激活，开始生效
            this.EnableAction += behaviour.Enable;
            // AE关闭，销毁相关资源
            this.DisableAction += behaviour.Disable;
            // 重置计时器
            this.ResetDurationAction += behaviour.ResetDuration;
            // 更新
            this.OnUpdateAction += behaviour.OnUpdate;
            // 被超时空冻结更新
            this.OnTemporalUpdateAction += behaviour.OnTemporalUpdate;
            // 挂载AE的单位出现在地图上
            this.OnPutAction += behaviour.OnPut;
            // 挂载AE的单位从地图隐藏
            this.OnRemoveAction += behaviour.OnRemove;
            // 收到伤害
            this.OnReceiveDamageAction += behaviour.OnReceiveDamage;
            // 收到伤害导致死亡
            this.OnDestroyAction += behaviour.OnDestroy;
            // 按下G键
            this.OnGuardCommandAction += behaviour.OnGuardCommand;
            // 按下S键
            this.OnStopCommandAction += behaviour.OnStopCommand;
        }

        /// <summary>
        /// 激活
        /// </summary>
        public void Enable(Pointer<ObjectClass> pObject, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker)
        {
            this.Active = true;
            this.pHouse.Pointer = pHouse;
            this.pAttacker.Pointer = pAttacker;
            if (!delayToEnable || initialDelayTimer.Expired())
            {

                EnableEffects(pObject, pHouse, pAttacker);
            }
        }

        private void EnableEffects(Pointer<ObjectClass> pObject, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker)
        {
            delayToEnable = false;
            SetupLifeTimer();
            EnableAction?.Invoke(pObject, pHouse, pAttacker);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Disable(CoordStruct location)
        {
            this.Active = false;
            if (delayToEnable)
            {
                return;
            }
            DisableAction?.Invoke(location);
        }

        public bool IsActive()
        {
            if (Active)
            {
                // Logger.Log("AE Type {0} {1} and {2}", Type.Name, IsDeath() ? "is death" : "not dead", IsAlive() ? "is alive" : "not alive");
                Active = delayToEnable || (!IsDeath() && IsAlive());
            }
            return Active;
        }

        public bool IsAlive()
        {
            foreach (IAttachEffectBehaviour effect in effects)
            {
                if (!effect.IsAlive())
                {
                    // Logger.Log($"{Game.CurrentFrame} - AE {Name} 模块 {effect.GetType().Name} 狗带了");
                    return false;
                }
            }
            return true;
            // return (null == Animation || Animation.IsAlive())
            //     && (null == AttachStatus || AttachStatus.IsAlive())
            //     && (null == AutoWeapon || AutoWeapon.IsAlive())
            //     && (null == BlackHole || BlackHole.IsAlive())
            //     && (null == DestroySelf || DestroySelf.IsAlive())
            //     && (null == Paintball || Paintball.IsAlive())
            //     && (null == Stand || Stand.IsAlive())
            //     && (null == Transform || Transform.IsAlive())
            //     && (null == Weapon || Weapon.IsAlive());
        }

        private bool IsDeath()
        {
            // Logger.Log("AE Type {0} duration {1} time left {2}", Type.Name, duration, lifeTimer.TimeLeft);
            return duration <= 0 || (!immortal && lifeTimer.Expired());
        }

        private void ForceStartLifeTimer(int timeLeft)
        {
            this.immortal = false;
            this.lifeTimer.Start(timeLeft);
            // Logger.Log("启动{0}生命计时器，生命{1}，计时{2}", Name, duration, timeLeft);
        }

        public bool IsSameGroup(AttachEffectType otherType)
        {
            return this.Type.Group > -1 && otherType.Group > -1 && this.Type.Group == otherType.Group;
        }

        public void MergeDuation(int otherDuration)
        {
            if (delayToEnable || otherDuration == 0)
            {
                // Logger.Log("{0}延迟激活中，不接受时延修改", Name);
                return;
            }
            // 重设时间
            if (otherDuration < 0)
            {
                // 剩余时间
                int timeLeft = immortal ? this.duration : lifeTimer.GetTimeLeft();
                // 削减生命总长
                this.duration += otherDuration;
                if (this.duration <= 0 || timeLeft <= 0)
                {
                    // 削减的时间超过总长度，直接减没了
                    this.Active = false;
                }
                else
                {
                    // Logger.Log("削减{0}持续时间{1}，{2}生命{3}，当前剩余{4}", Name, otherDuration, this.immortal ? "无限" : "", this.duration, timeLeft);
                    timeLeft += otherDuration;
                    if (timeLeft <= 0)
                    {
                        // 削减完后彻底没了
                        this.Active = false;
                    }
                    else
                    {
                        // 还有剩
                        // 重设时间
                        ForceStartLifeTimer(timeLeft);
                    }
                }
            }
            else
            {
                // 累加持续时间
                this.duration += otherDuration;
                if (!immortal)
                {
                    int timeLeft = lifeTimer.GetTimeLeft();
                    // Logger.Log("增加{0}持续时间{1}，当前剩余{2}", Name, otherDuration, timeLeft);
                    timeLeft += otherDuration;
                    ForceStartLifeTimer(timeLeft);
                }
            }
        }

        public void ResetDuration()
        {
            SetupLifeTimer();
            ResetDurationAction?.Invoke();
        }

        public void OnUpdate(Pointer<ObjectClass> pObject, bool isDead)
        {
            if (delayToEnable)
            {
                if (initialDelayTimer.InProgress())
                {
                    return;
                }
                EnableEffects(pObject, pHouse, pAttacker);
            }
            OnUpdateAction?.Invoke(pObject, isDead);
        }

        public void OnTemporalUpdate(TechnoExt ext, Pointer<TemporalClass> pTemporal)
        {
            if (delayToEnable)
            {
                return;
            }
            OnTemporalUpdateAction?.Invoke(ext, pTemporal);
        }

        public void OnPut(Pointer<ObjectClass> pObject, Pointer<CoordStruct> pCoord, DirStruct faceDir)
        {
            if (delayToEnable)
            {
                return;
            }
            OnPutAction?.Invoke(pObject, pCoord, faceDir);
        }

        public void OnRemove(Pointer<ObjectClass> pObject)
        {
            if (delayToEnable)
            {
                return;
            }
            OnRemoveAction?.Invoke(pObject);
        }

        public void OnReceiveDamage(Pointer<ObjectClass> pObject, Pointer<int> pDamage, int distanceFromEpicenter, Pointer<WarheadTypeClass> pWH,
            Pointer<ObjectClass> pAttacker, bool ignoreDefenses, bool preventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        {
            if (delayToEnable)
            {
                return;
            }
            OnReceiveDamageAction?.Invoke(pObject, pDamage, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse);
        }

        public void OnDestroy(Pointer<ObjectClass> pObject)
        {
            if (delayToEnable)
            {
                return;
            }
            OnDestroyAction?.Invoke(pObject);
        }

        public void OnGuardCommand()
        {
            if (delayToEnable)
            {
                return;
            }
            OnGuardCommandAction?.Invoke();
        }

        public void OnStopCommand()
        {
            if (delayToEnable)
            {
                return;
            }
            OnStopCommandAction?.Invoke();
        }

    }
}