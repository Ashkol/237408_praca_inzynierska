namespace MiniAstro.TerrainGeneration
{
    using UnityEngine;

    public class CustomNoise
    {
        public static float EasyPerlin3D(float x, float y, float z)
        {
            float xy = Mathf.PerlinNoise(x, y);
            float yz = Mathf.PerlinNoise(y, z);
            float zx = Mathf.PerlinNoise(z, x);

            float yx = Mathf.PerlinNoise(y, x);
            float zy = Mathf.PerlinNoise(z, y);
            float xz = Mathf.PerlinNoise(x, z);

            return (xy + yz + zx + yx + zy + xz) / 6f;
        }

        public static float HeightMap(float x, float y, float z, float isoLevel, int octaves, float persistence)
        {
            float total = 0f;
            float frequency = 1f;
            float amplitude = 1f;
            float maxValue = 0f;

            for (int i = 0; i < octaves; i++)
            {
                total += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;

                maxValue += amplitude;

                amplitude *= persistence;
                frequency *= 2;
            }

            return (y / (total / maxValue)) * isoLevel;
        }
    }

}