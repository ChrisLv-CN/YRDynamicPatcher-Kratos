using System.Drawing;
using System.Threading;
using System.IO;
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


    public partial class TechnoExt
    {
        public unsafe void TechnoClass_Init_DeployToTransform()
        {
            if (null != Type.DeployToTransformData && OwnerObject.CastIf(AbstractType.Infantry, out Pointer<InfantryClass> pInf))
            {
                OnUpdateAction += InfantryClass_Update_DeployToTransform;
            }
        }

        public unsafe void InfantryClass_Update_DeployToTransform()
        {
            if (OwnerObject.Convert<InfantryClass>().Ref.SequenceAnim == SequenceAnimType.Deployed)
            {
                AttachEffectManager.GiftBoxState.Enable(Type.DeployToTransformData);
            }
        }

        // Hook触发
        public unsafe void UnitClass_Deployed_DeployToTransform()
        {
            if (null != Type.DeployToTransformData && OwnerObject.Convert<UnitClass>().Ref.Deployed)
            {
                AttachEffectManager.GiftBoxState.Enable(Type.DeployToTransformData);
            }
        }

    }

    public partial class TechnoTypeExt
    {
        public GiftBoxType DeployToTransformData;

        /// <summary>
        /// [TechnoType]
        /// DeployToTransform=HTNK,E2 ;部署跨类型变形，可以写多个
        /// DeployToTransform.Nums=1,1 ;数量
        /// DeployToTransform.Chances=1.0,1.0 ;抽中的概率，当决定要刷出这个类型时，可以刷出来的概率，每个类型单独计算概率，不写为100%
        /// DeployToTransform.RandomType=no ;随机从列表中选取类型，并释放等于Nums列表中数值总和的礼物数量
        /// DeployToTransform.RandomWeights=50,50 ;随机从列表中选区类型，对应列表中每个类型的权重值，数字越大概率越高，不写为1
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadDeployToTransform(INIReader reader, string section)
        {
            List<string> toTypes = null;
            if (reader.ReadStringList(section, "DeployToTransform", ref toTypes))
            {
                GiftBoxData data = new GiftBoxData();
                data.TryReadType(reader, section, "DeployToTransform.");
                data.Gifts = toTypes;
                data.Delay = 0;
                data.RandomDelay = default;
                DeployToTransformData = new GiftBoxType();
                DeployToTransformData.TryReadType(reader, section, "DeployToTransform.");
                DeployToTransformData.ForTransform();
                DeployToTransformData.Enable = true;
                DeployToTransformData.Data = data;
                DeployToTransformData.EliteData = data;
            }

        }
    }
}