using System.Collections.Generic;
using UnityEngine;

public class Targetting
{
    List<TargetCandidate> targetCandidates = new List<TargetCandidate>();
    public List<TargetCandidate> Candidates => targetCandidates;

    public Damageable currentTarget;
    public Damageable Current => currentTarget;

    private Unit unit;

    // Returns true if we have a target
    public bool HasTarget() => currentTarget != null;


    public void Init(Unit unit)
    {
        this.unit = unit;
    }

    public virtual void Tick()
    {
        if (unit.IsDead) return;

        CheckTargets();
    }

    public void Target(Damageable target)
    {
        currentTarget = target;
    }

    // Returns the target's grid position
    public Vector2Int TargetPos()
    {
        return currentTarget != null ? currentTarget.GridPos : Vector2Int.zero;
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

            unit.Targetting.Candidates.Add(newCandidate);
        }
    }

    public TargetCandidate GetCandidate(Damageable target)
    {
        // Check if we already have this target
        foreach (TargetCandidate existing in unit.Targetting.Candidates)
        {
            if (existing.target == target)
            {
                return existing;
            }
        }
        return null;
    }

    // Gets the candidate with the highest threat and targets it
    private void CheckTargets()
    {
        UpdateThreat();

        TargetCandidate highestThreat = HighestThreat();

        if (highestThreat != null && highestThreat.target != unit.Targetting.Current)
        {
            // TODO: have a threshold to ensure it doesn't continuously switch targets when threat is close
            unit.Targetting.Target(highestThreat.target);
        }
    }

    private void UpdateThreat()
    {
        // Iterate backwards so candidates can be removed while scanning.
        for (int i = unit.Targetting.Candidates.Count - 1; i >= 0; i--)
        {
            TargetCandidate candidate = unit.Targetting.Candidates[i];
            if (candidate == null || candidate.target == null) // Remove null (dead) candidates
            {
                unit.Targetting.Candidates.RemoveAt(i);
                continue;
            }
        }
    }

    // Returns the threat candidate that currently has the highest threat
    TargetCandidate HighestThreat()
    {
        float highestVal = 0;
        TargetCandidate highestThreat = null;
        for (int i = unit.Targetting.Candidates.Count - 1; i >= 0; i--)
        {
            TargetCandidate candidate = unit.Targetting.Candidates[i];
            if (candidate.threat > highestVal)
            {
                highestVal = candidate.threat;
                highestThreat = candidate;
            }
        }

        return highestThreat;
    }

    public void AddThreat(Damageable target, float threat)
    {
        TargetCandidate targetCandidate = unit.Targetting.GetCandidate(target);
        if (targetCandidate == null)
        {
            // If no candidate with target exists, set threat and add to candidates list.
            TargetCandidate newCandidate = new TargetCandidate()
            {
                target = target,
                threat = threat
            };

            unit.Targetting.Candidates.Add(newCandidate);
        }
        else
        {
            targetCandidate.threat += threat;
        }
    }

    
}
