using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    public static class Game
    {
        private static IntPtr pCurrentFrame = new IntPtr(0xA8ED84);
        public static int CurrentFrame { get => pCurrentFrame.Convert<int>().Data; set => pCurrentFrame.Convert<int>().Ref = value; }

        // The height in the middle of a cell with a slope of 30 degrees
        public const int LevelHeight = 104;//89DE70
        public const int BridgeLevels = 4;
        public const int BridgeHeight = LevelHeight * BridgeLevels;//ABC5DC

        public const int CellSize = 256;

        /*
         * This thing is ridiculous
         * all xxTypeClass::Create functions use it:

          // doing this makes no sense - it's just a wrapper around CTOR, which doesn't call any Mutex'd functions... but who cares
          InfantryTypeClass *foo = something;
          ++SomeMutex;
          InfantryClass *obj = foo->CreateObject();
          --SomeMutex;

          // XXX do not do this if you aren't sure if the object can exist in this place
          // - this flag overrides any placement checks so you can put Terror Drones into trees and stuff
          ++SomeMutex;
          obj->Put(blah);
          --SomeMutex;

          AI base node generation uses it:
          int level = SomeMutex;
          SomeMutex = 0;
          House->GenerateAIBuildList();
          SomeMutex = level;

          Building destruction uses it:
          if(!SomeMutex) {
            Building->ShutdownSensorArray();
            Building->ShutdownDisguiseSensor();
          }

          Building placement uses it:
          if(!SomeMutex) {
            UnitTypeClass *freebie = Building->Type->FreeUnit;
            if(freebie) {
                freebie->CreateObject(blah);
            }
          }

          Building state animations use it:
          if(SomeMutex) {
            // foreach attached anim
            // update anim state (normal | damaged | garrisoned) if necessary, play anim
          }

          building selling uses it:
          if(blah) {
            ++SomeMutex;
            this->Type->UndeploysInto->CreateAtMapCoords(blah);
            --SomeMutex;
          }

          Robot Control Centers use it:
          if ( !SomeMutex ) {
            VoxClass::PlayFromName("EVA_RobotTanksOffline/BackOnline", -1, -1);
          }

          and so on...
         */
        // Note: SomeMutex has been renamed to this because it reflects the usage better
        private static IntPtr pIKnowWhatImDoing = new IntPtr(0xA8E7AC);
        public static int IKnowWhatImDoing { get => pIKnowWhatImDoing.Convert<int>().Data; set => pIKnowWhatImDoing.Convert<int>().Ref = value; }

        /*
        public static unsafe Vector3D<float> MatrixMultiply(ref Vector3D<float> ret, Matrix3DStruct matrix3DStruct, Vector3D<float> vec)
        {
            var func = (delegate* unmanaged[Thiscall]<int, IntPtr, IntPtr, IntPtr, IntPtr>)ASM.FastCallTransferStation;
            func(0x5AFB80,
                Pointer<Vector3D<float>>.AsPointer(ref ret),
                Pointer<Matrix3DStruct>.AsPointer(ref matrix3DStruct),
                Pointer<Vector3D<float>>.AsPointer(ref vec));
            return ret;
        }

        public static unsafe Vector3D<float> MatrixMultiply(Matrix3DStruct mtx, Vector3D<float> vec = default)
        {
            Vector3D<float> ret = default;
            MatrixMultiply(ref ret, mtx, vec);
            return ret;
        }
        */

        public static unsafe SingleVector3D MatrixMultiply(ref SingleVector3D ret, Matrix3DStruct matrix3DStruct, SingleVector3D vec)
        {
            var func = (delegate* unmanaged[Thiscall]<int, IntPtr, IntPtr, IntPtr, IntPtr>)ASM.FastCallTransferStation;
            func(0x5AFB80,
                Pointer<SingleVector3D>.AsPointer(ref ret),
                Pointer<Matrix3DStruct>.AsPointer(ref matrix3DStruct),
                Pointer<SingleVector3D>.AsPointer(ref vec));
            return ret;
        }

        public static unsafe SingleVector3D MatrixMultiply(Matrix3DStruct mtx, SingleVector3D vec = default)
        {
            SingleVector3D ret = default;
            MatrixMultiply(ref ret, mtx, vec);
            return ret;
        }

    }
}
