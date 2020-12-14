namespace MiniAstro.TerrainGeneration
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class ChunkData
    {
        public Vector3Int coord;
        public Vector3 centre;
        public float boundsSize;
        public int numPointsPerAxis;
        public Vector4[] scalarField;
        public Vector3[] vec3s;

        public ChunkData(Chunk chunk)
        {
            this.coord = chunk.coord;
            this.centre = chunk.centre;
            this.boundsSize = chunk.boundsSize;
            this.numPointsPerAxis = chunk.numPointsPerAxis;
            this.scalarField = chunk.scalarField;
            vec3s = new Vector3[2];
            vec3s[0] = new Vector3(1, 1, 1);
            vec3s[1] = new Vector3(2, 2, 2);
        }
    }
}
