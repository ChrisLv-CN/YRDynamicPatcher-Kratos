using System.Reflection;
using System.Collections;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    [Serializable]
    public class DamageSelfState : AEState<DamageSelfType>
    {
        public bool Hit;

        public SwizzleablePointer<WarheadTypeClass> pWH = new SwizzleablePointer<WarheadTypeClass>(IntPtr.Zero);
        public BulletDamageStatus BulletDamageStatus = new BulletDamageStatus(1);

        private int delay;
        private TimerStruct DelayTimer;

        public override void OnEnable()
        {
            // 伤害弹头
            pWH.Pointer = RulesClass.Instance.Ref.C4Warhead;
            if (!string.IsNullOrEmpty(Data.Warhead))
            {
                Pointer<WarheadTypeClass> pCustomWH = WarheadTypeClass.ABSTRACTTYPE_ARRAY.Find(Data.Warhead);
                if (!pCustomWH.IsNull)
                {
                    pWH.Pointer = pCustomWH;
                }
            }
            // 抛射体伤害
            BulletDamageStatus.Damage = Data.Damage;
            BulletDamageStatus.Eliminate = false; // 非一击必杀
            BulletDamageStatus.Harmless = false; // 非和平处置

            Reset();
        }

        public void Reset()
        {
            this.Hit = false;
            this.delay = Data.ROF;
            if (delay > 0)
            {
                DelayTimer.Start(delay);
            }
        }

        public bool CanHitSelf()
        {
            return IsActive() && !Hit && Timeup();
        }

        private bool Timeup()
        {
            Hit = delay <= 0 || DelayTimer.Expired();
            return Hit;
        }

    }
}