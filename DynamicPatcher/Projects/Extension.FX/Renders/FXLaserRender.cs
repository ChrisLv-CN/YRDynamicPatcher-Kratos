using Extension.FX.Definitions;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Renders
{
    public class FXLaserRender : FXRenderer
    {
        public FXLaserRender(FXSystem system, FXEmitter emitter) : base(system, emitter)
        {
        }

        public Vector3 Color { get; set; } = new Vector3(1f, 1f, 1f);
        public int Duration { get; set; } = 2;
        public int Thickness { get; set; } = 1;

        public override FXScript Clone(FXSystem system = null, FXEmitter emitter = null)
        {
            var laserRender = new FXLaserRender(system ?? System, emitter ?? Emitter);

            laserRender.Color = Color;
            laserRender.Duration = Duration;
            laserRender.Thickness = Thickness;

            return laserRender;
        }

        public override void ParticleRender(FXParticle particle)
        {
            Vector3 curPosition = particle.Position;
            Vector3 previousPosition = particle.Map.GetValueOrDefault("PreviousPosition", curPosition);

            var distance = (curPosition - previousPosition).Length();
            if (distance > 32)
            {
                Vector3 trans = Color * 255;
                Pointer<LaserDrawClass> pLaser;
                lock (locker)
                {
                    pLaser = YRMemory.Create<LaserDrawClass>(
                        curPosition.ToCoordStruct(), previousPosition.ToCoordStruct(),
                        trans.ToColorStruct(), (trans * 0.5f).ToColorStruct(), (trans * 0.25f).ToColorStruct(),
                        Duration);
                }
                pLaser.Ref.Thickness = Thickness;
                pLaser.Ref.IsHouseColor = true;

                particle.Map.SetValue("PreviousPosition", curPosition);
            }
        }

        private static object locker = new object();
    }
}
