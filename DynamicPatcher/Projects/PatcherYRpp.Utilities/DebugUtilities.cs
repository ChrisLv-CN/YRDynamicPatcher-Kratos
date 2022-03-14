using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        public static void HighlightCell(Pointer<CellClass> pCell, ColorStruct color, int thickness = 3, int duration = 10)
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

            CoordStruct center = pCell.Ref.Base.GetCoords();

            const int offset = Game.CellSize / 2;
            var points = new[]{
                center + new CoordStruct(offset, offset, 0),
                center + new CoordStruct(-offset, offset, 0),
                center + new CoordStruct(-offset, -offset, 0),
                center + new CoordStruct(offset, -offset, 0) };

            for (int i = 0; i < points.Length; i++)
            {
                Draw(points[i], points[(i + 1) % points.Length]);
            }
        }

        public static void HighlightCircle(CoordStruct location, int range, ColorStruct color, Vector3 upVector = default, int thickness = 3, int duration = 10)
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

            var points = CircleDifferentiator.DivideArcByTolerance(location, range, 128, upVector);
            for (int i = 0; i < points.Count; i++)
            {
                Draw(points[i], points[(i + 1) % points.Count]);
            }
        }

        public static void HighlightDistance(CoordStruct from, CoordStruct to, ColorStruct color, int thickness = 3, int duration = 10)
        {
            void Draw(CoordStruct _from, CoordStruct _to)
            {
                ColorStruct innerColor = new ColorStruct(color.R, color.G, color.B);
                ColorStruct outerColor = new ColorStruct(color.R / 2, color.G / 2, color.B / 2);
                ColorStruct outerSpread = new ColorStruct(color.R / 4, color.G / 4, color.B / 4);

                Pointer<LaserDrawClass> pMarkLaser = YRMemory.Create<LaserDrawClass>(_from, _to, innerColor, outerColor, outerSpread, duration);
                pMarkLaser.Ref.Thickness = thickness;
                pMarkLaser.Ref.IsHouseColor = true;
            }

            Draw(from, to);
            HighlightCircle(to, 64, color, new Vector3(1, 1, 1), thickness, duration);
        }


        public static string GetAbstractID(Pointer<AbstractClass> pAbstract)
        {
            if (pAbstract.CastToObject(out var pObject))
            {
                Pointer<ObjectTypeClass> pType = pObject.Ref.GetObjectType();
                if(pType.IsNull == false)
                    return pType.Ref.Base.ID;
            }

            switch (pAbstract.Ref.WhatAmI())
            {
                case AbstractType.House:
                    var pHouse = pAbstract.Convert<HouseClass>();
                    return pHouse.Ref.Type.Ref.Base.ID;

                // TOFILL

                // AbstractTypeClasses
                case AbstractType.AircraftType:
                case AbstractType.AnimType:
                case AbstractType.BuildingType:
                case AbstractType.BulletType:
                case AbstractType.Campaign:
                case AbstractType.HouseType:
                case AbstractType.InfantryType:
                case AbstractType.IsotileType:
                case AbstractType.OverlayType:
                case AbstractType.ParticleSystemType:
                case AbstractType.ParticleType:
                case AbstractType.ScriptType:
                case AbstractType.Side:
                case AbstractType.SmudgeType:
                case AbstractType.SuperWeaponType:
                case AbstractType.TaskForce:
                case AbstractType.TeamType:
                case AbstractType.TerrainType:
                case AbstractType.TriggerType:
                case AbstractType.UnitType:
                case AbstractType.VoxelAnimType:
                case AbstractType.TagType:
                case AbstractType.Tiberium:
                case AbstractType.WeaponType:
                case AbstractType.WarheadType:
                case AbstractType.AITriggerType:
                    var pType = pAbstract.Convert<AbstractTypeClass>();
                    return pType.Ref.ID;
            }

            return "<no id>";
        }
    }
}
