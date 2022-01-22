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

        public unsafe void TechnoClass_Destroy_DestroyAnims()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            // Logger.Log("Techno IsAlive={0} IsActive={1}, IsOnMap={2}", pTechno.Ref.Base.IsAlive, pTechno.Ref.Base.IsActive(), pTechno.Ref.Base.IsOnMap);
            TechnoTypeExt typeExt = Type;
            List<string> destroyAnims = typeExt.DestroyAnims;
            if (null != destroyAnims && destroyAnims.Count > 0)
            {
                int facing = destroyAnims.Count;
                int index = 0;
                if (!typeExt.DestroyAnimsRandom && facing % 8 == 0)
                {
                    // uint bits = (uint)Math.Round(Math.Sqrt(facing), MidpointRounding.AwayFromZero);
                    // double face = pTechno.Ref.GetRealFacing().target().GetValue(bits);
                    // double x = (face / (1 << (int)bits)) * facing;
                    // index = (int)Math.Round(x, MidpointRounding.AwayFromZero);
                    // Logger.Log("Index={0}/{1}, x={2}, bits={3}, face={4}, ", index, facing, x, bits, face);
                    index = ExHelper.Dir2FacingIndex(pTechno.Ref.Facing.current(), facing);
                    index = (int)(facing / 8) + index;
                    if (index >= facing)
                    {
                        index = 0;
                    }
                }
                else
                {
                    index = ExHelper.Random.Next(0, destroyAnims.Count - 1);
                    // Logger.Log("随机选择摧毁动画{0}/{1}", index, facing);
                }
                string animID = destroyAnims[index];
                // Logger.Log("选择摧毁动画{0}/{1}[{2}], HouseClass={3}", index, facing, animID, pTechno.Ref.Owner.IsNull ? "Null" : pTechno.Ref.Owner.Ref.Type.Ref.Base.ID);
                Pointer<AnimTypeClass> pAnimType = AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(animID);
                if (!pAnimType.IsNull)
                {
                    // Logger.Log("AnimType AltPalette={0}, MakeInf={1}, Next={2}", pAnimType.Ref.AltPalette, pAnimType.Ref.MakeInfantry, !pAnimType.Ref.Next.IsNull);
                    pAnimType.Ref.AltPalette = true;
                    CoordStruct location = pTechno.Ref.Base.Base.GetCoords();
                    Pointer<AnimClass> pAnim = YRMemory.Create<AnimClass>(pAnimType, location);
                    // Logger.Log("Anim Owner={0}, IsPlaying={1}, PaletteName={2}, TintColor={3}, HouseColorIndex={4}", !pAnim.Ref.Owner.IsNull, pAnim.Ref.IsPlaying, pAnim.Ref.PaletteName, pAnim.Ref.TintColor, pHouse.Ref.ColorSchemeIndex);
                    pAnim.Ref.Owner = pTechno.Ref.Owner;
                }
            }
        }


    }

    public partial class TechnoTypeExt
    {
        public List<string> DestroyAnims = null;
        public bool DestroyAnimsRandom = true;

        /// <summary>
        /// [TechnoType]
        /// DestroyAnims=Anim1,Anim2,Anim3,Anim4,Anim5,Anim6,Anim7,Anim8
        /// DestroyAnims.Random=no
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadDestroyAnims(INIReader reader, string section)
        {
            List<string> destroyAnims = null;
            if (ExHelper.ReadList(reader, section, "DestroyAnims", ref destroyAnims))
            {
                DestroyAnims = destroyAnims;
                bool random = false;
                if (reader.ReadNormal(section, "DestroyAnims.Random", ref random))
                {
                    DestroyAnimsRandom = random;
                }
            }
        }
    }

}