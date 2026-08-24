
using System.Collections.Generic;
using UnityEngine;

namespace Cinderwild.WorldBuilder.Data
{
    // Stores chunk's resources (trees, etc.) rendering data
    [System.Serializable]
    public class ChunkResources
    {
        public List<int> ids = new List<int>();
        public Matrix4x4[] Matrices { get; private set; }
        public Vector2 seedOffset;
        private ChunkData chunk;
        private int count;
        public ResourceNode[] Nodes { get; private set; }

        public ChunkResources(ChunkData chunk)
        {
            this.chunk = chunk;
            count = chunk.Size * chunk.Size;
            Nodes = new ResourceNode[count];
            Matrices = new Matrix4x4[count];
        }

        public void SetNode(int index, ResourceNode node)
        {
            Nodes[index] = node;
        }

        public ResourceNode GetNode(int x, int y)
        {
            int index = y * chunk.Size + x;
            if (index < 0 || index >= Nodes.Length) return null;
            else return Nodes[index];
        }
    }
}
