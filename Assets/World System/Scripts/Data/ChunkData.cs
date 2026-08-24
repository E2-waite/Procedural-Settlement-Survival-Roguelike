using UnityEngine;

namespace Cinderwild.World.Data
{
    [System.Serializable]
    public class ChunkData
    {
        public Vector2 Position { get; private set; } // World position
        public Vector2Int GridPos { get; private set; } // Grid position
        public float[,] Noise { get; private set; }
        public ChunkVertex[,] Vertices { get; private set; }
        public int Size { get; private set; }
        public ChunkResources Resources { get; set; }
        private TileData[] tiles;

        public ChunkData(int size, Vector2 pos, Vector2Int gridPos)
        {
            Size = size;
            Position = pos;
            GridPos = gridPos;
            Noise = new float[Size + 3, Size + 3];
            Vertices = new ChunkVertex[Size + 3, Size + 3];
            tiles = new TileData[(Size + 2) * (Size + 2)];
        }

        public void SetTile(TileData tile, Vector2Int pos)
        {
            int rowSize = Size + 2;
            if (pos.x < 0 || pos.y < 0 || pos.x >= rowSize || pos.y >= rowSize) return;
            int index = pos.y * rowSize + pos.x;
            tiles[index] = tile;
        }

        public TileData GetTile(int x, int y)
        {
            int rowSize = Size + 2;
            if (x < 0 || y < 0 || x >= rowSize || y >= rowSize) return null;
            int index = y * rowSize + x;
            if (index < 0 || index >= tiles.Length) return null;
            else return tiles[index];
        }

        public TileData GetTile(Vector2Int pos)
        {
            return GetTile(pos.x, pos.y);
        }
    }
}