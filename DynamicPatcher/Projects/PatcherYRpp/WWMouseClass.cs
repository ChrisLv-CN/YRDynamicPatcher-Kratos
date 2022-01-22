using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 152)]
    [Serializable]
    public struct WWMouseClass
    {
        // private static IntPtr instance = new IntPtr(0x887640);
        public static ref WWMouseClass Instance => ref new Pointer<Pointer<WWMouseClass>>(0x887640).Ref.Ref;

        public unsafe int GetX()
        {
            var func = (delegate* unmanaged[Thiscall]<ref WWMouseClass, int>)Helpers.GetVirtualFunctionPointer(Pointer<WWMouseClass>.AsPointer(ref this), 11);
            return func(ref this);
        }

        public unsafe int GetY()
        {
            var func = (delegate* unmanaged[Thiscall]<ref WWMouseClass, int>)Helpers.GetVirtualFunctionPointer(Pointer<WWMouseClass>.AsPointer(ref this), 12);
            return func(ref this);
        }

        public unsafe Point2D GetCoords()
        {
            Point2D pBuffer = default;
            var func = (delegate* unmanaged[Thiscall]<ref WWMouseClass, IntPtr, IntPtr>)Helpers.GetVirtualFunctionPointer(Pointer<WWMouseClass>.AsPointer(ref this), 13);
            func(ref this, Pointer<Point2D>.AsPointer(ref pBuffer));
            return pBuffer;
        }

    }
}
