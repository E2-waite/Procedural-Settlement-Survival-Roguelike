using Cinderwild.World.Data;
using Cinderwild.World.Runtime;
using UnityEngine;

namespace Cinderwild.World.Generation
{
    public class ChunkBuilder
    {
        [SerializeField] private WorldManager world;
        private WorldProperties properties;
        private bool initialized = false;
        MeshGenerator meshGenerator = new();
        public ChunkBuilder(WorldManager world)
        {
            if (!initialized)
            {
                this.world = world;
                properties = world.Properties;
                initialized = true;
            }
        }
 
        public Chunk Generate(Vector2Int position)
        {
            Chunk chunk = world.SpawnChunk();
            chunk.Init(properties, position);

            CalculateNoise(chunk.Data);
            BuildVertices(chunk.Data);
            
            world.Tiles.BuildTiles(chunk.Data);

            meshGenerator.Generate(chunk, properties);

            world.Resources.Build(chunk.Data);

            world.Data.Chunks[position] = chunk;
            chunk.transform.parent = world.transform;
            chunk.Collider.sharedMesh = chunk.Mesh;

            return chunk;
        }

        // Calculates chunk's terrain noise
        private void CalculateNoise(ChunkData chunkData)
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
        private void BuildVertices(ChunkData data)
        {
            for (int x = 0; x < data.Size + 3; x++)
            {
                for (int z = 0; z < data.Size + 3; z++)
                {
                    data.Vertices[x, z] = new ChunkVertex(new Vector3((x - 1), 0, (z - 1)));
                }
            }
        }
    }
}