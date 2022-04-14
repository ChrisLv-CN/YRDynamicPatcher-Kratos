using Extension.Ext;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Utilities
{
    public class Finder
    {
        public static List<ExtensionReference<TechnoExt>> FindTechno(Pointer<HouseClass> pHouse, Func<Pointer<TechnoClass>, bool> expression, FindRange findRange)
        {
            ref DynamicVectorClass<Pointer<TechnoClass>> technos = ref TechnoClass.Array;
            List<ExtensionReference<TechnoExt>> targets = new List<ExtensionReference<TechnoExt>>();
            for (int i = technos.Count - 1; i >= 0; i--)
            {
                Pointer<TechnoClass> pTechno = technos.Get(i);
         
                if(IsValidTechno(pTechno,pHouse,expression,findRange))
                {
                    ExtensionReference<TechnoExt> tref = default;
                    tref.Set(TechnoExt.ExtMap.Find(pTechno));
                    targets.Add(tref);
                }
            }
            return targets;
        }

        public static ExtensionReference<TechnoExt> FineOneTechno(Pointer<HouseClass> pHouse, Func<Pointer<TechnoClass>, bool> expression, FindRange findRange)
        {
            ref DynamicVectorClass<Pointer<TechnoClass>> technos = ref TechnoClass.Array;
            ExtensionReference<TechnoExt> tref = default;

            for (int i = technos.Count - 1; i >= 0; i--)
            {
                Pointer<TechnoClass> pTechno = technos.Get(i);

                if (IsValidTechno(pTechno, pHouse, expression, findRange))
                {
                    tref.Set(TechnoExt.ExtMap.Find(pTechno));
                    return tref;
                }
            }
            return tref;
        }

        private static bool IsValidTechno(Pointer<TechnoClass> pTechno,Pointer<HouseClass> pHouse, Func<Pointer<TechnoClass>, bool> expression, FindRange findRange)
        {
            var houseIndex = pHouse.Ref.ArrayIndex;

            if (pTechno.IsNull)
                return false;
            if (pTechno.Ref.Owner.IsNull)
                return false;

            switch (findRange)
            {
                case FindRange.All:
                    {
                        break;
                    }
                case FindRange.Owner:
                    {
                        if (pTechno.Ref.Owner.Ref.ArrayIndex != houseIndex)
                            return false;
                        break;
                    }
                case FindRange.Allies:
                    {
                        if (pTechno.Ref.Owner.Ref.ArrayIndex != houseIndex && !pHouse.Ref.IsAlliedWith(pTechno.Ref.Owner.Ref.ArrayIndex))
                            return false;
                        break;
                    }
                case FindRange.Enermy:
                    {
                        if (pTechno.Ref.Owner.Ref.ArrayIndex == houseIndex || pHouse.Ref.IsAlliedWith(pTechno.Ref.Owner.Ref.ArrayIndex) || !pTechno.Ref.Base.IsAlive)
                            return false;
                        break;
                    }
                default:
                    break;
            }

            return expression(pTechno);

        }
    }




    public enum FindRange
    {
        All,
        Owner,
        Allies,
        Enermy
    }
}
