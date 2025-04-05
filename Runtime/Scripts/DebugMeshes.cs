using System.Collections.Generic;
using UnityEngine;

namespace DebugDrawer
{
    /// <summary>
    /// Mesh Construction Methods
    /// </summary>
    internal static class DebugMeshes
    {
        // Implement mesh construction methods for various shapes
        internal static Mesh Construct(DebugShape shape, float length = 1)
        {
            Mesh mesh = new Mesh();

            switch(shape)
            {
                case DebugShape.Line:
                    mesh = CreateLineMesh(length);
                    break;
                case DebugShape.WireQuad:
                    mesh = CreateWireQuadMesh();
                    break;
                case DebugShape.Quad:
                    mesh = CreateQuadMesh();
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

        private static Mesh CreateWireQuadMesh()
        {
            Mesh mesh = new Mesh();

            // Define vertices and indices for a quad
            Vector3[] vertices =
            {
                new Vector2(-1, 1),
                new Vector2( 1, 1),
                new Vector2(-1,-1),
                new Vector2( 1,-1)
            };

            int[] indices = new int[]
            {
                0,1,
                1,2,
                2,0,
                1,3,
                3,2
            };

            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateQuadMesh()
        {
            Mesh mesh = new Mesh();

            // Define vertices and indices for a quad
            Vector3[] vertices =
            {
                new Vector2(-1, 1),
                new Vector2( 1, 1),
                new Vector2(-1,-1),
                new Vector2( 1,-1)
            };

            int[] indices = new int[]
            {
                2,0,1,
                2,1,3
            };

            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            mesh.RecalculateBounds();
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
        WireQuad,
        Quad,
        Cube,
        Sphere,
        WireSphere,
        Arrow,
        // Add other shapes as needed
    } 
}