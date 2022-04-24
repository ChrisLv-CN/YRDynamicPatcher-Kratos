using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    public partial class AnimExt
    {

        public string MyExtensionTest = nameof(MyExtensionTest);

        public unsafe void OnInit()
        {

        }

        public unsafe void OnUpdate()
        {
            AnimClass_Update_Damage();
            AnimClass_Update_SuperWeapon();
        }

        public unsafe void OnLoop()
        {
            AnimClass_Loop_SuperWeapon();
        }

        public unsafe void OnDone()
        {
            AnimClass_Done_SuperWeapon();
        }

        public unsafe void OnNext()
        {

        }

        public unsafe void OnRender()
        {

        }

        public unsafe void OnUnInit()
        {

        }

        public unsafe void OnDamage()
        {
            // 动画产生的伤害
            Explosion_Damage();
        }

        public unsafe void OnDebrisDamage()
        {
            // 碎片、流星砸地产生的伤害
            Explosion_Damage(true, true);
        }

        public unsafe void HitWater_Meteor()
        {
            // 流星砸水
            if (RulesExt.Instance.AllowDamageIfDebrisHitWater)
            {
                // 制造伤害
                Explosion_Damage(true);
            }
        }

        public unsafe void HitWater_Debris()
        {
            // 碎片砸水
            if (RulesExt.Instance.AllowDamageIfDebrisHitWater)
            {
                // 制造伤害
                Explosion_Damage(true);
            }
        }

    }

    public partial class AnimTypeExt : ITypeExtension
    {


        [INILoadAction]
        public void LoadINI(Pointer<CCINIClass> pINI)
        {
            // rules reader
            INIReader reader = new INIReader(pINI);
            string section = OwnerObject.Ref.Base.Base.ID;

            ReadAresFlags(reader, section);

            ReadExpireAnimOnWater(reader, section);
            ReadFireSuperWeapon(reader, section);
            ReadAnimDamage(reader, section);
        }

        [LoadAction]
        public void Load(IStream stream) { }

        [SaveAction]
        public void Save(IStream stream) { }
    }

}
