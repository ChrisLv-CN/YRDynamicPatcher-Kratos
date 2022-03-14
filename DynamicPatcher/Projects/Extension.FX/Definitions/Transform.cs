using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Definitions
{
    public struct Rotator
    {
        public Rotator(float roll, float pitch, float yaw)
        {
            Roll = roll;
            Pitch = pitch;
            Yaw = yaw;
        }

        public static Rotator Zero = new Rotator();
        public bool IsZero => this == Zero;

        public static Rotator operator -(Rotator a)
        {
            return new Rotator(-a.X, -a.Y, -a.Z);
        }
        public static Rotator operator +(Rotator a, Rotator b)
        {
            return new Rotator(
                 a.X + b.X,
                 a.Y + b.Y,
                 a.Z + b.Z);
        }
        public static Rotator operator -(Rotator a, Rotator b)
        {
            return new Rotator(
                 a.X - b.X,
                 a.Y - b.Y,
                 a.Z - b.Z);
        }
        public static Rotator operator *(Rotator a, double r)
        {
            return new Rotator(
                 (float)(a.X * r),
                 (float)(a.Y * r),
                 (float)(a.Z * r));
        }
        public static Rotator operator /(Rotator a, double r)
        {
            return new Rotator(
                 (float)(a.X / r),
                 (float)(a.Y / r),
                 (float)(a.Z / r));
        }

        public static implicit operator Vector3(Rotator val) => new Vector3(val.X, val.Y, val.Z);
        public static implicit operator Rotator(Vector3 val) => new Rotator(val.X, val.Y, val.Z);

        public static bool operator ==(Rotator a, Rotator b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }
        public static bool operator !=(Rotator a, Rotator b) => !(a == b);

        public override bool Equals(object obj) => this == (Rotator)obj;
        public override int GetHashCode() => base.GetHashCode();

        public float Roll;
        public float Pitch;
        public float Yaw;

        public float X { get => Roll; set => Roll = value; }
        public float Y { get => Pitch; set => Pitch = value; }
        public float Z { get => Yaw; set => Yaw = value; }
    }

    public struct Transform
    {
        public Transform(Vector3 location, Rotator rotation, Vector3 scale)
        {
            Location = location;
            Rotation = rotation;
            Scale = scale;
        }

        public Vector3 Location;
        public Rotator Rotation;
        public Vector3 Scale;

    }
}
