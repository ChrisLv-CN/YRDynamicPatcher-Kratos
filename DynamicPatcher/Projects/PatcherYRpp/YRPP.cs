using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace PatcherYRpp
{
    public static class YRPP
    {
        public class GLOBAL_DVC_ARRAY<T>
        {
            public Pointer<DynamicVectorClass<Pointer<T>>> Pointer;
            public ref DynamicVectorClass<Pointer<T>> Array { get => ref Pointer.Ref; }

            public GLOBAL_DVC_ARRAY(IntPtr pVector)
            {
                Pointer = pVector;
            }

            public Pointer<T> Find(string ID)
            {
                int idx = FindIndex(ID);
                if (idx >= 0)
                {
                    return Array.Get(idx);
                }

                return Pointer<T>.Zero;
            }

            public int FindIndex(string ID)
            {
                int i = 0;
                foreach (var ptr in Array)
                {
                    Pointer<AbstractTypeClass> pItem = ptr.Convert<AbstractTypeClass>();
                    if (pItem.Ref.ID == ID)
                    {
                        return i;
                    }

                    i++;
                }
                return -1;
            }
        }

        public static Pointer<T>[] Finds<T>(this GLOBAL_DVC_ARRAY<T> dvc, IEnumerable<string> ts)
        {
            return ts.Select(id => dvc.Find(id)).ToArray();
        }
        public static int[] FindIndexes<T>(this GLOBAL_DVC_ARRAY<T> dvc, IEnumerable<string> ts)
        {
            return ts.Select(id => dvc.FindIndex(id)).ToArray();
        }

        static YRPP()
        {
        }
    }
}
