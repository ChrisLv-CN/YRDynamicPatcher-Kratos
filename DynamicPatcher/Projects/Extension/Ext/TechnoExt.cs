using DynamicPatcher;
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
    public partial class TechnoExt : Extension<TechnoClass>, IDecorative, IDecorative<EventDecorator>, IDecorative<PairDecorator>
    {
        public static Container<TechnoExt, TechnoClass> ExtMap = new Container<TechnoExt, TechnoClass>("TechnoClass");

        internal Lazy<TechnoScriptable> scriptable;
        public TechnoScriptable Scriptable
        {
            get
            {
                if (scriptable.IsValueCreated || Type.Script != null)
                {
                    return scriptable.Value;
                }
                return null;
            }
        }

        SwizzleablePointer<TechnoTypeClass> pSourceType;

        ExtensionReference<TechnoTypeExt> type;
        public TechnoTypeExt Type
        {
            get
            {
                if (type.TryGet(out TechnoTypeExt ext) == false
                    || (!OwnerObject.Ref.Type.IsNull && OwnerObject.Ref.Type != pSourceType))
                {
                    pSourceType.Pointer = OwnerObject.Ref.Type;
                    type.Set(OwnerObject.Ref.Type);
                    ext = type.Get();
                }
                return ext;
            }
        }

        public TechnoExt(Pointer<TechnoClass> OwnerObject) : base(OwnerObject)
        {
            scriptable = new Lazy<TechnoScriptable>(() => ScriptManager.GetScriptable(Type.Script, this) as TechnoScriptable);
            pSourceType = new SwizzleablePointer<TechnoTypeClass>(OwnerObject.Ref.Type);
        }

        DecoratorMap decoratorMap = new DecoratorMap();

        public TDecorator CreateDecorator<TDecorator>(DecoratorId id, string description, params object[] parameters) where TDecorator : Decorator
        {
            TDecorator decorator = decoratorMap.CreateDecorator<TDecorator>(id, description, parameters);
            decorator.Decorative = this;
            return decorator;
        }

        public Decorator Get(DecoratorId id) => decoratorMap.Get(id);

        public void Remove(DecoratorId id) => decoratorMap.Remove(id);

        public void Remove(Decorator decorator) => decoratorMap.Remove(decorator);

        IEnumerable<EventDecorator> IDecorative<EventDecorator>.GetDecorators() => decoratorMap.GetEventDecorators();

        IEnumerable<PairDecorator> IDecorative<PairDecorator>.GetDecorators() => decoratorMap.GetPairDecorators();

        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);
        }

        [OnSerializing]
        protected void OnSerializing(StreamingContext context) { }

        [OnSerialized]
        protected void OnSerialized(StreamingContext context) { }

        [OnDeserializing]
        protected void OnDeserializing(StreamingContext context) { }

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context) { }

        public override void SaveToStream(IStream stream)
        {
            base.SaveToStream(stream);
            Scriptable?.SaveToStream(stream);
        }

        public override void LoadFromStream(IStream stream)
        {
            base.LoadFromStream(stream);
            Scriptable?.LoadFromStream(stream);
        }

        //[Hook(HookType.AresHook, Address = 0x6F3260, Size = 5)]
        public static unsafe UInt32 TechnoClass_CTOR(REGISTERS* R)
        {
            var pItem = (Pointer<TechnoClass>)R->ESI;

            TechnoExt.ExtMap.FindOrAllocate(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x6F4500, Size = 5)]
        public static unsafe UInt32 TechnoClass_DTOR(REGISTERS* R)
        {
            var pItem = (Pointer<TechnoClass>)R->ECX;

            TechnoExt ext = TechnoExt.ExtMap.Find(pItem);
            ext?.OnUnInit();
            ext?.Scriptable?.OnUnInit();

            TechnoExt.ExtMap.Remove(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x70C250, Size = 8)]
        //[Hook(HookType.AresHook, Address = 0x70BF50, Size = 5)]
        public static unsafe UInt32 TechnoClass_SaveLoad_Prefix(REGISTERS* R)
        {
            var pItem = R->Stack<Pointer<TechnoClass>>(0x4);
            var pStm = R->Stack<Pointer<IStream>>(0x8);
            IStream stream = Marshal.GetObjectForIUnknown(pStm) as IStream;

            TechnoExt.ExtMap.PrepareStream(pItem, stream);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x70C249, Size = 5)]
        public static unsafe UInt32 TechnoClass_Load_Suffix(REGISTERS* R)
        {
            TechnoExt.ExtMap.LoadStatic();
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x70C264, Size = 5)]
        public static unsafe UInt32 TechnoClass_Save_Suffix(REGISTERS* R)
        {
            TechnoExt.ExtMap.SaveStatic();
            return 0;
        }
    }

}
