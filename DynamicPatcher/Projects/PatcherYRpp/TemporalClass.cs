using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PatcherYRpp.FileFormats;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 80)]
    public struct TemporalClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0xB0EC60);
        public static ref DynamicVectorClass<Pointer<TemporalClass>> Array { get => ref DynamicVectorClass<Pointer<TemporalClass>>.GetDynamicVector(ArrayPointer); }

        public static unsafe void Constructor(Pointer<TemporalClass> pThis, Pointer<TechnoClass> pOwnerUnit)
        {
            var func = (delegate* unmanaged[Thiscall]<ref TemporalClass, IntPtr, void>)0x71A4E0;
            func(ref pThis.Ref, pOwnerUnit);
        }

        public unsafe bool CanWarpTarget(Pointer<TechnoClass> pTarget)
        { 
            var func = (delegate* unmanaged[Thiscall]<ref TemporalClass, IntPtr, Bool>)0x71AE50;
            return func(ref this, pTarget);
        }

        public unsafe void Detach()
        { 
            var func = (delegate* unmanaged[Thiscall]<ref TemporalClass, void>)0x71ADE0;
            func(ref this);
        }

        public unsafe void Fire(Pointer<TechnoClass> pTarget)
        { 
            var func = (delegate* unmanaged[Thiscall]<ref TemporalClass, IntPtr, void>)0x71AF20;
            func(ref this, pTarget);
        }

        public unsafe int GetWarpPerStep(int helperCount)
        { 
            var func = (delegate* unmanaged[Thiscall]<ref TemporalClass, int, int>)0x71AB10;
            return func(ref this, helperCount);
        }

        public unsafe void JustLetGo()
        { 
            var func = (delegate* unmanaged[Thiscall]<ref TemporalClass, void>)0x71AD40;
            func(ref this);
        }

        public unsafe void LetGo()
        { 
            var func = (delegate* unmanaged[Thiscall]<ref TemporalClass, void>)0x71ABC0;
            func(ref this);
        }

        [FieldOffset(36)] public IntPtr owner;
        public Pointer<TechnoClass> Owner {get => owner; set => this.owner = value; }

        [FieldOffset(40)] public IntPtr target;
        public Pointer<TechnoClass> Target {get => target; set => this.target = value; }

        [FieldOffset(44)] public TimerStruct LifeTimer;

        [FieldOffset(60)] public IntPtr sourceSW;
        public Pointer<SuperClass> SourceSW {get => sourceSW; set => this.sourceSW = value; }

        [FieldOffset(72)] public int WarpRemaining;

        [FieldOffset(76)] public int WarpPerStep;

    }
}
