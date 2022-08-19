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
        public Paintball Paintball;

        private void InitPaintball()
        {
            if (null != Type.PaintballType)
            {
                this.Paintball = Type.PaintballType.CreateObject(Type);
                RegisterAction(Paintball);
            }
        }
    }


    [Serializable]
    public class Paintball : StateEffect<Paintball, PaintballType>
    {
        private PaintballType data;

        public override IAEState GetState()
        {
            ColorStruct color = Type.IsHouseColor ? AE.pSourceHouse.Pointer.Ref.LaserColor : Type.Color;
            data = new PaintballType();
            data.Color = color;
            data.BrightMultiplier = Type.BrightMultiplier;
            return OwnerAEM.PaintballState;
        }

        public override IAEStateData GetData()
        {
            return data;
        }

    }

}