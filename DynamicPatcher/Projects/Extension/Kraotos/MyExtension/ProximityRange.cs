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

    [Serializable]
    public class ProximityData
    {
        public bool Force;
        public int Arm;
        public int ZOffset;
        public bool AffectsOwner;
        public bool AffectsAllies;
        public bool AffectsEnemies;
        public bool AffectsClocked;

        public bool Penetration;
        public string PenetrationWarhead;
        public string PenetrationWeapon;
        public int PenetrationTimes;
        public bool PenetrationBuildingOnce;

        public ProximityData()
        {
            this.Force = false;
            this.Arm = 128;
            this.ZOffset = Game.LevelHeight;
            this.AffectsOwner = false;
            this.AffectsAllies = false;
            this.AffectsEnemies = true;
            this.AffectsClocked = true;

            this.Penetration = false;
            this.PenetrationWarhead = null;
            this.PenetrationWeapon = null;
            this.PenetrationTimes = -1;
            this.PenetrationBuildingOnce = false;
        }
    }

    [Serializable]
    public class Proximity
    {
        public ProximityData Data;

        public bool Enable;
        public SwizzleablePointer<CellClass> pCheckedCell;
        public List<SwizzleablePointer<BuildingClass>> BuildingMarks;

        private bool count;
        private int times;
        private bool safe;
        private TimerStruct safeTimer;

        public Proximity(ProximityData data, Pointer<TechnoClass> pAttacker, int safeDelay)
        {
            this.Data = data;
            this.Enable = true;
            this.pCheckedCell = new SwizzleablePointer<CellClass>(IntPtr.Zero);
            this.BuildingMarks = new List<SwizzleablePointer<BuildingClass>>();
            this.safe = safeDelay > 0;
            this.safeTimer.Start(safeDelay);
            this.count = data.PenetrationTimes > 0;
            this.times = data.PenetrationTimes;
        }

        public bool IsSafe()
        {
            if (safe)
            {
                safe = safeTimer.InProgress();
            }
            return safe;
        }

        public void ThroughOnce()
        {
            if (count)
            {
                times--;
            }
        }

        public bool Explodes()
        {
            return !Data.Penetration || (count && times <= 0);
        }

        public bool CheckAndMarkBuilding(Pointer<BuildingClass> pBuilding)
        {
            bool find = false;
            for (int i = 0; i < BuildingMarks.Count; i++)
            {
                SwizzleablePointer<BuildingClass> pMark = BuildingMarks[i];
                if (pBuilding == pMark)
                {
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                SwizzleablePointer<BuildingClass> mark = new SwizzleablePointer<BuildingClass>(pBuilding);
                BuildingMarks.Add(mark);
            }
            return find;
        }
    }

    [Serializable]
    public class ProximityRangeData
    {
        public int Range;
        public bool Random;
        public int MaxRange;
        public int MinRange;

        public ProximityRangeData(int range)
        {
            this.Range = range;
            this.Random = false;
            this.MinRange = 0;
            this.MaxRange = range;
        }

    }

    [Serializable]
    public class ProximityRange
    {
        public ProximityRangeData Data;
        public bool Enable;
        public int Range;

        public ProximityRange(ProximityRangeData data, int range)
        {
            this.Data = data;
            this.Enable = range > 0;
            this.Range = range;
        }
    }

    public partial class BulletExt
    {
        private ProximityRange proximityRange;

        public Proximity Proximity;

        public unsafe void BulletClass_Put_ProximityRange(Pointer<CoordStruct> pCoord)
        {
            Pointer<BulletClass> pBullet = OwnerObject;

            // 设置提前引爆抛射体
            Pointer<WeaponTypeClass> pWeapon = pBullet.Ref.WeaponType;
            if (!pWeapon.IsNull)
            {
                WeaponTypeExt ext = WeaponTypeExt.ExtMap.Find(pWeapon);
                if (null != ext.ProximityRangeData)
                {
                    int range = ext.ProximityRangeData.Range;
                    if (ext.ProximityRangeData.Random)
                    {
                        range = MathEx.Random.Next(ext.ProximityRangeData.MinRange, ext.ProximityRangeData.MaxRange);
                    }
                    proximityRange = new ProximityRange(ext.ProximityRangeData, range);
                }
            }

            // 设置触碰引信
            if (Type.ProximityData.Force)
            {
                // this.Proximity = new Proximity(pBullet.Ref.Owner, pBullet.Ref.Type.Ref.CourseLockDuration);
                ActiveProximity();
            }
        }

        public unsafe void ActiveProximity()
        {
            this.Proximity = new Proximity(Type.ProximityData, OwnerObject.Ref.Owner, OwnerObject.Ref.Type.Ref.CourseLockDuration);
        }

        public unsafe void BulletClass_Update_ProximityRange()
        {
            Pointer<BulletClass> pBullet = OwnerObject;
            CoordStruct sourcePos = pBullet.Ref.Base.Base.GetCoords();
            BulletVelocity velocity = pBullet.Ref.Velocity;
            // 加上提前量才是当前帧所在位置
            sourcePos += new CoordStruct((int)velocity.X, (int)velocity.Y, (int)velocity.Z);

            // 提前引爆抛射体
            if (null != proximityRange && proximityRange.Enable)
            {
                if (sourcePos.DistanceFrom(pBullet.Ref.TargetCoords) <= proximityRange.Range)
                {
                    // KABOOM!
                    ManualDetonation(sourcePos);
                }
            }

            // 启用触碰引信
            if (null != Proximity && Proximity.Enable && !Proximity.IsSafe())
            {
                Pointer<TechnoClass> pBulletOwner = OwnerObject.Ref.Owner;


                // 计算碰撞的半径，超过1格，确定搜索范围
                int cellSpread = (Proximity.Data.Arm / 256) + 1;
                // Logger.Log("Arm = {0}，确定搜索范围 {1} 格", Proximity.Data.Arm, cellSpread);

                // 每个格子只检查一次
                if (MapClass.Instance.TryGetCellAt(sourcePos, out Pointer<CellClass> pCell) && pCell != Proximity.pCheckedCell)
                {
                    Proximity.pCheckedCell.Pointer = pCell;
                    CoordStruct cellPos = pCell.Ref.Base.GetCoords();

                    // BulletEffectHelper.GreenCell(cellPos, 128, 1, 75);

                    // 获取这个格子上的所有对象，不包括飞机
                    HashSet<Pointer<TechnoClass>> pTechnoSet = new HashSet<Pointer<TechnoClass>>();

                    // 搜索范围加大一格，搜索需要排除建筑
                    // 获取范围内的所有对象，不包括飞行器
                    CellSpreadEnumerator enumerator = new CellSpreadEnumerator((uint)cellSpread);
                    do
                    {
                        CellStruct cur = pCell.Ref.MapCoords;
                        CellStruct offset = enumerator.Current;
                        if (MapClass.Instance.TryGetCellAt(cur + offset, out Pointer<CellClass> pCheckCell))
                        {
                            // BulletEffectHelper.RedCell(pCheckCell.Ref.Base.GetCoords(), 128, 1, 30);
                            ExHelper.FindTechnoInCell(pCheckCell, (pTarget) =>
                            {
                                // 检查死亡和发射者和替身
                                if (!IsDeadOrStand(pTarget, pBulletOwner))
                                {
                                    // 过滤建筑，建筑只查当前格
                                    if (pTarget.Ref.Base.Base.WhatAmI() != AbstractType.Building || pCheckCell == pCell)
                                    {
                                        pTechnoSet.Add(pTarget);
                                    }
                                }
                                return false;
                            });
                            // 获取JJ
                            Pointer<TechnoClass> pJJ = pCheckCell.Ref.Jumpjet.Convert<TechnoClass>();
                            if (!pJJ.IsNull && !IsDeadOrStand(pJJ, pBulletOwner))
                            {
                                pTechnoSet.Add(pJJ);
                            }
                        }
                    } while (enumerator.MoveNext());

                    // 获取所有在天上的玩意儿，JJ，飞起来的坦克，包含路过的飞机
                    ExHelper.FindTechno(IntPtr.Zero, (pTarget) =>
                    {
                        if (pTarget.Ref.Base.GetHeight() > 0 && !IsDeadOrStand(pTarget, pBulletOwner))
                        {
                            // 消去高度差，检查和当前坐标距离在cellSpread格范围内
                            CoordStruct targetPos = pTarget.Ref.Base.Base.GetCoords();
                            targetPos.Z = cellPos.Z;
                            if (targetPos.DistanceFrom(cellPos) <= cellSpread * 256)
                            {
                                pTechnoSet.Add(pTarget);
                            }
                        }
                        return false;
                    }, true, true, true, true);

                    // 筛选并处理找到的对象
                    foreach (Pointer<TechnoClass> pTarget in pTechnoSet)
                    {
                        CoordStruct targetPos = pTarget.Ref.Base.Base.GetCoords();
                        // BulletEffectHelper.BlueLineZ(targetPos, 1024, 1, 75);

                        bool hit = false;

                        if (pTarget.Ref.Base.Base.WhatAmI() == AbstractType.Building)
                        {
                            // 检查建筑在范围内
                            Pointer<BuildingClass> pBuilding = pTarget.Convert<BuildingClass>();
                            int height = pBuilding.Ref.Type.Ref.Height;
                            // Logger.Log("Building Height {0}", height);
                            // 建筑只获取抛射体经过的当前格，所以判断高度在范围内即可算命中
                            hit = sourcePos.Z <= (targetPos.Z + height * Game.LevelHeight + Proximity.Data.ZOffset);
                            // 检查建筑是否被炸过
                            if (hit && Proximity.Data.PenetrationBuildingOnce)
                            {
                                hit = !Proximity.CheckAndMarkBuilding(pBuilding);
                            }
                        }
                        else
                        {
                            // 使用抛射体所经过的格子的中心点作为判定原点
                            CoordStruct sourceTestPos = cellPos;
                            // 判定原点抬升至与抛射体同高
                            sourceTestPos.Z = sourcePos.Z;
                            // 目标点在脚下，加上高度修正偏移值
                            CoordStruct targetTestPos = targetPos + new CoordStruct(0, 0, Proximity.Data.ZOffset);
                            // BulletEffectHelper.RedCrosshair(sourceTestPos, 128, 1, 75);
                            // BulletEffectHelper.RedCrosshair(targetTestPos, 128, 1, 75);
                            // BulletEffectHelper.BlueLine(sourceTestPos, targetTestPos, 3, 75);
                            hit = targetTestPos.DistanceFrom(sourceTestPos) <= Proximity.Data.Arm;
                            // Logger.Log("目标单位坐标加上修正值{0}, 与抛射体的距离{1}，检测半径{2}", Proximity.Data.ZOffset, targetTestPos.DistanceFrom(sourceTestPos), Proximity.Data.Arm);
                        }

                        // 敌我识别并引爆
                        if (hit && AffectTarget(pTarget))
                        {
                            // 引爆
                            CoordStruct detonatePos = targetPos; // 爆点在与目标位置
                            // BulletEffectHelper.RedCrosshair(detonatePos, 2048, 1, 75);
                            // Logger.Log("抛射体位置{0}，偏移位置{1}", sourcePos, detonatePos);
                            if (ManualDetonation(sourcePos, !Proximity.Data.Penetration, pBulletOwner, pTarget.Convert<AbstractClass>(), detonatePos))
                            {
                                // 爆了就结束了
                                break;
                            }
                        }

                    }
                }
            }
        }

        private bool IsDeadOrStand(Pointer<TechnoClass> pTarget, Pointer<TechnoClass> pBulletOwner)
        {
            // 检查死亡和发射者
            if (pTarget.IsNull || pTarget == pBulletOwner || (Proximity.Data.AffectsClocked ? pTarget.IsDeadOrInvisible() : pTarget.IsDeadOrInvisibleOrCloaked()) || pTarget.Ref.IsImmobilized)
            {
                return true;
            }
            // 过滤掉替身
            TechnoExt targetExt = TechnoExt.ExtMap.Find(pTarget);
            if (null == targetExt || !targetExt.MyMaster.IsNull)
            {
                return true;
            }
            return false;
        }

        private bool AffectTarget(Pointer<TechnoClass> pTarget)
        {
            Pointer<HouseClass> pTargetOwner = IntPtr.Zero;
            if (!pTarget.IsNull && !(pTargetOwner = pTarget.Ref.Owner).IsNull)
            {
                if (pSourceHouse == pTargetOwner)
                {
                    return Proximity.Data.AffectsAllies || Proximity.Data.AffectsOwner;
                }
                else if (pSourceHouse.Ref.IsAlliedWith(pTargetOwner))
                {
                    return Proximity.Data.AffectsAllies;
                }
                else
                {
                    return Proximity.Data.AffectsEnemies;
                }
            }
            return false;
        }

        private bool ManualDetonation(CoordStruct sourcePos, bool KABOOM = true, Pointer<TechnoClass> pBulletOwner = default, Pointer<AbstractClass> pTarget = default, CoordStruct detonatePos = default)
        {
            if (!KABOOM && pBulletOwner.IsDead())
            {
                // 发射者死亡，抛射体跟着一起玩完
                Proximity = null;
                KABOOM = true;
            }

            // 检查穿透次数是否用完
            KABOOM = KABOOM || null == Proximity || Proximity.Explodes();

            if (KABOOM)
            {
                // 抛射体原地爆炸
                // string pTargetID = pTarget.IsNull ? "null" : pTarget.Ref.WhatAmI().ToString();
                // if (pTarget.CastToTechno(out Pointer<TechnoClass> pT))
                // {
                //     pTargetID = pT.Ref.Type.Ref.Base.Base.ID;
                // }
                // Logger.Log("抛射体{0}检测到碰撞{1}，在{2}位置引爆", OwnerObject.Ref.Type.Ref.Base.Base.ID, pTargetID, sourcePos);
                OwnerObject.Ref.Detonate(sourcePos);
                OwnerObject.Ref.Base.Remove();
                OwnerObject.Ref.Base.UnInit();
            }
            else if (!pTarget.IsNull)
            {
                // 抛射体原地爆炸，但不销毁，并且启用自定义武器，自定义弹头
                if (default == detonatePos)
                {
                    detonatePos = sourcePos;
                }

                // 使用自身制造伤害
                int damage = OwnerObject.Ref.Base.Health;
                Pointer<WarheadTypeClass> pWH = OwnerObject.Ref.WH;

                // 检查自定义武器是否存在，存在则使用自定义武器制造伤害，不存在就用自身制造伤害
                string weaponId = Proximity.Data.PenetrationWeapon;
                if (!pBulletOwner.IsNull && !string.IsNullOrEmpty(weaponId))
                {
                    // 对敌人造成自定义武器的伤害
                    Pointer<WeaponTypeClass> pWeapon = WeaponTypeClass.ABSTRACTTYPE_ARRAY.Find(weaponId);
                    if (!pWeapon.IsNull)
                    {
                        damage = pWeapon.Ref.Damage;
                        pWH = pWeapon.Ref.Warhead;
                    }
                }
                // 检查是否使用其他弹头
                string warheadId = Proximity.Data.PenetrationWarhead;
                if (!string.IsNullOrEmpty(warheadId))
                {
                    Pointer<WarheadTypeClass> pOverrideWH = WarheadTypeClass.ABSTRACTTYPE_ARRAY.Find(warheadId);
                    if (!pOverrideWH.IsNull)
                    {
                        pWH = pOverrideWH;
                    }
                }

                // 在预定引爆地点引爆弹头
                MapClass.DamageArea(detonatePos, damage, pBulletOwner, pWH, pWH.Ref.Tiberium, pSourceHouse);
                // 播放弹头动画
                LandType landType = Proximity.pCheckedCell.IsNull ? LandType.Clear : Proximity.pCheckedCell.Ref.LandType;
                Pointer<AnimTypeClass> pAnimType = MapClass.SelectDamageAnimation(damage, pWH, landType, sourcePos);
                if (!pAnimType.IsNull)
                {
                    Pointer<AnimClass> pAnim = YRMemory.Create<AnimClass>(pAnimType, sourcePos);
                    pAnim.Ref.Owner = pSourceHouse;
                }
                // 计数器减1
                Proximity.ThroughOnce();
            }
            return KABOOM;
        }
    }

    public partial class BulletTypeExt
    {

        public ProximityData ProximityData = new ProximityData();

        /// <summary>
        /// Proximity.Force=no
        /// Proximity.Arm=128
        /// Proximity.ZOffset=104
        /// Proximity.AffectsOwner=no
        /// Proximity.AffectsAllies=no
        /// Proximity.AffectsEnemies=yes
        /// Proximity.AffectsClocked=yes
        /// 
        /// Proximity.Penetration=no
        /// Proximity.PenetrationWarhead=HE
        /// Proximity.PenetrationWeapon=RedEye2
        /// Proximity.PenetrationTimes=-1
        /// Proximity.PenetrationBuildingOnce=no
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadProximity(INIReader reader, string section)
        {
            bool force = false;
            if (reader.ReadNormal(section, "Proximity.Force", ref force))
            {
                ProximityData.Force = force;
            }

            int arm = 0;
            if (reader.ReadNormal(section, "Proximity.Arm", ref arm))
            {
                ProximityData.Arm = arm;
            }

            int z = 0;
            if (reader.ReadNormal(section, "Proximity.ZOffset", ref z))
            {
                ProximityData.ZOffset = z;
            }

            bool affectsOwner = false;
            if (reader.ReadNormal(section, "Proximity.AffectsOwner", ref affectsOwner))
            {
                ProximityData.AffectsOwner = affectsOwner;
            }

            bool affectsAllies = false;
            if (reader.ReadNormal(section, "Proximity.AffectsAllies", ref affectsAllies))
            {
                ProximityData.AffectsAllies = affectsAllies;
            }

            bool affectsEnemies = true;
            if (reader.ReadNormal(section, "Proximity.AffectsEnemies", ref affectsEnemies))
            {
                ProximityData.AffectsEnemies = affectsEnemies;
            }

            bool affectsClocked = true;
            if (reader.ReadNormal(section, "Proximity.AffectsClocked", ref affectsClocked))
            {
                ProximityData.AffectsClocked = affectsClocked;
            }


            bool penetration = true;
            if (reader.ReadNormal(section, "Proximity.Penetration", ref penetration))
            {
                ProximityData.Penetration = penetration;
            }

            string penetrationWarhead = null;
            if (reader.ReadNormal(section, "Proximity.PenetrationWarhead", ref penetrationWarhead))
            {
                if (!string.IsNullOrEmpty(penetrationWarhead))
                {
                    ProximityData.PenetrationWarhead = penetrationWarhead;
                }
            }

            string penetrationWeapon = null;
            if (reader.ReadNormal(section, "Proximity.PenetrationWeapon", ref penetrationWeapon))
            {
                if (!string.IsNullOrEmpty(penetrationWeapon))
                {
                    ProximityData.PenetrationWeapon = penetrationWeapon;
                }
            }

        }
    }

    public partial class WeaponTypeExt
    {

        public ProximityRangeData ProximityRangeData;

        /// <summary>
        /// [WeaponType]
        /// ProximityRange=0
        /// ProximityRange.Random=0,1
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadProximityRange(INIReader reader, string section)
        {
            int range = 0;
            if (reader.ReadNormal(section, "ProximityRange", ref range))
            {
                ProximityRangeData = new ProximityRangeData(range * 256);
            }

            List<int> randomRange = null;
            if (ExHelper.ReadIntList(reader, section, "ProximityRange.Random", ref randomRange))
            {
                if (null != randomRange && randomRange.Count > 1)
                {
                    if (null == ProximityRangeData)
                    {
                        ProximityRangeData = new ProximityRangeData(0);
                    }
                    ProximityRangeData.Random = true;
                    ProximityRangeData.MinRange = randomRange[0] * 256;
                    ProximityRangeData.MaxRange = randomRange[1] * 256;
                    if (ProximityRangeData.MaxRange < ProximityRangeData.MinRange)
                    {
                        int temp = ProximityRangeData.MaxRange;
                        ProximityRangeData.MaxRange = ProximityRangeData.MinRange;
                        ProximityRangeData.MinRange = temp;
                    }
                }
            }
        }
    }
}
