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
        public DamageReaction DamageReaction;

        private void InitDamageReaction()
        {
            if (null != Type.DamageReactionType)
            {
                this.DamageReaction = Type.DamageReactionType.CreateObject(Type);
                RegisterAction(DamageReaction);
            }
        }
    }


    [Serializable]
    public class DamageReaction : Effect<DamageReactionType>
    {

        public override void OnEnable(Pointer<ObjectClass> pObject)
        {
            OwnerAEM.DamageReactionState.EnableAndReplace<DamageReactionType>(this);
        }

        public override void Disable(CoordStruct location)
        {
            OwnerAEM.DamageReactionState.Disable(Token);
        }

    }

}