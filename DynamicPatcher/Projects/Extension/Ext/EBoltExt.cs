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
    public partial class EBoltExt : Extension<EBolt>
    {
        public static Container<EBoltExt, EBolt> ExtMap = new Container<EBoltExt, EBolt>("EBolt");

        public ColorStruct Color1 = default;
        public ColorStruct Color2 = default;
        public ColorStruct Color3 = default;
        public bool Disable1 = false;
        public bool Disable2 = false;
        public bool Disable3 = false;
        public string ParticleSystemName = null;

        public EBoltExt(Pointer<EBolt> OwnerObject) : base(OwnerObject)
        {
        }

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
        }

        public override void LoadFromStream(IStream stream)
        {
            base.LoadFromStream(stream);
        }

    }

}
