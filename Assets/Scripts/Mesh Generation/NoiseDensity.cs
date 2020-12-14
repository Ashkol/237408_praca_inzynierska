namespace MiniAstro.TerrainGeneration
{
    using System.Collections.Generic;
    using UnityEngine;
    using MiniAstro.Management;

    public class NoiseDensity : DensityGenerator
    {
        [Header("Noise parameters")]
        public int seed;
        [Range(1,25)] public int numOctaves = 4;
        [Range(0, 10)] public float lacunarity = 2;
        [Range(0, 10)] public float persistence = .5f;
        [Range(0, 10)] public float noiseScale = 1;
        [Range(0, 10)] public float noiseWeight = 1;
        [Range(0, 400)] public float floorOffset = 1;
        [Range(-200, 200)] public float hardFloorHeight;
        [Range(0, 200)] public float hardFloorWeight;

        [Header("Extra parameters")]
        public Vector4 shaderParams;

        public override ComputeBuffer GenerateComputeBuffer(ComputeBuffer pointsBuffer, int numPointsPerAxis, float boundsSize, Vector3 worldBounds, Vector3 centre, Vector3 offset, float spacing)
        {
            buffersToRelease = new List<ComputeBuffer>();

            // Noise parameters
            System.Random prng;
            if (SettingsManager.Instance != null)
                prng = new System.Random(SettingsManager.Instance.MapSettings.GetSeed());
            else
                prng = new System.Random(seed);

            var offsets = new Vector3[numOctaves];
            float offsetRange = 1000;
            for (int i = 0; i < numOctaves; i++)
            {
                offsets[i] = new Vector3((float)prng.NextDouble() * 2 - 1, (float)prng.NextDouble() * 2 - 1, (float)prng.NextDouble() * 2 - 1) * offsetRange;
            }

            var offsetsBuffer = new ComputeBuffer(offsets.Length, sizeof(float) * 3);
            offsetsBuffer.SetData(offsets);
            buffersToRelease.Add(offsetsBuffer);

            densityShader.SetVector("centre", new Vector4(centre.x, centre.y, centre.z));
            densityShader.SetInt("octaves", Mathf.Max(1, numOctaves));
            densityShader.SetFloat("lacunarity", lacunarity);
            densityShader.SetFloat("persistence", persistence);
            densityShader.SetFloat("noiseScale", noiseScale);
            densityShader.SetFloat("noiseWeight", noiseWeight);
            densityShader.SetBuffer(0, "offsets", offsetsBuffer);
            densityShader.SetFloat("floorOffset", floorOffset);
            densityShader.SetFloat("hardFloor", hardFloorHeight);
            densityShader.SetFloat("hardFloorWeight", hardFloorWeight);

            densityShader.SetVector("params", shaderParams);

            return base.GenerateComputeBuffer(pointsBuffer, numPointsPerAxis, boundsSize, worldBounds, centre, offset, spacing);
        }
    }
}