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
    public partial class BulletExt : Extension<BulletClass>, IDecorative, IDecorative<EventDecorator>, IDecorative<PairDecorator>
    {
        public static Container<BulletExt, BulletClass> ExtMap = new Container<BulletExt, BulletClass>("BulletClass");

        internal Lazy<BulletScriptable> scriptable;
        public BulletScriptable Scriptable
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

        public BulletExt(Pointer<BulletClass> OwnerObject) : base(OwnerObject)
        {
            scriptable = new Lazy<BulletScriptable>(() => ScriptManager.GetScriptable(Type.Script, this) as BulletScriptable);
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

        //[Hook(HookType.AresHook, Address = 0x4664BA, Size = 5)]
        public static unsafe UInt32 BulletClass_CTOR(REGISTERS* R)
        {
            var pItem = (Pointer<BulletClass>)R->ESI;

            BulletExt.ExtMap.FindOrAllocate(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x4665E9, Size = 0xA)]
        public static unsafe UInt32 BulletClass_DTOR(REGISTERS* R)
        {
            var pItem = (Pointer<BulletClass>)R->ESI;

            BulletExt ext = BulletExt.ExtMap.Find(pItem);
            ext?.OnUnInit();
            ext?.Scriptable?.OnUnInit();

            BulletExt.ExtMap.Remove(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x46AFB0, Size = 8)]
        //[Hook(HookType.AresHook, Address = 0x46AE70, Size = 5)]
        public static unsafe UInt32 BulletClass_SaveLoad_Prefix(REGISTERS* R)
        {
            var pItem = R->Stack<Pointer<BulletClass>>(0x4);
            var pStm = R->Stack<Pointer<IStream>>(0x8);
            IStream stream = Marshal.GetObjectForIUnknown(pStm) as IStream;

            BulletExt.ExtMap.PrepareStream(pItem, stream);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x46AF97, Size = 7)]
        //[Hook(HookType.AresHook, Address = 0x46AF9E, Size = 7)]
        public static unsafe UInt32 BulletClass_Load_Suffix(REGISTERS* R)
        {
            BulletExt.ExtMap.LoadStatic();
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x46AFC4, Size = 3)]
        public static unsafe UInt32 BulletClass_Save_Suffix(REGISTERS* R)
        {
            BulletExt.ExtMap.SaveStatic();
            return 0;
        }
    }
}
