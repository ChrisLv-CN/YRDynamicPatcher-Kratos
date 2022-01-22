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
    [Serializable]
    public partial class WeaponTypeExt : Extension<WeaponTypeClass>
    {
        public static Container<WeaponTypeExt, WeaponTypeClass> ExtMap = new Container<WeaponTypeExt, WeaponTypeClass>("WeaponTypeClass");

        public WeaponTypeExt(Pointer<WeaponTypeClass> OwnerObject) : base(OwnerObject)
        {

        }

        protected override void LoadFromINIFile(Pointer<CCINIClass> pINI)
        {
            INIReader reader = new INIReader(pINI);
            string section = OwnerObject.Ref.Base.ID;

        }

        //[Hook(HookType.AresHook, Address = 0x771EE9, Size = 5)]
        public static unsafe UInt32 WeaponTypeClass_CTOR(REGISTERS* R)
        {
            var pItem = (Pointer<WeaponTypeClass>)R->ESI;

            WeaponTypeExt.ExtMap.FindOrAllocate(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x77311D, Size = 6)]
        public static unsafe UInt32 WeaponTypeClass_SDDTOR(REGISTERS* R)
        {
            var pItem = (Pointer<WeaponTypeClass>)R->ESI;

            WeaponTypeExt.ExtMap.Remove(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x7729C7, Size = 5)]
        //[Hook(HookType.AresHook, Address = 0x7729D6, Size = 5)]
        //[Hook(HookType.AresHook, Address = 0x7729B0, Size = 5)]
        public static unsafe UInt32 WeaponTypeClass_LoadFromINI(REGISTERS* R)
        {
            var pItem = (Pointer<WeaponTypeClass>)R->ESI;
            var pINI = R->Stack<Pointer<CCINIClass>>(0xE4);

            WeaponTypeExt.ExtMap.LoadFromINI(pItem, pINI);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x772EB0, Size = 5)]
        //[Hook(HookType.AresHook, Address = 0x772CD0, Size = 7)]
        public static unsafe UInt32 WeaponTypeClass_SaveLoad_Prefix(REGISTERS* R)
        {
            var pItem = R->Stack<Pointer<WeaponTypeClass>>(0x4);
            var pStm = R->Stack<Pointer<IStream>>(0x8);
            IStream stream = Marshal.GetObjectForIUnknown(pStm) as IStream;

            WeaponTypeExt.ExtMap.PrepareStream(pItem, stream);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x772EA6, Size = 6)]
        public static unsafe UInt32 WeaponTypeClass_Load_Suffix(REGISTERS* R)
        {
            WeaponTypeExt.ExtMap.LoadStatic();
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x772F8C, Size = 5)]
        public static unsafe UInt32 WeaponTypeClass_Save_Suffix(REGISTERS* R)
        {
            WeaponTypeExt.ExtMap.SaveStatic();
            return 0;
        }
    }
}
