
# C# Style YRpp

[![license](https://www.gnu.org/graphics/gplv3-or-later.png)](https://www.gnu.org/licenses/gpl-3.0.en.html)

[Pointer](Helpers/Pointer.cs) & [PointerHandle](Helpers/PointerHandle.cs)
============
It is suggested that use Pointer<T> to replace T*.

Use Pointer<T>.Ref to access members of T.

Use Pointer<TFrom>.Convert<TTo>() if it is need to convert the pointer type. IntPtr also has this extend function.

Use Pointer<T>.AsPointer(ref obj) to get an object address.

Use PointerHandle<T> if it is need to set the pointer later(avoid GC moving).

How to add class content I want?
--------
First, you need a classes layout of YRPP.

Every class should be writen below:
``` csharp

[StructLayout(LayoutKind.Explicit, Size = struct_size)]
public struct YourClass
{
  // your function
  public unsafe void function()
  {
      var func = (delegate* unmanaged[Thiscall]<ref YourClass, void>)function_address;
      func(ref this);
  }
  
  // your virtual function
  public unsafe void function()
  {
      var func = (delegate* unmanaged[Thiscall]<ref YourClass, void>)Helpers.GetVirtualFunctionPointer(Pointer<YourClass>.AsPointer(ref this), virtual_function_index);
      func(ref this);
  }

  // *REMARK*
  // If you meet 'fastcall' function, you should use ASM.FastCallTransferStation as below.
  public static unsafe void function(int para)
  {
     var func = (delegate* unmanaged[Thiscall]<int, int, void>)ASM.FastCallTransferStation;
     func(function_address, para);
  }

  // YR's class DVC
  static public readonly IntPtr ArrayPointer = new IntPtr(DVC_address);
  static public ref DynamicVectorClass<Pointer<TechnoClass>> Array { get => ref DynamicVectorClass<Pointer<TechnoClass>>.GetDynamicVector(ArrayPointer); }

  // for xxxTypeClass
	static public YRPP.ABSTRACTTYPE_ARRAY<YourClass> ABSTRACTTYPE_ARRAY = new YRPP.ABSTRACTTYPE_ARRAY<YourClass>(ArrayPointer);

  // your member
  [FieldOffset(member_offset)] public TMember Member;

  // *REMARK*
  // If you meet 'bool' type, you should write as 'byte' as below.
  [FieldOffset(member_offset)] public byte member;
  public bool Member { get => Convert.ToBoolean(member); set => member = Convert.ToByte(value); }
  // or use simple version
  [FieldOffset(member_offset)] public Bool member;

  
  // *REMARK*
  // If you meet 'T[N]' type, you should write it as below.
  // Then you can access Member[i] as normal array.
  [FieldOffset(member_offset)] public T array_first;
  public Pointer<T> Member => Pointer<T>.AsPointer(ref array_first);

  // *REMARK*
  // you should avoid "Circular Reference" as below
  // which may cause Circular Reference - T1->T2->T3->T1
  [FieldOffset(member_offset)] public GenericClass<T1> gT1;
  // the solution is use property
  [FieldOffset(member_offset)] public byte gT1;
  public ref GenericClass<T1> Member => Pointer<GenericClass<T1>>.AsPointer(ref gT1).Ref;
  
  // *REMARK*
  // If you meet string member, you should write it as below.
  [FieldOffset(member_offset)] public byte str_first;
  public AnsiStringPointer Member => Pointer<byte>.AsPointer(ref str_first);
  // for unicode
  [FieldOffset(member_offset)] public char str_first;
  public UniStringPointer Member => Pointer<char>.AsPointer(ref str_first);
}

```

In general, you can not write any [Non-Blittable Types](http://msdn.microsoft.com/en-us/library/75dwhxf7.aspx) in struct, or you will get many strange problem.

