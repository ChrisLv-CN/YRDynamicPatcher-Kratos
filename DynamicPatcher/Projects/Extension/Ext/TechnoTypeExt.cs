using DynamicPatcher;
using Extension.Script;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    [Serializable]
    public partial class TechnoTypeExt : Extension<TechnoTypeClass>
    {
        public static Container<TechnoTypeExt, TechnoTypeClass> ExtMap = new Container<TechnoTypeExt, TechnoTypeClass>("TechnoTypeClass");

        public List<Script.Script> Scripts;

        public TechnoTypeExt(Pointer<TechnoTypeClass> OwnerObject) : base(OwnerObject)
        {

        }

        protected override void LoadFromINIFile(Pointer<CCINIClass> pINI)
        {
            INI_EX exINI = new INI_EX(pINI);
            INIReader reader = new INIReader(exINI);
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

        //public override void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    base.GetObjectData(info, context);

        //    info.AddValue("ScriptName", Script?.Name);
        //}
        //protected TechnoTypeExt(SerializationInfo info, StreamingContext context) : base(info, context)
        //{
        //    string scriptName = info.GetString("ScriptName");
        //    Script = ScriptManager.GetScript<TechnoScript>(scriptName);
        //}

        //[Hook(HookType.AresHook, Address = 0x711835, Size = 5)]
        static public unsafe UInt32 TechnoTypeClass_CTOR(REGISTERS* R)
        {
            var pItem = (Pointer<TechnoTypeClass>)R->ESI;

            TechnoTypeExt.ExtMap.FindOrAllocate(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x711AE0, Size = 5)]
        static public unsafe UInt32 TechnoTypeClass_DTOR(REGISTERS* R)
        {
            var pItem = (Pointer<TechnoTypeClass>)R->ECX;

            TechnoTypeExt.ExtMap.Remove(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x716132, Size = 5)]
        //[Hook(HookType.AresHook, Address = 0x716123, Size = 5)]
        static public unsafe UInt32 TechnoTypeClass_LoadFromINI(REGISTERS* R)
        {
            var pItem = (Pointer<TechnoTypeClass>)R->EBP;
            var pINI = R->Stack<Pointer<CCINIClass>>(0x380);

            TechnoTypeExt.ExtMap.LoadFromINI(pItem, pINI);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x716DC0, Size = 5)]
        //[Hook(HookType.AresHook, Address = 0x7162F0, Size = 6)]
        static public unsafe UInt32 TechnoTypeClass_SaveLoad_Prefix(REGISTERS* R)
        {
            var pItem = R->Stack<Pointer<TechnoTypeClass>>(0x4);
            var pStm = R->Stack<Pointer<IStream>>(0x8);
            IStream stream = Marshal.GetObjectForIUnknown(pStm) as IStream;

            TechnoTypeExt.ExtMap.PrepareStream(pItem, stream);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x716DAC, Size = 0xA)]
        static public unsafe UInt32 TechnoTypeClass_Load_Suffix(REGISTERS* R)
        {
            TechnoTypeExt.ExtMap.LoadStatic();
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x717094, Size = 5)]
        static public unsafe UInt32 TechnoTypeClass_Save_Suffix(REGISTERS* R)
        {
            TechnoTypeExt.ExtMap.SaveStatic();
            return 0;
        }
    }

}
