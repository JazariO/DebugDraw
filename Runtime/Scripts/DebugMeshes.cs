using Codice.Client.Common.GameUI;
using System;
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
        internal static Mesh Construct(DebugShape shape, float length = 1f, float height = 2f, float radius = 1f)
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
                case DebugShape.WireArrow:
                    mesh = CreateWireArrowMesh(length);
                    break;
                case DebugShape.Arrow:
                    mesh = CreateArrowMesh(length);
                    break;
                case DebugShape.WireCapsule:
                    mesh = CreateWireCapsuleMesh(height, radius);
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
        /// Returns a wire arrow mesh.
        /// </summary>
        /// <param name="arrowLength">The total length of the arrow. Tip to tail.</param>
        /// <returns></returns>
        private static Mesh CreateWireArrowMesh(float arrowLength)
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

        /// <summary>Returns an arrow mesh.</summary>
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
                // Tris
                2,3,4,  //Arrow base
                2,4,5,

                2,3,6,  //Arrow faces
                3,4,6,
                4,5,6,
                5,2,6
            };

            mesh.vertices = vertices;
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary> Returns a capsule mesh. </summary>
        private static Mesh CreateWireCapsuleMesh(float height, float radius)
        {
            Mesh mesh = new();

            float cylinderHeight = height * 0.5f - radius;
            float topPoint =  cylinderHeight;
            float botPoint = -cylinderHeight;

            const int circleResolution = 50;
            const int semiCircleResolution = circleResolution/2;

            // Add cylinder body vertices
            List<Vector3> vertices = new List<Vector3>()
            {
                // Cylinder Body Vertices
                new ( 0 * radius, botPoint, 1 * radius), // 0
                new ( 1 * radius, botPoint, 0 * radius), // 1
                new ( 0 * radius, botPoint,-1 * radius), // 2
                new (-1 * radius, botPoint, 0 * radius), // 3

                new ( 0* radius, topPoint, 1 * radius), // 4
                new ( 1* radius, topPoint, 0 * radius), // 5
                new ( 0* radius, topPoint,-1 * radius), // 6
                new (-1* radius, topPoint, 0 * radius), // 7
            };

            // Add cylinder base cap vertices
            for(int i = 0; i < circleResolution; i++)
            {
                float p = 2 * Mathf.PI / circleResolution * i;
                float cp = Mathf.Cos(p) * radius;
                float sp = Mathf.Sin(p) * radius;

                Vector3 v = new Vector3(cp, botPoint, sp);
                vertices.Add(v);
            }

            // Add cylinder top cap vertices
            for(int i = 0; i < circleResolution; i++)
            {
                float p = 2 * Mathf.PI / circleResolution * i;
                float cp = Mathf.Cos(p) * radius;
                float sp = Mathf.Sin(p) * radius;

                Vector3 v = new Vector3(cp, topPoint, sp);
                vertices.Add(v);
            }

            // Add cylinder base XY dome cross-shape vertices
            for(int i = 0; i <= semiCircleResolution; i++)
            { 
                float p = Mathf.PI / semiCircleResolution * i;

                float cp = Mathf.Cos(p) * radius;
                float sp = Mathf.Sin(p) * radius;

                Vector3 v = new Vector3(cp, botPoint - sp, 0);
                vertices.Add(v);

            }

            // Add cylinder base ZY dome cross-shape vertices
            for(int i = 0; i <= semiCircleResolution; i++)
            {
                float p = Mathf.PI / semiCircleResolution * i;

                float cp = Mathf.Cos(p) * radius;
                float sp = Mathf.Sin(p) * radius;

                Vector3 v = new Vector3(0, botPoint - sp, cp);
                vertices.Add(v);
            }

            // Add cylinder cap XY dome cross-shape vertices
            for(int i = 0; i <= semiCircleResolution; i++)
            {
                float p = Mathf.PI / semiCircleResolution * i;

                float cp = Mathf.Cos(p) * radius;
                float sp = Mathf.Sin(p) * radius;

                Vector3 v = new Vector3(cp, topPoint + sp, 0);
                vertices.Add(v);
            }

            // Add cylinder cap ZY dome cross-shape vertices
            for(int i = 0; i <= semiCircleResolution; i++)
            {
                float p = Mathf.PI / semiCircleResolution * i;

                float cp = Mathf.Cos(p) * radius;
                float sp = Mathf.Sin(p) * radius;

                Vector3 v = new Vector3(0, topPoint + sp, cp);
                vertices.Add(v);
            }

            // Add cylinder body indices
            List<int> indices = new List<int>()
            {
                //Cylinder lines
                0,4,
                1,5,
                2,6,
                3,7,
            };

            // Add cylinder base indices
            for(int i = 0; i < circleResolution; i++)
            {
                int first = 8 + i;
                int next = 8 + ((i + 1) % circleResolution);
                indices.Add(first);
                indices.Add(next);
            }

            // Add cylinder top indices
            for(int i = 0; i < circleResolution; i++)
            {
                int offset = 8 + circleResolution;
                int first = offset + i;
                int next = offset + ((i + 1) % circleResolution);
                indices.Add(first);
                indices.Add(next);
            }

            // Add cylinder base XY dome indices
            for(int i = 0; i < semiCircleResolution - 1; i++)
            {
                int offset = 8 + circleResolution * 2 + i;
                int first = offset;
                int next = offset + 1;
                indices.Add(first);
                indices.Add(next);
            }

            // Connect last point to base cap circle vertex
            indices.Add(8 + circleResolution * 2 + semiCircleResolution - 1);
            indices.Add(3); // (0, botPoint, radius)

            // Add cylinder base ZY dome cross-shape indices
            for(int i = 1; i < semiCircleResolution; i++) // Skip over the previous line
            {
                int offset = 8 + circleResolution * 2 + semiCircleResolution + i;
                int first = offset;
                int next = offset + 1;
                indices.Add(first);
                indices.Add(next);
            }

            // Connect last point on ZY to base cap circle vertex
            indices.Add(8 + circleResolution * 2 + semiCircleResolution * 2);
            indices.Add(2);

            // Add cylinder top XY dome cross-shape indices
            for(int i = 1; i < semiCircleResolution; i++) // Skip over previous line
            {
                int offset = 8 + circleResolution * 2 + semiCircleResolution * 2 + i + 1;
                int first = offset;
                int next = offset + 1;
                indices.Add(first);
                indices.Add(next);
            }

            // Connect last point to top cap circle vertex
            indices.Add(8 + circleResolution * 2 + semiCircleResolution * 3 + 1);
            indices.Add(7);

            // Add cylinder base ZY dome cross-shape indices
            for(int i = 2; i <= semiCircleResolution; i++) // Skip over the previous line
            {
                int offset = 8 + circleResolution * 2 + semiCircleResolution * 3 + i + 1;
                int first = offset;
                int next = offset + 1;
                indices.Add(first);
                indices.Add(next);
            }

            // Connect last point to top cap circle vertex
            indices.Add(8 + circleResolution * 2 + semiCircleResolution * 4 + 2);
            indices.Add(6);

            mesh.SetVertices(vertices);
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
        WireArrow,
        Arrow,
        WireCapsule,
        Capsule,
        // Add other shapes as needed
    } 
}