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
    public class StraightBulletData
    {
        public bool Force;
        public bool AbsolutelyStraight;

        public StraightBulletData(bool force)
        {
            this.Force = force;
            this.AbsolutelyStraight = false;
        }

        public bool IsStraight()
        {
            return Force || AbsolutelyStraight;
        }
    }

    [Serializable]
    public class StraightBullet
    {
        public bool Enable;
        public CoordStruct sourcePos;
        public CoordStruct targetPos;
        public BulletVelocity Velocity;

        public StraightBullet(bool enable, CoordStruct sourcePos, CoordStruct targetPos, BulletVelocity bulletVelocity)
        {
            this.Enable = enable;
            this.sourcePos = sourcePos;
            this.targetPos = targetPos;
            this.Velocity = bulletVelocity;
        }
    }

    public partial class BulletExt
    {
        public bool SubjectToGround;
        private StraightBullet straightBullet;

        public unsafe void BulletClass_Init_StraightTrajectory()
        {
            Pointer<BulletClass> pBullet = OwnerObject;
            if (default == Type.SubjectToGround)
            {
                SubjectToGround = pBullet.Ref.Type.Ref.ROT != 1 && !Type.StraightBulletData.IsStraight();
            }
            else
            {
                SubjectToGround = Type.SubjectToGround >= 0;
            }
        }

        public unsafe void BulletClass_Put_StraightTrajectory(Pointer<CoordStruct> pCoord)
        {
            Pointer<BulletClass> pBullet = OwnerObject;
            if (null == Type.StraightBulletData ? pBullet.Ref.Type.Ref.ROT == 1 : Type.StraightBulletData.IsStraight())
            {
                // 直线弹道
                CoordStruct sourcePos = pBullet.Ref.SourceCoords;
                CoordStruct targetPos = pBullet.Ref.TargetCoords;

                // 绝对直线，重设目标坐标
                if (null != Type.StraightBulletData && Type.StraightBulletData.AbsolutelyStraight && !pBullet.Ref.Owner.IsNull)
                {
                    // Logger.Log("{0} 绝对直线弹道", pBullet.Ref.Type.Ref.Base.Base.ID);
                    double distance = targetPos.DistanceFrom(sourcePos);
                    DirStruct facing = pBullet.Ref.Owner.Ref.GetRealFacing().current();
                    // if (pBullet.Ref.Owner.Ref.HasTurret())
                    // {
                    //     facing = pBullet.Ref.Owner.Ref.TurretFacing.current();
                    // }
                    // else
                    // {
                    //     facing = pBullet.Ref.Owner.Ref.Facing.current();
                    // }
                    targetPos = ExHelper.GetFLHAbsoluteCoords(sourcePos, new CoordStruct((int)distance, 0, 0), facing);
                    pBullet.Ref.TargetCoords = targetPos;

                    // BulletEffectHelper.BlueLine(pBullet.Ref.SourceCoords, pBullet.Ref.TargetCoords, 1, 90);
                }

                // 重设速度
                BulletVelocity velocity = RecalculateBulletVelocity(sourcePos, targetPos);
                straightBullet = new StraightBullet(true, sourcePos, targetPos, velocity);

                // 设置触碰引信
                if (pBullet.Ref.Type.Ref.Proximity)
                {
                    // this.Proximity = new Proximity(pBullet.Ref.Owner, pBullet.Ref.Type.Ref.CourseLockDuration);
                    ActiveProximity();
                }
            }
        }

        private unsafe BulletVelocity RecalculateBulletVelocity(CoordStruct sourcePos, CoordStruct targetPos)
        {
            BulletVelocity velocity = new BulletVelocity(targetPos.X - sourcePos.X, targetPos.Y - sourcePos.Y, targetPos.Z - sourcePos.Z);
            velocity *= OwnerObject.Ref.Speed / targetPos.DistanceFrom(sourcePos);
            OwnerObject.Ref.Velocity = velocity;
            return velocity;
        }

        public unsafe void BulletClass_Update_StraightTrajectory()
        {
            Pointer<BulletClass> pBullet = OwnerObject;

            if (IsStraight())
            {
                // 强制修正速度
                pBullet.Ref.Velocity = straightBullet.Velocity;
            }
        }

        public unsafe bool IsStraight()
        {
            if (null != straightBullet && straightBullet.Enable)
            {
                return true;
            }
            return false;
        }
    }

    public partial class BulletTypeExt
    {

        public StraightBulletData StraightBulletData;
        public int SubjectToGround; // 0=auto， 1=true, -1=false

        /// <summary>
        /// [ProjectileType]
        /// ROT=1
        /// Straight=yes
        /// AbsolutelyStraight=no
        /// SubjectToGround=yes
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        private void ReadStraightTrajectory(INIReader reader, string section)
        {
            bool straight = false;
            if (reader.ReadNormal(section, "Straight", ref straight))
            {
                StraightBulletData = new StraightBulletData(straight);
                SubjectToGround = -1;
            }

            bool absolutely = false;
            if (reader.ReadNormal(section, "AbsolutelyStraight", ref absolutely))
            {
                if (null == StraightBulletData)
                {
                    StraightBulletData = new StraightBulletData(true);
                }
                StraightBulletData.AbsolutelyStraight = absolutely;
                SubjectToGround = -1;
            }

            bool subjectToGround = true;
            if (reader.ReadNormal(section, "SubjectToGround", ref subjectToGround))
            {
                SubjectToGround = subjectToGround ? 1 : -1;
            }
        }
    }
}
