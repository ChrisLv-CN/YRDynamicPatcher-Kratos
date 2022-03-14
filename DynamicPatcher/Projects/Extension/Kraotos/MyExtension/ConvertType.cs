using System.Reflection;
using DynamicPatcher;
using Extension.Utilities;
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
    public class ConvertTypeStatus
    {
        public SwizzleablePointer<TechnoTypeClass> pSourceType;

        public SwizzleablePointer<TechnoTypeClass> pTargetType;

        public bool HasBeenChanged;
        public bool Locked;

        public ConvertTypeStatus(Pointer<TechnoTypeClass> pType)
        {
            pSourceType = new SwizzleablePointer<TechnoTypeClass>(pType);
            pTargetType = new SwizzleablePointer<TechnoTypeClass>(IntPtr.Zero);
            HasBeenChanged = false;
            Locked = false;
        }

        public void ChangeTypeTo(Pointer<TechnoTypeClass> pNewType)
        {
            pTargetType.Pointer = pNewType;
            HasBeenChanged = false;
        }

        public void ResetType()
        {
            pTargetType.Pointer = IntPtr.Zero;
        }

        public override string ToString()
        {
            return string.Format("{{\"SourceType\":{0}, \"TargetType\":{1}, \"HasBeenChanged\":{2}}}",
                pSourceType.IsNull ? "null" : pSourceType.Ref.Base.Base.ID,
                pTargetType.IsNull ? "null" : pTargetType.Ref.Base.Base.ID,
                HasBeenChanged
            );
        }
    }

    public partial class TechnoExt
    {

        public ConvertTypeStatus ConvertTypeStatus;

        public unsafe void TechnoClass_Init_ConvertType()
        {
            ConvertTypeStatus = new ConvertTypeStatus(OwnerObject.Ref.Type);
        }

        public unsafe void TechnoClass_Update_ConvertType()
        {
            if (null != ConvertTypeStatus)
            {
                if (OwnerObject.Ref.Base.IsAlive && OwnerObject.Ref.Base.Health > 0 && !ConvertTypeStatus.Locked)
                {
                    // 更改
                    if (!ConvertTypeStatus.pTargetType.IsNull && !ConvertTypeStatus.HasBeenChanged)
                    {
                        // Logger.Log("更改类型 {0}, ConverTypeStatus = {1}", OwnerObject, ConvertTypeStatus);
                        ChangeTechnoTypeTo(ConvertTypeStatus.pTargetType);
                        ConvertTypeStatus.HasBeenChanged = true;
                    }
                    // 还原
                    if (ConvertTypeStatus.HasBeenChanged && ConvertTypeStatus.pTargetType.IsNull)
                    {
                        // Logger.Log("还原类型 {0}, ConverTypeStatus = {1}", OwnerObject, ConvertTypeStatus);
                        ChangeTechnoTypeTo(ConvertTypeStatus.pSourceType);
                        ConvertTypeStatus.HasBeenChanged = false;
                    }
                }

            }

        }

        private unsafe void ChangeTechnoTypeTo(Pointer<TechnoTypeClass> pNewType)
        {
            switch (OwnerObject.Ref.Base.Base.WhatAmI())
            {
                case AbstractType.Infantry:
                    OwnerObject.Convert<InfantryClass>().Ref.Type = pNewType.Convert<InfantryTypeClass>();
                    break;
                case AbstractType.Unit:
                    OwnerObject.Convert<UnitClass>().Ref.Type = pNewType.Convert<UnitTypeClass>();
                    break;
                case AbstractType.Aircraft:
                    OwnerObject.Convert<AircraftClass>().Ref.Type = pNewType.Convert<AircraftTypeClass>();
                    break;
            }
        }

        public unsafe bool TryConvertTypeTo(string newType)
        {
            Pointer<TechnoTypeClass> pTarget = IntPtr.Zero;
            // 检查目标是否同类
            switch (OwnerObject.Ref.Base.Base.WhatAmI())
            {
                case AbstractType.Infantry:
                    pTarget = InfantryTypeClass.ABSTRACTTYPE_ARRAY.Find(newType).Convert<TechnoTypeClass>();
                    break;
                case AbstractType.Unit:
                    pTarget = UnitTypeClass.ABSTRACTTYPE_ARRAY.Find(newType).Convert<TechnoTypeClass>();
                    break;
                case AbstractType.Aircraft:
                    pTarget = AircraftTypeClass.ABSTRACTTYPE_ARRAY.Find(newType).Convert<TechnoTypeClass>();
                    break;
            }
            if (!pTarget.IsNull && pTarget != ConvertTypeStatus.pTargetType)
            {
                ConvertTypeStatus.ChangeTypeTo(pTarget);
                return true;
            }
            return false;
        }

        public unsafe void CancelConverType(string newType)
        {
            Pointer<TechnoTypeClass> pTarget = IntPtr.Zero;
            // 检查目标是否同类
            switch (OwnerObject.Ref.Base.Base.WhatAmI())
            {
                case AbstractType.Infantry:
                    pTarget = InfantryTypeClass.ABSTRACTTYPE_ARRAY.Find(newType).Convert<TechnoTypeClass>();
                    break;
                case AbstractType.Unit:
                    pTarget = UnitTypeClass.ABSTRACTTYPE_ARRAY.Find(newType).Convert<TechnoTypeClass>();
                    break;
                case AbstractType.Aircraft:
                    pTarget = AircraftTypeClass.ABSTRACTTYPE_ARRAY.Find(newType).Convert<TechnoTypeClass>();
                    break;
            }
            if (!pTarget.IsNull)
            {
                ConvertTypeStatus.ResetType();
            }
        }

        public unsafe void TechnoClass_Destroy_ConvertType()
        {
            if (ConvertTypeStatus.HasBeenChanged)
            {
                // 死亡时强制还原
                // Logger.Log("强制还原类型 {0}, ConverTypeStatus = {1}", OwnerObject, ConvertTypeStatus);
                ChangeTechnoTypeTo(ConvertTypeStatus.pSourceType);
                ConvertTypeStatus.HasBeenChanged = false;
                ConvertTypeStatus.Locked = true;
            }
        }

    }


}