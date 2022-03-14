using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 1824)]
    public struct BuildingClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0xA8EB40);
        public static ref DynamicVectorClass<Pointer<BuildingClass>> Array { get => ref DynamicVectorClass<Pointer<BuildingClass>>.GetDynamicVector(ArrayPointer); }

        [FieldOffset(0)] public TechnoClass Base;
        [FieldOffset(0)] public RadioClass BaseRadio;
        [FieldOffset(0)] public MissionClass BaseMission;
        [FieldOffset(0)] public ObjectClass BaseObject;
        [FieldOffset(0)] public AbstractClass BaseAbstract;

        [FieldOffset(1312)] public Pointer<BuildingTypeClass> Type;

        [FieldOffset(1316)] public Pointer<FactoryClass> Factory;

        [FieldOffset(1632)] public Bool HasPower;

        [FieldOffset(1633)] public Bool IsOverpowered;

        [FieldOffset(1757)] public Bool unknown_bool_6DD;
    }
}
