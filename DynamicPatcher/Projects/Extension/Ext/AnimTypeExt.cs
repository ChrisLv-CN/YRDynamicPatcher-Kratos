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
    public partial class AnimTypeExt : Extension<AnimTypeClass>
    {
        public static Container<AnimTypeExt, AnimTypeClass> ExtMap = new Container<AnimTypeExt, AnimTypeClass>("AnimTypeClass");

        public AnimTypeExt(Pointer<AnimTypeClass> OwnerObject) : base(OwnerObject)
        {

        }

        protected override void LoadFromINIFile(Pointer<CCINIClass> pINI)
        {
            INIReader reader = new INIReader(pINI);
            string section = OwnerObject.Ref.Base.Base.ID;

        }

        //[Hook(HookType.AresHook, Address = 0x42784B, Size = 5)]
        static public unsafe UInt32 AnimTypeClass_CTOR(REGISTERS* R)
        {
            var pItem = (Pointer<AnimTypeClass>)R->EAX;

            AnimTypeExt.ExtMap.FindOrAllocate(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x428EA8, Size = 5)]
        static public unsafe UInt32 AnimTypeClass_SDDTOR(REGISTERS* R)
        {
            var pItem = (Pointer<AnimTypeClass>)R->ECX;

            AnimTypeExt.ExtMap.Remove(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x4287E9, Size = 0xA)]
        //[Hook(HookType.AresHook, Address = 0x4287DC, Size = 0xA)]
        static public unsafe UInt32 AnimTypeClass_LoadFromINI(REGISTERS* R)
        {
            var pItem = (Pointer<AnimTypeClass>)R->ESI;
            var pINI = R->Stack<Pointer<CCINIClass>>(0xBC);

            AnimTypeExt.ExtMap.LoadFromINI(pItem, pINI);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x428970, Size = 8)]
        //[Hook(HookType.AresHook, Address = 0x428800, Size = 0xA)]
        static public unsafe UInt32 AnimTypeClass_SaveLoad_Prefix(REGISTERS* R)
        {
            var pItem = R->Stack<Pointer<AnimTypeClass>>(0x4);
            var pStm = R->Stack<Pointer<IStream>>(0x8);
            IStream stream = Marshal.GetObjectForIUnknown(pStm) as IStream;

            AnimTypeExt.ExtMap.PrepareStream(pItem, stream);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x42892C, Size = 6)]
        //[Hook(HookType.AresHook, Address = 0x428958, Size = 6)]
        static public unsafe UInt32 AnimTypeClass_Load_Suffix(REGISTERS* R)
        {
            AnimTypeExt.ExtMap.LoadStatic();
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x42898A, Size = 3)]
        static public unsafe UInt32 AnimTypeClass_Save_Suffix(REGISTERS* R)
        {
            AnimTypeExt.ExtMap.SaveStatic();
            return 0;
        }
    }
}
