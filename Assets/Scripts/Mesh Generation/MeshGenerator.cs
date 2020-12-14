namespace MiniAstro.TerrainGeneration
{
    using System.Collections.Generic;
    using UnityEngine;
    using MiniAstro.Management;

    public class MeshGenerator : MonoBehaviour
    {
        #region Attributes
        [Header("Mesh settings")]
        [Range(0, 1)] public float isoLevel = 0.45f;
        public bool generateCollider;
        public bool generateMesh;
        [HideInInspector] public MapSettings mapSettings;

        [Header("Prefabs")]
        public Chunk chunkPrefab;
        public Material material;

        // Compute Shaders
        public ComputeShader marchingCubeShader;

        [Header("Gizmos")]
        public bool showBoundsGizmo = true;
        public Color boundsGizmoCol = Color.white;

        // Buffers
        ComputeBuffer triangleBuffer;
        [HideInInspector] public ComputeBuffer pointsBuffer;
        ComputeBuffer triCountBuffer;
        List<Chunk> chunks;
        #endregion 

        public List<Chunk> InitChunks(MapSettings settings)
        {
            var chunks = new List<Chunk>();
            for (int y = 0; y < settings.NumOfChunks.y; y++)
            {
                for (int z = 0; z < settings.NumOfChunks.z; z++)
                {
                    for (int x = 0; x < settings.NumOfChunks.x; x++)
                    {
                        chunks.Add(CreateChunk(new Vector3Int(x, y, z)));
                        chunks[chunks.Count - 1].SetUp(material, generateCollider, settings.BoundsLength,
                            settings.NumPointsPerAxis, CenterFromCoord(chunks[chunks.Count - 1].coord));
                    }
                }
            }
            return chunks;
        }

        public void CreateBuffers()
        {
            Debug.Log("Creating buffers");
            int numPoints = mapSettings.NumPointsPerAxis * mapSettings.NumPointsPerAxis * mapSettings.NumPointsPerAxis;
            int numVoxelsPerAxis = mapSettings.NumPointsPerAxis - 1;
            int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
            int maxTriangleCount = numVoxels * 5;

            //if (!Application.isPlaying || (pointsBuffer == null || numPoints != pointsBuffer.count))
            //{
            if (Application.isPlaying)
            {
                ReleaseBuffers();
            }
            triangleBuffer = new ComputeBuffer(maxTriangleCount, sizeof(float) * 3 * 3, ComputeBufferType.Append);
            pointsBuffer = new ComputeBuffer(numPoints, sizeof(float) * 4);
            triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
            System.GC.SuppressFinalize(triangleBuffer);
            System.GC.SuppressFinalize(pointsBuffer);
            System.GC.SuppressFinalize(triCountBuffer);
            //}
        }

        private Chunk CreateChunk(Vector3Int coord)
        {
            Chunk chunk = Instantiate(chunkPrefab, transform);
            chunk.gameObject.name = $"Chunk [{coord.x}, {coord.y}, {coord.z}]";
            chunk.coord = coord;
            return chunk;
        }

        public void GenerateChunkMesh(Chunk chunk, int numThreadGroupsPerAxis)
        {
            triangleBuffer.SetCounterValue(0);
            marchingCubeShader.SetBuffer(marchingCubeShader.FindKernel("March"), "points", pointsBuffer);
            marchingCubeShader.SetBuffer(marchingCubeShader.FindKernel("March"), "triangles", triangleBuffer);
            marchingCubeShader.SetInt("numPointsPerAxis", mapSettings.NumPointsPerAxis);
            marchingCubeShader.SetFloat("isoLevel", isoLevel);

            marchingCubeShader.Dispatch(0, numThreadGroupsPerAxis, numThreadGroupsPerAxis, numThreadGroupsPerAxis);

            ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);
            int[] triCountArray = { 0 };
            triCountBuffer.GetData(triCountArray);
            int numTris = triCountArray[0];

            Triangle[] tris = new Triangle[numTris];
            triangleBuffer.GetData(tris, 0, 0, numTris);

            Mesh mesh = chunk.mesh;
            mesh.Clear();

            var vertices = new Vector3[numTris * 3];
            var meshTriangles = new int[numTris * 3];

            for (int i = 0; i < numTris; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    meshTriangles[i * 3 + j] = i * 3 + j;
                    vertices[i * 3 + j] = tris[i][j];
                }
            }
            mesh.vertices = vertices;
            mesh.triangles = meshTriangles;

            mesh.RecalculateNormals();
        }

        public void GenerateMesh(Chunk chunk, Vector4[] scalarField)
        {
            if (generateMesh)
            {
                int threadGroupSize = 8;
                int numVoxelsPerAxis = chunk.numPointsPerAxis - 1;
                int numThreadGroupsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float)threadGroupSize);
                GenerateChunkMesh(chunk, numThreadGroupsPerAxis, scalarField);
                if (generateCollider)
                    chunk.UpdateCollider();
            }
        }

        private void GenerateChunkMesh(Chunk chunk, int numThreadsPerAxis, Vector4[] scalarField)
        {
            pointsBuffer.SetData(scalarField);
            triangleBuffer.SetCounterValue(0);
            marchingCubeShader.SetBuffer(marchingCubeShader.FindKernel("March"), "points", pointsBuffer);
            marchingCubeShader.SetBuffer(marchingCubeShader.FindKernel("March"), "triangles", triangleBuffer);
            marchingCubeShader.SetInt("numPointsPerAxis", mapSettings.NumPointsPerAxis);
            marchingCubeShader.SetFloat("isoLevel", isoLevel);

            marchingCubeShader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

            ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);
            int[] triCountArray = { 0 };
            triCountBuffer.GetData(triCountArray);
            int numTris = triCountArray[0];

            Triangle[] tris = new Triangle[numTris];
            triangleBuffer.GetData(tris, 0, 0, numTris);

            Mesh mesh = chunk.mesh;
            mesh.Clear();

            var vertices = new Vector3[numTris * 3];
            var meshTriangles = new int[numTris * 3];

            for (int i = 0; i < numTris; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    meshTriangles[i * 3 + j] = i * 3 + j;
                    vertices[i * 3 + j] = tris[i][j];
                }
            }
            mesh.vertices = vertices;
            mesh.triangles = meshTriangles;

            mesh.RecalculateNormals();
        }

        private Vector3 CenterFromCoord(Vector3Int coord)
        {
            Vector3 totalBounds = (Vector3)mapSettings.NumOfChunks * mapSettings.BoundsLength;
            return -totalBounds / 2 + (Vector3)coord * mapSettings.BoundsLength + Vector3.one * mapSettings.BoundsLength / 2;
        }

        struct Triangle
        {
#pragma warning disable 649
            public Vector3 a;
            public Vector3 b;
            public Vector3 c;

            public Vector3 this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 0:
                            return a;
                        case 1:
                            return b;
                        default:
                            return c;
                    }
                }
            }
        }

        private void ReleaseBuffers()
        {
            //Debug.Log("Releasing buffers");
            if (triangleBuffer != null)
            {
                Debug.Log("Releasing buffers");
                triangleBuffer.Release();
                pointsBuffer.Release();
                triCountBuffer.Release();
            }
        }

        void OnDestroy()
        {
            ReleaseBuffers();
        }
    }
}

