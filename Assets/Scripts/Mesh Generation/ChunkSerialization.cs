namespace MiniAstro.TerrainGeneration
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using AshkolTools.Serialization;
    using UnityEngine;

    public static class ChunkSerializer
    {
        public static bool Save(Chunk chunk)
        {
            bool success = false;
            FileStream fileStream = new FileStream("ChunkData.dat", FileMode.Create);
            BinaryFormatter binFormatter = new BinaryFormatter();

            // Necessary for Vector3 and Vector4 serialization
            SurrogateSelector surrogateSelector = new SurrogateSelector();
            Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();
            Vector4SerializationSurrogate vector4SS = new Vector4SerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);
            surrogateSelector.AddSurrogate(typeof(Vector4), new StreamingContext(StreamingContextStates.All), vector4SS);
            binFormatter.SurrogateSelector = surrogateSelector;

            try
            {
                binFormatter.Serialize(fileStream, chunk.GetChunkData());
            }
            catch (SerializationException ex)
            {
                throw ex;
            }
            finally
            {
                fileStream.Close();
                success = true;
            }

            return success;
        }
    }

    public static class ChunkDeserializer
    {
        public static ChunkData Load()
        {
            ChunkData chunkData;
            FileStream fileStream = new FileStream("ChunkData.dat", FileMode.Open);
            BinaryFormatter binFormatter = new BinaryFormatter();

            // Necessary for Vector3 and Vector4 serialization
            SurrogateSelector surrogateSelector = new SurrogateSelector();
            Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();
            Vector4SerializationSurrogate vector4SS = new Vector4SerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);
            surrogateSelector.AddSurrogate(typeof(Vector4), new StreamingContext(StreamingContextStates.All), vector4SS);
            binFormatter.SurrogateSelector = surrogateSelector;

            try
            {
                chunkData = (ChunkData)binFormatter.Deserialize(fileStream);
            }
            catch (SerializationException ex)
            {
                throw ex;
            }
            finally
            {
                fileStream.Close();
            }

            return chunkData;
        }
    }

}
