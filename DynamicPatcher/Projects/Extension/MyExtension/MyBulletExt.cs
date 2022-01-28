using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    public partial class BulletExt
    {

        public string MyExtensionTest = nameof(MyExtensionTest);

        public SwizzleablePointer<HouseClass> pSourceHouse = new SwizzleablePointer<HouseClass>(IntPtr.Zero);

        public unsafe void OnInit()
        {

        }

        public unsafe void OnPut(Pointer<CoordStruct> pCoord)
        {
            if (!OwnerObject.Ref.Owner.IsNull)
            {
                pSourceHouse.Pointer = OwnerObject.Ref.Owner.Ref.Owner;
            }
            BulletClass_Put_AntiBullet(pCoord);
            BulletClass_Put_ArcingTrajectory(pCoord);
            BulletClass_Put_AttachEffect(pCoord);
            BulletClass_Put_MissileTrajectory(pCoord);
            BulletClass_Put_ProximityRange(pCoord);
            BulletClass_Put_StraightTrajectory(pCoord);
            BulletClass_Put_Trail(pCoord);
        }

        public unsafe void OnUpdate()
        {
            BulletClass_Update_DestroySelf(); // 自毁使用AntiBullet逻辑制造伤害
            BulletClass_Update_AntiBullet();
            // 检查死亡
            if (OwnerObject.IsNull || OwnerObject.Ref.Base.InLimbo || !OwnerObject.Ref.Base.IsAlive || OwnerObject.Ref.Base.Health <= 0 ||
                (null != BulletLifeStatus && BulletLifeStatus.IsDetonate))
            {
                // Logger.Log("{0} {1} update事件检测到死亡.", OwnerObject, OwnerObject.Ref.Type.Ref.Base.Base.ID);
                return;
            }
            BulletClass_Update_ProximityRange();
            BulletClass_Update_StraightTrajectory();
            BulletClass_Update_Trail();
            // 重设属性
            BulletClass_Update_RecalculateStatus();
            // AE要在属性重设的后面，即便AE失效后也是在下一帧再改变属性
            BulletClass_Update_AttachEffect();
        }

        public unsafe void OnDetonate(CoordStruct location)
        {
            // 爆炸的位置离目标位置非常近视为命中
            CoordStruct targetLocation = location;
            bool snapped = false;
            int snapDistance = 64;
            Pointer<AbstractClass> pBulletTarget = OwnerObject.Ref.Target;
            if (!pBulletTarget.IsNull && OwnerObject.Ref.Base.DistanceFrom(pBulletTarget.Convert<ObjectClass>()) < snapDistance)
            {
                targetLocation = pBulletTarget.Convert<AbstractClass>().Ref.GetCoords();
                snapped = true;
            }
            if (snapped)
            {
                // 使用弹头爆炸事件进行AE赋予
                int damage = OwnerObject.Ref.Base.Health;
                Pointer<WarheadTypeClass> pWH = OwnerObject.Ref.WH;
                WarheadTypeExt whExt = WarheadTypeExt.ExtMap.Find(pWH);
                Pointer<ObjectClass> pAttacker = IntPtr.Zero;
                if (!OwnerObject.Ref.Owner.IsNull)
                {
                    pAttacker = OwnerObject.Ref.Owner.Convert<ObjectClass>();
                }

                // 检索爆炸范围内的单位类型
                List<Pointer<TechnoClass>> pTechnoList = ExHelper.GetCellSpreadTechnos(targetLocation, pWH.Ref.CellSpread, null != whExt ? whExt.AffectsAir : true, false);
                // Logger.Log("弹头{0}半径{1}, 影响的单位{2}个", pWH.Ref.Base.ID, pWH.Ref.CellSpread, pTechnoList.Count);
                foreach (Pointer<TechnoClass> pTarget in pTechnoList)
                {
                    // 检查死亡
                    if (pTarget.IsNull || pTarget.Ref.Base.InLimbo || !pTarget.Ref.Base.IsAlive || pTarget.Ref.Base.Health <= 0)
                    {
                        continue;
                    }
                    // CoordStruct pos1 = pTarget.Ref.Base.Location;
                    // BulletEffectHelper.GreenLine(pos1, pos1 + new CoordStruct(0, 0, 2048), 1, 15);
                    // Logger.Log("pTarget {0}", pTarget.Ref.Type.Ref.Base.Base.ID);
                    TechnoExt targetExt = TechnoExt.ExtMap.Find(pTarget);
                    if (null != targetExt)
                    {
                        int distanceFromEpicenter = (int)targetLocation.DistanceFrom(pTarget.Ref.Base.Location);
                        if (targetExt.AffectMe(pAttacker, pWH, pSourceHouse, whExt)
                            && targetExt.DamageMe(damage, distanceFromEpicenter, whExt, out int realDamage)
                            && (pTarget.Ref.Base.Health - realDamage) > 0 // 收到本次伤害后会死，就不再进行赋予
                        )
                        {
                            BulletClass_Detonate_AttachEffect_Techno(pAttacker, targetExt, whExt);
                        }
                    }
                }

                if (null != whExt ? whExt.AffectsBullet : false)
                {
                    // 检索爆炸范围内的抛射体类型
                    HashSet<Pointer<BulletClass>> pBulletList = ExHelper.GetCellSpreadBullets(targetLocation, pWH.Ref.CellSpread);
                    // Logger.Log("弹头{0}半径{1}, 影响的抛射体{2}个", pWH.Ref.Base.ID, pWH.Ref.CellSpread, pBulletList.Count);
                    foreach (Pointer<BulletClass> pBullet in pBulletList)
                    {
                        // 检查死亡
                        // Logger.Log(" - {0}, InLimbo = {1}, IsAlive = {2}， Health = {3}", pBullet, pBullet.IsNull ? "null" : pBullet.Ref.Base.InLimbo, pBullet.IsNull ? "null" : pBullet.Ref.Base.IsAlive, pBullet.IsNull ? "null" : pBullet.Ref.Base.Health);
                        if (pBullet.IsNull || pBullet.Ref.Type.Ref.Inviso || pBullet.Ref.Base.InLimbo || !pBullet.Ref.Base.IsAlive || pBullet.Ref.Base.Health <= 0)
                        {
                            continue;
                        }
                        if (pBullet != OwnerObject)
                        {
                            BulletExt targetExt = BulletExt.ExtMap.Find(pBullet);
                            if (null != targetExt)
                            {
                                bool canAffect = false;
                                if (canAffect = whExt.CanAffectHouse(targetExt.pSourceHouse, pSourceHouse))
                                {
                                    BulletClass_Detonate_AttachEffect_Bullet(pAttacker, targetExt, whExt);
                                }
                            }
                        }
                    }
                }
            }

            BulletClass_Detonate_AttachEffect(location);
            BulletClass_Detonate_AntiBullet(location);
        }

        public unsafe void OnUnInit()
        {
            BulletClass_UnInit_AttachEffect();
        }

    }

    public partial class BulletTypeExt
    {

        [INILoadAction]
        public void LoadINI(Pointer<CCINIClass> pINI)
        {
            // rules reader
            INIReader reader = new INIReader(pINI);
            string section = OwnerObject.Ref.Base.Base.ID;

            // art reader
            INIReader artReader = reader;
            if (null != CCINIClass.INI_Art && !CCINIClass.INI_Art.IsNull)
            {
                artReader = new INIReader(CCINIClass.INI_Art);
            }
            string artSection = section;
            string image = default;
            if (reader.ReadNormal(section, "Image", ref image))
            {
                artSection = image;
            }

            ReadAresFlags(reader, section);

            ReadAntiBullet(reader, section);
            ReadAttachEffect(reader, section);
            ReadArcingTrajectory(reader, section);
            ReadMissileTrajectory(reader, section);
            ReadProximity(reader, section);
            ReadStraightTrajectory(reader, section);
            ReadTrail(reader, section, artReader, artSection);
        }

    }

}
