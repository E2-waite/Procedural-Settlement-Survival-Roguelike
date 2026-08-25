using Cinderwild.World.Data;
using Cinderwild.World.Generation;
using System.Collections.Generic;
using UnityEngine;

namespace Cinderwild.World.Runtime
{
    /// <summary>
    /// Provides global access to the current World and manages runtime world operations.
    /// </summary>
    public static class WorldSystem
    {
        public static WorldManager World { get; private set; }
        private static Chunk lastChunk = null;
        private static Vector2Int lastPos = Vector2Int.zero;
        private static HashSet<Vector2Int> required = new HashSet<Vector2Int>();
        private static HashSet<Vector2Int> active = new HashSet<Vector2Int>();

        public static void Init(WorldManager world)
        {
            World = world;
        }

        // Returns the chunk at the passed world position
        public static Chunk GetChunk(Vector3 position)
        {
            position /= World.Properties.chunkSize;
            Vector2Int chunkPos = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));

            World.Data.Chunks.TryGetValue(chunkPos, out Chunk chunk);

            return chunk;
        }

        public static TileData GetTile(Vector3 position)
        {
            Vector2Int tilePos = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));

            World.Data.Tiles.TryGetValue(tilePos, out TileData tile);
            return tile;
        }

        // Streams surrounding chunks, enabling/creating valid chunks and disabling invalid chunks
        public static void StreamChunks(Chunk chunk)
        {
            if (chunk == null || chunk == lastChunk) return; // Ignore if already handled or null
            lastChunk = chunk;

            required.Clear();

            int dist = 1;

            if (World.Properties != null)
            {
                dist = World.Properties.streamDist;
            }

            for (int x = chunk.Data.GridPos.x - dist; x <= chunk.Data.GridPos.x + dist; x++)
            {
                for (int y = chunk.Data.GridPos.y - dist; y <= chunk.Data.GridPos.y + dist; y++)
                {
                    required.Add(new Vector2Int(x, y));
                }
            }

            // Activate/generate required chunks
            foreach (Vector2Int pos in required)
            {
                if (!World.Data.Chunks.TryGetValue(pos, out Chunk other))
                {
                    other = ChunkBuilder.Generate(pos);
                }

                if (!other.isActiveAndEnabled)
                {
                    other.gameObject.SetActive(true);
                }
            }

            // Disable non-required chunks
            foreach (Vector2Int pos in active)
            {
                if (required.Contains(pos)) continue;
                World.Data.Chunks[pos]?.gameObject.SetActive(false);
            }

            active.Clear();
            active.UnionWith(required);
        }

    }
}
