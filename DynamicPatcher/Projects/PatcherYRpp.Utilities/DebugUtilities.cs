using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp.Utilities
{
    public static class DebugUtilities
    {
        public static void MarkLocation(CoordStruct location, ColorStruct color, int beamHeight = 1000, int thickness = 4, int duration = 10)
        {
            ColorStruct innerColor = new ColorStruct(color.R, color.G, color.B);
            ColorStruct outerColor = new ColorStruct(color.R / 2, color.G / 2, color.B / 2);
            ColorStruct outerSpread = new ColorStruct(color.R / 4, color.G / 4, color.B / 4);

            Pointer<LaserDrawClass> pMarkLaser = YRMemory.Create<LaserDrawClass>(location, location + new CoordStruct(0, 0, beamHeight), innerColor, outerColor, outerSpread, duration);
            pMarkLaser.Ref.Thickness = thickness;
            pMarkLaser.Ref.IsHouseColor = true;
        }


        public static void MarkTarget(Pointer<AbstractClass> pTarget, ColorStruct color, int beamHeight = 1000, int thickness = 4, int duration = 10)
        {
            CoordStruct location = pTarget.Ref.GetCoords();

            MarkLocation(location, color, beamHeight, duration);
        }


        public static void HighlightObjectBlock(ObjectBlock block, ColorStruct color, int thickness = 4, int duration = 10)
        {
            void Draw(CoordStruct from, CoordStruct to)
            {
                ColorStruct innerColor = new ColorStruct(color.R, color.G, color.B);
                ColorStruct outerColor = new ColorStruct(color.R / 2, color.G / 2, color.B / 2);
                ColorStruct outerSpread = new ColorStruct(color.R / 4, color.G / 4, color.B / 4);

                Pointer<LaserDrawClass> pMarkLaser = YRMemory.Create<LaserDrawClass>(from, to, innerColor, outerColor, outerSpread, duration);
                pMarkLaser.Ref.Thickness = thickness;
                pMarkLaser.Ref.IsHouseColor = true;
            }

            var points = new List<CoordStruct>();
            int offset = block.Container.BlockRange;

            foreach (var cellOffset in new[] {
                new CellStruct(offset, offset),
                new CellStruct(-offset, offset),
                new CellStruct(-offset, -offset),
                new CellStruct(offset, -offset) })
            {
                if (MapClass.Instance.TryGetCellAt(block.Center + cellOffset, out var pCell))
                {
                    points.Add(pCell.Ref.Base.GetCoords());
                }
            }

            for (int i = 0; i < points.Count; i++)
            {
                Draw(points[i], points[(i + 1) % points.Count]);
            }
        }
    }
}
