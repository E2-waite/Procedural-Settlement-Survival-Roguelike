using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
    Player player = null;
    private List<Agent> unitAgents = new List<Agent>();
    private List<Agent> enemyAgents = new List<Agent>();
    private List<ChunkData> neighbours = new List<ChunkData>();

    public void AddNeighbour(ChunkData neighbour)
    {
        if (!neighbours.Contains(neighbour))
        {
            neighbours.Add(neighbour);
        }
    }

    public void AddAgent(Agent agent)
    {
        if (agent is Unit)
        {
            Unit follower = (Unit)agent;
            if (!unitAgents.Contains(follower))
                unitAgents.Add(follower);
        }
        else if (agent is Enemy)
        {
            Enemy enemy = (Enemy)agent;
            if (!enemyAgents.Contains(enemy))
                enemyAgents.Add(enemy);
        }
    }

    public void RemoveAgent(Agent agent)
    {
        if (agent is Unit)
        {
            Unit follower = (Unit)agent;
            if (unitAgents.Contains(follower))
                unitAgents.Remove(follower);
        }
        else if (agent is Enemy)
        {
            Enemy enemy = (Enemy)agent;
            if (enemyAgents.Contains(enemy))
                enemyAgents.Remove(enemy);
        }
    }

    public void AddPlayer(Player player)
    {
        this.player = player;
    }

    public void RemovePlayer()
    {
        player = null;
    }

    public List<Agent> GetEnemies(bool includeSurrounding = true)
    {
        if (!includeSurrounding) return enemyAgents;

        // Include neighbouring chunks so units near chunk edges can still detect each other.
        List<Agent> enemyList = new List<Agent>(enemyAgents);

        if (includeSurrounding)
        {
            foreach (ChunkData neighbour in neighbours)
            {
                enemyList.AddRange(neighbour.GetEnemies(false));
            }
        }

        return enemyList;
    }

    public List<Agent> GetUnits(bool includeSurrounding = true)
    {
        if (!includeSurrounding) return unitAgents;

        List<Agent> followerList = new List<Agent>(unitAgents);

        if (includeSurrounding)
        {
            foreach (ChunkData neighbour in neighbours)
            {
                followerList.AddRange(neighbour.GetUnits(false));
            }
        }

        return followerList;
    }
}
