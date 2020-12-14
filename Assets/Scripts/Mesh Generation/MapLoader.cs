namespace MiniAstro.TerrainGeneration
{
    using MiniAstro.Management;
    using System.Collections.Generic;

    class MapLoader
    {
        public MapData mapData;

        public MapSettings LoadMapSettings()
        {
            LoadMapData();
            return new MapSettings(mapData);
        }

        public void LoadChunksData(List<Chunk> chunks)
        {
            LoadMapData();
            for (int i = 0; i < chunks.Count; i++)
            {
                chunks[i].AssignData(mapData.chunks[i]);
            }
        }

        private void LoadMapData()
        {
            if (mapData == null)
            {
                mapData = GameManager.instance.SaveManager.MapData;
            }
        }
    }
}
