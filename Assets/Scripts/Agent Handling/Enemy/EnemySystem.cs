using System.Collections.Generic;
using UnityEngine;

public class EnemySystem
{
    public List<Enemy> enemies = new List<Enemy>();
    private Player player;
    private WorldGrid grid;
    private EnemyCatalog catalog;
    private DayNightSystem dayNightSystem;
    private GameContext gameContext;
    private EnemyManager manager;
    List<EnemySettlementBuilding> settlements = new List<EnemySettlementBuilding>();
    private float angerLevel = 0; // Anger level for determining how hostile enemies are (spawns more/stronger enemies)
    
    public void Init(GameContext context)
    {
        gameContext = context;
        player = context.player;
        World world = context.world;
        grid = world.Context.grid;
        catalog = context.enemyCatalog;
        dayNightSystem = context.dayNightSystem;
        manager = context.enemyManager;
    }

    public void AddEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }

    public void Tick()
    {
        
    }

    public void OnSettlementDestroy(EnemySettlementBuilding settlement)
    {
        angerLevel++;

        settlements.Remove(settlement);

        // Spawn a new one
        manager.SpawnSettlement();
    }

    public void OnSettlementSpawn(EnemySettlementBuilding settlement)
    {
        if (!settlements.Contains(settlement))
            settlements.Add(settlement);
    }
}
