using UnityEngine;
using System;
using Cinderwild.World.Data;
using Cinderwild.World.Generation;

namespace Cinderwild.World.Runtime
{
    /// <summary>
    /// Represents the current game world, including its configuration and runtime data.
    /// Responsible for generating and managing the world's state.
    /// </summary>
    public class WorldManager : MonoBehaviour
    {
        public WorldSystem System { get; private set; }
        public ChunkBuilder Chunks { get; private set; }
        public TileBuilder Tiles { get; private set; }
        public ResourceBuilder Resources { get; private set; }
        public WorldData Data { get; private set; }
        public WorldProperties Properties => properties;
        [SerializeField] private WorldProperties properties;
        [SerializeField] private Chunk chunkPrefab;

        public void Generate()
        {
            Clear();
            properties?.Init();
            System = new WorldSystem(this);
            Chunks = new ChunkBuilder(this);
            Resources = new ResourceBuilder(this);
            Tiles = new TileBuilder(this);
            Data = new WorldData();

            for (int x = 0; x < properties.worldSize.x; x++)
            {
                for (int y = 0; y < properties.worldSize.y; y++)
                {
                    Chunks.Generate(new Vector2Int(x, y));
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            Data = null;
        }

        public Chunk SpawnChunk()
        {
            return Instantiate(chunkPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}
