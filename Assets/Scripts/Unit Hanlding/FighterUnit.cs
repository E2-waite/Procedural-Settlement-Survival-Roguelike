using System.Collections.Generic;
using UnityEngine;

public class FighterUnit : FollowerUnit
{
    [SerializeField] public UnitCombat combat = new UnitCombat();
    public override UnitCombat Combat => combat;

    protected override void Start()
    {
        base.Start();
        combat.SetUnit(this);
    }

    protected override void Update()
    {
        base.Update();
    }

    void SearchForHostile()
    {
        // Find hostile units
        List<Unit> hostile = GetNearbyHostile();

        // Add nearby hostile targets to target candidates
        Combat.AddTargets(hostile);

        // Set to combat state if targets exist
        if (Combat.HasTargets)
        {
            SetState(State.Combat);
        }
    }

    public void Init(WorkerUnit unit)
    {
        health = unit.Health;
    }

    // Called when unit reaches target tile when in move state
    protected override void TargetTileReached()
    {
        // Starts defending if reached target position
        SetState(State.Combat);
        Combat.SetState(UnitCombat.CombatState.Defending);
    }

    protected override void CombatState()
    {
        base.CombatState();

        // Search for hostile
        SearchForHostile();
    }

    #region Commanding

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

    #endregion

    #region Detecting Units
    // Gets nearby follower units for targetting
    public override List<Unit> GetNearbyHostile()
    {
        if (chunk != null)
        {
            return chunk.GetEnemies();
        }

        return null;
    }

    #endregion
}
