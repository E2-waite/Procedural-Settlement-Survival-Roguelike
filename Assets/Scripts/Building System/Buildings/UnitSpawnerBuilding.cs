using UnityEngine;
using static GlobalDefs;

public class UnitSpawnerBuilding : SpawnerBuilding
{
    public override Faction Faction => Faction.Neutral;

    public override void Init(GameContext context)
    {
        WorldBuilder world = context.world;
        grid = world.Context.grid;
        base.Init(context);
    }

    protected override Agent Spawn(GridTile tile)
    {
        if (tile == null) return null;

        GameObject enemyObj = Instantiate(agentPrefab, tile.worldPosition, Quaternion.identity);
        Unit unitAgent = enemyObj.GetComponent<Unit>();
        unitAgent.Init(gameContext, this);
        return unitAgent;
    }
}