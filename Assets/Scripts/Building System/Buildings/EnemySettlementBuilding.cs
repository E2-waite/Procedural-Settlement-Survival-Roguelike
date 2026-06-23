using UnityEngine;
using static DayNightSystem;
using static GlobalDefs;
public class EnemySettlementBuilding : SpawnerBuilding
{
    private EnemyCatalog catalog;
    public override Faction Faction => Faction.Enemy;
    private AgentSquad currentSquad;
    
    public override void Init(GameContext context)
    {
        World world = context.world;
        grid = world.Context.grid;
        catalog = context.enemyCatalog;
        context.dayNightSystem.PhaseChanged += OnPhaseChanged;
        base.Init(context);
    }

    protected override Agent Spawn(GridTile tile)
    {
        if (tile == null) return null;

        if (currentSquad == null)
        {
            currentSquad = new AgentSquad();
            currentSquad.Init(gameContext, Faction);
        }

        GameObject enemyObj = Instantiate(catalog.testEnemy.prefab, tile.worldPosition, Quaternion.identity);
        Enemy enemyAgent = enemyObj.GetComponent<Enemy>();
        enemyAgent.Init(gameContext, this, tile);
        currentSquad.AddAgent(enemyAgent);

        return enemyAgent;
    }

    // Triggered when time becomes night
    public void DetachSquad()
    {
        foreach (Enemy enemy in currentSquad.Agents)
        {
            if (enemy == null) continue;

            enemy.OrderTo(gameContext.mainFireBuilding);
        }

        // Send squad to attack
        currentSquad = null;


    }

    public void OnPhaseChanged(DayPhase newPhase)
    {
        if (newPhase == DayPhase.Night)
        {
            spawning = false;
            DetachSquad();
        }
        {
            spawning = true;
        }
        Debug.Log("Spawner phase change");
    }
}
