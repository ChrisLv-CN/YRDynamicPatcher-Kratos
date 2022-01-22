using PatcherYRpp.FileFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 256)]
	[Serializable]
    public struct SuperWeaponTypeClass
	{
		public static readonly IntPtr ArrayPointer = new IntPtr(0xA8E330);

		public static YRPP.GLOBAL_DVC_ARRAY<SuperWeaponTypeClass> ABSTRACTTYPE_ARRAY = new YRPP.GLOBAL_DVC_ARRAY<SuperWeaponTypeClass>(ArrayPointer);

		[FieldOffset(0)] public AbstractTypeClass Base;

		[FieldOffset(152)] public int ArrayIndex;
		[FieldOffset(156)] public Pointer<WeaponTypeClass> WeaponType;
		[FieldOffset(160)] public int RechargeVoice; // not read, unused
		[FieldOffset(164)] public int ChargingVoice; // not read, unused
		[FieldOffset(168)] public int ImpatientVoice; // not read, unused
		[FieldOffset(172)] public int SuspendVoice; // not read, unused
		[FieldOffset(176)] public int RechargeTime; //in frames
		[FieldOffset(180)] public SuperWeaponType Type;
		[FieldOffset(184)] public Pointer<SHPStruct> SidebarImage;
		[FieldOffset(188)] public Action Action;
		[FieldOffset(192)] public int SpecialSound;
		[FieldOffset(196)] public int StartSound;
		[FieldOffset(200)] public Pointer<BuildingTypeClass> AuxBuilding;
		[FieldOffset(204)] public byte SidebarImageFile_first;
		public AnsiStringPointer SidebarImageFile => Pointer<byte>.AsPointer(ref SidebarImageFile_first);
		[FieldOffset(229)] public Bool UseChargeDrain;
		[FieldOffset(230)] public Bool IsPowered;
		[FieldOffset(231)] public Bool DisableableFromShell;
		[FieldOffset(232)] public int FlashSidebarTabFrames;
		[FieldOffset(236)] public Bool AIDefendAgainst;
		[FieldOffset(237)] public Bool PreClick;
		[FieldOffset(238)] public Bool PostClick;
		[FieldOffset(240)] public int PreDependent;
		[FieldOffset(244)] public Bool ShowTimer;
		[FieldOffset(245)] public Bool ManualControl;
		[FieldOffset(248)] public float Range;
		[FieldOffset(252)] public int LineMultiplier;
	}
}
