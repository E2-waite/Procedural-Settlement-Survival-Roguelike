using System.Collections.Generic;
using UnityEngine;

public class EnemySystem
{
    public List<EnemyUnit> enemies = new List<EnemyUnit>();
    private Player player;
    private WorldGrid grid;
    private EnemySpawner spawner;
    private EnemyCatalog catalog;
    private DayNightSystem dayNightSystem;
    private GameContext gameContext;

    public void Init(GameContext context)
    {
        gameContext = context;
        player = context.player;
        World world = context.world;
        grid = world.Context.grid;
        spawner = context.enemySpawner;
        catalog = context.enemyCatalog;
        dayNightSystem = context.dayNightSystem;
    }

    public void AddEnemy(EnemyUnit enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void RemoveEnemy(EnemyUnit enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }

    public void Tick()
    {
        


    }

    
}
