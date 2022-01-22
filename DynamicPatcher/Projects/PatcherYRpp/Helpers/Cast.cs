using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
    public static class Cast
    {
        public static bool CastToTechno(this Pointer<AbstractClass> pAbstract, out Pointer<TechnoClass> pTechno)
        {
            switch (pAbstract.Ref.WhatAmI())
            {
                case AbstractType.Unit:
                case AbstractType.Aircraft:
                case AbstractType.Building:
                case AbstractType.Infantry:
                    pTechno = pAbstract.Convert<TechnoClass>();
                    return true;
            }
            pTechno = Pointer<TechnoClass>.Zero;
            return false;
        }
        public static bool CastToTechno(this Pointer<ObjectClass> pObject, out Pointer<TechnoClass> pTechno)
        {
            return pObject.Convert<AbstractClass>().CastToTechno(out pTechno);
        }
    }
}
