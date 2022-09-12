using DynamicPatcher;
using Extension.Script;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    public partial class AttachEffectManager
    {
        // 强制替身同步
        // 染色
        public PaintballState PaintballState = new PaintballState();

        // 可分离替身与JOJO
        // 自毁
        public DestroySelfState DestroySelfState = new DestroySelfState();
        // 礼盒
        public GiftBoxState GiftBoxState = new GiftBoxState();
        // 禁武
        public AEState<DisableWeaponType> DisableWeaponState = new AEState<DisableWeaponType>();
        // 替武
        public OverrideWeaponState OverrideWeaponState = new OverrideWeaponState();
        // 发射超武
        public AEState<FireSuperType> FireSuperState = new AEState<FireSuperType>();
        // 不可选择
        public AEState<DeselectType> DeselectState = new AEState<DeselectType>();
        // 伤害响应
        public DamageReactionState DamageReactionState = new DamageReactionState();

        public void EnableAEStatsToStand(int duration, string token, IAEStateData data)
        {
            foreach (AttachEffect ae in AttachEffects)
            {
                Stand stand = ae.Stand;
                if (null != stand && ae.IsActive())
                {
                    Pointer<TechnoClass> pStand = stand.pStand;
                    TechnoExt ext = TechnoExt.ExtMap.Find(pStand);
                    if (null != ext)
                    {
                        // Logger.Log($"{Game.CurrentFrame} - 同步开启AE {ae.Name} 的替身状态 {data.GetType().Name} token {token}");
                        if (data is DestroySelfType)
                        {
                            // 自毁
                            ext.AttachEffectManager.DestroySelfState.Enable(duration, token, data);
                        }
                        else if (data is GiftBoxType)
                        {
                            // 同步礼盒
                            ext.AttachEffectManager.GiftBoxState.Enable(duration, token, data);
                        }
                        else if (data is DisableWeaponType)
                        {
                            // 同步禁武
                            ext.AttachEffectManager.DisableWeaponState.Enable(duration, token, data);
                        }
                        else if (data is OverrideWeaponType)
                        {
                            // 同步替武
                            ext.AttachEffectManager.OverrideWeaponState.Enable(duration, token, data);
                        }
                        else if (data is FireSuperType)
                        {
                            // 同步发射超武
                            ext.AttachEffectManager.FireSuperState.Enable(duration, token, data);
                        }
                        else if (data is DeselectType)
                        {
                            // 同步禁止选择
                            ext.AttachEffectManager.DeselectState.Enable(duration, token, data);
                        }
                    }
                }
            }
        }

    }

}
