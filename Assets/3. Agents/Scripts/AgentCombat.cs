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
    private AgentTargetting targeting;

    public float attackDist = 1f, chaseDist = 10f, attackDamage = 10f;
    protected float attackInterval = 1.5f, attackTimer = 0;
    // Potential targets decay over time so units eventually stop caring about distant threats.
    bool initialized = false;
    Vector3 formationPos = Vector3.zero;
    CombatType type;


    public void Init(Agent agent, AgentTargetting targeting, CombatType type)
    {
        this.agent = agent;
        this.targeting = targeting;
        this.type = type;
        initialized = true;
        if (type == CombatType.Melee)
            attackDist = 1f;
        else
            attackDist = 10f;
    }

    public void Tick()
    {
        if (!initialized || agent.IsDead) return;

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    public void SetState(CombatState state)
    {
        if (state != this.state)
        {
            lastState = this.state;
            this.state = state;

            // If defending, path to the defending tile
            if (state == CombatState.Defending && formationPos != Vector3.zero)
            {
                agent?.RequestPath(formationPos);
            }
        }
    }

    // Updates the current combat state
    public void UpdateState()
    {
        if (agent.IsDead) return;

        if (targeting.Current == null && targeting.Candidates.Count == 0)
        {
            // Defend if defending tile is available else set to none
            if (formationPos == Vector3.zero)
                SetState(CombatState.None);
            else
                SetState(CombatState.Defending);
        }
        else if (InAttackRange())
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
        if (agent.IsDead) return;

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
    public void SetFormationPos(Vector3 pos)
    {
        formationPos = pos;
    }

    // Returns true if in attack range of the target
    public bool InAttackRange()
    {
        if (targeting?.Current == null) return false;
        float dist = Vector3.Distance(agent.WorldPos, targeting.Current.WorldPos);
        return dist < attackDist;
    }

    // Returns true if in chase range of the target
    public bool InChaseRange()
    {
        if (targeting?.Current == null) return false;
        float dist = Vector3.Distance(agent.WorldPos, targeting.Current.WorldPos);
        return dist < chaseDist;
    }

    #region Actions
    // Attack (hit) the current target
    public bool Attack()
    {
        if (targeting.Current == null || attackTimer > 0) return false;

        attackTimer = attackInterval;

        if (type == CombatType.Melee)
        {
            Vector3 hitDir = targeting.Current.WorldPos - agent.WorldPos;
            if (targeting.Current.OnHit(agent, attackDamage, hitDir))
            {

            }
        }
        else if (type == CombatType.Ranged)
        {
            agent.LaunchProjectile(targeting.Current, attackDamage);
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
        if (!targeting.HasTarget || attackTimer > 0) return;

        agent.movement.SetTargetPos(targeting.TargetWorldPos());

        // Request a new path only when there is no active path or pending request.
        if (agent != null && !agent.pathRequested && (agent.movement.TargetChanged || !agent.movement.HasPath))
        {
            agent.RequestPath(targeting.TargetPos(), targeting.Current is Building);
        }

        agent?.movement.FollowPath();
    }

    // Move away from target
    void Flee()
    {
        
    }
    #endregion
}
