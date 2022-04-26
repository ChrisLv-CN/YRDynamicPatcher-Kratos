using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PatcherYRpp.FileFormats;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 88)]
    public struct ParasiteClass
    {

        [FieldOffset(36)] public Pointer<FootClass> Owner;

        [FieldOffset(40)] public Pointer<FootClass> Victim;

        [FieldOffset(44)] public TimerStruct SuppressionTimer;

        [FieldOffset(56)] public TimerStruct DamageDeliveryTimer;

        [FieldOffset(68)] public Pointer<AnimClass> GrappleAnim;

        [FieldOffset(72)] public int GrappleState;

        [FieldOffset(76)] public int GrappleAnimFrame;

        [FieldOffset(80)] public int GrappleAnimDelay;

        [FieldOffset(84)] public Bool GrappleAnimGotInvalid;

    }
}
