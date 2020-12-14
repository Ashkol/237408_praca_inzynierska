namespace MiniAstro.Management
{
    using MiniAstro.TerrainGeneration;
    using UnityEngine;

    public class MapSettings
    {
        private const int DEFAULT_SEED = 1997;
        public enum MapType
        {
            Plane,
            Sphere
        }

        public int Seed { get; set; }
        public Vector3Int NumOfChunks { get; set; }
        public float BoundsLength { get; set; }
        public int NumPointsPerAxis { get; set; }
        public bool FixedMapSize { get; set; }
        public MapType TypeOfMap { get; set; } 

        #region Constructors
        public MapSettings()
        {
            Seed = DEFAULT_SEED;
        }

        public MapSettings(int seed)
        {
            Seed = seed;
        }

        public MapSettings(MapData data)
        {
            Seed = data.seed;
            NumOfChunks = data.numOfChunks;
            BoundsLength = data.boundsLength;
            NumPointsPerAxis = data.numPointsPerAxis;
            FixedMapSize = data.fixedMapSize;
            TypeOfMap = data.typeOfMap;
        }
        #endregion


    }
}

