namespace MiniAstro.Management
{
    using UnityEngine;
    using MiniAstro.TerrainGeneration;
    using System.Collections.Generic;
    using System.IO;

    public class GameSaveManager
    {
        private string saveFolder = Application.persistentDataPath;
        private string fileExtension = "dat";
        public MapData MapData { get; private set; }

        public void Save(MapSettings mapSettings, Chunk[] chunks, string saveName)
        {
            ChunkData[] chunkDatas = new ChunkData[chunks.Length];
            for (int i = 0; i < chunks.Length; i++)
            {
                chunkDatas[i] = new ChunkData(chunks[i]);
            }
            MapData mapData = new MapData(mapSettings, chunkDatas);

            MapSerializer.Save(mapData, saveFolder, saveName, fileExtension);
        }

        public MapData Load(string saveName)
        {
            MapData = MapDeserializer.Load(saveFolder, saveName);
            return MapData;
        }

        public List<string> FetchSaveFiles()
        {
            Debug.Log($"Path {saveFolder}");
            DirectoryInfo dirInfo = new DirectoryInfo(saveFolder);
            FileInfo[] info = dirInfo.GetFiles($"*.{fileExtension}");

            List<string> saveNames = new List<string>();
            foreach (var file in info)
            {
                saveNames.Add(file.Name);
            }

            return saveNames;
        }

    }
}
