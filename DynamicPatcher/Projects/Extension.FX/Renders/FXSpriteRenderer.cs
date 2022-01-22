using Extension.FX.Definitions;
using Extension.FX.Graphic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Renders
{
    public class FXSpriteRenderer : FXRenderer
    {
        public FXSpriteRenderer(FXSystem system, FXEmitter emitter) : base(system, emitter)
        {
        }

        public string Texture { get; set; } = "default_sprite.png";

        public override FXScript Clone(FXSystem system = null, FXEmitter emitter = null)
        {
            var spriteRenderer = new FXSpriteRenderer(system ?? System, emitter ?? Emitter);

            spriteRenderer.Texture = Texture;

            return spriteRenderer;
        }

        private FXDrawObject CreateDrawObject()
        {
            return new FXDrawObject(Texture);
        }

        public override void ParticleRender(FXParticle particle)
        {
            Vector3 position = particle.Position;

            if (FXEngine.InScreen(position))
            {
                FXDrawObject drawObject = particle.Map.GetValueOrDefault("DrawObject", CreateDrawObject);

                drawObject.MoveTo(position);

                FXGraphic.DrawObject(drawObject);
            }
        }
    }
}
