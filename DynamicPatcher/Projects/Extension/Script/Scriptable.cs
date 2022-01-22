using Extension.Ext;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Script
{
    public interface IAbstractScriptable
    {
        public void OnInit();
        public void OnUpdate();
        public void OnUnInit();
    }
    public interface IObjectScriptable : IAbstractScriptable
    {
        public void OnPut(Pointer<CoordStruct> pCoord, Direction faceDir);
        public void OnRemove();
        public void OnReceiveDamage(Pointer<int> pDamage, int DistanceFromEpicenter, Pointer<WarheadTypeClass> pWH,
            Pointer<ObjectClass> pAttacker, bool IgnoreDefenses, bool PreventPassengerEscape, Pointer<HouseClass> pAttackingHouse);
        public void OnDestroy();
    }
    public interface ITechnoScriptable : IObjectScriptable
    {
        public void OnTemporalUpdate(Pointer<TemporalClass> pTemporal);
        public void CanFire(Pointer<AbstractClass> pTarget, Pointer<WeaponTypeClass> pWeapon, ref bool ceaseFire);
        public void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex);

        public void OnSelect(ref bool selectable);
        public void OnGuardCommand();
        public void OnStopCommand();
    }

    public interface IScriptable : IReloadable
    {
    }

    [Serializable]
    public abstract class Scriptable<T> : IScriptable
    {
        public T Owner { get; protected set; }
        public Scriptable(T owner)
        {
            Owner = owner;
        }
        public virtual void SaveToStream(IStream stream) { }
        public virtual void LoadFromStream(IStream stream) { }
    }

    [Serializable]
    public class TechnoScriptable : Scriptable<TechnoExt>, ITechnoScriptable
    {
        public TechnoScriptable(TechnoExt owner) : base(owner) { }

        public virtual void OnInit() { }
        public virtual void OnUpdate() { }
        public virtual void OnUnInit() { }

        public virtual void OnTemporalUpdate(Pointer<TemporalClass> pTemporal) { }

        public virtual void OnPut(Pointer<CoordStruct> pCoord, Direction faceDir) { }
        public virtual void OnRemove() { }

        public virtual void OnReceiveDamage(Pointer<int> pDamage, int DistanceFromEpicenter, Pointer<WarheadTypeClass> pWH,
            Pointer<ObjectClass> pAttacker, bool IgnoreDefenses, bool PreventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        { }
        public virtual void OnDestroy() { }

        public virtual void CanFire(Pointer<AbstractClass> pTarget, Pointer<WeaponTypeClass> pWeapon, ref bool ceaseFire) { }
        public virtual void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex) { }

        public virtual void OnSelect(ref bool selectable) { }
        public virtual void OnGuardCommand() { }
        public virtual void OnStopCommand() { }
    }

    [Serializable]
    public class BulletScriptable : Scriptable<BulletExt>, IObjectScriptable
    {
        public BulletScriptable(BulletExt owner) : base(owner) { }

        public virtual void OnInit() { }
        public virtual void OnUpdate() { }
        public virtual void OnUnInit() { }

        public virtual void OnDetonate(CoordStruct location) { }

        public virtual void OnPut(Pointer<CoordStruct> pCoord, Direction faceDir) { }
        public virtual void OnRemove() { }

        public virtual void OnReceiveDamage(Pointer<int> pDamage, int DistanceFromEpicenter, Pointer<WarheadTypeClass> pWH,
            Pointer<ObjectClass> pAttacker, bool IgnoreDefenses, bool PreventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        { }
        public virtual void OnDestroy() { }
    }
}
