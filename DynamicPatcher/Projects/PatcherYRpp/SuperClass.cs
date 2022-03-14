using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 128)]
    public struct SuperClass
    {
        public unsafe void Launch(CellStruct cell, bool isPlayer)
        {
            var func = (delegate* unmanaged[Thiscall]<ref SuperClass, ref CellStruct, Bool, void>)0x6CC390;
            func(ref this, ref cell, isPlayer);
        }

        public unsafe void Reset()
        {
            var func = (delegate* unmanaged[Thiscall]<ref SuperClass, void>)0x6CE0B0;
            func(ref this);
        }

        public unsafe bool CanFire()
        {
            var func = (delegate* unmanaged[Thiscall]<ref SuperClass, Bool>)0x6CC360;
            return func(ref this);
        }

        public unsafe bool ClickFire(bool isPlayer, CoordStruct coords)
        {
            var func = (delegate* unmanaged[Thiscall]<ref SuperClass, Bool, ref CoordStruct, Bool>)0x6CB920;
            return func(ref this, isPlayer, ref coords);
        }

        [FieldOffset(0)] public AbstractClass Base;

        [FieldOffset(36)] public int CustomChargeTime;
        [FieldOffset(40)] public Pointer<SuperWeaponTypeClass> Type;
        [FieldOffset(44)] public IntPtr owner;
        public Pointer<HouseClass> Owner { get => owner; set => owner = value; }

        [FieldOffset(48)] public TimerStruct RechargeTimer;

        [FieldOffset(64)] public Bool BlinkState;

        [FieldOffset(72)] public long BlinkTimer;
        [FieldOffset(80)] public int SpecialSoundDuration; // see 0x6CD14F
        [FieldOffset(84)] public CoordStruct SpecialSoundLocation;
        [FieldOffset(96)] public Bool CanHold;          // 0x60

        [FieldOffset(98)] public CellStruct ChronoMapCoords;  // 0x62

        [FieldOffset(104)] private IntPtr animation;                // 0x68
        public Pointer<AnimClass> Animation { get => animation; set => animation = value; }
        [FieldOffset(108)] public Bool AnimationGotInvalid;
        [FieldOffset(109)] public Bool Granted;
        [FieldOffset(110)] public Bool OneTime; // remove this SW when it has been fired once
        [FieldOffset(111)] public Bool IsCharged;
        [FieldOffset(112)] public Bool IsOnHold;

        [FieldOffset(116)] public int ReadinessFrame; // when did it become ready?
        [FieldOffset(120)] public int CameoChargeState;
        [FieldOffset(124)] public ChargeDrainState ChargeDrainState;
    }
}
