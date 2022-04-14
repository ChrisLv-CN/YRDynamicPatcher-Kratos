using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    [Serializable]
    public class FighterAreaGuardData
    {
        public bool AreaGuard;
        public int GuardRange;
        public bool AutoFire;
        public int MaxAmmo;
   
        public FighterAreaGuardData(bool areaGuard)
        {
            AreaGuard = areaGuard;
            GuardRange = 5;
            AutoFire = false;
            MaxAmmo = 1;
        }

    }



    public partial class TechnoExt
    {
        private bool isAreaProtecting = false;

        private CoordStruct areaProtectTo;

        private static List<CoordStruct> areaGuardCoords = new List<CoordStruct>()
        {
            new CoordStruct(-300,-300,0),
            new CoordStruct(-300,0,0),
            new CoordStruct(0,0,0),
            new CoordStruct(300,0,0),
            new CoordStruct(300,300,0),
            new CoordStruct(0,300,0),
        };

        private int currentAreaProtectedIndex = 0;

        private bool isAreaGuardReloading = false;

        private int areaGuardTargetCheckRof = 20;


        public unsafe void TechnoClass_Update_Fighter_Area_Guard()
        {
            FighterAreaGuardData data = Type.FighterAreaGuardData;

        
            if (!data.AreaGuard)
            {
                return;
            }

            var mission = OwnerObject.Convert<MissionClass>();

            if (mission.Ref.CurrentMission == Mission.Move)
            {
                isAreaProtecting = false;
                isAreaGuardReloading = false;
                return;
            }

            if (OwnerObject.CastToFoot(out Pointer<FootClass> pfoot))
            {
                if (!isAreaProtecting)
                {
                    if (mission.Ref.CurrentMission == Mission.Area_Guard)
                    {
                        isAreaProtecting = true;

                        CoordStruct dest = pfoot.Ref.Locomotor.Destination();
                        areaProtectTo = dest;
                    }
                }


                if (isAreaProtecting)
                {
                    //没弹药的情况下返回机场
                    if(OwnerObject.Ref.Ammo==0 && !isAreaGuardReloading)
                    {
                        OwnerObject.Ref.SetTarget(default);
                        OwnerObject.Ref.SetDestination(default(Pointer<CellClass>), false);
                        mission.Ref.ForceMission(Mission.Stop);
                        isAreaGuardReloading = true;
                        return;
                    }

                    //填弹完毕后继续巡航
                    if (isAreaGuardReloading)
                    {
                        if(OwnerObject.Ref.Ammo >= data.MaxAmmo)
                        {
                            isAreaGuardReloading = false;
                            mission.Ref.ForceMission(Mission.Area_Guard);
                        }
                        else
                        {
                            if (mission.Ref.CurrentMission != Mission.Sleep && mission.Ref.CurrentMission != Mission.Enter)
                            {
                                if (mission.Ref.CurrentMission == Mission.Guard)
                                {
                                    mission.Ref.ForceMission(Mission.Sleep);
                                }
                                else
                                {
                                    mission.Ref.ForceMission(Mission.Enter);
                                }
                                return;
                            }
                        }
                    }
                   

                    if (mission.Ref.CurrentMission == Mission.Move)
                    {
                        isAreaProtecting = false;
                        return;
                    }
                    else if (mission.Ref.CurrentMission == Mission.Attack)
                    {
                        return;
                    }
                    else if (mission.Ref.CurrentMission == Mission.Enter)
                    {
                        if(isAreaGuardReloading)
                        {
                            return;
                        }
                        else
                        {
                            mission.Ref.ForceMission(Mission.Stop);
                        }
                    }else if(mission.Ref.CurrentMission == Mission.Sleep)
                    {
                        if (isAreaGuardReloading)
                        {
                            return;
                        }
                    }


                    if (areaProtectTo != null)
                    {
                        var dest = areaProtectTo;

                        var house = OwnerObject.Ref.Owner;

                        if(data.AutoFire)
                        {
                            if (areaProtectTo.DistanceFrom(OwnerObject.Ref.Base.Base.GetCoords()) <= 2000)
                            {
                                if (areaGuardTargetCheckRof-- <= 0)
                                {
                                    areaGuardTargetCheckRof = 20;

                                    var target = Finder.FineOneTechno(house, x =>
                                    {
                                        var coords = x.Ref.Base.Base.GetCoords();
                                        var height = x.Ref.Base.GetHeight();
                                        var type = x.Ref.Base.Base.WhatAmI();

                                        if (x.Ref.Base.InLimbo)
                                        {
                                            return false;
                                        }

                                        var bounsRange = 0;
                                        if (x.Ref.Base.GetHeight() > 10)
                                        {
                                            bounsRange = data.GuardRange;
                                        }

                                        //if (coords.Z > dest.Z)
                                        //{
                                        //    if (coords.Z - location.Z > 300)
                                        //    {
                                        //        //空中目标奖励距离
                                        //        bounsRange = 1024;
                                        //    }
                                        //}

                                        if ((coords - new CoordStruct(0, 0, height)).DistanceFrom(dest) <= (data.GuardRange * 256 + bounsRange) && type != AbstractType.Building)
                                        {
                                            return true;
                                        }
                                        return false;
                                    }, FindRange.Enermy);

                                    if (target.TryGet(out TechnoExt targetTechno))
                                    {
                                        OwnerObject.Ref.SetTarget(targetTechno.OwnerObject.Convert<AbstractClass>());
                                        mission.Ref.ForceMission(Mission.Stop);
                                        mission.Ref.ForceMission(Mission.Attack);
                                        return;
                                    }
                                }

                            }
                        }
                       


                        if (areaProtectTo.DistanceFrom(OwnerObject.Ref.Base.Base.GetCoords()) <= 2000)
                        {
                            if (currentAreaProtectedIndex > areaGuardCoords.Count() - 1)
                            {
                                currentAreaProtectedIndex = 0;
                            }
                            dest += areaGuardCoords[currentAreaProtectedIndex];
                            currentAreaProtectedIndex++;
                        }

                        pfoot.Ref.Locomotor.Move_To(dest);
                        var cell = MapClass.Coord2Cell(dest);
                        if (MapClass.Instance.TryGetCellAt(cell, out Pointer<CellClass> pcell))
                        {
                            OwnerObject.Ref.SetDestination(pcell, false);
                        }
                    }
                }
            }

        }

    }

    public partial class TechnoTypeExt
    {
        public FighterAreaGuardData FighterAreaGuardData;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadFighterGuardData(INIReader reader, string section)
        {
            if(FighterAreaGuardData==null)
            {
                bool areaGuard = false;
                if (reader.ReadNormal(section, "Fighter.AreaGuard", ref areaGuard))
                {
                    FighterAreaGuardData = new FighterAreaGuardData(areaGuard);

                    if (areaGuard)
                    {
                        int guardRange = 5;
                        if (reader.ReadNormal(section, "Fighter.GuardRange", ref guardRange))
                        {
                            FighterAreaGuardData.GuardRange = guardRange;
                        }
                        bool autoFire = false;
                        if (reader.ReadNormal(section, "Fighter.AutoFire", ref autoFire))
                        {
                            FighterAreaGuardData.AutoFire = autoFire;
                        }

                        int maxAmmo = 1;
                        if (reader.ReadNormal(section, "Ammo", ref maxAmmo))
                        {
                            FighterAreaGuardData.MaxAmmo = maxAmmo;
                        }
                    }
                }
                else
                {
                    FighterAreaGuardData = new FighterAreaGuardData(areaGuard);
                }

            }
      
        }
    }

}
