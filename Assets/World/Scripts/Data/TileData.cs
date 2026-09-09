using UnityEngine;

namespace Cinderwild.World.Data
{
    [System.Serializable]
    public class TileData
    {
        public ChunkVertex[] Vertices => vertices;
        public ChunkVertex[] StepVertices => stepVertices;
        private ChunkVertex[] vertices = new ChunkVertex[4];
        private ChunkVertex[] stepVertices = new ChunkVertex[4];

        private float[] noise = new float[4];
        public float[] Noise => noise;
        private Vector2Int position = Vector2Int.zero;
        public Vector2Int Position => position;
        public Vector3 WorldPosition { get; set; }
        public Vector3 Center { get; set; }
        public bool IsWalkable => Object != null && Object.walkable && resource == null;
        public TileConfig Object  { get; set; }
        public bool IsFlat { get; set; }
        public ResourceNode resource = null;

        public TileData(Vector2Int position)
        {
            this.position = position;
        }

        // Returns offset position based on vertex index
        public Vector2Int VertexPosition(int index)
        {
            Vector2Int offset = Defs.corners[index];
            return new Vector2Int(position.x + offset.x, position.y + offset.y);
        }

        public ChunkVertex GetVertex(VertexDir dir)
        {
            return vertices[(int)dir];
        }

        public float AverageNoise()
        {
            float total = 0;
            for (int i = 0; i < 4; i++)
            {
                total += noise[i];
            }
            return total / 4;
        }
    }
}