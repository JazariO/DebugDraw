using UnityEngine;
using System;
using System.Collections.Generic;

namespace DebugDrawer
{
    [Flags]
    public enum DebugLayers : uint
    {
        None = 0,
        Layer1 = 1 << 1,
        Layer2 = 1 << 2,
        Layer3 = 1 << 3,
        Layer4 = 1 << 4,
        Layer5 = 1 << 5,
        Layer6 = 1 << 6,
        Layer7 = 1 << 7,
        Layer8 = 1 << 8,
        All = Layer1 | Layer2 | Layer3 | Layer4 | Layer5 | Layer6 | Layer7 | Layer8
    }

    /// <summary>
    /// Main DebugDraw API
    /// </summary>
    public class DebugDraw : MonoBehaviour
    {
        // Singleton instance
        private static DebugDraw _instance;
        public static DebugDraw Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Create a new GameObject and attach this script
                    GameObject go = new GameObject("DebugDraw");
                    _instance = go.AddComponent<DebugDraw>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private static DebugMeshDrawer _meshDrawer;
        private static uint _enabledLayers = (uint)DebugLayers.All;
        private static bool _doDepthTest = true;
        private static int _maxPoolSize = 1024;
        private static int _startingPoolSize = 256;

        internal static Action OnDrawSettingsUpdated;

        private static void InvokeWithInit(Action action)
        {
            if(_instance == null)
            {
                _ = Instance;
            }
            if (_meshDrawer == null)
            {
                Debug.LogError("DebugDraw or DebugDrawer is not initialized.");
                return;
            }

            action?.Invoke();
        }

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                _meshDrawer = new DebugMeshDrawer(this);
            } else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Update()
        {
            // Update and render debug shapes
            _meshDrawer.Update();
        }

        [System.Diagnostics.Conditional("DEBUG_DRAW")]
        public static void SetDrawingDepthTestEnabled(bool enabled)
        {
            if (enabled != _doDepthTest)
            {
                _doDepthTest = enabled;
                _meshDrawer.SetDepthTestEnabled(_doDepthTest);
                OnDrawSettingsUpdated?.Invoke();
            }
        }

        [System.Diagnostics.Conditional("DEBUG_DRAW")]
        public static void SetEnabledLayers(uint layers)
        {
            _enabledLayers = layers;
            OnDrawSettingsUpdated?.Invoke();
        }

        [System.Diagnostics.Conditional("DEBUG_DRAW")]
        public static void SetLayerEnabled(uint layer, bool enabled)
        {
            if (enabled)
            {
                _enabledLayers |= layer;
            } else
            {
                _enabledLayers &= ~layer;
            }
            OnDrawSettingsUpdated?.Invoke();
        }

        internal static bool GetDoDepthTest()=> _doDepthTest;
        internal static uint GetEnabledLayers() => _enabledLayers;
        internal static int GetMaxPoolSize() => _maxPoolSize;
        internal static int GetStartingPoolSize() => _startingPoolSize;

        ///////////////////////////////////
        // Static methods to draw shapes //
        ///////////////////////////////////
        #region Drawing Methods
        
        /// <summary>
        /// Draws a runtime debug line.
        /// </summary>
        /// <param name="start">The starting position of the line.</param>
        /// <param name="end">The ending position of the line.</param>
        /// <param name="color">Color for the line.</param>
        public static void Line(Vector3 start, Vector3 end, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1)
        {
            InvokeWithInit(() =>
            {
                float distance = (end - start).magnitude;
                Vector3 direction = (end - start).normalized;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, (end - start).normalized);
                Matrix4x4 transform = Matrix4x4.TRS(start, rotation, Vector3.one * distance);
                DebugDraw._meshDrawer.DrawLine(transform, duration, color ?? Color.white, layers, 1);
            });
        }

        public static void WireQuad(Vector3 position, Quaternion normal, Vector3 scale, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1)
        {
            InvokeWithInit(() =>
            {
                Matrix4x4 transform = Matrix4x4.TRS(position, normal, scale);
                DebugDraw._meshDrawer.DrawWireQuad(transform, duration, color ?? Color.white, layers, 1);
            });
        }

        /// <summary>
        /// Draws a runtime box shape.
        /// </summary>
        public static void Box(Vector3 position, Quaternion rotation, Vector3 scale, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1)
        {
            InvokeWithInit (() =>
            {
                Matrix4x4 transform = Matrix4x4.TRS(position, rotation, scale);
                DebugDraw._meshDrawer.DrawBox(transform, duration, color ?? Color.white, layers);
            });
        }

        public static void Box(Matrix4x4 transform, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1)
        {
            InvokeWithInit(() =>
            {
                DebugDraw._meshDrawer.DrawBox(transform, duration, color ?? Color.white, layers);
            });
        }

        public static void Sphere(Vector3 position, Quaternion rotation, float radius = 1f, Color? color = null, float duration = 0f,  uint layers = (uint)DebugLayers.Layer1)
        {
            InvokeWithInit(() =>
            {
                Matrix4x4 transform = Matrix4x4.TRS(position, rotation, Vector3.one * radius * 2f);
                DebugDraw._meshDrawer.DrawSphere(transform, duration, color ?? Color.white, layers);
            });
        }

        public static void Sphere(Matrix4x4 transform, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1)
        {
            InvokeWithInit(() =>
            {
                DebugDraw._meshDrawer.DrawSphere(transform, duration, color ?? Color.white, layers);
            });
        }

        public static void WireSphere(Vector3 position, Quaternion rotation, float radius = 1f, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1)
        {
            InvokeWithInit(() =>
            {
                Matrix4x4 transform = Matrix4x4.TRS(position, rotation, Vector3.one * radius * 2f);
                DebugDraw._meshDrawer.DrawWireSphere(transform, duration, color ?? Color.white, layers);
            });
        }

        public static void WireArrow(Vector3 start, Vector3 end, Vector3 up, float arrowLength = 1f, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1)
        {
            InvokeWithInit(() =>
            {
                float distance = (end - start).magnitude;
                Vector3 direction = (end - start).normalized;

                Quaternion rotation = Quaternion.LookRotation(direction, up);
                
                Matrix4x4 transform = Matrix4x4.TRS(start, rotation, new Vector3(1, 1, distance));
                DebugDraw._meshDrawer.DrawArrow(transform, duration, color ?? Color.white, layers, arrowLength);
            });
        }

        // Implement other shapes (Cylinder, Capsule, etc.) similarly

        #endregion
    }
}