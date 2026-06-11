using System.Collections.Generic;
using UnityEngine;
using static GlobalDefs;

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
    private AgentTargetting targetting;

    public float attackDist = 1f, attackDamage = 10f;
    protected float attackInterval = 0.5f, attackTimer = 0;
    // Potential targets decay over time so units eventually stop caring about distant threats.
    [SerializeField] private GridTile defendingTile = null;
    bool initialized = false;
    CombatType type;

    public void Init(Agent agent, AgentTargetting targetting, CombatType type)
    {
        this.agent = agent;
        this.targetting = targetting;
        this.type = type;
        initialized = true;
        if (type == CombatType.Melee)
            attackDist = 1f;
        else
            attackDist = 5f;
    }

    public void Tick()
    {
        if (!initialized || agent.IsDead) return;

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        UpdateState();
        ExecuteState();
    }

    public void SetState(CombatState state)
    {
        if (state != this.state)
        {
            lastState = this.state;
            this.state = state;

            // If defending, path to the defending tile
            if (state == CombatState.Defending && defendingTile != null)
            {
                agent?.RequestPath(defendingTile.position, defendingTile.worldPosition);
            }
        }
    }

    // Updates the current combat state
    public void UpdateState()
    {
        if (agent.IsDead) return;

        if (targetting.Current == null && targetting.Candidates.Count == 0)
        {
            // Defend if defending tile is available else set to none
            if (defendingTile == null)
                SetState(CombatState.None);
            else
                SetState(CombatState.Defending);
        }
        else if (InRange())
        {
            // Start attacking if we have targets and in range
            SetState(CombatState.Attacking);
        }
        else
        {
            // Start chasing if we have targets but not in range
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

    // Start defending a tile and requests path
    public void DefendTile(GridTile tile)
    {
        defendingTile = tile;
    }

    // Returns true if in range of the target
    public bool InRange()
    {
        if (targetting?.Current == null) return false;
        float dist = Vector3.Distance(agent.transform.position, targetting.Current.transform.position);
        return dist < attackDist;
    }

    #region Actions
    // Attack (hit) the current target
    public bool Attack()
    {
        if (targetting.Current == null || attackTimer > 0) return false;

        attackTimer = attackInterval;

        if (type == CombatType.Melee)
        {
            Vector3 hitDir = targetting.Current.transform.position - agent.transform.position;
            if (targetting.Current.Hit(agent, attackDamage, hitDir))
            {

            }
        }
        else if (type == CombatType.Ranged)
        {
            agent.LaunchProjectile(targetting.Current, attackDamage);
        }


        return true;
    }

    // Returns to defending tile
    void Defend()
    {
        agent?.movement.FollowPath();
    }

    // Move towards target
    void Chase()
    {
        if (!targetting.HasTarget) return;

        // Request a new path only when there is no active path or pending request.
        if (agent != null && !agent.pathRequested && (!agent.movement.HasPath || agent.Movement.TargetReached))
        {
            agent.RequestPath(targetting.TargetPos(), targetting.Current.transform.position);
        }

        agent?.movement.FollowPath();
    }

    // Move away from target
    void Flee()
    {
        
    }
    #endregion
}
