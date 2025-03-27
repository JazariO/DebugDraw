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

        public static void WireArrow(Vector3 start, Vector3 end, float arrowLength = 1f, Color? color = null, float duration = 0f, uint layers = (uint)DebugLayers.Layer1)
        {
            InvokeWithInit(() =>
            {
                float distance = (end - start).magnitude;
                Vector3 direction = (end - start).normalized;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, (end - start).normalized);
                Matrix4x4 transform = Matrix4x4.TRS(start, rotation, new Vector3(1,1, distance));
                DebugDraw._meshDrawer.DrawArrow(transform, duration, color ?? Color.white, layers, arrowLength);
            });
        }

        // Implement other shapes (Cylinder, Capsule, etc.) similarly

        #endregion
    }

    /// <summary>
    /// Handles all collections and update logic
    /// </summary>
    internal class DebugMeshDrawer
    {
        private readonly DebugDraw _parent;

        internal readonly ObjectPool<DrawMeshInstance> MeshPool;
        private readonly List<DebugMeshCollection> _collections = new List<DebugMeshCollection>();

        // Collections for different shapes
        private readonly DebugMeshCollection _lineCollection;
        private readonly DebugMeshCollection _boxCollection;
        private readonly DebugMeshCollection _sphereCollection;
        private readonly DebugMeshCollection _wireSphereCollection;
        private readonly DebugMeshCollection _arrowCollection;
        // Add other collections as needed

        internal DebugMeshDrawer(DebugDraw parent)
        {
            _parent = parent;

            MeshPool = new ObjectPool<DrawMeshInstance>(DebugDraw.GetMaxPoolSize(), DebugDraw.GetStartingPoolSize());

            // Create materials
            Material wireframeMaterial = CreateDefaultMaterial();

            // Create meshes
            Mesh lineMesh = DebugMeshes.Construct(DebugShape.Line);
            Mesh boxMesh = DebugMeshes.Construct(DebugShape.Cube);
            Mesh sphereMesh = DebugMeshes.Construct(DebugShape.Sphere);
            Mesh wireSphereMesh = DebugMeshes.Construct(DebugShape.WireSphere);
            Mesh arrowMesh = DebugMeshes.Construct(DebugShape.Arrow);
            // Create other meshes as needed

            // Create collections
            _lineCollection = new DebugMeshCollection(lineMesh, wireframeMaterial);
            _boxCollection = new DebugMeshCollection(boxMesh, wireframeMaterial);
            _sphereCollection = new DebugMeshCollection(sphereMesh, wireframeMaterial);
            _wireSphereCollection = new DebugMeshCollection(wireSphereMesh, wireframeMaterial);
            _arrowCollection = new DebugMeshCollection(arrowMesh, wireframeMaterial);
            // Add other collections as needed

            _collections.Add(_lineCollection);
            _collections.Add(_boxCollection);
            _collections.Add(_sphereCollection);
            _collections.Add(_wireSphereCollection);
            _collections.Add(_arrowCollection);
            // Add other collections as needed

            DebugMeshCollection.OnInstanceRemoved += inst => MeshPool.Return(inst);
        }

        private Material CreateDefaultMaterial()
        {
            Shader shader = Shader.Find("DebugDrawer/DebugDraw");
            if(shader == null)
            {
                Debug.LogError("Shader not found. Ensure the shader is included in the project.");
                shader = Shader.Find("Unlit/Color");
            }
            Material material = new Material(shader);
            material.enableInstancing = true;

            // Set default values
            material.SetFloat("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
            material.SetFloat("_ZWrite", 1);

            return material;
        }

        private DrawMeshInstance GetAMeshInstance(Matrix4x4 transform, float duration, Color color, uint layers, float length = 1f)
        {
            DrawMeshInstance inst = MeshPool.Retrieve();
            if(inst != null)
            {
                inst.Transform = transform;
                inst.length = length;
                inst.SetDuration(duration);
                inst.Color = color;
                inst.DrawLayers = layers;
            }
            return inst;
        }

        internal void Update()
        {
            foreach(var collection in _collections)
            {
                collection.Update();
            }
        }

        internal void SetDepthTestEnabled(bool doDepthTest)
        {
            foreach(var collection in _collections)
            {
                Material material = collection.Material;
                if(doDepthTest)
                {
                    material.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    material.SetFloat("_ZWrite", 1);
                }
                else
                {
                    material.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    material.SetFloat("_ZWrite", 0);
                }
            }
        }

        #region Drawing Methods

        internal void DrawLine(Matrix4x4 transform, float duration, Color color, uint layers, float lineLength)
        {
            if((DebugDraw.GetEnabledLayers() & layers) == 0)
                return;

            var instance = GetAMeshInstance(transform, duration, color, layers, lineLength);
            _lineCollection.Add(instance);
        }

        internal void DrawBox(Matrix4x4 transform, float duration, Color color, uint layers)
        {
            if((DebugDraw.GetEnabledLayers() & layers) == 0)
                return;

            var instance = GetAMeshInstance(transform, duration, color, layers);
            _boxCollection.Add(instance);
        }

        internal void DrawSphere(Matrix4x4 transform, float duration, Color color, uint layers)
        {
            if((DebugDraw.GetEnabledLayers() & layers) == 0)
                return;

            var instance = GetAMeshInstance(transform, duration, color, layers);
            _sphereCollection.Add(instance);
        }

        internal void DrawWireSphere(Matrix4x4 transform, float duration, Color color, uint layers)
        {
            if((DebugDraw.GetEnabledLayers() & layers) == 0)
                return;

            var instance = GetAMeshInstance(transform, duration, color, layers);
            _wireSphereCollection.Add(instance);
        }

        internal void DrawArrow(Matrix4x4 transform, float duration, Color color, uint layers, float arrowLength)
        {
            if((DebugDraw.GetEnabledLayers() & layers) == 0) 
                return;

            // Create a unique arrow mesh for the given arrowLength
            Mesh customArrowMesh = DebugMeshes.Construct(DebugShape.Arrow, arrowLength);

            // Create a new collection for this custom arrow mesh if it doesn't exist
            DebugMeshCollection customArrowCollection = new DebugMeshCollection(customArrowMesh, _arrowCollection.Material);

            // Create a mesh instance and add it to the custom collection
            var instance = GetAMeshInstance(transform, duration, color, layers, arrowLength);
            customArrowCollection.Add(instance);

            // Temporarily store the custom collection in a list to update it in the Update method
            _collections.Add(customArrowCollection);
        }

        // Implement other shapes similarly

        #endregion
    }

    /// <summary>
    /// Collection which handles a single type of shape collection e.g., all Box meshes
    /// </summary>
    internal class DebugMeshCollection
    {
        internal Mesh Mesh { get; }
        internal Material Material { get; }
        private readonly List<DrawMeshInstance> _drawInstances = new List<DrawMeshInstance>();

        internal static Action<DrawMeshInstance> OnInstanceRemoved;

        internal DebugMeshCollection(Mesh mesh, Material material)
        {
            Mesh = mesh;
            Material = material;
        }

        internal void Update()
        {
            // Remove expired instances
            for (int i = _drawInstances.Count - 1; i >= 0; i--)
            {
                if (_drawInstances[i].IsExpired())
                {
                    OnInstanceRemoved?.Invoke(_drawInstances[i]);
                    _drawInstances.RemoveAt(i);
                }
            }

            if (_drawInstances.Count == 0)
            {
                return;
            }

            // Prepare matrices and colors
            int instanceCount = _drawInstances.Count;
            Matrix4x4[] matrices = new Matrix4x4[instanceCount];
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            Vector4[] colors = new Vector4[instanceCount];

            for (int i = 0; i < instanceCount; i++)
            {
                matrices[i] = _drawInstances[i].Transform;
                colors[i] = _drawInstances[i].Color;
                _drawInstances[i].BeenDrawn = true;
            }

            // Set per-instance colors
            propertyBlock.SetVectorArray("_Color", colors);

            // Draw instances
            Graphics.DrawMeshInstanced(
                Mesh, 
                0, 
                Material, 
                matrices, 
                instanceCount, 
                propertyBlock, 
                UnityEngine.Rendering.ShadowCastingMode.Off,    // disables casting shadows
                false                                           // disables receiving shadows
            );
        }

        internal void Add(DrawMeshInstance instance)
        {
            if (instance != null)
            {
                _drawInstances.Add(instance);
            }
        }
    }

    internal interface IPoolable
    {
        void Reset();
    }

    internal class ObjectPool<T> where T : IPoolable, new()
    {
        internal readonly int MaxSize;
        internal int CurrentSize;
        internal int FreeObjects;
        private readonly Queue<T> _pool;

        internal ObjectPool(int maxSize, int startingSize)
        {
            _pool = new Queue<T>();
            MaxSize = maxSize;
            ExpandPool(startingSize);
        }

        internal T Retrieve()
        {
            if (FreeObjects == 0 && !ExpandPool(1))
            {
                Debug.LogWarning($"{typeof(T)} pool has no free objects, consider increasing max size");
                return default;
            }
            FreeObjects--;
            return _pool.Dequeue();
        }

        internal bool ExpandPool(int expansion)
        {
            expansion = Mathf.Min(expansion, MaxSize - CurrentSize);
            if (expansion == 0)
            {
                return false;
            }
            for (int i = 0; i < expansion; i++)
            {
                _pool.Enqueue(new T());
            }
            FreeObjects += expansion;
            CurrentSize += expansion;
            return true;
        }

        internal void Return(T obj)
        {
            obj.Reset();
            _pool.Enqueue(obj);
            FreeObjects++;
        }
    }

    /// <summary>
    /// Base class for all drawable instances
    /// </summary>
    internal class DrawInstance : IPoolable
    {
        internal Color Color;
        internal bool BeenDrawn;
        protected float ExpirationTime;
        internal uint DrawLayers;

        internal void SetDuration(float duration)
        {
            ExpirationTime = Time.time + duration;
            BeenDrawn = false;
        }

        internal virtual bool IsExpired()
        {
            return (Time.time > ExpirationTime && BeenDrawn);
        }

        public virtual void Reset()
        {
            BeenDrawn = false;
            Color = default;
            ExpirationTime = 0f;
            DrawLayers = 0;
        }
    }

    /// <summary>
    /// Specialized instance for meshes
    /// </summary>
    internal class DrawMeshInstance : DrawInstance
    {
        internal Matrix4x4 Transform;
        internal float length;

        public override void Reset()
        {
            base.Reset();
            Transform = Matrix4x4.identity;
            length = 0.2f;
        }
    }

    /// <summary>
    /// Mesh Construction Methods
    /// </summary>
    internal static class DebugMeshes
    {
        // Implement mesh construction methods for various shapes
        internal static Mesh Construct(DebugShape shape, float length = 1)
        {
            Mesh mesh = new Mesh();

            switch (shape)
            {
                case DebugShape.Line:
                    mesh = CreateLineMesh(length);
                    break;
                case DebugShape.Cube:
                    mesh = CreateCubeMesh();
                    break;
                case DebugShape.Sphere:
                    mesh = CreateSphereMesh(16, 16);
                    break;
                case DebugShape.WireSphere:
                    mesh = CreateWireSphereMesh(16, 16);
                    break;
                case DebugShape.Arrow:
                    mesh = CreateArrowMesh(length);
                    break;
                // Implement other shapes

                default:
                    Debug.LogError($"DebugShape {shape} not implemented");
                    break;
            }
            return mesh;
        }

        private static Mesh CreateLineMesh(float lineLength)
        {
            Mesh mesh = new Mesh();

            // Define vertices and indices for a line
            Vector3[] vertices =
            {
                new (0, 0, 0),
                new (0, 0, Mathf.Clamp(lineLength, 0, Mathf.Infinity))
            };

            int[] indices =
            {
                0,1
            };

            mesh.vertices = vertices;
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            return mesh;
        }

        private static Mesh CreateCubeMesh()
        {
            Mesh mesh = new();

            // Define vertices and indices for a cube
            Vector3[] vertices = 
            {
                new (-0.5f, -0.5f, -0.5f),
                new ( 0.5f, -0.5f, -0.5f),
                new ( 0.5f,  0.5f, -0.5f),
                new (-0.5f,  0.5f, -0.5f),
                new (-0.5f, -0.5f,  0.5f),
                new ( 0.5f, -0.5f,  0.5f),
                new ( 0.5f,  0.5f,  0.5f),
                new (-0.5f,  0.5f,  0.5f)
            };

            int[] indices = 
            {
                // Lines
                0,1, 1,2, 2,3, 3,0, // Bottom face
                4,5, 5,6, 6,7, 7,4, // Top face
                0,4, 1,5, 2,6, 3,7  // Sides
            };

            mesh.vertices = vertices;
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateSphereMesh(int longitudeSegments, int latitudeSegments)
        {
            // Implement sphere mesh generation
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            float radius = 0.5f;
            for (int lat = 0; lat <= latitudeSegments; lat++)
            {
                float theta = lat * Mathf.PI / latitudeSegments;
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);

                for (int lon = 0; lon <= longitudeSegments; lon++)
                {
                    float phi = lon * 2 * Mathf.PI / longitudeSegments;
                    float sinPhi = Mathf.Sin(phi);
                    float cosPhi = Mathf.Cos(phi);

                    Vector3 vertex = new Vector3(
                        cosPhi * sinTheta,
                        cosTheta,
                        sinPhi * sinTheta
                    ) * radius;
                    vertices.Add(vertex);
                }
            }

            for (int lat = 0; lat < latitudeSegments; lat++)
            {
                for (int lon = 0; lon < longitudeSegments; lon++)
                {
                    int first = (lat * (longitudeSegments + 1)) + lon;
                    int second = first + longitudeSegments + 1;

                    // Triangles
                    indices.Add(first);
                    indices.Add(second);
                    indices.Add(first + 1);

                    indices.Add(second);
                    indices.Add(second + 1);
                    indices.Add(first + 1);
                }
            }

            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateWireSphereMesh(int longitudeSegments, int latitudeSegments)
        {
            // Implement wire sphere mesh generation
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            float radius = 0.5f;
            for(int lat = 0; lat <= latitudeSegments; lat++)
            {
                float theta = lat * Mathf.PI / latitudeSegments;
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);

                for(int lon = 0; lon <= longitudeSegments; lon++)
                {
                    float phi = lon * 2 * Mathf.PI / longitudeSegments;
                    float sinPhi = Mathf.Sin(phi);
                    float cosPhi = Mathf.Cos(phi);

                    Vector3 vertex = new Vector3(
                        cosPhi * sinTheta,
                        cosTheta,
                        sinPhi * sinTheta
                    ) * radius;
                    vertices.Add(vertex);
                }
            }

            for(int lat = 0; lat < latitudeSegments; lat++)
            {
                for(int lon = 0; lon < longitudeSegments; lon++)
                {
                    int first = (lat * (longitudeSegments + 1)) + lon;
                    int second = first + longitudeSegments + 1;

                    // Lines
                    indices.Add(first);
                    indices.Add(second);

                    indices.Add(second);
                    indices.Add(second + 1);

                    indices.Add(second + 1);
                    indices.Add(first + 1);

                    indices.Add(first + 1);
                    indices.Add(first);
                }
            }

            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary>
        /// Returns an arrow mesh.
        /// </summary>
        /// <param name="arrowLength">The total length of the arrow. Tip to tail.</param>
        /// <returns></returns>
        private static Mesh CreateArrowMesh(float arrowLength)
        {
            Mesh mesh = new();

            // Base Position
            float bPos = Mathf.Clamp(arrowLength - 0.225f, 0, Mathf.Infinity);       

            // Define verticies and indices for an arrow
            Vector3[] vertices =
            {
                new (0.0f, 0.0f, 0.0f),                                              // 0 Origin Point           
                new (0.0f, 0.0f, Mathf.Clamp(arrowLength - 0.25f,0,Mathf.Infinity)), // 1 Arrowhead Base Centre  
                new (-.2f, -.2f, bPos),                                              // 2 Arrowhead Base Square  
                new (0.2f, -.2f, bPos),                                              // 3 ...                    
                new (0.2f, 0.2f, bPos),                                              // 4 ...                    
                new (-.2f, 0.2f, bPos),                                              // 5 ...                    
                new (0.0f, 0.0f, Mathf.Clamp(arrowLength,0,Mathf.Infinity)),         // 6 Arrowhead Apex         
            };

            int[] indices =
            {
                // Lines
                0,1,                // Origin to Arrowhead Base
                2,3, 3,4, 4,5, 5,2, // Arrowhead Base
                2,6, 3,6, 4,6, 5,6  // Arrowhead Tip
            };

            mesh.vertices = vertices;
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            return mesh;
        }
    }

    internal enum DebugShape
    {
        Line,
        Cube,
        Sphere,
        WireSphere,
        Arrow,
        // Add other shapes as needed
    }
}