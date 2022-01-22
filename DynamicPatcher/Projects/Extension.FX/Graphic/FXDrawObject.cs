using Extension.FX.Definitions;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

using D3D11 = SharpDX.Direct3D11;

namespace Extension.FX.Graphic
{
    public class FXDrawObject : CriticalFinalizerObject, IDisposable, ICloneable
    {
        private VS_INPUT[] vertices = new VS_INPUT[] {
            new VS_INPUT(new SharpDX.Vector3(-0.5f, -0.5f, 0.0f), new SharpDX.Vector2(0.0f, 0.0f)),
            new VS_INPUT(new SharpDX.Vector3(-0.5f, 0.5f, 0.0f),  new SharpDX.Vector2(0.0f, 1.0f)),
            new VS_INPUT(new SharpDX.Vector3(0.5f, -0.5f, 0.0f), new SharpDX.Vector2(1.0f, 0.0f)),
            new VS_INPUT(new SharpDX.Vector3(0.5f, 0.5f, 0.0f), new SharpDX.Vector2(1.0f, 1.0f))
        };
        private static int[] indices = new int[] {
            0,2,1,
            1,2,3,
        };

        private Lazy<D3D11.Buffer> _vertexBuffer;
        private Lazy<D3D11.Buffer> _indexBuffer;
        private Lazy<ShaderResourceView> _textureView;
        private int _textureWidth;
        private int _textureHeight;
        private Transform transform;

        public FXDrawObject(string texturePath)
        {
            TexturePath = texturePath;

            _vertexBuffer = new Lazy<D3D11.Buffer>(() => FXGraphic.CreateBuffer(BindFlags.VertexBuffer, vertices));
            _indexBuffer = new Lazy<D3D11.Buffer>(() => FXGraphic.CreateBuffer(BindFlags.IndexBuffer, indices, usage: ResourceUsage.Immutable));
            _textureView = new Lazy<ShaderResourceView>(() => FXGraphic.GetTexture(TexturePath));

            Scale = new Vector3(1, 1, 1);
        }

        public string TexturePath { get; }

        public Transform Transform { get => transform; set => transform = value; }
        public Vector3 Location { get => transform.Location; set => transform.Location = value; }
        public Rotator Rotation { get => transform.Rotation; set => transform.Rotation = value; }
        public Vector3 Scale { get => transform.Scale; set => transform.Scale = value; }

        public D3D11.Buffer VertexBuffer => _vertexBuffer.Value;
        public D3D11.Buffer IndexBuffer => _indexBuffer.Value;
        public int IndexCount => indices.Length;

        public ShaderResourceView TextureView => _textureView.Value;

        public int TextureWidth
        {
            get
            {
                if(_textureWidth == 0)
                {
                    SetTextureInfo();
                }
                return _textureWidth;
            }
        }
        public int TextureHeight
        {
            get
            {
                if (_textureHeight == 0)
                {
                    SetTextureInfo();
                }
                return _textureHeight;
            }
        }
        private void SetTextureInfo()
        {
            using (var tex = TextureView.ResourceAs<Texture2D>())
            {
                _textureWidth = tex.Description.Width;
                _textureHeight = tex.Description.Height;
            }
        }

        public virtual FXDrawObject Clone()
        {
            FXDrawObject drawObject = new FXDrawObject(TexturePath);

            drawObject.Transform = Transform;
            drawObject._textureWidth = _textureWidth;
            drawObject._textureHeight = _textureHeight;

            return drawObject;
        }

        public void RotateTo(Vector3 dest)
        {
            Vector3 offset = dest - Location;

            Rotation = FXEngine.FindLookAtRotation(Location, dest);

            Vector3 sourcePosition = FXEngine.WorldToClient(Location);
            Vector3 targetPosition = FXEngine.WorldToClient(dest);

            SetLocalBuffer(sourcePosition, targetPosition);
        }

