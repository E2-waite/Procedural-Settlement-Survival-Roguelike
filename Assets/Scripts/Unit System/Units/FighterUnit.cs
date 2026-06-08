using System.Collections.Generic;
using UnityEngine;

public class FighterUnit : FollowerUnit
{
    [SerializeField] public UnitCombat combat = new UnitCombat();
    public override UnitCombat Combat => combat;
    public float searchDist = 10f; // Only target enemies within this distance when scanning
    private FighterTargetting targetting = new FighterTargetting();
    public override Targetting Targetting => (Targetting)targetting;

    void SearchForHostile()
    {
        // Find hostile units
        List<Unit> hostile = GetNearbyHostile();

        foreach (Unit hostileUnit in hostile)
        {
            Targetting.AddTarget(hostileUnit, 10f);
        }

        // Add nearby hostile targets to target candidates
        //Combat.AddTargets(hostile);
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

        bool handled = true;
        SetState(State.Combat);
        Combat.DefendTile(tile);

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
