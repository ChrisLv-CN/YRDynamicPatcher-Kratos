using System.Reflection;
using System.Collections;
using DynamicPatcher;
using Extension.Utilities;
using Extension.Script;
using PatcherYRpp;
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

    public interface IAttachEffectBehaviour
    {

        // 返回AE是否还存活
        bool IsAlive();
        // AE激活，开始生效
        void Enable(Pointer<ObjectClass> pOwner, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker);
        // AE关闭，销毁相关资源
        void Disable(CoordStruct location);
        // 重置计时器
        void ResetDuration();
        // 更新
        void OnUpdate(Pointer<ObjectClass> pOwner, bool isDead);
        // 被超时空冻结更新
        void OnTemporalUpdate(TechnoExt ext, Pointer<TemporalClass> pTemporal);
        // 挂载AE的单位出现在地图上
        void OnPut(Pointer<ObjectClass> pOwner, Pointer<CoordStruct> pCoord, short faceDirValue8);
        // 挂载AE的单位从地图隐藏
        void OnRemove(Pointer<ObjectClass> pOwner);
        // 收到伤害
        void OnReceiveDamage(Pointer<ObjectClass> pOwner, Pointer<int> pDamage, int DistanceFromEpicenter, Pointer<WarheadTypeClass> pWH, Pointer<ObjectClass> pAttacker, bool IgnoreDefenses, bool PreventPassengerEscape, Pointer<HouseClass> pAttackingHouse);
        // 收到伤害导致死亡
        void OnDestroy(Pointer<ObjectClass> pOwner);
        // 按下G键
        void OnGuardCommand();
        // 按下S键
        void OnStopCommand();
    }

    [Serializable]
    public class AttachEffectBehaviour : IAttachEffectBehaviour
    {
        // 返回AE是否还存活
        public virtual bool IsAlive() { return true; }
        // AE激活，开始生效
        public virtual void Enable(Pointer<ObjectClass> pOwner, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker) { }
        // AE关闭，销毁相关资源
        public virtual void Disable(CoordStruct location) { }
        // 重置计时器
        public virtual void ResetDuration() { }
        // 更新
        public virtual void OnUpdate(Pointer<ObjectClass> pOwner, bool isDead) { }
        // 被超时空冻结更新
        public virtual void OnTemporalUpdate(TechnoExt ext, Pointer<TemporalClass> pTemporal) { }
        // 挂载AE的单位出现在地图上
        public virtual void OnPut(Pointer<ObjectClass> pOwner, Pointer<CoordStruct> pCoord, short faceDirValue8) { }
        // 挂载AE的单位从地图隐藏
        public virtual void OnRemove(Pointer<ObjectClass> pOwner) { }
        // 收到伤害
        public virtual void OnReceiveDamage(Pointer<ObjectClass> pOwner, Pointer<int> pDamage, int DistanceFromEpicenter, Pointer<WarheadTypeClass> pWH, Pointer<ObjectClass> pAttacker, bool IgnoreDefenses, bool PreventPassengerEscape, Pointer<HouseClass> pAttackingHouse) { }
        // 收到伤害导致死亡
        public virtual void OnDestroy(Pointer<ObjectClass> pOwner) { }
        // 按下G键
        public virtual void OnGuardCommand() { }
        // 按下S键
        public virtual void OnStopCommand() { }

    }

}