//using Alea;
//using Alea.Parallel;
using Extension.FX.Definitions;
using Extension.FX.Graphic;
using PatcherYRpp;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Extension.FX
{
    public static class FXEngine
    {
        static FXEngine()
        {
            DynamicPatcher.Logger.LogWarning("FX feature is not finished yet, it may be unstable and imperfect.");
#if FX_USED3D
            InitializeGraphic();
#endif
        }

        #region Constant
        public static float DeltaTime = 1f / 60.0f; // 60fps
        #endregion

        #region Math
        public static float Clamp(float a, float min, float max)
        {
            if (a < min)
            {
                return min;
            }

            if (a > max)
            {
                return max;
            }

            return a;
        }

        private static Random _random = new Random(60);
        private static object _random_locker = new object();
        public static float CalculateRandomRange(float min = 0.0f, float max = 1.0f)
        {
            if (min == max)
            {
                return min;
            }

            lock (_random_locker)
            {
                float length = max - min;
                return min + (float)_random.NextDouble() * length;
            }
        }

        public static Vector3 CalculateRandomUnitVector()
        {
            lock (_random_locker)
            {
                const float r = 1;
                const float PI2 = (float)(Math.PI * 2);

                float azimuth = (float)(_random.NextDouble() * PI2);
                float elevation = (float)(_random.NextDouble() * PI2);

                return new Vector3(
                    (float)(r * Math.Cos(elevation) * Math.Cos(azimuth)),
                    (float)(r * Math.Cos(elevation) * Math.Sin(azimuth)),
                    (float)(r * Math.Sin(elevation))
                    );
            }
        }
        public static Vector3 CalculateRandomPointInSphere(float innerRadius, float outerRadius)
        {
            return CalculateRandomUnitVector() * CalculateRandomRange(innerRadius, outerRadius);
        }

        public static Vector3 CalculateRandomPointInBox(Vector3 size)
        {
            return new Vector3(
                CalculateRandomRange(0, size.X) - size.X / 2f,
                CalculateRandomRange(0, size.Y) - size.Y / 2f,
                CalculateRandomRange(0, size.Z) - size.Z / 2f
                );
        }

        public static Rotator FindLookAtRotation(Vector3 start, Vector3 target)
        {
            var lookAt = SharpDX.Quaternion.LookAtLH(
                new SharpDX.Vector3(start.Y, start.Z, start.X),
                new SharpDX.Vector3(target.Y, target.Z, target.X),
                SharpDX.Vector3.Up
                );

            lookAt.Normalize();
            lookAt.Conjugate();

            var rotator = lookAt.Axis * lookAt.Angle;

            return new Rotator(rotator.Z, rotator.X, rotator.Y);

            //Vector3 offset = target - start;
            //Vector2 xy = offset.XY;
            //Vector2 xz = offset.XZ;
            ////Vector2 yz = offset.YZ;

            //return new Rotator(0, (float)Math.Acos(offset.X / xz.Length), (float)Math.Acos(offset.X / xy.Length));
        }

        public static Vector3 GetForwardVector(Rotator rotator)
        {
            //var lookAt = SharpDX.Quaternion.RotationYawPitchRoll(rotator.Yaw, rotator.Pitch, rotator.Roll);

            //var forward = SharpDX.Vector3.Transform(SharpDX.Vector3.ForwardLH, lookAt);
            //forward.Normalize();

            //return new Vector3(forward.Z, forward.X, forward.Y);

            return new Vector3(
                 (float)(Math.Cos(rotator.Pitch) * Math.Cos(rotator.Yaw)),
                 (float)(Math.Cos(rotator.Pitch) * Math.Sin(rotator.Yaw)),
                 (float)(Math.Sin(rotator.Pitch))
                );
        }
        public static Vector3 GetUpVector(Rotator rotator)
        {
            var forward = GetForwardVector(rotator);

            var lookAt = SharpDX.Quaternion.RotationYawPitchRoll(0, (float)Math.PI / 2, 0);

            var up = SharpDX.Vector3.Transform(new SharpDX.Vector3(forward.X, forward.Y, forward.Z), lookAt);
            up.Normalize();

            return new Vector3(up.X, up.Y, up.Z);
        }
        public static Vector3 GetRightVector(Rotator rotator)
        {
            var forward = GetForwardVector(rotator);

            var lookAt = SharpDX.Quaternion.RotationYawPitchRoll((float)Math.PI / 2, 0, 0);

            var right = SharpDX.Vector3.Transform(new SharpDX.Vector3(forward.X, forward.Y, forward.Z), lookAt);
            right.Normalize();

            return new Vector3(right.X, right.Y, right.Z);
        }

        public static Vector2 XY(this Vector3 vector) => new Vector2(vector.X, vector.Y);
        public static Vector2 XZ(this Vector3 vector) => new Vector2(vector.X, vector.Z);
        public static Vector2 YZ(this Vector3 vector) => new Vector2(vector.Y, vector.Z);
        public static Vector2 Normalize(this Vector2 vector) => Vector2.Normalize(vector);
        public static Vector2 Direction(this Vector2 vector) => Vector2.Normalize(vector);
        public static Vector2 Normal(this Vector2 vector) => new Vector2(-vector.Y, vector.X);

        public static Vector3 Normalize(this Vector3 vector) => Vector3.Normalize(vector);
        public static Vector3 Direction(this Vector3 vector) => Vector3.Normalize(vector);
        #endregion

        #region Work
        public static bool EnableParallelSpawn { get; set; } = true;
        public static bool EnableParallelUpdate { get; set; } = true;
        public static bool EnableParallelRender { get; set; } = true;
        public static bool EnableParallel
        {
            get => EnableParallelSpawn && EnableParallelUpdate && EnableParallelRender;
            set => EnableParallelSpawn = EnableParallelUpdate = EnableParallelRender = value;
        }

        public static List<FXSystem> WorkSystems { get; private set; } = new List<FXSystem>();
        public static ReaderWriterLockSlim WorkListRWLock = new ReaderWriterLockSlim();
        public static bool Rendered { get; set; } = false;

        /// <summary>
        /// Add FXSystem to work list next frame. Adding when updating is allowed.
        /// </summary>
        public static void AddSystem(FXSystem system)
        {
            WorkListRWLock.EnterWriteLock();

            WorkSystems.Add(system);

            WorkListRWLock.ExitWriteLock();
        }
        /// <summary>
        /// Remove FXSystem from work list next frame. Removing when updating is allowed..
        /// </summary>
        public static void RemoveSystem(FXSystem system)
        {
            WorkSystems.Remove(system);
        }

        public static void Restart()
        {
            WorkSystems.Clear();
        }

        public static void Update()
        {
            try
            {
                WorkListRWLock.EnterWriteLock();

                var list = WorkSystems.ToArray();

                if (FXEngine.EnableParallelUpdate)
                {
                    Parallel.ForEach(list, system => system.Update());
                }
                else
                {
                    foreach (var system in list)
                    {
                        system.Update();
                    }
                }

                foreach (var system in list)
                {
                    switch (system.ExecutionState)
                    {
                        case FXExecutionState.InactiveClear:
                        case FXExecutionState.Complete:
                            RemoveSystem(system);
                            system.ExecutionState = FXExecutionState.Complete;
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                DynamicPatcher.Logger.PrintException(ex);
            }
            finally
            {
                WorkListRWLock.ExitWriteLock();
            }
        }
        public static void Render()
        {
            try
            {
#if FX_USED3D
                KeepScreenSize();
#endif

                WorkListRWLock.EnterReadLock();

                if (WorkSystems.Any())
                {
#if FX_USED3D
                    BeginDraw();
#endif

                    if (FXEngine.EnableParallelRender)
                    {
                        Parallel.ForEach(WorkSystems, system => system.Render());
                    }
                    else
                    {
                        foreach (var system in WorkSystems)
                        {
                            system.Render();
                        }
                    }

#if FX_USED3D
                    EndDraw();
#endif
                }
                else
                {
                    Rendered = false;
                }

            }
            catch (Exception ex)
            {
                DynamicPatcher.Logger.PrintException(ex);

                if (ex is SharpDX.SharpDXException gpuEx)
                {
                    var removedReason = FXGraphic.ImmediateContext.Device.DeviceRemovedReason;
                    DynamicPatcher.Logger.LogError($"DeviceRemovedReason: {removedReason}");
                }
            }
            finally
            {
                WorkListRWLock.ExitReadLock();
            }
        }
        public static void BeginDraw()
        {
#if FX_TOFIX
            ZBuffer.FillZTexture();
            
            YRGraphic.FillTexture();
            FXGraphic.DrawObject(YRGraphic.drawObject);
#endif
            //FXGraphic.BeginDraw();
            YRGraphic.BeginDraw();
        }
        public static void EndDraw()
        {
            //FXGraphic.EndDraw();
            YRGraphic.EndDraw();
#if FX_TOFIX
            FXGraphic.Render();

            Rendered = true;

            Present();
#endif
        }
        public static int Present()
        {
            return FXGraphic.Present().Code;
        }
        public static void OnPresent(IntPtr pDevice)
        {
            D3D9.OnPresent((SharpDX.Direct3D9.Device)pDevice);
        }
#endregion

#region Graphic
        //public static bool HasGPU => Device.Devices.Any();
        public static bool HasGPU => true;
        public static void InitializeGraphic()
        {
            if (HasGPU)
            {
                //PrepareAlea();
            }

            KeepScreenSize();
        }

        private static int _screenSize;
        private static void KeepScreenSize()
        {
            var rect = YRGraphic.SurfaceRect;
            int screenSize = rect.Width * rect.Height;
            if (screenSize != _screenSize)
            {
                _screenSize = screenSize;
                ResizeScreen();
            }
        }

        private static void ResizeScreen()
        {
            try
            {
                WorkListRWLock.EnterWriteLock();

                var list = WorkSystems.ToArray();
                WorkSystems = (from s in list select s.Clone()).ToList();
            }
            catch (Exception ex)
            {
                DynamicPatcher.Logger.PrintException(ex);
            }
            finally
            {
                WorkListRWLock.ExitWriteLock();
            }

            FXGraphic.Dispose();

            FXGraphic.Initialize();

            GC.Collect();
        }

        public static bool InScreen(Vector3 worldPosition)
        {
            var coords = new CoordStruct((int)worldPosition.X, (int)worldPosition.Y, (int)worldPosition.Z);
            var clientPoint = TacticalClass.Instance.Ref.CoordsToClient(coords);
            return clientPoint.X >= 0 && clientPoint.Y >= 0 && clientPoint.X <= YRGraphic.ZBuffer.Width && clientPoint.Y <= YRGraphic.ZBuffer.Height;
        }

        public static Vector3 WorldToClient(Vector3 worldPosition)
        {
            var coords = new CoordStruct((int)worldPosition.X, (int)worldPosition.Y, (int)worldPosition.Z);
            var clientPoint = TacticalClass.Instance.Ref.CoordsToClient(coords);

            float currentBaseZ = YRGraphic.ZBuffer.Rect.Y + (short)YRGraphic.ZBuffer.CurrentBaseZ;

            float z = -clientPoint.Y + currentBaseZ + TacticalClass.Instance.Ref.AdjustForZ(coords.Z);

            return new Vector3(clientPoint.X, clientPoint.Y, z);
        }


        public static void DrawTexture(ShaderResourceView texture, Vector3 worldPosition, Vector3 rotation, Vector3 scale)
        {
            var coords = new CoordStruct((int)worldPosition.X, (int)worldPosition.Y, (int)worldPosition.Z);
            var clientPoint = TacticalClass.Instance.Ref.CoordsToClient(coords);

            var clientRotation = new Vector2();
            var clientScale = new Vector2(1,1);

            FXGraphic.DrawTexture(texture, new Vector3(clientPoint.X, clientPoint.Y, 0), clientRotation, clientScale);
        }

        public static void DrawTexture(ShaderResourceView texture, Vector3 worldPosition, Vector2 rotation, Vector2 scale)
        {
            var coords = new CoordStruct((int)worldPosition.X, (int)worldPosition.Y, (int)worldPosition.Z);
            var clientPoint = TacticalClass.Instance.Ref.CoordsToClient(coords);

            FXGraphic.DrawTexture(texture, new Vector3(clientPoint.X, clientPoint.Y, 0), rotation, scale);
        }

        //public static void PrepareAlea()
        //{
        //    Device.PrintAll();

        //    Action<string, string> copyDir = (src, dest) =>
        //    {
        //        var files = Directory.GetFiles(src);
        //        foreach (var file in files)
        //        {
        //            Directory.CreateDirectory(Path.Combine(dest, Path.GetFileName(src)));
        //            File.Copy(file, Path.Combine(dest, Path.GetFileName(src), Path.GetFileName(file)), true);
        //        }
        //    };
        //    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        //    {
        //        if (string.Equals(assembly.GetName().Name, "Alea", StringComparison.OrdinalIgnoreCase))
        //        {
        //            string aleaDir = Path.GetDirectoryName(assembly.Location);
        //            string gameDir = Directory.GetCurrentDirectory();

        //            var dirs = Directory.GetDirectories(aleaDir);
        //            foreach (var dir in dirs)
        //            {
        //                copyDir(dir, gameDir);
        //            }

        //            break;
        //        }
        //    }
        //}

#endregion
    }
}
