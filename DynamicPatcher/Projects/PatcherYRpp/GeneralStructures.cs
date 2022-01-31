using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TimerStruct
    {
        public TimerStruct(int duration) : this() { this.Start(duration); }
        public void Start(int duration)
        {
            this.StartTime = Game.CurrentFrame;
            this.TimeLeft = duration;
        }

        public void Stop()
        {
            this.StartTime = -1;
            this.TimeLeft = 0;
        }

        public void Pause()
        {
            if (this.IsTicking())
            {
                this.TimeLeft = this.GetTimeLeft();
                this.StartTime = -1;
            }
        }

        public void Resume()
        {
            if (!this.IsTicking())
            {
                this.StartTime = Game.CurrentFrame;
            }
        }

        public int GetTimeLeft()
        {
            if (!this.IsTicking())
            {
                return this.TimeLeft;
            }

            var passed = Game.CurrentFrame - this.StartTime;
            var left = this.TimeLeft - passed;

            return (left <= 0) ? 0 : left;
        }

        // returns whether a ticking timer has finished counting down.
        public bool Completed()
        {
            return this.IsTicking() && !this.HasTimeLeft();
        }

        // returns whether a delay is active or a timer is still counting down.
        // this is the 'opposite' of Completed() (meaning: incomplete / still busy)
        // and logically the same as !Expired() (meaning: blocked / delay in progress)
        public bool InProgress()
        {
            return this.IsTicking() && this.HasTimeLeft();
        }

        // returns whether a delay is inactive. same as !InProgress().
        public bool Expired()
        {
            return !this.IsTicking() || !this.HasTimeLeft();
        }

        internal bool IsTicking()
        {
            return this.StartTime != -1;
        }

        internal bool HasTimeLeft()
        {
            return this.GetTimeLeft() > 0;
        }

        public int StartTime;
        public int gap;
        public int TimeLeft;

        public override string ToString()
        {
            return string.Format("{{\"StartTime\":{0}, \"gap\":{1}, \"TimeLeft\":{2}}}", StartTime, gap, TimeLeft);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RepeatableTimerStruct
    {
        public RepeatableTimerStruct(int duration) : this()
        {
            this.Start(duration);
        }

        public void Start(int duration)
        {
            this.Duration = duration;
            this.Restart();
        }

        public void Restart()
        {
            Base.Start(this.Duration);
        }

        public TimerStruct Base;
        public int Duration;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct ProgressTimer
    {
        public ProgressTimer(int duration)
        {
            this.Value = 0;
            this.HasChanged = false;
            this.Step = 1;

            this.Timer = new RepeatableTimerStruct(duration);
        }

        public void Start(int duration)
        {
            this.Timer.Start(duration);
        }

        public void Start(int duration, int step)
        {
            this.Step = step;
            this.Start(duration);
        }

        // returns whether the value changed.
        public bool Update()
        {
            if (this.Timer.Base.GetTimeLeft() != 0 || this.Timer.Duration == 0)
            {
                // timer is still running or hasn't been set yet.
                this.HasChanged = false;
            }
            else
            {
                // timer expired. move one step forward.
                this.Value += this.Step;
                this.HasChanged = true;
                this.Timer.Restart();
            }

            return this.HasChanged;
        }

        public int Value; // the current value
        public Bool HasChanged; // if the timer expired this frame and the value changed
        public RepeatableTimerStruct Timer;
        public int Step; // added to value every time the timer expires
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct RectangleStruct
    {
        public int X, Y, Width, Height;

        public RectangleStruct(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public override string ToString()
        {
            return string.Format("{{\"X\":{0}, \"Y\":{1}, \"Width\":{2}, \"Height\":{3}}}", X, Y, Width, Height);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DirStruct
    {
        public DirStruct(int value)
        {
            Value = (short)value;
            unused_2 = default;
        }
        public DirStruct(double rad) : this()
        {
            this.radians(rad);
        }
        public DirStruct(uint bits, short value)
                : this((short)TranslateFixedPoint(bits, 16, (ushort)value, 0))
        { }

        public short GetValue(uint Bits = 16)
        {
            if (Bits > 0 && Bits <= 16)
            {
                return (short)(TranslateFixedPoint(16, Bits, (uint)(this.Value), 0));
            }
            else
            {
                throw new InvalidOperationException("Bits has to be greater than 0 and lower or equal to 16.");
            }
        }


        public void SetValue(short value, uint Bits = 16)
        {
            if (Bits > 0 && Bits <= 16)
            {
                this.Value = (short)TranslateFixedPoint(Bits, 16, (uint)(value), 0);
            }
            else
            {
                throw new InvalidOperationException("Bits has to be greater than 0 and lower or equal to 16.");
            }
        }

        public static uint TranslateFixedPoint(uint bitsFrom, uint bitsTo, uint value, uint offset = 0)
        {
            uint MaskIn = (1u << (int)bitsFrom) - 1;
            uint MaskOut = (1u << (int)bitsTo) - 1;

            if (bitsFrom > bitsTo)
            {
                // converting down
                return (((((value & MaskIn) >> (int)(bitsFrom - bitsTo - 1)) + 1) >> 1) + offset) & MaskOut;
            }
            else if (bitsFrom < bitsTo)
            {
                // converting up
                return (((value - offset) & MaskIn) << (int)(bitsTo - bitsFrom)) & MaskOut;
            }
            else
            {
                return value & MaskOut;
            }
        }

        public short value8()
        {
            return this.GetValue(3);
        }

        public void value8(short value)
        {
            this.SetValue(value, 3);
        }

        public short value32()
        {
            return this.GetValue(5);
        }

        public void value32(short value)
        {
            this.SetValue(value, 5);
        }

        public short value256()
        {
            return this.GetValue(8);
        }

        public void value256(short value)
        {
            this.SetValue(value, 8);
        }

        public short value()
        {
            return this.GetValue(16);
        }

        public void value(short value)
        {
            this.SetValue(value, 16);
        }

        public double radians(uint Bits = 16)
        {
            if (Bits > 0 && Bits <= 16)
            {
                int Max = (1 << (int)Bits) - 1;

                int value = Max / 4 - this.GetValue(Bits);
                return -value * -(Math.PI * 2 / Max);
            }
            else
            {
                throw new InvalidOperationException("Bits has to be greater than 0 and lower or equal to 16.");
            }
        }

        public void radians(double rad, uint Bits = 16)
        {
            if (Bits > 0 && Bits <= 16)
            {
                int Max = (1 << (int)Bits) - 1;

                int value = (int)(rad * (Max / Math.PI * 2));
                this.SetValue((short)(Max / 4 - value), Bits);
            }
            else
            {
                throw new InvalidOperationException("Bits has to be greater than 0 and lower or equal to 16.");
            }
        }

        public static bool operator ==(DirStruct a, DirStruct b)
        {
            return a.Value == b.Value;
        }
        public static bool operator !=(DirStruct a, DirStruct b) => !(a == b);
        public override bool Equals(object obj) => this == (DirStruct)obj;
        public override int GetHashCode() => Value.GetHashCode();

        public short Value;
        ushort unused_2;

    }


    [StructLayout(LayoutKind.Sequential)]
    public struct FacingStruct
    {

        public short turn_rate()
        {
            return this.ROT.value();
        }

        public void turn_rate(short rot)
        {
            if (rot > 127)
                rot = 127;
            this.ROT.value8(rot);
        }

        public bool in_motion()
        {
            return this.turn_rate() > 0 && this.Timer.GetTimeLeft() > 0;
        }

        public DirStruct target()
        {
            return Value;
        }

        public DirStruct current(bool flip = false, int offset = 0)
        {
            DirStruct ret = this.Value;
            if (this.in_motion())
            {
                int diff = this.difference(flip);
                int num_steps = this.num_steps(flip);
                if (num_steps > 0)
                {
                    int steps_left = this.Timer.GetTimeLeft() - offset;
                    ret.Value -= (short)(steps_left * diff / num_steps);
                }
            }
            return ret;
        }

        public DirStruct next(bool flip = false)
        {
            return current(flip, 1);
        }

        public bool set(DirStruct value)
        {
            bool ret = (this.current(false) != value);
            if (ret)
            {
                this.Value = value;
                this.Initial = value;
            }
            this.Timer.Start(0);
            return ret;
        }

        public bool turn(DirStruct value)
        {
            return turn(value, false);
        }

        public bool turn(DirStruct value, bool flip = false)
        {
            if (this.Value == value)
                return false;
            this.Initial = this.current(flip);
            this.Value = value;
            if (this.turn_rate() > 0)
            {
                this.Timer.Start(this.num_steps(flip));
            }
            return true;
        }

        private int difference(bool flip)
        {
            // return Math.Abs((int)this.Value.value()) - Math.Abs((int)this.Initial.value());
            // return this.Value.value() - this.Initial.value();
            int v = this.Value.value();
            if (v < 0)
                v = 65536 - -v;
            int i = this.Initial.value();
            if (i < 0)
                i = 65536 - -i;
            int a = v - i;
            int b = this.Value.value() - this.Initial.value();
            int diff = Math.Abs(a) < Math.Abs(b) ? a : b;
            if (!flip)
            {
                return diff;
            }
            int flipDiff = 65536 - Math.Abs(diff);
            return diff < 0 ? flipDiff : -flipDiff;
        }

        private int num_steps(bool flip)
        {
            return Math.Abs(this.difference(flip)) / this.turn_rate();
        }

        public override string ToString()
        {
            return string.Format("{{\"Value\":{0}, \"Initial\":{1}, \"Timer\":{2}, \"ROT\":{3}}}", Value, Initial, Timer, ROT);
        }
        public DirStruct Value; // target facing
        public DirStruct Initial; // rotation started here
        public TimerStruct Timer; // counts rotation steps
        public DirStruct ROT; // Rate of Turn. INI Value * 256

    }


    [StructLayout(LayoutKind.Explicit, Size = 828)]
    public struct BytePalette
    {
        public const int EntriesCount = 256;
        [FieldOffset(0)] public ColorStruct Entries_first;
        public Pointer<ColorStruct> Entries => Pointer<ColorStruct>.AsPointer(ref Entries_first);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TintStruct
    {
        int Red;
        int Green;
        int Blue;
    };

    [StructLayout(LayoutKind.Explicit, Size = 20)]
    public struct SomeVoxelCache{

    }
}
