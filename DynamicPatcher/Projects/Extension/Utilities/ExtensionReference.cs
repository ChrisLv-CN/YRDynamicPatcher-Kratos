using Extension.Ext;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Utilities
{
    [Serializable]
    public struct ExtensionReference<TExt> : ISerializable where TExt : class, IExtension
    {
        WeakReference<TExt> weakReference;
        PointerHandle<IntPtr> storedOwnerObject;

        [SecurityPermission(SecurityAction.LinkDemand,
            Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (this.TryGet(out TExt ext))
            {
                info.AddValue("OwnerObject", (int)ext.OwnerObject);
            }
        }
        private ExtensionReference(SerializationInfo info, StreamingContext context)
        {
            weakReference = null;
            try
            {
                IntPtr ownerObject = (IntPtr)info.GetInt32("OwnerObject");
                storedOwnerObject = new PointerHandle<IntPtr>(ownerObject);
                SwizzleManagerClass.Instance.Swizzle(ref storedOwnerObject.Pointer);
            }
            catch (SerializationException)
            {
                storedOwnerObject = null;
            }
        }

        public ExtensionReference(TExt ext)
        {
            weakReference = new WeakReference<TExt>(ext);
            storedOwnerObject = null;
        }
        public ExtensionReference(IntPtr ptr) : this(FindByPointer(ptr))
        {
        }

        public void Set(TExt ext)
        {
            if(weakReference == null)
            {
                weakReference = new WeakReference<TExt>(ext);
                storedOwnerObject = null;
            }
            else
            {
                weakReference.SetTarget(ext);
            }
        }
        public void Set(IntPtr ptr)
        {
            this.Set(FindByPointer(ptr));
        }

        public TExt Get()
        {
            this.TryGet(out TExt result);
            return result;
        }
        public bool TryGet(out TExt result)
        {
            if (weakReference != null && weakReference.TryGetTarget(out result))
            {
                if (result.OwnerObject != IntPtr.Zero)
                {
                    return true;
                }
            }
            else if(storedOwnerObject != null)
            {
                TExt ext = FindByPointer(storedOwnerObject.Pointer);
                if (ext != null)
                {
                    this.Set(ext);
                    result = ext;
                    return true;
                }

                // can not find Target because target destroyed
                storedOwnerObject = null;
            }

            result = null;
            return false;
        }

        private static IContainer ExtMap = ContainerHelper.GetContainer<TExt>();
        private static TExt FindByPointer(IntPtr ptr)
        {
            return (TExt)ExtMap.Find(ptr);
        }

        public bool ReferenceCollected
        {
            get
            {
                weakReference.TryGetTarget(out TExt ext);
                return ext == null;
            }
        }

        public static implicit operator TExt(ExtensionReference<TExt> reference) => reference.Get();
        public static implicit operator ExtensionReference<TExt>(TExt ext) => new ExtensionReference<TExt>(ext);
    }

    //[Serializable]
    //public struct ExtensionReference<TBase> : ISerializable
    //{
    //    WeakReference<Extension<TBase>> weakReference;
    //    PointerHandle<TBase> storedOwnerObject;

    //    [SecurityPermission(SecurityAction.LinkDemand,
    //        Flags = SecurityPermissionFlag.SerializationFormatter)]
    //    public void GetObjectData(SerializationInfo info, StreamingContext context)
    //    {
    //        if (this.TryGet(out Extension<TBase> ext))
    //        {
    //            info.AddValue("OwnerObject", ext.OwnerObject);
    //        }
    //    }
    //    private ExtensionReference(SerializationInfo info, StreamingContext context)
    //    {
    //        weakReference = null;
    //        try
    //        {
    //            storedOwnerObject = new PointerHandle<TBase>((IntPtr)info.GetInt32("OwnerObject"));
    //            SwizzleManagerClass.Instance.Swizzle(ref storedOwnerObject.Pointer);
    //        }
    //        catch (SerializationException)
    //        {
    //            storedOwnerObject = null;
    //        }
    //    }

    //    public ExtensionReference(Extension<TBase> ext)
    //    {
    //        weakReference = new WeakReference<Extension<TBase>>(ext);
    //        storedOwnerObject = null;
    //    }
    //    public ExtensionReference(IntPtr ptr) : this(FindByPointer(ptr))
    //    {
    //    }

    //    public void Set(Extension<TBase> ext)
    //    {
    //        if (weakReference == null)
    //        {
    //            weakReference = new WeakReference<Extension<TBase>>(ext);
    //            storedOwnerObject = null;
    //        }
    //        else
    //        {
    //            weakReference.SetTarget(ext);
    //        }
    //    }
    //    public Extension<TBase> Get()
    //    {
    //        this.TryGet(out Extension<TBase> result);
    //        return result;
    //    }
    //    public bool TryGet(out Extension<TBase> result)
    //    {
    //        if (weakReference != null && weakReference.TryGetTarget(out result))
    //        {
    //            if (result.Expired == false)
    //            {
    //                return true;
    //            }
    //        }
    //        else if (storedOwnerObject != null)
    //        {
    //            Extension<TBase> ext = FindByPointer(storedOwnerObject.Pointer);
    //            if (ext != null)
    //            {
    //                this.Set(ext);
    //                result = ext;
    //                return true;
    //            }

    //            // can not find Target because target destroyed
    //            storedOwnerObject = null;
    //        }

    //        result = null;
    //        return false;
    //    }

    //    private static Extension<TBase> FindByPointer(Pointer<TBase> ptr)
    //    {
    //        Extension<TBase> ext = null;
    //        return (Extension<TBase>)ext.GetContainer().Find(ptr);
    //    }

    //    public bool ReferenceCollected
    //    {
    //        get
    //        {
    //            weakReference.TryGetTarget(out Extension<TBase> ext);
    //            return ext == null;
    //        }
    //    }

    //    public static implicit operator Extension<TBase>(ExtensionReference<TBase> reference) => reference.Get();
    //    public static implicit operator ExtensionReference<TBase>(Extension<TBase> ext) => new ExtensionReference<TBase>(ext);
    //}
}
