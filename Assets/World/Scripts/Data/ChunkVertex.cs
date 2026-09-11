using UnityEngine;

namespace Cinderwild.World.Data
{
    public class ChunkVertex
    {
        public Vector3 position;
        public TileConfig type;
        public Color color;

        public ChunkVertex(Vector3 position)
        {
            this.position = position;
        }

        public void SetHeight(float height)
        {
            position.y = height;
        }
    }
}