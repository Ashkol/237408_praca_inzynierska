namespace MiniAstro.TerrainGeneration
{
    using MiniAstro.Management;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class MapData
    {
        public int seed;
        public Vector3Int numOfChunks;
        public float boundsLength;
        public int numPointsPerAxis;
        public bool fixedMapSize;
        public ChunkData[] chunks;
        public MapSettings.MapType typeOfMap;
    
        public MapData(MapSettings mapSettings, ChunkData[] chunkDatas)
        {
            this.seed = mapSettings.Seed;
            this.numOfChunks = mapSettings.NumOfChunks;
            this.boundsLength = mapSettings.BoundsLength;
            this.numPointsPerAxis = mapSettings.NumPointsPerAxis;
            this.fixedMapSize = mapSettings.FixedMapSize;
            this.chunks = chunkDatas;
            this.typeOfMap = mapSettings.TypeOfMap;

        }

        public override string ToString()
        {
            return $"Seed {this.seed}\nNumber of chunks {numOfChunks}\nBounds length {boundsLength}\nNumber of points per axis {numPointsPerAxis}";
        }
    }
}
