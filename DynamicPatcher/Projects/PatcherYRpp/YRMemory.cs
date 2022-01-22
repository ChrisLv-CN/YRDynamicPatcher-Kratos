using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Caching;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp
{
	// provides access to the game's operator new and operator delete.
	public class YRMemory
	{
		// both functions are naked, which means neither prolog nor epilog are
		// generated for them. thus, a simple jump suffices to redirect to the
		// original methods, and no more book keeping or cleanup has to be
		// performed the calling convention has to match for this trick to work.

		// naked does not support inlining. the inline modifier here means that
		// multiple definitions are allowed.

		// the game's operator new
		public static unsafe IntPtr Allocate(uint size)
		{
			var func = (delegate* unmanaged[Cdecl]<uint, IntPtr>)0x7C8E17;
			return func(size);
		}

		// the game's operator delete
		public static unsafe void Deallocate(IntPtr mem)
		{
			var func = (delegate* unmanaged[Cdecl]<IntPtr, void>)0x7C8B3D;
			func(mem);
		}

		public static IntPtr AllocateChecked(uint size)
        {
            var ptr = Allocate(size);
            if (ptr == IntPtr.Zero)
			{
				throw new OutOfMemoryException("YRMemory Alloc fail.");
			}
            return ptr;
        }

		class ConstructorCache
        {
            public ConstructorCache(Type[] paramTypes, object[] paramList, Expression expression)
            {
                ParamTypes = paramTypes;
                ParamList = paramList;
                Expression = expression;
            }

            public Type[] ParamTypes { get; set; }
			public object[] ParamList { get; set; }
			public Expression Expression { get; set; }
		}

		static MemoryCache cache = new MemoryCache("constructor parameters");
		static Tuple<Type[], object[]> GetCache(int length)
        {
			string key = length.ToString();
			var ret = cache.Get(key);

			if (ret == null)
            {
				var policy = new CacheItemPolicy
				{
					SlidingExpiration = TimeSpan.FromSeconds(60.0)
				};
				cache.Set(key, new Tuple<Type[], object[]>(new Type[length], new object[length]), policy);
				ret = cache.Get(key);
			}
			return ret as Tuple<Type[], object[]>;
		}

		delegate T ConstructorFunction<T>(ref T @this, params object[] list);
		public static Pointer<T> Create<T>(params object[] list)
		{
			Pointer<T> ptr = AllocateChecked((uint)Marshal.SizeOf<T>());

			Type type = typeof(T);

			int paramCount = list.Length + 1;

			var tuple = GetCache(paramCount);
			Type[] paramTypes = tuple.Item1;
			object[] paramList = tuple.Item2;

			paramTypes[0] = ptr.GetType();
            for (int i = 0; i < list.Length; i++)
            {
				paramTypes[i + 1] = list[i].GetType();
			}

			paramList[0] = ptr;
			list.CopyTo(paramList, 1);

			MethodInfo constructor = type.GetMethod("Constructor", paramTypes);

			constructor.Invoke(null, paramList);

			return ptr;
		}

		public static void Delete<T>(Pointer<T> ptr)
		{
			if(ptr.IsNull == false)
			{
				Type type = typeof(T);
				MethodInfo destructor = type.GetMethod("Destructor", new Type[] { typeof(Pointer<T>) });

				destructor.Invoke(null, new object[] { ptr });
				Deallocate(ptr);
			}
		}
	}

}
