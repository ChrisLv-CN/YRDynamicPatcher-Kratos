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
        public Deselect Deselect;

        private void InitDeselect()
        {
            if (null != Type.DeselectType)
            {
                this.Deselect = Type.DeselectType.CreateObject(Type);
                RegisterAction(Deselect);
            }
        }
    }


    [Serializable]
    public class Deselect : StateEffect<Deselect, DeselectType>
    {
        public override IAEState GetState()
        {
            return OwnerAEM.DeselectState;
        }
    }

}