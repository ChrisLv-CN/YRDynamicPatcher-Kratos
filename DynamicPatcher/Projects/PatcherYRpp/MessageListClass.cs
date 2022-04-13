using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 1)]
    public struct MessageListClass
    {
        private static IntPtr instance = new IntPtr(0xA8BC60);
        public static ref MessageListClass Instance { get => ref instance.Convert<MessageListClass>().Ref; }

        // if pLabel is given, the message will be {$pLabel}:{$pMessage}
        // else it will be just {$pMessage}
        public unsafe void PrintMessage(UniString label, int unk1, UniString message, ColorSchemeIndex colorSchemeIndex = ColorSchemeIndex.Yellow, int unk2 = 0x4046, int duration = 0x96, bool silent = false)
        {
            var func = (delegate* unmanaged[Thiscall]<ref MessageListClass, IntPtr, int, IntPtr, ColorSchemeIndex, int, int, Bool, void>)0x5D3BA0;
            func(ref this, label, unk1, message, colorSchemeIndex, unk2, duration, silent);
        }

        public unsafe void PrintMessage(string message, ColorSchemeIndex colorSchemeIndex = ColorSchemeIndex.Yellow, int duration = 0x96, bool silent = false)
        {
            PrintMessage(null, 0, message, colorSchemeIndex, 0x4046, duration, silent);
        }

        public unsafe void PrintMessage(string label, string message, ColorSchemeIndex ColorSchemeIndex = ColorSchemeIndex.Yellow, int duration = 0x96, bool silent = false)
        {
            PrintMessage(label, 0, message, ColorSchemeIndex, 0x4046, duration, silent);
        }

    }
}



