
using System.Net;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Script;
using Extension.Decorators;
using Extension.Utilities;
using System.Threading.Tasks;

namespace Scripts
{
    [Serializable]
    public class GTNKScript : TechnoScriptable
    {
        public GTNKScript(TechnoExt owner) : base(owner) { }

        Pointer<TechnoClass> pStand;

        public override void OnPut(CoordStruct coord, Direction faceDir)
        {
            
        }

        public override void OnUpdate()
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            CoordStruct sourcePos = pTechno.Ref.Base.Base.GetCoords();

            if (pStand.IsNull)
            {
                pStand = ExHelper.CreateTechno("SREF", Owner.OwnerObject.Ref.Owner, sourcePos, false);
                // pStand.Ref.Base.Put(new CoordStruct(0, 0, 10000), Direction.N);
                // pStand.Ref.Base.SetLocation(sourcePos);
                // Pointer<FootClass> pFoot = pStand.Convert<FootClass>();
                // pFoot.Ref.Locomotor.Ref.Move_To(sourcePos);
                pStand.Ref.Base.Type.Ref.Immune = true;
                pStand.Ref.Base.IsOnMap = true;
                pStand.Ref.Base.NeedsRedraw = true;
                pStand.Ref.Base.InLimbo = false;
            }
            else
            {
                // pStand.Ref.Base.NeedsRedraw = true;
                CoordStruct pos = ExHelper.GetFLHAbsoluteCoords(pTechno, new CoordStruct(-256, 0, 512), true);
                pStand.Ref.Base.Location = pos;
                CoordStruct targetPos = pStand.Ref.Base.Base.GetCoords();
                LaserHelper.DrawLine(sourcePos, targetPos);

                Pointer<AbstractClass> target = pTechno.Ref.Target;
                if (!target.IsNull)
                {
                    pStand.Ref.Fire_IgnoreType(target, 0);
                }
            }

            if (pTechno.Ref.Base.IsSelected)
            {
                if (!pStand.IsNull)
                {
                    Logger.Log("Stand {0} InLimbo = {1}, IsAlive = {2}, IsOnMap = {3}, IsVisible = {4}, NeedRedraw = {5}",
                        pStand.Ref.Type.Convert<AbstractTypeClass>().Ref.ID,
                        pStand.Ref.Base.InLimbo,
                        pStand.Ref.Base.IsAlive,
                        pStand.Ref.Base.IsOnMap,
                        pStand.Ref.Base.IsVisible,
                        pStand.Ref.Base.NeedsRedraw
                    );
                }
            }
        }

        public override void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            Logger.Log("[{0}] {1} 开火. 武器ROF: 0 = {2}, 1 = {3}, ReloadTimer = {4}", pTechno.Ref.Owner.Ref.Type.Ref.Base.ID, pTechno.Ref.Type.Convert<AbstractTypeClass>().Ref.ID, pTechno.Ref.GetROF(0), pTechno.Ref.GetROF(1), pTechno.Ref.ReloadTimer);
        }

        public override void OnRemove()
        {
            // Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            // Logger.Log("Techno IsAlive={0} IsActive={1}, IsOnMap={2}", pTechno.Ref.Base.IsAlive, pTechno.Ref.Base.IsActive(), pTechno.Ref.Base.IsOnMap);
            if (!pStand.IsNull)
            {
                // pStand.Ref.Base.Remove();
                // pStand.Ref.Base.UnInit();
            }
        }
    }

}

