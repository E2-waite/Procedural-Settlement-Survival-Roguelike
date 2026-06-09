using UnityEngine;
using static Enemy;
public class EnemyTargetting : AgentTargetting
{
    private DayNightSystem dayNightSystem;
    private Enemy enemy;
    private MainFireBuilding mainFire;
    bool targetting = false, returning = false;

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
        if (dayNightSystem.Phase == DayNightSystem.DayPhase.Night && !targetting)
        {
            // Target fire
            TargetFire();
        }
        else if (dayNightSystem.Phase == DayNightSystem.DayPhase.Day && !returning)
        {
            ReturnToSpawn();
        }
    }

    private void ReturnToSpawn()
    {
        Debug.Log(enemy.name + " returning to spawn");
        enemy.RequestPath(enemy.SpawnTile);
        enemy.SetState(State.Moving);
        returning = true;
        targetting = false;
    }

    private void TargetFire()
    {
        if (mainFire != null)
        {
            Debug.Log(enemy.name + " targetting fire");

            targetting = true;
            returning = false;
            Target(mainFire);

            enemy.SetState(State.Combat);

            enemy.RequestPath(TargetPos(), mainFire.transform.position);
        }
    }
}
