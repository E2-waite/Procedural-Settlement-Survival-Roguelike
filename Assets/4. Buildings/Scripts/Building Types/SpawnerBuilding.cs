using System.Collections.Generic;
using UnityEngine;

public class SpawnerBuilding : Building
{
    [SerializeField] private float spawnInterval = 5;
    [SerializeField] protected int max = 1;

    private float spawnTimer = 0;
    private float cleanupTimer = 0, cleanupInterval = 0.1f;
    protected GameContext gameContext;
    int minDist = 1, maxDist = 10;
    public override bool PreBuild => true;
    protected bool initialized = false;
    public GameObject agentPrefab;
    protected WorldGrid grid;
    protected List<Agent> spawnedAgents = new List<Agent>();
    protected bool spawning = true;
    public override void Init(GameContext context)
    {
        spawnTimer = spawnInterval;
        gameContext = context;
        WorldBuilder world = context.world;
        grid = world.Context.grid;
        initialized = true;
    }

    private void Update()
    {
        if (initialized)
        {
            if (spawnTimer > 0) spawnTimer -= Time.deltaTime;
            else if (spawning && spawnedAgents.Count < max)
            {
                spawnTimer = spawnInterval;
                Agent spawnedAgent = Spawn(FindSpawnTile());
                if (spawnedAgent != null)
                    spawnedAgents.Add(spawnedAgent);
            }

            if (cleanupTimer > 0) cleanupTimer -= Time.deltaTime;
            else CleanupAgents();
        }
    }

    public void RemoveAgent(Agent agent)
    {
        spawnedAgents.Remove(agent);
    }

    protected void CleanupAgents()
    {
        cleanupTimer = cleanupInterval;
        for (int i = spawnedAgents.Count - 1; i >= 0; i--)
        {
            if (spawnedAgents[i] == null || spawnedAgents[i].IsDead)
                spawnedAgents.RemoveAt(i);
        }
    }

    protected virtual Agent Spawn(GridTile tile)
    {
        // Base class doesn't spawn anything
        return null;
    }

    protected virtual GridTile FindSpawnTile()
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
