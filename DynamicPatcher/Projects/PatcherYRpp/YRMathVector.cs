using System.Numerics;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{

    [StructLayout(LayoutKind.Sequential, Size = 4)]
    [Serializable]
    public struct Vector2D<T>
    {
        public Vector2D(T x, T y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return string.Format("{{\"X\":{0}, \"Y\":{1}}}", X, Y);
        }

        public T X;
        public T Y;
    }

    [StructLayout(LayoutKind.Sequential, Size = 12)]
    [Serializable]
    public struct Vector3D<T>
    {
        public Vector3D(T x, T y, T z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return string.Format("{{\"X\":{0}, \"Y\":{1}, \"Z\":{2}}}", X, Y, Z);
        }

        public T X;
        public T Y;
        public T Z;

    }

    [StructLayout(LayoutKind.Sequential, Size = 48)]
    [Serializable]
    public struct Matrix3DStruct
    {

        public Matrix3DStruct(bool identity = false)
        {
            if (identity)
            {
                this.MakeIdentity();
            }
        }

        public unsafe void MakeIdentity()
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, void>)0x5AE860;
            func(ref this);
        }

        public unsafe void Translate(float x, float y, float z)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, float, float, void>)0x5AE890;
            func(ref this, x, y, z);
        }

        public unsafe void Translate(SingleVector3D vector3D)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, ref SingleVector3D, void>)0x5AE8F0;
            func(ref this, ref vector3D);
        }

        public unsafe void TranslateX(float x)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, void>)0x5AE980;
            func(ref this, x);
        }

        public unsafe void TranslateY(float y)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, void>)0x5AE9B0;
            func(ref this, y);
        }

        public unsafe void TranslateZ(float z)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, void>)0x5AE9E0;
            func(ref this, z);
        }

        public unsafe void Scale(float factor)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, void>)0x5AEA10;
            func(ref this, factor);
        }

        public unsafe void Scale(float x, float y, float z)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, float, float, void>)0x5AEA70;
            func(ref this, z, y, z);
        }

        public unsafe void ScaleX(float x)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, void>)0x5AEAD0;
            func(ref this, x);
        }

        public unsafe void ScaleY(float y)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, void>)0x5AEAF0;
            func(ref this, y);
        }

        public unsafe void ScaleZ(float z)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, void>)0x5AEB20;
            func(ref this, z);
        }

        public unsafe void ShearYZ(float y, float z)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, float, void>)0x5AEB50;
            func(ref this, y, z);
        }

        public unsafe void ShearXY(float x, float y)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, float, void>)0x5AEBA0;
            func(ref this, x, y);
        }

        public unsafe void ShearXZ(float x, float z)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, float, void>)0x5AEBF0;
            func(ref this, x, z);
        }

        public unsafe void PreRotateX(float theta)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, void>)0x5AEC40;
            func(ref this, theta);
        }
        public unsafe void PreRotateY(float theta)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, void>)0x5AED50;
            func(ref this, theta);
        }
        public unsafe void PreRotateZ(float theta)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, void>)0x5AEE50;
            func(ref this, theta);
        }

        public unsafe void RotateX(float theta)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, void>)0x5AEF60;
            func(ref this, theta);
        }

        public unsafe void RotateX(float sin, float cos)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, float, void>)0x5AF000;
            func(ref this, sin, cos);
        }

        public unsafe void RotateY(float theta)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, void>)0x5AF080;
            func(ref this, theta);
        }

        public unsafe void RotateY(float sin, float cos)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, float, void>)0x5AF120;
            func(ref this, sin, cos);
        }

        public unsafe void RotateZ(float theta)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, void>)0x5AF1A0;
            func(ref this, theta);
        }

        public unsafe void RotateZ(float sin, float cos)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float, float, void>)0x5AF240;
            func(ref this, sin, cos);
        }

        public unsafe float GetXVal()
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float>)0x5AF2C0;
            return func(ref this);
        }

        public unsafe float GetYVal()
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float>)0x5AF310;
            return func(ref this);
        }

        public unsafe float GetZVal()
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float>)0x5AF360;
            return func(ref this);
        }

        public unsafe float GetXRotation()
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float>)0x5AF3B0;
            return func(ref this);
        }

        public unsafe float GetYRotation()
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float>)0x5AF410;
            return func(ref this);
        }

        public unsafe float GetZRotation()
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, float>)0x5AF470;
            return func(ref this);
        }

        public unsafe Pointer<SingleVector3D> RotateVector(Pointer<SingleVector3D> ret, Pointer<SingleVector3D> rotate)
        {
            var func = (delegate* unmanaged[Thiscall]<ref Matrix3DStruct, IntPtr, IntPtr, IntPtr>)0x5AF4D0;
            return func(ref this, ret, rotate);
        }

        public unsafe SingleVector3D RotateVector(ref SingleVector3D rotate)
        {
            SingleVector3D buffer = default;
            RotateVector(Pointer<SingleVector3D>.AsPointer(ref buffer), Pointer<SingleVector3D>.AsPointer(ref rotate));
            return buffer;
        }
    }
}
