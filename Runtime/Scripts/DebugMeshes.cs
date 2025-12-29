using System.Collections.Generic;
using UnityEngine;

namespace Proselyte.DebugDrawer
{
    /// <summary>
    /// Mesh Construction Methods
    /// </summary>
    internal static class DebugMeshes
    {
        // FIXED: Removed parameters from Construct - all meshes are now unit-sized and scaled via transforms
        internal static Mesh Construct(DebugShape shape)
        {
            Mesh mesh = new Mesh();

            switch(shape)
            {
                case DebugShape.Line:
                    mesh = CreateLineMesh();
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
                    mesh = CreateWireArrowMesh();
                    break;
                case DebugShape.Arrow:
                    mesh = CreateArrowMesh();
                    break;
                case DebugShape.CapsuleDome:
                    mesh = CreateCapsuleDomeMesh();
                    break;
                case DebugShape.CapsuleCylinder:
                    mesh = CreateCapsuleCylinderMesh();
                    break;

                default:
                    Debug.LogError($"DebugShape {shape} not implemented");
                    break;
            }
            return mesh;
        }

        /// <summary>
        /// Creates a hemisphere dome wireframe consisting of:
        /// - Rim circle in XZ plane
        /// - Vertical arc in ZY plane
        /// - Vertical arc in XY plane
        /// No grid lines, no triangles.
        /// </summary>
        private static Mesh CreateCapsuleDomeMesh()
        {
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            const int segments = 32;
            float radius = 0.5f;

            // --- 1. XZ PLANE RIM CIRCLE ---
            int xzStart = vertices.Count;
            for(int i = 0; i <= segments; i++)
            {
                float angle = i * 2f * Mathf.PI / segments;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                vertices.Add(new Vector3(x, 0f, z));
            }
            for(int i = 0; i < segments; i++)
            {
                indices.Add(xzStart + i);
                indices.Add(xzStart + i + 1);
            }

            // --- 2. ZY PLANE ARC  ---
            int zyStart = vertices.Count;
            for(int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                float theta = t * Mathf.PI;

                float z = Mathf.Cos(theta) * radius;
                float y = Mathf.Sin(theta) * radius;

                vertices.Add(new Vector3(0f, y, z));
            }
            for(int i = 0; i < segments; i++)
            {
                indices.Add(zyStart + i);
                indices.Add(zyStart + i + 1);
            }

            // --- 3. XY PLANE ARC  ---
            int xyStart = vertices.Count;
            for(int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                float theta = t * Mathf.PI;

                float x = Mathf.Cos(theta) * radius;
                float y = Mathf.Sin(theta) * radius;

                vertices.Add(new Vector3(x, y, 0f));
            }
            for(int i = 0; i < segments; i++)
            {
                indices.Add(xyStart + i);
                indices.Add(xyStart + i + 1);
            }

            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            return mesh;
        }


