
using System.Collections;
using System.Drawing;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Script;
using Extension.Utilities;

namespace ExtensionHooks
{
    public class TechnoExtHooks
    {


        [Hook(HookType.AresHook, Address = 0x6F3260, Size = 5)]
        public static unsafe UInt32 TechnoClass_CTOR(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_CTOR(R);
        }

        [Hook(HookType.AresHook, Address = 0x6F4500, Size = 5)]
        public static unsafe UInt32 TechnoClass_DTOR(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_DTOR(R);
        }

        [Hook(HookType.AresHook, Address = 0x70C250, Size = 8)]
        [Hook(HookType.AresHook, Address = 0x70BF50, Size = 5)]
        public static unsafe UInt32 TechnoClass_SaveLoad_Prefix(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_SaveLoad_Prefix(R);
        }

        [Hook(HookType.AresHook, Address = 0x70C249, Size = 5)]
        public static unsafe UInt32 TechnoClass_Load_Suffix(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_Load_Suffix(R);
        }

        [Hook(HookType.AresHook, Address = 0x70C264, Size = 5)]
        public static unsafe UInt32 TechnoClass_Save_Suffix(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_Save_Suffix(R);
        }

        [Hook(HookType.AresHook, Address = 0x6F42ED, Size = 10)]
        public static unsafe UInt32 TechnoClass_Init(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnInit();

                ext?.AttachedComponent.Foreach(c => (c as ITechnoScriptable)?.OnInit());
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        #region After Render
        public static UInt32 TechnoClass_Render2(Pointer<TechnoClass> pTechno)
        {
            try
            {
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext.OnRender2();

                return 0;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return 0;
            }
        }
        [Hook(HookType.AresHook, Address = 0x4149F0, Size = 5)]
        public static unsafe UInt32 AircraftClass_Render2(REGISTERS* R)
        {
            Pointer<AircraftClass> pAircraft = (IntPtr)R->ECX;
            return TechnoClass_Render2(pAircraft.Convert<TechnoClass>());
        }
        // [Hook(HookType.AresHook, Address = 0x43DA6C, Size = 7)]
        // public static unsafe UInt32 BuildingClass_Render2(REGISTERS* R)
        // {
        //     Pointer<BuildingClass> pBuilding = (IntPtr)R->ECX;
        //     return TechnoClass_Render2(pBuilding.Convert<TechnoClass>());
        // }
        [Hook(HookType.AresHook, Address = 0x51961A, Size = 5)]
        public static unsafe UInt32 InfantryClass_Render2(REGISTERS* R)
        {
            Pointer<InfantryClass> pInfantry = (IntPtr)R->ECX;
            return TechnoClass_Render2(pInfantry.Convert<TechnoClass>());
        }
        [Hook(HookType.AresHook, Address = 0x73D410, Size = 5)]
        public static unsafe UInt32 UnitClass_Render2(REGISTERS* R)
        {
            Pointer<UnitClass> pUnit = (IntPtr)R->ECX;
            return TechnoClass_Render2(pUnit.Convert<TechnoClass>());
        }
        #endregion

        [Hook(HookType.AresHook, Address = 0x6F9E50, Size = 5)]
        public static unsafe UInt32 TechnoClass_Update(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                // technoClassUpdate?.Invoke(pTechno, ext);
                ext?.OnUpdate();

                ext?.AttachedComponent.Foreach(c => c?.OnUpdate());
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x71A88D, Size = 0)]
        public static unsafe UInt32 TemporalClass_UpdateA(REGISTERS* R)
        {
            try
            {
                Pointer<TemporalClass> pTemporal = (IntPtr)R->ESI;

                Pointer<TechnoClass> pTechno = pTemporal.Ref.Target;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnTemporalUpdate(pTemporal);

                ext?.AttachedComponent.Foreach(c => (c as ITechnoScriptable)?.OnTemporalUpdate(pTemporal));
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            if ((int)R->EAX <= (int)R->EBX)
            {
                return 0x71A895;
            }
            return 0x71AB08;
        }

        // [Hook(HookType.AresHook, Address = 0x7067E8, Size = 7)]
        // public static unsafe UInt32 TechnoClass_Draw_Voxel(REGISTERS* R)
        // {
        //     Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
        //     pTechno.Ref.Type.Ref.DisableVoxelCache=true;
        //     Logger.Log($"{Game.CurrentFrame} 单位 {pTechno} [{pTechno.Ref.Type.Ref.Base.Base.ID}] DisableVoxelCache {pTechno.Ref.Type.Ref.DisableVoxelCache} Draw vxl {R->Stack<uint>(0x5C)} ZGradient = {pTechno.Ref.GetZGradient()} ZAdjust = {pTechno.Ref.GetZAdjustment()}");
        //     return 0;
        // }

        // [Hook(HookType.AresHook, Address = 0x706866, Size = 5)]
        // public static unsafe UInt32 TechnoClass_voxel_707480(REGISTERS* R)
        // {
        //     Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
        //     Logger.Log($"{Game.CurrentFrame} 地面单位 {pTechno} [{pTechno.Ref.Type.Ref.Base.Base.ID}] DisableVoxelCache {pTechno.Ref.Type.Ref.DisableVoxelCache} Draw vxl {R->Stack<uint>(0x5C)} ZGradient = {pTechno.Ref.GetZGradient()} ZAdjust = {pTechno.Ref.GetZAdjustment()}");
        //     return 0;
        // }

        // [Hook(HookType.AresHook, Address = 0x7068BB, Size = 5)]
        // public static unsafe UInt32 TechnoClass_Draw_Voxel_706ED0(REGISTERS* R)
        // {
        //     Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
        //     Logger.Log($"{Game.CurrentFrame} 浮空单位 {pTechno} [{pTechno.Ref.Type.Ref.Base.Base.ID}] DisableVoxelCache {pTechno.Ref.Type.Ref.DisableVoxelCache} Draw vxl {R->Stack<uint>(0x5C)} ZGradient = {pTechno.Ref.GetZGradient()} ZAdjust = {pTechno.Ref.GetZAdjustment()}");
        //     return 0;
        // }

        // [Hook(HookType.AresHook, Address = 0x707210, Size = 5)]
        // public static unsafe UInt32 OOXXXX2(REGISTERS* R)
        // {
        //     Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
        //     Logger.Log($"{Game.CurrentFrame} 浮空单位 {pTechno.Ref.Type.Ref.Base.Base.ID} 获取色盘 ZGradient = {pTechno.Ref.GetZGradient()} ZAdjust = {pTechno.Ref.GetZAdjustment()}");
        //     return 0;
        // }

        #region Amin ChronoSparkle
        [Hook(HookType.AresHook, Address = 0x414C27, Size = 5)]
        public static unsafe UInt32 AircraftClass_Update_SkipCreateChronoSparkleAnimOnStand(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && !ext.MyMaster.IsNull && null != ext.StandType && ext.StandType.Immune)
                {
                    return 0x414C78;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }
        [Hook(HookType.AresHook, Address = 0x440499, Size = 5)]
        public static unsafe UInt32 BuildingClass_Update_SkipCreateChronoSparkleAnimOnStand1(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && !ext.MyMaster.IsNull && null != ext.StandType && ext.StandType.Immune)
                {
                    return 0x4404D9;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }
        [Hook(HookType.AresHook, Address = 0x44050C, Size = 5)]
        public static unsafe UInt32 BuildingClass_Update_SkipCreateChronoSparkleAnimOnStand2(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && !ext.MyMaster.IsNull && null != ext.StandType && ext.StandType.Immune)
                {
                    return 0x44055D;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }
        [Hook(HookType.AresHook, Address = 0x51BB17, Size = 5)]
        public static unsafe UInt32 InfantryClass_Update_SkipCreateChronoSparkleAnimOnStand(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && !ext.MyMaster.IsNull && null != ext.StandType && ext.StandType.Immune)
                {
                    return 0x51BB6E;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }
        [Hook(HookType.AresHook, Address = 0x736250, Size = 5)]
        public static unsafe UInt32 UnitClass_Update_SkipCreateChronoSparkleAnimOnStand(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && !ext.MyMaster.IsNull && null != ext.StandType && ext.StandType.Immune)
                {
                    return 0x7362A7;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }
        #endregion

        #region UnitClass Deplayed
        // [Hook(HookType.AresHook, Address = 0x739B6A, Size = 6)] // Has Anim
        // [Hook(HookType.AresHook, Address = 0x739C6A, Size = 6)] // No Anim
        // Phobos Skip ↑ those address.
        [Hook(HookType.AresHook, Address = 0x739C74, Size = 6)]
        public static unsafe UInt32 UnitClass_Deployed(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            ext.UnitClass_Deployed_DeployToTransform();
            return 0;
        }
        #endregion

        [Hook(HookType.AresHook, Address = 0x6F6CA0, Size = 7)]
        public static unsafe UInt32 TechnoClass_Put(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
            var pCoord = R->Stack<Pointer<CoordStruct>>(0x4);
            var faceDirValue8 = R->Stack<short>(0x8);

            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            ext?.OnPut(pCoord, faceDirValue8);

            ext?.AttachedComponent.Foreach(c => (c as ITechnoScriptable)?.OnPut(pCoord.Data, faceDirValue8));
            return 0;
        }

        // [Hook(HookType.AresHook, Address = 0x6F6AC0, Size = 5)]
        [Hook(HookType.AresHook, Address = 0x6F6AC4, Size = 5)]
        public static unsafe UInt32 TechnoClass_Remove(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnRemove();

                ext?.AttachedComponent.Foreach(c => (c as ITechnoScriptable)?.OnRemove());
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x701900, Size = 6)]
        public static unsafe UInt32 TechnoClass_ReceiveDamage(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
            var pDamage = R->Stack<Pointer<int>>(0x4);
            var distanceFromEpicenter = R->Stack<int>(0x8);
            var pWH = R->Stack<Pointer<WarheadTypeClass>>(0xC);
            var pAttacker = R->Stack<Pointer<ObjectClass>>(0x10);
            var ignoreDefenses = R->Stack<bool>(0x14);
            var preventPassengerEscape = R->Stack<bool>(0x18);
            var pAttackingHouse = R->Stack<Pointer<HouseClass>>(0x1C);
            // Logger.Log($"{Game.CurrentFrame} - 单位 {pTechno} {pTechno.Ref.Type.Ref.Base.Base.ID} 收到来自 {pAttacker} 伤害死亡, 伤害 {pDamage.Data}, 弹头 {pWH.Ref.Base.ID}");

            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            ext?.OnReceiveDamage(pDamage, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse);

            ext?.AttachedComponent.Foreach(c => (c as ITechnoScriptable)?.OnReceiveDamage(pDamage, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse));
            return 0;
        }
        // after TakeDamage
        [Hook(HookType.AresHook, Address = 0x701DFF, Size = 7)]
        public static unsafe UInt32 TechnoClass_ReceiveDamage2(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
            Pointer<int> pRealDamage = (IntPtr)R->EBX;
            Pointer<WarheadTypeClass> pWH = (IntPtr)R->EBP;
            DamageState damageState = (DamageState)R->EDI;

            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            ext?.OnReceiveDamage2(pRealDamage, pWH, damageState);

            return 0;
        }


