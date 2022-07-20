using System.Drawing;
using System.Threading;
using PatcherYRpp;
using PatcherYRpp.FileFormats;
using PatcherYRpp.Utilities;
using Extension;
using Extension.Utilities;
using DynamicPatcher;
using System;
using System.Collections.Generic;
using System.Reflection;
using Extension.Ext;

namespace Extension.Utilities
{

    [Flags]
    public enum Relation
    {
        NONE = 0x0, OWNER = 0x1, ALLIES = 0x2, ENEMIES = 0x4,

        Team = OWNER | ALLIES,
        NotAllies = OWNER | ENEMIES,
        NotOwner = ALLIES | ENEMIES,
        All = OWNER | ALLIES | ENEMIES
    }


    public static partial class ExHelper
    {

        public static bool IsCivilian(this Pointer<HouseClass> pHouse)
        {
            return pHouse.IsNull || pHouse.Ref.Defeated || pHouse.Ref.Type.IsNull
                || HouseClass.CIVILIAN == pHouse.Ref.Type.Ref.Base.ID
                || HouseClass.SPECIAL == pHouse.Ref.Type.Ref.Base.ID; // 被狙掉驾驶员的阵营是Special
        }

        public static Relation GetRelationWithPlayer(this Pointer<HouseClass> pHouse)
        {
            return pHouse.GetRelation(HouseClass.Player);
        }

        public static Relation GetRelation(this Pointer<HouseClass> pHosue, Pointer<HouseClass> pTargetHouse)
        {
            if (pHosue == pTargetHouse)
            {
                return Relation.OWNER;
            }
            if (pHosue.Ref.IsAlliedWith(pTargetHouse))
            {
                return Relation.ALLIES;
            }
            return Relation.ENEMIES;
        }

        public static bool CastToBullet(this Pointer<ObjectClass> pObject, out Pointer<BulletClass> pBullet)
        {
            return pObject.CastIf(AbstractType.Bullet, out pBullet);
        }

        public static bool CastToBuilding(this Pointer<ObjectClass> pObject, out Pointer<BuildingClass> pBuilding)
        {
            return pObject.CastIf(AbstractType.Building, out pBuilding);
        }

        public static CoordStruct ToCoordStruct(this BulletVelocity bulletVelocity)
        {
            return new CoordStruct(bulletVelocity.X, bulletVelocity.Y, bulletVelocity.Z);
        }

        public static CoordStruct ToCoordStruct(this SingleVector3D vector3D)
        {
            return new CoordStruct(vector3D.X, vector3D.Y, vector3D.Z);
        }

        public static BulletVelocity ToBulletVelocity(this CoordStruct coord)
        {
            return new BulletVelocity(coord.X, coord.Y, coord.Z);
        }

        public static BulletVelocity ToBulletVelocity(this SingleVector3D vector3D)
        {
            return new BulletVelocity(vector3D.X, vector3D.Y, vector3D.Z);
        }

        public static SingleVector3D ToSingleVector3D(this CoordStruct coord)
        {
            return new SingleVector3D(coord.X, coord.Y, coord.Z);
        }

        public static Point2D ToClientPos(this CoordStruct coord)
        {
            return TacticalClass.Instance.Ref.CoordsToClient(coord);
        }

        public static SingleVector3D ToSingleVector3D(this BulletVelocity bulletVelocity)
        {
            return new SingleVector3D(bulletVelocity.X, bulletVelocity.Y, bulletVelocity.Z);
        }

        public static ColorStruct ToColorAdd(this ColorStruct color)
        {
            int B = color.B >> 3;
            int G = color.G >> 2;
            int R = color.R >> 3;
            return new ColorStruct(R, G, B);
        }

        public static uint Add2RGB565(this ColorStruct colorAdd)
        {
            string R2 = Convert.ToString(colorAdd.R, 2).PadLeft(5, '0');
            string G2 = Convert.ToString(colorAdd.G, 2).PadLeft(6, '0');
            string B2 = Convert.ToString(colorAdd.B, 2).PadLeft(5, '0');
            string c2 = R2 + G2 + B2;
            return Convert.ToUInt32(c2, 2);
        }