        /// <summary>
        /// Creates a unit cylinder for capsule body.
        /// Cylinder is 1 unit tall, 1 unit diameter, centered at origin.
        /// </summary>
        private static Mesh CreateCapsuleCylinderMesh()
        {
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            const int circleSegments = 32;
            float radius = 0.5f;

            // Top circle vertices
            for(int i = 0; i <= circleSegments; i++)
            {
                float angle = i * 2 * Mathf.PI / circleSegments;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                vertices.Add(new Vector3(x, 0.5f, z)); // Top at Y = 0.5
            }

            // Bottom circle vertices
            for(int i = 0; i <= circleSegments; i++)
            {
                float angle = i * 2 * Mathf.PI / circleSegments;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                vertices.Add(new Vector3(x, -0.5f, z)); // Bottom at Y = -0.5
            }

            // Top circle lines
            for(int i = 0; i < circleSegments; i++)
            {
                indices.Add(i);
                indices.Add(i + 1);
            }

            // Bottom circle lines
            int bottomOffset = circleSegments + 1;
            for(int i = 0; i < circleSegments; i++)
            {
                indices.Add(bottomOffset + i);
                indices.Add(bottomOffset + i + 1);
            }

            // Vertical lines connecting top and bottom
            for(int i = 0; i <= circleSegments; i += circleSegments / 4) // 4 vertical lines
            {
                indices.Add(i);
                indices.Add(bottomOffset + i);
            }

            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateLineMesh()
        {
            Mesh mesh = new Mesh();

            // Unit line along Z axis
            Vector3[] vertices =
            {
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 1)
            };

            int[] indices =
            {
                0, 1
            };

            mesh.vertices = vertices;
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateWireQuadMesh()
        {
            Mesh mesh = new Mesh();

            // FIXED: Quad in XY plane (Z forward is normal)
            Vector3[] vertices =
            {
                new Vector3(-0.5f,  0.5f, 0), // Top-left
                new Vector3( 0.5f,  0.5f, 0), // Top-right
                new Vector3( 0.5f, -0.5f, 0), // Bottom-right
                new Vector3(-0.5f, -0.5f, 0)  // Bottom-left
            };

            int[] indices = new int[]
            {
                0, 1,
                1, 2,
                2, 3,
                3, 0
            };

            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateQuadMesh()
        {
            Mesh mesh = new Mesh();

            // FIXED: Quad in XY plane (Z forward is normal), facing positive Z
            Vector3[] vertices =
            {
                new Vector3(-0.5f,  0.5f, 0), // Top-left
                new Vector3( 0.5f,  0.5f, 0), // Top-right
                new Vector3( 0.5f, -0.5f, 0), // Bottom-right
                new Vector3(-0.5f, -0.5f, 0)  // Bottom-left
            };

            int[] indices = new int[]
            {
                0, 1, 2,  // First triangle (counter-clockwise when viewed from +Z)
                0, 2, 3   // Second triangle
            };

            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateCubeMesh()
        {
            Mesh mesh = new Mesh();

            // Define vertices and indices for a cube
            Vector3[] vertices =
            {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3( 0.5f, -0.5f, -0.5f),
                new Vector3( 0.5f,  0.5f, -0.5f),
                new Vector3(-0.5f,  0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f,  0.5f),
                new Vector3( 0.5f, -0.5f,  0.5f),
                new Vector3( 0.5f,  0.5f,  0.5f),
                new Vector3(-0.5f,  0.5f,  0.5f)
            };

            int[] indices =
            {
                // Lines
                0, 1, 1, 2, 2, 3, 3, 0, // Bottom face
                4, 5, 5, 6, 6, 7, 7, 4, // Top face
                0, 4, 1, 5, 2, 6, 3, 7  // Sides
            };

            mesh.vertices = vertices;
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateSphereMesh(int longitudeSegments, int latitudeSegments)
        {
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
        /// Returns a unit-sized wire arrow mesh.
        /// FIXED: No parameters - size controlled by transform scale.
        /// Arrow points along +Z axis with base at origin.
        /// </summary>
        private static Mesh CreateWireArrowMesh()
        {
            Mesh mesh = new Mesh();

            // Unit arrow - arrowhead at Z=1, base at Z=0
            // Arrowhead size is 0.25 units (will scale with transform)
            float shaftEnd = 0.75f;
            float headBase = 0.75f;
            float headSize = 0.2f;

            Vector3[] vertices =
            {
                new Vector3(0.0f, 0.0f, 0.0f),        // 0 Origin Point
                new Vector3(0.0f, 0.0f, shaftEnd),    // 1 Shaft end / Arrowhead base center
                new Vector3(-headSize, -headSize, headBase), // 2 Arrowhead base square
                new Vector3( headSize, -headSize, headBase), // 3
                new Vector3( headSize,  headSize, headBase), // 4
                new Vector3(-headSize,  headSize, headBase), // 5
                new Vector3(0.0f, 0.0f, 1.0f),        // 6 Arrowhead apex
            };

            int[] indices =
            {
                // Shaft
                0, 1,
                // Arrowhead base square
                2, 3, 3, 4, 4, 5, 5, 2,
                // Arrowhead edges to apex
                2, 6, 3, 6, 4, 6, 5, 6
            };

            mesh.vertices = vertices;
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary>
        /// Returns a unit-sized filled arrow mesh.
        /// FIXED: No parameters - size controlled by transform scale.
        /// </summary>
        private static Mesh CreateArrowMesh()
        {
            Mesh mesh = new Mesh();

            float shaftEnd = 0.75f;
            float headBase = 0.75f;
            float headSize = 0.2f;

            Vector3[] vertices =
            {
                new Vector3(0.0f, 0.0f, 0.0f),        // 0 Origin Point
                new Vector3(0.0f, 0.0f, shaftEnd),    // 1 Shaft end / Arrowhead base center
                new Vector3(-headSize, -headSize, headBase), // 2 Arrowhead base square
                new Vector3( headSize, -headSize, headBase), // 3
                new Vector3( headSize,  headSize, headBase), // 4
                new Vector3(-headSize,  headSize, headBase), // 5
                new Vector3(0.0f, 0.0f, 1.0f),        // 6 Arrowhead apex
            };

            int[] indices =
            {
                // Arrowhead base (two triangles)
                2, 3, 4,
                2, 4, 5,
                // Arrowhead faces (pyramid)
                2, 3, 6,
                3, 4, 6,
                4, 5, 6,
                5, 2, 6
            };

            mesh.vertices = vertices;
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
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
        CapsuleDome,      // Hemisphere for capsule ends
        CapsuleCylinder,  // Cylinder for capsule body
        WireCapsule,      // Deprecated - kept for compatibility
        Capsule,
    }
}