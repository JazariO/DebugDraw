using Proselyte.DebugDrawer;
using System.Collections.Generic;

using UnityEngine;

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
    private readonly DebugMeshCollection _wireQuadCollection;
    private readonly DebugMeshCollection _quadCollection;
    private readonly DebugMeshCollection _boxCollection;
    private readonly DebugMeshCollection _sphereCollection;
    private readonly DebugMeshCollection _wireSphereCollection;
    private readonly DebugMeshCollection _wireArrowCollection;
    private readonly DebugMeshCollection _arrowCollection;

    // Capsule components (drawn as separate parts)
    private readonly DebugMeshCollection _capsuleDomeCollection;
    private readonly DebugMeshCollection _capsuleCylinderCollection;

    internal DebugMeshDrawer(DebugDraw parent)
    {
        _parent = parent;

        MeshPool = new ObjectPool<DrawMeshInstance>(DebugDraw.GetMaxPoolSize(), DebugDraw.GetStartingPoolSize());

        // Create materials
        Material wireframeMaterial = CreateDefaultMaterial();

        // Create meshes (single standard mesh per shape type)
        Mesh lineMesh = DebugMeshes.Construct(DebugShape.Line);
        Mesh wireQuadMesh = DebugMeshes.Construct(DebugShape.WireQuad);
        Mesh quadMesh = DebugMeshes.Construct(DebugShape.Quad);
        Mesh boxMesh = DebugMeshes.Construct(DebugShape.Cube);
        Mesh sphereMesh = DebugMeshes.Construct(DebugShape.Sphere);
        Mesh wireSphereMesh = DebugMeshes.Construct(DebugShape.WireSphere);
        Mesh wireArrowMesh = DebugMeshes.Construct(DebugShape.WireArrow);
        Mesh arrowMesh = DebugMeshes.Construct(DebugShape.Arrow);
        Mesh capsuleDomeMesh = DebugMeshes.Construct(DebugShape.CapsuleDome);
        Mesh capsuleCylinderMesh = DebugMeshes.Construct(DebugShape.CapsuleCylinder);

        // Create collections
        _lineCollection = new DebugMeshCollection(lineMesh, wireframeMaterial);
        _wireQuadCollection = new DebugMeshCollection(wireQuadMesh, wireframeMaterial);
        _quadCollection = new DebugMeshCollection(quadMesh, wireframeMaterial);
        _boxCollection = new DebugMeshCollection(boxMesh, wireframeMaterial);
        _sphereCollection = new DebugMeshCollection(sphereMesh, wireframeMaterial);
        _wireSphereCollection = new DebugMeshCollection(wireSphereMesh, wireframeMaterial);
        _wireArrowCollection = new DebugMeshCollection(wireArrowMesh, wireframeMaterial);
        _arrowCollection = new DebugMeshCollection(arrowMesh, wireframeMaterial);
        _capsuleDomeCollection = new DebugMeshCollection(capsuleDomeMesh, wireframeMaterial);
        _capsuleCylinderCollection = new DebugMeshCollection(capsuleCylinderMesh, wireframeMaterial);

        _collections.Add(_lineCollection);
        _collections.Add(_wireQuadCollection);
        _collections.Add(_quadCollection);
        _collections.Add(_boxCollection);
        _collections.Add(_sphereCollection);
        _collections.Add(_wireSphereCollection);
        _collections.Add(_wireArrowCollection);
        _collections.Add(_arrowCollection);
        _collections.Add(_capsuleDomeCollection);
        _collections.Add(_capsuleCylinderCollection);

        DebugMeshCollection.OnInstanceRemoved += inst => MeshPool.Return(inst);
    }

    internal void Cleanup()
    {
        // Unsubscribe from events
        DebugMeshCollection.OnInstanceRemoved -= inst => MeshPool.Return(inst);

        // Destroy all meshes to prevent memory leaks
        foreach(var collection in _collections)
        {
            if(collection != null && collection.Mesh != null)
            {
                Object.Destroy(collection.Mesh);
            }
        }

        // Clear collections
        _collections.Clear();
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

    private DrawMeshInstance GetAMeshInstance(Matrix4x4 transform, float duration, Color color, uint layers, float length = 1f, float height = 1f, float radius = 1f, bool fromFixedUpdate = false)
    {
        DrawMeshInstance inst = MeshPool.Retrieve();
        if(inst != null)
        {
            inst.Transform = transform;
            inst.length = length;
            inst.SetDuration(duration, fromFixedUpdate: fromFixedUpdate);
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

    internal void DrawLine(Matrix4x4 transform, float duration, Color color, uint layers, float lineLength, bool fromFixedUpdate)
    {
        if((DebugDraw.GetEnabledLayers() & layers) == 0)
            return;

        var instance = GetAMeshInstance(transform, duration, color, layers, lineLength, 1, 1, fromFixedUpdate);
        _lineCollection.Add(instance);
    }

    internal void DrawWireQuad(Matrix4x4 transform, float duration, Color color, uint layers, float lineLength, bool fromFixedUpdate)
    {
        // FIXED: Changed | to & for proper layer filtering
        if((DebugDraw.GetEnabledLayers() & layers) == 0)
            return;

        var instance = GetAMeshInstance(transform, duration, color, layers, lineLength, 1, 1, fromFixedUpdate);
        _wireQuadCollection.Add(instance);
    }

    internal void DrawQuad(Matrix4x4 transform, float duration, Color color, uint layers, float length, float height, bool fromFixedUpdate)
    {
        // FIXED: Changed | to & for proper layer filtering
        if((DebugDraw.GetEnabledLayers() & layers) == 0)
            return;

        var instance = GetAMeshInstance(transform, duration, color, layers, length, height, 1, fromFixedUpdate);
        _quadCollection.Add(instance);
    }

    internal void DrawBox(Matrix4x4 transform, float duration, Color color, uint layers, float length, float height, bool fromFixedUpdate)
    {
        if((DebugDraw.GetEnabledLayers() & layers) == 0)
            return;

        var instance = GetAMeshInstance(transform, duration, color, layers, length, height, 1, fromFixedUpdate);
        _boxCollection.Add(instance);
    }

    internal void DrawSphere(Matrix4x4 transform, float duration, Color color, uint layers, float radius, bool fromFixedUpdate)
    {
        if((DebugDraw.GetEnabledLayers() & layers) == 0)
            return;

        var instance = GetAMeshInstance(transform, duration, color, layers, 1, 1, radius, fromFixedUpdate);
        _sphereCollection.Add(instance);
    }

    internal void DrawWireSphere(Matrix4x4 transform, float duration, Color color, uint layers, float radius, bool fromFixedUpdate)
    {
        if((DebugDraw.GetEnabledLayers() & layers) == 0)
            return;

        var instance = GetAMeshInstance(transform, duration, color, layers, 1, 1, radius, fromFixedUpdate);
        _wireSphereCollection.Add(instance);
    }

    internal void DrawWireArrow(Matrix4x4 transform, float duration, Color color, uint layers, bool fromFixedUpdate)
    {
        if((DebugDraw.GetEnabledLayers() & layers) == 0)
            return;

        // FIXED: Use standard mesh with transform scale instead of generating new meshes
        // The arrow size is controlled by the transform's scale (passed from DebugDraw.WireArrow)
        var instance = GetAMeshInstance(transform, duration, color, layers, 1, 1, 1, fromFixedUpdate);
        _wireArrowCollection.Add(instance);
    }

    internal void DrawArrow(Matrix4x4 transform, float duration, Color color, uint layers, bool fromFixedUpdate)
    {
        if((DebugDraw.GetEnabledLayers() & layers) == 0)
            return;

        // FIXED: Use standard mesh with transform scale instead of generating new meshes
        var instance = GetAMeshInstance(transform, duration, color, layers, 1, 1, 1, fromFixedUpdate);
        _arrowCollection.Add(instance);
    }

    internal void DrawWireCapsule(Matrix4x4 transform, float duration, Color color, uint layers, float height, float radius, bool fromFixedUpdate)
    {
        if((DebugDraw.GetEnabledLayers() & layers) == 0)
            return;

        // FIXED: Draw capsule as 3 separate parts - bottom dome, cylinder, top dome
        // This allows proper scaling without distorting the dome spheres

        // Extract position and rotation from the transform
        Vector3 position = transform.GetColumn(3);
        Quaternion rotation = Quaternion.LookRotation(transform.GetColumn(2), transform.GetColumn(1));

        // Calculate cylinder height (total height minus the two dome caps)
        float cylinderHeight = height - (radius * 2f);

        // Bottom dome at origin, scaled by radius
        Vector3 bottomDomePos = position - (rotation * Vector3.up * (cylinderHeight * 0.5f));
        Matrix4x4 bottomDomeTransform = Matrix4x4.TRS(bottomDomePos, rotation * Quaternion.Euler(180, 0, 0), Vector3.one * radius * 2f);
        var bottomDomeInstance = GetAMeshInstance(bottomDomeTransform, duration, color, layers, 1, 1, radius, fromFixedUpdate);
        _capsuleDomeCollection.Add(bottomDomeInstance);

        // Cylinder body - scale XZ by radius, Y by cylinder height
        Matrix4x4 cylinderTransform = Matrix4x4.TRS(position, rotation, new Vector3(radius * 2f, cylinderHeight, radius * 2f));
        var cylinderInstance = GetAMeshInstance(cylinderTransform, duration, color, layers, 1, cylinderHeight, radius, fromFixedUpdate);
        _capsuleCylinderCollection.Add(cylinderInstance);

        // Top dome at end, scaled by radius
        Vector3 topDomePos = position + (rotation * Vector3.up * (cylinderHeight * 0.5f));
        Matrix4x4 topDomeTransform = Matrix4x4.TRS(topDomePos, rotation, Vector3.one * radius * 2f);
        var topDomeInstance = GetAMeshInstance(topDomeTransform, duration, color, layers, 1, 1, radius, fromFixedUpdate);
        _capsuleDomeCollection.Add(topDomeInstance);
    }

    #endregion

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
            if(FreeObjects == 0 && !ExpandPool(1))
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
            if(expansion == 0)
            {
                return false;
            }
            for(int i = 0; i < expansion; i++)
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
}