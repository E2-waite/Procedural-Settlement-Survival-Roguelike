using UnityEngine;

namespace Shadowvale.World.Data
{
    public class ResourceNode
    {
        public int Index => index;
        public ResourceConfig Config => config;
        public TileData Tile => tile;

        private int index = 0;
        private ResourceConfig config;
        private TileData tile;

        public int nodeSize = 0;


        public ResourceNode(ResourceConfig config, TileData tile, int index, int size)
        {
            this.config = config;
            this.tile = tile;
            this.index = index;
            nodeSize = size;
            tile.resource = this;
        }
    }
}
