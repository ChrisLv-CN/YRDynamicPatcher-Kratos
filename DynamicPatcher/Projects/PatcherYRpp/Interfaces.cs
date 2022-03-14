using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using HRESULT = System.UInt32;
using VARIANT_BOOL = System.Int16;
using IStream = System.Runtime.InteropServices.ComTypes.IStream;

namespace PatcherYRpp
{
	[Guid("070F3290-9841-11D1-B709-00A024DDAFD1")]
	[ComVisible(true), ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface ILocomotion
	{
		void Link_To_Object(IntPtr pointer); //Links object to locomotor.
		[PreserveSig]
		bool Is_Moving();   //Sees if object is moving.
		[PreserveSig]
		IntPtr Destination(IntPtr pcoord);  //Fetches destination coordinate.
		[PreserveSig] 
		IntPtr Head_To_Coord(IntPtr pcoord); // Fetches immediate (next cell) destination coordinate.
		[PreserveSig]
		Move Can_Enter_Cell(CellStruct cell); //Determine if specific cell can be entered.
		[PreserveSig]
		bool Is_To_Have_Shadow();   //Should object cast a shadow?
		[PreserveSig]
		IntPtr Draw_Matrix(IntPtr pMatrix, IntPtr pKey); //Fetch voxel draw matrix.
		[PreserveSig]
		IntPtr Shadow_Matrix(IntPtr pMatrix, IntPtr pKey);   //Fetch shadow draw matrix.
		[PreserveSig]
		IntPtr Draw_Point(IntPtr pPoint);   //Draw point center location.
		[PreserveSig]
		IntPtr Shadow_Point(IntPtr pPoint); //Shadow draw point center location.
		[PreserveSig]
		VisualType Visual_Character([MarshalAs(UnmanagedType.VariantBool)] bool unused);   //Visual character for drawing.
		[PreserveSig]
		int Z_Adjust(); //Z adjust control value.
		[PreserveSig]
		ZGradient Z_Gradient(); //Z gradient control value.
		[PreserveSig]
		bool Process(); //Process movement of object.]
		[PreserveSig]
		void Move_To(CoordStruct to);   //Instruct to move to location specified.
		[PreserveSig]
		void Stop_Moving(); //Stop moving at first opportunity.
		[PreserveSig]
		void Do_Turn(DirStruct direction);  //Try to face direction specified.
		[PreserveSig]
		void Unlimbo(); //Object is appearing in the world.
		[PreserveSig]
		void Tilt_Pitch_AI();   //Special tilting AI function.
		[PreserveSig]
		bool Power_On();    //Locomotor becomes powered.
		[PreserveSig]
		bool Power_Off();   //Locomotor loses power.
		[PreserveSig]
		bool Is_Powered();  //Is locomotor powered?
		[PreserveSig] 
		bool Is_Ion_Sensitive();    //Is locomotor sensitive to ion storms?
		[PreserveSig]
		bool Push(DirStruct dir);   //Push object in direction specified.
		[PreserveSig]
		bool Shove(DirStruct dir);  //Shove object (with spin) in direction specified.
		[PreserveSig]
		void Force_Track(int track, CoordStruct coord); //Force drive track -- special case only.
		[PreserveSig]
		Layer In_Which_Layer(); //What display layer is it located in.
		[PreserveSig]
		void Force_Immediate_Destination(CoordStruct coord);    //Don't use this function.
		[PreserveSig]
		void Force_New_Slope(int ramp); //Force a voxel unit to a given slope. Used in cratering.
		[PreserveSig]
		bool Is_Moving_Now();   //Is it actually moving across the ground this very second?
		[PreserveSig]
		int Apparent_Speed();   //Actual current speed of object expressed as leptons per game frame.
		[PreserveSig]
		int Drawing_Code(); //Special drawing feedback code (locomotor specific meaning)
		[PreserveSig]
		FireError Can_Fire();   //Queries if any locomotor specific state prevents the object from firing.
		[PreserveSig]
		int Get_Status();   //Queries the general state of the locomotor.
		[PreserveSig]
		void Acquire_Hunter_Seeker_Target();    //Forces a hunter seeker droid to find a target.
		[PreserveSig]
		bool Is_Surfacing();    //Is this object surfacing?
		[PreserveSig]
		void Mark_All_Occupation_Bits(int mark);    //Lifts all occupation bits associated with the object off the map
		[PreserveSig]
		bool Is_Moving_Here(CoordStruct to);    //Is this object in the process of moving into this coord.
		[PreserveSig]
		bool Will_Jump_Tracks();    //Will this object jump tracks?
		[PreserveSig]
		bool Is_Really_Moving_Now();    //Infantry moving query function
		[PreserveSig]
		void Stop_Movement_Animation(); //Falsifies the IsReallyMoving flag in WalkLocomotionClass
		[PreserveSig]
		void Lock();    //Locks the locomotor from being deleted
		[PreserveSig]
		void Unlock();  //Unlocks the locomotor from being deleted
		[PreserveSig]
		void ILocomotion_B8();  //Unknown, must have been added after LOCOS.TLB was generated. -pd
		[PreserveSig]
		int Get_Track_Number(); //Queries internal variables
		[PreserveSig]
		int Get_Track_Index();  //Queries internal variables
		[PreserveSig]
		int Get_Speed_Accum();  //Queries internal variables
	}

	public static class ILocomotionHelpers
    {
		public static CoordStruct Destination(this ILocomotion locomotion)
        {
			CoordStruct tmp = default;
			locomotion.Destination(tmp.GetThisPointer());
			return tmp;
        }
		public static CoordStruct Head_To_Coord(this ILocomotion locomotion)
		{
			CoordStruct tmp = default;
			locomotion.Head_To_Coord(tmp.GetThisPointer());
			return tmp;
		}
	}

	[Guid("92FEA800-A184-11D1-B70A-00A024DDAFD1")]
	[ComVisible(true), ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IPiggyback
	{
		void Begin_Piggyback(ILocomotion locomotion);   //Piggybacks a locomotor onto this one.
		void End_Piggyback(out ILocomotion locomotion); //End piggyback process and restore locomotor interface pointer.
		[PreserveSig]
		bool Is_Ok_To_End();    //Is it ok to end the piggyback process?
		void Piggyback_CLSID(out Guid classid); //Fetches piggybacked locomotor class ID.
		[PreserveSig]
		bool Is_Piggybacking();	//Is it currently piggy backing another locomotor?
	}

	public static class IStreamHelpers
	{
		public static uint Write(this IStream stream, byte[] buffer)
		{
			uint written = 0;
			stream.Write(buffer, buffer.Length, Pointer<uint>.AsPointer(ref written));
			return written;
		}
		public static uint Write<T>(this IStream stream, T obj)
		{
			var ptr = Pointer<T>.AsPointer(ref obj);
			byte[] buffer = new byte[Pointer<T>.TypeSize()];
			Marshal.Copy(ptr, buffer, 0, buffer.Length);
			return stream.Write(buffer);
		}
		public static uint Read(this IStream stream, byte[] buffer)
		{
			uint written = 0;
			stream.Read(buffer, buffer.Length, Pointer<uint>.AsPointer(ref written));
			return written;
		}
		public static uint Read<T>(this IStream stream, ref T obj)
		{
			var ptr = Pointer<T>.AsPointer(ref obj);
			byte[] buffer = new byte[Pointer<T>.TypeSize()];
			uint written = stream.Read(buffer);
			Marshal.Copy(buffer, 0, ptr, buffer.Length);
			return written;
		}
	}
}
