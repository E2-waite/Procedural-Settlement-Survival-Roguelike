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

    public class ThreatCandidate
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
    List<ThreatCandidate> targetCandidates = new List<ThreatCandidate>();

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
        }
    }

    public void Update()
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

    public bool HasTargets => targetCandidates.Count > 0;

    // Gets the candidate with the highest threat and targets it
    private void CheckTargets()
    {
        ThreatCandidate highestThreat = HighestThreat();

        if (highestThreat != null && highestThreat.target != currentTarget)
        {
            // TODO: have a threshold to ensure it doesn't continuously switch targets when threat is close
            Target(highestThreat.target);
        }
        else if (highestThreat == null && currentTarget == null)
        {
            // No target
            SetState(CombatState.None);
        }
    }

    public void AddTargets(List<Unit> newTarget)
    {
        if (newTarget == null) return;

        // Detection can run repeatedly, so avoid adding duplicate candidates.
        foreach (Unit targetUnit in newTarget)
        {
            if (targetUnit == null) continue;

            bool exists = false;

            // Check if we already have this target
            foreach (ThreatCandidate existing in targetCandidates)
            {
                if (existing.target == targetUnit)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                // Adds target to candidates list if it doesn't exist
                ThreatCandidate newCandidate = new ThreatCandidate()
                {
                    target = targetUnit,
                    threat = 100
                };

                targetCandidates.Add(newCandidate);
            }
        }
    }

    private void UpdateThreat()
    {
        // Iterate backwards so candidates can be removed while scanning.
        for (int i = targetCandidates.Count - 1; i >= 0; i--)
        {
            ThreatCandidate candidate = targetCandidates[i];
            if (candidate == null || candidate.target == null) // Remove null (dead) candidates
            {
                targetCandidates.RemoveAt(i);
                continue;
            }

            float dist = Vector3.Distance(unit.transform.position, candidate.target.transform.position);
            dist = Mathf.Clamp(dist - 10, 0, float.MaxValue);

            // Remove threat if above 10 dist
            candidate.threat -= dist * Time.deltaTime;

            // Remove candidate if it has no threat
            if (candidate.threat <= 0)
            {
                targetCandidates.RemoveAt(i);
            }
        }
    }

    // Returns the threat candidate that currently has the highest threat
    ThreatCandidate HighestThreat()
    {
        float highestVal = 0;
        ThreatCandidate highestThreat = null;
        for (int i = targetCandidates.Count - 1; i >= 0; i--)
        {
            ThreatCandidate candidate = targetCandidates[i];
            if (candidate.threat > highestVal)
            {
                highestVal = candidate.threat;
                highestThreat = candidate;
            }
        }

        return highestThreat;
    }

    // Add threat to the threat candidate associated with the target
    public void AddThreat(Damageable target, float threat)
    {
        if (target == null) return;

        for (int i = targetCandidates.Count - 1; i >= 0; i--)
        {
            ThreatCandidate candidate = targetCandidates[i];
            if (candidate != null && candidate.target == target)
            {
                // If candidate with target exists, add threat and return
                candidate.threat += threat;
                return;
            }
        }


        // If no candidate with target exists, set threat and add to candidates list.
        ThreatCandidate newCandidate = new ThreatCandidate()
        {
            target = target,
            threat = threat
        };

        targetCandidates.Add(newCandidate);
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
        
    }

    // Move towards target
    void Chase()
    {
        // Request a new path only when there is no active path or pending request.
        if (!unit.pathRequested && unit.Movement.TargetReached)
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
