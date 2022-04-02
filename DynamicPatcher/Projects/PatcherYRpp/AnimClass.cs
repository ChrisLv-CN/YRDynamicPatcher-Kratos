using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 456)]
    public struct AnimClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0xA8E9A8);

        public static ref DynamicVectorClass<Pointer<AnimClass>> Array { get => ref DynamicVectorClass<Pointer<AnimClass>>.GetDynamicVector(ArrayPointer); }


        public unsafe void SetOwnerObject(Pointer<ObjectClass> pOwner)
        {
            var func = (delegate* unmanaged[Thiscall]<ref AnimClass, IntPtr, void>)0x424B50;
            func(ref this, pOwner);
        }

        public void Pause()
        {
            this.Paused = true;
            this.Unpaused = false;
            this.PausedAnimFrame = this.Animation.Value;
        }

        public void Unpause()
        {
            this.Paused = false;
            this.Unpaused = true;
        }

        public static void Constructor(Pointer<AnimClass> pThis, Pointer<AnimTypeClass> pAnimType, CoordStruct Location)
        {
            Constructor(pThis, pAnimType, Location, 0);
        }
        public static unsafe void Constructor(Pointer<AnimClass> pThis, Pointer<AnimTypeClass> pAnimType, CoordStruct Location, int LoopDelay = 0,
            int LoopCount = 1, uint flags = 0x600, int ForceZAdjust = 0, bool reverse = false)
        {
            var func = (delegate* unmanaged[Thiscall]<ref AnimClass, IntPtr, ref CoordStruct, int,
                int, uint, int, Bool, void>)0x421EA0;
            func(ref pThis.Ref, pAnimType, ref Location, LoopDelay, LoopCount, flags, ForceZAdjust, reverse);
        }

        [FieldOffset(0)] public ObjectClass Base;

        [FieldOffset(172)] public ProgressTimer Animation;
        [FieldOffset(200)] public Pointer<AnimTypeClass> Type;
        [FieldOffset(204)] public Pointer<ObjectClass> OwnerObject; // set by AnimClass::SetOwnerObject (0x424B50)

        //[FieldOffset(212)] public Pointer<LightConvertClass> LightConvert;     //Palette?
        [FieldOffset(216)] public int LightConvertIndex; // assert( (*ColorScheme::Array)[this->LightConvertIndex] == this->LightConvert ;
        [FieldOffset(220)] public byte PaletteName_first; // filename set for destroy anims
        public AnsiStringPointer PaletteName => Pointer<byte>.AsPointer(ref PaletteName_first);
        [FieldOffset(252)] public int TintColor;
        [FieldOffset(256)] public int ZAdjust;
        [FieldOffset(260)] public int YSortAdjust; // same as YSortAdjust from Type
        [FieldOffset(264)] public CoordStruct FlamingGuyCoords; // the destination the anim tries to reach
        [FieldOffset(276)] public int FlamingGuyRetries; // number of failed attemts to reach water. the random destination generator stops if >= 7
        [FieldOffset(280)] public Bool IsBuildingAnim; // whether this anim will invalidate on buildings, and whether it's tintable
        [FieldOffset(281)] public Bool UnderTemporal; // temporal'd building's active anims
        [FieldOffset(282)] public Bool Paused; // if paused, does not advance anim, does not deliver damage
        [FieldOffset(283)] public Bool Unpaused; // set when unpaused
        [FieldOffset(284)] public int PausedAnimFrame; // the animation value when paused
        [FieldOffset(288)] public Bool Reverse; // anim is forced to be played from end to start

        [FieldOffset(296)] public BounceClass Bounce;
        [FieldOffset(376)] public byte TranslucencyLevel; // on a scale of 1 - 100
        [FieldOffset(377)] public Bool TimeToDie; // or something to that effect, set just before UnInit
        [FieldOffset(380)] private IntPtr attachedBullet;
        public Pointer<BulletClass> AttachedBullet { get => attachedBullet; set => attachedBullet = value; }

        [FieldOffset(384)] public Pointer<HouseClass> Owner; //Used for remap (AltPalette).
        [FieldOffset(388)] public int LoopDelay; // randomized value, depending on RandomLoopDelay
        [FieldOffset(392)] public double Damage; // defaults to 1.0 , added to Type->Damage in some cases
        [FieldOffset(400)] public BlitterFlags AnimFlags; // argument that's 0x600 most of the time
        [FieldOffset(404)] public Bool HasExtras; // enables IsMeteor and Bouncer special behavior (AnimExtras)
        [FieldOffset(405)] public int RemainingIterations; // defaulted to deleteAfterIterations, when reaches zero, UnInit() is called
        [FieldOffset(406)] public byte unknown_196;
        [FieldOffset(407)] public byte unknown_197;
        [FieldOffset(408)] public Bool IsPlaying;
        [FieldOffset(409)] public Bool IsFogged;
        [FieldOffset(410)] public Bool FlamingGuyExpire; // finish animation and remove
        [FieldOffset(411)] public Bool UnableToContinue; // set when something prevents the anim from going on: cell occupied, veins destoyed or unit gone, ...
        [FieldOffset(412)] public Bool SkipProcessOnce; // set in constructor, cleared during Update. skips damage, veins, tiberium chain reaction and animation progress
        [FieldOffset(413)] public Bool Invisible; // don't draw, but Update state anyway
        [FieldOffset(414)] public Bool PowerOff; // powered animation has no power

        //[FieldOffset(416)] public AudioController Audio3;
        //[FieldOffset(436)] public AudioController Audio4;

    }
}
