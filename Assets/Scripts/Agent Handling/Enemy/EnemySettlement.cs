using UnityEngine;
using static GlobalDefs;
public class EnemySettlement : Building
{
    private EnemySystem enemySystem;
    private EnemySpawner spawner;
    private DayNightSystem dayNightSystem;
    private GameContext gameContext;
    private WorldGrid grid;
    private EnemyCatalog catalog;
    private bool initialized = false;
    private float spawnTimer = 0, spawnInterval = 10;
    int minDist = 1, maxDist = 10;
    public override Faction Owner => Faction.Enemy;

    public override void Init(GameContext context)
    {
        gameContext = context;
        enemySystem = context.enemySystem;
        dayNightSystem = context.dayNightSystem;
        spawnTimer = spawnInterval;
        spawner = context.enemySpawner;
        World world = context.world;
        grid = world.Context.grid;
        catalog = context.enemyCatalog;
        initialized = true;
    }

    private void Update()
    {
        if (initialized)
        {
            if (spawnTimer > 0) spawnTimer -= Time.deltaTime;
            else
            {
                //if (dayNightSystem.Phase == DayNightSystem.DayPhase.Night) return;

                spawnTimer = spawnInterval;

                GridTile spawnTile = FindSpawnTile();

                if (spawnTile != null)
                {
                    Enemy enemy = spawner.SpawnEnemy(catalog.testEnemy, spawnTile);
                    enemy.Init(gameContext, this, spawnTile);
                }
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

            Vector2Int spawnPos = new Vector2Int(GridPos.x + x, GridPos.y + y);
            spawnTile = grid.GetTile(spawnPos);

            if (spawnTile != null && spawnTile.IsEmpty) valid = true;
        }

        return spawnTile;
    }
}
