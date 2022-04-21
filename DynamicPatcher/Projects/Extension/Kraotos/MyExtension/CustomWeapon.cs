using Extension;
using Extension.Utilities;
using DynamicPatcher;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    [Serializable]
    public class DelayFireWeapon
    {
        public bool FireOwnWeapon;
        public int WeaponIndex;
        public SwizzleablePointer<WeaponTypeClass> pWeapon;
        public SwizzleablePointer<AbstractClass> pTarget;
        public int delay;
        public TimerStruct timer;
        public int count;

        public DelayFireWeapon(int weaponIndex, Pointer<AbstractClass> pTarget, int delay = 0, int count = 1)
        {
            this.FireOwnWeapon = true;
            this.WeaponIndex = weaponIndex;
            this.pWeapon = new SwizzleablePointer<WeaponTypeClass>(IntPtr.Zero);
            this.pTarget = new SwizzleablePointer<AbstractClass>(pTarget);
            this.delay = delay;
            this.timer.Start(delay);
            this.count = count;
        }

        public DelayFireWeapon(Pointer<WeaponTypeClass> pWeapon, Pointer<AbstractClass> pTarget, int delay = 0, int count = 1)
        {
            this.FireOwnWeapon = false;
            this.WeaponIndex = -1;
            this.pWeapon = new SwizzleablePointer<WeaponTypeClass>(pWeapon);
            this.pTarget = new SwizzleablePointer<AbstractClass>(pTarget);
            this.delay = delay;
            this.timer.Start(delay);
            this.count = count;
        }

        public bool TimesUp()
        {
            return timer.Expired();
        }

        public void ReduceOnce()
        {
            count--;
            timer.Start(delay);
        }

        public bool NotDone()
        {
            return count > 0;
        }

    }

    public partial class TechnoExt
    {
        public Queue<DelayFireWeapon> DelayFires = new Queue<DelayFireWeapon>();
        public CustomWeaponManager CustomWeaponManager = new CustomWeaponManager();

        public unsafe void EnqueueDelayFireWeapon(int weaponIndex, Pointer<AbstractClass> pTarget, int delay = 0, int count = 1)
        {
            DelayFireWeapon delayFire = new DelayFireWeapon(weaponIndex, pTarget, delay, count);
            DelayFires.Enqueue(delayFire);
        }

        public unsafe void EnqueueDelayFireWeapon(Pointer<WeaponTypeClass> pWeapon, Pointer<AbstractClass> pTarget, int delay = 0, int count = 1)
        {
            DelayFireWeapon delayFire = new DelayFireWeapon(pWeapon, pTarget, delay, count);
            DelayFires.Enqueue(delayFire);
        }

        public bool FireCustomWeapon(Pointer<TechnoClass> pShooter, Pointer<TechnoClass> pAttacker, Pointer<AbstractClass> pTarget, string weaponId, CoordStruct flh, CoordStruct bulletSourcePos, double rofMult = 1, FireBulletToTarget callback = null)
        {
            return CustomWeaponManager.FireCustomWeapon(pShooter, pAttacker, pTarget, weaponId, flh, bulletSourcePos, rofMult, callback);
        }

        public unsafe void TechnoClass_Update_CustomWeapon()
        {
            // 发射延迟武器队列
            for (int i = 0; i < DelayFires.Count; i++)
            {
                DelayFireWeapon delayFire = DelayFires.Dequeue();
                if (delayFire.TimesUp())
                {
                    // 发射武器
                    if (delayFire.FireOwnWeapon)
                    {
                        OwnerObject.Ref.Fire_IgnoreType(delayFire.pTarget, delayFire.WeaponIndex);
                    }
                    else
                    {
                        ExHelper.FireWeaponTo(OwnerObject, OwnerObject, delayFire.pTarget, delayFire.pWeapon, default);
                    }
                    delayFire.ReduceOnce();
                }
                if (delayFire.NotDone())
                {
                    DelayFires.Enqueue(delayFire);
                }
            }
            // 发射自定义武器
            CustomWeaponManager.Update(this);
        }

        public unsafe void TechnoClass_UnInit_CustomWeapon()
        {
            CustomWeaponManager.UnInitAll();
        }

    }


    [Serializable]
    public class AttachFireData
    {

        public bool UseROF;
        public bool CheckRange;
        public bool RadialFire;
        public int RadialAngle;
        public bool SimulateBurst;
        public int SimulateBurstDelay;
        public int SimulateBurstMode;
        public bool OnlyFireInTransport;
        public bool UseAlternateFLH;

        public AttachFireData()
        {
            this.UseROF = true;
            this.CheckRange = false;
            this.RadialFire = false;
            this.RadialAngle = 180;
            this.SimulateBurst = false;
            this.SimulateBurstDelay = 7;
            this.SimulateBurstMode = 0;
            this.OnlyFireInTransport = false;
            this.UseAlternateFLH = false;
        }
    }


    public partial class WeaponTypeExt
    {

        public AttachFireData AttachFireData = new AttachFireData();

        private void ReadCustomWeapon(INIReader reader, string section)
        {
            bool useROF = false;
            if (reader.ReadNormal(section, "AttachFire.UseROF", ref useROF))
            {
                AttachFireData.UseROF = useROF;
            }

            bool checkRange = false;
            if (reader.ReadNormal(section, "AttachFire.CheckRange", ref checkRange))
            {
                AttachFireData.CheckRange = checkRange;
            }

            bool radialFire = false;
            if (reader.ReadNormal(section, "AttachFire.RadialFire", ref radialFire))
            {
                AttachFireData.RadialFire = radialFire;
            }

            int radialAngle = 0;
            if (reader.ReadNormal(section, "AttachFire.RadialAngle", ref radialAngle))
            {
                AttachFireData.RadialAngle = radialAngle;
            }

            bool simulateBurst = false;
            if (reader.ReadNormal(section, "AttachFire.SimulateBurst", ref simulateBurst))
            {
                AttachFireData.SimulateBurst = simulateBurst;
            }

            int simulateBurstDelay = 0;
            if (reader.ReadNormal(section, "AttachFire.SimulateBurstDelay", ref simulateBurstDelay))
            {
                AttachFireData.SimulateBurstDelay = simulateBurstDelay;
            }

            int simulateBurstMode = 0;
            if (reader.ReadNormal(section, "AttachFire.SimulateBurstMode", ref simulateBurstMode))
            {
                AttachFireData.SimulateBurstMode = simulateBurstMode;
            }

            bool onlyFireInTransport = false;
            if (reader.ReadNormal(section, "AttachFire.OnlyFireInTransport", ref onlyFireInTransport))
            {
                AttachFireData.OnlyFireInTransport = onlyFireInTransport;
            }

            bool useAlternateFLH = false;
            if (reader.ReadNormal(section, "AttachFire.UseAlternateFLH", ref useAlternateFLH))
            {
                AttachFireData.UseAlternateFLH = useAlternateFLH;
            }
        }
    }

}