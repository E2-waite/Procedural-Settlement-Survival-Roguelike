using Cinderwild.World.Data;
using Cinderwild.World.Generation;
using System.Collections.Generic;
using UnityEngine;

namespace Cinderwild.World.Runtime
{
    /// <summary>
    /// Provides global access to the current world and manages runtime world operations.
    /// </summary>
    public class WorldSystem
    {
        private Chunk lastChunk = null;
        private Vector2Int lastPos = Vector2Int.zero;
        private HashSet<Vector2Int> required = new HashSet<Vector2Int>();
        private HashSet<Vector2Int> active = new HashSet<Vector2Int>();
        private WorldManager world;

        public WorldSystem(WorldManager worldManager)
        {
            world = worldManager;
        }

        // Returns the chunk at the passed world position
        public Chunk GetChunk(Vector3 position)
        {
            position /= world.Properties.chunkSize;
            Vector2Int chunkPos = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));

            world.Data.Chunks.TryGetValue(chunkPos, out Chunk chunk);

            return chunk;
        }

        public TileData GetTile(Vector3 position)
        {
            Vector2Int tilePos = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));

            return GetTile(tilePos);
        }

        public TileData GetTile(Vector2Int position)
        {
            world.Data.Tiles.TryGetValue(position, out TileData tile);
            return tile;
        }

        // Streams surrounding chunks, enabling/creating valid chunks and disabling invalid chunks
        public void StreamChunks(Chunk chunk)
        {
            if (chunk == null || chunk == lastChunk) return; // Ignore if already handled or null
            lastChunk = chunk;

            required.Clear();

            int dist = 1;

            if (world.Properties != null)
            {
                dist = world.Properties.streamDist;
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
                if (!world.Data.Chunks.TryGetValue(pos, out Chunk other))
                {
                    other = world.Chunks.Generate(pos);
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
                world.Data.Chunks[pos]?.gameObject.SetActive(false);
            }

            active.Clear();
            active.UnionWith(required);
        }

    }
}
