using System.Collections.Generic;
using UnityEngine;

public class EnemySystem
{
    public List<EnemyUnit> enemies = new List<EnemyUnit>();
    private Player player;
    private WorldGrid grid;
    private EnemySpawner spawner;
    private EnemyCatalog catalog;
    private GameContext gameContext;
    private float spawnTimer = 0, spawnInterval = 15;
    int minDist = 10, maxDist = 25;

    public void Init(GameContext context)
    {
        gameContext = context;
        player = context.player;
        World world = context.world;
        grid = world.Context.grid;
        spawner = context.enemySpawner;
        catalog = context.enemyCatalog;
        spawnTimer = spawnInterval;
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
        if (spawnTimer > 0) spawnTimer -= Time.deltaTime;
        else
        {
            spawnTimer = spawnInterval;

            GridTile spawnTile = FindSpawnTile();

            if (spawnTile != null)
            {
                EnemyUnit enemy = spawner.SpawnEnemy(catalog.testEnemy, spawnTile);
                enemy.Init(gameContext);
            }
        }


    }

    private GridTile FindSpawnTile()
    {
        bool valid = false;
        GridTile spawnTile = null;
        while (!valid)
        {
            int x = Random.Range(minDist, maxDist);
            int y = Random.Range(minDist, maxDist);

            x = Random.Range(0, 1) == 1 ? -x : x;
            y = Random.Range(0, 1) == 1 ? -y : y;



            Vector2Int spawnPos = new Vector2Int(player.GridPos.x + x, player.GridPos.y + y);
            Debug.Log("Enemy spawn pos: " + spawnPos.ToString() + "(dist = " + Vector2Int.Distance(spawnPos, player.GridPos) + ")");

            spawnTile = grid.GetTile(spawnPos);

            if (spawnTile != null && spawnTile.Walkable()) valid = true;
        }

        return spawnTile;
    }
}
