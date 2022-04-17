
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

                ext?.AttachedComponent.Foreach(c => (c as IObjectScriptable)?.OnUnInit());
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

                ext.AttachedComponent.Foreach(c => c.OnUpdate());
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

    }
}