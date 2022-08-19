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
        public FireSuper FireSuper;

        private void InitFireSuper()
        {
            if (null != Type.FireSuperType)
            {
                this.FireSuper = Type.FireSuperType.CreateObject(Type);
                RegisterAction(FireSuper);
            }
        }
    }


    [Serializable]
    public class FireSuper : StateEffect<FireSuper, FireSuperType>
    {

        public override IAEState GetState()
        {
            return OwnerAEM.FireSuperState;
        }

    }

}