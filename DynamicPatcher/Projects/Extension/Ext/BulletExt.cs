using DynamicPatcher;
using Extension.Components;
using Extension.Decorators;
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
    public partial class BulletExt : Extension<BulletClass>, IHaveComponent
    {
        public static Container<BulletExt, BulletClass> ExtMap = new Container<BulletExt, BulletClass>("BulletClass");

        ExtensionReference<BulletTypeExt> type;
        public BulletTypeExt Type
        {
            get
            {
                if (type.TryGet(out BulletTypeExt ext) == false)
                {
                    type.Set(OwnerObject.Ref.Type);
                    ext = type.Get();
                }
                return ext;
            }
        }

        private ExtComponent<BulletExt> _extComponent;
        private DecoratorComponent _decoratorComponent;
        public ExtComponent<BulletExt> ExtComponent => _extComponent.GetAwaked();
        public DecoratorComponent DecoratorComponent => _decoratorComponent;
        public Component AttachedComponent => ExtComponent;

        public BulletExt(Pointer<BulletClass> OwnerObject) : base(OwnerObject)
        {
            _extComponent = new ExtComponent<BulletExt>(this, 0, "BulletExt root component");
            _decoratorComponent = new DecoratorComponent();
            _extComponent.OnAwake += () => ScriptManager.CreateScriptableTo(_extComponent, Type.Scripts, this);
            _extComponent.OnAwake += () => _decoratorComponent.AttachToComponent(_extComponent);
        }

        public override void SaveToStream(IStream stream)
        {
            base.SaveToStream(stream);
            _extComponent.Foreach(c => c.SaveToStream(stream));
        }

        public override void LoadFromStream(IStream stream)
        {
            base.LoadFromStream(stream);
            _extComponent.Foreach(c => c.LoadFromStream(stream));
        }

        //[Hook(HookType.AresHook, Address = 0x4664BA, Size = 5)]
        static public unsafe UInt32 BulletClass_CTOR(REGISTERS* R)
        {
            var pItem = (Pointer<BulletClass>)R->ESI;

            BulletExt.ExtMap.FindOrAllocate(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x4665E9, Size = 0xA)]
        static public unsafe UInt32 BulletClass_DTOR(REGISTERS* R)
        {
            var pItem = (Pointer<BulletClass>)R->ESI;

            BulletExt.ExtMap.Remove(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x46AFB0, Size = 8)]
        //[Hook(HookType.AresHook, Address = 0x46AE70, Size = 5)]
        static public unsafe UInt32 BulletClass_SaveLoad_Prefix(REGISTERS* R)
        {
            var pItem = R->Stack<Pointer<BulletClass>>(0x4);
            var pStm = R->Stack<Pointer<IStream>>(0x8);
            IStream stream = Marshal.GetObjectForIUnknown(pStm) as IStream;

            BulletExt.ExtMap.PrepareStream(pItem, stream);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x46AF97, Size = 7)]
        //[Hook(HookType.AresHook, Address = 0x46AF9E, Size = 7)]
        static public unsafe UInt32 BulletClass_Load_Suffix(REGISTERS* R)
        {
            BulletExt.ExtMap.LoadStatic();
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x46AFC4, Size = 3)]
        static public unsafe UInt32 BulletClass_Save_Suffix(REGISTERS* R)
        {
            BulletExt.ExtMap.SaveStatic();
            return 0;
        }
    }
}
