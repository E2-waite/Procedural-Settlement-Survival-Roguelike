using UnityEngine;
using System;
using Cinderwild.WorldBuilder.Data;
using Cinderwild.WorldBuilder.Runtime;

namespace Cinderwild.WorldBuilder.Generation
{
    [ExecuteAlways]
    public class World : MonoBehaviour
    {
        public WorldProperties Properties => properties;
        public WorldData Data { get; private set; }
        public Vector2 SeedOffset => seedOffset;
        [SerializeField] private Chunk chunkPrefab;
        [SerializeField] private WorldProperties properties;
        private Vector2 seedOffset = Vector2.zero;
        public static Action OnPropertiesChanged;

        private void OnEnable()
        {
            Debug.Log("Subscribed");
            OnPropertiesChanged -= Generate;
            OnPropertiesChanged += Generate;
        }

        private void OnDisable()
        {
            OnPropertiesChanged -= Generate;
        }


        public void Generate()
        {
            properties?.Init();

            ChunkBuilder.Init(this);
            WorldManager.Init(this);
            TileBuilder.Init(this);
            ResourceBuilder.Init(this);
            Data = new WorldData();

            if (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            GameObject chunksObj = new GameObject("Chunks");
            chunksObj.transform.parent = transform;

            for (int x = 0; x < properties.worldSize.x; x++)
            {
                for (int y = 0; y < properties.worldSize.y; y++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);
                    Chunk chunk = ChunkBuilder.GenerateChunk(gridPos);
                    chunk.transform.parent = chunksObj.transform;
                    Data.Chunks[gridPos] = chunk;
                }
            }
        }

        public Chunk SpawnChunk()
        {
            return Instantiate(chunkPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}
