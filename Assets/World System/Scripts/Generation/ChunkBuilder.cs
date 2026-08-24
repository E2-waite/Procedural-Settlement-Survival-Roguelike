using Cinderwild.WorldBuilder.Data;
using Cinderwild.WorldBuilder.Runtime;
using UnityEngine;

namespace Cinderwild.WorldBuilder.Generation
{
    public static class ChunkBuilder
    {
        [SerializeField] private static Runtime.WorldBuilder world;
        private static WorldProperties properties;
        private static bool initialized = false;
        public static void Init(Runtime.WorldBuilder builder)
        {
            if (!initialized)
            {
                world = builder;
                properties = builder.Properties;
                initialized = true;
            }
        }

        // Generates a chunk and 
        public static Chunk Generate(Vector2Int position)
        {
            Chunk chunk = world.SpawnChunk();
            chunk.Init(properties, position);

            CalculateNoise(chunk.Data);
            BuildVertices(chunk.Data);
            
            TileBuilder.BuildTiles(chunk.Data);

            MeshGenerator.Generate(chunk, properties);

            ResourceBuilder.Build(chunk.Data);

            world.Data.Chunks[position] = chunk;
            chunk.transform.parent = world.transform;

            return chunk;
        }

        // Calculates chunk's terrain noise
        private static void CalculateNoise(ChunkData chunkData)
        {
            for (int x = 0; x < chunkData.Size + 3; x++)
            {
                for (int y = 0; y < chunkData.Size + 3; y++)
                {
                    chunkData.Noise[x, y] = Noise.GetNoise(x - 1 + chunkData.Position.x, y - 1 + chunkData.Position.y, properties.noiseScale, world.SeedOffset) - properties.seaLevel;
                    if (chunkData.Noise[x, y] > 0) chunkData.Noise[x, y] *= properties.heightMultiplier;
                    chunkData.Noise[x, y] = Mathf.Clamp01(chunkData.Noise[x, y]);
                }
            }
        }

        // Create vertex grid
        private static void BuildVertices(ChunkData data)
        {
            for (int x = 0; x < data.Size + 3; x++)
            {
                for (int z = 0; z < data.Size + 3; z++)
                {
                    data.Vertices[x, z] = new ChunkVertex(new Vector3((x - 1) * properties.tileScale, 0, (z - 1) * properties.tileScale));
                }
            }
        }
    }
}