        public void MoveTo(Vector3 dest)
        {
            Location = dest;
            Vector3 facing = FXEngine.GetForwardVector(Rotation) * 100;

            Vector3 sourcePosition = FXEngine.WorldToClient(Location);
            Vector3 facingPosition = FXEngine.WorldToClient(Location + facing);

            Vector2 direction = (facingPosition.XY - sourcePosition.XY).Direction * Scale.X * TextureHeight / 2;

            SetLocalBuffer(new Vector3(sourcePosition.XY - direction, sourcePosition.Z), new Vector3(sourcePosition.XY + direction, sourcePosition.Z));
            //Vector3 sourcePosition = FXEngine.WorldToClient(Location);
            //float baseZ = sourcePosition.Z + sourcePosition.Y;

            //Vector2 pos1 = sourcePosition.XY + new Vector2(-TextureWidth / 2, -TextureHeight / 2);
            //Vector2 pos2 = sourcePosition.XY + new Vector2(-TextureWidth / 2, TextureHeight / 2);
            //Vector2 pos3 = sourcePosition.XY + new Vector2(TextureWidth / 2, -TextureHeight / 2);
            //Vector2 pos4 = sourcePosition.XY + new Vector2(TextureWidth / 2, TextureHeight / 2);

            //vertices[0] = new VS_INPUT(new SharpDX.Vector3(pos1.X, pos1.Y, baseZ - pos1.Y), new SharpDX.Vector2(0.0f, 0.0f));
            //vertices[1] = new VS_INPUT(new SharpDX.Vector3(pos2.X, pos2.Y, baseZ - pos2.Y), new SharpDX.Vector2(0.0f, 1.0f));
            //vertices[2] = new VS_INPUT(new SharpDX.Vector3(pos3.X, pos3.Y, baseZ - pos3.Y), new SharpDX.Vector2(1.0f, 0.0f));
            //vertices[3] = new VS_INPUT(new SharpDX.Vector3(pos4.X, pos4.Y, baseZ - pos4.Y), new SharpDX.Vector2(1.0f, 1.0f));
        }

        public void SetLocalBuffer(Vector3 sourcePosition, Vector3 targetPosition)
        {
            Vector2 unitNormal = (sourcePosition.XY - targetPosition.XY).Normal.Direction;
            Vector2 normal = unitNormal * Scale.Y * TextureWidth / 2;

            float baseZ = sourcePosition.Z + sourcePosition.Y;
            Vector2 pos1 = sourcePosition.XY - normal;
            Vector2 pos2 = sourcePosition.XY + normal;
            Vector2 pos3 = targetPosition.XY - normal;
            Vector2 pos4 = targetPosition.XY + normal;

            vertices[0] = new VS_INPUT(new SharpDX.Vector3(pos1.X, pos1.Y, baseZ - pos1.Y), new SharpDX.Vector2(0.0f, 0.0f));
            vertices[2] = new VS_INPUT(new SharpDX.Vector3(pos2.X, pos2.Y, baseZ - pos2.Y), new SharpDX.Vector2(1.0f, 0.0f));
            vertices[1] = new VS_INPUT(new SharpDX.Vector3(pos3.X, pos3.Y, baseZ - pos3.Y), new SharpDX.Vector2(0.0f, 1.0f));
            vertices[3] = new VS_INPUT(new SharpDX.Vector3(pos4.X, pos4.Y, baseZ - pos4.Y), new SharpDX.Vector2(1.0f, 1.0f));
        }

        public void SetBuffer()
        {
            //using (FXGraphic.GetImmediateContextLock())
            //{
            //    FXGraphic.ImmediateContext.UpdateSubresource(vertices, VertexBuffer, 0);
            //}
            FXGraphic.ImmediateContext.UpdateSubresource(vertices, VertexBuffer, 0);

            //using (var surface = VertexBuffer.QueryInterface<Surface>())
            //{
            //    var map = surface.Map(SharpDX.DXGI.MapFlags.Write);
            //    PatcherYRpp.Pointer<VS_INPUT> ptr = map.DataPointer;

            //    for (int idx = 0; idx < vertices.Length; idx++)
            //    {
            //        ptr[idx] = vertices[idx];
            //    }

            //    surface.Unmap();
            //}
        }

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                if (_vertexBuffer.IsValueCreated)
                {
                    VertexBuffer.Dispose();
                }
                if (_indexBuffer.IsValueCreated)
                {
                    IndexBuffer.Dispose();
                }

                _vertexBuffer = null;
                _indexBuffer = null;
                _textureView = null;

                disposedValue = true;
            }
        }

        ~FXDrawObject()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
