using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PatcherYRpp.FileFormats;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 172)]
    public struct ObjectClass
    {

        static public readonly IntPtr CurrentObjectsPointer = new IntPtr(0xA8ECB8);
        static public ref DynamicVectorClass<Pointer<ObjectClass>> CurrentObjects { get => ref DynamicVectorClass<Pointer<ObjectClass>>.GetDynamicVector(CurrentObjectsPointer); }

        static public readonly IntPtr ArrayPointer = new IntPtr(0xA8E360);
        static public ref DynamicVectorClass<Pointer<ObjectClass>> Array { get => ref DynamicVectorClass<Pointer<ObjectClass>>.GetDynamicVector(ArrayPointer); }

        static public readonly IntPtr ObjectsInLayersPointer = new IntPtr(0x8A0360);

        public unsafe Pointer<TechnoTypeClass> TechnoType
        {
            get
            {
                var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, IntPtr>)
                    Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 33);
                return func(ref this);
            }
        }

        public unsafe Pointer<ObjectTypeClass> Type
        {
            get
            {
                var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, IntPtr>)
                    Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 34);
                return func(ref this);
            }
        }

        public unsafe bool IsSelectable()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, Bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 25);
            return func(ref this);
        }

        public unsafe Pointer<SHPStruct> GetImage()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, IntPtr>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 27);
            return func(ref this);
        }

        public unsafe Action MouseOverCell(CellStruct cell, bool checkFog = false, bool ignoreForce = false)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, CellStruct, Bool, Bool, Action>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 28);
            return func(ref this, cell, checkFog, ignoreForce);
        }

        public unsafe Action MouseOverObject(Pointer<ObjectClass> pObj, bool ignoreForce = false)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, IntPtr, Bool, Action>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 29);
            return func(ref this, pObj, ignoreForce);
        }

        public unsafe Layer InWhichLayer()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, Layer>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 30);
            return func(ref this);
        }

        public unsafe Layer IsSurfaced()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, Layer>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 31);
            return func(ref this);
        }

        public unsafe Layer IsStrange()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, Layer>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 32);
            return func(ref this);
        }

        public unsafe bool IsActive()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 39);
            return func(ref this);
        }

        public unsafe Pointer<CoordStruct> GetDockCoords(ref CoordStruct pCrd, int unknown)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, ref CoordStruct, int, IntPtr>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 42);
            return func(ref this, ref pCrd, unknown);
        }

        public unsafe Pointer<CoordStruct> GetFLH(ref CoordStruct dest, int idxWeapon, CoordStruct baseCoords)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, ref CoordStruct, int, CoordStruct, IntPtr>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 44);
            return func(ref this, ref dest, idxWeapon, baseCoords);
        }

        public unsafe bool IsDisguised()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, Bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 49);
            return func(ref this);
        }

        public unsafe bool IsDisguisedAs(Pointer<HouseClass> target)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, IntPtr, Bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 50);
            return func(ref this, target);
        }

        public unsafe Pointer<ObjectTypeClass> GetDisguise(bool disguisedAgainstAllies)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, Bool, IntPtr>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 51);
            return func(ref this, disguisedAgainstAllies);
        }

        public unsafe Pointer<HouseClass> GetDisguiseHouse(bool disguisedAgainstAllies)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, Bool, IntPtr>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 52);
            return func(ref this, disguisedAgainstAllies);
        }

        public unsafe bool Remove()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, Bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 53);
            return func(ref this);
        }

        public unsafe bool Put(CoordStruct where, Direction faceDir)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, ref CoordStruct, Direction, Bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 54);
            return func(ref this, ref where, faceDir);
        }

        // cleanup things (lose line trail, deselect, etc). Permanently: destroyed/removed/gone opposed to just going out of sight.
        public unsafe void Disappear(bool permanently)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, bool, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 55);
            func(ref this, permanently);
        }

        public unsafe bool SpawnParachuted(CoordStruct where)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, ref CoordStruct, bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 58);
            return func(ref this, ref where);
        }

        public unsafe bool UnInit()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 62);
            return func(ref this);
        }

        public unsafe int KickOutUnit(Pointer<TechnoClass> pTechno, CellStruct cell)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, IntPtr, CellStruct, int>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 64);
            return func(ref this, pTechno, cell);
        }

        public unsafe void DrawIfVisible(Pointer<RectangleStruct> visibleArea, Bool evenIfClocked, int dwUnk3)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, IntPtr, Bool, int, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 65);
            func(ref this, visibleArea, evenIfClocked, dwUnk3);
        }

        public unsafe bool UpdatePlacement(PlacementType type)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, PlacementType, bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 73);
            return func(ref this, type);
        }

        public unsafe void MarkForRedraw()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 77);
            func(ref this);
        }

        public unsafe bool CanBeSelected()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 78);
            return func(ref this);
        }

        public unsafe bool CanBeSelectedNow()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 79);
            return func(ref this);
        }

        public unsafe void Select()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 83);
            func(ref this);
        }

        public unsafe void Deselect()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 84);
            func(ref this);
        }

        public unsafe void IronCurtain(int duration, Pointer<HouseClass> pHouse, bool forceShield)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, int, IntPtr, Bool, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 85);
            func(ref this, duration, pHouse, forceShield);
        }

        public unsafe void StopAirstrikeTimer()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 86);
            func(ref this);
        }

        public unsafe void StartAirstrikeTimer(int duration)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, int, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 87);
            func(ref this, duration);
        }

        public unsafe bool IsIronCurtained()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, bool>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 88);
            return func(ref this);
        }

        public unsafe int GetWeaponRange(int idxWeapon)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, int, int>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 90);
            return func(ref this, idxWeapon);
        }

        public unsafe DamageState TakeDamage(int damage, bool crewed)
        {
            return TakeDamage(damage, RulesClass.Global().C4Warhead, crewed);
        }

        public unsafe DamageState TakeDamage(int damage, Pointer<WarheadTypeClass> pWH, bool crewed)
        {
            return TakeDamage(damage, pWH, IntPtr.Zero, IntPtr.Zero, crewed);
        }

        public unsafe DamageState TakeDamage(int damage, Pointer<WarheadTypeClass> pWH, Pointer<ObjectClass> pAttacker, Pointer<HouseClass> pAttackingHouse, bool crewed)
        {
            return ReceiveDamage(damage, 0, pWH, pAttacker, true, !crewed, pAttackingHouse);
        }

        public unsafe DamageState ReceiveDamage(int damage, int distanceFromEpicenter, Pointer<WarheadTypeClass> pWH,
            Pointer<ObjectClass> pAttacker, bool ignoreDefenses, bool preventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, IntPtr, int, IntPtr, IntPtr, bool, bool, IntPtr, DamageState>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 91);
            return func(ref this, Pointer<int>.AsPointer(ref damage), distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse);
        }

        public unsafe void Destroy()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 92);
            func(ref this);
        }

        public unsafe Mission GetCurrentMission()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, Mission>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 97);
            return func(ref this);
        }

        public unsafe void RestoreMission(Mission mission)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, Mission, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 98);
            func(ref this, mission);
        }

        public unsafe void SetLocation(CoordStruct where)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, ref CoordStruct, void>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 109);
            func(ref this, ref where);
        }

        public unsafe int GetHeight()
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, int>)
                Helpers.GetVirtualFunctionPointer(Pointer<ObjectClass>.AsPointer(ref this), 114);
            return func(ref this);
        }

        public double GetHealthPercentage()
        {
            return Math.Round((double)Health / Type.Ref.Strength, 2);
        }

        public unsafe int Distance(Pointer<ObjectClass> that)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, IntPtr, int>)0x5F6360;
            return func(ref this, that);
        }

        public unsafe int DistanceFrom(Pointer<ObjectClass> that)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ObjectClass, IntPtr, int>)0x5F6440;
            return func(ref this, that);
        }

        public CoordStruct GetFLH(int idxWeapon, CoordStruct baseCoords)
        {
            CoordStruct pos = new CoordStruct();
            GetFLH(ref pos, idxWeapon, baseCoords);
            return pos;
        }


        [FieldOffset(0)]
        public AbstractClass Base;

        [FieldOffset(44)] public int FallRate; //how fast is it falling down? only works if FallingDown is set below, and actually positive numbers will move the thing UPWARDS
        [FieldOffset(48)] private IntPtr nextObject;    //Next Object in the same cell or transport. This is a linked list of Objects.
        public Pointer<ObjectClass> NextObject { get => nextObject; set => nextObject = value; }

        [FieldOffset(100)] public int CustomSound;
        [FieldOffset(104)] public Bool BombVisible; // In range of player's bomb seeing units, so should draw it

        [FieldOffset(108)] public int Health;     //The current Health.
        [FieldOffset(112)] public int EstimatedHealth; // used for auto-targeting threat estimation
        [FieldOffset(116)] public Bool IsOnMap; // has this object been placed on the map?

        [FieldOffset(128)] public Bool NeedsRedraw;
        [FieldOffset(129)] public Bool InLimbo; // act as if it doesn't exist - e.g., post mortem state before being deleted
        [FieldOffset(130)] public Bool InOpenToppedTransport;
        [FieldOffset(131)] public Bool IsSelected;    //Has the player selected this Object?
        [FieldOffset(132)] public Bool HasParachute;  //Is this Object parachuting?

        [FieldOffset(136)] private IntPtr parachute;       //Current parachute Anim.
        public Pointer<AnimClass> Parachute { get => parachute; set => parachute = value; }
        [FieldOffset(140)] public Bool OnBridge;
        [FieldOffset(141)] public Bool IsFallingDown;
        [FieldOffset(142)] public Bool WasFallingDown; // last falling state when FootClass::Update executed. used to find out whether it changed.
        [FieldOffset(143)] public Bool IsABomb; // if set, will explode after FallingDown brings it to contact with the ground
        [FieldOffset(144)] public Bool IsAlive;       //Self-explanatory.

        [FieldOffset(148)] public Layer LastLayer;
        [FieldOffset(152)] public Bool IsInLogic; // has this object been added to the logic collection?
        [FieldOffset(153)] public Bool IsVisible; // was this object in viewport when drawn?

        [FieldOffset(156)] public CoordStruct Location; //Absolute current 3D location (in leptons)
    }
}
