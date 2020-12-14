namespace MiniAstro.TerrainGeneration
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using AshkolTools.Serialization;
    using UnityEngine;
    using MiniAstro.Management;
    using System.Threading.Tasks;

    static class MapSerializer
    {
        public static bool Save(MapData mapData, string path, string saveName, string extension)
        {
            bool success = false;

            FileStream fileStream = new FileStream(System.IO.Path.Combine(path, saveName + "." + extension), FileMode.Create);
            BinaryFormatter binFormatter = new BinaryFormatter();

            // Necessary for Vector3 and Vector4 serialization
            SurrogateSelector surrogateSelector = new SurrogateSelector();
            Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();
            Vector3IntSerializationSurrogate vector3IntSS = new Vector3IntSerializationSurrogate();
            Vector4SerializationSurrogate vector4SS = new Vector4SerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);
            surrogateSelector.AddSurrogate(typeof(Vector3Int), new StreamingContext(StreamingContextStates.All), vector3IntSS);
            surrogateSelector.AddSurrogate(typeof(Vector4), new StreamingContext(StreamingContextStates.All), vector4SS);
            binFormatter.SurrogateSelector = surrogateSelector;

            try
            {
                binFormatter.Serialize(fileStream, mapData);
            }
            catch (SerializationException ex)
            {
                throw;
            }
            finally
            {
                fileStream.Close();
                success = true;
            }

            return success;
        }

    }

    static class MapDeserializer
    {
        public static MapData Load(string path, string saveName)
        {
            MapData mapData;
            FileStream fileStream = new FileStream(System.IO.Path.Combine(path, saveName), FileMode.Open);
            BinaryFormatter binFormatter = new BinaryFormatter();

            // Necessary for Vector3 and Vector4 serialization
            SurrogateSelector surrogateSelector = new SurrogateSelector();
            Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();
            Vector3IntSerializationSurrogate vector3IntSS = new Vector3IntSerializationSurrogate();
            Vector4SerializationSurrogate vector4SS = new Vector4SerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);
            surrogateSelector.AddSurrogate(typeof(Vector3Int), new StreamingContext(StreamingContextStates.All), vector3IntSS);
            surrogateSelector.AddSurrogate(typeof(Vector4), new StreamingContext(StreamingContextStates.All), vector4SS);
            binFormatter.SurrogateSelector = surrogateSelector;

            try
            {
                mapData = (MapData)binFormatter.Deserialize(fileStream);
            }
            catch (SerializationException ex)
            {
                throw;
            }
            finally
            {
                fileStream.Close();
            }

            return mapData;
        }

        public static async Task<MapData> LoadAsync(string path, string saveName)
        {
            Task<MapData> mapData;
            FileStream fileStream = new FileStream(System.IO.Path.Combine(path, saveName), FileMode.Open);
            BinaryFormatter binFormatter = new BinaryFormatter();

            // Necessary for Vector3 and Vector4 serialization
            SurrogateSelector surrogateSelector = new SurrogateSelector();
            Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();
            Vector3IntSerializationSurrogate vector3IntSS = new Vector3IntSerializationSurrogate();
            Vector4SerializationSurrogate vector4SS = new Vector4SerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);
            surrogateSelector.AddSurrogate(typeof(Vector3Int), new StreamingContext(StreamingContextStates.All), vector3IntSS);
            surrogateSelector.AddSurrogate(typeof(Vector4), new StreamingContext(StreamingContextStates.All), vector4SS);
            binFormatter.SurrogateSelector = surrogateSelector;
            
            Debug.Log("Loading Async Starting");
            try
            {
                //mapData = (MapData)binFormatter.Deserialize(fileStream);
                mapData =  DeserializeDataAsync(binFormatter, fileStream);
            }
            catch (SerializationException ex)
            {
                throw;
            }
            finally
            {
                fileStream.Close();
            }

            return await mapData.ConfigureAwait(false);
        }

        private static async Task<MapData> DeserializeDataAsync(BinaryFormatter bf, FileStream fs)
        {
            MapData mapData = null;
            await Task.Run(() =>
            {
                mapData = (MapData) bf.Deserialize(fs);
            });
            Debug.Log("Loaded Async");
            return mapData;
        }
    }

}
