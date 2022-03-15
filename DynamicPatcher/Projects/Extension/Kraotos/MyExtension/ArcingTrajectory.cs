using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    public partial class BulletExt
    {

        public unsafe void BulletClass_Put_ArcingTrajectory(Pointer<CoordStruct> pCoord)
        {
            Pointer<BulletClass> pBullet = OwnerObject;
            if (pBullet.Ref.Type.Ref.Arcing && Type.ArcingAdvanced)
            {
                CoordStruct sourcePos = pCoord.Data;
                CoordStruct targetPos = pBullet.Ref.TargetCoords;

                // 速度控制
                if (Type.ArcingFixedSpeed > 0)
                {
                    // Logger.Log("原抛射体速度{0}, 改使用恒定速度{1}", pBullet.Ref.Speed, Type.ArcingFixedSpeed);
                    pBullet.Ref.Speed = Type.ArcingFixedSpeed;
                }
                else
                {
                    // Logger.Log("原抛射体速度{0}, 高级弹道学, 加速度{1}", pBullet.Ref.Speed, pBullet.Ref.Type.Ref.Acceleration);
                    pBullet.Ref.Speed += pBullet.Ref.Type.Ref.Acceleration;
                }

                // 高抛弹道
                if (!pBullet.Ref.WeaponType.IsNull && pBullet.Ref.WeaponType.Ref.Lobber)
                {
                    pBullet.Ref.Speed = (int)(pBullet.Ref.Speed * 0.5);
                    // Logger.Log("高抛弹道, 削减速度{0}", pBullet.Ref.Speed);
                }

                // 不精确
                if (pBullet.Ref.Type.Ref.Inaccurate)
                {
                    
                    // 不精确, 需要修改目标坐标
                    int min = Type.Ares.BallisticScatterMin;
                    int max = Type.Ares.BallisticScatterMax > 0 ? Type.Ares.BallisticScatterMax : RulesClass.Global().BallisticScatter;
                    // Logger.Log("炮弹[{0}]不精确, 需要重新计算目标位置, 散布范围=[{1}, {2}]", pBullet.Ref.Type.Convert<AbstractTypeClass>().Ref.ID, min, max);
                    if (min > max)
                    {
                        int temp = min;
                        min = max;
                        max = temp;
                    }
                    // 随机
                    double r = MathEx.Random.Next(min, max);
                    var theta = MathEx.Random.NextDouble() * 2 * Math.PI;
                    CoordStruct offset = new CoordStruct((int)(r * Math.Cos(theta)), (int)(r * Math.Sin(theta)), 0);
                    targetPos += offset;
                    pBullet.Ref.TargetCoords = targetPos;
                    // Logger.Log("计算结果, 随机半径{0}[{1},{2}], 随机角度{3}, 偏移{4}", r, min, max, theta, offset);
                }

                // 重算抛物线弹道
                int zDiff = targetPos.Z - sourcePos.Z;
                targetPos.Z = 0;
                sourcePos.Z = 0;
                double distance = targetPos.DistanceFrom(sourcePos);
                // Logger.Log("位置和目标的水平距离{0}", distance);
                double speed = pBullet.Ref.Speed;
                // Logger.Log("重新计算初速度, 当前速度{0}", speed);
                double vZ = (zDiff * speed) / distance + (0.5 * RulesClass.Global().Gravity * distance) / speed;
                // Logger.Log("计算Z方向的初始速度{0}", vZ);
                BulletVelocity v = new BulletVelocity(targetPos.X - sourcePos.X, targetPos.Y - sourcePos.Y, 0);
                v *= speed / distance;
                v.Z = vZ;
                pBullet.Ref.Velocity = v;
            }
        }

    }

    public partial class BulletTypeExt
    {
        public bool ArcingAdvanced = true;
        public int ArcingFixedSpeed = 0;

        /// <summary>
        /// [ProjectileType]
        /// AdvancedBallistics=yes
        /// Arcing=yes
        /// Arcing.FixedSpeed=0
        /// Acceleration=0
        /// Inaccurate=yes
        /// BallisticScatter.Min=0
        /// BallisticScatter.Max=BallisticScatter
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        private void ReadArcingTrajectory(INIReader reader, string section)
        {
            bool advanced = true;
            if (reader.ReadNormal(section, "AdvancedBallistics", ref advanced))
            {
                ArcingAdvanced = advanced;
            }

            int fixedSpeed = 0;
            if (reader.ReadNormal(section, "Arcing.FixedSpeed", ref fixedSpeed))
            {
                ArcingFixedSpeed = fixedSpeed;
            }
        }
    }
}