using Cinderwild.World.Data;
using Cinderwild.World.Runtime;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Cinderwild.World.Generation
{
    public static class ChunkBuilder
    {
        private static WorldGenerator world;
        private static WorldProperties properties;
        private static bool initialized = false;
        public static void Init(WorldGenerator builder)
        {
            if (!initialized)
            {
                world = builder;
                properties = builder.Properties;
                initialized = true;
            }
        }

        public static Chunk GenerateChunk(Vector2 worldPos, Vector2Int gridPos)
        {
            Chunk chunk = world.SpawnChunk(worldPos);
            chunk.Init(properties, worldPos, gridPos);

            CalculateNoise(chunk.Data);
            BuildVertices(chunk.Data);
            
            TileBuilder.BuildTiles(chunk.Data);

            
            MeshGenerator.Generate(chunk, properties);

            ResourceBuilder.Build(chunk.Data);
            return chunk;
        }

        // Calculates chunk's terrain noise
        private static void CalculateNoise(ChunkData data)
        {
            for (int x = 0; x < data.Size + 3; x++)
            {
                for (int y = 0; y < data.Size + 3; y++)
                {
                    data.Noise[x, y] = Noise.GetNoise(x - 1 + data.Position.x, y - 1 + data.Position.y, properties.noiseScale, world.SeedOffset) - properties.seaLevel;
                    if (data.Noise[x, y] > 0) data.Noise[x, y] *= properties.heightMultiplier;
                    data.Noise[x, y] = Mathf.Clamp01(data.Noise[x, y]);
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