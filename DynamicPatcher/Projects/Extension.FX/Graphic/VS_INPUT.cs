using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Graphic
{
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct VS_INPUT
    {
        public readonly Vector3 Position;
        public readonly Vector2 UV;
        public VS_INPUT(Vector3 position, Vector2 uv)
        {
            Position = position;
            UV = uv;
        }
    }
}
