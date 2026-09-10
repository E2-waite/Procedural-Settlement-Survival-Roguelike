using Cinderwild.World.Data;
using Cinderwild.World.Generation;
using System.Collections.Generic;
using UnityEngine;

namespace Cinderwild.World.Runtime
{
    /// <summary>
    /// System class for handling chunk streaming
    /// </summary>
    public class WorldSystem
    {
        private ChunkData lastChunk = null;
        private Vector2Int lastPos = Vector2Int.zero;
        private HashSet<Vector2Int> required = new HashSet<Vector2Int>();
        private HashSet<Vector2Int> active = new HashSet<Vector2Int>();
        private WorldManager world;

        public WorldSystem(WorldManager worldManager)
        {
            world = worldManager;
        }

        // Streams surrounding chunks, enabling/creating valid chunks and disabling invalid chunks
        public void StreamChunks(ChunkData chunk)
        {
            // TODO: clear far chunks from memory (serialize and destroy GameObject)

            if (chunk == null) Debug.LogWarning("CANNOT STREAM NULL CHUNK!");
            if (chunk == null || chunk == lastChunk) return; // Ignore if already handled or null
            lastChunk = chunk;

            required.Clear();

            int dist = 1;

            if (world.Properties != null)
            {
                dist = world.Properties.streamDist;
            }

            for (int x = chunk.GridPos.x - dist; x <= chunk.GridPos.x + dist; x++)
            {
                for (int y = chunk.GridPos.y - dist; y <= chunk.GridPos.y + dist; y++)
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
