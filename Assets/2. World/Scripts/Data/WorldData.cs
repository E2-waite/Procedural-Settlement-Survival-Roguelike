using System.Collections.Generic;
using UnityEngine;

namespace Cinderwild.World.Data
{
    public class WorldData
    {
        private Dictionary<Vector2Int, ChunkData> chunks = new Dictionary<Vector2Int, ChunkData>();
        private Dictionary<Vector2Int, TileData> tiles = new Dictionary<Vector2Int, TileData>();


    }
}