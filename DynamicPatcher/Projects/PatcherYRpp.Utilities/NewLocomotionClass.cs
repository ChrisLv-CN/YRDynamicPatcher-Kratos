using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using IStream = System.Runtime.InteropServices.ComTypes.IStream;

namespace PatcherYRpp.Utilities
{
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public abstract class NewLocomotionClass : IPersistStream, ILocomotion
    {
        public NewLocomotionClass() { }

        // ILocomotion
        public virtual void Acquire_Hunter_Seeker_Target() { }
        public virtual int Apparent_Speed()
        {
            return 0;
        }
        public virtual Move Can_Enter_Cell(CellStruct cell)
        {
            return Move.OK;
        }

        public virtual FireError Can_Fire()
        {
            return FireError.OK;
        }

        IntPtr ILocomotion.Destination(IntPtr pcoord)
        {
            var pCrd = pcoord.Convert<CoordStruct>();
            pCrd.Data = Destination();
            return pcoord;
        }
        public virtual CoordStruct Destination()
        {
            return new CoordStruct();
        }

        public virtual void Do_Turn(DirStruct direction) { }
        public virtual int Drawing_Code()
        {
            return 0;
        }
        public virtual IntPtr Draw_Matrix(IntPtr pMatrix, IntPtr pKey)
        {
            return pMatrix;
        }
        public virtual IntPtr Draw_Point(IntPtr pPoint)
        {
            return pPoint;
        }
        public virtual void Force_Immediate_Destination(CoordStruct coord) { }
        public virtual void Force_New_Slope(int ramp) { }
        public virtual void Force_Track(int track, CoordStruct coord) { }
        public virtual int Get_Speed_Accum()
        {
            return -1;
        }
        public virtual int Get_Status()
        {
            return 0;
        }
        public virtual int Get_Track_Index()
        {
            return -1;
        }
        public virtual int Get_Track_Number()
        {
            return -1;
        }

        IntPtr ILocomotion.Head_To_Coord(IntPtr pcoord)
        {
            var pCrd = pcoord.Convert<CoordStruct>();
            pCrd.Data = Head_To_Coord();
            return pcoord;
        }
        public virtual CoordStruct Head_To_Coord() => new CoordStruct();

        public virtual void ILocomotion_B8() { }
        public virtual Layer In_Which_Layer()
        {
            return Layer.Ground;
        }
        public virtual bool Is_Ion_Sensitive()
        {
            return false;
        }
        public virtual bool Is_Moving()
        {
            return false;
        }
        public virtual bool Is_Moving_Here(CoordStruct to)
        {
            return false;
        }
        public virtual bool Is_Moving_Now()
        {
            return Is_Moving();
        }
        public virtual bool Is_Powered()
        {
            return false;
        }
        public virtual bool Is_Really_Moving_Now()
        {
            return Is_Moving_Now();
        }
        public virtual bool Is_Surfacing()
        {
            return false;
        }
        public virtual bool Is_To_Have_Shadow()
        {
            return false;
        }
        public abstract void Link_To_Object(IntPtr pointer);
        public virtual void Lock() { }
        public virtual void Mark_All_Occupation_Bits(int mark) { }
        public virtual void Move_To(CoordStruct to) { }
        public virtual bool Power_Off()
        {
            return true;
        }
        public virtual bool Power_On()
        {
            return false;
        }
        public virtual bool Process()
        {
            return Is_Moving();
        }
        public virtual bool Push(DirStruct dir)
        {
            return false;
        }
        public virtual IntPtr Shadow_Matrix(IntPtr pMatrix, IntPtr pKey)
        {
            return pMatrix;
        }
        public virtual IntPtr Shadow_Point(IntPtr pPoint)
        {
            return pPoint;
        }
        public virtual bool Shove(DirStruct dir)
        {
            return false;
        }
        public virtual void Stop_Movement_Animation() { }
        public virtual void Stop_Moving() { }
        public virtual void Tilt_Pitch_AI() { }
        public virtual void Unlimbo() { }
        public virtual void Unlock() { }

        public virtual VisualType Visual_Character(bool unused)
        {
            return VisualType.Normal;
        }

        public virtual bool Will_Jump_Tracks()
        {
            return false;
        }

        public virtual int Z_Adjust()
        {
            return 0;
        }

        public virtual ZGradient Z_Gradient()
        {
            return ZGradient.Deg90;
        }


        // IPersistStream

        public virtual int GetClassID(out Guid pClassID)
        {
            pClassID = GetType().GUID;
            return 0;
        }

        public virtual int IsDirty()
        {
            return 0;
        }

        void IPersistStream.Load(Microsoft.VisualStudio.OLE.Interop.IStream pstm)
        {
            var stream = pstm as IStream;
            Load(stream);
        }
        public abstract void Load(IStream stream);

        void IPersistStream.Save(Microsoft.VisualStudio.OLE.Interop.IStream pstm, int fClearDirty)
        {
            var stream = pstm as IStream;
            Save(stream, fClearDirty);
        }
        public abstract void Save(IStream stream, int fClearDirty);

        void IPersistStream.GetSizeMax(ULARGE_INTEGER[] pcbSize)
        {
            pcbSize[0].QuadPart = (ulong)SaveSize();
        }
        public abstract int SaveSize();

    }
}