        public static int GetRandomValue(this Point2D point, int defVal)
        {
            int min = point.X;
            int max = point.Y;
            if (min > max)
            {
                min = max;
                max = point.X;
            }
            if (max > 0)
            {
                return MathEx.Random.Next(min, max);
            }
            return defVal;
        }

        public static bool IsDead(this Pointer<TechnoClass> pTechno)
        {
            return pTechno.IsNull || pTechno.Convert<ObjectClass>().IsDead() || pTechno.Ref.IsCrashing || pTechno.Ref.IsSinking;
        }

        public static bool IsDead(this Pointer<ObjectClass> pObject)
        {
            return pObject.IsNull || pObject.Ref.Health <= 0 || !pObject.Ref.IsAlive;
        }

        public static bool IsInvisible(this Pointer<TechnoClass> pTechno)
        {
            return pTechno.IsNull || pTechno.Convert<ObjectClass>().IsInvisible();
        }

        public static bool IsInvisible(this Pointer<ObjectClass> pObject)
        {
            return pObject.IsNull || pObject.Ref.InLimbo; // || !pObject.Ref.IsVisible;
        }

        public static bool IsCloaked(this Pointer<TechnoClass> pTechno, bool includeCloaking = true)
        {
            return pTechno.IsNull || pTechno.Ref.CloakStates == CloakStates.Cloaked || !includeCloaking || pTechno.Ref.CloakStates == CloakStates.Cloaking;
        }

        public static bool IsDeadOrInvisible(this Pointer<TechnoClass> pTarget)
        {
            return pTarget.IsDead() || pTarget.IsInvisible();
        }

        public static bool IsDeadOrInvisible(this Pointer<BulletClass> pBullet)
        {
            Pointer<ObjectClass> pObject = pBullet.Convert<ObjectClass>();
            return pObject.IsDead() || pObject.IsInvisible();
        }

        public static bool IsDeadOrInvisibleOrCloaked(this Pointer<TechnoClass> pTechno)
        {
            return pTechno.IsDeadOrInvisible() || pTechno.IsCloaked();
        }

        public static bool InAir(this Pointer<TechnoClass> pTechno, bool stand)
        {
            if (!pTechno.IsNull)
            {
                if (stand)
                {
                    return pTechno.Ref.Base.GetHeight() > Game.LevelHeight * 2;
                }
                return pTechno.Ref.Base.Base.IsInAir();
            }
            return false;
        }

        public static void SetAnimOwner(this Pointer<AnimClass> pAnim, Pointer<ObjectClass> pObject)
        {
            switch (pObject.Ref.Base.WhatAmI())
            {
                case AbstractType.Building:
                case AbstractType.Infantry:
                case AbstractType.Unit:
                case AbstractType.Aircraft:
                    pAnim.SetAnimOwner(pObject.Convert<TechnoClass>());
                    break;
                case AbstractType.Bullet:
                    pAnim.SetAnimOwner(pObject.Convert<BulletClass>());
                    break;
            }
        }

        public static void SetAnimOwner(this Pointer<AnimClass> pAnim, Pointer<TechnoClass> pTechno)
        {
            pAnim.Ref.Owner = pTechno.Ref.Owner;
        }

        public static void SetAnimOwner(this Pointer<AnimClass> pAnim, Pointer<BulletClass> pBullet)
        {
            if (pBullet.TryGetOwnerHouse(out Pointer<HouseClass> pHouse))
            {
                pAnim.Ref.Owner = pHouse;
            }
        }

        public static void Show(this Pointer<AnimClass> pAnim, Relation visibility)
        {
            AnimExt ext = AnimExt.ExtMap.Find(pAnim);
            if (null != ext)
            {
                ext.UpdateVisibility(visibility);
            }
            else
            {
                pAnim.Ref.Invisible = false;
            }
        }

        public static void Hidden(this Pointer<AnimClass> pAnim)
        {
            pAnim.Ref.Invisible = true;
        }

