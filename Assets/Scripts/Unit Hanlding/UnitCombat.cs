using System.Data.Common;
using UnityEngine;
using static Pathfinding;

[System.Serializable]
public class UnitCombat
{
    enum CombatState
    {
        None,
        Attacking,
        Defending,
        Chasing,
        Fleeing
    }
    [SerializeField]CombatState state, lastState;

    private Unit unit;

    public float attackDist = 1f, attackDamage = 10f;
    protected float attackInterval = 0.5f, attackTimer = 0;
    public Damageable target;

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
    }

    void SetState(CombatState state)
    {
        lastState = this.state;
        this.state = state;
    }

    public void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    // Updates the current combat state
    public void UpdateState()
    {
        if (target == null)
        {
            // Scan for new targets
            SetState(CombatState.Defending);
        }
        else
        {
            if (InRange())
            {
                SetState(CombatState.Attacking);
            }
            else
            {
                SetState(CombatState.Chasing);
            }
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

    public void Target(Damageable target)
    {
        this.target = target;
        SetState(CombatState.Attacking);
    }

    // Returns the target's grid position
    public Vector2Int TargetPos()
    {
        return target.GridPos();
    }

    // Returns true if we have a target
    public bool HasTarget()
    {
        return target != null;
    }

    // Returns true if in range of the target
    public bool InRange()
    {
        float dist = Vector3.Distance(unit.transform.position, target.transform.position);

        return dist < attackDist;
    }

    #region States
    public bool Attack()
    {
        if (target == null || attackTimer > 0) return false;

        attackTimer = attackInterval;
        if (target.Hit(attackDamage, unit))
        {
            Unit nearbyUnit = unit.ScanForHostile(true);

            if (nearbyUnit == null)
            {
                // Become idle if no nearby valid units
                unit.SetIdle();
            }
            else
            {
                // Target unit if nearby unit was found
                Target(nearbyUnit);
            }
        }

        return true;
    }

    // Stays at position and scans for targets
    void Defend()
    {

    }

    // Move towards target
    void Chase()
    {
        if (!unit.pathRequested && !unit.movement.HasPath())
        {
            unit.RequestPath(TargetPos(), target.transform.position);
        }

        unit.movement.FollowPath();
    }

    // Move away from target
    void Flee()
    {
        
    }
    #endregion
}
