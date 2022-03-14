using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 1752)]
    public struct AircraftClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0xA8E390);
        public static ref DynamicVectorClass<Pointer<AircraftClass>> Array { get => ref DynamicVectorClass<Pointer<AircraftClass>>.GetDynamicVector(ArrayPointer); }

        [FieldOffset(0)] public FootClass Base;
        [FieldOffset(0)] public TechnoClass BaseTechno;
        [FieldOffset(0)] public RadioClass BaseRadio;
        [FieldOffset(0)] public MissionClass BaseMission;
        [FieldOffset(0)] public ObjectClass BaseObject;
        [FieldOffset(0)] public AbstractClass BaseAbstract;

        [FieldOffset(1732)] public Pointer<AircraftTypeClass> Type;

        [FieldOffset(1737)] public Bool HasPassengers;

        [FieldOffset(1738)] public Bool IsCrashing;

    }
}
