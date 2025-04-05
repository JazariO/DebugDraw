using DebugDrawer;
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
        Mesh wireQuadMesh = DebugMeshes.Construct(DebugShape.WireQuad);
        Mesh quadMesh = DebugMeshes.Construct(DebugShape.Quad);
        Mesh boxMesh = DebugMeshes.Construct(DebugShape.Cube);
        Mesh sphereMesh = DebugMeshes.Construct(DebugShape.Sphere);
        Mesh wireSphereMesh = DebugMeshes.Construct(DebugShape.WireSphere);
        Mesh arrowMesh = DebugMeshes.Construct(DebugShape.Arrow);
        // Create other meshes as needed

        // Create collections
        _lineCollection = new DebugMeshCollection(lineMesh, wireframeMaterial);
        _wireQuadCollection = new DebugMeshCollection(wireQuadMesh, wireframeMaterial);
        _quadCollection = new DebugMeshCollection(quadMesh, wireframeMaterial);
        _boxCollection = new DebugMeshCollection(boxMesh, wireframeMaterial);
        _sphereCollection = new DebugMeshCollection(sphereMesh, wireframeMaterial);
        _wireSphereCollection = new DebugMeshCollection(wireSphereMesh, wireframeMaterial);
        _arrowCollection = new DebugMeshCollection(arrowMesh, wireframeMaterial);
        // Add other collections as needed

        _collections.Add(_lineCollection);
        _collections.Add(_wireQuadCollection);
        _collections.Add(_quadCollection);
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

    internal void DrawWireQuad(Matrix4x4 transform, float duration, Color color, uint layers, float lineLength)
    {
        if((DebugDraw.GetEnabledLayers() | layers) == 0)
            return;

        var instance = GetAMeshInstance(transform, duration, color, layers, lineLength);
        _wireQuadCollection.Add(instance);
    }
    
    internal void DrawQuad(Matrix4x4 transform, float duration, Color color, uint layers, float lineLength)
    {
        if((DebugDraw.GetEnabledLayers() | layers) == 0)
            return;

        var instance = GetAMeshInstance(transform, duration, color, layers, lineLength);
        _quadCollection.Add(instance);
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