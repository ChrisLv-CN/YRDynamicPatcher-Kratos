using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 3704)]
    [Serializable]
    public struct UnitTypeClass
    {
        public static readonly IntPtr ArrayPointer = new IntPtr(0xA83CE0);

        public static YRPP.GLOBAL_DVC_ARRAY<UnitTypeClass> ABSTRACTTYPE_ARRAY = new YRPP.GLOBAL_DVC_ARRAY<UnitTypeClass>(ArrayPointer);

        [FieldOffset(0)] public TechnoTypeClass Base;
        [FieldOffset(0)] public ObjectTypeClass BaseObjectType;
        [FieldOffset(0)] public AbstractTypeClass BaseAbstractType;

        [FieldOffset(3596)] public Bool Passive;

        [FieldOffset(3597)] public Bool CrateGoodie;

        [FieldOffset(3598)] public Bool Harvester;

        [FieldOffset(3599)] public Bool Weeder;

        [FieldOffset(3600)] public Bool unknown_E10;

        [FieldOffset(3601)] public Bool HasTurret;

        [FieldOffset(3602)] public Bool DeployToFire;

        [FieldOffset(3603)] public Bool IsSimpleDeployer;

        [FieldOffset(3604)] public Bool IsTilter;

        [FieldOffset(3605)] public Bool UseTurretShadow;

        [FieldOffset(3606)] public Bool TooBigToFitUnderBridge;

        [FieldOffset(3607)] public Bool CanBeach;

        [FieldOffset(3608)] public Bool SmallVisceroid;

        [FieldOffset(3609)] public Bool LargeVisceroid;

        [FieldOffset(3610)] public Bool CarriesCrate;

        [FieldOffset(3611)] public Bool NonVehicle;

        [FieldOffset(3612)] public int StandingFrames;

        [FieldOffset(3616)] public int DeathFrames;

        [FieldOffset(3620)] public int DeathFrameRate;

        [FieldOffset(3624)] public int StartStandFrame;

        [FieldOffset(3628)] public int StartWalkFrame;

        [FieldOffset(3632)] public int StartFiringFrame;

        [FieldOffset(3636)] public int StartDeathFrame;

        [FieldOffset(3640)] public int MaxDeathCounter;

        [FieldOffset(3644)] public int Facings;

        [FieldOffset(3648)] public int FiringSyncFrame0;

        [FieldOffset(3652)] public int FiringSyncFrame1;

        [FieldOffset(3656)] public int BurstDelay0;

        [FieldOffset(3660)] public int BurstDelay1;

        [FieldOffset(3664)] public int BurstDelay2;

        [FieldOffset(3668)] public int BurstDelay3;

    }
}
