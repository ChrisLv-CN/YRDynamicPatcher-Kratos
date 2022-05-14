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
        public DamageSelf DamageSelf;

        private void InitDamageSelf()
        {
            if (null != Type.DamageSelfType)
            {
                this.DamageSelf = Type.DamageSelfType.CreateObject(Type);
                RegisterAction(DamageSelf);
            }
        }
    }


    [Serializable]
    public class DamageSelf : StateEffect<DamageSelf, DamageSelfType>
    {

        public override IAEState GetState(Pointer<ObjectClass> pOwner, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker)
        {
            return OwnerAEM.DamageSelfState;
        }

    }

}