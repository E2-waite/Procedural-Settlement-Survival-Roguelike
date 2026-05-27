using System.Collections.Generic;
using UnityEngine;

public class FighterUnit : FollowerUnit
{
    // YOU MUST ENSURE CONSISTENCY 0 = idle, 1 = moving, 2 = following
    public enum FighterState
    {
        Idle,
        Moving,
        Following,
        Fighting
    }

    public FighterState state;
    FighterState lastState;

    protected override void Start()
    {
        base.Start();
        state = FighterState.Idle;
        lastState = FighterState.Idle;
        combatUnit = true;
    }

    protected override int GetState()
    {
        return (int)state;
    }

    protected override void SetState(int newState)
    {
        lastState = state;
        state = (FighterState)newState;
    }

    void SetState(FighterState newState)
    {
        lastState = state;
        state = newState;
    }

    public override bool Hit(float damage, Unit source)
    {
        if (base.Hit(damage, source)) return true;

        if (source is EnemyUnit)
        {
            // Update target?
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

    // Command to interact with tile
    public override void Command(GridTile tile)
    {
        if (tile == null) return;
        bool handled = false;

        if (!handled)
            base.Command(tile);
    }

    // Command to interact with unit
    public override void Command(Unit unit)
    {
        if (unit == null || unit == this) return;

        if (unit is EnemyUnit)
        {
            TargetUnit(unit);
        }
    }

    protected override List<Unit> GetNearbyUnits()
    {
        if (chunk != null)
        {
            return chunk.GetEnemies();
        }

        return null;
    }
}
