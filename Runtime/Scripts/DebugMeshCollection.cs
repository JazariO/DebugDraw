using Proselyte.DebugDrawer;
using System.Collections.Generic;
using System;

using UnityEngine;

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
        for(int i = _drawInstances.Count - 1; i >= 0; i--)
        {
            if(_drawInstances[i].IsExpired())
            {
                OnInstanceRemoved?.Invoke(_drawInstances[i]);
                _drawInstances.RemoveAt(i);
            }
        }

        if(_drawInstances.Count == 0)
        {
            return;
        }

        // Prepare matrices and colors
        int instanceCount = _drawInstances.Count;
        Matrix4x4[] matrices = new Matrix4x4[instanceCount];
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        Vector4[] colors = new Vector4[instanceCount];

        for(int i = 0; i < instanceCount; i++)
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
        if(instance != null)
        {
            _drawInstances.Add(instance);
        }
    }
}