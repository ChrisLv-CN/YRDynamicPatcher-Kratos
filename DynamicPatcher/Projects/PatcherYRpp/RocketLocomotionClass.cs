using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Explicit, Size = 92)]
    public struct RocketLocomotionClass
    {
        [FieldOffset(24)] public CoordStruct Destination;

        [FieldOffset(36)] public Timer24 Timer24;

        [FieldOffset(52)] public Timer34 Timer34; // 飞行轨迹阶段，0x66231F

        [FieldOffset(68)] public int Speed; // 0x44

        [FieldOffset(81)] public Bool UseElite;
    }


    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct Timer24
    {
        public int StartFrame;
        public Pointer<AircraftClass> Rocket; // 某种指针，Lazy时什么都不是，但在DMISL中，当 Step>=2 时为 rocket 自己
        public int Frames; // PauseFrames or TiltFrames

        public override string ToString()
        {
            return string.Format("{{\"StartFrame\":{0}, \"Rocket\":{1}, \"Frames\":{2}}}", StartFrame, Rocket, Frames);
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct Timer34
    {
        public int StartFrame; // 意义不明的数字331，但当 State>=3时为上一帧的帧号，并按照 Delay 保存2帧
        public int Unknown; // 意义不明的数字33
        public int Delay; // 意义不明的数字0，当 State >= 3时为2

        // 重要，火箭的飞行阶段
        // 1准备Pause，2起竖Tilt，3上升，4下降\平飞，5下降（Lazy=no）
        // Lazy=yes时，没有5，阶段4为下降阶段，并开始检查与目标的距离，然后引爆
        // Lazy=no 时，阶段4为平飞的阶段，阶段5为下降阶段，并开始检查与目标的距离，然后引爆
        public int Step;

        public override string ToString()
        {
            return string.Format("{{\"StartFrame\":{0}, \"Unknown\":{1}, \"Delay\":{2}, \"Step\":{3}}}", StartFrame, Unknown, Delay, Step);
        }
    }
}
