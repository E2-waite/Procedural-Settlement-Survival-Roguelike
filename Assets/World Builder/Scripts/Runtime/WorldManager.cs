using Cinderwild.WorldBuilder.Generation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cinderwild.WorldBuilder.Runtime
{
    public static class WorldManager
    {
        public static World World { get; private set; }
        private static Chunk lastChunk = null;
        private static HashSet<Vector2Int> required = new HashSet<Vector2Int>();
        private static HashSet<Vector2Int> active = new HashSet<Vector2Int>();

        public static void Init(World world)
        {
            World = world;
        }

        // Streams chunks, enabling/creating valid chunks and disabling invalid chunks
        public static void StreamChunks(Chunk chunk)
        {
            if (chunk == lastChunk) return; // Ignore if already handled
            lastChunk = chunk;

            required.Clear();
            for (int x = chunk.Data.GridPos.x - 1; x <= chunk.Data.GridPos.x + 1; x++)
            {
                for (int y = chunk.Data.GridPos.y - 1; y <= chunk.Data.GridPos.y + 1; y++)
                {
                    required.Add(new Vector2Int(x, y));
                }
            }

            // Activate/generate required chunks
            foreach (Vector2Int pos in active)
            {
                if (!World.Data.Chunks.TryGetValue(pos, out Chunk other))
                {
                    other = ChunkBuilder.GenerateChunk(pos);
                    World.Data.Chunks[pos] = other;
                }

                if (!other.isActiveAndEnabled)
                {
                    other.gameObject.SetActive(true);
                }
            }

            // Disable non-required chunks
            foreach (Vector2Int pos in required)
            {
                if (required.Contains(pos)) continue;
                World.Data.Chunks[pos]?.gameObject.SetActive(false);
            }

            active.Clear();
            active.UnionWith(required);
        }

    }
}
