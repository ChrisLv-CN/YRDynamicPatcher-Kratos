using Extension.FX.Definitions;
using Extension.FX.Graphic;
using PatcherYRpp;
using PatcherYRpp.FileFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Renders
{
    public class FXSHPRenderer : FXRenderer
    {
        public FXSHPRenderer(FXSystem system, FXEmitter emitter) : base(system, emitter)
        {
        }

        public string SHP { get; set; }
        public string PAL { get; set; }
        public int CurFrameIdx { get; set; } = 2;

        public override FXScript Clone(FXSystem system = null, FXEmitter emitter = null)
        {
            var shpRenderer = new FXSHPRenderer(system ?? System, emitter ?? Emitter);

            shpRenderer.SHP = SHP;
            shpRenderer.PAL = PAL;

            return shpRenderer;
        }


        public override void ParticleRender(FXParticle particle)
        {
            Vector3 position = particle.Position;

            if (FXEngine.InScreen(position))
            {
                if(SHPCache.TryGet(SHP, out var shp))
                {
                    CoordStruct coords = new CoordStruct((int)position.X, (int)position.Y, (int)position.Z);
                    Point2D client = TacticalClass.Instance.Ref.CoordsToClient(coords);

                    BlitterFlags flags = BlitterFlags.bf_400 | BlitterFlags.Centered | BlitterFlags.Plain;
                    int zAdjust = -TacticalClass.Instance.Ref.AdjustForZ(coords.Z) - 2;

                    var bound = shp.Ref.GetFrameBounds(CurFrameIdx);
                    var lockRect = new RectangleStruct(client.X - bound.Width / 2, client.Y - bound.Height / 2, bound.Width, bound.Height);

                    YRGraphic.DrawSHP(FileSystem.ANIM_PAL, shp, CurFrameIdx, client, lockRect, flags, 0, zAdjust, 2, 1000, 0, IntPtr.Zero, 0, 0, 0);
                }
            }
        }

        static class SHPCache
        {
            public static Dictionary<string, Pointer<SHPStruct>> _shps = new Dictionary<string, Pointer<SHPStruct>>();

            private static bool TryLoad(string name, out Pointer<SHPStruct> shp)
            {
                shp = FileSystem.LoadSHPFile(name);
                return shp.IsNull == false;
            }
            public static bool TryGet(string name, out Pointer<SHPStruct> shp)
            {
                if (_shps.TryGetValue(name, out shp))
                {
                    return true;
                }

                lock (_shps)
                {
                    if (TryLoad(name, out shp))
                    {
                        _shps[name] = shp;
                        return true;
                    }
                }

                shp = Pointer<SHPStruct>.Zero;
                return false;
            }
        }
    }
}