        public static bool TryGetOwnerHouse(this Pointer<BulletClass> pBullet, out Pointer<HouseClass> pHouse)
        {
            pHouse = IntPtr.Zero;
            Pointer<TechnoClass> pBulletOwner = pBullet.Ref.Owner;
            BulletExt ext = BulletExt.ExtMap.Find(pBullet);
            if (null != ext && !ext.pSourceHouse.IsNull)
            {
                pHouse = ext.pSourceHouse;
            }
            else
            {
                if (!pBulletOwner.IsNull && !pBulletOwner.Ref.Owner.IsNull)
                {
                    pHouse = pBulletOwner.Ref.Owner;
                }
            }
            return !pHouse.IsNull;
        }


        public static double GetROFMult(this Pointer<TechnoClass> pTechno)
        {
            bool rofAbility = false;
            if (pTechno.Ref.Veterancy.IsElite())
            {
                rofAbility = pTechno.Ref.Type.Ref.VeteranAbilities.ROF || pTechno.Ref.Type.Ref.EliteAbilities.ROF;
            }
            else if (pTechno.Ref.Veterancy.IsVeteran())
            {
                rofAbility = pTechno.Ref.Type.Ref.VeteranAbilities.ROF;
            }
            return !rofAbility ? 1.0 : RulesClass.Global().VeteranROF * ((pTechno.Ref.Owner.IsNull || pTechno.Ref.Owner.Ref.Type.IsNull) ? 1.0 : pTechno.Ref.Owner.Ref.Type.Ref.ROFMult);
        }

        public static double GetDamageMult(this Pointer<TechnoClass> pTechno)
        {
            if (pTechno.IsNull || !pTechno.Ref.Base.IsAlive)
            {
                return 1;
            }
            bool firepower = false;
            if (pTechno.Ref.Veterancy.IsElite())
            {
                firepower = pTechno.Ref.Type.Ref.VeteranAbilities.FIREPOWER || pTechno.Ref.Type.Ref.EliteAbilities.FIREPOWER;
            }
            else if (pTechno.Ref.Veterancy.IsVeteran())
            {
                firepower = pTechno.Ref.Type.Ref.VeteranAbilities.FIREPOWER;
            }
            return (!firepower ? 1.0 : RulesClass.Global().VeteranCombat) * pTechno.Ref.FirepowerMultiplier * ((pTechno.Ref.Owner.IsNull || pTechno.Ref.Owner.Ref.Type.IsNull) ? 1.0 : pTechno.Ref.Owner.Ref.Type.Ref.FirepowerMult);
        }


        public static int GetRealDamage(this Pointer<TechnoClass> pTechno, int damage, Pointer<WarheadTypeClass> pWH, bool ignoreArmor = true, int distance = 0)
        {
            int realDamage = damage;
            if (!ignoreArmor)
            {
                // 计算实际伤害
                if (realDamage > 0)
                {
                    realDamage = MapClass.GetTotalDamage(damage, pWH, pTechno.Ref.Base.Type.Ref.Armor, distance);
                }
                else
                {
                    realDamage = -MapClass.GetTotalDamage(-damage, pWH, pTechno.Ref.Base.Type.Ref.Armor, distance);
                }
            }
            return realDamage;
        }


        public static Pointer<AnimClass> PlayWarheadAnim(this Pointer<WarheadTypeClass> pWH, CoordStruct location, int damage = 1, LandType landType = LandType.Clear)
        {
            Pointer<AnimClass> pAnim = IntPtr.Zero;
            if (MapClass.Instance.TryGetCellAt(location, out Pointer<CellClass> pCell))
            {
                landType = pCell.Ref.LandType;
            }
            Pointer<AnimTypeClass> pAnimType = MapClass.SelectDamageAnimation(damage, pWH, landType, location);
            if (!pAnimType.IsNull)
            {
                pAnim = YRMemory.Create<AnimClass>(pAnimType, location);
            }
            return pAnim;
        }


    }

}