using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

    public class ThreatCandidate
    {
        public Damageable target;
        public float threat;
    }

    [SerializeField]CombatState state, lastState;

    private Unit unit;

    public float attackDist = 1f, attackDamage = 10f;
    protected float attackInterval = 0.5f, attackTimer = 0;
    public Damageable currentTarget;
    List<ThreatCandidate> threatCandidates = new List<ThreatCandidate>(); // List of potential targets to focus on

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

        if (currentTarget == null && threatCandidates.Count == 0)
        {
            // Scan for new targets
            SetState(CombatState.Defending);
        }
        else 
        {
            CheckThreat();

            if (currentTarget == null)
            {
                SetState(CombatState.None);
                unit.SetIdle();
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

    // Gets the candidate with the highest threat and targets it
    private void CheckThreat()
    {
        ThreatCandidate highestThreat = HighestThreat();

        if (highestThreat != null && highestThreat.target != currentTarget)
        {
            // TODO: have a threshold to ensure it doesn't continuously switch targets when threat is close
            Target(highestThreat.target);
        }
    }

    // Returns the threat candidate that currently has the highest threat
    ThreatCandidate HighestThreat()
    {
        float highestVal = 0;
        ThreatCandidate highestThreat = null;
        for (int i = threatCandidates.Count - 1; i >= 0; i--)
        {
            ThreatCandidate candidate = threatCandidates[i];
            if (candidate == null || candidate.target == null) // Remove dead candidates
            {
                threatCandidates.RemoveAt(i);
                continue;
            }

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

        for (int i = threatCandidates.Count - 1; i >= 0; i--)
        {
            ThreatCandidate candidate = threatCandidates[i];
            if (candidate != null && candidate.target == target)
            {
                // If candidate with target exists, add threat and return
                candidate.threat += threat;
                return;
            }
        }


        // If no candidate with target exists, set thread and add to candidates list;
        ThreatCandidate newCandidate = new ThreatCandidate()
        {
            target = target,
            threat = threat
        };

        threatCandidates.Add(newCandidate);
    }

    public void Target(Damageable target)
    {
        this.currentTarget = target;
    }

    // Returns the target's grid position
    public Vector2Int TargetPos()
    {
        return currentTarget.GridPos();
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

    #region States
    public bool Attack()
    {
        if (currentTarget == null || attackTimer > 0) return false;

        attackTimer = attackInterval;
        if (currentTarget.Hit(attackDamage, unit))
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
                unit.StartCombat();
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
