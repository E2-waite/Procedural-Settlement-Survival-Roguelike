using UnityEngine;
using static GlobalDefs;

public class UnitSpawnerBuilding : SpawnerBuilding
{
    public override Faction Owner => Faction.Unit;

    public override void Init(GameContext context)
    {
        World world = context.world;
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