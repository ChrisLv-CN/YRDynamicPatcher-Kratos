using DynamicPatcher;
using Extension.Script;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    [Serializable]
    public partial class BulletTypeExt : Extension<BulletTypeClass>
    {
        public static Container<BulletTypeExt, BulletTypeClass> ExtMap = new Container<BulletTypeExt, BulletTypeClass>("BulletTypeClass");

        public List<Script.Script> Scripts;

        public BulletTypeExt(Pointer<BulletTypeClass> OwnerObject) : base(OwnerObject)
        {

        }

        protected override void LoadFromINIFile(Pointer<CCINIClass> pINI)
        {
            INIReader reader = new INIReader(pINI);
            string section = OwnerObject.Ref.Base.Base.ID;

            reader.ReadScripts(section, "Scripts", ref Scripts);
        }

        public override void SaveToStream(IStream stream)
        {
            base.SaveToStream(stream);
        }
        public override void LoadFromStream(IStream stream)
        {
            base.LoadFromStream(stream);
        }

        //[Hook(HookType.AresHook, Address = 0x46BDD9, Size = 5)]
        static public unsafe UInt32 BulletTypeClass_CTOR(REGISTERS* R)
        {
            var pItem = (Pointer<BulletTypeClass>)R->EAX;

            BulletTypeExt.ExtMap.FindOrAllocate(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x46C8B6, Size = 6)]
        static public unsafe UInt32 BulletTypeClass_SDDTOR(REGISTERS* R)
        {
            var pItem = (Pointer<BulletTypeClass>)R->ESI;

            BulletTypeExt.ExtMap.Remove(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x46C429, Size = 0xA)]
        //[Hook(HookType.AresHook, Address = 0x46C41C, Size = 0xA)]
        static public unsafe UInt32 BulletTypeClass_LoadFromINI(REGISTERS* R)
        {
            var pItem = (Pointer<BulletTypeClass>)R->ESI;
            var pINI = R->Stack<Pointer<CCINIClass>>(0x90);

            BulletTypeExt.ExtMap.LoadFromINI(pItem, pINI);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x46C730, Size = 8)]
        //[Hook(HookType.AresHook, Address = 0x46C6A0, Size = 5)]
        static public unsafe UInt32 BulletTypeClass_SaveLoad_Prefix(REGISTERS* R)
        {
            var pItem = R->Stack<Pointer<BulletTypeClass>>(0x4);
            var pStm = R->Stack<Pointer<IStream>>(0x8);
            IStream stream = Marshal.GetObjectForIUnknown(pStm) as IStream;

            BulletTypeExt.ExtMap.PrepareStream(pItem, stream);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x46C722, Size = 4)]
        static public unsafe UInt32 BulletTypeClass_Load_Suffix(REGISTERS* R)
        {
            BulletTypeExt.ExtMap.LoadStatic();
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x46C74A, Size = 3)]
        static public unsafe UInt32 BulletTypeClass_Save_Suffix(REGISTERS* R)
        {
            BulletTypeExt.ExtMap.SaveStatic();
            return 0;
        }
    }
}

