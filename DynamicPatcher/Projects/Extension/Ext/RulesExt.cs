using DynamicPatcher;
using Extension.Script;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    [Serializable]
    public partial class RulesExt : Extension<RulesClass>
    {

        public const string SectionGeneral = "General";
        public const string SectionCombatDamage = "CombatDamage";
        public const string SectionAV = "AudioVisual";

        [NonSerialized]
        public static RulesExt Instance = null;
        [NonSerialized]
        public IStream SavingStream;

        public RulesExt(Pointer<RulesClass> OwnerObject) : base(OwnerObject)
        {

        }

        public static RulesClass Global()
        {
            return Instance.OwnerObject.Ref;
        }

        public void SaveStatic()
        {
            if (!OwnerObject.IsNull && null != SavingStream)
            {
                SavingStream.WriteObject(Instance);
                Instance.SaveToStream(SavingStream);
                Instance.PartialSaveToStream(SavingStream);
            }
            SavingStream = null;
        }

        public void LoadStatic()
        {
            if (null != SavingStream)
            {
                SavingStream.ReadObject(out RulesExt val);
                val.OwnerObject = Instance.OwnerObject;
                val.EnsureConstanted();
                val.LoadFromStream(SavingStream);
                val.PartialLoadFromStream(SavingStream);
                Instance = val;
            }
            SavingStream = null;
        }

        public void LoadBeforeTypeData(Pointer<RulesClass> pRules, Pointer<CCINIClass> pINI)
        {
            // Logger.Log("Load before type data");
        }

        public void LoadAfterTypeData(Pointer<RulesClass> pRules, Pointer<CCINIClass> pINI)
        {
            // Logger.Log("Load after type data");
        }

        public static void Allocate(Pointer<RulesClass> pRules)
        {
            Instance = Activator.CreateInstance(typeof(RulesExt), pRules) as RulesExt;
            Instance.EnsureConstanted();

        }

        public static void Remove(Pointer<RulesClass> pRules)
        {
            Instance = null;
        }

        public static void LoadFromINIFile(Pointer<RulesClass> pRules, Pointer<CCINIClass> pINI)
        {
            Instance?.LoadFromINI(pINI);

            // 搭车
            TrailTypeExt.LoadFromINIList(CCINIClass.INI_Art);
            AttachEffectTypeExt.LoadFromINIList(CCINIClass.INI_Rules);
        }

        public static void RulesClass_SaveLoad_Prefix(IStream steam)
        {
            if (null != Instance)
            {
                Instance.SavingStream = steam;
            }
        }

        public static void RulesClass_Save_Suffix()
        {
            Instance?.SaveStatic();
        }

        public static void RulesClass_Load_Suffix()
        {
            Instance?.LoadStatic();
        }

        public static void RulesData_LoadBeforeTypeData(Pointer<RulesClass> pRules, Pointer<CCINIClass> pINI)
        {
            Instance?.LoadBeforeTypeData(pRules, pINI);
        }

        public static void RulesData_LoadAfterTypeData(Pointer<RulesClass> pRules, Pointer<CCINIClass> pINI)
        {
            Instance?.LoadAfterTypeData(pRules, pINI);
        }

    }
}

