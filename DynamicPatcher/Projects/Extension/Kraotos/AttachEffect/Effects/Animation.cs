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

        private bool OnwerIsDead;

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
            // Logger.Log("播放持续动画{0}", Type.IdleAnim);
            if (!pAnim.IsNull)
            {
                // Logger.Log("创建动画{0}时动画已存在，清除再创建{0}", Type.IdleAnim);
                KillAnim();
            }
            // 创建动画
            if (!pObject.IsNull && pAnim.IsNull)
            {
                // Logger.Log("创建持续动画{0}", Type.IdleAnim);
                Pointer<AnimTypeClass> pAnimType = AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(Type.IdleAnim);
                if (!pAnimType.IsNull)
                {
                    Pointer<AnimClass> pAnim = YRMemory.Create<AnimClass>(pAnimType, pObject.Ref.Base.GetCoords());
                    // Logger.Log(" - 成功创建动画{0}实例", Type.IdleAnim);
                    pAnim.Ref.SetOwnerObject(pObject);
                    // Logger.Log(" - 将动画{0}赋予对象", Type.IdleAnim);
                    pAnim.Ref.Loops = 0xFF;
                    // Logger.Log(" - 设置动画{0}的剩余迭代次数为{1}", Type.IdleAnim, 0xFF);
                    pAnim.SetAnimOwner(pObject);
                    this.pAnim.Pointer = pAnim;
                    // Logger.Log(" - 缓存动画{0}的实例对象指针", Type.IdleAnim);
                }
            }
        }

        public override void Disable(CoordStruct location)
        {
            // Logger.Log("效果结束，移除持续动画{0}", Type.IdleAnim);
            KillAnim();
            // Logger.Log("播放结束动画{0}", Type.DoneAnim);
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
                if (!OnwerIsDead)
                {
                    pAnim.Ref.TimeToDie = true;
                    pAnim.Ref.Base.UnInit(); // 包含了SetOwnerObject(0) 0x4255B0
                }
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

        public override void OnUpdate(Pointer<ObjectClass> pOwner, bool isDead)
        {
            this.OnwerIsDead = isDead;
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