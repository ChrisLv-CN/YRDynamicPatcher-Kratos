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
            }
        }
    }


    [Serializable]
    public class Paintball : AttachEffectBehaviour
    {
        public PaintballType Type;
        private TechnoExt OwnerExt;
        private ColorStruct Color;
        private string token;
        private int duration;

        public Paintball(PaintballType type, AttachEffectType attachEffectType) : base(attachEffectType)
        {
            this.Type = type;
            this.token = new Guid().ToString();
            this.duration = attachEffectType.Duration;
        }

        public override void Enable(Pointer<ObjectClass> pObject, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker)
        {
            if (pObject.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                OwnerExt = TechnoExt.ExtMap.Find(pTechno);
                if (null != OwnerExt)
                {
                    Color = Type.IsHouseColor ? pHouse.Ref.LaserColor : Type.Color;
                    OwnerExt.PaintballState.Enable(duration, token, Color, Type.BrightMultiplier);
                }
            }
        }

        public override void Disable(CoordStruct location)
        {
            OwnerExt?.PaintballState.Disable(token);
        }

        public override void ResetDuration()
        {
            if (null != OwnerExt)
            {
                OwnerExt.PaintballState.Enable(duration, token, Color, Type.BrightMultiplier);
            }
        }

    }

}