using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 2280)]
    [Serializable]
    public struct UnitClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0x8B4108);
        public static ref DynamicVectorClass<Pointer<UnitClass>> Array { get => ref DynamicVectorClass<Pointer<UnitClass>>.GetDynamicVector(ArrayPointer); }


        public unsafe void DrawAsVXL(Point2D coords, RectangleStruct boundingRect, int dwUnk7, int dwUnk8)
        {
            var func = (delegate* unmanaged[Thiscall]<ref UnitClass, Point2D, RectangleStruct, int, int, void>)0x73B470;
            func(ref this, coords, boundingRect, dwUnk7, dwUnk8);
        }

        public unsafe void DrawAsSHP(Point2D coords, RectangleStruct boundingRect, int dwUnk7, int dwUnk8)
        {
            var func = (delegate* unmanaged[Thiscall]<ref UnitClass, Point2D, RectangleStruct, int, int, void>)0x73C5F0;
            func(ref this, coords, boundingRect, dwUnk7, dwUnk8);
        }

        [FieldOffset(0)] public FootClass Base;
        [FieldOffset(1732)] public Pointer<UnitTypeClass> Type;
        [FieldOffset(1736)] public IntPtr FollowerCar;
        [FieldOffset(1744)] public Bool HasFollowerCar;
        [FieldOffset(1745)] public Bool Unloading;
        [FieldOffset(1752)] public int DeathFrameCounter;
        [FieldOffset(1760)] public Bool Deployed;
        [FieldOffset(1761)] public Bool Deploying;
        [FieldOffset(1762)] public Bool Undeploying;

    }
}
