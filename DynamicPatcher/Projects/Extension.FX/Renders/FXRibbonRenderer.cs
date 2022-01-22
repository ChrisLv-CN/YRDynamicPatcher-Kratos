using Extension.FX.Definitions;
using Extension.FX.Graphic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Renders
{
    public class FXRibbonRenderer : FXRenderer
    {
        public FXRibbonRenderer(FXSystem system, FXEmitter emitter) : base(system, emitter)
        {
        }

        public string Texture { get; set; } = "default_ribbon";

        public override FXScript Clone(FXSystem system = null, FXEmitter emitter = null)
        {
            var ribbonRenderer = new FXRibbonRenderer(system ?? System, emitter ?? Emitter);

            ribbonRenderer.Texture = Texture;

            return ribbonRenderer;
        }

        public override void ParticleRender(FXParticle particle)
        {
            Vector3 curPosition = particle.Position;
            Vector3 previousPosition = particle.Map.GetValueOrDefault("PreviousPosition", curPosition);

            var distance = (curPosition - previousPosition).Length;
            if (distance > 128)
            {
                var texture = FXGraphic.GetTexture(Texture);


                particle.Map.SetValue("PreviousPosition", curPosition);
            }
        }
    }
}
