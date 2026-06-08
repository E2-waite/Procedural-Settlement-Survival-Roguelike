using UnityEngine;
using static Unit;
public class EnemyTargetting : Targetting
{
    private DayNightSystem dayNightSystem;
    private EnemyUnit unit;
    private MainFireBuilding mainFire;
    bool targetting = false, returning = false;

    public void Init(GameContext context, EnemyUnit unit)
    {
        this.unit = unit;
        dayNightSystem = context.dayNightSystem;
        mainFire = context.mainFireBuilding;
    }

    public override void Tick()
    {
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
        Debug.Log(unit.name + " returning to spawn");
        unit.RequestPath(unit.SpawnTile);
        unit.SetState(State.Moving);
        returning = true;
        targetting = false;
    }

    private void TargetFire()
    {
        if (mainFire != null)
        {
            Debug.Log(unit.name + " targetting fire");

            targetting = true;
            returning = false;
            Target(mainFire);

            unit.SetState(State.Combat);

            unit.RequestPath(TargetPos(), mainFire.transform.position);
        }
    }
}
