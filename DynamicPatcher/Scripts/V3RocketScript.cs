
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Script;
using Extension.Utilities;
using System.Threading.Tasks;

namespace Scripts
{
    [Serializable]
    public class V3Rocket : TechnoScriptable
    {

        public V3Rocket(TechnoExt owner) : base(owner)
        {
            
        }

        private bool ChangeMislFlag = false;
        public override void OnUpdate()
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            if (!pTechno.Ref.SpawnOwner.IsNull && !pTechno.Ref.SpawnOwner.Ref.Target.IsNull)
            {
                Pointer<AbstractClass> pTarget = pTechno.Ref.SpawnOwner.Ref.Target;
                CoordStruct tPos = pTarget.Ref.GetCoords();
                // pTechno.Convert<FootClass>().Ref.StopMoving();
                Pointer<SpawnManagerClass> pSpawnManager = pTechno.Ref.SpawnOwner.Ref.SpawnManager;
                // if (!ChangeMislFlag && pSpawnManager.Ref.SpawnedNodes.Count > 0)
                // {
                //     for (int i = 0; i < pSpawnManager.Ref.SpawnedNodes.Count; i++)
                //     {
                //         Pointer<SpawnNode> pNode = pSpawnManager.Ref.SpawnedNodes.Get(i);
                //         if (pNode.Ref.Unit == pTechno && pNode.Ref.IsSpawnMissile)
                //         {
                //             ChangeMislFlag = true;
                //             // pNode.Ref.IsSpawnMissile = false;
                //         }

                //     }
                // }
                pTechno.Convert<FootClass>().Ref.Destination = pTarget;
            }
            if (!pTechno.Ref.Veterancy.IsElite())
            {
                Pointer<AbstractClass> pTarget = pTechno.Ref.Target;
                if (!pTarget.IsNull && pTarget.Ref.WhatAmI() == AbstractType.Cell)
                {
                    Pointer<CellClass> pCell = pTarget.Convert<CellClass>();
                    CoordStruct targetPos = MapClass.Cell2Coord(pCell.Ref.MapCoords);
                    ExHelper.FindOwnerTechno(pTechno.Ref.Owner, (techno) => {
                        TechnoExt ext = TechnoExt.ExtMap.Find(techno);
                        if (null != ext && ext.attackBeacon.Enable)
                        {
                            CoordStruct pos = techno.Ref.Base.Base.GetCoords();
                            if (targetPos.DistanceFrom(techno.Ref.Base.Base.GetCoords()) <= 512)
                            {
                                pTechno.Ref.Veterancy.SetElite();
                                return true;
                            }
                        }
                        return false;
                    }, true);
                }
            }
        }
    }
}

