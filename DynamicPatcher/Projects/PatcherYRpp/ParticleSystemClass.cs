using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 256)]
    [Serializable]
    public struct ParticleSystemClass
    {

        public static unsafe void Constructor(Pointer<ParticleSystemClass> pThis, Pointer<ParticleSystemTypeClass> pType, CoordStruct sourcePos, CoordStruct targetPos)
        {
            Constructor(pThis, pType, sourcePos, Pointer<AbstractClass>.Zero, Pointer<TechnoClass>.Zero, targetPos);
        }

        public static unsafe void Constructor(Pointer<ParticleSystemClass> pThis, Pointer<ParticleSystemTypeClass> pType, CoordStruct sourcePos, CoordStruct targetPos, Pointer<TechnoClass> pOwner)
        {
            Constructor(pThis, pType, sourcePos, Pointer<AbstractClass>.Zero, pOwner, targetPos);
        }

        public static unsafe void Constructor(Pointer<ParticleSystemClass> pThis, Pointer<ParticleSystemTypeClass> pType, CoordStruct sourcePos, Pointer<AbstractClass> pTarget, Pointer<TechnoClass> pOwner)
        {
            Constructor(pThis, pType, sourcePos, pTarget, pOwner, pTarget.IsNull ? sourcePos : pTarget.Ref.GetCoords());
        }

        public static unsafe void Constructor(Pointer<ParticleSystemClass> pThis, Pointer<ParticleSystemTypeClass> pType, CoordStruct sourcePos, Pointer<AbstractClass> pTarget, Pointer<TechnoClass> pOwner, CoordStruct targetPos)
        {
            Constructor(pThis, pType, sourcePos, pTarget, pOwner, targetPos, 0);
        }

        public static unsafe void Constructor(Pointer<ParticleSystemClass> pThis, Pointer<ParticleSystemTypeClass> pType, CoordStruct sourcePos, Pointer<AbstractClass> pTarget, Pointer<TechnoClass> pOwner, CoordStruct targetPos, int unk)
        {
            var func = (delegate* unmanaged[Thiscall]<ref ParticleSystemClass, IntPtr, ref CoordStruct, IntPtr, IntPtr, ref CoordStruct, int, void>)0x62DC50;
            func(ref pThis.Ref, pType, ref sourcePos, pTarget, pOwner, ref targetPos, unk);
        }

        [FieldOffset(0)] public ObjectClass Base;

        [FieldOffset(172)] public Pointer<ParticleSystemTypeClass> Type;

    }
}
