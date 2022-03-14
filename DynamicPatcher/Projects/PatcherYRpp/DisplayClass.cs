using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PatcherYRpp.FileFormats;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 4584)]
    public struct DisplayClass
    {

        //Decides which mouse pointer to set and then does it.
        //Mouse is over cell pMapCoords which is bShrouded and holds pObject.

        public unsafe Bool ConvertAction(Pointer<CellStruct> cell, bool bShrouded, Pointer<ObjectClass> pObject, Action action, int dwUnk)
        {
            var func = (delegate* unmanaged[Thiscall]<ref DisplayClass, IntPtr, Bool, IntPtr, Action, int, Bool>)
                this.GetVirtualFunctionPointer(46);
            return func(ref this, cell, bShrouded, pObject, action, dwUnk);
        }

        public unsafe void LeftMouseButtonDown(Pointer<Point2D> point)
        {
            var func = (delegate* unmanaged[Thiscall]<ref DisplayClass, IntPtr, void>)
                this.GetVirtualFunctionPointer(47);
            func(ref this, point);
        }

        public unsafe void LeftMouseButtonUp(Pointer<CoordStruct> pCoords, Pointer<CellStruct> pCell, Pointer<ObjectClass> pObject, Action action, int dwUnk = 0)
        {
            var func = (delegate* unmanaged[Thiscall]<ref DisplayClass, IntPtr, IntPtr, IntPtr, Action, int, void>)
                this.GetVirtualFunctionPointer(48);
            func(ref this, pCoords, pCell, pObject, action, dwUnk);
        }

        public unsafe void RightMouseButtonUp(int dwUnk)
        {
            var func = (delegate* unmanaged[Thiscall]<ref DisplayClass, int, void>)
                this.GetVirtualFunctionPointer(49);
            func(ref this, dwUnk);
        }

        public unsafe void LMBUp(Pointer<CoordStruct> xyz, Pointer<CellStruct> pMapCoords, Pointer<ObjectClass> pObject, Action action, int dwUnk2 = 0)
        {
            var func = (delegate* unmanaged[Thiscall]<ref DisplayClass, IntPtr, IntPtr, IntPtr, Action, int, void>)0x4AB9B0;
            func(ref this, xyz, pMapCoords, pObject, action, dwUnk2);
        }

        public unsafe void RMBUp(int dwUnk2)
        {
            var func = (delegate* unmanaged[Thiscall]<ref DisplayClass, int, void>)0x4AAD30;
            func(ref this, dwUnk2);
        }

        public unsafe Action DecideAction(Pointer<CellStruct> pMapCoords, Pointer<ObjectClass> pObject, int dwUnk)
        {
            var func = (delegate* unmanaged[Thiscall]<ref DisplayClass, IntPtr, IntPtr, int, Action>)0x692610;
            return func(ref this, pMapCoords, pObject, dwUnk);
        }

    }
}
