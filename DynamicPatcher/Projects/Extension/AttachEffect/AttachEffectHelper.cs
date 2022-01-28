using System.IO;
using System.Collections;
using DynamicPatcher;
using Extension.Ext;
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
    public class AttachEffectHelper
    {

        public static int FindInsertIndex(AttachEffectManager manager, AttachEffect ae)
        {
            if (null != ae.Stand && ae.Stand.Type.IsTrain)
            {
                StandType type = ae.Stand.Type;
                int index = -1;
                // 插入头还是尾
                if (type.CabinHead)
                {
                    // 插入队列末位
                    // 检查是否有分组
                    if (type.CabinGroup > -1)
                    {
                        // 倒着找自己的分组
                        for (int j = manager.Count() - 1; j >= 0; j--)
                        {
                            AttachEffect temp = manager.AttachEffects[j];
                            Stand tempStand = null;
                            if (null != (tempStand = temp.Stand))
                            {
                                if (type.CabinGroup == tempStand.Type.CabinGroup)
                                {
                                    // 找到组员
                                    index = j;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // 插入队列首位
                    index = 0;
                    // 检查是否有分组
                    if (type.CabinGroup > -1)
                    {
                        // 顺着找自己的分组
                        for (int j = 0; j < manager.Count(); j++)
                        {
                            AttachEffect temp = manager.AttachEffects[j];
                            Stand tempStand = null;
                            if (null != (tempStand = temp.Stand))
                            {
                                if (type.CabinGroup == tempStand.Type.CabinGroup)
                                {
                                    // 找到组员
                                    index = j;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (index > -1)
                {
                    return index;
                }
            }
            return 0;
        }

        public static void UpdateStandLocation(AttachEffectManager manager, Pointer<ObjectClass> pObject, Stand stand, ref int markIndex)
        {
            if (stand.Type.IsTrain)
            {
                // 查找可以用的记录点
                double length = 0;
                LocationMark preMark = null;
                for (int j = markIndex; j < manager.LocationMarks.Count; j++)
                {
                    markIndex = j;
                    LocationMark mark = manager.LocationMarks[j];
                    if (null == preMark)
                    {
                        preMark = mark;
                        continue;
                    }
                    length += mark.Location.DistanceFrom(preMark.Location);
                    preMark = mark;
                    if (length >= manager.LocationSpace)
                    {
                        break;
                    }
                }

                if (null != preMark)
                {
                    stand.UpdateLocation(preMark);
                    return;
                }
            }
            // 获取挂载对象的位置和方向
            LocationMark location = GetLocation(pObject, stand.Type);
            stand.UpdateLocation(location);
        }

        public static LocationMark GetLocation(Pointer<ObjectClass> pObject, StandType standType)
        {
            CoordStruct sourcePos = pObject.Ref.Location;

            CoordStruct targetPos = sourcePos;
            DirStruct targetDir = default;
            if (standType.IsOnWorld)
            {
                // 绑定世界坐标，朝向固定北向
                targetDir = new DirStruct();
                targetPos = ExHelper.GetFLHAbsoluteCoords(sourcePos, standType.Offset, targetDir);
            }
            else
            {
                // 绑定单体坐标
                switch (pObject.Ref.Base.WhatAmI())
                {
                    case AbstractType.Unit:
                    case AbstractType.Aircraft:
                    case AbstractType.Building:
                    case AbstractType.Infantry:
                        // 以单位获得参考，取目标的位点
                        Pointer<TechnoClass> pTechno = pObject.Convert<TechnoClass>();
                        // 获取方向
                        targetDir = GetDirection(pTechno, standType.Direction, standType.IsOnTurret);
                        // 获取位置
                        targetPos = ExHelper.GetFLHAbsoluteCoords(pTechno, standType.Offset, standType.IsOnTurret);
                        break;
                    case AbstractType.Bullet:
                        // 以抛射体作为参考，取抛射体当前位置和目标位置获得方向，按照方向获取发射位点和目标位点
                        Pointer<BulletClass> pBullet = pObject.Convert<BulletClass>();
                        // 增加抛射体偏移值取下一帧所在实际位置
                        sourcePos += pBullet.Ref.Velocity.ToCoordStruct();
                        // 获取面向
                        targetDir = ExHelper.Point2Dir(sourcePos, pBullet.Ref.TargetCoords);
                        targetPos = ExHelper.GetFLHAbsoluteCoords(sourcePos, standType.Offset, targetDir);
                        break;
                }
            }
            return new LocationMark(targetPos, targetDir);
        }

        public static DirStruct GetDirection(Pointer<TechnoClass> pMaster, int dir, bool isOnTurret)
        {
            // turn offset
            DirStruct targetDir = ExHelper.DirNormalized(dir, 16);

            if (pMaster.Convert<AbstractClass>().Ref.WhatAmI() != AbstractType.Building)
            {
                double targetRad = targetDir.radians();

                DirStruct sourceDir = pMaster.Ref.Facing.current();
                if (!pMaster.Ref.IsSinking)
                {
                    if (isOnTurret)
                    {
                        sourceDir = pMaster.Ref.GetRealFacing().current();
                    }
                    double sourceRad = sourceDir.radians();

                    float angle = (float)(sourceRad - targetRad);
                    targetDir = ExHelper.Radians2Dir(angle);
                }
            }

            return targetDir;
        }

    }

}