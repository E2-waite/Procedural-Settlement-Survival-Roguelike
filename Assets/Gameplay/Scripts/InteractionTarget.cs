using Cinderwild.Gameplay.Agents;
using Cinderwild.World.Data;
using UnityEngine;

namespace Cinderwild.Core.Interaction
{
    public class InteractionTarget
    {
        public enum Type
        {
            None,
            Agent,
            Tile,
            Building
        }

        Agent agent = null;
        TileData tile = null;
        private Type type = Type.None;

        public void SelectAgent(Agent agent)
        {
            if (agent == null || agent == this.agent) return;
            ClearSelection();
            this.agent = agent;
            type = Type.Agent;
        }

        public void SelectTile(TileData tile)
        {
            if (tile == this.tile) return;
            ClearSelection();
            this.tile = tile;
            type = Type.Tile;
        }

        private void ClearSelection()
        {
            agent = null;
            tile = null;
        }
    }
}