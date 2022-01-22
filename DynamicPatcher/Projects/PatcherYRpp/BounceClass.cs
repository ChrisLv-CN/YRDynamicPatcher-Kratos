using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size=80)]
    [Serializable]
    public struct BounceClass
    {
        public enum Status
        {
            None = 0,
            Bounce = 1,
            Impact = 2
        };

        public BounceClass(CoordStruct coords, double elasticity, double gravity,
            double maxVelocity, SingleVector3D velocity, double angularVelocity) : this()
        {
            this.Initialize(coords, elasticity, gravity, maxVelocity, velocity, angularVelocity);
        }

        public unsafe void Initialize(CoordStruct coords, double elasticity, double gravity,
            double maxVelocity, SingleVector3D velocity, double angularVelocity)
        {
            var func = (delegate* unmanaged[Thiscall]<ref BounceClass, ref CoordStruct, double, double, double, ref SingleVector3D, double, void>)0x4397E0;
            func(ref this, ref coords, elasticity, gravity, maxVelocity, ref velocity, angularVelocity);
        }

        public unsafe CoordStruct GetCoords()
        {
            var func = (delegate* unmanaged[Thiscall]<ref BounceClass, IntPtr, IntPtr>)0x4399A0;

            CoordStruct ret = default;
            func(ref this, Pointer<CoordStruct>.AsPointer(ref ret));
            return ret;
        }

        //Matrix3DStruct* GetDrawingMatrix(Matrix3DStruct* pBuffer)
        //{
        //    JMP_THIS(0x4399E0);
        //}

        public unsafe Status Update()
        {
            var func = (delegate* unmanaged[Thiscall]<ref BounceClass, Status>)0x439B00;
            return func(ref this);
        }

        [FieldOffset(0)] public double Elasticity; // speed multiplier when bouncing off the ground
        [FieldOffset(8)] public double Gravity; // subtracted from the Z coords every frame
        [FieldOffset(16)] public double MaxVelocity; // 0.0 disables check
        [FieldOffset(24)] public SingleVector3D Coords; // position with precision
        [FieldOffset(36)] public SingleVector3D Velocity; // speed components
        [FieldOffset(48)] public Quaternion CurrentAngle; // quaternion for drawing
        [FieldOffset(64)] public Quaternion AngularVelocity; // second quaternion as per-frame delta
    }
}
