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

    public class TargetCandidate
    {
        public Damageable target;
        public float threat;
    }

    // TODO: add array of threat defs for target types
    [SerializeField] CombatState state, lastState;

    private Unit unit;

    public float attackDist = 1f, attackDamage = 10f;
    protected float attackInterval = 0.5f, attackTimer = 0;
    public Damageable currentTarget;
    // Potential targets decay over time so units eventually stop caring about distant threats.
    List<TargetCandidate> targetCandidates = new List<TargetCandidate>();
    [SerializeField] private GridTile defendingTile = null;

    public void Init(Unit unit)
    {
        this.unit = unit;
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

    // Updates the current combat state
    public void UpdateState()
    {
        if (unit.IsDead) return;

        // Target choice is based on threat first, then distance/range determines the action.
        UpdateThreat();
        CheckTargets();

        if (currentTarget == null && targetCandidates.Count == 0)
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

    public bool HasTargets => targetCandidates.Count > 0;

    // Gets the candidate with the highest threat and targets it
    private void CheckTargets()
    {
        TargetCandidate highestThreat = HighestThreat();

        if (highestThreat != null && highestThreat.target != currentTarget)
        {
            // TODO: have a threshold to ensure it doesn't continuously switch targets when threat is close
            Target(highestThreat.target);
        }
    }

    private void UpdateThreat()
    {
        // Iterate backwards so candidates can be removed while scanning.
        for (int i = targetCandidates.Count - 1; i >= 0; i--)
        {
            TargetCandidate candidate = targetCandidates[i];
            if (candidate == null || candidate.target == null) // Remove null (dead) candidates
            {
                targetCandidates.RemoveAt(i);
                continue;
            }

            //float dist = Vector3.Distance(unit.transform.position, candidate.target.transform.position);
            //dist = Mathf.Clamp(dist - 10, 0, float.MaxValue);

            //// Remove threat if above 10 dist
            //candidate.threat -= dist * Time.deltaTime;

            //// Remove candidate if it has no threat
            //if (candidate.threat <= 0)
            //{
            //    targetCandidates.RemoveAt(i);
            //}
        }
    }

    // Returns the threat candidate that currently has the highest threat
    TargetCandidate HighestThreat()
    {
        float highestVal = 0;
        TargetCandidate highestThreat = null;
        for (int i = targetCandidates.Count - 1; i >= 0; i--)
        {
            TargetCandidate candidate = targetCandidates[i];
            if (candidate.threat > highestVal)
            {
                highestVal = candidate.threat;
                highestThreat = candidate;
            }
        }

        return highestThreat;
    }

    private TargetCandidate GetCandidate(Damageable target)
    {
        // Check if we already have this target
        foreach (TargetCandidate existing in targetCandidates)
        {
            if (existing.target == target)
            {
                return existing;
            }
        }
        return null;
    }

    // Add threat to the threat candidate associated with the target
    public void AddTarget(Damageable target, float threat)
    {
        if (target == null) return;

        TargetCandidate targetCandidate = GetCandidate(target);
        if (targetCandidate == null)
        {
            // If no candidate with target exists, set threat and add to candidates list.
            TargetCandidate newCandidate = new TargetCandidate()
            {
                target = target,
                threat = threat
            };

            targetCandidates.Add(newCandidate);
        }
    }

    public void AddThreat(Damageable target, float threat)
    {
        TargetCandidate targetCandidate = GetCandidate(target);
        if (targetCandidate == null)
        {
            // If no candidate with target exists, set threat and add to candidates list.
            TargetCandidate newCandidate = new TargetCandidate()
            {
                target = target,
                threat = threat
            };

            targetCandidates.Add(newCandidate);
        }
        else
        {
            targetCandidate.threat += threat;
        }
    }

    public void Target(Damageable target)
    {
        this.currentTarget = target;
    }

    // Returns the target's grid position
    public Vector2Int TargetPos()
    {
        return currentTarget.GridPos;
    }

    // Returns true if we have a target
    public bool HasTarget()
    {
        return currentTarget != null;
    }

    // Returns true if in range of the target
    public bool InRange()
    {
        float dist = Vector3.Distance(unit.transform.position, currentTarget.transform.position);

        return dist < attackDist;
    }

    #region Actions
    public bool Attack()
    {
        if (currentTarget == null || attackTimer > 0) return false;

        attackTimer = attackInterval;
        if (currentTarget.Hit(attackDamage, unit))
        {

        }

        return true;
    }

    // Should return to defending pos
    void Defend()
    {
        if (targetCandidates.Count > 0) // Target new threat if one exists
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
            unit.RequestPath(TargetPos(), currentTarget.transform.position);
        }

        unit.movement.FollowPath();
    }

    // Move away from target
    void Flee()
    {
        
    }
    #endregion
}
