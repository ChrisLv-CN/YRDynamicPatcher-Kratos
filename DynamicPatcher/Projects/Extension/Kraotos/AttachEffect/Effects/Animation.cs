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

    public partial class AttachEffect
    {
        public Animation Animation;

        private void InitAnimation()
        {
            if (null != Type.AnimationType)
            {
                this.Animation = Type.AnimationType.CreateObject(Type);
                RegisterAction(Animation);
            }
        }
    }


    [Serializable]
    public class Animation : Effect<AnimationType>
    {
        private SwizzleablePointer<AnimClass> pAnim;
        private BlitterFlags animFlags;

        private bool OnwerIsDead;
        private bool OnwerIsCloakable;

        public Animation()
        {
            this.pAnim = new SwizzleablePointer<AnimClass>(IntPtr.Zero);
            this.OnwerIsDead = false;
        }

        public override void OnEnable(Pointer<ObjectClass> pObject, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker)
        {
            // 激活动画
            // Logger.Log("效果激活，播放激活动画{0}", Type.ActiveAnim);
            if (!string.IsNullOrEmpty(Type.ActiveAnim))
            {
                Pointer<AnimTypeClass> pAnimType = AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(Type.ActiveAnim);
                if (!pAnimType.IsNull)
                {
                    Pointer<AnimClass> pAnim = YRMemory.Create<AnimClass>(pAnimType, pObject.Ref.Base.GetCoords());
                    pAnim.Ref.SetOwnerObject(pObject);
                    pAnim.SetAnimOwner(pObject);
                }
            }
            // 持续动画
            CreateAnim(pObject);
        }

        private void CreateAnim(Pointer<ObjectClass> pObject)
        {
            if (!pAnim.IsNull)
            {
                // Logger.Log($"{Game.CurrentFrame} AE[{AEType.Name}]持续动画[{Type.IdleAnim}]已存在，清除再重新创建");
                KillAnim();
            }
            if (string.IsNullOrEmpty(Type.IdleAnim) || pObject.IsInvisible() || (Type.RemoveInCloak && OnwerIsCloakable))
            {
                return;
            }
            // 创建动画
            // Logger.Log($"{Game.CurrentFrame} AE[{AEType.Name}]创建持续动画[{Type.IdleAnim}]");
            Pointer<AnimTypeClass> pAnimType = AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(Type.IdleAnim);
            if (!pAnimType.IsNull)
            {
                Pointer<AnimClass> pAnim = YRMemory.Create<AnimClass>(pAnimType, pObject.Ref.Base.GetCoords());
                // Logger.Log($"{Game.CurrentFrame} AE[{AEType.Name}]成功创建持续动画[{Type.IdleAnim}], 指针 {pAnim}");
                // pAnim.Ref.SetOwnerObject(pObject); // 当单位隐形时，附着在单位上的动画会被游戏注销
                // Logger.Log(" - 将动画{0}赋予对象", Type.IdleAnim);
                pAnim.Ref.Loops = 0xFF;
                // Logger.Log(" - 设置动画{0}的剩余迭代次数为{1}", Type.IdleAnim, 0xFF);
                pAnim.SetAnimOwner(pObject);
                pAnim.Show(Type.Visibility);
                this.pAnim.Pointer = pAnim;
                this.animFlags = pAnim.Ref.AnimFlags;
                // Logger.Log(" - 缓存动画{0}的实例对象指针", Type.IdleAnim);
            }
        }

        public override void Disable(CoordStruct location)
        {
            // Logger.Log($"{Game.CurrentFrame} AE[{AEType.Name}]效果结束，移除持续动画[{Type.IdleAnim}]");
            KillAnim();
            // Logger.Log($"{Game.CurrentFrame} AE[{AEType.Name}]效果结束，播放结束动画[{Type.DoneAnim}]");
            // 结束动画
            if (!string.IsNullOrEmpty(Type.DoneAnim))
            {
                Pointer<AnimTypeClass> pAnimType = AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(Type.DoneAnim);
                if (!pAnimType.IsNull)
                {
                    Pointer<AnimClass> pAnim = YRMemory.Create<AnimClass>(pAnimType, location);
                    // pAnim.Ref.SetOwnerObject(pObject);
                }
            }
        }

        private void KillAnim()
        {
            if (!pAnim.IsNull)
            {
                // 不将动画附着于单位上，动画就不会自行注销，需要手动注销
                pAnim.Ref.TimeToDie = true;
                pAnim.Ref.Base.UnInit(); // 包含了SetOwnerObject(0) 0x4255B0
                // Logger.Log("{0} - 已销毁动画{1}实例", Game.CurrentFrame, Type.IdleAnim);
                pAnim.Pointer = IntPtr.Zero;
                // Logger.Log("{0} - 成功移除持续动画{1}", Game.CurrentFrame, Type.IdleAnim);
            }
        }

        public override void OnPut(Pointer<ObjectClass> pObject, Pointer<CoordStruct> pCoord, short faceDirValue8)
        {
            // Logger.Log("单位显现，创建持续动画{0}", Type.IdleAnim);
            CreateAnim(pObject);
        }

        public override void OnRender2(Pointer<ObjectClass> pOwner, CoordStruct location)
        {
            // Logger.Log($"{Game.CurrentFrame} AE[{AEType.Name}]持续动画 {pAnim.Pointer}[{Type.IdleAnim}], 透明度 {pAnim.Ref.UnderTemporal}, 类型透明度 {pAnim.Ref.Type.Ref.Translucency}");
            UpdateLocation(location);
        }

        public override void OnUpdate(Pointer<ObjectClass> pOwner, CoordStruct location, bool isDead)
        {
            this.OnwerIsDead = isDead;
            if (!isDead && pOwner.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                // Logger.Log($"{Game.CurrentFrame} AE[{AEType.Name}]附着单位 {pOwner} [{pOwner.Ref.Type.Ref.Base.ID}] 隐形 = {pTechno.Ref.Cloakable}，隐形状态 {pTechno.Ref.CloakStates}");
                switch (pTechno.Ref.CloakStates)
                {
                    case CloakStates.UnCloaked:
                        // 显形状态
                        if (OnwerIsCloakable)
                        {
                            OnwerIsCloakable = false;
                            if (Type.RemoveInCloak)
                            {
                                CreateAnim(pOwner);
                            }
                            else if (Type.TranslucentInCloak)
                            {
                                // 恢复不透明
                                pAnim.Ref.AnimFlags = animFlags;
                            }
                        }
                        break;
                    default:
                        // 进入隐形或处于正在隐\显形
                        if (!OnwerIsCloakable)
                        {
                            OnwerIsCloakable = true;
                            if (Type.RemoveInCloak)
                            {
                                KillAnim();
                            }
                            else if (Type.TranslucentInCloak)
                            {
                                // 半透明
                                pAnim.Ref.AnimFlags |= BlitterFlags.TransLucent50;
                            }
                        }
                        break;
                }
            }
            UpdateLocation(location);
        }

        private void UpdateLocation(CoordStruct location)
        {
            if (!pAnim.IsNull)
            {
                // 没有附着在单位上，需要手动同步动画的位置
                pAnim.Ref.Base.SetLocation(location);
            }
        }

        public override void OnTemporalUpdate(TechnoExt ext, Pointer<TemporalClass> pTemporal)
        {
            
        }

        public override void OnRemove(Pointer<ObjectClass> pObject)
        {
            // Logger.Log("{0} 单位{1}隐藏，移除持续动画{2}", Game.CurrentFrame, pObject, Type.IdleAnim);
            KillAnim();
        }

        public override void OnReceiveDamage(Pointer<ObjectClass> pObject, Pointer<int> pDamage, int DistanceFromEpicenter, Pointer<WarheadTypeClass> pWH, Pointer<ObjectClass> pAttacker, bool IgnoreDefenses, bool PreventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        {
            // Logger.Log("播放受击动画{0}", Type.HitAnim);
            // 受击动画
            if (!string.IsNullOrEmpty(Type.HitAnim))
            {
                Pointer<AnimTypeClass> pAnimType = AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(Type.HitAnim);
                if (!pAnimType.IsNull)
                {
                    Pointer<AnimClass> pAnim = YRMemory.Create<AnimClass>(pAnimType, pObject.Ref.Base.GetCoords());
                    pAnim.Ref.SetOwnerObject(pObject);
                    if (pObject.CastToTechno(out Pointer<TechnoClass> pTechno) && !pTechno.Ref.Owner.IsNull)
                    {
                        pAnim.Ref.Owner = pTechno.Ref.Owner;
                    }
                }
            }
        }

        public override void OnDestroy(Pointer<ObjectClass> pOwner)
        {
            // 单位被杀死时，附着动画会自动remove，0x4A9770
            // Logger.Log("{0} 单位{1}被炸死，不移除持续动画{2}", Game.CurrentFrame, pOwner, Type.IdleAnim);
            // this.Disable(pOwner.Ref.Location);
            this.OnwerIsDead = true;
        }

    }

}