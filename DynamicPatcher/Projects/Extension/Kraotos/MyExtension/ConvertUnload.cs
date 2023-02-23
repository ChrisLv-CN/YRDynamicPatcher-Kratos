using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    public partial class TechnoExt
    {
        private bool needConvertWhenLanding = false;
        private bool landed = false;
        private string FloatingType;
        private string LandingType;


        public unsafe void UnitClass_Init_Convert_Unload()
        {
            if (string.IsNullOrEmpty(Type.ConvertUnloadTo)) return;
            needConvertWhenLanding = true;
            FloatingType = OwnerObject.Ref.Type.Ref.Base.Base.ID;
            LandingType = Type.ConvertUnloadTo;

            OnUpdateAction += UnitClass_Update_Convert_Unload;
        }

        public unsafe void UnitClass_Update_Convert_Unload()
        {
            if (!needConvertWhenLanding) return;
            var mission = OwnerObject.Convert<MissionClass>();

            if (landed == false)
            {
                if (mission.Ref.CurrentMission == Mission.Unload)
                {
                    OwnerObject.Convert<UnitClass>().Ref.Type = TechnoTypeClass.ABSTRACTTYPE_ARRAY.Find(LandingType).Convert<UnitTypeClass>();
                    landed = true;
                }
            }
            else
            {
                if (mission.Ref.CurrentMission == Mission.Move)
                {
                    OwnerObject.Convert<UnitClass>().Ref.Type = TechnoTypeClass.ABSTRACTTYPE_ARRAY.Find(FloatingType).Convert<UnitTypeClass>();
                    landed = false;
                }
            }
        }

    }

    public partial class TechnoTypeExt
    {
        public string ConvertUnloadTo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadConvertUnloadData(INIReader reader, string section)
        {
            if (string.IsNullOrEmpty(ConvertUnloadTo))
            {
                reader.ReadNormal(section, "Convert.Unload", ref ConvertUnloadTo);
            }
        }
    }
}
