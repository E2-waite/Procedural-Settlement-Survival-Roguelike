using System.Collections.Generic;
using UnityEngine;
using Cinderwild.WorldBuilder.Runtime;

namespace Cinderwild.WorldBuilder.Data
{
    public class WorldData
    {
        public static WorldData Instance { get; private set; }
        public static WorldProperties Properties { get; private set; }
        public Dictionary<Vector2Int, Chunk> Chunks => chunks;
        public Dictionary<Vector2Int, TileData> Tiles => tiles;
        private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
        private Dictionary<Vector2Int, TileData> tiles = new Dictionary<Vector2Int, TileData>();

        public WorldData(WorldProperties properties)
        {
            Properties = properties;
            Instance = this;
        }
    }
}