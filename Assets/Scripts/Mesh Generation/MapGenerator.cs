namespace MiniAstro.TerrainGeneration
{
    using MiniAstro.Management;
    using System.Collections.Generic;
    using UnityEngine;

    public class MapGenerator : MonoBehaviour
    {
        [Header("Generators")]
        [SerializeField] private MeshGenerator meshGenerator = null;
        [SerializeField] private DensityGenerator densityGenerator = null ;
        [SerializeField] private MapSettings.MapType typeOfMap;

        [Header("Map settings")]
        public bool autoUpdateInPlay;
        [HideInInspector] public bool loadMap = false;
        //public bool saveMap;
        [Range(10, 100)] public float boundsLength;
        [Range(2, 256)] public int numPointsPerAxis = 32;
        [SerializeField] Vector3Int numberOfChunks = Vector3Int.one;

        [Header("Gizmos")]
        public bool showBoundsGizmo = true;
        public Color boundsGizmoCol = Color.blue;

        private MapLoader mapLoader;
        private List<Chunk> chunks;
        public List<Chunk> Chunks { get { return chunks; } private set { chunks = value; } }
        private MapSettings mapSettings;
        public MapSettings MapSettings { get { return mapSettings; } private set { mapSettings = value; } }

        void Start()
        {

            mapSettings = GenerateDefaultMapSettings();
            meshGenerator.mapSettings = mapSettings;
            mapLoader = new MapLoader();
            Run();
        }

        private MapSettings GenerateDefaultMapSettings()
        {
            MapSettings settings = new MapSettings();
            settings.BoundsLength = boundsLength;
            settings.NumPointsPerAxis = numPointsPerAxis;
            settings.NumOfChunks = numberOfChunks;
            settings.FixedMapSize = true;
            settings.TypeOfMap = typeOfMap;
            return settings;
        }

        private void Run()
        {
            var startTime = Time.realtimeSinceStartup;
            meshGenerator.CreateBuffers();

            if (chunks == null)
                chunks = meshGenerator.InitChunks(mapSettings);
            var postInitChunksTime = Time.realtimeSinceStartup - startTime;

            if (loadMap)
            {
                mapSettings = mapLoader.LoadMapSettings();
                Debug.Log(mapLoader.mapData.chunks[0].scalarField[1]);
            }

            if (loadMap)
                mapLoader.LoadChunksData(chunks);
            else
            {
                foreach (var chunk in chunks)
                {
                    densityGenerator.CreateBuffers(mapSettings);
                    densityGenerator.GenerateDensity(chunk, mapSettings);
                }
            }
            var postDensityTime = Time.realtimeSinceStartup - postInitChunksTime;

            foreach (var chunk in chunks)
            {
                if (!loadMap)
                    meshGenerator.pointsBuffer = densityGenerator.pointsBuffer;
                //else
                //    meshGenerator.pointsBuffer.GetData(chunk.scalarField);
                meshGenerator.GenerateMesh(chunk, chunk.scalarField);
            }
            var postMarchingCubesTime = Time.realtimeSinceStartup - postDensityTime;
            Debug.Log($"Time to generate initialize Chunks = {postInitChunksTime}\nTime to generate density = {postDensityTime}\nTime to generate mesh = {postMarchingCubesTime}");
        }

        public void AlterTerrain(Chunk chunk)
        {
            int threadGroupSize = 8;
            int numVoxelsPerAxis = chunk.numPointsPerAxis - 1;
            int numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float)threadGroupSize);
            meshGenerator.pointsBuffer.SetData(chunk.scalarField);
            meshGenerator.GenerateChunkMesh(chunk, numThreadsPerAxis);
            chunk.UpdateCollider();
        }

        public void RequestMeshUpdate()
        {
            if ((Application.isPlaying && autoUpdateInPlay && chunks != null))
            {
                Run();
            }
        }

        void OnDrawGizmos()
        {
            if (showBoundsGizmo)
            {
                Gizmos.color = boundsGizmoCol;

                List<Chunk> chunks = (this.chunks == null) ? new List<Chunk>(FindObjectsOfType<Chunk>()) : this.chunks;
                foreach (var chunk in chunks)
                {
                    Gizmos.color = boundsGizmoCol;
                    Gizmos.DrawWireCube(densityGenerator.CenterFromCoord(chunk.coord, mapSettings), Vector3.one * mapSettings.BoundsLength);
                }
            }
        }
    }

}