using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Components
{
    [Serializable]
    public class Transform : Component
    {
        public Transform() : base(true)
        {
            _transform = this;
        }

        public virtual Vector3 Location { get => _location; set => _location = value; }
        public virtual Vector3 Rotation { get => _rotation; set => _rotation = value; }
        public virtual Vector3 Scale { get => _scale; set => _scale = value; }

        public Transform GetParent()
        {
            return Parent.Parent.Transform;
        }

        public new Transform GetRoot()
        {
            return base.GetRoot().Transform;
        }

        public override void LoadFromStream(IStream stream)
        {
            base.LoadFromStream(stream);

            stream.Read(ref _location);
            stream.Read(ref _rotation);
            stream.Read(ref _scale);
        }
        public override void SaveToStream(IStream stream)
        {
            base.SaveToStream(stream);

            stream.Write(_location);
            stream.Write(_rotation);
            stream.Write(_scale);
        }

        [NonSerialized]
        protected Vector3 _location;
        [NonSerialized]
        protected Vector3 _rotation;
        [NonSerialized]
        protected Vector3 _scale;
    }
}

