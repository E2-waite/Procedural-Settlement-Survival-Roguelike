using System.Collections.Generic;
using UnityEngine;
using static FighterUnit;
using static UnityEditorInternal.VersionControl.ListControl;
using static WorkerUnit;

public class EnemyUnit : Unit
{
    public enum EnemyState
    {
        Idle,
        Moving,
        Following,
        Fighting
    }

    public EnemyState state;
    EnemyState lastState;

    protected override void Start()
    {
        base.Start();
        state = EnemyState.Idle;
        lastState = EnemyState.Idle;

        EnemyHandler.Instance.AddEnemy(this);
        combatUnit = true;
    }

    protected override void Die()
    {
        EnemyHandler.Instance.RemoveEnemy(this);
        base.Die();
    }

    protected override int GetState()
    {
        return (int)state;
    }

    protected override void SetState(int newState)
    {
        lastState = state;
        state = (EnemyState)newState;
    }

    void SetState(EnemyState newState)
    {
        lastState = state;
        state = newState;
    }

    public override bool Hit(float damage, Unit source)
    {
        if (base.Hit(damage, source)) return true;

        if (source is FighterUnit)
        {
            // Targets the unit that hit this enemy
            TargetUnit(source);
        }

        return false;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override bool HandleStates()
    {
        bool handled = base.HandleStates();
        if (handled) return true;

        return false;
    }

    protected override List<Unit> GetNearbyUnits()
    {
        if (chunk != null)
        {
            return chunk.GetFollowers();
        }

        return null;
    }
}
