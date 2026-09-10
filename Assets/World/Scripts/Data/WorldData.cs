using Cinderwild.World.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace Cinderwild.World.Data
{
    [System.Serializable]
    public class WorldData
    {
        public Dictionary<Vector2Int, Chunk> Chunks => chunks;
        public Dictionary<Vector2Int, TileData> Tiles => tiles;
        private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
        private Dictionary<Vector2Int, TileData> tiles = new Dictionary<Vector2Int, TileData>();

        public TileData GetTile(Vector2Int pos)
        {
            Tiles.TryGetValue(pos, out TileData tile);
            return tile;
        }

        public TileData GetTile(Vector3 position)
        {
            Vector2Int tilePos = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));

            return GetTile(tilePos);
        }
    }
}