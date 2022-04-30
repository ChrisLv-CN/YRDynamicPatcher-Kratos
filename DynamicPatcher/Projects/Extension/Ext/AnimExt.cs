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
    public partial class AnimExt : Extension<AnimClass>, IHaveComponent
    {
        public static Container<AnimExt, AnimClass> ExtMap = new Container<AnimExt, AnimClass>("AnimClass");

        private SwizzleablePointer<AnimTypeClass> pLastType;

        ExtensionReference<AnimTypeExt> type;
        public AnimTypeExt Type
        {
            get
            {
                if (type.TryGet(out AnimTypeExt ext) == false
                    || (!OwnerObject.Ref.Type.IsNull && OwnerObject.Ref.Type != pLastType))
                {
                    pLastType.Pointer = OwnerObject.Ref.Type;
                    type.Set(OwnerObject.Ref.Type);
                    ext = type.Get();
                }
                return ext;
            }
        }

        private ExtComponent<AnimExt> _extComponent;
        private DecoratorComponent _decoratorComponent;
        public ExtComponent<AnimExt> ExtComponent => _extComponent.GetAwaked();
        public DecoratorComponent DecoratorComponent => _decoratorComponent;
        public Component AttachedComponent => ExtComponent;

        public AnimExt(Pointer<AnimClass> OwnerObject) : base(OwnerObject)
        {
            _extComponent = new ExtComponent<AnimExt>(this, 0, "AnimExt root component");
            _decoratorComponent = new DecoratorComponent();
            _extComponent.OnAwake += () => ScriptManager.CreateScriptableTo(_extComponent, Type.Scripts, this);
            _extComponent.OnAwake += () => _decoratorComponent.AttachToComponent(_extComponent);
            pLastType = new SwizzleablePointer<AnimTypeClass>(OwnerObject.Ref.Type);
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

        // DEFINE_HOOK_AGAIN(0x422126, AnimClass_CTOR, 0x5)
        // DEFINE_HOOK_AGAIN(0x422707, AnimClass_CTOR, 0x5)
        // DEFINE_HOOK(0x4228D2, AnimClass_CTOR, 0x5)
        public static unsafe UInt32 AnimClass_CTOR(REGISTERS* R)
        {
            var pItem = (Pointer<AnimClass>)R->ESI;

            AnimExt.ExtMap.FindOrAllocate(pItem);
            return 0;
        }

        // DEFINE_HOOK(0x422967, AnimClass_DTOR, 0x6)
        public static unsafe UInt32 AnimClass_DTOR(REGISTERS* R)
        {
            var pItem = (Pointer<AnimClass>)R->ESI;

            AnimExt.ExtMap.Remove(pItem);
            return 0;
        }

        // DEFINE_HOOK_AGAIN(0x425280, AnimClass_SaveLoad_Prefix, 0x5)
        // DEFINE_HOOK(0x4253B0, AnimClass_SaveLoad_Prefix, 0x5)
        public static unsafe UInt32 AnimClass_SaveLoad_Prefix(REGISTERS* R)
        {
            var pItem = R->Stack<Pointer<AnimClass>>(0x4);
            var pStm = R->Stack<Pointer<IStream>>(0x8);
            IStream stream = Marshal.GetObjectForIUnknown(pStm) as IStream;

            AnimExt.ExtMap.PrepareStream(pItem, stream);
            return 0;
        }


        // DEFINE_HOOK_AGAIN(0x425391, AnimClass_Load_Suffix, 0x7)
        // DEFINE_HOOK_AGAIN(0x4253A2, AnimClass_Load_Suffix, 0x7)
        // DEFINE_HOOK(0x425358, AnimClass_Load_Suffix, 0x7)
        public static unsafe UInt32 AnimClass_Load_Suffix(REGISTERS* R)
        {
            AnimExt.ExtMap.LoadStatic();
            return 0;
        }

        // DEFINE_HOOK(0x4253FF, AnimClass_Save_Suffix, 0x5)
        public static unsafe UInt32 AnimClass_Save_Suffix(REGISTERS* R)
        {
            AnimExt.ExtMap.SaveStatic();
            return 0;
        }
    }
}
