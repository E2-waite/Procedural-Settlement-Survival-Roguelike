using UnityEngine;
using System;
using Cinderwild.World.Data;
using Cinderwild.World.Runtime;

namespace Cinderwild.World.Generation
{
    [ExecuteAlways]
    public class WorldGenerator : MonoBehaviour
    {
        public WorldProperties Properties => properties;
        public Vector2 SeedOffset => seedOffset;
        [SerializeField] private Chunk chunkPrefab;
        [SerializeField] private WorldProperties properties;
        private Vector2 seedOffset = Vector2.zero;
        [SerializeField] private GameObject world = null;

        public static Action OnPropertiesChanged;

        private void OnEnable()
        {
            Debug.Log("Subscribed");
            OnPropertiesChanged -= GenerateWorld;
            OnPropertiesChanged += GenerateWorld;
        }

        private void OnDisable()
        {
            OnPropertiesChanged -= GenerateWorld;
        }


        public void GenerateWorld()
        {
            properties?.Init();

            ChunkBuilder.Init(this);
            TileBuilder.Init(this);
            ResourceBuilder.Init(this);

            if (world != null)
            {
                DestroyImmediate(world);
                world = null;
            }

            world = new GameObject("World");

            for (int x = 0; x < properties.worldSize.x; x++)
            {
                for (int y = 0; y < properties.worldSize.y; y++)
                {
                    Vector2Int chunkPos = new Vector2Int(x * properties.chunkSize, y * properties.chunkSize);
                    Chunk chunk = ChunkBuilder.GenerateChunk(chunkPos, new Vector2Int(x, y));
                    chunk.transform.parent = world.transform;
                }
            }
        }

        public Chunk SpawnChunk(Vector2 pos)
        {
            Chunk chunk = Instantiate(chunkPrefab, new Vector3(pos.x * properties.tileScale, 0, pos.y * properties.tileScale), Quaternion.identity);
            return chunk;
        }
    }
}