        [Hook(HookType.AresHook, Address = 0x702050, Size = 6)]
        public static unsafe UInt32 TechnoClass_Destroy(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                // Logger.Log($"{Game.CurrentFrame} - 单位 {pTechno} {pTechno.Ref.Type.Ref.Base.Base.ID} 受伤害死亡, 所属 {pTechno.Ref.Owner}");
                ext?.OnDestroy();
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }


        [Hook(HookType.AresHook, Address = 0x70256C, Size = 6)]
        public static unsafe UInt32 TechnoClass_Destroy_Debris_Remap(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                Pointer<AnimClass> pAnim = (IntPtr)R->EDI;
                // Logger.Log($"{Game.CurrentFrame} - 单位 {pTechno} [{pTechno.Ref.Type.Ref.Base.Base.ID}] 所属 {pTechno.Ref.Owner} 死亡动画 ECX = {R->ECX} EDI = {R->EDI}");
                if (!pAnim.IsNull)
                {
                    pAnim.Ref.Owner = pTechno.Ref.Owner;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x7024B0, Size = 6)]
        public static unsafe UInt32 TechnoClass_Destroy_Debris_Remap2(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                Pointer<AnimClass> pAnim = (IntPtr)R->EBX;
                // Logger.Log($"{Game.CurrentFrame} - 单位 {pTechno} [{pTechno.Ref.Type.Ref.Base.Base.ID}] 所属 {pTechno.Ref.Owner} 死亡动画2 ECX = {R->ECX} EBX = {R->EBX}");
                if (!pAnim.IsNull)
                {
                    pAnim.Ref.Owner = pTechno.Ref.Owner;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }


        #region Unit explosion anims

        [Hook(HookType.AresHook, Address = 0x737F13, Size = 6)]
        public static unsafe UInt32 UnitClass_Destroy_Debris_Remap(REGISTERS* R)
        {
            try
            {
                Pointer<UnitClass> pUnit = (IntPtr)R->ESI;
                Pointer<AnimClass> pAnim = (IntPtr)R->EAX;
                // Logger.Log($"{Game.CurrentFrame} - 载具 {pUnit} [{pUnit.Ref.Type.Ref.Base.Base.Base.ID}] owner = {pUnit.Ref.Base.Base.Owner} 死亡碎片 ECX = {R->ECX} EAX = {R->EAX}");
                if (!pAnim.IsNull)
                {
                    pAnim.Ref.Owner = pUnit.Ref.Base.Base.Owner;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }
        [Hook(HookType.AresHook, Address = 0x737F66, Size = 5)] // 需要接管
        public static unsafe UInt32 UnitClass_Destroy_Debris_Remap2(REGISTERS* R)
        {
            try
            {
                Pointer<UnitClass> pUnit = (IntPtr)R->ESI;
                Pointer<AnimClass> pAnim = (IntPtr)R->EAX;
                // Logger.Log($"{Game.CurrentFrame} - 载具 {pUnit} [{pUnit.Ref.Type.Ref.Base.Base.Base.ID}] owner = {pUnit.Ref.Base.Base.Owner} 死亡碎片2 ECX = {R->ECX} EAX = {R->EAX}");
                if (!pAnim.IsNull)
                {
                    pAnim.Ref.Owner = pUnit.Ref.Base.Base.Owner;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }
        [Hook(HookType.AresHook, Address = 0x738749, Size = 6)]
        public static unsafe UInt32 UnitClass_Destroy_Explosion_Remap(REGISTERS* R)
        {
            try
            {
                Pointer<UnitClass> pUnit = (IntPtr)R->ESI;
                Pointer<AnimClass> pAnim = (IntPtr)R->EAX;
                // Logger.Log($"{Game.CurrentFrame} - 载具 {pUnit} [{pUnit.Ref.Type.Ref.Base.Base.Base.ID}] owner = {pUnit.Ref.Base.Base.Owner} 死亡动画 ECX = {R->ECX} EAX = {R->EAX}");
                if (!pAnim.IsNull)
                {
                    pAnim.Ref.Owner = pUnit.Ref.Base.Base.Owner;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        // Take over to Create DestroyAnim Anim
        [Hook(HookType.AresHook, Address = 0x738801, Size = 6)]
        public static unsafe UInt32 UnitClass_Destroy_DestroyAnim_Remap(REGISTERS* R)
        {
            try
            {
                Pointer<UnitClass> pUnit = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pUnit.Convert<TechnoClass>());
                ext.DestroyAnim();
                return 0x73887E;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        #endregion

        #region Building explosion anims

        [Hook(HookType.AresHook, Address = 0x441A26, Size = 6)]
        public static unsafe UInt32 BuildingClass_Destroy_Explosion_Remap(REGISTERS* R)
        {
            try
            {
                Pointer<BuildingClass> pBuilding = (IntPtr)R->ESI;
                Pointer<AnimClass> pAnim = (IntPtr)R->EBP;
                // Logger.Log($"{Game.CurrentFrame} - 建筑 {pBuilding} [{pBuilding.Ref.Type.Ref.Base.Base.Base.ID}] owner = {pBuilding.Ref.Base.Owner} 死亡动画 ECX = {R->ECX} EBP = {R->EBP}");
                if (!pAnim.IsNull)
                {
                    pAnim.Ref.Owner = pBuilding.Ref.Base.Owner;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }
        [Hook(HookType.AresHook, Address = 0x441B22, Size = 6)]
        public static unsafe UInt32 BuildingClass_Destroy_Exploding_Remap(REGISTERS* R)
        {
            try
            {
                Pointer<BuildingClass> pBuilding = (IntPtr)R->ESI;
                Pointer<AnimClass> pAnim = (IntPtr)R->EBP;
                // Logger.Log($"{Game.CurrentFrame} - 建筑 {pBuilding} [{pBuilding.Ref.Type.Ref.Base.Base.Base.ID}] owner = {pBuilding.Ref.Base.Owner} 死亡动画2 ECX = {R->ECX} EBP = {R->EBP}");
                if (!pAnim.IsNull)
                {
                    pAnim.Ref.Owner = pBuilding.Ref.Base.Owner;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }
        [Hook(HookType.AresHook, Address = 0x441D25, Size = 0xA)]
        public static unsafe UInt32 BuildingClass_Destroy_DestroyAnim_Remap(REGISTERS* R)
        {
            try
            {
                Pointer<BuildingClass> pBuilding = (IntPtr)R->ESI;
                Pointer<AnimClass> pAnim = (IntPtr)R->EBP;
                // Logger.Log($"{Game.CurrentFrame} - 建筑 {pBuilding} [{pBuilding.Ref.Type.Ref.Base.Base.Base.ID}] owner = {pBuilding.Ref.Base.Owner} 摧毁动画 ECX = {R->ECX} EBP = {R->EBP}");
                if (!pAnim.IsNull)
                {
                    pAnim.Ref.Owner = pBuilding.Ref.Base.Owner;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        #endregion


        [Hook(HookType.AresHook, Address = 0x6FC339, Size = 6)]
        public static unsafe UInt32 TechnoClass_CanFire(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                Pointer<WeaponTypeClass> pWeapon = (IntPtr)R->EDI;
                var pTarget = R->Stack<Pointer<AbstractClass>>(0x20 - (-0x4));

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                bool ceaseFire = false;
                ext?.CanFire(pTarget, pWeapon, ref ceaseFire);
                if (!ceaseFire)
                {
                    ext?.AttachedComponent.Foreach(c => (c as ITechnoScriptable)?.CanFire(pTarget, pWeapon, ref ceaseFire));
                }
                if (ceaseFire)
                {
                    // Logger.Log($"{Game.CurrentFrame} {pTechno} [{pTechno.Ref.Type.Ref.Base.Base.ID}] cease fire !!!");
                    return 0x6FCB7E;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        // 替身需要显示在上层时，修改了渲染的层，导致单位在试图攻击替身时，需要武器具备AA
        [Hook(HookType.AresHook, Address = 0x6FC749, Size = 5)]
        public static unsafe UInt32 TechnoClass_CanFire_WhichLayer_Stand(REGISTERS* R)
        {
            Layer layer = (Layer)R->EAX;
            uint inAir = 0x6FC74E;
            uint onGround = 0x6FC762;
            if (layer != Layer.Ground)
            {
                try
                {
                    Pointer<AbstractClass> pTarget = R->Stack<Pointer<AbstractClass>>(0x20 - (-0x4));
                    if (pTarget.CastToTechno(out Pointer<TechnoClass> pTechno))
                    {
                        TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                        if (null != ext && !ext.MyMaster.IsNull)
                        {
                            if (pTechno.InAir(true))
                            {
                                // in air
                                return inAir;
                            }
                            // on ground
                            return onGround;
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.PrintException(e);
                }
                return inAir;
            }
            return onGround;
        }

        [Hook(HookType.AresHook, Address = 0x6FDD50, Size = 6)]
        public static unsafe UInt32 TechnoClass_Fire(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
                var pTarget = R->Stack<Pointer<AbstractClass>>(0x4);
                var weaponIndex = R->Stack<int>(0x8);

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnFire(pTarget, weaponIndex);

                ext?.AttachedComponent.Foreach(c => (c as ITechnoScriptable)?.OnFire(pTarget, weaponIndex));
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x6FDD61, Size = 5)]
        public static unsafe UInt32 TechnoClass_Fire_OverrideWeapon(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                var pTarget = R->Stack<Pointer<AbstractClass>>(0x4);
                var weaponIndex = R->Stack<int>(0x8);
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext.OverrideWeaponState && ext.OverrideWeaponState.TryGetOverrideWeapon(weaponIndex, pTechno.Ref.Veterancy.IsElite(), out Pointer<WeaponTypeClass> pWeapon))
                {
                    if (!pWeapon.IsNull)
                    {
                        // Logger.Log("Override weapon {0}", pWeapon.Ref.Base.ID);
                        R->EBX = (uint)pWeapon;
                        return 0x6FDD71;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x6FF66C, Size = 6)]
        public static unsafe UInt32 TechnoClass_Fire_DecreaseAmmo(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);

                if (!ext.MyMaster.IsNull && ext.StandType.UseMasterAmmo)
                {
                    ext.MyMaster.Ref.DecreaseAmmo();
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x6FF929, Size = 6)]
        public static unsafe UInt32 TechnoClass_Fire_FireOnce(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnFireOnce();
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x702E9D, Size = 6)]
        public static unsafe UInt32 TechnoClass_RegisterDestruction(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                Pointer<TechnoClass> pKiller = (IntPtr)R->EDI;
                int cost = (int)R->EBP;

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);

                bool skip = false;
                ext?.OnRegisterDestruction(pKiller, cost, ref skip);
                // skip the entire veterancy
                if (skip)
                {
                    return 0x702FF5;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x5F45A0, Size = 5)]
        public static unsafe UInt32 TechnoClass_Select(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->EDI;
                // Logger.Log("{0} Select", pTechno.IsNull ? "Unknow" : pTechno.Ref.Type.Ref.Base.Base.ID);
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                bool selectable = true;
                ext?.OnSelect(ref selectable);

                ext?.AttachedComponent.Foreach(c => (c as ITechnoScriptable)?.OnSelect(ref selectable));
                if (!selectable)
                {
                    return 0x5F45A9;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x6FC018, Size = 6)]
        public static unsafe UInt32 TechnoClass_Select_SkipVoice(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (ext.SkipSelectVoice)
                {
                    return 0x6FC01E;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }


        [Hook(HookType.AresHook, Address = 0x6F65D1, Size = 6)]
        public static unsafe UInt32 TechnoClass_DrawHealthBar_Building(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;

                int length = (int)R->EBX;
                Pointer<Point2D> pLocation = R->Stack<IntPtr>(0x4C - (-0x4));
                Pointer<RectangleStruct> pBound = R->Stack<IntPtr>(0x4C - (-0x8));

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.DrawHealthBar_Building(length, pLocation, pBound);
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x6F683C, Size = 7)]
        public static unsafe UInt32 TechnoClass_DrawHealthBar_Other(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;

                int length = pTechno.Ref.Base.Base.WhatAmI() == AbstractType.Infantry ? 8 : 17;
                Pointer<Point2D> pLocation = R->Stack<IntPtr>(0x4C - (-0x4));
                Pointer<RectangleStruct> pBound = R->Stack<IntPtr>(0x4C - (-0x8));

                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.DrawHealthBar_Other(length, pLocation, pBound);
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        // case VISUAL_NORMAL
        [Hook(HookType.AresHook, Address = 0x7063FF, Size = 7)]
        public static unsafe UInt32 TechnoClass_DrawSHP_Colour(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.DrawSHP_Colour(R);
                // if (!pTechno.IsNull && pTechno.Convert<ObjectClass>().Ref.IsSelected)
                // {
                //     Logger.Log($"{Game.CurrentFrame} - {pTechno.Ref.Type.Ref.Base.Base.ID} tint = {R->EAX}, bright = {R->EBP}");
                // }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x706640, Size = 5)]
        public static unsafe UInt32 TechnoClass_DrawVXL_Colour(REGISTERS* R)
        {
            try
            {
                Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
                // Pointer<Point2D> pPos = R->Stack<IntPtr>(0x18);
                // uint bright = R->Stack<uint>(0x20);
                // uint tint = R->Stack<uint>(0x24);
                // R->Stack<uint>(0x20, 500);
                // R->Stack<uint>(0x24, ExHelper.ColorAdd2RGB565(new ColorStruct(255, 0, 0)));
                // Logger.Log($"{Game.CurrentFrame} - Techno {pTechno} [{pTechno.Ref.Type.Ref.Base.Base.ID}] vxl draw. Pos = {R->Stack<uint>(0x18)}, Bright = {bright}, Tint = {tint}");
                // Only for Building's turret
                if (pTechno.Ref.Base.Base.WhatAmI() == AbstractType.Building)
                {
                    TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                    ext?.DrawVXL_Colour(R, true);
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        // after Techno_DrawVXL change berzerk color
        [Hook(HookType.AresHook, Address = 0x73C15F, Size = 7)]
        public static unsafe UInt32 UnitClass_DrawVXL_Colour(REGISTERS* R)
        {
            try
            {
                Pointer<UnitClass> pUnit = (IntPtr)R->EBP;
                // uint bright = R->Stack<uint>(0x1E0);
                // uint tint = R->ESI;
                // Logger.Log($"{Game.CurrentFrame} - Unit {pUnit.Ref.Type.Ref.Base.Base.Base.ID} vxl draw. Bright = {bright}, Tint = {tint}");

                TechnoExt ext = TechnoExt.ExtMap.Find(pUnit.Convert<TechnoClass>());
                ext?.DrawVXL_Colour(R, false);
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x6F9039, Size = 5)]
        public static unsafe UInt32 TechnoClass_Greatest_Threat_HealWeaponRange(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
            int guardRange = pTechno.Ref.Type.Ref.GuardRange;
            Pointer<WeaponStruct> pirmary = pTechno.Ref.GetWeapon(0);
            if (!pirmary.IsNull && !pirmary.Ref.WeaponType.IsNull)
            {
                int range = pirmary.Ref.WeaponType.Ref.Range;
                if (range > guardRange)
                {
                    guardRange = range;
                }
            }
            Pointer<WeaponStruct> secondary = pTechno.Ref.GetWeapon(1);
            if (!secondary.IsNull && !secondary.Ref.WeaponType.IsNull)
            {
                int range = secondary.Ref.WeaponType.Ref.Range;
                if (range > guardRange)
                {
                    guardRange = range;
                }
            }
            R->EDI = (uint)guardRange;
            return 0x6F903E;
        }

        [Hook(HookType.AresHook, Address = 0x730E8F, Size = 6)]
        public static unsafe UInt32 ObjectClass_GuardCommand(REGISTERS* R)
        {
            Pointer<ObjectClass> pObject = (IntPtr)R->ESI;
            if (pObject.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                // Logger.Log("{0} Guard Command", pTechno.IsNull ? "Unknow" : pTechno.Ref.Type.Ref.Base.Base.ID);
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnGuardCommand();

                ext?.AttachedComponent.Foreach(c => (c as ITechnoScriptable)?.OnGuardCommand());
            }
            return 0;
        }


        [Hook(HookType.AresHook, Address = 0x730F1C, Size = 5)]
        public static unsafe UInt32 ObjectClass_StopCommand(REGISTERS* R)
        {
            Pointer<ObjectClass> pObject = (IntPtr)R->ESI;
            if (pObject.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                // Logger.Log("{0} Stop Command", pTechno.IsNull ? "Unknow" : pTechno.Ref.Type.Ref.Base.Base.ID);
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                ext?.OnStopCommand();

                ext?.AttachedComponent.Foreach(c => (c as ITechnoScriptable)?.OnStopCommand());
            }
            return 0;
        }


        // [Hook(HookType.AresHook, Address = 0x738801, Size = 6)]
        // public static unsafe UInt32 UnitClass_Destroy(REGISTERS* R)
        // {
        //     try
        //     {
        //         Pointer<UnitClass> pUnit = (IntPtr)R->ESI;
        //         //Pointer<ObjectClass> pKiller = (IntPtr)R->EAX;
        //         //Logger.Log("pKill {0}", pKiller.IsNull ? "is null" : (pKiller.Ref.Type.IsNull ? "type is null" : pKiller.Ref.Type.Convert<AbstractTypeClass>().Ref.ID));
        //         TechnoExt ext = TechnoExt.ExtMap.Find(pUnit.Convert<TechnoClass>());
        //         ext?.OnDestroy_UnitClass();
        //     }
        //     catch (Exception e)
        //     {
        //         Logger.PrintException(e);
        //     }
        //     return 0;
        // }

        // Someone wants to enter the cell where I am
        // [Hook(HookType.AresHook, Address = 0x73F528, Size = 6)]
        // public static unsafe UInt32 UnitClass_CanEnterCell(REGISTERS* R)
        // {
        //     try
        //     {
        //         Pointer<TechnoClass> pUnit = (IntPtr)R->EBX;
        //         Pointer<TechnoClass> pOccupier = (IntPtr)R->ESI;

        //         if (pUnit == pOccupier)
        //         {
        //             return 0x73FC10;
        //         }

        //         TechnoExt ext = TechnoExt.ExtMap.Find(pUnit);
        //         if (null != ext)
        //         {
        //             bool ignoreOccupier = false;
        //             ext?.CanEnterCell_UnitClass(pOccupier, ref ignoreOccupier);
        //             if (ignoreOccupier)
        //             {
        //                 return 0x73FC10;
        //             }
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Logger.PrintException(e);
        //     }
        //     return 0x73F530;
        // }

        // Someone wants to enter the cell where I am
        // [Hook(HookType.AresHook, Address = 0x51C251, Size = 6)]
        // public static unsafe UInt32 InfantryClass_CanEnterCell(REGISTERS* R)
        // {
        //     try
        //     {
        //         Pointer<TechnoClass> pInf = (IntPtr)R->EBP;
        //         Pointer<TechnoClass> pOccupier = (IntPtr)R->ESI;

        //         if (pInf == pOccupier)
        //         {
        //             return 0x51C70F;
        //         }

        //         TechnoExt ext = TechnoExt.ExtMap.Find(pInf);
        //         if (null != ext)
        //         {
        //             bool ignoreOccupier = false;
        //             ext?.CanEnterCell_InfantryClass(pOccupier, ref ignoreOccupier);
        //             if (ignoreOccupier)
        //             {
        //                 return 0x51C70F;
        //             }
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Logger.PrintException(e);
        //     }
        //     return 0x51C259;
        // }


        // [Hook(HookType.AresHook, Address= 0x5F5854, Size = 6)]
        // public static unsafe UInt32 ObjectClass_Mark(REGISTERS* R)
        // {
        //     Logger.Log("{0} OOXX {1}", R->ESI, R->Stack<int>(-0x4));
        //     Pointer<ObjectClass> pTechno = (IntPtr)R->ESI;
        //     Logger.Log("{0} OOXX", pTechno.IsNull ? "Unknow" : pTechno.Ref.Type.Ref.Base.ID);
        //     return 0;
        // }

        // [Hook(HookType.AresHook, Address = 0x707557, Size = 6)]
        // public static unsafe UInt32 TechnoClass_DrawVXL(REGISTERS* R)
        // {
        //     Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
        //     Logger.Log("{0} ooxx", pTechno.Ref.Type.Ref.Base.Base.ID);
        //     return 0;
        // }

        [Hook(HookType.AresHook, Address = 0x5F6B90, Size = 5)]
        public static unsafe UInt32 ObjectClass_InAir_SkipCheckIsOnMap(REGISTERS* R)
        {
            Pointer<ObjectClass> pObject = (IntPtr)R->ECX;
            if (pObject.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && !ext.MyMaster.IsNull)
                {
                    return 0x5F6B97;
                }
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x4D94B0, Size = 5)]
        public static unsafe UInt32 TechnoClass_SetDestination_Stand(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->ECX;
            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            if (null != ext && !ext.MyMaster.IsNull)
            {
                // 跳过目的地设置
                // Logger.Log("跳过替身的目的地设置");
                return 0x4D9711;
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x704363, Size = 5)]
        public static unsafe UInt32 TechnoClass_GetZAdjust_Stand(REGISTERS* R)
        {

            Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
            int height = (int)R->EAX;
            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            if (null != ext && !ext.MyMaster.IsNull && (null == ext.StandType || !ext.StandType.IsTrain))
            {
                int ZAdjust = TacticalClass.Instance.Ref.AdjustForZ(height);
                int offset = null != ext.StandType ? ext.StandType.ZOffset : 14;
                // Logger.Log($"{Game.CurrentFrame} - {pTechno} [{pTechno.Ref.Type.Ref.Base.Base.ID}] GetZAdjust EAX = {height}, AdjForZ = {ZAdjust}, offset = {offset}");
                R->ECX = (uint)(ZAdjust + offset);
                // Logger.Log("ZOffset = {0}, ECX = {1}, EAX = {2}", offset, R->ECX, R->EAX);
                return 0x704368;
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x4DB7F7, Size = 6)]
        public static unsafe UInt32 FootClass_In_Which_Layer_Stand(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->ESI;
            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            if (null != ext && !ext.MyMaster.IsNull && null != ext.StandType && !ext.StandType.IsTrain && ext.StandType.ZOffset != 0)
            {
                // Logger.Log($"{Game.CurrentFrame} - {pTechno} [{pTechno.Ref.Type.Ref.Base.Base.ID}] StandType.DrawLayer = {ext.StandType.DrawLayer}, StandType.ZOffset = {ext.StandType.ZOffset}");
                Layer layer = ext.StandType.DrawLayer;
                if (layer == Layer.None)
                {
                    // Logger.Log($" - {Game.CurrentFrame} - {pTechno} [{pTechno.Ref.Type.Ref.Base.Base.ID}] EAX = {(Layer)R->EAX} InAir = {pTechno.Ref.Base.Base.IsInAir()}");
                    R->EAX = pTechno.Ref.Base.Base.IsInAir() ? (uint)Layer.Top : (uint)Layer.Air;
                }
                else
                {
                    R->EAX = (uint)layer;
                }
                return 0x4DB803;
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x54B8E9, Size = 6)]
        public static unsafe UInt32 JumpjetLocomotionClass_In_Which_Layer_Deviation(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)R->EAX;
            if (pTechno.Ref.Base.Base.IsInAir())
            {
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && ext.AttachEffectManager.HasStand())
                {
                    // Override JumpjetHeight / CruiseHeight check so it always results in 3 / Layer::Air.
                    R->EDX = Int32.MaxValue;
                    return 0x54B96B;
                }
            }
            return 0;
        }


    }
}