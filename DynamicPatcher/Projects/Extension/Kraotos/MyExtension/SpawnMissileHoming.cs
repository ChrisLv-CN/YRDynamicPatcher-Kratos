using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    public partial class TechnoExt
    {
        public bool IsHoming;
        public CoordStruct HomingTargetLocation;

        public unsafe void AircraftClass_Init_SpawnMissileHoming()
        {
            if (OwnerObject.CastIf(AbstractType.Aircraft, out Pointer<AircraftClass> pAircraft)
                && pAircraft.Ref.Type.Ref.Base.Spawned && pAircraft.Ref.Type.Ref.Base.MissileSpawn)
            {
                this.IsHoming = Type.SpawnMissileHoming;
                OnUpdateAction += AircraftClass_Update_SpawnMissileHoming;
            }
        }

        public unsafe void AircraftClass_Update_SpawnMissileHoming()
        {
            // 子机管理器可能会强制开启
            if (IsHoming)
            {
                Pointer<AbstractClass> pTarget = OwnerObject.Ref.Target;
                if (!pTarget.IsNull)
                {
                    if (pTarget.CastToTechno(out Pointer<TechnoClass> pTargetTechno))
                    {
                        // 如果目标消失，导弹会追到最后一个位置然后爆炸
                        if (!pTargetTechno.IsDeadOrInvisibleOrCloaked())
                        {
                            HomingTargetLocation = pTarget.Ref.GetCoords();
                            // Logger.Log($"{Game.CurrentFrame} 更新导弹 {OwnerObject} [{OwnerObject.Ref.Type.Ref.Base.Base.ID}] 目的地 {HomingTargetLocation}");
                        }
                    }
                }

                if (default != HomingTargetLocation)
                {
                    ILocomotion loco = OwnerObject.Convert<FootClass>().Ref.Locomotor;
                    Pointer<LocomotionClass> pLoco = loco.ToLocomotionClass();
                    if (LocomotionClass.Rocket == pLoco.Ref.GetClassID())
                    {
                        Pointer<RocketLocomotionClass> pRLoco = pLoco.Convert<RocketLocomotionClass>();
                        if (pRLoco.Ref.Timer34.Step > 2)
                        {
                            // Logger.Log($"{Game.CurrentFrame} 重设导弹 {OwnerObject} [{OwnerObject.Ref.Type.Ref.Base.Base.ID}] 目的地 {HomingTargetLocation}");
                            pRLoco.Ref.Destination = HomingTargetLocation;
                        }
                    }
                }
            }
        }


    }

    public partial class TechnoTypeExt
    {

        public bool SpawnMissileHoming;

        /// <summary>
        /// [AircraftType]
        /// Missile.Homing=no
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadSpawnMissileHoming(INIReader reader, string section)
        {
            bool homing = false;
            if (reader.ReadNormal(section, "Missile.Homing", ref homing))
            {
                this.SpawnMissileHoming = homing;
            }
        }
    }


}