
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
    public class AnimExtHooks
    {
        [Hook(HookType.AresHook, Address = 0x422126, Size = 5)]
        [Hook(HookType.AresHook, Address = 0x422707, Size = 5)]
        [Hook(HookType.AresHook, Address = 0x4228D2, Size = 5)]
        public static unsafe UInt32 AnimClass_CTOR(REGISTERS* R)
        {
            return AnimExt.AnimClass_CTOR(R);
        }

        [Hook(HookType.AresHook, Address = 0x422967, Size = 6)]
        public static unsafe UInt32 AnimClass_DTOR(REGISTERS* R)
        {
            try
            {
                Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
                AnimExt ext = AnimExt.ExtMap.Find(pAnim);
                ext?.OnUnInit();

                // ext?.AttachedComponent.Foreach(c => (c as IObjectScriptable)?.OnUnInit());
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return AnimExt.AnimClass_DTOR(R);
        }

        [Hook(HookType.AresHook, Address = 0x425280, Size = 5)]
        [Hook(HookType.AresHook, Address = 0x4253B0, Size = 5)]
        public static unsafe UInt32 AnimClass_SaveLoad_Prefix(REGISTERS* R)
        {
            return AnimExt.AnimClass_SaveLoad_Prefix(R);
        }

        [Hook(HookType.AresHook, Address = 0x425391, Size = 7)]
        [Hook(HookType.AresHook, Address = 0x4253A2, Size = 7)]
        [Hook(HookType.AresHook, Address = 0x425358, Size = 7)]
        public static unsafe UInt32 AnimClass_Load_Suffix(REGISTERS* R)
        {
            return AnimExt.AnimClass_Load_Suffix(R);
        }

        [Hook(HookType.AresHook, Address = 0x4253FF, Size = 5)]
        public static unsafe UInt32 AnimClass_Save_Suffix(REGISTERS* R)
        {
            return AnimExt.AnimClass_Save_Suffix(R);
        }


        [Hook(HookType.AresHook, Address = 0x423AC0, Size = 6)]
        public static unsafe UInt32 AnimClass_Update(REGISTERS* R)
        {
            Pointer<AnimClass> pAnim = (IntPtr)R->ECX;
            try
            {
                AnimExt ext = AnimExt.ExtMap.Find(pAnim);
                ext?.OnUpdate();

                // ext.AttachedComponent.Foreach(c => c.OnUpdate());
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        // Takes over all damage from animations, including Ares
        [Hook(HookType.AresHook, Address = 0x424538, Size = 6)] // 动画伤害
        public static unsafe UInt32 AnimClass_Update_Explosion(REGISTERS* R)
        {
            try
            {
                if (RulesExt.Instance.AllowAnimDamageTakeOverByKratos)
                {
                    Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
                    AnimExt ext = AnimExt.ExtMap.Find(pAnim);
                    ext.OnDamage();
                    // Logger.Log($"{Game.CurrentFrame} - {pAnim} [{pAnim.Ref.Type.Ref.Base.Base.ID}] 爆炸啦");
                    return 0x42464C;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x424785, Size = 6)]
        public static unsafe UInt32 AnimClass_Loop(REGISTERS* R)
        {
            Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
            try
            {
                AnimExt ext = AnimExt.ExtMap.Find(pAnim);
                ext?.OnLoop();
                // Logger.Log($"{Game.CurrentFrame} - {pAnim.Ref.Type.Ref.Base.Base.ID}循环完毕，剩余{pAnim.Ref.Loops}");
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x4247F3, Size = 6)]
        [Hook(HookType.AresHook, Address = 0x424298, Size = 6)] // Meteor hit ground or water
        public static unsafe UInt32 AnimClass_Done(REGISTERS* R)
        {
            Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
            try
            {
                AnimExt ext = AnimExt.ExtMap.Find(pAnim);
                ext?.OnDone();

                // string animID = pAnim.Ref.Type.Ref.Base.Base.ID;
                // if (!animID.StartsWith("FIRE") && animID.IndexOf("SMOKE") < 0)
                // {
                //     Logger.Log($"{Game.CurrentFrame} - {pAnim} [{pAnim.Ref.Type.Ref.Base.Base.ID}] 播放完毕, 所属 {pAnim.Ref.Owner}");
                // }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x424807, Size = 6)]
        public static unsafe UInt32 AnimClass_Next(REGISTERS* R)
        {
            Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
            Pointer<AnimTypeClass> pNextAnimType = (IntPtr)R->ECX;
            try
            {
                AnimExt ext = AnimExt.ExtMap.Find(pAnim);
                ext?.OnNext();
                // Logger.Log($"{Game.CurrentFrame} - {pAnim} {pAnim.Ref.Type.Ref.Base.Base.ID}播放结束，Next动画[{pNextAnimType.Ref.Base.Base.ID}] 所属 {pAnim.Ref.Owner}");
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }


        [Hook(HookType.AresHook, Address = 0x422CA0, Size = 5)]
        public static unsafe UInt32 AnimClass_Render(REGISTERS* R)
        {
            Pointer<AnimClass> pAnim = (IntPtr)R->ECX;
            try
            {
                AnimExt ext = AnimExt.ExtMap.Find(pAnim);
                ext?.OnRender();

                ext.AttachedComponent.Foreach(c => c.OnRender());
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }


        [Hook(HookType.AresHook, Address = 0x423630, Size = 6)]
        public static unsafe UInt32 AnimClass_Draw_It(REGISTERS* R)
        {
            //Logger.Log("Hook 0x00423130 calling...");
            Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
            // R->EBP = ExHelper.ColorAdd2RGB565(new ColorStruct(255, 0, 0));
            // R->Stack<uint>(0x38, 500);
            // Logger.Log($"{Game.CurrentFrame} {R->Stack<uint>(0xFC)} {R->Stack<uint>(0x14 + 0x20 + 0x1C)} {R->Stack<uint>(0x14 - 0x20 - 0x1C)}");
            // Logger.Log($"{Game.CurrentFrame} - {pAnim.Ref.Type.Ref.Base.Base.ID} is Drawing. color = {R->EBP}, bright = {R->Stack<int>(0x38)}, pTechno= {R->Stack<uint>(0x14)}");
            if (!pAnim.IsNull && pAnim.Ref.IsBuildingAnim)
            {
                CoordStruct location = pAnim.Ref.Base.Base.GetCoords();
                if (MapClass.Instance.TryGetCellAt(location, out Pointer<CellClass> pCell))
                {
                    Pointer<BuildingClass> pBuilding = pCell.Ref.GetBuilding();
                    if (!pBuilding.IsNull)
                    {
                        TechnoExt ext = TechnoExt.ExtMap.Find(pBuilding.Convert<TechnoClass>());
                        ext?.TechnoClass_DrawSHP_Paintball_BuildAnim(R);
                    }
                }
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x42312A, Size = 6)]
        public static unsafe UInt32 AnimClass_Draw_Remap(REGISTERS* R)
        {
            //Logger.Log("Hook 0x00423130 calling...");
            Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
            if (!pAnim.IsNull && pAnim.Ref.Type.Ref.AltPalette && !pAnim.Ref.Owner.IsNull)
            {
                // string id = pAnim.Ref.Owner.IsNull ? "NULL" : pAnim.Ref.Owner.Ref.Type.Ref.Base.ID;
                // int index = pAnim.Ref.Owner.IsNull ? -1 : pAnim.Ref.Owner.Ref.ArrayIndex;
                // ColorStruct color = pAnim.Ref.Owner.IsNull ? default : pAnim.Ref.Owner.Ref.Color;
                // Logger.Log(" 强转所属 Anim[{0}] 从所属中{1}-{2}获取颜色. Colour={3}, pHouse={4}", pAnim.Ref.Type.Convert<AbstractTypeClass>().Ref.ID, index, id, color, pAnim.Ref.Owner);
                return 0x423130;
            }
            return 0x4231F3;
        }

        [Hook(HookType.AresHook, Address = 0x423136, Size = 6)]
        public static unsafe UInt32 AnimClass_Draw_Remap2(REGISTERS* R)
        {
            Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
            if (!pAnim.IsNull && pAnim.Ref.Type.Ref.AltPalette && !pAnim.Ref.Owner.IsNull)
            {
                // var edx = R->EDX;
                // var ecx = R->ECX;
                R->ECX = (uint)pAnim.Ref.Owner;
                // Logger.Log(" - Anim[{0}] ECX={1}, EDX={2}", pAnim.Ref.Type.Convert<AbstractTypeClass>().Ref.ID, ecx, edx);
            }
            return 0;
        }


        [Hook(HookType.AresHook, Address = 0x423E75, Size = 6)]
        public static unsafe UInt32 AnimClass_Extras_Remap(REGISTERS* R)
        {
            Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
            Pointer<AnimClass> pNewAnim = (IntPtr)R->EDI;
            pNewAnim.Ref.Owner = pAnim.Ref.Owner;
            // Logger.Log($"{Game.CurrentFrame} - {pAnim} [{pAnim.Ref.Type.Ref.Base.Base.ID}] Extras ECX = {R->ECX} EDI = {R->EDI} 所属 {pAnim.Ref.Owner}");
            return 0;
        }

        // Takes over all damage from animations, including Ares
        [Hook(HookType.AresHook, Address = 0x423E7B, Size = 0xA)]
        public static unsafe UInt32 AnimClass_Extras_Explosion(REGISTERS* R)
        {
            try
            {
                // 碎片、流星敲地板，砸水中不执行
                if (RulesExt.Instance.AllowAnimDamageTakeOverByKratos)
                {
                    Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
                    AnimExt ext = AnimExt.ExtMap.Find(pAnim);
                    ext.OnDebrisDamage();
                    // Logger.Log($"{Game.CurrentFrame} - {pAnim} [{pAnim.Ref.Type.Ref.Base.Base.ID}] 碎片敲地板爆炸啦");
                    return 0x423EFD;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        // Take over to create Extras Anim when Meteor hit the water
        [Hook(HookType.AresHook, Address = 0x423CEA, Size = 5)]
        public static unsafe UInt32 AnimClass_Extras_HitWater_Meteor(REGISTERS* R)
        {
            try
            {
                Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
                AnimExt ext = AnimExt.ExtMap.Find(pAnim);
                if (RulesExt.Instance.AllowAnimDamageTakeOverByKratos)
                {
                    ext.HitWater_Meteor();
                }
                if (ext.OverrideExpireAnimOnWater())
                {
                    // Logger.Log($"{Game.CurrentFrame} - {pAnim} {pAnim.Ref.Type.Ref.Base.Base.ID} Extras {R->EDI} {R->EAX} ");
                    R->EAX = 0;
                    return 0x423CEF;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        // Take over to create Extras Anim when Debris hit the water
        [Hook(HookType.AresHook, Address = 0x423D46, Size = 5)]
        public static unsafe UInt32 AnimClass_Extras_HitWater_Other(REGISTERS* R)
        {
            try
            {
                Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
                AnimExt ext = AnimExt.ExtMap.Find(pAnim);
                if (RulesExt.Instance.AllowAnimDamageTakeOverByKratos)
                {
                    ext.HitWater_Debris();
                }
                if (ext.OverrideExpireAnimOnWater())
                {
                    // Logger.Log($"{Game.CurrentFrame} - {pAnim} {pAnim.Ref.Type.Ref.Base.Base.ID} Extras {R->EDI} {R->EAX} ");
                    R->EAX = 0;
                    return 0x423D9B;
                }
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        // Take over to Create Spawn Anim
        [Hook(HookType.AresHook, Address = 0x423F8C, Size = 5)]
        public static unsafe UInt32 AnimClass_Spawn_Remap(REGISTERS* R)
        {
            try
            {
                Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
                // Pointer<AnimClass> pNewAnim = (IntPtr)R->EAX;
                if (!pAnim.Ref.Type.IsNull && !pAnim.Ref.Type.Ref.Spawns.IsNull)
                {
                    Pointer<AnimClass> pNewAnim = YRMemory.Create<AnimClass>(pAnim.Ref.Type.Ref.Spawns, pAnim.Ref.Base.Base.GetCoords());
                    pNewAnim.Ref.Owner = pAnim.Ref.Owner;
                }
                // Logger.Log($"{Game.CurrentFrame} - {pAnim} {pAnim.Ref.Type.Ref.Base.Base.ID} Spawns {pAnim.Ref.Type.Ref.Spawns.Ref.Base.Base.ID} owner {pAnim.Ref.Owner} ");
                return 0x423FC3;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

        // Take over to Create Trailer Anim
        [Hook(HookType.AresHook, Address = 0x4242E1, Size = 5)]
        public static unsafe UInt32 AnimClass_Trailer_Remap(REGISTERS* R)
        {
            try
            {
                Pointer<AnimClass> pAnim = (IntPtr)R->ESI;
                if (!pAnim.Ref.Type.IsNull && !pAnim.Ref.Type.Ref.TrailerAnim.IsNull)
                {
                    Pointer<AnimClass> pNewAnim = YRMemory.Create<AnimClass>(pAnim.Ref.Type.Ref.TrailerAnim, pAnim.Ref.Base.Base.GetCoords());
                    pNewAnim.Ref.Owner = pAnim.Ref.Owner;
                }
                return 0x424322;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
            }
            return 0;
        }

    }
}