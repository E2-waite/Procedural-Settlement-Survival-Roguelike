using System.Collections.Generic;
using UnityEngine;

public class FighterUnit : Unit
{
    public override AgentCombat Combat => combat;
    public float searchDist = 10f; // Only target enemies within this distance when scanning
    private FighterTargetting targetting = new FighterTargetting();
    public override AgentTargetting Targetting => targetting;

    void SearchForEnemies()
    {
        // Find hostile units
        List<Agent> enemies = GetNearbyHostile();

        foreach (Agent enemy in enemies)
        {
            Targetting.AddTarget(enemy, 10f);
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
        Combat.SetState(AgentCombat.CombatState.Defending);
    }

    protected override void CombatState()
    {
        base.CombatState();

        // Search for hostile
        SearchForEnemies();
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
    public virtual void Command(Enemy enemy)
    {
        if (enemy == null || enemy == this) return;

        if (enemy is Enemy)
        {
            targetting.Target(enemy);
        }
    }

    #endregion

    #region Detecting Units
    // Gets nearby follower units for targetting
    public override List<Agent> GetNearbyHostile()
    {
        if (chunk != null)
        {
            return chunk.GetEnemies();
        }

        return null;
    }

    #endregion
}
