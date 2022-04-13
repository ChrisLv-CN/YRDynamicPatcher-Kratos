using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 828)]
    public struct ColorScheme
    {
    }

    public enum ColorSchemeIndex
    {
        //ColorScheme indices, since they are hardcoded all over the exe, why shan't we do it as well?
        Yellow = 3,
        White = 5,
        Grey = 7,
        Red = 11,
        Orange = 13,
        Pink = 15,
        Purple = 17,
        Blue = 21,
        Green = 29,
    }
}
