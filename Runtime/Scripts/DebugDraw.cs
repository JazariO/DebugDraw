using UnityEngine;
using System;

namespace Proselyte.DebugDrawer
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

    public class DebugDraw : MonoBehaviour
    {
        private static DebugDraw _instance;
        public static DebugDraw Instance
        {
            get
            {
                if(_instance == null)
                {
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
            if(_meshDrawer == null)
            {
                Debug.LogError("DebugDraw or DebugDrawer is not initialized.");
                return;
            }

            action?.Invoke();
        }

        void Awake()
        {
            if(_instance == null)
            {
                _instance = this;
                _meshDrawer = new DebugMeshDrawer(this);
            }
            else if(_instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Update()
        {
            _meshDrawer.Update();
        }

        void LateUpdate()
        {
            // Also update in LateUpdate to catch FixedUpdate drawings
            _meshDrawer.Update();
        }

        void OnDestroy()
        {
            if(_instance == this)
            {
                _meshDrawer?.Cleanup();
                _meshDrawer = null;
                _instance = null;
            }
        }

        [System.Diagnostics.Conditional("DEBUG_DRAW")]
        public static void SetDrawingDepthTestEnabled(bool enabled)
        {
            if(enabled != _doDepthTest)
            {
                _doDepthTest = enabled;
                _meshDrawer?.SetDepthTestEnabled(_doDepthTest);
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
            if(enabled)
            {
                _enabledLayers |= layer;
            }
            else
            {
                _enabledLayers &= ~layer;
            }
            OnDrawSettingsUpdated?.Invoke();
        }

        public static bool GetDoDepthTest() => _doDepthTest;
        public static uint GetEnabledLayers() => _enabledLayers;
        internal static int GetMaxPoolSize() => _maxPoolSize;
        internal static int GetStartingPoolSize() => _startingPoolSize;

        #region Drawing Methods

        /// <summary>
        /// Draws a runtime debug line.
        /// </summary>
        /// <param name="start">The starting position of the line.</param>
        /// <param name="end">The ending position of the line.</param>
        /// <param name="color">Color for the line.</param>
        /// <param name="duration">How long the line persists.</param>
        /// <param name="layers">Which debug layers to draw on.</param>
        /// <param name="fromFixedUpdate">Set to true when calling from FixedUpdate.</param>
        [System.Diagnostics.Conditional("DEBUG_DRAW")]
        public static void Line(Vector3 start, Vector3 end, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1, bool fromFixedUpdate = false)
        {
            InvokeWithInit(() =>
            {
                Vector3 delta = end - start;
                float distance = delta.magnitude;

                // Skip zero-length lines
                if(distance < 0.0001f)
                {
                    return;
                }

                Vector3 direction = delta / distance;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, direction);
                Matrix4x4 transform = Matrix4x4.TRS(start, rotation, Vector3.one * distance);
                DebugDraw._meshDrawer.DrawLine(transform, duration, color ?? Color.white, layers, 1, fromFixedUpdate);
            });
        }

        /// <summary>
        /// Draws a wireframe quad.
        /// </summary>
        /// <param name="position">Center position of the quad.</param>
        /// <param name="normal">Normal vector that the quad should face.</param>
        /// <param name="scale">Scale of the quad.</param>
        [System.Diagnostics.Conditional("DEBUG_DRAW")]
        public static void WireQuad(Vector3 position, Vector3 normal, Vector3 scale, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1, bool fromFixedUpdate = false)
        {
            InvokeWithInit(() =>
            {
                // Convert normal vector to rotation
                Quaternion rotation = Quaternion.LookRotation(normal);
                Matrix4x4 transform = Matrix4x4.TRS(position, rotation, scale);
                DebugDraw._meshDrawer.DrawWireQuad(transform, duration, color ?? Color.white, layers, 1, fromFixedUpdate);
            });
        }

        /// <summary>
        /// Draws a filled quad.
        /// </summary>
        /// <param name="position">Center position of the quad.</param>
        /// <param name="normal">Normal vector that the quad should face.</param>
        /// <param name="scale">Scale of the quad.</param>
        [System.Diagnostics.Conditional("DEBUG_DRAW")]
        public static void Quad(Vector3 position, Vector3 normal, Vector3 scale, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1, bool fromFixedUpdate = false)
        {
            InvokeWithInit(() =>
            {
                // Convert normal vector to rotation
                Quaternion rotation = Quaternion.LookRotation(normal);
                Matrix4x4 transform = Matrix4x4.TRS(position, rotation, scale);
                DebugDraw._meshDrawer.DrawQuad(transform, duration, color ?? Color.white, layers, 1, 1, fromFixedUpdate);
            });
        }

        [System.Diagnostics.Conditional("DEBUG_DRAW")]
        public static void Box(Vector3 position, Quaternion rotation, Vector3 scale, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1, bool fromFixedUpdate = false)
        {
            InvokeWithInit(() =>
            {
                Matrix4x4 transform = Matrix4x4.TRS(position, rotation, scale);
                DebugDraw._meshDrawer.DrawBox(transform, duration, color ?? Color.white, layers, 1, 1, fromFixedUpdate);
            });
        }

        [System.Diagnostics.Conditional("DEBUG_DRAW")]
        public static void Box(Matrix4x4 transform, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1, bool fromFixedUpdate = false)
        {
            InvokeWithInit(() =>
            {
                DebugDraw._meshDrawer.DrawBox(transform, duration, color ?? Color.white, layers, 1, 1, fromFixedUpdate);
            });
        }

        [System.Diagnostics.Conditional("DEBUG_DRAW")]
        public static void Sphere(Vector3 position, Quaternion rotation, float radius = 1f, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1, bool fromFixedUpdate = false)
        {
            InvokeWithInit(() =>
            {
                Matrix4x4 transform = Matrix4x4.TRS(position, rotation, Vector3.one * radius * 2f);
                DebugDraw._meshDrawer.DrawSphere(transform, duration, color ?? Color.white, layers, 1, fromFixedUpdate);
            });
        }

        [System.Diagnostics.Conditional("DEBUG_DRAW")]
        public static void Sphere(Matrix4x4 transform, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1, bool fromFixedUpdate = false)
        {
            InvokeWithInit(() =>
            {
                DebugDraw._meshDrawer.DrawSphere(transform, duration, color ?? Color.white, layers, 1, fromFixedUpdate);
            });
        }

        [System.Diagnostics.Conditional("DEBUG_DRAW")]
        public static void WireSphere(Vector3 position, Quaternion rotation, float radius = 1f, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1, bool fromFixedUpdate = false)
        {
            InvokeWithInit(() =>
            {
                Matrix4x4 transform = Matrix4x4.TRS(position, rotation, Vector3.one * radius * 2f);
                DebugDraw._meshDrawer.DrawWireSphere(transform, duration, color ?? Color.white, layers, 1, fromFixedUpdate);
            });
        }

        /// <summary>
        /// Draws a wireframe arrow.
        /// </summary>
        /// <param name="start">Start position of the arrow.</param>
        /// <param name="end">End position of the arrow (arrow points here).</param>
        /// <param name="up">Up direction for the arrow orientation.</param>
        /// <param name="arrowHeadSize">Size of the arrowhead.</param>
        [System.Diagnostics.Conditional("DEBUG_DRAW")]
        public static void WireArrow(Vector3 start, Vector3 end, Vector3 up, float arrowHeadSize = 0.25f, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1, bool fromFixedUpdate = false)
        {
            InvokeWithInit(() =>
            {
                Vector3 delta = end - start;
                float distance = delta.magnitude;

                // Early exit if zero length
                if(distance < 0.0001f)
                { 
                    return;
                }

                Vector3 direction = delta / distance;
                Quaternion rotation = Quaternion.LookRotation(direction, up);

                Matrix4x4 transform = Matrix4x4.TRS(start, rotation, new Vector3(arrowHeadSize, arrowHeadSize, distance));
                DebugDraw._meshDrawer.DrawWireArrow(transform, duration, color ?? Color.white, layers, fromFixedUpdate);
            });
        }

        /// <summary>
        /// Draws a filled arrow.
        /// </summary>
        /// <param name="start">Start position of the arrow.</param>
        /// <param name="end">End position of the arrow (arrow points here).</param>
        /// <param name="up">Up direction for the arrow orientation.</param>
        /// <param name="arrowHeadSize">Size of the arrowhead.</param>
        [System.Diagnostics.Conditional("DEBUG_DRAW")]
        public static void Arrow(Vector3 start, Vector3 end, Vector3 up, float arrowHeadSize = 0.25f, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1, bool fromFixedUpdate = false)
        {
            InvokeWithInit(() =>
            {
                Vector3 delta = end - start;
                float distance = delta.magnitude;

                // Early exit if zero length
                if(distance < 0.0001f)
                {
                    Debug.LogWarning("DebugDraw.Arrow: start and end positions are too close");
                    return;
                }

                Vector3 direction = delta / distance;
                Quaternion rotation = Quaternion.LookRotation(direction, up);

                // Draw the line portion
                DebugDraw.Line(start, end, color, duration, layers, fromFixedUpdate);

                // Draw the arrowhead
                Matrix4x4 transform = Matrix4x4.TRS(start, rotation, new Vector3(arrowHeadSize, arrowHeadSize, distance));
                DebugDraw._meshDrawer.DrawArrow(transform, duration, color ?? Color.white, layers, fromFixedUpdate);
            });
        }

        /// <summary>
        /// Draws a wireframe capsule between two points.
        /// </summary>
        /// <param name="point1">First endpoint of the capsule.</param>
        /// <param name="point2">Second endpoint of the capsule.</param>
        /// <param name="radius">Radius of the capsule.</param>
        [System.Diagnostics.Conditional("DEBUG_DRAW")]
        public static void WireCapsule(Vector3 point1, Vector3 point2, float radius = 1f, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1, bool fromFixedUpdate = false)
        {
            InvokeWithInit(() =>
            {
                Vector3 delta = point2 - point1;
                float cylinderHeight = delta.magnitude;

                // Handle degenerate case
                if(cylinderHeight < 0.0001f)
                {
                    // Just draw a sphere if points are the same
                    WireSphere(point1, Quaternion.identity, radius, color, duration, layers, fromFixedUpdate);
                    return;
                }

                float totalHeight = cylinderHeight + radius * 2f;
                Vector3 centre = point1 + delta * 0.5f;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, delta / cylinderHeight);

                Matrix4x4 transform = Matrix4x4.TRS(centre, rotation, Vector3.one);
                DebugDraw._meshDrawer.DrawWireCapsule(transform, duration, color ?? Color.white, layers, totalHeight, radius, fromFixedUpdate);
            });
        }

        #endregion
    }
}