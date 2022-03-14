using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp.Utilities
{
	public class TechnoPlacer
	{
		public static CellStruct FindPlaceableCellNear(Pointer<TechnoClass> pTechno, CoordStruct location)
        {
			return FindPlaceableCellNear(pTechno, CellClass.Coord2Cell(location));
        }
		public static CellStruct FindPlaceableCellNear(Pointer<TechnoClass> pTechno, CellStruct location)
		{
			Pointer<TechnoTypeClass> pType = pTechno.Ref.Base.GetTechnoType();
			pTechno.Convert<AbstractClass>().CastIf(AbstractType.Building, out Pointer<BuildingClass> pBuilding);

			// get the best options to search for a place
			short extentX = 1;
			short extentY = 1;
			SpeedType speedType = SpeedType.Track;
			MovementZone movementZone = MovementZone.Normal;
			bool buildable = false;
			bool anywhere = false;

			if (!pBuilding.IsNull)
			{
				var pBuildingType = pBuilding.Ref.Type;
				extentX = pBuildingType.Ref.GetFoundationWidth();
				extentY = pBuildingType.Ref.GetFoundationHeight(true);
				anywhere = pBuildingType.Ref.PlaceAnywhere;
				if (pType.Ref.SpeedType == SpeedType.Float)
				{
					speedType = SpeedType.Float;
				}
				else
				{
					buildable = true;
				}
			}
			else
			{
				// place aircraft types on ground explicitly
				if (pType.Ref.Base.Base.Base.WhatAmI() != AbstractType.AircraftType)
				{
					speedType = pType.Ref.SpeedType;
					movementZone = pType.Ref.MovementZone;
				}
			}

			// move the target cell so this object is centered on the actual location
			var placeCoords = location - new CellStruct(extentX / 2, extentY / 2);

			// find a place to put this
			if (!anywhere)
			{
				int a5 = -1; // usually MapClass::CanLocationBeReached call. see how far we can get without it
				placeCoords = MapClass.Instance.Pathfinding_Find(placeCoords,
					speedType, a5, movementZone, false, extentX, extentY, !pBuilding.IsNull,
					false, false, false, default, false, buildable);
			}

			return placeCoords;
		}

		public static bool PlaceTechnoNear(Pointer<TechnoTypeClass> pType, Pointer<HouseClass> pOwner, CellStruct location, bool buildUp = false)
		{
			var pTechno = pType.Ref.Base.CreateObject(pOwner).Convert<TechnoClass>();
			return PlaceTechnoNear(pTechno, location, buildUp);
		}
		public static bool PlaceTechnoNear(Pointer<TechnoClass> pTechno, CellStruct location, bool buildUp = false)
		{
			Pointer<TechnoTypeClass> pType = pTechno.Ref.Base.GetTechnoType();
			Pointer<HouseClass> pOwner = pTechno.Ref.Owner;
			pTechno.Convert<AbstractClass>().CastIf(AbstractType.Building, out Pointer<BuildingClass> pBuilding);

            CellStruct placeCoords = FindPlaceableCellNear(pTechno, location);

			if (MapClass.Instance.TryGetCellAt(placeCoords, out var pCell))
			{
				pTechno.Ref.Base.OnBridge = pCell.Ref.ContainsBridge();

				// set the appropriate mission
				if (!pBuilding.IsNull && buildUp)
				{
					pBuilding.Ref.Base.BaseMission.QueueMission(Mission.Construction, false);
				}
				else
				{
					// only computer units can hunt
					var Guard = !pBuilding.IsNull || pOwner.Ref.ControlledByHuman();
					var mission = Guard ? Mission.Guard : Mission.Hunt;
					pTechno.Ref.BaseMission.QueueMission(mission, false);
				}

				// place and set up
				var XYZ = pCell.Ref.GetCoordsWithBridge();

				var isPut = pTechno.Ref.Base.Put(XYZ, (Direction)(MapClass.GetCellIndex(pCell.Ref.MapCoords) & 7u));

				if (isPut)
				{
					if (!pBuilding.IsNull)
					{
						if (buildUp)
						{
							pBuilding.Ref.Base.Base.DiscoveredBy(pOwner);
							pBuilding.Ref.unknown_bool_6DD = 1;
						}
					}
					else if (pType.Ref.BalloonHover || pType.Ref.JumpJet)
					{
						pTechno.Ref.Base.Scatter(default, true, false);
					}
					return true;
				}
				else
				{
					pTechno.Ref.Base.UnInit();
				}
			}

			return false;
		}

		public static bool PlaceTechnoFromEdge(Pointer<TechnoTypeClass> pType, Pointer<HouseClass> pOwner, Edge edge = Edge.None)
		{
			var pTechno = pType.Ref.Base.CreateObject(pOwner).Convert<TechnoClass>();
			return PlaceTechnoFromEdge(pTechno, edge == Edge.None ? pOwner.Ref.GetStartingEdge() : edge);
		}
		public static bool PlaceTechnoFromEdge(Pointer<TechnoClass> pTechno, Edge edge = Edge.None)
        {
            CellStruct crd = MapClass.Instance.PickCellOnEdge(
				edge == Edge.None ? pTechno.Ref.Owner.Ref.GetStartingEdge() : edge,
				default, default, SpeedType.Winged, true, MovementZone.Normal);
            CoordStruct placeCoords = CellClass.Cell2Coord(crd);
            bool isPut = pTechno.Ref.Base.Put(placeCoords, Direction.N);
			if (!isPut)
			{
				pTechno.Ref.Base.UnInit();
				return false;
			}
			return true;
		}
	}
}
