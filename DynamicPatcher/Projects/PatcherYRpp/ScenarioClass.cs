using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 41)]
    [Serializable]
    public struct Variable
    {
        [FieldOffset(0)] public byte Name_first;
        public AnsiStringPointer Name => Pointer<byte>.AsPointer(ref Name_first);
        [FieldOffset(40)] public byte Value;
    };

    [StructLayout(LayoutKind.Explicit, Size = 14144)]
    [Serializable]
    public struct ScenarioClass
    {
        private static IntPtr instance = new IntPtr(0xA8B230);
        public static ref ScenarioClass Instance { get => ref instance.Convert<Pointer<ScenarioClass>>().Ref.Ref; }

        [FieldOffset(536) ]public Randomizer Random; //218

        [FieldOffset(4700)] public byte FileName_first;
        public AnsiStringPointer FileName => Pointer<byte>.AsPointer(ref FileName_first);

        [FieldOffset(7304)] public Variable GlobalVariables_first;
        public Pointer<Variable> GlobalVariables => Pointer<Variable>.AsPointer(ref GlobalVariables_first);
        [FieldOffset(9354)] public Variable LocalVariables_first;
        public Pointer<Variable> LocalVariables => Pointer<Variable>.AsPointer(ref LocalVariables_first);
    }
}
