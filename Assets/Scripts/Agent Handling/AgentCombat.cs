using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AgentCombat
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

    private Agent agent;

    public float attackDist = 1f, attackDamage = 10f;
    protected float attackInterval = 0.5f, attackTimer = 0;
    // Potential targets decay over time so units eventually stop caring about distant threats.
    [SerializeField] private GridTile defendingTile = null;

    public void Init(Agent agent)
    {
        this.agent = agent;
    }

    public void Tick()
    {
        if (!agent.IsDead)
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
                agent.RequestPath(defendingTile.position, defendingTile.worldPosition);
            }
        }
    }

    // Updates the current combat state
    public void UpdateState()
    {
        if (agent.IsDead) return;

        // Target choice is based on threat first, then distance/range determines the action.
        if (agent.Targetting.Current == null && agent.Targetting.Candidates.Count == 0)
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
        agent.RequestPath(defendingTile.position, defendingTile.worldPosition);
    }

    //public bool HasTargets => targetCandidates.Count > 0;

    // Returns true if in range of the target
    public bool InRange()
    {
        if (agent.Targetting?.Current == null) return false;

        float dist = Vector3.Distance(agent.transform.position, agent.Targetting.Current.transform.position);

        return dist < attackDist;
    }

    #region Actions
    public bool Attack()
    {
        if (agent.Targetting.Current == null || attackTimer > 0) return false;

        attackTimer = attackInterval;
        if (agent.Targetting.Current.Hit(attackDamage, agent))
        {

        }

        return true;
    }

    // Should return to defending pos
    void Defend()
    {
        if (agent.Targetting.Candidates.Count > 0) // Target new threat if one exists
        {
            SetState(CombatState.Attacking);
        }
        else // Move back to defending tile
        {
            // Move towards defending tile
            agent.movement.FollowPath();
        }
}

    // Move towards target
    void Chase()
    {
       // if ()

        // Request a new path only when there is no active path or pending request.
        if (!agent.pathRequested && (!agent.movement.HasPath || agent.Movement.TargetReached))
        {
            agent.RequestPath(agent.Targetting.TargetPos(), agent.Targetting.Current.transform.position);
        }

        agent.movement.FollowPath();
    }

    // Move away from target
    void Flee()
    {
        
    }
    #endregion
}
