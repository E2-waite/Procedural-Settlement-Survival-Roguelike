using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitCombat
{
    public enum CombatState
    {
        None,
        Attacking,
        Defending,
        Chasing,
        Fleeing
    }

    [SerializeField] CombatState state, lastState;

    private Unit unit;

    public float attackDist = 1f, attackDamage = 10f;
    protected float attackInterval = 0.5f, attackTimer = 0;
    // Potential targets decay over time so units eventually stop caring about distant threats.
    [SerializeField] private GridTile defendingTile = null;

    public void Init(Unit unit)
    {
        this.unit = unit;
    }

    public void Tick()
    {
        if (!unit.IsDead)
        {
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
        }
    }

    public void SetState(CombatState state)
    {
        if (state != this.state)
        {
            lastState = this.state;
            this.state = state;

            if (state == CombatState.Defending && defendingTile != null)
            {
                unit.RequestPath(defendingTile.position, defendingTile.worldPosition);
            }
        }
    }

    // Updates the current combat state
    public void UpdateState()
    {
        if (unit.IsDead) return;

        // Target choice is based on threat first, then distance/range determines the action.
        if (unit.Targetting.Current == null && unit.Targetting.Candidates.Count == 0)
        {
            SetState(CombatState.Defending);
        }
        else if (InRange())
        {
            SetState(CombatState.Attacking);
        }
        else
        {
            SetState(CombatState.Chasing);
        }
    }

    // Executes the current combat state
    public void ExecuteState()
    {
        switch (state)
        {
            case CombatState.Attacking:
                Attack(); break;

            case CombatState.Defending:
                Defend(); break;

            case CombatState.Chasing:
                Chase(); break;

            case CombatState.Fleeing:
                Flee(); break;
        }
    }

    public void DefendTile(GridTile tile)
    {
        defendingTile = tile;
        SetState(CombatState.Defending);
        unit.RequestPath(defendingTile.position, defendingTile.worldPosition);
    }

    //public bool HasTargets => targetCandidates.Count > 0;

    // Returns true if in range of the target
    public bool InRange()
    {
        float dist = Vector3.Distance(unit.transform.position, unit.Targetting.Current.transform.position);

        return dist < attackDist;
    }

    #region Actions
    public bool Attack()
    {
        if (unit.Targetting.Current == null || attackTimer > 0) return false;

        attackTimer = attackInterval;
        if (unit.Targetting.Current.Hit(attackDamage, unit))
        {

        }

        return true;
    }

    // Should return to defending pos
    void Defend()
    {
        if (unit.Targetting.Candidates.Count > 0) // Target new threat if one exists
        {
            SetState(CombatState.Attacking);
        }
        else // Move back to defending tile
        {
            // Move towards defending tile
            unit.movement.FollowPath();
        }
}

    // Move towards target
    void Chase()
    {
       // if ()

        // Request a new path only when there is no active path or pending request.
        if (!unit.pathRequested && (!unit.movement.HasPath || unit.Movement.TargetReached))
        {
            unit.RequestPath(unit.Targetting.TargetPos(), unit.Targetting.Current.transform.position);
        }

        unit.movement.FollowPath();
    }

    // Move away from target
    void Flee()
    {
        
    }
    #endregion
}
