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

    [Serializable]
    public class JJFacingData
    {
        public bool Enable;
        public int Facing;
        public int Forward;

        public JJFacingData(bool enable)
        {
            this.Enable = enable;
            this.Facing = 8;
            this.Forward = -2;
        }

        public void SetFacing(int facing, int y)
        {
            this.Facing = facing;
            this.Forward = -2 * y;
        }
    }

    [Serializable]
    public class JJFacingToTarget
    {
        public bool NeedToTurn;
        public DirStruct ToDir;
        public int Facing;

        public void TurnTo(DirStruct toDir, int facing)
        {
            this.NeedToTurn = true;
            this.ToDir = toDir;
            this.Facing = facing;
        }

        public void Turning()
        {
            this.NeedToTurn = false;
        }

        public void Cancel()
        {
            this.NeedToTurn = false;
        }
    }

    public partial class TechnoExt
    {
        // private JJFacingToTarget jjFacingToTarget;
        private JJFacingToTarget JJFacing;

        public unsafe void UnitClass_Init_JumpjetFacingToTarget()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (null != Type.JJFacingData && Type.JJFacingData.Enable
                && pTechno.Convert<FootClass>().Ref.Locomotor.ToLocomotionClass().Ref.GetClassID() == LocomotionClass.Jumpjet
            )
            {
                JJFacing = new JJFacingToTarget();
                OnUpdateAction += UnitClass_Update_JumpjetFacingToTarget;
            }
        }

        public unsafe void UnitClass_Update_JumpjetFacingToTarget()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (pTechno.Convert<AbstractClass>().Ref.IsInAir())
            {
                if (JJFacing.NeedToTurn)
                {
                    Pointer<FootClass> pFoot = pTechno.Convert<FootClass>();
                    // Logger.Log("需要转向, 当前速度{0}, {1}", pFoot.Ref.GetCurrentSpeed(), pFoot.Ref.FacingChanging ? "正在转向" : "没有转向");
                    if (pFoot.Ref.GetCurrentSpeed() == 0)
                    {
                        JJFacing.Turning();
                        pFoot.Ref.StopMoving();
                        Pointer<JumpjetLocomotionClass> jjLoco = pFoot.Ref.Locomotor.ToLocomotionClass<JumpjetLocomotionClass>();
                        jjLoco.Ref.LocomotionFacing.turn(JJFacing.ToDir); // Discovery by Trsdy

                        /* // old school
                        CoordStruct location = pTechno.Ref.Base.Location;
                        if (MapClass.Instance.TryGetCellAt(location, out Pointer<CellClass> pCell) && !pCell.IsNull)
                        {
                            CoordStruct offsetLocation = ExHelper.GetFLHAbsoluteCoords(location, new CoordStruct(Type.JJFacingData.Forward, 0, 0), JJFacing.ToDir);
                            pTechno.Ref.Base.SetLocation(offsetLocation);

                            pTechno.Ref.SetDestination(pCell, true);
                            // pTechno.Convert<MissionClass>().Ref.QueueMission(Mission.Move, true);
                            // Logger.Log("转向, 目标方向{0}, 本体方向{1}, 偏移向量{2}, 最小向量{3}", JJFacing.ToDir.value32(), pTechno.Ref.GetRealFacing().current().value32(), offset, vector);
                        }
                        */
                    }
                    else
                    {
                        JJFacing.Cancel();
                        // Logger.Log("当前速度{0}, 没有停稳, 取消转向", pFoot.Ref.GetCurrentSpeed());
                    }
                }
                else if (!pTechno.Ref.Target.IsNull)
                {
                    Pointer<AbstractClass> pTarget = pTechno.Ref.Target;
                    bool canFire = false;
                    int weaponIdx = pTechno.Ref.SelectWeapon(pTarget);
                    FireError fireError = pTechno.Ref.GetFireError(pTarget, weaponIdx, true);
                    switch (fireError)
                    {
                        case FireError.ILLEGAL:
                        case FireError.CANT:
                        case FireError.MOVING:
                        case FireError.RANGE:
                            break;
                        default:
                            canFire = pTechno.Ref.IsCloseEnough(pTarget, weaponIdx);
                            break;
                    }
                    if (canFire)
                    {
                        CoordStruct sourcePos = pTechno.Ref.Base.Location;
                        CoordStruct targetPos = pTechno.Ref.Target.Ref.GetCoords();
                        DirStruct toDir = ExHelper.Point2Dir(sourcePos, targetPos);
                        DirStruct selfDir = pTechno.Ref.Facing.current();
                        int facing = Type.JJFacingData.Facing;
                        int toIndex = ExHelper.Dir2FacingIndex(toDir, facing);
                        int selfIndex = ExHelper.Dir2FacingIndex(selfDir, facing);
                        if (selfIndex != toIndex)
                        {
                            DirStruct targetDir = ExHelper.DirNormalized(toIndex, facing);
                            JJFacing.TurnTo(targetDir, facing);
                            // Logger.Log("需要转向, 当前朝向{0}, 目标朝向{1}", ExHelper.Dir2FacingIndex(selfDir, facing), ExHelper.Dir2FacingIndex(toDir, facing));
                        }
                        else
                        {
                            JJFacing.Cancel();
                            // Logger.Log("无需转向, 当前朝向{0}, 目标朝向{1}", ExHelper.Dir2FacingIndex(selfDir, facing), ExHelper.Dir2FacingIndex(toDir, facing));
                        }
                    }
                    else
                    {
                        JJFacing.Cancel();
                    }
                }
            }
        }

    }

    public partial class TechnoTypeExt
    {
        public JJFacingData JJFacingData;

        /// <summary>
        /// [TechnoType]
        /// JumpjetFacingToTarget=yes
        /// JumpjetFacing=8
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadJumpjetFacingToTarget(INIReader reader, string section)
        {
            bool facingToTarget = false;
            if (reader.ReadNormal(section, "JumpjetFacingToTarget", ref facingToTarget))
            {
                JJFacingData = new JJFacingData(facingToTarget);

                int facing = 8;
                if (reader.ReadNormal(section, "JumpjetFacing", ref facing))
                {
                    if (facing >= 8)
                    {
                        int x = facing % 8;
                        int y = facing / 8;
                        if (x == 0)
                        {
                            JJFacingData.SetFacing(facing, y);
                        }
                        else if (x > 4)
                        {
                            JJFacingData.SetFacing(8 * (y + 1), y + 1);
                        }
                        else
                        {
                            JJFacingData.SetFacing(8 * y, y);
                        }
                    }
                }
            }
        }
    }

}