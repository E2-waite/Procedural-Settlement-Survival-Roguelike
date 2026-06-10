using UnityEngine;
using static GlobalDefs;
public class EnemySpawnerBuilding : SpawnerBuilding
{
    private EnemyCatalog catalog;
    public override Faction Owner => Faction.Enemy;

    public override void Init(GameContext context)
    {
        World world = context.world;
        grid = world.Context.grid;
        catalog = context.enemyCatalog;
        base.Init(context);
    }

    protected override Agent Spawn(GridTile tile)
    {
        if (tile == null) return null;

        GameObject enemyObj = Instantiate(catalog.testEnemy.prefab, tile.worldPosition, Quaternion.identity);
        Enemy enemyAgent = enemyObj.GetComponent<Enemy>();
        enemyAgent.Init(gameContext, this, tile);
        return enemyAgent;
    }
}
