using System.Collections.Generic;
using UnityEngine;
using static Enemy;
public class EnemyTargetting : AgentTargetting
{
    private DayNightSystem dayNightSystem;
    private Enemy enemy;
    private MainFireBuilding mainFire;
    bool targeting = false, returning = false;

    public void Init(GameContext context, Enemy enemy)
    {
        this.enemy = enemy;
        base.Init(enemy);

        dayNightSystem = context.dayNightSystem;
        //mainFire = context.mainFireBuilding;
    }

    public override void Tick()
    {
        base.Tick();
        SearchForUnits();
        return;
        if (dayNightSystem.Phase == DayNightSystem.DayPhase.Night && !targeting)
        {
            // Target fire
            TargetFire();
        }
        else if (dayNightSystem.Phase == DayNightSystem.DayPhase.Day && !returning)
        {
            ReturnToSpawn();
        }
    }

    void SearchForUnits()
    {
        List<Agent> units = agent.GetNearbyHostile();

        foreach (Agent unit in units)
        {
            AddTarget(unit, 10f);
        }
    }

    private void ReturnToSpawn()
    {
        Debug.Log(enemy.name + " returning to spawn");
        enemy.RequestPath(enemy.SpawnTile);
        enemy.SetState(State.Moving);
        returning = true;
        targeting = false;
    }

    private void TargetFire()
    {
        if (mainFire != null)
        {
            Debug.Log(enemy.name + " targeting fire");

            targeting = true;
            returning = false;
            Target(mainFire);

            enemy.SetState(State.Combat);

            enemy.RequestPath(TargetPos());
        }
    }
}
