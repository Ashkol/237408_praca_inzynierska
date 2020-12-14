namespace MiniAstro.TerrainGeneration
{
    using System.Collections.Generic;
    using UnityEngine;
    using MiniAstro.Management;

    public class DensityGenerator : MonoBehaviour
    {
        const int threadGroupSize = 8;
        [Header("Compute shader")]
        public ComputeShader densityShader;
        protected List<ComputeBuffer> buffersToRelease;

        // Buffers
        public ComputeBuffer pointsBuffer;

        void OnValidate()
        {
            if (FindObjectOfType<MapGenerator>())
            {
                FindObjectOfType<MapGenerator>().RequestMeshUpdate();
            }
        }

        public void GenerateDensity(Chunk chunk, MapSettings mapSettings)
        {
            float pointSpacing = chunk.boundsSize / (chunk.numPointsPerAxis - 1);
            Vector3Int coord = chunk.coord;
            Vector3 center = CenterFromCoord(coord, mapSettings);
            var numOfChunks = mapSettings.NumOfChunks;
            Vector3 worldBounds = new Vector3(numOfChunks.x, numOfChunks.y, numOfChunks.z) * mapSettings.BoundsLength;
            Vector3 offset = Vector3.zero;

            GenerateComputeBuffer(pointsBuffer, mapSettings.NumPointsPerAxis, mapSettings.BoundsLength, worldBounds, center, offset, pointSpacing);
            // Pass points values to the chunk
            pointsBuffer.GetData(chunk.scalarField);
        }

        public virtual ComputeBuffer GenerateComputeBuffer(ComputeBuffer pointsBuffer, int numPointsPerAxis, float boundsSize, Vector3 worldBounds, Vector3 centre, Vector3 offset, float spacing)
        {
            int numThreadsPerAxis = Mathf.CeilToInt(numPointsPerAxis / (float)threadGroupSize);
            // Set paramaters
            densityShader.SetBuffer(0, "points", pointsBuffer);
            densityShader.SetInt("numPointsPerAxis", numPointsPerAxis);
            densityShader.SetFloat("boundsSize", boundsSize);
            densityShader.SetVector("centre", new Vector4(centre.x, centre.y, centre.z));
            densityShader.SetVector("offset", new Vector4(offset.x, offset.y, offset.z));
            densityShader.SetFloat("spacing", spacing);
            densityShader.SetVector("worldSize", worldBounds);

            // Dispatch shader
            densityShader.Dispatch(densityShader.FindKernel("GenerateDensity"), numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

            if (buffersToRelease != null)
            {
                foreach (var b in buffersToRelease)
                {
                    b.Release();
                }
            }

            // Returns voxel data to generate mesh
            return pointsBuffer;
        }

        public void CreateBuffers(MapSettings mapSettings)
        {
            int numPoints = mapSettings.NumPointsPerAxis * mapSettings.NumPointsPerAxis * mapSettings.NumPointsPerAxis;
            int numVoxelsPerAxis = mapSettings.NumPointsPerAxis - 1;
            int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
            int maxTriangleCount = numVoxels * 5;

            // Release buffers is application is playing to prevent memore leaks !!!
            if (Application.isPlaying)
            {
                ReleaseBuffers();
            }
            pointsBuffer = new ComputeBuffer(numPoints, sizeof(float) * 4);

        }

        void ReleaseBuffers()
        {
            if (pointsBuffer != null)
                pointsBuffer.Release();
        }

        public Vector3 CenterFromCoord(Vector3Int coord, MapSettings mapSettings)
        {
            // Centre entire map at origin
            if (mapSettings.FixedMapSize)
            {
                Vector3 totalBounds = (Vector3)mapSettings.NumOfChunks * mapSettings.BoundsLength;
                return -totalBounds / 2 + (Vector3)coord * mapSettings.BoundsLength + Vector3.one * mapSettings.BoundsLength / 2;
            }

            return new Vector3(coord.x, coord.y, coord.z) * mapSettings.BoundsLength;
        }

        void OnDestroy()
        {
            ReleaseBuffers();
        }
    }
}
