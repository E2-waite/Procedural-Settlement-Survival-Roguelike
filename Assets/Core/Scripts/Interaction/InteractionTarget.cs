using Cinderwild.Gameplay.Agents;
using Cinderwild.World.Data;

namespace Cinderwild.Core
{
    public enum TargetType
    {
        None,
        Agent,
        Tile,
        Building
    }
    public class InteractionTarget
    {
        private TargetType type = TargetType.None;
        Agent agent = null;
        TileData tile = null;
        public TargetType Type => type;
        public Agent Agent => agent;
        public TileData Tile => tile;

        public InteractionTarget() { }

        // Copy constructor
        public InteractionTarget(InteractionTarget other)
        {
            type = other.type;
            agent = other.agent;
            tile = other.tile;
        }

        public void SelectAgent(Agent agent)
        {
            if (agent == null || agent == this.agent) return;
            ClearSelection();
            this.agent = agent;
            type = TargetType.Agent;
            agent?.Highlight();
        }

        public void SelectTile(TileData tile)
        {
            if (tile == this.tile) return;
            ClearSelection();
            this.tile = tile;
            type = TargetType.Tile;
        }

        private void ClearSelection()
        {
            agent?.ClearHighlight();
            agent = null;
            tile = null;
        }
    }
}