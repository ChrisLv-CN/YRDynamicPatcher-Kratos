using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

using IPersistStream = Microsoft.VisualStudio.OLE.Interop.IPersistStream;

namespace PatcherYRpp.Utilities
{
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public abstract class ExtendedLocomotionClass : NewLocomotionClass, IDisposable
    {
        public ExtendedLocomotionClass()
        {

        }
        public ExtendedLocomotionClass(Guid clsid)
        {
            //_baseLocomotion.CreateInstance(clsid);
            //Base = _baseLocomotion.Interface;
            //_baseLocomotion.Release();
            //_baseLocomotion = _base.Interface.GetCOMPtr();

            _base.CreateInstance(clsid);
        }

        public ILocomotion Base
        {
            get => _base.Interface;
            set
            {
                //_baseLocomotion.Interface = value;
                _base.Interface = value;
            }
        }

        public override void Link_To_Object(IntPtr pointer) => Base.Link_To_Object(pointer);
        public override CoordStruct Destination() => Base.Destination();
        public override void Move_To(CoordStruct to) => Base.Move_To(to);
        public override void Stop_Moving() => Base.Stop_Moving();
        public override bool Is_Moving() => Base.Is_Moving();
        public override int Drawing_Code() => Base.Drawing_Code();
        public override IntPtr Draw_Matrix(IntPtr pMatrix, IntPtr pKey) => Base.Draw_Matrix(pMatrix, pKey);
        public override IntPtr Draw_Point(IntPtr pPoint) => Base.Draw_Point(pPoint);
        public override CoordStruct Head_To_Coord() => Base.Head_To_Coord();
        public override Layer In_Which_Layer() => Base.In_Which_Layer();
        public override void Do_Turn(DirStruct direction) => Base.Do_Turn(direction);
        public override bool Process() => Base.Process();
        public override void Acquire_Hunter_Seeker_Target() => Base.Acquire_Hunter_Seeker_Target();
        public override int Apparent_Speed() => Base.Apparent_Speed();
        public override Move Can_Enter_Cell(CellStruct cell) => Base.Can_Enter_Cell(cell);
        public override FireError Can_Fire() => Base.Can_Fire();
        public override void Force_Immediate_Destination(CoordStruct coord) => Base.Force_Immediate_Destination(coord);
        public override void Force_New_Slope(int ramp) => Base.Force_New_Slope(ramp);
        public override void Force_Track(int track, CoordStruct coord) => Base.Force_Track(track, coord);
        public override int Get_Speed_Accum() => Base.Get_Speed_Accum();
        public override int Get_Status() => Base.Get_Status();
        public override int Get_Track_Index() => Base.Get_Track_Index();
        public override int Get_Track_Number() => Base.Get_Track_Number();
        public override void ILocomotion_B8() => Base.ILocomotion_B8();
        public override bool Is_Ion_Sensitive() => Base.Is_Ion_Sensitive();
        public override bool Is_Moving_Here(CoordStruct to) => Base.Is_Moving_Here(to);
        public override bool Is_Moving_Now() => Base.Is_Moving_Now();
        public override bool Is_Powered() => Base.Is_Powered();
        public override bool Is_Really_Moving_Now() => Base.Is_Really_Moving_Now();
        public override bool Is_Surfacing() => Base.Is_Surfacing();
        public override bool Is_To_Have_Shadow() => Base.Is_To_Have_Shadow();
        public override void Lock() => Base.Lock();
        public override void Mark_All_Occupation_Bits(int mark) => Base.Mark_All_Occupation_Bits(mark);
        public override bool Power_Off() => Base.Power_Off();
        public override bool Power_On() => Base.Power_On();
        public override bool Push(DirStruct dir) => Base.Push(dir);
        public override IntPtr Shadow_Matrix(IntPtr pMatrix, IntPtr pKey) => Base.Shadow_Matrix(pMatrix, pKey);
        public override IntPtr Shadow_Point(IntPtr pPoint) => Base.Shadow_Point(pPoint);
        public override bool Shove(DirStruct dir) => Base.Shove(dir);
        public override void Stop_Movement_Animation() => Base.Stop_Movement_Animation();
        public override void Tilt_Pitch_AI() => Base.Tilt_Pitch_AI();
        public override void Unlimbo() => Base.Unlimbo();
        public override void Unlock() => Base.Unlock();
        public override VisualType Visual_Character(bool unused) => Base.Visual_Character(unused);
        public override bool Will_Jump_Tracks() => Base.Will_Jump_Tracks();
        public override int Z_Adjust() => Base.Z_Adjust();
        public override ZGradient Z_Gradient() => Base.Z_Gradient();

        public override void Load(IStream stream)
        {
            Guid clsid = default;
            stream.Read(ref clsid);

            //_baseLocomotion.CreateInstance(clsid);
            //_base.Interface = _baseLocomotion.Interface;
            //_baseLocomotion.Release();
            //_baseLocomotion = _base.Interface.GetCOMPtr();
            //_baseLocomotion.QueryInterface<IPersistStream>().Load(stream);

            _base.CreateInstance(clsid);
            _base.QueryInterface<IPersistStream>().Load(stream);

            stream.Read(ref disposedValue);
        }

        public override void Save(IStream stream, int fClearDirty)
        {
            //_baseLocomotion.QueryInterface<IPersistStream>().GetClassID(out Guid clsid);
            //stream.Write(clsid);
            //_baseLocomotion.QueryInterface<IPersistStream>().Save(stream, fClearDirty);

            _base.QueryInterface<IPersistStream>().GetClassID(out Guid clsid);
            stream.Write(clsid);
            _base.QueryInterface<IPersistStream>().Save(stream, fClearDirty);

            stream.Write(disposedValue);
        }

        public override int SaveSize() => _baseLocomotion.QueryInterface<IPersistStream>().SaveSize() + sizeof(bool);

        COMPtr<ILocomotion> _baseLocomotion = new();
        COMObject<ILocomotion> _base = new();

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
                Base = null;
            }
        }

        ~ExtendedLocomotionClass()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public abstract class ExtendedLocomotionClass<TLocomotionClass> : ExtendedLocomotionClass
    {
        public ExtendedLocomotionClass() : base(typeof(TLocomotionClass).GUID)
        {
        }
    }
}
