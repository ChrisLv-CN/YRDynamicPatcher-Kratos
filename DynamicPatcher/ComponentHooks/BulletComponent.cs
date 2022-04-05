using DynamicPatcher;
using Extension.Ext;
using Extension.Script;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentHooks
{
    public class BulletComponentHooks
    {
        // Kratos moved to ExtensionHook
        // [Hook(HookType.AresHook, Address = 0x4666F7, Size = 6)]
        public static unsafe UInt32 BulletClass_Update_Components(REGISTERS* R)
        {
            try
            {
                Pointer<BulletClass> pBullet = (IntPtr)R->EBP;

                BulletExt ext = BulletExt.ExtMap.Find(pBullet);
                ext.AttachedComponent.Foreach(c => c.OnUpdate());

                return 0;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return 0;
            }
        }

        // Kratos moved to ExtensionHook
        // [Hook(HookType.AresHook, Address = 0x4690B0, Size = 6)]
        public static unsafe UInt32 BulletClass_Detonate_Components(REGISTERS* R)
        {
            try
            {
                Pointer<BulletClass> pBullet = (IntPtr)R->ECX;
                var pCoords = R->Stack<Pointer<CoordStruct>>(0x4);

                BulletExt ext = BulletExt.ExtMap.Find(pBullet);
                ext.AttachedComponent.Foreach(c => (c as IBulletScriptable)?.OnDetonate(pCoords));

                return 0;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return 0;
            }
        }

        #region Render
        [Hook(HookType.AresHook, Address = 0x468090, Size = 6)]
        public static unsafe UInt32 BulletClass_Render_Components(REGISTERS* R)
        {
            try
            {
                Pointer<BulletClass> pBullet = (IntPtr)R->ECX;

                BulletExt ext = BulletExt.ExtMap.Find(pBullet);
                ext.OnRender();
                ext.AttachedComponent.Foreach(c => c.OnRender());

                return 0;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return 0;
            }
        }
        #endregion
    }
}
