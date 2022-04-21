using DynamicPatcher;
using Extension.Ext;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    public partial class AttachEffect
    {
        public GiftBox GiftBox;

        private void InitGiftBox()
        {
            if (null != Type.GiftBoxType)
            {
                this.GiftBox = Type.GiftBoxType.CreateObject(Type);
                RegisterAction(GiftBox);
            }
        }
    }


    [Serializable]
    public class GiftBox : StateEffect<GiftBox, GiftBoxType>
    {
        public override IAEState GetState(Pointer<ObjectClass> pOwner, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker)
        {
            return OwnerAEM.GiftBoxState;
        }
    }

}