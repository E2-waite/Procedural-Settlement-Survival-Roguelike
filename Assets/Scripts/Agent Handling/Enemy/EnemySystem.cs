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

    public void Init(GameContext context)
    {
        gameContext = context;
        player = context.player;
        World world = context.world;
        grid = world.Context.grid;
        catalog = context.enemyCatalog;
        dayNightSystem = context.dayNightSystem;
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

    
}
