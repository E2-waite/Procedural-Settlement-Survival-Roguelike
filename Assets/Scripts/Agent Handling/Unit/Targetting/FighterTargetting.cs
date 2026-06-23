using System.Collections.Generic;
using UnityEngine;

public class FighterTargetting : AgentTargetting
{
    public override void Tick()
    {
        base.Tick();
        SearchForEnemies();
    }

    void SearchForEnemies()
    {
        List<Agent> enemies = agent.GetNearbyHostile();

        foreach (Agent enemy in enemies)
        {
            AddTarget(enemy, 0f);
        }
    }
}
